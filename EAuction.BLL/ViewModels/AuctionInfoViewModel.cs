using EAuction.Core.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EAuction.BLL.ViewModels
{
    public class AuctionInfoViewModel
    {
        public string AuctionId { get; set; }
        public string Status { get; set; }
        public string AuctionType { get; set; }
        public string OrganizationName { get; set; }
        public string Description { get; set; }
        public string MinRatingForParticipant { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingConditions { get; set; }
        public decimal StartPrice { get; set; }
        public decimal PriceStep { get; set; }
        public decimal MinPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public DateTime ? FinishDateAtActual { get; set; }
        public List<AuctionFile> AuctionFiles { get; set; }

    }
}