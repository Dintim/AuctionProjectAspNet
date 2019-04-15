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
                FullName = "ТестКомпани",
                IdentificationNumber = "22222222",
                OrganizationType = "ТОО",
                CeoFirstName = "Джон",
                CeoLastName = "Джонов",
                CeoEmail = "john@gmail.com",
                CeoDoB = new DateTime(1985, 3, 9),
                Password = "asdf111",
                PasswordConfirm = "asdf111"
            };

            OrganizationManagementService sut = new OrganizationManagementService();
            sut.OpenOrganization(model);

            Organization org = applicationDb.Organizations
                .SingleOrDefault(p => p.FullName == "ТестКомпани" && p.IdentificationNumber == "22222222");
            Employee emp = applicationDb.Employees.SingleOrDefault(p => p.FirstName == "Джон" && p.LastName == "Джонов");
            ApplicationUser user = identityDb.ApplicationUsers.SingleOrDefault(p => p.Email == "john@gmail.com");
            ApplicationUserPasswordHistory userPasswordHistory = identityDb.ApplicationUserPasswordHistories
                .SingleOrDefault(p => p.ApplicationUserId == user.Id && p.Password == "asdf111");
            ApplicationUserSignInHistory userSignInHistory = identityDb.ApplicationUserSignInHistories
                .SingleOrDefault(p => p.ApplicationUserId == user.Id);

            Assert.IsNotNull(org);
            Assert.IsNotNull(emp);
            Assert.IsNotNull(user);
            Assert.IsNotNull(userPasswordHistory);
            Assert.IsNotNull(userSignInHistory);
        }

        [TestMethod]
        public void OrganizationManagementService_EditOrganizationInfo()
        {

        }
    }
}
