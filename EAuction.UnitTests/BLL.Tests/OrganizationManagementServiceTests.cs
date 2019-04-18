using EAuction.BLL.Sevices;
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
    public class OrganizationManagementServiceTests
    {
        [TestMethod]
        public void OrganizationManagementService_OpenOrganization()
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();
            IdentityDbContext identityDb = new IdentityDbContext();
            applicationDb.Database.CreateIfNotExists();
            identityDb.Database.CreateIfNotExists();

            RegisterOrganizationViewModel model = new RegisterOrganizationViewModel()
            {
                FullName = "TestCompany_01",
                IdentificationNumber = "22222222",
                OrganizationType = "ТОО",
                CeoFirstName = "TestCeo",
                CeoLastName = "TestCeo",
                CeoEmail = "test@gmail.com",
                CeoDoB = new DateTime(1985, 3, 9),
                Password = "test111",
                PasswordConfirm = "test111"
            };

            OrganizationManagementService sut = new OrganizationManagementService();
            sut.OpenOrganization(model);

            Organization org = applicationDb.Organizations
                .SingleOrDefault(p => p.FullName == "TestCompany_01" && p.IdentificationNumber == "22222222");
            Employee emp = applicationDb.Employees.SingleOrDefault(p => p.FirstName == "TestCeo" && p.LastName == "TestCeo");
            ApplicationUser user = identityDb.ApplicationUsers.SingleOrDefault(p => p.Email == "test@gmail.com");
            ApplicationUserPasswordHistory userPasswordHistory = identityDb.ApplicationUserPasswordHistories
                .SingleOrDefault(p => p.ApplicationUserId == user.Id && p.Password == "test111");
            ApplicationUserSignInHistory userSignInHistory = identityDb.ApplicationUserSignInHistories
                .SingleOrDefault(p => p.ApplicationUserId == user.Id);

            Assert.IsNotNull(org);
            Assert.IsNotNull(emp);
            Assert.IsNotNull(user);
            Assert.IsNotNull(userPasswordHistory);
            Assert.IsNotNull(userSignInHistory);
        }

        [TestMethod]
        public void OrganizationManagementService_PutRatingScoreToOrganization()
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();            
            applicationDb.Database.CreateIfNotExists();

            OrganizationType organizationType = new OrganizationType()
            {
                Id = Guid.NewGuid(),
                Name = "TestType_02"
            };
            applicationDb.OrganizationTypes.Add(organizationType);
            applicationDb.SaveChanges();

            Organization organization = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompany_02",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(organization);
            applicationDb.SaveChanges();

            double score = 10.5;

            OrganizationManagementService sut = new OrganizationManagementService();
            sut.PutRatingScoreToOrganization(organization.Id, score);

            OrganizationRating rating = applicationDb.OrganizationRatings
                .SingleOrDefault(p => p.OrganizationId == organization.Id);
            Assert.IsNotNull(rating);
            Assert.IsTrue(rating.Score == score);
        }

        [TestMethod]
        public void OrganizationManagementService_MakeTransaction()
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();
            applicationDb.Database.CreateIfNotExists();

            OrganizationType organizationType = new OrganizationType()
            {
                Id = Guid.NewGuid(),
                Name = "TestType_03"
            };
            applicationDb.OrganizationTypes.Add(organizationType);
            applicationDb.SaveChanges();

            Organization organization = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompany_03",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(organization);
            applicationDb.SaveChanges();

            TransactionInfoViewModel model = new TransactionInfoViewModel()
            {
                TransactionTypeName = "Deposit",
                Sum = 145000.50M,
                Description = "test transaction",
                OrganizationId = organization.Id.ToString(),
                OrganizationName = organization.FullName
            };

            OrganizationManagementService sut = new OrganizationManagementService();
            sut.MakeTransaction(model);

            Transaction transaction = applicationDb.Transactions.SingleOrDefault(p => p.OrganizationId == organization.Id);

            Assert.IsNotNull(transaction);
            Assert.IsTrue(transaction.Sum == model.Sum && transaction.TransactionType.Equals(TransactionType.Deposit));
        }

        [TestMethod]
        public void OrganizationManagementService_EditOrganizationInfo()
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();
            applicationDb.Database.CreateIfNotExists();

            OrganizationType organizationType = new OrganizationType()
            {
                Id = Guid.NewGuid(),
                Name = "TestType_04"
            };
            applicationDb.OrganizationTypes.Add(organizationType);
            applicationDb.SaveChanges();

            Organization organization = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompany_04",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(organization);
            applicationDb.SaveChanges();

            List<HttpPostedFileBase> files = new List<HttpPostedFileBase>();
            OrganizationInfoViewModel model = new OrganizationInfoViewModel()
            {
                OrganizationId = organization.Id.ToString(),
                FullName = "TestCompany_04",
                IdentificationNumber = organization.IdentificationNumber,
                Address = organization.Address,
                Email = "testc04@gmail.com",
                Contacts = organization.Contacts,
                Site = organization.Site,
                OrganizationType = "AnotherTestType_04",
                UploadedFiles=files
            };

            OrganizationManagementService sut = new OrganizationManagementService();
            sut.EditOrganizationInfo(model);

            Organization org = applicationDb.Organizations.SingleOrDefault(p => p.Id == organization.Id);

            Assert.IsNotNull(org);
            //Assert.IsTrue(org.FullName == "TestCompany_04" && org.Email == "testc04@gmail.com");
        }
    }
}
