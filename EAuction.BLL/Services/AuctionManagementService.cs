﻿using EAuction.BLL.ViewModels;
using EAuction.Core.DataModels;
using EAuction.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EAuction.BLL.Services
{
    public class AuctionManagementService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public void OpenAuction(CreateAuctionViewModel model, Guid organizationId)
        {
            if (model == null)
                throw new Exception($"{typeof(CreateAuctionViewModel).Name} is null");

            int maximumAllowedActiveAuctions = 3;

            var auctionsCheck = _applicationDbContext
                .Organizations
                .Find(organizationId)
                .Auctions
                .Where(p => p.Status == AuctionStatus.Active)
                .Count() < maximumAllowedActiveAuctions;

            var categoryCheck = _applicationDbContext.AuctionTypes
                .SingleOrDefault(p => p.Name == model.AuctionType).Id;

            if (categoryCheck == null)
                throw new Exception("Ошибка валидации модели!");

            if (!auctionsCheck)
                throw new Exception("Превышено максимальное количество активных аукционов!");


            Auction auction = new Auction()
            {
                Id = Guid.NewGuid(),
                Description = model.Description,
                ShippingAddress = model.ShippingAddress,
                ShippingConditions = model.ShippingConditions,
                StartPrice = model.StartPrice,
                PriceStep = model.PriceStep,
                MinPrice = model.MinPrice,
                StartDate = model.StartDate,
                FinishDate = model.FinishDate,
                Status = AuctionStatus.Active,
                AuctionTypeId = categoryCheck,
                OrganizationId = organizationId
            };
            _applicationDbContext.Auctions.Add(auction);
            _applicationDbContext.SaveChanges();


            //загружаем файлы аукциона в бд
            List<AuctionFile> auctionFiles = new List<AuctionFile>();

            if (model.UploadFiles.Count != 0)
            {
                foreach (HttpPostedFileBase i in model.UploadFiles)
                {
                    AuctionFile file = new AuctionFile();
                    byte[] fileData = null;

                    // считываем файл в массив байтов
                    using (var binaryReader = new BinaryReader(i.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(i.ContentLength);
                    }

                    // установка массива байтов
                    file.FileName = i.FileName;
                    file.Extension = i.ContentType;
                    file.Content = fileData;
                    file.CreatedAt = DateTime.Now;
                    _applicationDbContext.AuctionFiles.Add(file);
                }
                _applicationDbContext.SaveChanges();
            }
        }

        public void MakeBidToAuction(MakeBidViewModel model)
        {
            var bidExists = _applicationDbContext.Bids
                .Any(p => p.Price == model.Price &&
                p.AuctionId == model.AuctionId &&
                p.Description == model.Description &&
                p.OrganizationId == model.OrganizationId);

            if (bidExists)
                throw new Exception("Такая ставка уже существует");

            var inValidPriceRange = _applicationDbContext
                .Auctions.Where(p => p.Id == model.AuctionId &&
                p.MinPrice < model.Price &&
                p.StartPrice > model.Price);

            var inStepRange = inValidPriceRange
                .Any(p => (p.StartPrice - model.Price) % p.PriceStep == 0);

            if (!inStepRange)
                throw new Exception("Invalid bid according price step");

            var activeBidStatus = _applicationDbContext.BidStatuses.SingleOrDefault(p => p.StatusName == "Active").Id;
            if (activeBidStatus == null)
                throw new Exception("Ошибка таблицы статусов ставок");

            Bid bid = new Bid()
            {
                Id=Guid.NewGuid(),
                Price = model.Price,
                Description = model.Description,
                AuctionId = model.AuctionId,
                OrganizationId = model.OrganizationId,
                CreatedDate = DateTime.Now,
                BidStatusId=activeBidStatus
            };

            _applicationDbContext.Bids.Add(bid);
            _applicationDbContext.SaveChanges();
        }

        public void RevokeBidFromAuction(Guid bidId)
        {
            var bidExists = _applicationDbContext.Bids.SingleOrDefault(p => p.Id == bidId);
            if (bidExists==null)
                throw new Exception("Такой ставки не существует!");

            var revokeBidStatus = _applicationDbContext.BidStatuses.SingleOrDefault(p => p.StatusName == "Revoke").Id;
            if (revokeBidStatus == null)
                throw new Exception("Ошибка таблицы статусов ставок");

            bidExists.BidStatusId = revokeBidStatus;
            _applicationDbContext.SaveChanges();
        }

        public AuctionInfoViewModel GetAuctionInfo(Guid auctionId)
        {            
            var auction = _applicationDbContext.Auctions.Include("Organizations").SingleOrDefault(p => p.Id == auctionId);
            if (auction == null)
                throw new Exception($"Аукциона с таким id {auctionId} не существует");

            var auctionFiles = _applicationDbContext.AuctionFiles.Where(p => p.AuctionId == auctionId).ToList();
            //if (auctionFiles.Count == 0)
            //    throw new Exception($"У аукциона {auctionId} нет прикрепленных документов");

            var organization = auction.Organization.FullName;
            AuctionInfoViewModel model = new AuctionInfoViewModel()
            {
                AuctionId = auctionId.ToString(),
                Status = auction.Status.ToString(),
                AuctionType = auction.AuctionType.Name,
                OrganizationName = organization,
                ShippingAddress = auction.ShippingAddress,
                ShippingConditions = auction.ShippingConditions,
                StartPrice = auction.StartPrice,
                PriceStep = auction.PriceStep,
                MinPrice = auction.MinPrice,
                StartDate = auction.StartDate,
                FinishDate = auction.FinishDate,
                FinishDateAtActual = auction.FinishDateAtActual,
                AuctionFiles = auctionFiles
            };

            return model;
        }       

        public void ElectWinnerInAuction(Guid auctionId, Guid organizationId) //рейтинг, сумма на счете?
        {
            var auction = _applicationDbContext.Auctions.SingleOrDefault(p => p.Id == auctionId);
            if (auction == null)
                throw new Exception($"Аукциона с id {auctionId} не существует");

            var organization = _applicationDbContext.Organizations.SingleOrDefault(p => p.Id == organizationId);
            if (organization == null)
                throw new Exception($"Организации с id {organizationId} не существует");

            var IsAuctionCreator = _applicationDbContext.Auctions.Any(p => p.Id == auctionId && p.OrganizationId == organizationId);
            if (IsAuctionCreator)
                throw new Exception($"Организация-создатель аукциона {auctionId} не может быть победителем данного аукциона");

            AuctionWin win = new AuctionWin()
            {
                Id = Guid.NewGuid(),
                SetupDate = DateTime.Now,
                AuctionId = auctionId,
                OrganizationId = organizationId
            };
            _applicationDbContext.AuctionWins.Add(win);
            _applicationDbContext.SaveChanges();
        }

        public List<BidInfoViewModel> ShowAllBidsForAuction(Guid auctionId)
        {
            var auction = _applicationDbContext.Auctions.Include("Organizations").Include("Bids").SingleOrDefault(p => p.Id == auctionId);
            if (auction == null)
                throw new Exception($"Аукциона с таким id {auctionId} не существует");

            var activeBidStatus = _applicationDbContext.BidStatuses.SingleOrDefault(p => p.StatusName == "Active").Id;
            if (activeBidStatus == null)
                throw new Exception("Ошибка таблицы статусов ставок");

            var bidForAuction = auction.Bids.Where(p=>p.BidStatusId== activeBidStatus).ToList();
            if (bidForAuction.Count == 0)
                throw new Exception($"По аукциону {auctionId} нет активных ставок");

            List<BidInfoViewModel> bids = new List<BidInfoViewModel>();
            foreach (Bid item in bidForAuction)
            {
                BidInfoViewModel bid = new BidInfoViewModel()
                {
                    BidId = item.Id.ToString(),
                    AuctionId = auctionId.ToString(),
                    AuctionType = item.Auction.AuctionType.ToString(),
                    AuctionDescription = item.Auction.Description,
                    BidStatus = item.BidStatus.StatusName,
                    OrganizationId = item.Organization.Id.ToString(),
                    OrganizationName = item.Organization.FullName,
                    Price = item.Price,
                    CreatedDate = item.CreatedDate,
                    BidDescription = item.Description
                };
                bids.Add(bid);
            }
            return bids;
        }

        public List<BidInfoViewModel> ShowBidsForAuctionWithFitOrganizationRating(Guid auctionId)
        {
            var auction = _applicationDbContext.Auctions.Include("Organizations").Include("Bids").SingleOrDefault(p => p.Id == auctionId);
            if (auction == null)
                throw new Exception($"Аукциона с таким id {auctionId} не существует");

            var activeBidStatus = _applicationDbContext.BidStatuses.SingleOrDefault(p => p.StatusName == "Active").Id;
            if (activeBidStatus == null)
                throw new Exception("Ошибка таблицы статусов ставок");

            var allBids = auction.Bids.Where(p => p.BidStatusId == activeBidStatus).ToList();
            if (allBids.Count == 0)
                throw new Exception($"По аукциону {auctionId} нет активных ставок");

            List<Bid> bidsWithFitRating = new List<Bid>();
            foreach (Bid item in allBids)
            {
                var org = item.OrganizationId;
                var avg = _applicationDbContext.OrganizationRatings.Where(p => p.OrganizationId == org).Average(p => p.Score);
                if (avg >= auction.MinRatingForParticipant)
                    bidsWithFitRating.Add(item);
            }
            if (bidsWithFitRating.Count == 0)
                throw new Exception($"По аукциону {auctionId} нет ставок от организаций с нужным рейтингом");

            List<BidInfoViewModel> bids = new List<BidInfoViewModel>();
            foreach (Bid item in bidsWithFitRating)
            {
                BidInfoViewModel bid = new BidInfoViewModel()
                {
                    BidId = item.Id.ToString(),
                    AuctionId = auctionId.ToString(),
                    AuctionType = item.Auction.AuctionType.ToString(),
                    AuctionDescription = item.Auction.Description,
                    BidStatus = item.BidStatus.StatusName,
                    OrganizationId = item.Organization.Id.ToString(),
                    OrganizationName = item.Organization.FullName,
                    Price = item.Price,
                    CreatedDate = item.CreatedDate,
                    BidDescription = item.Description
                };
                bids.Add(bid);
            }
            return bids;
        }


        public List<BidInfoViewModel> ShowBidsForAuctionWithFitOrganizationBalance(Guid auctionId)
        {
            var auction = _applicationDbContext.Auctions.Include("Organizations").Include("Bids").SingleOrDefault(p => p.Id == auctionId);
            if (auction == null)
                throw new Exception($"Аукциона с таким id {auctionId} не существует");

            var activeBidStatus = _applicationDbContext.BidStatuses.SingleOrDefault(p => p.StatusName == "Active").Id;
            if (activeBidStatus == null)
                throw new Exception("Ошибка таблицы статусов ставок");

            var allBids = auction.Bids.Where(p=>p.BidStatusId== activeBidStatus).ToList();
            if (allBids.Count == 0)
                throw new Exception($"По аукциону {auctionId} нет активных ставок");

            List<Bid> bidsWithFitBalance = new List<Bid>();
            foreach (Bid item in allBids)
            {
                var org = item.OrganizationId;
                var sumDeposits = _applicationDbContext.Transactions
                    .Where(p => p.OrganizationId == org && p.TransactionType == TransactionType.Deposit).Sum(p => p.Sum);
                var sumWithdraws = _applicationDbContext.Transactions
                    .Where(p => p.OrganizationId == org && p.TransactionType == TransactionType.Withdraw).Sum(p => p.Sum);
                if (sumDeposits- sumWithdraws > item.Price)
                    bidsWithFitBalance.Add(item);
            }
            if (bidsWithFitBalance.Count == 0)
                throw new Exception($"По аукциону {auctionId} нет ставок от организаций с достаточным балансом");

            List<BidInfoViewModel> bids = new List<BidInfoViewModel>();
            foreach (Bid item in bidsWithFitBalance)
            {
                BidInfoViewModel bid = new BidInfoViewModel()
                {
                    BidId = item.Id.ToString(),
                    AuctionId = auctionId.ToString(),
                    AuctionType = item.Auction.AuctionType.ToString(),
                    AuctionDescription = item.Auction.Description,
                    BidStatus = item.BidStatus.StatusName,
                    OrganizationId = item.Organization.Id.ToString(),
                    OrganizationName = item.Organization.FullName,
                    Price = item.Price,
                    CreatedDate = item.CreatedDate,
                    BidDescription = item.Description
                };
                bids.Add(bid);
            }
            return bids;
        }


        public List<BidInfoViewModel> ShowBidsForAuctionWithFitOrganizationRatingAndBalance(Guid auctionId)
        {
            var auction = _applicationDbContext.Auctions.Include("Organizations").Include("Bids").SingleOrDefault(p => p.Id == auctionId);
            if (auction == null)
                throw new Exception($"Аукциона с таким id {auctionId} не существует");

            var activeBidStatus = _applicationDbContext.BidStatuses.SingleOrDefault(p => p.StatusName == "Active").Id;
            if (activeBidStatus == null)
                throw new Exception("Ошибка таблицы статусов ставок");

            var allBids = auction.Bids.Where(p=>p.BidStatusId==activeBidStatus).ToList();
            if (allBids.Count == 0)
                throw new Exception($"По аукциону {auctionId} нет активных ставок");

            List<Bid> bidsWithFitRating = new List<Bid>();
            foreach (Bid item in allBids)
            {
                var org = item.OrganizationId;
                var avg = _applicationDbContext.OrganizationRatings.Where(p => p.OrganizationId == org).Average(p => p.Score);
                if (avg >= auction.MinRatingForParticipant)
                    bidsWithFitRating.Add(item);
            }
            if (bidsWithFitRating.Count == 0)
                throw new Exception($"По аукциону {auctionId} нет ставок от организаций с нужным рейтингом");

            List<Bid> bidsWithFitRatingBalance = new List<Bid>();
            foreach (Bid item in bidsWithFitRating)
            {
                var org = item.OrganizationId;
                var sumDeposits = _applicationDbContext.Transactions
                    .Where(p => p.OrganizationId == org && p.TransactionType == TransactionType.Deposit).Sum(p => p.Sum);
                var sumWithdraws = _applicationDbContext.Transactions
                    .Where(p => p.OrganizationId == org && p.TransactionType == TransactionType.Withdraw).Sum(p => p.Sum);
                if (sumDeposits - sumWithdraws > item.Price)
                    bidsWithFitRatingBalance.Add(item);
            }
            if (bidsWithFitRatingBalance.Count == 0)
                throw new Exception($"По аукциону {auctionId} нет ставок от организаций с достаточным балансом");

            List<BidInfoViewModel> bids = new List<BidInfoViewModel>();
            foreach (Bid item in bidsWithFitRatingBalance)
            {
                BidInfoViewModel bid = new BidInfoViewModel()
                {
                    BidId = item.Id.ToString(),
                    AuctionId = auctionId.ToString(),
                    AuctionType = item.Auction.AuctionType.ToString(),
                    AuctionDescription = item.Auction.Description,
                    BidStatus = item.BidStatus.StatusName,
                    OrganizationId = item.Organization.Id.ToString(),
                    OrganizationName = item.Organization.FullName,
                    Price = item.Price,
                    CreatedDate = item.CreatedDate,
                    BidDescription = item.Description
                };
                bids.Add(bid);
            }
            return bids;
        }

        public List<AuctionInfoViewModel> ShowActiveAuctionsExceptMine(Guid organizationId)
        {
            var organization = _applicationDbContext.Organizations.SingleOrDefault(p => p.Id == organizationId);
            if (organization==null)
                throw new Exception($"Организации с таким id {organizationId} не существует");

            var auctions = _applicationDbContext.Auctions
                .Where(p => p.OrganizationId != organizationId && p.Status==AuctionStatus.Active).ToList();
            if (auctions.Count==0)
                throw new Exception("Активных аукционов на данный момент в базе нет");

            List<AuctionInfoViewModel> auctionModels = new List<AuctionInfoViewModel>();
            foreach (Auction item in auctions)
            {
                AuctionInfoViewModel model = new AuctionInfoViewModel()
                {
                    AuctionId = item.Id.ToString(),
                    Status = item.Status.ToString(),
                    AuctionType = item.AuctionType.Name,
                    OrganizationName = item.Organization.FullName,
                    ShippingAddress = item.ShippingAddress,
                    ShippingConditions = item.ShippingConditions,
                    StartPrice = item.StartPrice,
                    PriceStep = item.PriceStep,
                    MinPrice = item.MinPrice,
                    StartDate = item.StartDate,
                    FinishDate = item.FinishDate,
                    AuctionFiles = item.AuctionFiles.ToList()
                };
                auctionModels.Add(model);
            }

            return auctionModels;
        }



        public AuctionManagementService()
        {
            _applicationDbContext = new ApplicationDbContext();
        }
    }
}