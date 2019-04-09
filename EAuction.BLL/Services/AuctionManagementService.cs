using EAuction.BLL.ViewModels;
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

        //public void MakeBidToAuction(MakeBidVm model)
        //{
        //    var bidExists = _applicationDbContext.Bids
        //        .Any(p => p.Price == model.Price &&
        //        p.AuctionId == model.AuctionId &&
        //        p.Description == model.Description &&
        //        p.OrganizationId == model.OrganizationId);

        //    if (bidExists)
        //        throw new Exception("Invalid bid");

        //    var inValidPriceRange = _applicationDbContext
        //        .Auctions.Where(p => p.Id == model.AuctionId &&
        //        p.PriceAtMinimum < model.Price &&
        //        p.PriceAtStart > model.Price);

        //    var inStepRange = inValidPriceRange
        //        .Any(p => (p.PriceAtStart - model.Price) % p.PriceChangeStep == 0);

        //    if (!inStepRange)
        //        throw new Exception("Invalid bid according price step");

        //    Bid bid = new Bid()
        //    {
        //        Price = model.Price,
        //        Description = model.Description,
        //        AuctionId = model.AuctionId,
        //        OrganizationId = model.OrganizationId,
        //        CreatedDate = DateTime.Now
        //    };
        //    _applicationDbContext.Bids.Add(bid);
        //    _applicationDbContext.SaveChanges();

        //}

        public void RevokeBidFromAuction()
        {

        }

        public void GetAuctionInfo()
        {

        }

        public void ElectWinnerInAuction()
        {

        }

        public AuctionManagementService()
        {
            _applicationDbContext = new ApplicationDbContext();
        }
    }
}