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

        }
    }
}
