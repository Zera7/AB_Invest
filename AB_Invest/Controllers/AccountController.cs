using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AB_invest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace AB_invest.Controllers
{
    public class AccountController : Controller
    {
        private Context db;

        public AccountController(Context context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LogInData data)
        {
            if (ModelState.IsValid)
            {
                var account = db.Accounts
                    .Where(q => q.Phone == data.Phone && q.Password == data.Password)
                    .FirstOrDefault();
                if (account != null)
                    await Authenticate(account.Id); // аутентификация
                return RedirectToAction("Index", "Main");
            }
            return View(data);
        }

        async public Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Main");
        }

        private async Task Authenticate(int userID)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userID.ToString())
            };

            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("Type,Phone,Password,UserData")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.RegistrationDate = DateTime.Now;
                account.Type += 1;

                //Обогатить юзера
                var cur = db.Currency
                    .FirstOrDefault();
                if (cur != null)
                    if (account.Balances == null)
                        account.Balances = new List<CurrencyBalance>();
                    account.Balances.Add(new CurrencyBalance {
                        Account = account,
                        Amount = 10000,
                        Currency = cur,
                        Type = Account.GetByteByType(AccountType.user),
                    });
                db.Accounts.Add(account);
                await db.SaveChangesAsync();
                await Authenticate(account.Id); // аутентификация
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        public IActionResult CreateCompany()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompany([Bind("Type,Phone,Password,CompanyData")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.RegistrationDate = DateTime.Now;
                account.Type += 2;
                db.Accounts.Add(account);
                await db.SaveChangesAsync();
                await Authenticate(account.Id); // аутентификация
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }
    }
}