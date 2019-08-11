using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.WebPages;
using Bloggengine.Models;
using Microsoft.AspNet.Identity;

namespace Bloggengine.Controllers
{
    [Authorize]
    public class BlogsController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();

        // GET: Blogs
        public async Task<ActionResult> Index()
        {
            return View(await db.Blogs.ToListAsync());
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
            ViewBag.LIKE = countlike;
            ViewBag.DISLIKE = countdislike;
            if (blogs == null)
            {
                return HttpNotFound();
            }
            return View(blogs);
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
                     //db.Blogs.Add(blogs);
                    //if (db.SaveChanges() > 0)
                    //  {
                    //      file.SaveAs(path);
                    //      TempData["msg"] = "Blog Created";
                    //      ModelState.Clear();
                    //      return RedirectToAction("Index");
                    //  }
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
        public ActionResult Edit(HttpPostedFileBase file1, [Bind(Include = "Blog_id,Title,Author,Images,Content,Posted,GetLikeCount,GetDislikeCount")] Blogs blogs)
        {
            
            if (ModelState.IsValid)
            {
                if (file1 != null)
                {
                    string filename = Path.GetFileName(file1.FileName);
                    string ext = Path.GetExtension(file1.FileName);
                    string _filename = DateTime.Now.ToString("yymmssff") + filename;
                    string path = Path.Combine(Server.MapPath("~/Image/"), _filename);
                    blogs.Images = "~/Image/" + _filename;
                    
                    if (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg" || ext.ToLower() == ".png")
                    {
                        if (file1.ContentLength <= 1000000)
                        {
                            db.Entry(blogs).State = EntityState.Modified;
                            string oldimg = Request.MapPath(Session["imgpath"].ToString());


                            if (db.SaveChanges() > 0)
                            {

                                if (System.IO.File.Exists(oldimg))
                                {
                                    System.IO.File.Delete(oldimg);
                                }

                                file1.SaveAs(path);
                                ViewBag.Message = "Data Updated";
                                return RedirectToAction("Index");
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

                }
                else
                {
                    //blogs.Images = Session["imgpath"].ToString();
                    db.Entry(blogs).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        ViewBag.Message = "Data Updated";
                        ModelState.Clear();
                        return RedirectToAction("Index");
                    }

                }
            }

            return View();
        }


        [HttpGet]
        public ActionResult Like(int id, int u_id, [Bind(Include = "Like_id,Blog_Id,User_id,isLiked")] Likes Like)
        {   
            //Likes Like = new Likes();
            if (ModelState.IsValid)
            {
                var likeconnn = db.LikeConns.Where(x => x.Blog_Id == id && x.User_id == u_id).First();
                if (likeconnn != null)
                {
                    Like.IsLiked = true;
                    db.Entry(Like).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["msg"] = "Thank you for a like!!";
                    ModelState.Clear();
                    return View("Index");
                }
                else
                {
                    Like.Blog_Id = id;
                    Like.User_id = u_id;
                    Like.IsLiked = true;
                    db.LikeConns.Add(Like);
                    db.SaveChanges();
                    TempData["msg"] = "Thank you for a like!!";
                    ModelState.Clear();
                    return View("Index");
                }
               
            }

            if (Like == null)
            {
                return HttpNotFound();
            }

            return RedirectToAction("BlogPost", id);
        }

        [HttpGet]
        public ActionResult Dislike(int id, int u_id)
        {
            Blogs blogs = new Blogs();
            Likes Like = new Likes();
            if (ModelState.IsValid)
            {
                var likeconnn = db.LikeConns.Where(x => x.Blog_Id == id && x.User_id == u_id).First();
                if (likeconnn != null)
                {
                    Like.IsLiked = false;
                    db.SaveChanges();
                }
                else
                {
                    Like.Blog_Id = id;
                    Like.User_id = u_id;
                    Like.IsLiked = false;
                    db.LikeConns.Add(Like);
                }
                
                if (db.SaveChanges() > 0)
                {
                    TempData["msg"] = "Thank you!!";
                    ModelState.Clear();
                    return RedirectToAction("BlogPost", id);
                }
            }

            if (Like == null)
            {
                return HttpNotFound();
            }

            return View();
        }
        //[HttpGet]
        //public ActionResult GetDisLikeCount(int? id)
        //{
        //    return View();
        //}



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
