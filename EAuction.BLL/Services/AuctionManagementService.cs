using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EAuction.BLL.Services
{
    public class AuctionManagementService
    {
        //private readonly AplicationDbContext _aplicationDbContext;
        //public void OpenAuction(
        //    OpenAuctionRequestVm model,
        //    int organizationId)
        //{
        //    if (model == null)
        //        throw new ArgumentNullException($"{typeof(OpenAuctionRequestVm).Name} is null");

        //    int maximumAllowedActiveAuctions = 3;

        //    var auctionsCheck = _aplicationDbContext
        //        .Organizations
        //        .Find(organizationId)
        //        .Auctions
        //        .Where(p => p.AuctionStatus == AuctionStatus.Active)
        //        .Count() < maximumAllowedActiveAuctions;

        //    var categoryCheck = _aplicationDbContext.AuctionCategories
        //        .SingleOrDefault(p => p.Name == model.AuctionCategory);

        //    if (categoryCheck == null)
        //        throw new Exception("Ошибка валидации модели!");

        //    if (!auctionsCheck)
        //        throw new OpenAuctionProcessException(model, "Превышено максимальное количество активных аукционов!");

        //    var auctionModel = Mapper.Map<Auction>(model);
        //    auctionModel.AuctionStatus = AuctionStatus.Active;
        //    auctionModel.Category = categoryCheck;
        //    auctionModel.OrganizationId = organizationId;
        //    _aplicationDbContext.Auctions.Add(auctionModel);
        //    _aplicationDbContext.SaveChanges();

        //}

        //public void MakeBidToAuction(MakeBidVm model)
        //{
        //    var bidExists = _aplicationDbContext.Bids
        //        .Any(p => p.Price == model.Price &&
        //        p.AuctionId == model.AuctionId &&
        //        p.Description == model.Description &&
        //        p.OrganizationId == model.OrganizationId);

        //    if (bidExists)
        //        throw new Exception("Invalid bid");

        //    var inValidPriceRange = _aplicationDbContext
        //        .Auctions.Where(p => p.Id == model.AuctionId &&
        //        p.PriceAtMinimum < model.Price &&
        //        p.PriceAtStart > model.Price);

        //    var inStepRange = inValidPriceRange
        //        .Any(p => (p.PriceAtStart - model.Price) % p.PriceChangeStep == 0);

        //    if (!inStepRange)
        //        throw new Exception("Invalid bid according price step");

        //    Bid bid = new Bid()
        //    {
        //        Price = model.Price,
        //        Description = model.Description,
        //        AuctionId = model.AuctionId,
        //        OrganizationId = model.OrganizationId,
        //        CreatedDate = DateTime.Now
        //    };
        //    _aplicationDbContext.Bids.Add(bid);
        //    _aplicationDbContext.SaveChanges();

        //}

        //public void RevokeBidFromAuction()
        //{

        //}

        //public void GetAuctionInfo()
        //{

        //}

        //public void ElectWinnerInAuction()
        //{

        //}

        //public AuctionManagementService()
        //{
        //    _aplicationDbContext = new AplicationDbContext();
        //}
    }
}