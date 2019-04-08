using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAuction.Infrastructure.ViewModels
{
    public class CreateAuctionViewModel
    {
        public Guid AuctionTypeId { get; set; }
        public string Description { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingConditions { get; set; }
        public decimal StartPrice { get; set; }
        public decimal PriceStep { get; set; }
        public decimal MinPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        //public List<HttpPostedFileBase> UploadFiles { get; set; }
    }
}
