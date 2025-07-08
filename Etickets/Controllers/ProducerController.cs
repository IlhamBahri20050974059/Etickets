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
    public class ProducerController : Controller
    {
        private eTicketsEntities db = new eTicketsEntities();

        // GET: Producer
        public ActionResult Index(string search, int page = 1)
        {
            if (Session["UserId"] == null)
            {
                // Jika belum login, redirect ke halaman login
                return RedirectToAction("Login", "Users");
            }
            var producers = db.Producers.AsQueryable();

            // Filter movies by search keyword
            if (!string.IsNullOrEmpty(search))
            {
                producers = producers.Where(m => m.FullName.Contains(search));
                ViewBag.Search = search;
            }

            // Paging
            int pageSize = 3;
            var pagedproducers = producers.OrderBy(m => m.FullName).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Page = page;

            return View(pagedproducers);
        }

        // GET: Producer/Details/5
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
            Producer producer = db.Producers.Find(id);
            if (producer == null)
            {
                return HttpNotFound();
            }
            return View(producer);
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
