using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Etickets.Models;

namespace Etickets.Controllers
{
    public class MovieCinemasController : Controller
    {
        private eTicketsEntities db = new eTicketsEntities();

        // GET: MovieCinemas
        public ActionResult Index()
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
            var movieCinemas = db.MovieCinemas.Include(m => m.Cinema).Include(m => m.movie);
            return View(movieCinemas.ToList());
        }

        // GET: MovieCinemas/Details/5
        public ActionResult Details(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieCinema movieCinema = db.MovieCinemas.Find(id);
            if (movieCinema == null)
            {
                return HttpNotFound();
            }
            return View(movieCinema);
        }

        // GET: MovieCinemas/Create
        public ActionResult Create()
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
            ViewBag.IdCinema = new SelectList(db.Cinemas, "Id", "Name");
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name");
            return View();
        }

        // POST: MovieCinemas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdMovie,IdCinema")] MovieCinema movieCinema)
        {
            if (ModelState.IsValid)
            {
                db.MovieCinemas.Add(movieCinema);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdCinema = new SelectList(db.Cinemas, "Id", "Name", movieCinema.IdCinema);
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", movieCinema.IdMovie);
            return View(movieCinema);
        }

        // GET: MovieCinemas/Edit/5
        public ActionResult Edit(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieCinema movieCinema = db.MovieCinemas.Find(id);
            if (movieCinema == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCinema = new SelectList(db.Cinemas, "Id", "Name", movieCinema.IdCinema);
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", movieCinema.IdMovie);
            return View(movieCinema);
        }

        // POST: MovieCinemas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdMovie,IdCinema")] MovieCinema movieCinema)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movieCinema).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdCinema = new SelectList(db.Cinemas, "Id", "Name", movieCinema.IdCinema);
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", movieCinema.IdMovie);
            return View(movieCinema);
        }

        // GET: MovieCinemas/Delete/5
        public ActionResult Delete(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieCinema movieCinema = db.MovieCinemas.Find(id);
            if (movieCinema == null)
            {
                return HttpNotFound();
            }
            return View(movieCinema);
        }

        // POST: MovieCinemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovieCinema movieCinema = db.MovieCinemas.Find(id);
            db.MovieCinemas.Remove(movieCinema);
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
