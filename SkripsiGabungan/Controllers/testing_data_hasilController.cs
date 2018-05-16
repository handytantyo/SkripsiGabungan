using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SkripsiGabungan.Models;

namespace SkripsiGabungan.Controllers
{
    public class testing_data_hasilController : Controller
    {
        private tugasakhirEntities db = new tugasakhirEntities();

        // GET: testing_data_hasil
        public ActionResult Index()
        {
            return View(db.testing_data_hasil.ToList());
        }

        // GET: testing_data_hasil/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            testing_data_hasil testing_data_hasil = db.testing_data_hasil.Find(id);
            if (testing_data_hasil == null)
            {
                return HttpNotFound();
            }
            return View(testing_data_hasil);
        }

        // GET: testing_data_hasil/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: testing_data_hasil/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,ROE,ROI,cash_ratio,current_ratio,CP,PP,TATO,TMS_TA,Output,TingkatKesehatan,Grade")] testing_data_hasil testing_data_hasil)
        {
            if (ModelState.IsValid)
            {
                db.testing_data_hasil.Add(testing_data_hasil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(testing_data_hasil);
        }

        // GET: testing_data_hasil/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            testing_data_hasil testing_data_hasil = db.testing_data_hasil.Find(id);
            if (testing_data_hasil == null)
            {
                return HttpNotFound();
            }
            return View(testing_data_hasil);
        }

        // POST: testing_data_hasil/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ROE,ROI,cash_ratio,current_ratio,CP,PP,TATO,TMS_TA,Output,TingkatKesehatan,Grade")] testing_data_hasil testing_data_hasil)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testing_data_hasil).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(testing_data_hasil);
        }

        // GET: testing_data_hasil/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            testing_data_hasil testing_data_hasil = db.testing_data_hasil.Find(id);
            if (testing_data_hasil == null)
            {
                return HttpNotFound();
            }
            return View(testing_data_hasil);
        }

        // POST: testing_data_hasil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            testing_data_hasil testing_data_hasil = db.testing_data_hasil.Find(id);
            db.testing_data_hasil.Remove(testing_data_hasil);
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
