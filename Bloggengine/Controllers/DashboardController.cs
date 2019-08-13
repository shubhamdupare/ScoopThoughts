using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bloggengine.Models;

namespace Bloggengine.Controllers
{
    public class DashboardController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();

        // GET: Dashboard
        public async Task<ActionResult> Index()
        {
            var user_id = db.Accounts.Where(x => x.UserName == User.Identity.Name).Select(i => i.Id).FirstOrDefault();
            ViewBag.u_id = user_id;
            return View(await db.Blogs.ToListAsync());
        }
        
    }
}
