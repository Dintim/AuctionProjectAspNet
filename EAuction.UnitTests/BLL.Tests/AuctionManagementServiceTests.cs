using EAuction.BLL.Services;
using EAuction.BLL.ViewModels;
using EAuction.Core.DataModels;
using EAuction.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EAuction.UnitTests.BLL.Tests
{
    [TestClass]
    public class AuctionManagementServiceTests
    {
        [TestMethod]
        public void AuctionManagementService_OpenAuction()
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();
            applicationDb.Database.CreateIfNotExists();

            OrganizationType organizationType = new OrganizationType()
            {
                Id = Guid.NewGuid(),
                Name = "TstTypeA_01"
            };
            applicationDb.OrganizationTypes.Add(organizationType);
            applicationDb.SaveChanges();

            Organization organization = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompanyA_01",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(organization);
            applicationDb.SaveChanges();

            var files = new List<HttpPostedFileBase>();
            CreateAuctionViewModel model = new CreateAuctionViewModel()
            {
                AuctionType = "Test_Услуги",
                Description = "test_поставки турагентский услуг",
                RatingForParticipant=10.5,
                StartDate = new DateTime(2019, 4, 24),
                FinishDate = new DateTime(2019, 5, 20),
                MinPrice = 140000.50M,
                StartPrice = 350000M,
                PriceStep = 50000M,
                UploadFiles=files
            };

            AuctionManagementService sut = new AuctionManagementService();
            sut.OpenAuction(model, organization.Id);

            Auction auction = applicationDb.Auctions
                .SingleOrDefault(p => p.OrganizationId == organization.Id && p.Description == "test_поставки турагентский услуг");

            Assert.IsNotNull(auction);
            Assert.IsTrue(auction.MinRatingForParticipant == 10.5);
        }

        [TestMethod]
        public void AuctionManagementService_MakeBidToAuction()
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();
            applicationDb.Database.CreateIfNotExists();

            OrganizationType organizationType = new OrganizationType()
            {
                Id = Guid.NewGuid(),
                Name = "TstTypeA_02"
            };
            applicationDb.OrganizationTypes.Add(organizationType);
            applicationDb.SaveChanges();

            Organization auctionMaster = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompanyA_02",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(auctionMaster);
            applicationDb.SaveChanges();

            Organization auctionParticipant = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompanyA_022",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(auctionParticipant);
            applicationDb.SaveChanges();

            AuctionType auctionType = new AuctionType()
            {
                Id = Guid.NewGuid(),
                Name = "TestAuctionType_02"
            };
            applicationDb.AuctionTypes.Add(auctionType);
            applicationDb.SaveChanges();

            Auction auction = new Auction()
            {
                Id = Guid.NewGuid(),
                Description = "test_any description",
                MinRatingForParticipant = 10.5,
                StartPrice = 500000M,
                MinPrice = 100000M,
                PriceStep = 50000M,
                StartDate = new DateTime(2019, 4, 26),
                FinishDate = new DateTime(2019, 5, 22),
                Status = AuctionStatus.Active,
                AuctionTypeId = auctionType.Id,
                OrganizationId = auctionMaster.Id
            };
            applicationDb.Auctions.Add(auction);
            applicationDb.SaveChanges();

            Transaction depositTrans = new Transaction()
            {
                Id = Guid.NewGuid(),
                TransactionType = TransactionType.Deposit,
                Sum = 400000,
                TransactionDate = DateTime.Now,
                OrganizationId = auctionParticipant.Id
            };
            applicationDb.Transactions.Add(depositTrans);
            applicationDb.SaveChanges();

            Transaction withdrawTrans = new Transaction()
            {
                Id = Guid.NewGuid(),
                TransactionType = TransactionType.Withdraw,
                Sum = 100000,
                TransactionDate = DateTime.Now,
                OrganizationId = auctionParticipant.Id
            };
            applicationDb.Transactions.Add(withdrawTrans);
            applicationDb.SaveChanges();

            MakeBidViewModel model = new MakeBidViewModel()
            {
                AuctionId = auction.Id.ToString(),
                OrganizationId = auctionParticipant.Id.ToString(),
                Price = 200000M
            };

            decimal bidCost = 50000M;
            AuctionManagementService sut = new AuctionManagementService();
            sut.MakeBidToAuction(model, bidCost);

            Bid bid = applicationDb.Bids
                .SingleOrDefault(p => p.AuctionId == auction.Id && p.OrganizationId == auctionParticipant.Id);
            Transaction transaction = applicationDb.Transactions
                .SingleOrDefault(p => p.OrganizationId == auctionParticipant.Id && p.Sum == bidCost);

            Assert.IsNotNull(bid);
            Assert.IsNotNull(transaction);

        }

        [TestMethod]
        public void AuctionManagementService_RevokeBidFromAuction()
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();
            applicationDb.Database.CreateIfNotExists();

            OrganizationType organizationType = new OrganizationType()
            {
                Id = Guid.NewGuid(),
                Name = "TstTypeA_03"
            };
            applicationDb.OrganizationTypes.Add(organizationType);
            applicationDb.SaveChanges();

            Organization auctionMaster = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompanyA_03",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(auctionMaster);
            applicationDb.SaveChanges();

            Organization auctionParticipant = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompanyA_032",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(auctionParticipant);
            applicationDb.SaveChanges();

            AuctionType auctionType = new AuctionType()
            {
                Id = Guid.NewGuid(),
                Name = "TestAuctionType_03"
            };
            applicationDb.AuctionTypes.Add(auctionType);
            applicationDb.SaveChanges();

            Auction auction = new Auction()
            {
                Id = Guid.NewGuid(),
                Description = "test_any description_03",
                MinRatingForParticipant = 9.5,
                StartPrice = 600000M,
                MinPrice = 100000M,
                PriceStep = 50000M,
                StartDate = new DateTime(2019, 4, 26),
                FinishDate = new DateTime(2019, 5, 22),
                Status = AuctionStatus.Active,
                AuctionTypeId = auctionType.Id,
                OrganizationId = auctionMaster.Id
            };
            applicationDb.Auctions.Add(auction);
            applicationDb.SaveChanges();

            var activeBidStatus = applicationDb.BidStatuses.SingleOrDefault(p => p.StatusName == "Active");
            if (activeBidStatus == null)
            {
                BidStatus status = new BidStatus()
                {
                    Id = Guid.NewGuid(),
                    StatusName = "Active"
                };
                applicationDb.BidStatuses.Add(status);
                applicationDb.SaveChanges();
                activeBidStatus = status;
            }

            Bid bid = new Bid()
            {
                Id = Guid.NewGuid(),
                Price = 200000M,
                CreatedDate = DateTime.Now,
                BidStatusId = activeBidStatus.Id,
                AuctionId = auction.Id,
                OrganizationId = auctionParticipant.Id
            };
            applicationDb.Bids.Add(bid);
            applicationDb.SaveChanges();

            AuctionManagementService sut = new AuctionManagementService();
            sut.RevokeBidFromAuction(bid.Id);

            Bid checkBid = applicationDb.Bids.Find(bid.Id);

            Assert.IsNotNull(checkBid);
            //Assert.IsTrue(bid.BidStatus.StatusName== "Revoke");
        }

        [TestMethod]
        public void AuctionManagementService_ElectWinnerInAuction()
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();
            applicationDb.Database.CreateIfNotExists();

            OrganizationType organizationType = new OrganizationType()
            {
                Id = Guid.NewGuid(),
                Name = "TstTypeA_04"
            };
            applicationDb.OrganizationTypes.Add(organizationType);
            applicationDb.SaveChanges();

            Organization auctionMaster = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompanyA_04",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(auctionMaster);
            applicationDb.SaveChanges();

            Organization auctionParticipant = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompanyA_042",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(auctionParticipant);
            applicationDb.SaveChanges();

            AuctionType auctionType = new AuctionType()
            {
                Id = Guid.NewGuid(),
                Name = "TestAuctionType_04"
            };
            applicationDb.AuctionTypes.Add(auctionType);
            applicationDb.SaveChanges();

            Auction auction = new Auction()
            {
                Id = Guid.NewGuid(),
                Description = "test_any description_04",
                MinRatingForParticipant = 9.5,
                StartPrice = 600000M,
                MinPrice = 100000M,
                PriceStep = 50000M,
                StartDate = new DateTime(2019, 4, 26),
                FinishDate = new DateTime(2019, 5, 22),
                Status = AuctionStatus.Active,
                AuctionTypeId = auctionType.Id,
                OrganizationId = auctionMaster.Id
            };
            applicationDb.Auctions.Add(auction);
            applicationDb.SaveChanges();

            Transaction depositTrans = new Transaction()
            {
                Id = Guid.NewGuid(),
                TransactionType = TransactionType.Deposit,
                Sum = 400000,
                TransactionDate = DateTime.Now,
                OrganizationId = auctionParticipant.Id
            };
            applicationDb.Transactions.Add(depositTrans);
            applicationDb.SaveChanges();

            OrganizationRating rating = new OrganizationRating()
            {
                Id = Guid.NewGuid(),
                Score = 10,
                OrganizationId = auctionParticipant.Id
            };
            applicationDb.OrganizationRatings.Add(rating);
            applicationDb.SaveChanges();

            var activeBidStatus = applicationDb.BidStatuses.SingleOrDefault(p => p.StatusName == "Active");
            if (activeBidStatus == null)
            {
                BidStatus status = new BidStatus()
                {
                    Id = Guid.NewGuid(),
                    StatusName = "Active"
                };
                applicationDb.BidStatuses.Add(status);
                applicationDb.SaveChanges();
                activeBidStatus = status;
            }

            Bid bid = new Bid()
            {
                Id = Guid.NewGuid(),
                Price = 200000M,
                CreatedDate = DateTime.Now,
                BidStatusId = activeBidStatus.Id,
                AuctionId = auction.Id,
                OrganizationId = auctionParticipant.Id
            };
            applicationDb.Bids.Add(bid);
            applicationDb.SaveChanges();

            BidInfoViewModel model = new BidInfoViewModel()
            {
                BidId = bid.Id.ToString(),
                AuctionId = auction.Id.ToString(),
                OrganizationId = auctionParticipant.Id.ToString(),
                CreatedDate = DateTime.Now,
                Price=bid.Price
            };

            AuctionManagementService sut = new AuctionManagementService();
            sut.ElectWinnerInAuction(model);

            AuctionWin win = applicationDb.AuctionWins
                .SingleOrDefault(p => p.AuctionId.ToString() == model.AuctionId && p.OrganizationId.ToString() == model.OrganizationId);
            Transaction transaction = applicationDb.Transactions
                .SingleOrDefault(p => p.Sum == model.Price && p.TransactionType == TransactionType.Withdraw
                && p.OrganizationId.ToString()==model.OrganizationId);

            Assert.IsNotNull(win);
            Assert.IsNotNull(transaction);
        }
    }
}
