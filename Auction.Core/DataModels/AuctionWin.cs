using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAuction.Core.DataModels
{
    public class AuctionWin
    {
        public Guid Id { get; set; }
        public DateTime SetupDate { get; set; }
        public string Note { get; set; }

        public Guid AuctionId { get; set; }
        public Auction Auction { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
