﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAuction.Infrastructure.ViewModels
{
    public class RegisterOrganizationViewModel
    {
        public string FullName { get; set; }
        public string IdentificationNumber { get; set; }
        public Guid OrganizationTypeId { get; set; }
        public string CeoFirstName { get; set; }
        public string CeoLastName { get; set; }
        public string CeoEmail { get; set; }
        public DateTime CeoDoB { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }
}
