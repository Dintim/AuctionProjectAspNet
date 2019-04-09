using EAuction.BLL.ViewModels;
using EAuction.Infrastructure;
using EAuction.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EAuction.Core.DataModels;

namespace EAuction.BLL.Sevices
{
    public class OrganizationManagementService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IdentityDbContext _identityDbContext;

        public void OpenOrganization(RegisterOrganizationViewModel model)
        {
            if (model == null)
                throw new Exception($"{typeof(RegisterOrganizationViewModel).Name} is null");

            var checkOrganization = _applicationDbContext.Organizations
                .SingleOrDefault(p => p.IdentificationNumber == model.IdentificationNumber || p.FullName == model.FullName);
                /*(from o in _aplicationDbContext.Organizations
                                    where o.IdentificationNumber == model.IdentificationNumber ||
                                    o.FullName == model.FullName
                                    select o).ToList(); */                

            var checkOrganizationType = _applicationDbContext.OrganizationTypes
                .SingleOrDefault(p => p.Name == model.OrganizationType);

            if (checkOrganization != null || checkOrganizationType == null)
                throw new Exception("Model validation error!");


            Organization organization = new Organization()
            {
                FullName = model.FullName,
                IdentificationNumber = model.IdentificationNumber,
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = checkOrganizationType.Id
            };
            _applicationDbContext.Organizations.Add(organization);

            var checkEmployeeEmail = _applicationDbContext.Employees.Any(p => p.Email == model.CeoEmail);
            if (!checkEmployeeEmail)
            {
                var ceoPosition = _applicationDbContext.EmployeePositions.Where(p => p.Name == "CEO").Select(p => p.Id);
                Employee employee = new Employee()
                {
                    FirstName = model.CeoFirstName,
                    LastName = model.CeoLastName,
                    DoB = model.CeoDoB,
                    Email = model.CeoEmail,
                    EmployeePositionId = new Guid(ceoPosition.ToString())
                };
                _applicationDbContext.Employees.Add(employee);



            }
                


            _applicationDbContext.SaveChanges();
        }


        public OrganizationManagementService()
        {
            _applicationDbContext = new ApplicationDbContext();
            _identityDbContext = new IdentityDbContext();
        }
    }
}