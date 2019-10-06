using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AB_invest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AB_invest.Controllers
{
    public class TradingController : Controller
    {
        private Context db;

        public TradingController(Context context)
        {
            db = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(string shareId, string type)
        {
            var account = db.Accounts
                .Include(q => q.Balances)
                .Where(q => q.Id == int.Parse(User.Identity.Name))
                .FirstOrDefault();

            AccountType t = 0;

            if (type == null || type == "")
                t = account.GetDefaultType();
            else
                t = (AccountType)int.Parse(type);
            ViewBag.Type = t;

            // Nullable
            ViewBag.MyShare = db.ShareBalances
                .Include(q => q.Share)
                .Where(q => q.AccountId == int.Parse(User.Identity.Name) && q.ShareId == int.Parse(shareId) && q.Type == Account.GetByteByType(t))
                .FirstOrDefault();

            ViewBag.Share = db.Shares
                .Include(q => q.Company)
                .Include(q => q.Currency)
                .Where(q => q.Id == int.Parse(shareId))
                .FirstOrDefault();

            ViewBag.Account = account;

            return View();
        }

        // Buy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Type,ShareId,Amount")] ShareBalance newSellerShareBalance)
        {
            if (ModelState.IsValid && newSellerShareBalance.Amount > 0)
            {
                var share = db.Shares
                    .Include(q => q.Company)
                    .Where(q => q.Id == newSellerShareBalance.ShareId)
                    .FirstOrDefault();

                if (await db.ShareOperation(

                    new UserInfo { Id = int.Parse(User.Identity.Name), Type = Account.GetByteByType((AccountType)newSellerShareBalance.Type) },
                    new UserInfo { Id = share.Company.AccountId, Type = Account.GetByteByType(AccountType.company) },
                    newSellerShareBalance))
                    return RedirectToAction(nameof(Index), new { shareId = newSellerShareBalance.ShareId, type = newSellerShareBalance.Type });
            }
            return RedirectToAction(nameof(Index), new { shareId = newSellerShareBalance.ShareId, type = newSellerShareBalance.Type });
        }
    }
}