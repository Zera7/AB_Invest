using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AB_invest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace AB_invest.Controllers
{
    public class SharesController : Controller
    {
        private Context db;

        public SharesController(Context context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index(string orderBy, string sortOrder)
        {
            if (string.IsNullOrEmpty(orderBy))
                orderBy = "company";

            if (sortOrder == null)
                sortOrder = "";

            var sortOptions = ($"{orderBy} {sortOrder}").Trim();

            var shares = db.Shares
                .Include(q => q.Company)
                .Where(q => q.Status == ShareModerateStatus.Accepted)
                .OrderBy(sortOptions);

            ViewBag.OrderBy = orderBy;
            ViewBag.SortOrder = sortOrder;

            return View(shares);
        }

        public IActionResult AddShare()
        {
            ViewBag.Currency = db.Currency
                .Select(q => new SelectListItem() { Text = q.Name, Value = q.Id.ToString() })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddShare([Bind("Name, Dividends, CurrencyId, Cost, Frequency")] Share share)
        {
            if (ModelState.IsValid)
            {
                if (share.Name != "" && share.Name != null)
                {
                    var company = db.CompanyData
                        .Where(q => q.AccountId == int.Parse(User.Identity.Name))
                        .FirstOrDefault();

                    share.CompanyDataId = company.Id;
                    db.Shares.Add(share);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(ProfileController.Index), "Profile");
                }
            }
            ViewBag.Currency = db.Currency
                .Select(q => new SelectListItem() { Text = q.Name, Value = q.Id.ToString() })
                .ToList();

            return View(share);
        }

        [HttpGet]
        public IActionResult RequestShareVerification(string shareId)
        {
            var share = db.Shares
                .Where(q => q.Id == int.Parse(shareId) && q.Company.AccountId == int.Parse(User.Identity.Name))
                .FirstOrDefault();
            if (share != null)
            {
                share.Status = ShareModerateStatus.Verification;
                db.Update(share);
                db.SaveChanges();
            }
            return RedirectToAction(nameof(ProfileController.Index), "Profile", new { type = AccountType.company.ToString() });
        }

        [HttpGet]
        public IActionResult IssueShares(string shareId)
        {
            var share = db.Shares
                .Include(q => q.Currency)
                .Where(q => q.Id == int.Parse(shareId))
                .FirstOrDefault();

            ViewBag.Share = share;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueShares([Bind("ShareId,Amount")]ShareBalance balance)
        {
            if (ModelState.IsValid)
            {
                var account = db.Accounts
                    .Where(q => q.Id == int.Parse(User.Identity.Name))
                    .FirstOrDefault();

                var shareBalance = db.ShareBalances
                    .Where(q => q.Account.Id == account.Id && q.Type == Account.GetByteByType(AccountType.company))
                    .FirstOrDefault();

                if (shareBalance == null)
                {
                    balance.AccountId = account.Id;
                    balance.Type = 2;
                    db.ShareBalances.Add(balance);
                }
                else
                {
                    shareBalance.Amount += balance.Amount;
                    db.Update(shareBalance);
                }
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(ProfileController.Index), "Profile", new { type = AccountType.company.ToString() });
            }
            return View(balance);
        }
    }
}