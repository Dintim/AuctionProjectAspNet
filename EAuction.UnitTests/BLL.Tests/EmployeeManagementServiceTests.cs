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

            var orgAO = applicationDb.OrganizationTypes.SingleOrDefault(p => p.Name == "АО");
            if (orgAO == null)
            {
                OrganizationType ao = new OrganizationType()
                {
                    Id = Guid.NewGuid(),
                    Name = "АО"
                };
                applicationDb.OrganizationTypes.Add(ao);
                applicationDb.SaveChanges();
                orgAO = ao;
            }

            Organization organization = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "Тестовая компания",
                IdentificationNumber = "1111111",
                Address = "Almaty, Abai ave",
                Email = "111@mail.ru",
                Contacts = "+7777123456",
                Site = "111.org",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = orgAO.Id
            };            
            applicationDb.Organizations.Add(organization);
            applicationDb.SaveChanges();

            var ceoPos = applicationDb.EmployeePositions.SingleOrDefault(p => p.Name == "CEO");            
            if (ceoPos == null)
            {
                EmployeePosition pos = new EmployeePosition()
                {
                    Id = Guid.NewGuid(),
                    Name = "CEO"
                };
                applicationDb.EmployeePositions.Add(pos);
                applicationDb.SaveChanges();
                ceoPos = pos;
            }

            Employee ceo = new Employee()
            {
                Id = Guid.NewGuid(),
                FirstName = "Тест",
                LastName = "Тестов",
                DoB = new DateTime(1985, 3, 4),
                Email = "ttt@mail.ru",
                OrganizationId = organization.Id,
                EmployeePositionId = ceoPos.Id
            };            
            applicationDb.Employees.Add(ceo);
            applicationDb.SaveChanges();

            EmployeeInfoViewModel model = new EmployeeInfoViewModel()
            {
                PositionName = "Manager",
                FirstName = "Тестбек",
                LastName = "Тестбеков",
                DoB = new DateTime(1983, 5, 22),
                Email = "ttt2@mail.ru",
                Password = "qwerty123",
                PasswordConfirm = "qwerty123"
            };

            EmployeeManagementService sut = new EmployeeManagementService();
            var ceoId = ceo.Id;

            sut.CreateEmployeeByCeo(model, ceoId);

            Employee emp = applicationDb.Employees.SingleOrDefault(p => p.FirstName == "Динара" && p.LastName == "Кадырбаева");
            ApplicationUser user = identityDb.ApplicationUsers.SingleOrDefault(p => p.Email == "kadyr_dinara@mail.ru");
            ApplicationUserPasswordHistory userPasswordHistory = identityDb.ApplicationUserPasswordHistories
                .SingleOrDefault(p => p.ApplicationUserId == user.Id && p.Password== "qwerty123");

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

            Employee emp = applicationDb.Employees.SingleOrDefault(p => p.FirstName == "Тестий" && p.LastName == "Тестиевич");

            EmployeeInfoViewModel model = new EmployeeInfoViewModel()
            {
                EmployeeId = emp.Id.ToString(),
                PositionName = "Accountant",
                FirstName = "Тестбек",
                LastName = "Тестбаев",
                DoB = new DateTime(1970, 6, 18),
                Email = "ttt3@mail.ru",
                Password = "123qwerty123",
                PasswordConfirm = "123qwerty123"
            };

            EmployeeManagementService sut = new EmployeeManagementService();
            sut.EditEmployeeInfo(model);

            Employee employee = applicationDb.Employees
                .SingleOrDefault(p => p.Id == emp.Id && p.FirstName == "Тестбек" && p.LastName == "Тестбаев");
            ApplicationUser user = identityDb.ApplicationUsers.SingleOrDefault(p => p.Email == "ttt3@mail.ru");
            ApplicationUserPasswordHistory userPasswordHistory = identityDb.ApplicationUserPasswordHistories
                .SingleOrDefault(p => p.ApplicationUserId == user.Id && p.Password == "123qwerty123");

            Assert.IsNotNull(employee);
            Assert.IsNotNull(user);
            Assert.IsNotNull(userPasswordHistory);

        }
    }
}
