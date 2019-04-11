using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EAuction.BLL.ViewModels
{
    public class MakeBidViewModel
    {
        public decimal Price { get; set; }
        public string Description { get; set; }
        public Guid AuctionId { get; set; }
        public Guid OrganizationId { get; set; }        
    }
}