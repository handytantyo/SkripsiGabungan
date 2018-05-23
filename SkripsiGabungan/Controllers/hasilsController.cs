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
    public class hasilsController : Controller
    {
        private tugasakhirEntities db = new tugasakhirEntities();

        // GET: hasils
        public ActionResult Index()
        {
            var hasils = db.hasils.Include(h => h.perusahaan).Include(h => h.sumber).Include(h => h.tahun);
            return View(hasils.ToList());
        }

        public ActionResult GetData()
        {
            using (db)
            {
                db.Configuration.LazyLoadingEnabled = false;//most important
                //List<hasil> empList = db.hasils.ToList<hasil>();
                //var empList = db.hasils.OrderBy(a => a.id).ToList();
                var join = (from hasil in db.hasils
                            join perusahaan in db.perusahaans on hasil.id_perusahaan equals perusahaan.IDPerusahaan
                            join tahun in db.tahuns on hasil.id_tahun equals tahun.IDTahun
                            select new { hasil.IDHasil, perusahaan.nama_perusahaan, tahun.tahun1, hasil.ROE, hasil.ROI, hasil.tingkat_kesehatan, hasil.grade });
                //hasil.id digunakan untuk dipanggil function lain
                var list = new List<hasil>();
                foreach (var hasils in join)
                {
                    list.Add(new hasil()
                    {
                        IDHasil = hasils.IDHasil,
                        nama_perusahaan = hasils.nama_perusahaan,
                        tahun_1 = hasils.tahun1,
                        grade = hasils.grade,
                        tingkat_kesehatan = hasils.tingkat_kesehatan
                    });
                }
                list.OrderBy(a => a.IDHasil).ToList();
                return Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: hasils/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hasil hasil = db.hasils.Find(id);
            if (hasil == null)
            {
                return HttpNotFound();
            }
            return View(hasil);
        }

        // GET: hasils/Create
        public ActionResult Create()
        {
            ViewBag.id_perusahaan = new SelectList(db.perusahaans, "IDPerusahaan", "nama_perusahaan");
            ViewBag.IDHasil = new SelectList(db.sumbers, "IDSumber", "IDSumber");
            ViewBag.id_tahun = new SelectList(db.tahuns, "IDTahun", "IDTahun");
            return View();
        }

        // POST: hasils/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDHasil,id_perusahaan,id_tahun,ROE,ROI,cash_ratio,current_ratio,CP,PP,TATO,TMS_TA,target,target_2,tingkat_kesehatan,grade")] hasil hasil)
        {
            if (ModelState.IsValid)
            {
                db.hasils.Add(hasil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_perusahaan = new SelectList(db.perusahaans, "IDPerusahaan", "nama_perusahaan", hasil.id_perusahaan);
            ViewBag.IDHasil = new SelectList(db.sumbers, "IDSumber", "IDSumber", hasil.IDHasil);
            ViewBag.id_tahun = new SelectList(db.tahuns, "IDTahun", "IDTahun", hasil.id_tahun);
            return View(hasil);
        }

        // GET: hasils/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hasil hasil = db.hasils.Find(id);
            if (hasil == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_perusahaan = new SelectList(db.perusahaans, "IDPerusahaan", "nama_perusahaan", hasil.id_perusahaan);
            ViewBag.IDHasil = new SelectList(db.sumbers, "IDSumber", "IDSumber", hasil.IDHasil);
            ViewBag.id_tahun = new SelectList(db.tahuns, "IDTahun", "IDTahun", hasil.id_tahun);
            return View(hasil);
        }

        // POST: hasils/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDHasil,id_perusahaan,id_tahun,ROE,ROI,cash_ratio,current_ratio,CP,PP,TATO,TMS_TA,target,target_2,tingkat_kesehatan,grade")] hasil hasil)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hasil).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_perusahaan = new SelectList(db.perusahaans, "IDPerusahaan", "nama_perusahaan", hasil.id_perusahaan);
            ViewBag.IDHasil = new SelectList(db.sumbers, "IDSumber", "IDSumber", hasil.IDHasil);
            ViewBag.id_tahun = new SelectList(db.tahuns, "IDTahun", "IDTahun", hasil.id_tahun);
            return View(hasil);
        }

        // GET: hasils/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hasil hasil = db.hasils.Find(id);
            if (hasil == null)
            {
                return HttpNotFound();
            }
            return View(hasil);
        }

        // POST: hasils/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            hasil hasil = db.hasils.Find(id);
            db.hasils.Remove(hasil);
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
