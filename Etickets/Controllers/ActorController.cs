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
    public class ActorController : Controller
    {
        private eTicketsEntities db = new eTicketsEntities();

        // GET: Actor
        public ActionResult Index(string search, int page = 1)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }

            var actor = db.Actors.AsQueryable();

            // Filter movies by search keyword
            if (!string.IsNullOrEmpty(search))
            {
                actor = actor.Where(m => m.FullName.Contains(search));
                ViewBag.Search = search;
            }

            // Paging
            int pageSize = 3;
            var pagedactor = actor.OrderBy(m => m.FullName).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Page = page;

            return View(pagedactor);
        }
        //public class mActor : Actor
        //{
        //    List<movie> Movies = new List<movie>();
        //}

        // GET: Actor/Details/5
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
            Actor actor = db.Actors.Find(id);
            if (actor == null)
            {
                return HttpNotFound();
            }
            //else
            //{
            //    actor.movies = db.movies.Where(w => w.IdActor == actor.Id).ToList();
            //}
            return View(actor);
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
