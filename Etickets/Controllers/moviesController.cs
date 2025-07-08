using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Etickets.Models;
using System.IO;
using System.Data.Entity.Validation;
using static Etickets.GlobalClass;
using static Etickets.GlobalFunctions;

namespace Etickets.Controllers
{
    public class moviesController : Controller
    {
        private eTicketsEntities db = new eTicketsEntities();

        [AcceptVerbs(HttpVerbs.Get|HttpVerbs.Post)]
        public ActionResult Index(string searchString)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login atau bukan Admin, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }

            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "movie");
            }

            IQueryable<movie> movies = db.movies;

            // Filter movies berdasarkan searchString
            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m => m.Name.Contains(searchString));
            }

            return View(movies.ToList());
        }

        public ActionResult Report()
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login atau bukan Admin, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }

            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "movie");
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult ShowReport(string reporttype)
        {
            var rptfile = "MovieReport"; var rptname = "MovieReport";

            rptfile = rptfile.Replace("PDF", "");


            var sWhere = "WHERE 1=1 ";
            

            var irequest = new ReportRequest()
            {
                rptQuery = "",
                rptFile = rptfile,
                rptPaperSizeEnum = 14,
                rptPaperOrientationEnum = 2,
                rptExportType = reporttype,
                //rptParam = new Dictionary<string, string>()
            };
            //irequest.rptParam.Add("Periode", param.periodstart.ToDateTime().ToString("dd/MM/yyyy") + " - " + param.periodend.ToDateTime().ToString("dd/MM/yyyy"));
            //irequest.rptParam.Add("UserID", Session["UserID"].ToString());
            //irequest.rptParam.Add("sWhere", sWhere);

            var rpt_id = GenerateReport(irequest);

            return Json(new { rptname, rpt_id }, JsonRequestBehavior.AllowGet);
        }

        // GET: movies/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "movie");

            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            movie movie = db.movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: movies/Create
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "movie");

            }
            ViewBag.IdActor = new SelectList(db.Actors, "Id", "id");
            ViewBag.IdCinema = new SelectList(db.Cinemas, "Id", "id");
            ViewBag.IdProducer = new SelectList(db.Producers, "Id", "id");
            return View();
        }

        // POST: movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(movie movie, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    ImageFile.SaveAs(path);
                    movie.ImageURL = "~/Content/Images/" + fileName;
                }

                try
                {
                    db.movies.Add(movie);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var error in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in error.ValidationErrors)
                        {
                            ModelState.AddModelError("", "Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }
            }

            //ViewBag.IdActor = new SelectList(db.Actors, "Id", "ProfilePictureURL", movie.IdActor);
            //ViewBag.IdCinema = new SelectList(db.Cinemas, "Id", "Logo", movie.IdCinema);
            //ViewBag.IdProducer = new SelectList(db.Producers, "Id", "ProfilePictureURL", movie.IdProducer);
            return View(movie);
        }


        // GET: movies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "movie");

            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            movie movie = db.movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            //ViewBag.IdActor = new SelectList(db.Actors, "Id", "Id", movie.IdActor);
            //ViewBag.IdCinema = new SelectList(db.Cinemas, "Id", "Id", movie.IdCinema);
            //ViewBag.IdProducer = new SelectList(db.Producers, "Id", "Id", movie.IdProducer);
            return View(movie);
        }

        // POST: movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(movie movie, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    ImageFile.SaveAs(path);
                    movie.ImageURL = "~/Content/Images/" + fileName;
                }

                try
                {
                    db.Entry(movie).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var error in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in error.ValidationErrors)
                        {
                            ModelState.AddModelError("", "Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }
            }

            //ViewBag.IdActor = new SelectList(db.Actors, "Id", "ProfilePictureURL", movie.IdActor);
            //ViewBag.IdCinema = new SelectList(db.Cinemas, "Id", "Logo", movie.IdCinema);
            //ViewBag.IdProducer = new SelectList(db.Producers, "Id", "ProfilePictureURL", movie.IdProducer);
            return View(movie);
        }



        // GET: movies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "movie");

            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            movie movie = db.movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            movie movie = db.movies.Find(id);
            db.movies.Remove(movie);
            db.SaveChanges();
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
