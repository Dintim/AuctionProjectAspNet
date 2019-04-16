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

namespace EAuction.UnitTests.BLL.Tests
{
    [TestClass]
    public class EmployeeManagementServiceTests
    {
        [TestMethod]
        public void EmployeeManagementService_CreateEmployeeByCeo()
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();
            IdentityDbContext identityDb = new IdentityDbContext();
            applicationDb.Database.CreateIfNotExists();
            identityDb.Database.CreateIfNotExists();            

            OrganizationType organizationType = new OrganizationType()
            {
                Id = Guid.NewGuid(),
                Name = "TestTypeE_01"
            };
            applicationDb.OrganizationTypes.Add(organizationType);
            applicationDb.SaveChanges();

            Organization organization = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompanyE_01",
                IdentificationNumber = "1111111",
                Address = "Almaty, Abai ave",
                Email = "test@mail.ru",
                Contacts = "+7777123456",
                Site = "test.org",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };            
            applicationDb.Organizations.Add(organization);
            applicationDb.SaveChanges();

            var ceoPos = applicationDb.EmployeePositions.SingleOrDefault(p => p.Name == "CEO");
            if (ceoPos == null)
            {
                EmployeePosition employeePosition = new EmployeePosition()
                {
                    Id = Guid.NewGuid(),
                    Name = "CEO"
                };
                applicationDb.EmployeePositions.Add(employeePosition);
                applicationDb.SaveChanges();
                ceoPos = employeePosition;
            }

            Employee ceo = new Employee()
            {
                Id = Guid.NewGuid(),
                FirstName = "TestECeo_01",
                LastName = "TestECeo_01",
                DoB = new DateTime(1985, 3, 4),
                Email = "ttte01@mail.ru",
                OrganizationId = organization.Id,
                EmployeePositionId = ceoPos.Id
            };            
            applicationDb.Employees.Add(ceo);
            applicationDb.SaveChanges();

            EmployeeInfoViewModel model = new EmployeeInfoViewModel()
            {
                PositionName = "TestE_Position01",
                FirstName = "TestEmployee_01",
                LastName = "TestEmployee_01",
                DoB = new DateTime(1983, 5, 22),
                Email = "ttt01@mail.ru",
                Password = "ttt01qwerty123",
                PasswordConfirm = "ttt01qwerty123"
            };

            EmployeeManagementService sut = new EmployeeManagementService();
            var ceoId = ceo.Id;

            sut.CreateEmployeeByCeo(model, ceoId);

            Employee emp = applicationDb.Employees
                .SingleOrDefault(p => p.FirstName == "TestEmployee_01" && p.LastName == "TestEmployee_01");
            ApplicationUser user = identityDb.ApplicationUsers.SingleOrDefault(p => p.Email == "ttt01@mail.ru");
            ApplicationUserPasswordHistory userPasswordHistory = identityDb.ApplicationUserPasswordHistories
                .SingleOrDefault(p => p.ApplicationUserId == user.Id && p.Password== "ttt01qwerty123");

            Assert.IsNotNull(emp);
            Assert.IsNotNull(user);
            Assert.IsNotNull(userPasswordHistory);            
        }

        [TestMethod]
        public void EmployeeManagementService_EditEmployeeInfo()
        {
            ApplicationDbContext applicationDb = new ApplicationDbContext();
            IdentityDbContext identityDb = new IdentityDbContext();
            applicationDb.Database.CreateIfNotExists();
            identityDb.Database.CreateIfNotExists();

            OrganizationType organizationType = new OrganizationType()
            {
                Id = Guid.NewGuid(),
                Name = "TestTypeE_02"
            };
            applicationDb.OrganizationTypes.Add(organizationType);
            applicationDb.SaveChanges();

            Organization organization = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompanyE_02",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(organization);
            applicationDb.SaveChanges();

            EmployeePosition employeePosition = new EmployeePosition()
            {
                Id = Guid.NewGuid(),
                Name = "TestE_Position02"
            };
            applicationDb.EmployeePositions.Add(employeePosition);
            applicationDb.SaveChanges();

            Employee employee = new Employee()
            {
                Id = Guid.NewGuid(),
                FirstName = "TestEmployee_02",
                LastName = "TestEmployee_02",
                DoB = new DateTime(1983, 5, 22),
                Email = "ttt02@mail.ru",
                EmployeePositionId=employeePosition.Id,
                OrganizationId=organization.Id
            };
            applicationDb.Employees.Add(employee);
            applicationDb.SaveChanges();

            ApplicationUser applicationUser = new ApplicationUser()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Email = employee.Email,
                IsActive = true,
                FailedSignInCount = 0,
                AssosiatedEmployeeId = employee.Id
            };
            identityDb.ApplicationUsers.Add(applicationUser);
            identityDb.SaveChanges();

            ApplicationUserPasswordHistory applicationUserPassword = new ApplicationUserPasswordHistory()
            {
                Id = Guid.NewGuid(),
                SetupDate = DateTime.Now,
                Password = "aaaa111",
                ApplicationUserId = applicationUser.Id
            };
            identityDb.ApplicationUserPasswordHistories.Add(applicationUserPassword);
            identityDb.SaveChanges();
            

            EmployeeInfoViewModel model = new EmployeeInfoViewModel()
            {
                EmployeeId = employee.Id.ToString(),
                PositionName = "Accountant",
                FirstName = "AnotherTestEmployee_02",
                LastName = "AnotherTestEmployee_02",
                DoB = new DateTime(1970, 6, 18),
                Email = "ttt03@mail.ru",
                Password = "ttt03qwerty123",
                PasswordConfirm = "ttt03qwerty123"
            };

            EmployeeManagementService sut = new EmployeeManagementService();
            sut.EditEmployeeInfo(model);

            Employee emp = applicationDb.Employees
                .SingleOrDefault(p => p.Id == employee.Id && p.FirstName == "AnotherTestEmployee_02"
                && p.LastName == "AnotherTestEmployee_02");
            ApplicationUser user = identityDb.ApplicationUsers.SingleOrDefault(p => p.Email == "ttt03@mail.ru");
            ApplicationUserPasswordHistory userPasswordHistory = identityDb.ApplicationUserPasswordHistories
                .SingleOrDefault(p => p.ApplicationUserId == user.Id && p.Password == "ttt03qwerty123");

            Assert.IsNotNull(employee);
            Assert.IsNotNull(user);
            Assert.IsNotNull(userPasswordHistory);

        }
    }
}
