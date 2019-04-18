using EAuction.BLL.Services;
using EAuction.BLL.Sevices;
using EAuction.BLL.ViewModels;
using EAuction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Auction.ClientUI.Controllers
{
    public class HomeController : Controller
    {
        AuctionManagementService auctionManagement = new AuctionManagementService();
        OrganizationManagementService organizationManagement = new OrganizationManagementService();
        EmployeeManagementService employeeManagement = new EmployeeManagementService();
        UserManagementService userManagement = new UserManagementService();

        
        public ActionResult Index()
        {
            List<AuctionInfoViewModel> allAuctions = auctionManagement.GetAllActiveAuctions();
            ViewBag.allAuctions = allAuctions;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        
        public ActionResult ShowAuction(string auctionId)
        {            
            AuctionInfoViewModel auctionInfoVm = auctionManagement.GetAuctionInfo(auctionId);
            ViewBag.auctionInfoVm = auctionInfoVm;
            return View();
        }        

        public ActionResult CreateOrganizationAndCeo()
        {
            return View(new RegisterOrganizationViewModel());
        }

        [HttpPost]
        public ActionResult CreateOrganizationAndCeo(RegisterOrganizationViewModel organization)
        {
            return View(organization);
        }
    }
}