using EAuction.BLL.Services;
using EAuction.BLL.ViewModels;
using EAuction.Core.DataModels;
using EAuction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAuction.ClientTmp
{
    class Program
    {
        static void Main(string[] args)
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
        }
    }
}
