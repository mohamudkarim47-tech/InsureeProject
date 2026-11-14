using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InsureeProject.Data;
using InsureeProject.Models;

namespace InsureeProject.Controllers
{
    public class InsureesController : Controller
    {
        private InsureeProjectContext db = new InsureeProjectContext();

        // GET: Insurees
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insurees/Admin (THIS IS THE NEW METHOD)
        public ActionResult Admin()
        {
            var insurees = db.Insurees.ToList();
            return View(insurees);
        }

        // GET: Insurees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insurees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insurees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,FullCoverage")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                // 1. Start with a base of $50 / month
                decimal quote = 50m;

                // Calculate Age accurately
                int age = DateTime.Now.Year - insuree.DateOfBirth.Year;
                if (DateTime.Now.DayOfYear < insuree.DateOfBirth.DayOfYear) { age--; }

                // 2-4. Age Logic
                if (age <= 18)
                {
                    quote += 100m;
                }
                else if (age >= 19 && age <= 25)
                {
                    quote += 50m;
                }
                else
                {
                    quote += 25m;
                }

                // 5-6. Car Year Logic
                if (insuree.CarYear < 2000) { quote += 25m; }
                else if (insuree.CarYear > 2015) { quote += 25m; }

                // 7-8. Car Make & Model Logic
                if (insuree.CarMake.ToLower() == "porsche")
                {
                    quote += 25m;
                    if (insuree.CarModel.ToLower() == "911 carrera")
                    {
                        quote += 25m;
                    }
                }

                // 9. Speeding Tickets Logic
                quote += (insuree.SpeedingTickets * 10m);

                // 10. DUI Logic
                if (insuree.DUI)
                {
                    quote *= 1.25m; // Adds 25%
                }

                // 11. Full Coverage Logic
                if (insuree.FullCoverage)
                {
                    quote *= 1.50m; // Adds 50%
                }

                // Assign the calculated quote to the model
                insuree.Quote = quote;

                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: Insurees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insurees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,FullCoverage,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insurees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insurees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
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