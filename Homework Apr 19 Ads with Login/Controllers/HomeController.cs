using AdWithLogin_data;
using Homework_Apr_19_Ads_with_Login.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Homework_Apr_19_Ads_with_Login.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=AdsWithLogin;Integrated Security=true;";


        public IActionResult Index()
        {
            Database db = new Database(_connectionString);

            IndexPageViewModel vm = new()
            {
                ads = db.GetAds()
            };

            if(User.Identity.IsAuthenticated)
            {
                var currentUserEmail = User.Identity.Name;
                var user = db.GetByEmail(currentUserEmail);

                vm.UserAdIds = db.GetUserIds(user.Id);
                vm.user = user;
            }

            
            if(vm.UserAdIds == null)
            {
                vm.UserAdIds = new List<int>();
            }
            if(vm.ads == null)
            {
                vm.ads = new List<AdWithPersonInfo>();
            }
            if(vm.user == null)
            {
                vm.user=new Person();
            }
            

            return View(vm);
        }
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(Person p)
        {
            Database db = new Database(_connectionString);
            db.AddPerson(p);
            return Redirect("/home/login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var db = new Database(_connectionString);
            var user = db.Login(email, password);

            if (user == null)
            {
                TempData["message"] = "Invalid Login";
                return Redirect("/account/login");
            }

            //this code logs in the current user
            var claims = new List<Claim>
            {
                new Claim("person", email) //this will store the users email into the login cookie
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "person", "role")))
                .Wait();

            return Redirect("/home/index");
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            Database db = new Database(_connectionString);
            ad.DateCreated = DateTime.Now;
            var userEmail = User.Identity.Name;
            var user = db.GetByEmail(userEmail);
            db.NewAd(ad, user.Id);
            return Redirect("/home/index");
        }

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/home/index");
        }

        public IActionResult Delete(int id)
        {
            Database db = new Database(_connectionString);
            var userEmail = User.Identity.Name;
            var user = db.GetByEmail(userEmail);
            db.Delete(id, user.Id);
            return Redirect("/home/index");
        }

        [Authorize]
        public IActionResult MyAds()
        {
            Database db = new Database(_connectionString);
            var userEmail = User.Identity.Name;
            var user = db.GetByEmail(userEmail);
            List<Ad> ads = db.GetUserAds(user.Id);
            return View(ads);
        }
    }
}