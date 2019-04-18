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
            IdentityDbContext identityDb = new IdentityDbContext();
            identityDb.Database.CreateIfNotExists();

            //OrganizationType organizationType = new OrganizationType()
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "АО"
            //};
            //applicationDb.OrganizationTypes.Add(organizationType);
            //applicationDb.SaveChanges();

            //Organization kzto = new Organization()
            //{
            //    Id = Guid.NewGuid(),
            //    FullName = "Казахтелеком",
            //    RegistrationDate = DateTime.Now,
            //    OrganizationTypeId = organizationType.Id
            //};
            //applicationDb.Organizations.Add(kzto);
            //applicationDb.SaveChanges();

            //Organization ktzh = new Organization()
            //{
            //    Id = Guid.NewGuid(),
            //    FullName = "КТЖ",
            //    RegistrationDate = DateTime.Now,
            //    OrganizationTypeId = organizationType.Id
            //};
            //applicationDb.Organizations.Add(ktzh);
            //applicationDb.SaveChanges();

            //AuctionType auctionType1 = new AuctionType()
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Услуги питания"
            //};
            //applicationDb.AuctionTypes.Add(auctionType1);
            //applicationDb.SaveChanges();

            //AuctionType auctionType2 = new AuctionType()
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Канцелярские товары"
            //};
            //applicationDb.AuctionTypes.Add(auctionType2);
            //applicationDb.SaveChanges();

            //Auction auctionKZTO = new Auction()
            //{
            //    Id = Guid.NewGuid(),
            //    Description = "Закуп 1000 упаковок офисной бумаги",
            //    ShippingAddress = "г.Астана, пр.Кенесары, 185",
            //    ShippingConditions = "Поставка в течение 1 недели с даты определения победителя",
            //    MinRatingForParticipant = 10.5,
            //    StartPrice = 500000M,
            //    MinPrice = 100000M,
            //    PriceStep = 50000M,
            //    StartDate = new DateTime(2019, 4, 24),
            //    FinishDate = new DateTime(2019, 5, 15),
            //    Status = AuctionStatus.Active,
            //    AuctionTypeId = auctionType2.Id,
            //    OrganizationId = kzto.Id
            //};
            //applicationDb.Auctions.Add(auctionKZTO);
            //applicationDb.SaveChanges();

            //Auction auctionKTZH = new Auction()
            //{
            //    Id = Guid.NewGuid(),
            //    Description = "Закуп 50000 бутылок питьевой воды 0,5л",
            //    ShippingAddress = "г.Астана, пр.Мангилик ел, 43",
            //    ShippingConditions = "Поставка в течение 2 недель с даты определения победителя",
            //    MinRatingForParticipant = 11,
            //    StartPrice = 600000M,
            //    MinPrice = 100000M,
            //    PriceStep = 50000M,
            //    StartDate = new DateTime(2019, 4, 28),
            //    FinishDate = new DateTime(2019, 5, 28),
            //    Status = AuctionStatus.Active,
            //    AuctionTypeId = auctionType1.Id,
            //    OrganizationId = ktzh.Id
            //};
            //applicationDb.Auctions.Add(auctionKTZH);
            //applicationDb.SaveChanges();

            AuctionManagementService auctionManagement = new AuctionManagementService();
            var auctionList = auctionManagement.GetAllActiveAuctions();
            foreach (AuctionInfoViewModel item in auctionList)
            {
                Console.WriteLine(item);
            }

        }
    }
}
