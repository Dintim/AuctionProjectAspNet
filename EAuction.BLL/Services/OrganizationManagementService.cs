using EAuction.BLL.ViewModels;
using EAuction.Infrastructure;
using EAuction.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EAuction.Core.DataModels;
using EAuction.BLL.ExternalModels;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace EAuction.BLL.Sevices
{
    public class OrganizationManagementService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IdentityDbContext _identityDbContext;

        public void OpenOrganization(RegisterOrganizationViewModel model)
        {
            GeoLocationInfo geoLocationInfo = GetGeolocationInfo();

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
                Id = Guid.NewGuid(),
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
                    Id = Guid.NewGuid(),
                    FirstName = model.CeoFirstName,
                    LastName = model.CeoLastName,
                    DoB = model.CeoDoB,
                    Email = model.CeoEmail,
                    EmployeePositionId = new Guid(ceoPosition.ToString())
                };
                _applicationDbContext.Employees.Add(employee);

                ApplicationUser user = new ApplicationUser()
                {
                    Id = Guid.NewGuid(),
                    Email = model.CeoEmail,
                    IsActive = true,
                    FailedSignInCount = 0,
                    CreatedDate = DateTime.Now,
                    AssosiatedEmployeeId = employee.Id
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

                ApplicationUserSignInHistory userSignInHistory = new ApplicationUserSignInHistory()
                {
                    Id = Guid.NewGuid(),
                    SignInTime = DateTime.Now,
                    MachineIp = geoLocationInfo.ip,
                    IpToGeoCountry = geoLocationInfo.country_name,
                    IpToGeoCity = geoLocationInfo.city,
                    IpToGeoLatitude = geoLocationInfo.latitude,
                    IpToGeoLongitude = geoLocationInfo.longitude,
                    ApplicationUserId = user.Id
                };
                _identityDbContext.ApplicationUserSignInHistories.Add(userSignInHistory);
            }             
            
            _applicationDbContext.SaveChanges();
        }

        public void EditOrganizationInfo(OrganizationInfoViewModel model, Guid organizationId)
        {
            var organization = _applicationDbContext.Organizations.SingleOrDefault(p => p.Id == organizationId);
            if (organization == null)
                throw new Exception($"Организации с id {organizationId} таким в базе нет");

            organization.FullName = model.FullName;
            organization.IdentificationNumber = model.IdentificationNumber;
            organization.OrganizationTypeId = _applicationDbContext.OrganizationTypes.SingleOrDefault(p => p.Name == model.OrganizationType).Id;
            organization.Address = model.Address;
            organization.Contacts = model.Contacts;
            organization.Site = model.Site;
            
            if (model.UploadedFiles.Count != 0)
            {
                foreach (HttpPostedFileBase i in model.UploadedFiles)
                {
                    OrganizationFile file = new OrganizationFile();
                    byte[] fileData = null;
                    
                    using (var binaryReader = new BinaryReader(i.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(i.ContentLength);
                    }

                    file.OrganizationId = organizationId;
                    file.FileName = i.FileName;
                    file.Extension = i.ContentType;
                    file.Content = fileData;
                    file.CreatedAt = DateTime.Now;
                    _applicationDbContext.OrganizationFiles.Add(file);
                }                
            }

            _applicationDbContext.SaveChanges();
        }


        public OrganizationInfoViewModel GetOrganizationInfo(Guid organizationId)
        {
            var organization = _applicationDbContext.Organizations.Include("OrganizationRatings").SingleOrDefault(p => p.Id == organizationId);
            if (organization == null)
                throw new Exception($"Организации с таким id {organizationId} не существует");

            var organizationFiles = _applicationDbContext.OrganizationFiles.Where(p => p.OrganizationId == organizationId).ToList();            
            var averageScore = organization.OrganizationRatings.Average(p => p.Score);

            OrganizationInfoViewModel model = new OrganizationInfoViewModel()
            {
                OrganizationId = organization.Id.ToString(),
                FullName = organization.FullName,
                IdentificationNumber = organization.IdentificationNumber,
                OrganizationType = organization.OrganizationType.Name,
                OrganizationRating = averageScore,
                Address = organization.Address,
                Email = organization.Email,
                Contacts = organization.Contacts,
                Site = organization.Site,
                OrganizationFiles = organizationFiles
            };

            return model;
        }


        public GeoLocationInfo GetGeolocationInfo()
        {
            WebClient webClient = new WebClient();
            string externalIp = webClient
                .DownloadString("http://icanhazip.com");

            string ipStackAccessKey = "cb6a8892805bdd4727b7669b1f584318";
            string ipStackUrl = $"api.ipstack.com/{externalIp}?access_key={ipStackAccessKey}";
            ipStackUrl = "http://" + ipStackUrl;

            string ipInfoAsJson = webClient.DownloadString(ipStackUrl);

            GeoLocationInfo geoLocationInfo = JsonConvert.DeserializeObject<GeoLocationInfo>(ipInfoAsJson);
            return geoLocationInfo;
        }


        public OrganizationManagementService()
        {
            _applicationDbContext = new ApplicationDbContext();
            _identityDbContext = new IdentityDbContext();
        }
    }
}