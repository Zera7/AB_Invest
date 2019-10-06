using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AB_invest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AB_invest.Controllers
{
    public class AdminController : Controller
    {
        private Context db;

        public AdminController(Context context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            var a = db.Currency.ToList();
            return View(db.Currency.ToList());
        }

        public IActionResult AddCurrency()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCurrency([Bind("Name, Rate")] Currency currency)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(currency.Name))
                {
                    db.Currency.Add(currency);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(currency);
        }

        public IActionResult SharesToModerate()
        {
            ViewBag.Shares = db.Shares
                .Include(q => q.Company)
                .Include(q => q.Currency)
                .Where(q => q.Status == ShareModerateStatus.Verification);

            return View();
        }

        [HttpGet]
        public IActionResult ModerateShare(string shareId)
        {
            var share = db.Shares
                .Include(q => q.Company)
                .Include(q => q.Currency)
                .Where(q => q.Id == int.Parse(shareId))
                .FirstOrDefault();
            return View(share);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModerateShare([Bind("Id,MarkupPercent")]Share share)
        {
            if (ModelState.IsValid)
            {
                var newShare = db.Shares
                    .Where(q => q.Id == share.Id)
                    .FirstOrDefault();
                newShare.MarkupPercent = share.MarkupPercent;
                newShare.Status = ShareModerateStatus.Accepted;

                db.Update(newShare);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(share);
        }

        [HttpGet]
        public IActionResult RejectShare(string shareId)
        {
            var share = db.Shares
                .Where(q => q.Id == int.Parse(shareId))
                .FirstOrDefault();

            share.Status = ShareModerateStatus.Rejected;
            db.Update(share);
            db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}