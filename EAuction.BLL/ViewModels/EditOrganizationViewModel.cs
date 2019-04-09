using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace EAuction.BLL.ViewModels
{
    public class EditOrganizationViewModel
    {
        public string FullName { get; set; }
        public string IdentificationNumber { get; set; }
        public Guid OrganizationTypeId { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Contacts { get; set; }
        public string Site { get; set; }
        public List<HttpPostedFileBase> UploadFiles { get; set; }
    }
}
