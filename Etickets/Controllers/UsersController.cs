using Etickets.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Etickets.Controllers
{
    public class UsersController : Controller
    {
        private readonly eTicketsEntities db = new eTicketsEntities();

        
        // GET: User/Login
        [HttpGet]
        public ActionResult Login()
        {
            Session.Remove("UserId");
            Session.Remove("UserRole");
            return View(new User());
        }

        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                // Cari user dengan email dan password yang sesuai di database
                var foundUser = db.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                if (foundUser != null)
                {
                    // Simpan id user yang sedang login ke session
                    Session["UserId"] = foundUser.Id;
                    Session["UserRole"] = foundUser.Role;
                    if ((string)Session["UserRole"] == "Admin")
                    {
                        // Jika belum login atau bukan Admin, redirect ke halaman login
                        return RedirectToAction("Index", "movies");
                    }
                    return RedirectToAction("Index", "movie");
                }
                else
                {
                    ModelState.AddModelError("", "Email atau password salah");
                }
            }

            return View(user);
        }

        // GET: User/Register
        [HttpGet]
        public ActionResult Register()
        {
            Session.Remove("UserId");
            Session.Remove("UserRole");
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Cek apakah email yang dimasukkan sudah digunakan oleh user lain
                if (db.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email sudah digunakan");
                }
                else if (!isPasswordValid(user.Password))
                {
                    ModelState.AddModelError("Password", "Password harus terdiri dari minimal 8 karakter, terdapat huruf besar dan kecil, terdapat angka, dan simbol.");
                }
                else
                {
                    try
                    {
                        // Tambahkan user baru ke database
                        user.Role = "User"; // set role user
                        db.Users.Add(user);
                        db.SaveChanges();

                        // Simpan id user yang sedang login ke session
                        Session["UserId"] = user.Id;
                        Session["UserRole"] = user.Role;

                        return RedirectToAction("Index", "movie");
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException e)
                    {
                        ModelState.AddModelError("", getValidationException(e));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.ToString());
                    }
                }
            }

            return View(user);
        }


        // Method untuk memeriksa kekuatan password
        private bool isPasswordValid(string password)
{
    // Memeriksa apakah password memenuhi kriteria
    // Minimal 8 karakter, terdapat huruf besar dan kecil, terdapat angka, dan simbol.
    bool isPasswordStrong = password.Length >= 8 &&
        password.Any(char.IsUpper) &&
        password.Any(char.IsLower) &&
        password.Any(char.IsDigit) &&
        password.Any(c => !char.IsLetterOrDigit(c));

    return isPasswordStrong;
}

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
            else if (id != (int?)Session["UserId"])
            {
                id = Session["UserId"] as int?;
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }


        public ActionResult Edit(int? id)
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
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    ImageFile.SaveAs(path);
                    user.ProfilePictureURL = "~/Content/Images/" + fileName;
                }

                try
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = user.Id });
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
            return View(user);
        }

        // Method untuk mengambil pesan error validasi dari exception
        private string getValidationException(System.Data.Entity.Validation.DbEntityValidationException ex)
{
    StringBuilder sb = new StringBuilder();

    foreach (var failure in ex.EntityValidationErrors)
    {
        sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
        foreach (var error in failure.ValidationErrors)
        {
            sb.AppendFormat("- {0} : {1}\n", error.PropertyName, error.ErrorMessage);
        }
    }

    return sb.ToString();
}


        // GET: User/Logout
        [HttpGet]
        public ActionResult Logout()
        {
            // Hapus id user yang sedang login dari session
            Session.Remove("UserId");
            Session.Remove("UserRole");

            return RedirectToAction("Index", "Dashboard");
        }
    }
}

