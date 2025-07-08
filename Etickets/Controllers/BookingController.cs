using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Etickets.Models;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using static Etickets.GlobalClass;
using static Etickets.GlobalFunctions;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace Etickets.Controllers
{
    public class BookingController : Controller
    {
        private eTicketsEntities db = new eTicketsEntities();

        // GET: Booking
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            var bookings = db.Bookings.Include(b => b.movie);
            return View(bookings.ToList());
        }

        // GET: Booking/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
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

        // GET: Booking/Create
        public ActionResult Create(int idMovie)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }

            // Buat objek Booking dan isi propertinya dengan nilai default
            var booking = new Booking
            {
                IdMovie = idMovie,
                IdUser = (int)Session["UserId"]
            };

            return View(booking);
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Booking booking)
        {
            // mencari record Movie yang sesuai dengan IdMovie pada Booking
            var movie = db.movies.Find(booking.IdMovie);

            // validasi jika jumlah tiket yang dibooking melebihi sisa tiket yang tersedia
            if (movie.TicketsQuota - movie.TicketsBooked < booking.Tickets)
            {
                ModelState.AddModelError("Tickets", "The number of tickets you are trying to book exceeds the available quota.");
            }

            //// validasi jika FilmDate booking berada dalam rentang waktu StartDate dan EndDate film
            //if (booking.FilmDate < movie.StartDate || booking.FilmDate > movie.EndDate)
            //{
            //    ModelState.AddModelError("FilmDate", "The selected film date is not within the screening period.");
            //}

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

            return View(booking);
        }





        public ActionResult RatingReview(int id)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
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
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", booking.IdMovie);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RatingReview(int id, FormCollection form)
        {
            var booking = db.Bookings.Find(id);

            if (TryUpdateModel(booking))
            {
                booking.Rating = Convert.ToInt32(form["selected_rating"]);
                booking.Review = form["Review"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(booking);
        }


        // proses pembayaran
        [HttpPost]
        public async Task<ActionResult> Pay(int id)
        {
            string SERVER_KEY = "SB-Mid-server-v_NttAn3Ia-yNu7LEl4viJj-";
            string CLIENT_KEY = "SB-Mid-client-QZun0ob3_0XTeuc9";
        // ambil data pemesanan dari database
        Booking booking = new Booking();
            using (var db = new eTicketsEntities())
            {
                booking = db.Bookings.SingleOrDefault(x => x.Id == id);
            }

            // siapkan data transaksi untuk Midtrans Payment Gateway
            string transactionDetails = @"
            {
                ""transaction_details"": {
                    ""order_id"": """ + booking.Id + @""",
                    ""gross_amount"": "/* + booking.TotalPrice */+ @"
                },
                ""credit_card"": {
                    ""secure"": true
                }
            }";

            // buat request ke Midtrans Payment Gateway
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://app.sandbox.midtrans.com");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(SERVER_KEY + ":")));

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("server_key", SERVER_KEY),
                new KeyValuePair<string, string>("client_key", CLIENT_KEY),
                new KeyValuePair<string, string>("transaction_details", transactionDetails)
            });

            HttpResponseMessage response = await client.PostAsync("/snap/v1/transactions", content);
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // redirect ke Midtrans Payment Gateway untuk pembayaran
            return Redirect(responseString);
        }
    public ActionResult Pay()
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            return View();
        }


        public ActionResult Ticket(int id)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            string rptname = "Ticket";
            //ids = ClassFunction.Left(ids, ids.Length - 1);

            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Report"), rptname + ".rpt"));
            //report.SetParameterValue("UserID", Session["UserID"].ToString());
            report.SetParameterValue("sWhere", " WHERE Booking.Id IN(" + id + ")");
            ClassProcedure.SetDBLogonForReport(report);
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            report.Close(); report.Dispose();
            //return File(stream, "application/pdf", "SOMaterial.pdf");
            Response.AppendHeader("content-disposition", "inline; filename=" + rptname + ".pdf");
            return new FileStreamResult(stream, "application/pdf");
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
