using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using Bloggengine.Models;

namespace Bloggengine.Controllers
{
    public class AccountController : Controller
    {
        MyDbContext db = new MyDbContext();

      
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Account accountmodel)
        {
            if (db.Accounts.Any(x => x.UserName == accountmodel.UserName))
            {
                ModelState.AddModelError("UserName","Username is already present");
            }
            if (db.Accounts.Any(x => x.Email == accountmodel.Email))
            {
                ModelState.AddModelError("Email", "Email is already present");
            }
            if (ModelState.IsValid)
            {
                    db.Accounts.Add(accountmodel);
                    db.SaveChanges();  
                ModelState.Clear();
                ViewBag.Message = accountmodel.F_Name + " " + accountmodel.L_Name + "Successfully Registered.";
            }
            return View();
        }

        
        public ActionResult Login()
        {
            return View();
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Account accountmodel)
        {
            try
            {
                var usr = db.Accounts.Where(u => u.UserName == accountmodel.UserName && u.Password == accountmodel.Password).First();
                {
                    
                        FormsAuthentication.SetAuthCookie(accountmodel.UserName, false);
                        ViewBag.success = "Login Successfull";
                        return RedirectToAction("Index", "Dashboard");
                   
                }
            }

            catch(Exception)
            {
                ViewBag.Message = "Invalid credentials please insert proper credentials.";
            }

            return View(accountmodel);
        }

      
       
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Dashboard");
        }

        
    }
}