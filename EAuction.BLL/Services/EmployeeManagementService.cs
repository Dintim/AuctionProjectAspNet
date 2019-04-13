using EAuction.BLL.ViewModels;
using EAuction.Core.DataModels;
using EAuction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EAuction.BLL.Services
{
    public class EmployeeManagementService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IdentityDbContext _identityDbContext;

        public void CreateEmployeeByCeo(EmployeeInfoViewModel model, Guid ceoId)
        {
            var employee = _applicationDbContext.Employees.SingleOrDefault(p => p.Id == ceoId);
            if (employee==null)
                throw new Exception($"Работника с id {ceoId} в базе нет");

            var isCeo = employee.EmployeePosition.Name == "CEO";
            if (!isCeo)
                throw new Exception($"Работник с id {ceoId} не может создавать аккаунты других сотрудников компании");

            var positionId = _applicationDbContext.EmployeePositions.SingleOrDefault(p => p.Name == model.PositionName).Id;
            if (positionId == null)
            {
                EmployeePosition pos = new EmployeePosition()
                {
                    Id = Guid.NewGuid(),
                    Name = model.PositionName
                };
                positionId = pos.Id;
            }            

            Employee newEmployee = new Employee()
            {
                Id = Guid.NewGuid(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                DoB = model.DoB,
                Email = model.Email,
                EmployeePositionId= positionId,
                OrganizationId= employee.OrganizationId
            };
            _applicationDbContext.Employees.Add(newEmployee);            

            ApplicationUser user = new ApplicationUser()
            {
                Id = Guid.NewGuid(),
                Email = model.Email,
                IsActive = true,
                FailedSignInCount = 0,
                CreatedDate = DateTime.Now,
                AssosiatedEmployeeId = newEmployee.Id
            };
            _identityDbContext.ApplicationUsers.Add(user);

            ApplicationUserPasswordHistory userPasswordHistory = new ApplicationUserPasswordHistory()
            {
                Id = Guid.NewGuid(),
                SetupDate = DateTime.Now,
                Password = model.Password,
                ApplicationUserId = user.Id
            };
            _identityDbContext.ApplicationUserPasswordHistories.Add(userPasswordHistory);

            _applicationDbContext.SaveChanges();
            _identityDbContext.SaveChanges();
        }


        public void EditEmployeeInfo(EmployeeInfoViewModel model)
        {
            var employee = _applicationDbContext.Employees.SingleOrDefault(p => p.Id.ToString() == model.EmployeeId);
            if (employee == null)
                throw new Exception($"Работника с id {model.EmployeeId} в базе нет");

            var positionId = _applicationDbContext.EmployeePositions.SingleOrDefault(p => p.Name == model.PositionName).Id;

            employee.FirstName = model.FirstName;
            employee.LastName = model.LastName;
            employee.DoB = model.DoB;
            employee.EmployeePositionId = positionId;

            var user = _identityDbContext.ApplicationUsers.Include("ApplicationUserPasswordHistories")
                .SingleOrDefault(p => p.AssosiatedEmployeeId.ToString() == model.EmployeeId && p.IsActive == true);
            var userPasswordHistory = user.ApplicationUserPasswordHistories
                .SingleOrDefault(p => p.ApplicationUserId == user.Id && p.InvalidatedDate == null);

            if (employee.Email!=model.Email)
            {
                user.IsActive = false;
                userPasswordHistory.InvalidatedDate = DateTime.Now;

                ApplicationUser newUser = new ApplicationUser()
                {
                    Id = Guid.NewGuid(),
                    Email = model.Email,
                    IsActive = true,
                    FailedSignInCount = 0,
                    CreatedDate = DateTime.Now,
                    AssosiatedEmployeeId = employee.Id
                };
                _identityDbContext.ApplicationUsers.Add(newUser);

                ApplicationUserPasswordHistory newUserPasswordHistory = new ApplicationUserPasswordHistory()
                {
                    Id = Guid.NewGuid(),
                    SetupDate = DateTime.Now,
                    Password = model.Password,
                    ApplicationUserId = newUser.Id
                };
                _identityDbContext.ApplicationUserPasswordHistories.Add(newUserPasswordHistory);
            }
            else
            {
                if (userPasswordHistory.Password!=model.Password)
                {
                    userPasswordHistory.InvalidatedDate = DateTime.Now;
                    ApplicationUserPasswordHistory userNewPasswordHistory = new ApplicationUserPasswordHistory()
                    {
                        Id = Guid.NewGuid(),
                        SetupDate = DateTime.Now,
                        Password = model.Password,
                        ApplicationUserId = user.Id
                    };
                    _identityDbContext.ApplicationUserPasswordHistories.Add(userNewPasswordHistory);
                }
            }

            _applicationDbContext.SaveChanges();
            _identityDbContext.SaveChanges();
        }


        public EmployeeInfoViewModel GetEmployeeInfo(Guid employeeId)
        {
            var employee = _applicationDbContext.Employees.SingleOrDefault(p => p.Id == employeeId);
            if (employee == null)
                throw new Exception($"Работника с id {employeeId} в базе нет");

            EmployeeInfoViewModel model = new EmployeeInfoViewModel()
            {
                EmployeeId = employee.Id.ToString(),
                PositionName = employee.EmployeePosition.Name,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DoB = employee.DoB,
                Email = employee.Email
            };

            return model;
        }

        public List<EmployeeInfoViewModel> GetEmployeesInfoByOrganization(Guid organizationId)
        {
            var organization = _applicationDbContext.Organizations.Include("Employees").SingleOrDefault(p => p.Id == organizationId);
            if (organization == null)
                throw new Exception($"Организации с id {organizationId} в базе не существует");

            var employees = organization.Employees.ToList();
            if (employees.Count==0)
                throw new Exception($"В организации {organization.FullName} нет ни одного сотрудника");

            List<EmployeeInfoViewModel> employeeInfos = new List<EmployeeInfoViewModel>();
            foreach (Employee item in employees)
            {
                var model = GetEmployeeInfo(item.Id);
                employeeInfos.Add(model);
            }

            return employeeInfos;
        }



        public EmployeeManagementService()
        {
            _applicationDbContext = new ApplicationDbContext();
            _identityDbContext = new IdentityDbContext();
        }
    }
}