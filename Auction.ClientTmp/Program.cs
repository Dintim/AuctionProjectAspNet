using EAuction.BLL.Services;
using EAuction.BLL.Sevices;
using EAuction.BLL.ViewModels;
using EAuction.Core.DataModels;
using EAuction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EAuction.ClientTmp
{
    class Program
    {
        public enum TransType { Deposit, Withdraw }
        static void Main(string[] args)
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();
            applicationDb.Database.CreateIfNotExists();

            OrganizationType organizationType = new OrganizationType()
            {
                Id = Guid.NewGuid(),
                Name = "TstTypeA_86"
            };
            applicationDb.OrganizationTypes.Add(organizationType);
            applicationDb.SaveChanges();

            Organization auctionMaster = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompanyA_86",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(auctionMaster);
            applicationDb.SaveChanges();

            Organization auctionParticipant = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompanyA_862",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(auctionParticipant);
            applicationDb.SaveChanges();

            AuctionType auctionType = new AuctionType()
            {
                Id = Guid.NewGuid(),
                Name = "TestAuctionType_86"
            };
            applicationDb.AuctionTypes.Add(auctionType);
            applicationDb.SaveChanges();

            Auction auction = new Auction()
            {
                Id = Guid.NewGuid(),
                Description = "test_any description_86",
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
                Price = bid.Price
            };

            AuctionManagementService sut = new AuctionManagementService();
            sut.ElectWinnerInAuction(model);
        }
    }
}
