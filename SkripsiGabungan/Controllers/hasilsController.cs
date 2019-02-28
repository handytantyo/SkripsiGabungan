using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Highsoft.Web.Mvc.Charts;
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
                list.OrderBy(a => a.id_perusahaan).ToList();
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
            TempData["id_perusahaan"] = db.hasils.Find(id).id_perusahaan;
            if (hasil == null)
            {
                return HttpNotFound();
            }

            //string nama_perusahaan = db.perusahaans.Find(id).nama_perusahaan;
            

            var IDPerusahaan = db.hasils.Find(id).id_perusahaan;

            var selectedHasil = (from hasil_ in db.hasils
                                 join perusahaan in db.perusahaans on hasil_.id_perusahaan equals perusahaan.IDPerusahaan
                                 join tahun in db.tahuns on hasil_.id_tahun equals tahun.IDTahun
                                 where hasil_.id_perusahaan == IDPerusahaan
                                 select new { hasil_.IDHasil, perusahaan.nama_perusahaan, tahun.tahun1, hasil_.target_2, tahun.IDTahun })
                                 .OrderByDescending(x=>x.IDTahun).Take(5);

            string nama_perusahaan = "PT Nama Perusahaan";
            string[] KodeTahun = new string[5];
            double[] NilaiTarget = new double[5];
            int i = 4;
            foreach (var temp in selectedHasil)
            {
                nama_perusahaan = temp.nama_perusahaan;
                KodeTahun[i] = temp.tahun1.ToString();
                NilaiTarget[i] = Math.Round((double)temp.target_2 * 100, 0);
                i--;
            }

            List<string> KodeTahunValues = KodeTahun.ToList();
            List<double> TargetValues = NilaiTarget.ToList();
            List<double> BatasKurangSehat = new List<double> { 70, 70, 70, 70, 70 };
            List<double> BatasTidakSehat = new List<double> { 40, 40, 40, 40, 40 };

            List<SplineSeriesData> TargetData = new List<SplineSeriesData>();
            List<SplineSeriesData> DataKurangSehat = new List<SplineSeriesData>();
            List<SplineSeriesData> DataTidakSehat = new List<SplineSeriesData>();


            foreach (double value in TargetValues)
            {
                SplineSeriesData data = new SplineSeriesData();
                data.Y = value;
                //if (value == 86)
                //{
                //    data.Marker.Symbol = "url(http://www.highcharts.com/demo/gfx/sun.png)";
                //    //kasi warna
                //}

                TargetData.Add(data);
            }

            foreach (double value in BatasKurangSehat)
            {
                SplineSeriesData data = new SplineSeriesData();
                data.Y = value;

                DataKurangSehat.Add(data);
            }

            foreach (double value in BatasTidakSehat)
            {
                SplineSeriesData data = new SplineSeriesData();
                data.Y = value;

                DataTidakSehat.Add(data);
            }

            ViewData["Tahun"] = KodeTahunValues;
            ViewData["TargetData"] = TargetData;
            ViewData["DataKurangSehat"] = DataKurangSehat;
            ViewData["DataTidakSehat"] = DataTidakSehat;
            ViewData["NamaPerusahaan"] = nama_perusahaan;//db.hasils.Find(IDPerusahaan).nama_perusahaan;

            return View(hasil);
        }

        public ActionResult GetLineChart(string id_perusahaan)
        {
            id_perusahaan = TempData["id_perusahaan"].ToString();
            //var TargetNilai = db.hasils.Where(x => x.id_perusahaan.ToString() == id_perusahaan);
            string nama_perusahaan = db.perusahaans.Find(Convert.ToInt64(id_perusahaan)).nama_perusahaan;

            //var TempNilaiTarget = db.hasils.Find(Convert.ToInt64(id_perusahaan)).target_2;
            var join = (from hasil in db.hasils
                        join perusahaan in db.perusahaans on hasil.id_perusahaan equals perusahaan.IDPerusahaan
                        join tahun in db.tahuns on hasil.id_tahun equals tahun.IDTahun
                        where hasil.id_perusahaan.ToString() == id_perusahaan
                        select new { hasil.IDHasil, perusahaan.nama_perusahaan, tahun.tahun1, hasil.target_2});
            //double[] NilaiTarget = Array.ConvertAll(TempNilaiTarget, double.Parse);
            double[] NilaiTarget = new double[5];
            int i = 0;
            foreach(var temp in join)
            {
                NilaiTarget[i] = (double)temp.target_2 * 100;
                i++;
            }

            Nilai nilai = new Nilai();
            nilai.TargetTahun1 = NilaiTarget[0];
            nilai.TargetTahun2 = NilaiTarget[1];
            nilai.TargetTahun3 = NilaiTarget[2];
            nilai.TargetTahun4 = NilaiTarget[3];
            nilai.TargetTahun5 = NilaiTarget[4];
            nilai.NamaPerusahaan = nama_perusahaan;

            //ViewData["TargetData"] = TargetNilai;

            //return View();
            return Json(nilai, JsonRequestBehavior.AllowGet);
        }

        public class Nilai
        {
            public double TargetTahun1 { get; set; }
            public double TargetTahun2 { get; set; }
            public double TargetTahun3 { get; set; }
            public double TargetTahun4 { get; set; }
            public double TargetTahun5 { get; set; }
            public string NamaPerusahaan { get; set; }
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
