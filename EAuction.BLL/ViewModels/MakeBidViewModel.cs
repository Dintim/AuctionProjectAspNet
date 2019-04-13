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
        public string AuctionId { get; set; }
        public string OrganizationId { get; set; }        
    }
}