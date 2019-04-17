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

            OrganizationType organizationType = new OrganizationType()
            {
                Id = Guid.NewGuid(),
                Name = "TestType_05"
            };
            applicationDb.OrganizationTypes.Add(organizationType);
            applicationDb.SaveChanges();

            Organization organization = new Organization()
            {
                Id = Guid.NewGuid(),
                FullName = "TestCompany_05",
                RegistrationDate = DateTime.Now,
                OrganizationTypeId = organizationType.Id
            };
            applicationDb.Organizations.Add(organization);
            applicationDb.SaveChanges();

            List<HttpPostedFileBase> files = new List<HttpPostedFileBase>();
            OrganizationInfoViewModel model = new OrganizationInfoViewModel()
            {
                OrganizationId = organization.Id.ToString(),
                FullName = "AnotherTestCompany_05",
                IdentificationNumber = organization.IdentificationNumber,
                Address = organization.Address,
                Email = "testc05@gmail.com",
                Contacts = organization.Contacts,
                Site = organization.Site,
                OrganizationType = "AnotherTestType_05",
                UploadedFiles = files
            };

            OrganizationManagementService sut = new OrganizationManagementService();
            sut.EditOrganizationInfo(model);

            Organization org = applicationDb.Organizations.SingleOrDefault(p => p.Id == organization.Id);

            Console.WriteLine(org!=null);
            //Console.WriteLine(org.FullName == "AnotherTestCompany_05" && org.Email == "testc05@gmail.com");
        }
    }
}
