using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bloggengine.Models;
using PagedList;
using PagedList.Mvc;

namespace Bloggengine.Controllers
{
    [Authorize]
    public class BlogsController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();

        public ActionResult Index(int? page)
        {
            return View(db.Blogs.ToList().ToPagedList(page ?? 1, 4));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> BlogPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blogs blogs = await db.Blogs.FindAsync(id);
            var countlike = db.LikeConns.Count(x => x.Blog_Id == id && x.IsLiked == true);
            var countdislike = db.LikeConns.Count(x => x.Blog_Id == id && x.IsLiked == false);
            ViewData["LIKE"] = countlike;
            ViewData["DISLIKE"] = countdislike;
            if (blogs == null)
            {
                return HttpNotFound();
            }
            return View(blogs);
        }

        [HttpPost]
        public ActionResult Dislike(int id, int u_id)
        {
            if (ModelState.IsValid)
            {
                var likeconnn = db.LikeConns.Where(x => x.Blog_Id == id && x.User_id == u_id).FirstOrDefault();
                if (likeconnn != null)
                {
                    Likes Likee = db.LikeConns.Where(x => x.User_id == u_id && x.Blog_Id == id).FirstOrDefault();
                    db.LikeConns.Attach(Likee);
                    Likee.Blog_Id = id;
                    Likee.User_id = u_id;
                    Likee.IsLiked = false;
                    db.SaveChanges();
                    TempData["msg"] = "Thank you !!";
                    ModelState.Clear();
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    Likes Likee = new Likes();
                    db.LikeConns.Add(Likee);
                    Likee.Blog_Id = id;
                    Likee.User_id = u_id;
                    Likee.IsLiked = false;
                    
                    
                    db.SaveChanges();
                    TempData["msg"] = "Thank you!!";
                    ModelState.Clear();
                    return RedirectToAction("Index", "Dashboard");
                }

            }

            return RedirectToAction("BlogPost", new { id });
        }

        [HttpPost]
        public ActionResult Like(int id, int u_id)
        {
            if (ModelState.IsValid)
            {
                var likeconnn = db.LikeConns.Where(x => x.Blog_Id == id && x.User_id == u_id).FirstOrDefault();
                if (likeconnn != null)
                {
                    Likes Like = db.LikeConns.Where(x => x.User_id == u_id && x.Blog_Id == id).FirstOrDefault();
                    db.LikeConns.Attach(Like);
                    Like.IsLiked = true;
                    db.SaveChanges();
                    TempData["msg"] = "Thank you for a like!!";
                    ModelState.Clear();
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {

                    Likes Like = new Likes();
                    db.LikeConns.Add(Like);
                    Like.Blog_Id = id;
                    Like.User_id = u_id;
                    Like.IsLiked = true;
                    
                    
                    db.SaveChanges();
                    TempData["msg"] = "Thank you for a like!!";
                    ModelState.Clear();
                    return RedirectToAction("Index", "Dashboard");
                }

            }

            return RedirectToAction("BlogPost", new { id });
        }

        

        // GET: Blogs/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file, [Bind(Include = "Blog_Id,Title,Author,Images,Content,Posted,GetLikeCount,GetDislikeCount")] Blogs blogs)
        {
            string filename = Path.GetFileName(file.FileName);
            string _filename = DateTime.Now.ToString("yymmss") + filename;
            string path = Path.Combine(Server.MapPath("~/Image/"), _filename);
            string ext = Path.GetExtension(file.FileName);

            blogs.Images = "~/Image/" + _filename;
            
            
            if (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg" || ext.ToLower() == ".png")
            {
                if (file.ContentLength <= 1000000)
                {
                    try
                    {
                        db.Blogs.Add(blogs);
                        if (db.SaveChanges() > 0)                           //EntityValidation Errors
                        {
                            file.SaveAs(path);
                            TempData["msg"] = "Blog Created";
                            ModelState.Clear();
                            return RedirectToAction("Index");
                        }
                    }

                    catch (DbEntityValidationException dbex)                   //solution for Entity validation error
                    {
                        foreach (var validationErrors in dbex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.message = "File size must be less than or equal to 1mb";
                }
            }
            else
            {
                ViewBag.message = "Invalid File Type";
            }

            return View();
        }

        // GET: Blogs/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            var blogs = db.Blogs.Find(id);
            Session["imgpath"] = blogs.Images;
            
            if (blogs == null)
            {
                return HttpNotFound();
            }
            return View(blogs);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase file1, [Bind(Include = "Blog_ID,Title,Author,Images,Content,Posted")] Blogs blogs)
        {
            
            if (ModelState.IsValid)
            {
                if (file1 != null)
                {
                    string filename = Path.GetFileName(file1.FileName);
                    string ext = Path.GetExtension(file1.FileName);
                    string _filename = DateTime.Now.ToString("yymmssff") + filename;
                    string path = Path.Combine(Server.MapPath("~/Image/"), _filename);


                    if (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg" || ext.ToLower() == ".png")
                    {
                        if (file1.ContentLength <= 1000000)
                        {
                            db.Entry(blogs).State = EntityState.Modified;
                            blogs.Images = "~/Image/" + _filename;
                            string oldimg = Request.MapPath(Session["imgpath"].ToString());
                            file1.SaveAs(path);
                            db.SaveChanges();
                            if (System.IO.File.Exists(oldimg))
                            {
                                System.IO.File.Delete(oldimg);
                            }

                            ViewBag.Message = "Data Updated";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ViewBag.message = "File size must be less than or equal to 1mb";
                        }
                    }
                    else
                    {
                        ViewBag.message = "Invalid File Type";
                    }

                }
                else
                {
                    blogs.Images = Session["imgpath"].ToString();
                    db.Entry(blogs).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        ViewBag.Message = "Data Updated";
                        //ModelState.Clear();
                        return RedirectToAction("Index");
                    }

                }
            }


            return View();
        }
    
    


        

        // GET: Blogs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blogs blogs = await db.Blogs.FindAsync(id);
            if (blogs == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Blogs blogs = await db.Blogs.FindAsync(id);
            string path = Request.MapPath(blogs.Images);
            db.Entry(blogs).State = EntityState.Deleted;
            if (db.SaveChanges() > 0)
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

            }
            TempData["msg"] = "Data Deleted";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
