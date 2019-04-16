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
            //ApplicationDbContext applicationDb = new ApplicationDbContext();
            //applicationDb.Database.CreateIfNotExists();

            //OrganizationType organizationType = new OrganizationType()
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "TstTypeA_01"
            //};
            //applicationDb.OrganizationTypes.Add(organizationType);
            //applicationDb.SaveChanges();

            //Organization organization = new Organization()
            //{
            //    Id = Guid.NewGuid(),
            //    FullName = "TestCompanyA_01",
            //    RegistrationDate = DateTime.Now,
            //    OrganizationTypeId = organizationType.Id
            //};
            //applicationDb.Organizations.Add(organization);
            //applicationDb.SaveChanges();

            //var files = new List<HttpPostedFileBase>();
            //CreateAuctionViewModel model = new CreateAuctionViewModel()
            //{
            //    AuctionType = "Test_Услуги",
            //    Description = "test_поставки турагентский услуг",
            //    RatingForParticipant = 10.5,
            //    StartDate = new DateTime(2019, 4, 24),
            //    FinishDate = new DateTime(2019, 5, 20),
            //    MinPrice = 140000.50M,
            //    StartPrice = 350000M,
            //    PriceStep = 50000M,
            //    UploadFiles = files
            //};

            //AuctionManagementService sut = new AuctionManagementService();
            //sut.OpenAuction(model, organization.Id);
        }
    }
}
