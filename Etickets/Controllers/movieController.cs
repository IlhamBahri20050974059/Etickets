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
    public class movieController : Controller
    {
        private eTicketsEntities db = new eTicketsEntities();

        // GET: movie
        public ActionResult Index(string search, int? genre, int page = 1)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Filter movies by search keyword and/or genre
            var movies = db.movies.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                movies = movies.Where(m => m.Name.Contains(search));
                ViewBag.Search = search;
            }
            if (genre != null)
            {
                movies = movies.Where(m => m.MovieGenres.Any(mg => mg.IdGenre == genre));
                ViewBag.SelectedGenre = genre;
            }

            // Paging
            int pageSize = 3;
            var pagedMovies = movies.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Page = page;

            // Populate the ViewBag with the list of genres
            ViewBag.Genres = new SelectList(db.Genres, "Id", "Name");

            return View(pagedMovies);
        }





        // GET: movie/Details/5
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
            movie movie = db.movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        public ActionResult RatingReview(int idMovie)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            if (idMovie == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bookingmovie = db.BookingMovies
                                .Where(b => b.IdMovie == idMovie)
                                .ToList();
            var bookings = db.Bookings
                             .Where(b => b.Id == idMovie && b.Rating != null && b.Review != null)
                             .ToList();

            return View(bookings);
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
