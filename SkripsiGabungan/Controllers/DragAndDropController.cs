using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Bytescout.PDFExtractor;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using SkripsiGabungan.Models;
using System.Net;

namespace SkripsiGabungan.Controllers
{
    public class DragAndDropController : Controller
    {
        private tugasakhirEntities db = new tugasakhirEntities();

        // GET: DragAndDrop
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files)
        {
            //TextExtractor extractor = new TextExtractor();
            //extractor.RegistrationName = "demo";
            //extractor.RegistrationKey = "demo";

            foreach (var file in files)
            {
                string filePath = "default" + System.IO.Path.GetExtension(file.FileName);//Guid.NewGuid() + Path.GetExtension(file.FileName);
                //string fileapa = Server.MapPath(filePath);
                //extractor.LoadDocumentFromFile(fileapa);
                //extractor.OCRMode = OCRMode.Auto;
                //extractor.OCRLanguageDataFolder = @"C:\Users\Nabyl\Desktop\ocr1IronPDF\ocr1IronPDF\tessdata";
                //extractor.OCRLanguage = "eng";
                //extractor.OCRResolution = 300;
                //extractor.SaveTextToFile(Path.Combine(Server.MapPath("~/UploadedFiles/output.txt"), filePath));
                file.SaveAs(System.IO.Path.Combine(Server.MapPath("~/UploadedFiles"), filePath));//filePath
                //Here you can write code for save this information in your database if you want
                Page_Load(System.IO.Path.Combine(Server.MapPath("~/UploadedFiles"), filePath), filePath);

            }

            return Json("file uploaded successfully");
        }

        [NonAction]
        protected void Page_Load(string filepath, string name)
        {
            String inputFile = filepath;
            string namaFIle = name;

            // Create Bytescout.PDFExtractor.TextExtractor instance
            TextExtractor extractor = new TextExtractor();
            extractor.RegistrationName = "nabylchat13@gmail.com";
            extractor.RegistrationKey = "C5F0-D64B-193D-332F-169C-1751-6CE";

            // Load sample PDF document
            extractor.LoadDocumentFromFile(inputFile);

            Response.Clear();
            Response.ContentType = "text/html";

            Response.Write("<pre>");

            // Write extracted text to output stream
            //extractor.SaveTextToStream(Response.OutputStream);

            // Save extracted text to file
            extractor.SaveTextToFile(System.IO.Path.Combine(Server.MapPath("~/UploadedFiles"), namaFIle + "TextFIle"));
            Response.Write("</pre>");

            Response.End();
        }

        // GET: testing_data_hasil/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: testing_data_hasil/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,ROE,ROI,cash_ratio,current_ratio,CP,PP,TATO,TMS_TA,Output,TingkatKesehatan,Grade")] testing_data_hasil testing_data_hasil)
        {
            string pdfToTxt = System.IO.File.ReadAllText(Server.MapPath("~/UploadedFiles/default.pdfTextFIle"));
            //string pdfToTxt = ExtractTextFromPdf(extractor.ToString());
            //string[] words = extractor.ToString().Split('/', '.');
            long labaUsaha_ = labaUsaha(pdfToTxt);
            long labaBersih_ = labaBersih(pdfToTxt);
            long kasDanSetaraKas_ = kasDanSetaraKas(pdfToTxt);
            long penyusutan_ = penyusutan(pdfToTxt);
            long persediaan_ = persediaan(pdfToTxt);
            long AsetLancar_ = AsetLancar(pdfToTxt);
            long TotalAset_ = TotalAset(pdfToTxt);
            long Liabilitas_ = Liabilitas(pdfToTxt);
            long Ekuitas_ = Ekuitas(pdfToTxt);
            long Pendapatan_ = Pendapatan(pdfToTxt);
            //long PihakBerelasi_ = PihakBerelasi(pdfToTxt);
            //long PihakKetiga_ = PihakKetiga(pdfToTxt);
            long Investasi_ = Investasi(pdfToTxt);
            long CapitalEmployed = TotalAset_ - Liabilitas_;

            var Indikators = db.Indikators;

            testing_data_hasil.ROE = ((float)labaBersih_ / Ekuitas_) * 100;
            testing_data_hasil.ROI = (((float)labaUsaha_ + penyusutan_) / CapitalEmployed) * 100;
            testing_data_hasil.cash_ratio = (((float)kasDanSetaraKas_ + Investasi_) / Liabilitas_) * 100;//investasi belum
            testing_data_hasil.current_ratio = ((float)AsetLancar_ / Liabilitas_) * 100;
            //testing_data_hasil.CP = ((float)PiutangUsaha / Pendapatan) * 365;//piutang usaha belum
            testing_data_hasil.PP = ((float)persediaan_ / Pendapatan_) * 365;
            testing_data_hasil.TATO = ((float)Pendapatan_ / CapitalEmployed) * 100;
            testing_data_hasil.TMS_TA = ((float)Ekuitas_ / TotalAset_) * 100;

            //ANN and FNN
            double[] input = new double[3];
            input[0] = (double)testing_data_hasil.ROE;
            input[1] = (double)testing_data_hasil.ROI;
            input[2] = (double)testing_data_hasil.cash_ratio;

            double[] weight = new double[12];
            double[] bias = new double[2];
            float outoANN;

            double[] neth = new double[3];
            float[] outh = new float[3];            

            SQLConnectionForANN(weight, bias);

            outoANN = FeedForward(input, weight, bias, neth, outh);

            testing_data_hasil.OutputANN = outoANN;

            if (ModelState.IsValid)
            {
                db.testing_data_hasil.Add(testing_data_hasil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(testing_data_hasil);
        }

        static void SQLConnectionForANN(double[] Weight, double[] Bias)
        {
            //sqlconnection from https://www.youtube.com/watch?v=IcD9Jffstmw
            string connStr = @"Data Source=DESKTOP-ERK0RV1\SQLEXPRESS;Initial Catalog=tugasakhir;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM [Indikator] WHERE Algorithm = 'ANN'", conn);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    for(int weight = 0; weight < 12; weight++)
                    {
                        Weight[weight] = (double)reader["Weight" + weight + ""];
                    }

                    for (int bias = 0; bias < 2; bias++)
                    {
                        Bias[bias] = (double)reader["Bias" + bias + ""];
                    }

                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            conn.Close();
        }

        static float FeedForward(double[] input, double[] weight, double[] bias, double[] neth, float[] outh)
        {
            int kolom = 0;
            double neto;
            float outo;

            for (int i = 0; i < 3; i++)
            {
                neth[i] = input[ 0] * weight[kolom] + input[1] * weight[kolom + 1] + input[2] * weight[kolom + 2] + bias[0];//sudah bener			
                outh[i] = 1 / (1 + (float)Math.Exp(-neth[i]));
                //cout<<weight[kolom]<<" "<<weight[kolom+1]<<" "<<weight[kolom+2]<<endl<<endl;
                kolom = kolom + 3;
                //cout<<"["<<i+1<<"] "<<neth[i]<<" "<<outh[i]<<endl;						
            }
            kolom = 0;

            neto = weight[9] * outh[0] + weight[10] * outh[1] + weight[11] * outh[2] + bias[1];
            //cout<<neto<<endl;
            outo = 1 / (1 + (float)Math.Exp(-neto));

            return outo;
        }

        static long labaUsaha(string pdfToTxt)
        {
            string kataLabaUsaha = GetStringFromLaporan("laba usaha", pdfToTxt);

            string labaUsaha;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataLabaUsaha, i).Length > 7 && getBetweenChunk(pdfToTxt, kataLabaUsaha, i).ToCharArray()[0] < 58)
                {
                    labaUsaha = getBetweenChunk(pdfToTxt, kataLabaUsaha, i);
                    break;
                }
            }

            char[] arrayLabaUsaha = labaUsaha.ToCharArray();
            char[] arrayLabaUsaha2 = new char[arrayLabaUsaha.Length];
            int nextLabaUsaha = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayLabaUsaha.Length; i++)
            {
                char letterLabaUsaha = arrayLabaUsaha[i];
                //Console.WriteLine(letter);
                if (arrayLabaUsaha[i] != '.' && arrayLabaUsaha[i] != ',' && arrayLabaUsaha[i] != '(' && arrayLabaUsaha[i] != ')')
                {
                    arrayLabaUsaha2[nextLabaUsaha] = arrayLabaUsaha[i];
                    nextLabaUsaha++;
                }
            }

            string strLabaUsaha = new string(arrayLabaUsaha2);
            long lngLabaUsaha = Int64.Parse(strLabaUsaha);

            return lngLabaUsaha;
        }

        static long labaBersih(string pdfToTxt)
        {
            string kataLabaBersih = GetStringFromLaporan("laba bersih", pdfToTxt);

            string labaBersih;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataLabaBersih, i).Length > 7 && getBetweenChunk(pdfToTxt, kataLabaBersih, i).ToCharArray()[0] < 58)
                {
                    labaBersih = getBetweenChunk(pdfToTxt, kataLabaBersih, i);
                    break;
                }
            }

            char[] arrayLabaBersih = labaBersih.ToCharArray();
            char[] arrayLabaBersih2 = new char[arrayLabaBersih.Length];
            int nextLabaBersih = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayLabaBersih.Length; i++)
            {
                char letterLabaUsaha = arrayLabaBersih[i];
                //Console.WriteLine(letter);
                if (arrayLabaBersih[i] != '.' && arrayLabaBersih[i] != ',' && arrayLabaBersih[i] != '(' && arrayLabaBersih[i] != ')')
                {
                    arrayLabaBersih2[nextLabaBersih] = arrayLabaBersih[i];
                    nextLabaBersih++;
                }
            }

            string strLabaBersih = new string(arrayLabaBersih2);
            long lngLabaBersih = Int64.Parse(strLabaBersih);

            return lngLabaBersih;
        }

        static long kasDanSetaraKas(string pdfToTxt)
        {            
            string kataKasSetara = GetStringFromLaporan("kas dan setara kas", pdfToTxt);
            //int iKeKasSetara = getIfromDB("kas dan setara kas");
            string KasSetara;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataKasSetara, i).Length > 7 && getBetweenChunk(pdfToTxt, kataKasSetara, i).ToCharArray()[0] < 58)
                {
                    KasSetara = getBetweenChunk(pdfToTxt, kataKasSetara, i);
                    break;
                }
            }

            char[] arrayKasSetara = KasSetara.ToCharArray();
            char[] arrayKasSetara2 = new char[arrayKasSetara.Length];
            int nextKasSetara = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayKasSetara.Length; i++)
            {
                char letterKasSetara = arrayKasSetara[i];
                //Console.WriteLine(letter);
                if (arrayKasSetara[i] != '.' && arrayKasSetara[i] != ',')
                {
                    arrayKasSetara2[nextKasSetara] = arrayKasSetara[i];
                    nextKasSetara++;
                }
            }

            string strkas = new string(arrayKasSetara2);
            long lngKas = Int64.Parse(strkas);

            return lngKas;
        }

        static long penyusutan(string pdfToTxt)
        {
            string kataBebanUmum = GetStringFromLaporan("penyusutan", pdfToTxt);
            string kataPenyusutan = GetStringFromLaporan2("penyusutan", pdfToTxt);

            string Penyusutan;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunkSubPenyusutan(pdfToTxt, kataBebanUmum, kataPenyusutan, i).Length > 7 && getBetweenChunkSubPenyusutan(pdfToTxt, kataBebanUmum, kataPenyusutan, i).ToCharArray()[0] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kataBebanUmum, kataPenyusutan, i).ToCharArray()[1] < 58)
                {
                    Penyusutan = getBetweenChunkSubPenyusutan(pdfToTxt, kataBebanUmum, kataPenyusutan, i);
                    break;
                }
            }

            char[] arrayPenyusutan = Penyusutan.ToCharArray();
            char[] arrayPenyusutan2 = new char[arrayPenyusutan.Length];
            int nextPenyusutan = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayPenyusutan.Length; i++)
            {
                char letterPenyusutan = arrayPenyusutan[i];
                //Console.WriteLine(letter);
                if (arrayPenyusutan[i] != '.' && arrayPenyusutan[i] != ',')
                {
                    arrayPenyusutan2[nextPenyusutan] = arrayPenyusutan[i];
                    nextPenyusutan++;
                }
            }

            string strPenyusutan = new string(arrayPenyusutan2);
            long lngPenyusutan = Int64.Parse(strPenyusutan);

            return lngPenyusutan;
        }

        static long persediaan(string pdfToTxt)
        {
            string kataPersediaan = GetStringFromLaporan("persediaan", pdfToTxt);
            string Persediaan;

            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataPersediaan, i).Length > 7 && getBetweenChunk(pdfToTxt, kataPersediaan, i).ToCharArray()[0] < 58)
                {
                    Persediaan = getBetweenChunk(pdfToTxt, kataPersediaan, i);
                    break;
                }
            }

            char[] arrayPersediaan = Persediaan.ToCharArray();
            char[] arrayPersediaan2 = new char[arrayPersediaan.Length];
            int nextPersediaan = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayPersediaan.Length; i++)
            {
                char letterPersediaan = arrayPersediaan[i];
                //Console.WriteLine(letter);
                if (arrayPersediaan[i] != '.' && arrayPersediaan[i] != ',' && arrayPersediaan[i] != '(' && arrayPersediaan[i] != ')')
                {
                    arrayPersediaan2[nextPersediaan] = arrayPersediaan[i];
                    nextPersediaan++;
                }
            }

            string strPersediaan = new string(arrayPersediaan2);
            long lngPersediaan = Int64.Parse(strPersediaan);

            return lngPersediaan;
        } 

        static long AsetLancar(string pdfToTxt)
        {
            string kataAsetLancar = GetStringFromLaporan("aset lancar", pdfToTxt);
            //int iKeAsetLancar = getIfromDB("aset lancar");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            
            string AsetLancar;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataAsetLancar, i).Length > 7 && getBetweenChunk(pdfToTxt, kataAsetLancar, i).ToCharArray()[0] < 58)
                {
                    AsetLancar = getBetweenChunk(pdfToTxt, kataAsetLancar, i);
                    break;
                }
            }

            char[] arrayAsetLancar = AsetLancar.ToCharArray();
            char[] arrayAsetLancar2 = new char[arrayAsetLancar.Length];
            int nextAsetLancar = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayAsetLancar.Length; i++)
            {
                char letterAsetLancar = arrayAsetLancar[i];
                //Console.WriteLine(letter);
                if (arrayAsetLancar[i] != '.' && arrayAsetLancar[i] != ',')
                {
                    arrayAsetLancar2[nextAsetLancar] = arrayAsetLancar[i];
                    nextAsetLancar++;
                }
            }
            string strAsetLancar = new string(arrayAsetLancar2);
            long lngAsetLancar = Int64.Parse(strAsetLancar);

            return lngAsetLancar;
        }

        static long TotalAset (string pdfToTxt)
        {
            string kataTotalAset = GetStringFromLaporan("total aset", pdfToTxt);
            //int iKeTotalAset = getIfromDB("total aset");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe)

            string TotalAset;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataTotalAset, i).Length > 7 && getBetweenChunk(pdfToTxt, kataTotalAset, i).ToCharArray()[0] < 58)
                {
                    TotalAset = getBetweenChunk(pdfToTxt, kataTotalAset, i);
                    break;
                }
            }

            char[] arrayTotalAset = TotalAset.ToCharArray();
            char[] arrayTotalAset2 = new char[arrayTotalAset.Length];
            int nextTotalAset = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayTotalAset.Length; i++)
            {
                char letterTotalAset = arrayTotalAset[i];
                //Console.WriteLine(letter);
                if (arrayTotalAset[i] != '.' && arrayTotalAset[i] != ',')
                {
                    arrayTotalAset2[nextTotalAset] = arrayTotalAset[i];
                    nextTotalAset++;
                }
            }

            string strTotalAset = new string(arrayTotalAset2);
            long lngTotalAset = Int64.Parse(strTotalAset);

            return lngTotalAset;
        }

        static long Liabilitas (string pdfToTxt)
        {
            string kataLiabilitas = GetStringFromLaporan("liabilitas jangka pendek", pdfToTxt);
            // int iKeLiabilitas = getIfromDB("liabilitas jangka pendek");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
         

            string Liabilitas;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataLiabilitas, i).Length > 7 && getBetweenChunk(pdfToTxt, kataLiabilitas, i).ToCharArray()[0] < 58)
                {
                    Liabilitas = getBetweenChunk(pdfToTxt, kataLiabilitas, i);
                    break;
                }
            }

            char[] arrayLiabilitas = Liabilitas.ToCharArray();
            char[] arrayLiabilitas2 = new char[arrayLiabilitas.Length];
            int nextLiabilitas = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayLiabilitas.Length; i++)
            {
                char letterLiabilitas = arrayLiabilitas[i];
                //Console.WriteLine(letter);
                if (arrayLiabilitas[i] != '.' && arrayLiabilitas[i] != ',')
                {
                    arrayLiabilitas2[nextLiabilitas] = arrayLiabilitas[i];
                    nextLiabilitas++;
                }
            }

            string strLiabilitas = new string(arrayLiabilitas2);
            long lngLiabilitas = Int64.Parse(strLiabilitas);

            return lngLiabilitas;
        }

        static long Ekuitas (string pdfToTxt)
        {
            string kataEkuitas = GetStringFromLaporan("total ekuitas", pdfToTxt);

            string Ekuitas;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataEkuitas, i).Length > 7 && getBetweenChunk(pdfToTxt, kataEkuitas, i).ToCharArray()[0] < 58)
                {
                    Ekuitas = getBetweenChunk(pdfToTxt, kataEkuitas, i);
                    break;
                }
            }

            char[] arrayEkuitas = Ekuitas.ToCharArray();
            char[] arrayEkuitas2 = new char[arrayEkuitas.Length];
            int nextEkuitas = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayEkuitas.Length; i++)
            {
                char letterEkuitas = arrayEkuitas[i];
                //Console.WriteLine(letter);
                if (arrayEkuitas[i] != '.' && arrayEkuitas[i] != ',')
                {
                    arrayEkuitas2[nextEkuitas] = arrayEkuitas[i];
                    nextEkuitas++;
                }
            }

            string strEkuitas = new string(arrayEkuitas2);
            long lngEkuitas = Int64.Parse(strEkuitas);

            return lngEkuitas;
        }

        static long Pendapatan (string pdfToTxt)
        {
            string kataPendapatan = GetStringFromLaporan("total pendapatan", pdfToTxt);
            //int iKePendapatan = getIfromDB("total pendapatan");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            

            string Pendapatan;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataPendapatan, i).Length > 7 && getBetweenChunk(pdfToTxt, kataPendapatan, i).ToCharArray()[0] < 58)
                {
                    Pendapatan = getBetweenChunk(pdfToTxt, kataPendapatan, i);
                    break;
                }
            }
            char[] arrayPendapatan = Pendapatan.ToCharArray();
            char[] arrayPendapatan2 = new char[arrayPendapatan.Length];
            int nextPendapatan = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayPendapatan.Length; i++)
            {
                char letterPendapatan = arrayPendapatan[i];
                //Console.WriteLine(letter);
                if (arrayPendapatan[i] != '.' && arrayPendapatan[i] != ',')
                {
                    arrayPendapatan2[nextPendapatan] = arrayPendapatan[i];
                    nextPendapatan++;
                }
            }

            string strPendapatan = new string(arrayPendapatan2);
            long lngPendapatan = Int64.Parse(strPendapatan);

            return lngPendapatan;
        }

        static long PihakBerelasi (string pdfToTxt)
        {
            string kataPiutangUsaha = GetStringFromLaporan("pihak berelasi", pdfToTxt);
            string kataPihakBerelasi = GetStringFromLaporan2("pihak berelasi", pdfToTxt);
            string kata3 = GetStringFromLaporan3("pihak berelasi", pdfToTxt);
            //int iKePihakBerelasi = getIfromDB("pihak berelasi");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);


            string PihakBerelasi;
            
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk3nama(pdfToTxt, kataPiutangUsaha, kataPihakBerelasi, kata3, i).Length > 7 && getBetweenChunk3nama(pdfToTxt, kataPiutangUsaha, kataPihakBerelasi, kata3, i).ToCharArray()[0] < 58)
                {
                    PihakBerelasi = getBetweenChunk3nama(pdfToTxt, kataPiutangUsaha, kataPihakBerelasi, kata3, i);
                    break;
                }
            }
            char[] arrayPihakBerelasi = PihakBerelasi.ToCharArray();
            char[] arrayPihakBerelasi2 = new char[arrayPihakBerelasi.Length];
            int nextPihakBerelasi = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayPihakBerelasi.Length; i++)
            {
                char letterPihakBerelasi = arrayPihakBerelasi[i];
                //Console.WriteLine(letter);
                if (arrayPihakBerelasi[i] != '.' && arrayPihakBerelasi[i] != ',' && arrayPihakBerelasi[i] != '(' && arrayPihakBerelasi[i] != ')')
                {
                    arrayPihakBerelasi2[nextPihakBerelasi] = arrayPihakBerelasi[i];
                    nextPihakBerelasi++;
                }
            }
            string strPihakBerelasi = new string(arrayPihakBerelasi2);
            long lngPihakBerelasi = Int64.Parse(strPihakBerelasi);

            return lngPihakBerelasi;
        }

        static long PihakKetiga(string pdfToTxt)
        {
            string kataPihakKetiga = GetStringFromLaporan("pihak ketiga", pdfToTxt);
            //int iKePihakKetiga = getIfromDB("pihak ketiga");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
           

            string PihakKetiga;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataPihakKetiga, i).Length > 7 && getBetweenChunk(pdfToTxt, kataPihakKetiga, i).ToCharArray()[0] < 58)
                {
                    PihakKetiga = getBetweenChunk(pdfToTxt, kataPihakKetiga, i);
                    break;
                }
            }
            char[] arrayPihakKetiga = PihakKetiga.ToCharArray();
            char[] arrayPihakKetiga2 = new char[arrayPihakKetiga.Length];
            int nextPihakKetiga = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayPihakKetiga.Length; i++)
            {
                char letterPihakKetiga = arrayPihakKetiga[i];
                //Console.WriteLine(letter);
                if (arrayPihakKetiga[i] != '.' && arrayPihakKetiga[i] != ',' && arrayPihakKetiga[i] != '(' && arrayPihakKetiga[i] != ')')
                {
                    arrayPihakKetiga2[nextPihakKetiga] = arrayPihakKetiga[i];
                    nextPihakKetiga++;
                }
            }
            string strPihakKetiga = new string(arrayPihakKetiga2);
            long lngPihakKetiga = Int64.Parse(strPihakKetiga);

            return lngPihakKetiga;
        }

        static long Investasi(string pdfToTxt)
        {
            string kataInvestasi = GetStringFromLaporan("investasi jangka pendek", pdfToTxt);
            //int iKePihakKetiga = getIfromDB("pihak ketiga");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);

            string Investasi = "";
            for (int i = 2; i < 40; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataInvestasi, i).Length > 7 && getBetweenChunk(pdfToTxt, kataInvestasi, i).ToCharArray()[0] < 58)
                {
                    Investasi = getBetweenChunk(pdfToTxt, kataInvestasi, i);
                    break;
                }
            }

            char[] arrayInvestasi = Investasi.ToCharArray();
            char[] arrayInvestasi2 = new char[arrayInvestasi.Length];
            int nextInvestasi = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayInvestasi.Length; i++)
            {
                char letterPihakKetiga = arrayInvestasi[i];
                //Console.WriteLine(letter);
                if (arrayInvestasi[i] != '.' && arrayInvestasi[i] != ',' && arrayInvestasi[i] != '(' && arrayInvestasi[i] != ')')
                {
                    arrayInvestasi2[nextInvestasi] = arrayInvestasi[i];
                    nextInvestasi++;
                }
            }
            string strInvestasi = new string(arrayInvestasi2);
            long lngInvestasi;
            try
            {
                lngInvestasi = Int64.Parse(strInvestasi);
            }
            catch
            {
                lngInvestasi = 0;
            }

            return lngInvestasi;
        }

        [HttpGet]
        public ActionResult Result(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            testing_data_hasil testingData = db.testing_data_hasil.Find(id);
            if (testingData == null)
            {
                return HttpNotFound();
            }
            return View(testingData);            
        }       

        public static string ExtractTextFromPdf(string path)
        {
            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));

                }

                return text.ToString();
            }
        }

        public static string getBetweenChunk(string strSource, string strStart, int HowManyChunck)
        {
            int Start;
            char[] whitespace = new char[] { ' ', '\t', '\n' };

            if (strSource.Contains(strStart))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                string findwordChunk = strSource.Substring(Start, strSource.Length - Start);
                string[] Chunk = findwordChunk.Split(whitespace);
                return Chunk[HowManyChunck - 1];
            }
            else
            {
                return "";
            }
        }

        public static string getBetweenChunkSubPenyusutan(string strSource, string strStart, string next, int HowManyChunck)
        {
            int Start;
            int Start2 = 0;
            char[] whitespace = new char[] { ' ', '\t', '\n' };

            if (strSource.Contains(strStart))
            {
                //for (int i = 0; i < 6; i++)
                //{
                Start = strSource.IndexOf(strStart, Start2) + strStart.Length;
                Start2 = Start;
                //}
                //Start2 = strSource.IndexOf(strStart, Start) + strStart.Length;
                Start = strSource.IndexOf(next, Start2) + next.Length;
                Start2 = Start;

                string findwordChunk = strSource.Substring(Start2, strSource.Length - Start2);
                string[] Chunk = findwordChunk.Split(whitespace);
                return Chunk[HowManyChunck - 1];
            }
            else
            {
                return "";
            }
        }

        public static string getBetweenChunk3nama(string strSource, string strStart, string next, string next2, int HowManyChunck)
        {
            int Start;
            int Start2 = 0;
            int start3 = 0;
            char[] whitespace = new char[] { ' ', '\t', '\n' };

            if (strSource.Contains(strStart))
            {
                //for (int i = 0; i < 6; i++)
                //{
                Start = strSource.IndexOf(strStart, Start2) + strStart.Length;
                Start2 = Start;
                //}
                //Start2 = strSource.IndexOf(strStart, Start) + strStart.Length;
                Start = strSource.IndexOf(next, Start2) + next.Length;
                Start2 = Start;

                Start = strSource.IndexOf(next2, Start2) + next2.Length;
                start3 = Start;

                string findwordChunk = strSource.Substring(start3, strSource.Length - start3);
                string[] Chunk = findwordChunk.Split(whitespace);
                return Chunk[HowManyChunck - 1];
            }
            else
            {
                return "";
            }
        }

        public static string GetStringFromLaporan(string kode, string laporanKeuangan)
        {
            string result = "Not Found";
            List<string> namaList = new List<string>();
            namaList = parameterGet(kode);

            foreach (string nama in namaList)
            {
                //jika kalimat nama ini ada di laporan keuangan
                if (laporanKeuangan.Contains(nama))
                {
                    result = nama;
                    break;
                }
            }

            return result;
        }

        public static string GetStringFromLaporan2(string kode, string laporanKeuangan)
        {
            string result = "Not Found";
            List<string> namaList = new List<string>();
            namaList = parameterGet2(kode);

            foreach (string nama_2 in namaList)
            {

                //jika kalimat nama ini ada di laporan keuangan
                if (laporanKeuangan.Contains(nama_2))
                {
                    result = nama_2;
                    break;
                }
            }


            return result;
        }

        public static string GetStringFromLaporan3(string kode, string laporanKeuangan)
        {
            string result = "Not Found";
            List<string> namaList = new List<string>();
            namaList = parameterGet3(kode);

            foreach (string nama_3 in namaList)
            {

                //jika kalimat nama ini ada di laporan keuangan
                if (laporanKeuangan.Contains(nama_3))
                {
                    result = nama_3;
                    break;
                }
            }


            return result;
        }

        public static List<string> parameterGet(string NamaAsli)
        {
            List<string> NamaPenganti = new List<string>();
            string connStr = @"Data Source=DESKTOP-ERK0RV1\SQLEXPRESS;Initial Catalog=tugasakhir;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to SQL Server...");
                conn.Open();

                string stm = "SELECT source_ocr.nama FROM [source_ocr] JOIN [mapping] ON source_ocr.id_mapping = mapping.id WHERE mapping.nama_mapping = \'" + NamaAsli + "\'";//\""+NamaAsli+"\"

                SqlCommand command = new SqlCommand(stm, conn);
                SqlDataReader rdr = command.ExecuteReader();

                while (rdr.Read())
                {
                    NamaPenganti.Add(rdr.GetString(0)); //pada kolom ke 0 ambil string
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();

            Console.WriteLine("Done.");

            return NamaPenganti;

        }

        public static List<string> parameterGet2(string NamaAsli)
        {
            List<string> NamaPenganti = new List<string>();
            string connStr = @"Data Source=DESKTOP-ERK0RV1\SQLEXPRESS;Initial Catalog=tugasakhir;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string stm = "SELECT source_ocr.nama_2 FROM [source_ocr] JOIN [mapping] ON source_ocr.id_mapping = mapping.id WHERE mapping.nama_mapping = '" + NamaAsli + "'";
                SqlCommand cmd = new SqlCommand(stm, conn);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    NamaPenganti.Add(rdr.GetString(0)); //pada kolom ke 0 ambil string
                }

            }
            catch (Exception ex)
            {
                //   Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");

            return NamaPenganti;

        }

        public static List<string> parameterGet3(string NamaAsli)
        {
            List<string> NamaPenganti = new List<string>();
            string connStr = @"Data Source=DESKTOP-ERK0RV1\SQLEXPRESS;Initial Catalog=tugasakhir;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string stm = "SELECT source_ocr.nama_3 FROM [source_ocr] JOIN [mapping] ON source_ocr.id_mapping = mapping.id WHERE mapping.nama_mapping = " + NamaAsli + "";
                SqlCommand cmd = new SqlCommand(stm, conn);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    NamaPenganti.Add(rdr.GetString(0)); //pada kolom ke 0 ambil string
                }

            }
            catch (Exception ex)
            {
                //   Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");

            return NamaPenganti;

        }

    }
}