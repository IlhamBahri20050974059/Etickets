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
    public class MovieProducersController : Controller
    {
        private eTicketsEntities db = new eTicketsEntities();

        // GET: MovieProducers
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
            var movieProducers = db.MovieProducers.Include(m => m.movie).Include(m => m.Producer);
            return View(movieProducers.ToList());
        }

        // GET: MovieProducers/Details/5
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
            MovieProducer movieProducer = db.MovieProducers.Find(id);
            if (movieProducer == null)
            {
                return HttpNotFound();
            }
            return View(movieProducer);
        }

        // GET: MovieProducers/Create
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
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name");
            ViewBag.IdProducer = new SelectList(db.Producers, "Id", "FullName");
            return View();
        }

        // POST: MovieProducers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdMovie,IdProducer")] MovieProducer movieProducer)
        {
            if (ModelState.IsValid)
            {
                db.MovieProducers.Add(movieProducer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", movieProducer.IdMovie);
            ViewBag.IdProducer = new SelectList(db.Producers, "Id", "FullName", movieProducer.IdProducer);
            return View(movieProducer);
        }

        // GET: MovieProducers/Edit/5
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
            MovieProducer movieProducer = db.MovieProducers.Find(id);
            if (movieProducer == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", movieProducer.IdMovie);
            ViewBag.IdProducer = new SelectList(db.Producers, "Id", "FullName", movieProducer.IdProducer);
            return View(movieProducer);
        }

        // POST: MovieProducers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdMovie,IdProducer")] MovieProducer movieProducer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movieProducer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdMovie = new SelectList(db.movies, "Id", "Name", movieProducer.IdMovie);
            ViewBag.IdProducer = new SelectList(db.Producers, "Id", "Name", movieProducer.IdProducer);
            return View(movieProducer);
        }

        // GET: MovieProducers/Delete/5
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
            MovieProducer movieProducer = db.MovieProducers.Find(id);
            if (movieProducer == null)
            {
                return HttpNotFound();
            }
            return View(movieProducer);
        }

        // POST: MovieProducers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovieProducer movieProducer = db.MovieProducers.Find(id);
            db.MovieProducers.Remove(movieProducer);
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
