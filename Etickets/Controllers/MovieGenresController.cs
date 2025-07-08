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
    public class MovieGenresController : Controller
    {
        private eTicketsEntities db = new eTicketsEntities();

        // GET: MovieGenres
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
            var movieGenres = db.MovieGenres.Include(m => m.Genre).Include(m => m.movie);
            return View(movieGenres.ToList());
        }

        // GET: MovieGenres/Details/5
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
            MovieGenre movieGenre = db.MovieGenres.Find(id);
            if (movieGenre == null)
            {
                return HttpNotFound();
            }
            return View(movieGenre);
        }

        // GET: MovieGenres/Create
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
            ViewBag.IdGenre = new SelectList(db.Genres, "Id", "Name");
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name");
            return View();
        }

        // POST: MovieGenres/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdMovie,IdGenre")] MovieGenre movieGenre)
        {
            if (ModelState.IsValid)
            {
                db.MovieGenres.Add(movieGenre);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdGenre = new SelectList(db.Genres, "Id", "Name", movieGenre.IdGenre);
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", movieGenre.IdMovie);
            return View(movieGenre);
        }

        // GET: MovieGenres/Edit/5
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
            MovieGenre movieGenre = db.MovieGenres.Find(id);
            if (movieGenre == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdGenre = new SelectList(db.Genres, "Id", "Name", movieGenre.IdGenre);
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", movieGenre.IdMovie);
            return View(movieGenre);
        }

        // POST: MovieGenres/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdMovie,IdGenre")] MovieGenre movieGenre)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movieGenre).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdGenre = new SelectList(db.Genres, "Id", "Name", movieGenre.IdGenre);
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", movieGenre.IdMovie);
            return View(movieGenre);
        }

        // GET: MovieGenres/Delete/5
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
            MovieGenre movieGenre = db.MovieGenres.Find(id);
            if (movieGenre == null)
            {
                return HttpNotFound();
            }
            return View(movieGenre);
        }

        // POST: MovieGenres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovieGenre movieGenre = db.MovieGenres.Find(id);
            db.MovieGenres.Remove(movieGenre);
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
