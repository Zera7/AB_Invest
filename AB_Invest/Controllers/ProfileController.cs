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
    public class ProfileController : Controller
    {
        private Context db;

        public ProfileController(Context context)
        {
            db = context;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index(string type)
        {
            Account account = account = db.Accounts
                .Include(q => q.UserData)
                .Include(q => q.CompanyData)
                .Where(q => q.Id.ToString() == User.Identity.Name)
                .FirstOrDefault();

            var typeIsValid = Enum.TryParse(type, out AccountType accountType);

            // AccountType
            if (typeIsValid && account.HasSelectedType(accountType))
                ViewBag.AccountType = accountType;
            else
            {
                ViewBag.AccountType = account.GetDefaultType();
                accountType = ViewBag.AccountType;
            }

            // MinimalAccountData
            var minimalAccountData = new MinimalAccountData(account);

            ViewBag.MinimalAccountData = minimalAccountData;

            // AccountData
            // SharesData
            if (accountType == AccountType.user)
            {
                ViewBag.AccountData = account.UserData;

                var shareBalances = db.ShareBalances
                    .Include(q => q.Share.Company)
                    .Where(q => q.Account.Id == account.Id && q.Type == Account.GetByteByType(accountType))
                    .ToList();

                ViewBag.Shares = shareBalances;
            }
            else if (accountType == AccountType.company)
            {
                ViewBag.AccountData = account.CompanyData;

                var shares = db.Shares
                    .Where(q => q.CompanyDataId == account.CompanyData.Id)
                    .ToList();

                ViewBag.Shares = shares;
            }

            // Currency Balances
            var currencyBalances = db.CurrencyBalances
                .Include(q => q.Currency)
                .Where(q => q.AccountId == account.Id && q.Type == Account.GetByteByType(accountType))
                .ToList();

            ViewBag.CurrencyBalances = currencyBalances;

            return View();
        }
    }
}