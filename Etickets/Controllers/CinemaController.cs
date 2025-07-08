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
    public class CinemaController : Controller
    {
        private eTicketsEntities db = new eTicketsEntities();

        // GET: Cinema
        public ActionResult Index(string search, int page = 1)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }

            var cinema = db.Cinemas.AsQueryable();

            // Filter movies by search keyword
            if (!string.IsNullOrEmpty(search))
            {
                cinema = cinema.Where(m => m.Name.Contains(search));
                ViewBag.Search = search;
            }

            // Paging
            int pageSize = 3;
            var pagedcinema = cinema.OrderBy(m => m.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Page = page;

            return View(pagedcinema);
        }

        // GET: Cinema/Details/5
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
            Cinema cinema = db.Cinemas.Find(id);
            if (cinema == null)
            {
                return HttpNotFound();
            }
            return View(cinema);
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
