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
    public class ClassificationController : Controller
    {
        private tugasakhirEntities db = new tugasakhirEntities();
        
        // GET: Classification
        public ActionResult Index()
        {

            TempData["ID_Tahun"] = 2;
            ViewBag.id_tahun = new SelectList(db.tahuns, "IDTahun", "tahun1", 2);
            var hasils = db.hasils.Include(h => h.perusahaan).Include(h => h.sumber).Include(h => h.tahun);            
            return View(hasils.ToList());
        }

        [HttpPost]
        public ActionResult Index(int id_tahun)
        {            
            ViewBag.id_tahun = new SelectList(db.tahuns, "IDTahun", "tahun1");
            var hasils = db.hasils.Include(h => h.perusahaan).Include(h => h.tahun).Where(a => a.id_tahun == id_tahun);

            TempData["ID_Tahun"] = id_tahun;
            //return RedirectToAction("GetChart", "Classification", new { userId = id_tahun });
            return View(hasils.ToList());
        }

        public ActionResult GetChartAll()
        {
            var Sehat = db.hasils.Where(x => x.tingkat_kesehatan == "Sehat").Count();
            var KurangSehat = db.hasils.Where(x => x.tingkat_kesehatan == "Kurang Sehat").Count();
            var TidakSehat = db.hasils.Where(x => x.tingkat_kesehatan == "Tidak Sehat").Count();

            Ratio obj = new Ratio();
            obj.Sehat = Sehat;
            obj.KurangSehat = KurangSehat;
            obj.TidakSehat = TidakSehat;
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChart(string id_tahun)
        {
            var Sehat = db.hasils.Where(x => x.tingkat_kesehatan == "Sehat").Count();
            var KurangSehat = db.hasils.Where(x => x.tingkat_kesehatan == "Kurang Sehat").Count();
            var TidakSehat = db.hasils.Where(x => x.tingkat_kesehatan == "Tidak Sehat").Count();            

            id_tahun = TempData["ID_Tahun"].ToString();
            int KodeTahun = Convert.ToInt32(id_tahun) + 2011;
            if (id_tahun != null)
            {
                Sehat = db.hasils.Where(x => x.tingkat_kesehatan == "Sehat").Where(x => x.tahun.IDTahun.ToString() == id_tahun).Count();
                KurangSehat = db.hasils.Where(x => x.tingkat_kesehatan == "Kurang Sehat").Where(x => x.tahun.IDTahun.ToString() == id_tahun).Count();
                TidakSehat = db.hasils.Where(x => x.tingkat_kesehatan == "Tidak Sehat").Where(x => x.tahun.IDTahun.ToString() == id_tahun).Count();
            }

            //ViewData["Tahun"] = db.tahuns.Find(id_tahun).tahun1;

            Ratio obj = new Ratio();
            obj.Sehat = Sehat;
            obj.KurangSehat = KurangSehat;
            obj.TidakSehat = TidakSehat;
            obj.KodeTahun = KodeTahun;
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public class Ratio
        {
            public int Sehat { get; set; }
            public int KurangSehat { get; set; }
            public int TidakSehat { get; set; }
            public int KodeTahun { get; set; }
        }

        // GET: Classification/Details/5
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

        // GET: Classification/Create
        public ActionResult Create()
        {
            ViewBag.id_perusahaan = new SelectList(db.perusahaans, "IDPerusahaan", "nama_perusahaan");
            ViewBag.IDHasil = new SelectList(db.sumbers, "IDSumber", "IDSumber");
            ViewBag.id_tahun = new SelectList(db.tahuns, "IDTahun", "IDTahun");
            return View();
        }

        // POST: Classification/Create
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

        // GET: Classification/Edit/5
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

        // POST: Classification/Edit/5
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

        // GET: Classification/Delete/5
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

        // POST: Classification/Delete/5
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
