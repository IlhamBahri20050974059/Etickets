using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Etickets.Models;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using CrystalDecisions.Shared;
using static Etickets.GlobalClass;
using static Etickets.GlobalFunctions;

namespace Etickets.Controllers
{
    public class BookingsController : Controller
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
                return RedirectToAction("Index", "Booking");

            }
            var Bookings = db.Bookings.ToList();
            // Filter movies berdasarkan searchString
            if (!String.IsNullOrEmpty(searchString))
            {
                Bookings = Bookings.Where(b => b.Id.ToString().Contains(searchString)).ToList();
            }

            return View(Bookings);
        }
       

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "Booking");

            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "Booking");

            }
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name");
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View(new Booking());
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdUser,IdMovie,FilmDate,Tickets")] Booking booking)
        {
            // mencari record Movie yang sesuai dengan IdMovie pada Booking
            var movie = db.movies.Find(booking.IdMovie);

            // validasi jika jumlah tiket yang dibooking melebihi sisa tiket yang tersedia
            if (movie.TicketsQuota - movie.TicketsBooked < booking.Tickets)
            {
                ModelState.AddModelError("Tickets", "The number of tickets you are trying to book exceeds the available quota.");
            }

            // validasi jika FilmDate booking berada dalam rentang waktu StartDate dan EndDate film
            if (booking.FilmDate < movie.StartDate || booking.FilmDate > movie.EndDate)
            {
                ModelState.AddModelError("FilmDate", "The selected film date is not within the screening period.");
            }

            // validasi jika model state valid
            if (ModelState.IsValid)
            {
                booking.BookingDate = DateTime.Now;

                var price = movie.Price;
                booking.TotalPrice = price * booking.Tickets;

                movie.TicketsBooked += booking.Tickets; // menambahkan jumlah TicketsBooked sesuai dengan jumlah tiket yang dibooking

                db.Bookings.Add(booking);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", booking.IdMovie);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", booking.IdUser);
            return View(booking);
        }

        //public ActionResult PrintReport()
        //{
        //    if (Session["UserId"] == null)
        //    {
        //        // Jika belum login, redirect ke halaman login
        //        return RedirectToAction("Login", "Users");
        //    }
        //    if ((string)Session["UserRole"] != "Admin")
        //    {
        //        return RedirectToAction("Index", "Booking");

        //    }
        //    // buat instance report
        //    ReportDocument report = new ReportDocument();

        //    // set path file .rpt
        //    string rptPath = Path.Combine(Server.MapPath("~/Report"), "BookingReport.rpt");
        //    report.Load(rptPath);

        //    // set sumber data
        //    report.SetDataSource(db.Bookings.ToList());

        //    // tampilkan report dalam bentuk PDF
        //    Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
        //    return File(stream, "application/pdf");
        //}



        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "Booking");

            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", booking.IdUser);
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", booking.IdMovie);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdUser,IdMovie,BookingDate,FilmDate,Tickets,TotalPrice")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                var price = db.movies.Where(m => m.Id == booking.IdMovie).Select(m => m.Price).FirstOrDefault();
                booking.TotalPrice = price * booking.Tickets;

                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", booking.IdUser);
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", booking.IdMovie);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index", "Booking");

            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        

        #region Report
        public class FilterReport
        {
            public string periodstart { get; set; }
            public string periodend { get; set; }
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
        public ActionResult ShowReport(FilterReport param,string reporttype)
        {
            var rptfile = "BookingReport"; var rptname = "BookingReport";

            rptfile = rptfile.Replace("PDF", "");


            var sWhere = "WHERE 1=1 ";
            if (!string.IsNullOrEmpty(param.periodstart) && !string.IsNullOrEmpty(param.periodend))
                sWhere += $" AND Booking.BookingDate>=CAST('{param.periodstart} 00:00:00' AS DATETIME) AND Booking.BookingDate<=CAST('{param.periodend} 23:59:59' AS DATETIME)";

            var sParam = new Dictionary<string, string>();
            sParam.Add("sWhere", sWhere);

            var irequest = new ReportRequest()
            {
                rptQuery = "",
                rptFile = rptfile,
                rptPaperSizeEnum = 14,
                rptPaperOrientationEnum = 2,
                rptExportType = reporttype,
               rptParam = sParam
            };
            //irequest.rptParam.Add("Periode", param.periodstart.ToDateTime().ToString("dd/MM/yyyy") + " - " + param.periodend.ToDateTime().ToString("dd/MM/yyyy"));
            //irequest.rptParam.Add("UserID", Session["UserID"].ToString());
            //irequest.rptParam.Add("sWhere", sWhere);

            var rpt_id = GenerateReport(irequest);

            return Json(new { rptname, rpt_id }, JsonRequestBehavior.AllowGet);
        }

        #endregion
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
