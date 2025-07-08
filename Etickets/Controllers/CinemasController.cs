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
    public class CinemasController : Controller
    {
        private eTicketsEntities db = new eTicketsEntities();
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index(string searchString)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "Cinema");
            }

            var Cinemas = db.Cinemas.ToList();
            // Filter movies berdasarkan searchString
            if (!String.IsNullOrEmpty(searchString))
            {
                Cinemas = Cinemas.Where(u => u.Name.Contains(searchString)).ToList();
            }

            return View(Cinemas);
        }

        // GET: Cinemas/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "Cinema");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cinema cinema = db.Cinemas.Find(id);
            if (cinema == null)
            {
                return HttpNotFound();
            }
            return View(cinema);
        }

        // GET: Cinemas/Create
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "Cinema");
            }
            return View();
        }

        // POST: Cinemas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cinema cinema, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    ImageFile.SaveAs(path);
                    cinema.Logo = "~/Content/Images/" + fileName;
                }

                try
                {
                    db.Cinemas.Add(cinema);
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

            return View(cinema);
        }

        // GET: Cinemas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "Cinema");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cinema cinema = db.Cinemas.Find(id);
            if (cinema == null)
            {
                return HttpNotFound();
            }
            return View(cinema);
        }

        // POST: Cinemas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cinema cinema, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    ImageFile.SaveAs(path);
                    cinema.Logo = "~/Content/Images/" + fileName;
                }

                try
                {
                    db.Entry(cinema).State = EntityState.Modified;
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
            return View(cinema);
        }

        // GET: Cinemas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "Cinema");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cinema cinema = db.Cinemas.Find(id);
            if (cinema == null)
            {
                return HttpNotFound();
            }
            return View(cinema);
        }

        // POST: Cinemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cinema cinema = db.Cinemas.Find(id);
            db.Cinemas.Remove(cinema);
            db.SaveChanges();
            return RedirectToAction("Index");
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
            var rptfile = "CinemaReport"; var rptname = "CinemaReport";

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
