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
            long labaUsaha_ = labaUsaha(pdfToTxt);
            long labaBersih_ = labaBersih(pdfToTxt);
            long kasDanSetaraKas_ = kasDanSetaraKas(pdfToTxt);
            long penyusutan_ = penyusutan(pdfToTxt);//masih error
            long persediaan_ = persediaan(pdfToTxt);
            long AsetLancar_ = AsetLancar(pdfToTxt);
            long TotalAset_ = TotalAset(pdfToTxt);
            long Liabilitas_ = Liabilitas(pdfToTxt);
            long Ekuitas_ = Ekuitas(pdfToTxt);
            long Pendapatan_ = Pendapatan(pdfToTxt);
            long PihakBerelasi_ = PihakBerelasi(pdfToTxt);
            long PihakKetiga_ = PihakKetiga(pdfToTxt);
            long Investasi_ = Investasi(pdfToTxt);

            long CapitalEmployed = TotalAset_ - Liabilitas_;
            long PiutangUsaha = PihakBerelasi_ + PihakKetiga_;

            var Indikators = db.Indikators;

            testing_data_hasil.ROE = ((float)labaBersih_ / Ekuitas_) * 100;
            testing_data_hasil.ROI = (((float)labaUsaha_ + penyusutan_) / CapitalEmployed) * 100;
            testing_data_hasil.cash_ratio = (((float)kasDanSetaraKas_ + Investasi_) / Liabilitas_) * 100;//investasi belum
            testing_data_hasil.current_ratio = ((float)AsetLancar_ / Liabilitas_) * 100;
            testing_data_hasil.CP = ((float)PiutangUsaha / Pendapatan_) * 365;
            testing_data_hasil.PP = ((float)persediaan_ / Pendapatan_) * 365;
            testing_data_hasil.TATO = ((float)Pendapatan_ / CapitalEmployed) * 100;
            testing_data_hasil.TMS_TA = ((float)Ekuitas_ / TotalAset_) * 100;
            
            #region //ANN
            double[] input = new double[3];
            input[0] = (double)testing_data_hasil.ROE;
            input[1] = (double)testing_data_hasil.ROI;
            input[2] = (double)testing_data_hasil.cash_ratio;

            double[] weight = new double[12];
            double[] bias = new double[2];
            float outoANN;

            double[] neth = new double[3];
            float[] outh = new float[3];            

            SQLConnectionForMachineLearning(weight, bias, "ANN");
            outoANN = FeedForward(input, weight, bias, neth, outh);
            #endregion

            #region//FNN
            float[,] fuzzyInput = new float[3, 3];
            double[] norm = new double[3];
            int[] inputPopulation = new int[] { 0, 1, 2 };//ROE, ROI, cash ratio
            float outoFNN;
            
            fuzzyInput = selectedFuzzyInput(input, fuzzyInput, inputPopulation);
            norm = Tnorm(fuzzyInput);
            SQLConnectionForMachineLearning(weight, bias, "FNN");
            outoFNN = FeedForward(norm, weight, bias, neth, outh);
            #endregion

            testing_data_hasil.OutputANN = outoANN;
            testing_data_hasil.OutputFNN = outoFNN;
            testing_data_hasil.TingkatKesehatan = TingkatKesehatan((float)Math.Round(outoFNN, 1));
            testing_data_hasil.Grade = Grade((float)Math.Round(outoFNN, 1));

            if (ModelState.IsValid)
            {
                db.testing_data_hasil.Add(testing_data_hasil);
                db.SaveChanges();
                return RedirectToAction("Result", new { Id = testing_data_hasil.IDTesting });
            }

            return View(testing_data_hasil);
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

        static void SQLConnectionForMachineLearning(double[] Weight, double[] Bias, string Algorithm)
        {
            //sqlconnection from https://www.youtube.com/watch?v=IcD9Jffstmw
            string connStr = @"Data Source=DESKTOP-ERK0RV1\SQLEXPRESS;Initial Catalog=tugasakhir;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM [Indikator] WHERE Algorithm = '" + Algorithm + "'", conn);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    for (int weight = 0; weight < 12; weight++)
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
                //Console.WriteLine(ex.Message);
            }

            conn.Close();
        }

        static float[,] ROE(double[] input, float[,] roe, int parameter, int lowmedhigh)//[parameter, recordke-,lowmediumhigh]
        {
            //ROE
            float high = 13;
            float medium = 6.6F;
            float low = 1;
            if (input[parameter] >= high)
            {
                if (lowmedhigh == 0) roe[parameter, 0] = 0;
                else if (lowmedhigh == 1) roe[parameter, 1] = 0;
                else roe[parameter, 2] = 1;
            }
            else if (input[parameter] > medium)
            {
                if (lowmedhigh == 0) roe[parameter, 0] = 0;
                else if (lowmedhigh == 1) roe[parameter, 1] = (float)(high - input[parameter]) / (float)(high - medium);
                else roe[parameter, 2] = (float)(input[0] - medium) / (float)(high - medium);
            }
            else if (input[parameter] == medium)
            {
                if (lowmedhigh == 0) roe[parameter, 0] = 0;
                else if (lowmedhigh == 0) roe[parameter, 1] = 1;
                else roe[parameter, 2] = 0;
            }
            else if (input[parameter] > low)
            {
                if (lowmedhigh == 0) roe[parameter, 0] = (float)(medium - input[parameter]) / (float)(medium - low);
                else if (lowmedhigh == 1) roe[parameter, 1] = (float)(input[parameter] - 1) / (float)(medium - low);
                else roe[parameter, 2] = 0;
            }
            else
            {
                if (parameter == 0) roe[parameter, 0] = 1;
                else if (parameter == 1) roe[parameter, 1] = 0;
                else roe[parameter, 2] = 0;
            }

            return roe;
        }

        static float[,] ROI(double[] input, float[,] roi, int parameter, int lowmedhigh)
        {
            float high = 15F;
            float medium = 9F;
            float low = 1F;
            //ROI
            if (input[parameter] >= high)
            {
                if (lowmedhigh == 0) roi[parameter, 0] = 0;
                else if (lowmedhigh == 1) roi[parameter, 1] = 0;
                else roi[parameter, 2] = 1;
            }
            else if (input[parameter] > medium)
            {
                if (lowmedhigh == 0) roi[parameter, 0] = 0;
                else if (lowmedhigh == 1) roi[parameter, 1] = (float)(high - input[parameter]) / (float)(high - medium);
                else roi[parameter, 2] = (float)(input[parameter] - medium) / (float)(high - medium);
            }
            else if (input[parameter] == medium)
            {
                if (lowmedhigh == 0) roi[parameter, 0] = 0;
                else if (lowmedhigh == 1) roi[parameter, 1] = 1;
                else roi[parameter, 2] = 0;
            }
            else if (input[parameter] > low)
            {
                if (lowmedhigh == 0) roi[parameter, 0] = (float)(medium - input[parameter]) / (float)(medium - low);
                else if (lowmedhigh == 1) roi[parameter, 1] = (float)(input[parameter] - 1) / (float)(medium - low);
                else roi[parameter, 2] = 0;
            }
            else
            {
                if (lowmedhigh == 0) roi[parameter, 0] = 1;
                else if (lowmedhigh == 1) roi[parameter, 1] = 0;
                else roi[parameter, 2] = 0;
            }
            //Console.WriteLine(roi[roi_, 0]);

            return roi;
        }

        static float[,] cashRatio(double[] input, float[,] cash, int parameter, int lowmedhigh)
        {
            float high = 35F;
            float medium = 15F;
            float low = 5F;
            //cash ratio
            if (input[parameter] >= high)
            {
                if (lowmedhigh == 0) cash[parameter, 0] = 0;
                else if (lowmedhigh == 1) cash[parameter, 1] = 0;
                else cash[parameter, 2] = 1;
            }
            else if (input[parameter] > medium)
            {
                if (lowmedhigh == 0) cash[parameter, 0] = 0;
                else if (lowmedhigh == 1) cash[parameter, 1] = (float)(high - input[parameter]) / (float)(high - medium);
                else cash[parameter, 2] = (float)(input[2] - medium) / (float)(high - medium);
            }
            else if (input[parameter] == medium)
            {
                if (lowmedhigh == 0) cash[parameter, 0] = 0;
                else if (lowmedhigh == 1) cash[parameter, 1] = 1;
                else cash[parameter, 2] = 0;
            }
            else if (input[parameter] > low)
            {
                if (lowmedhigh == 0) cash[parameter, 0] = (float)(medium - input[parameter]) / (float)(medium - low);
                else if (lowmedhigh == 1) cash[parameter, 1] = (float)(input[parameter] - 5) / (float)(medium - low);
                else cash[parameter, 2] = 0;
            }
            else
            {
                if (lowmedhigh == 0) cash[parameter, 0] = 1;
                else if (lowmedhigh == 1) cash[parameter, 1] = 0;
                else cash[parameter, 2] = 0;
            }
            return cash;
        }

        static float[,] selectedFuzzyInput(double[] input, float[,] fuzzyInput, int[] inputPopulation)
        {
            for (int Parameter = 0; Parameter < 3; Parameter++)
            {
                switch (inputPopulation[Parameter])
                {
                    case 0:
                        for (int LowMedHigh = 0; LowMedHigh < 3; LowMedHigh++)
                            fuzzyInput = ROE(input, fuzzyInput, Parameter, LowMedHigh); break;
                    case 1:
                        for (int LowMedHigh = 0; LowMedHigh < 3; LowMedHigh++)
                            fuzzyInput = ROI(input, fuzzyInput, Parameter, LowMedHigh); break;
                    case 2:
                        for (int LowMedHigh = 0; LowMedHigh < 3; LowMedHigh++)
                            fuzzyInput = cashRatio(input, fuzzyInput, Parameter, LowMedHigh); break;

                }

            }
            return fuzzyInput;
        }

        static double[] Tnorm(float[,] fuzzy)//double[,]
        {
            double[] norm = new double[3];
            double[] phi = new double[3];

            phi[0] = (fuzzy[0, 2] + fuzzy[1, 2] + fuzzy[2, 2]) / 3;//high
            phi[1] = (fuzzy[0, 1] + fuzzy[1, 1] + fuzzy[2, 1]) / 3;//medium
            phi[2] = (fuzzy[0, 0] + fuzzy[1, 0] + fuzzy[2, 0]) / 3;//low

            norm[0] = phi[0] / (phi[0] + phi[1] + phi[2]);
            norm[1] = phi[1] / (phi[0] + phi[1] + phi[2]);
            norm[2] = phi[2] / (phi[0] + phi[1] + phi[2]);

            return norm;
        }

        static float FeedForward(double[] input, double[] weight, double[] bias, double[] neth, float[] outh)
        {
            int kolom = 0;
            double neto;
            float outo;

            for (int i = 0; i < 3; i++)
            {
                neth[i] = input[0] * weight[kolom] + input[1] * weight[kolom + 1] + input[2] * weight[kolom + 2] + bias[0];//sudah bener			
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

        static string TingkatKesehatan(float Hasil)
        {
            string result;
            if (Hasil >= 0.69) result = "Sehat";
            else if (Hasil >= 0.39 && Hasil < 0.7) result = "Kurang Sehat";
            else result = "Tidak Sehat";

            return result;
        }

        static string Grade(float Hasil)
        {
            string[] Grade = new string[] { "C", "CC", "CCC", "B", "BB", "BBB", "A", "AA", "AAA" };
            int array = (int)Math.Round(Hasil * 10, 1);
            string result = Grade[array - 1];

            return result;
        }

        static long labaUsaha(string pdfToTxt)
        {
            string kataLabaUsaha = GetStringFromLaporan("laba usaha", pdfToTxt);

            string labaUsaha;
            for (int i = 2; ; i++)
            {
                //if (getBetweenChunkSubPenyusutan(pdfToTxt, kata1LabaUsaha, kata2LabaUsaha, i).Length > 7 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1LabaUsaha, kata2LabaUsaha, i).ToCharArray()[0] < 58)
                //{
                //    labaUsaha = getBetweenChunkSubPenyusutan(pdfToTxt, kata1LabaUsaha, kata2LabaUsaha, i);
                //    break;
                //}

                if (getBetweenChunk(pdfToTxt, kataLabaUsaha, i).Length > 5 && getBetweenChunk(pdfToTxt, kataLabaUsaha, i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, kataLabaUsaha, i).ToCharArray()[1] < 58 && getBetweenChunk(pdfToTxt, kataLabaUsaha, i).ToCharArray()[2] < 58)
                {
                    if (getBetweenChunk(pdfToTxt, kataLabaUsaha, i).ToCharArray()[0] == 40)
                    {
                        labaUsaha = "-" + getBetweenChunk(pdfToTxt, kataLabaUsaha, i);
                        break;
                    }
                    else
                    {
                        labaUsaha = getBetweenChunk(pdfToTxt, kataLabaUsaha, i);
                        break;
                    }

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
                if (getBetweenChunk(pdfToTxt, kataLabaBersih, i).Length > 7 && getBetweenChunk(pdfToTxt, kataLabaBersih, i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, kataLabaBersih, i).ToCharArray()[1] < 58 && getBetweenChunk(pdfToTxt, kataLabaBersih, i).ToCharArray()[2] < 58)
                {
                    if (getBetweenChunk(pdfToTxt, kataLabaBersih, i).ToCharArray()[0] == 40)
                    {
                        labaBersih = "-" + getBetweenChunk(pdfToTxt, kataLabaBersih, i);
                        break;
                    }
                    else
                    {
                        labaBersih = getBetweenChunk(pdfToTxt, kataLabaBersih, i);
                        break;
                    }

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
            string[] arraykataKasSetara = GetStringFromLaporanAll2("kas dan setara kas", pdfToTxt);
            //int iKeKasSetara = getIfromDB("kas dan setara kas");
            string kataKasSetara = arraykataKasSetara[0];
            string kata2KasSetara = arraykataKasSetara[1];

            string KasSetara;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunkSubPenyusutan(pdfToTxt, kataKasSetara, kata2KasSetara, i).Length > 5 && getBetweenChunkSubPenyusutan(pdfToTxt, kataKasSetara, kata2KasSetara, i).ToCharArray()[0] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kataKasSetara, kata2KasSetara, i).ToCharArray()[1] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kataKasSetara, kata2KasSetara, i).ToCharArray()[2] < 58)
                {
                    KasSetara = getBetweenChunkSubPenyusutan(pdfToTxt, kataKasSetara, kata2KasSetara, i);
                    break;
                }

                else if (getBetweenChunk(pdfToTxt, kataKasSetara, i).Length > 5 && getBetweenChunk(pdfToTxt, kataKasSetara, i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, kataKasSetara, i).ToCharArray()[1] < 58 && getBetweenChunk(pdfToTxt, kataKasSetara, i).ToCharArray()[2] < 58)
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
            string[] contoh = GetStringFromLaporanAll("penyusutan", pdfToTxt);
            //int iKePenyusutan = getIfromDB("penyusutan");

            string kata1Penyusutan = contoh[0];
            string kata2Penyusutan = contoh[1];
            string kata3Penyusutan = contoh[2];
            //getBetweenChunk11(pdfToTxt, kata, int, iKe);

            string Penyusutan;
            for (int i = 2; ; i++)
            {
                
                if (getBetweenChunk3nama(pdfToTxt, kata1Penyusutan, kata2Penyusutan, kata3Penyusutan, i).Length > 5 && getBetweenChunk3nama(pdfToTxt, kata1Penyusutan, kata2Penyusutan, kata3Penyusutan, i).ToCharArray()[0] < 58 && getBetweenChunk3nama(pdfToTxt, kata1Penyusutan, kata2Penyusutan, kata3Penyusutan, i).ToCharArray()[1] < 58 && getBetweenChunk3nama(pdfToTxt, kata1Penyusutan, kata2Penyusutan, kata3Penyusutan, i).ToCharArray()[2] < 58)
                {
                    Penyusutan = getBetweenChunk3nama(pdfToTxt, kata1Penyusutan, kata2Penyusutan, kata3Penyusutan, i);
                } 
                else if (getBetweenChunkSubPenyusutan(pdfToTxt, kata1Penyusutan, kata2Penyusutan, i).Length > 5 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Penyusutan, kata2Penyusutan, i).ToCharArray()[0] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Penyusutan, kata2Penyusutan, i).ToCharArray()[1] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Penyusutan, kata2Penyusutan, i).ToCharArray()[2] < 58)
                {
                    Penyusutan = getBetweenChunkSubPenyusutan(pdfToTxt, kata1Penyusutan, kata2Penyusutan, i);
                    break;
                }
                else if (i > 60)
                {
                    Penyusutan = "0";
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
            string[] arraykataPersediaan = GetStringFromLaporanAll2("persediaan", pdfToTxt);
            //int iKePersediaan = getIfromDB("persediaan");

            string kata1Persediaan = arraykataPersediaan[0];
            string kata2Persediaan = arraykataPersediaan[1];
            //string kata3Persediaan = arraykataPersediaan[2];
            //getBetweenChunk11(pdfToTxt, kata, int, iKe);

            string Persediaan;

            if (kata2Persediaan == "Not Found")
            {
                Persediaan = "0";
            }
            else
            {
                for (int i = 2; ; i++)
                {
                    //if (getBetweenChunk3nama(pdfToTxt, kata1Persediaan, kata2Persediaan, kata3Persediaan, i).Length > 5 && getBetweenChunk3nama(pdfToTxt, kata1Persediaan, kata2Persediaan, kata3Persediaan, i).ToCharArray()[0] < 58 && getBetweenChunk3nama(pdfToTxt, kata1Persediaan, kata2Persediaan, kata3Persediaan, i).ToCharArray()[1] < 58 && getBetweenChunk3nama(pdfToTxt, kata1Persediaan, kata2Persediaan, kata3Persediaan, i).ToCharArray()[2] < 58)
                    //{
                    //    Persediaan = getBetweenChunk3nama(pdfToTxt, kata1Persediaan, kata2Persediaan, kata3Persediaan, i);
                    //}

                    if (getBetweenChunkSubPenyusutan(pdfToTxt, kata1Persediaan, kata2Persediaan, i).Length > 5 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Persediaan, kata2Persediaan, i).ToCharArray()[0] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Persediaan, kata2Persediaan, i).ToCharArray()[1] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Persediaan, kata2Persediaan, i).ToCharArray()[2] < 58)
                    {
                        Persediaan = getBetweenChunkSubPenyusutan(pdfToTxt, kata1Persediaan, kata2Persediaan, i);
                        break;
                    }

                    else if (getBetweenChunk(pdfToTxt, kata1Persediaan, i).Length > 5 && getBetweenChunk(pdfToTxt, kata1Persediaan, i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, kata1Persediaan, i).ToCharArray()[1] < 58 && getBetweenChunk(pdfToTxt, kata1Persediaan, i).ToCharArray()[2] < 58)
                    {
                        Persediaan = getBetweenChunk(pdfToTxt, kata1Persediaan, i);
                        break;
                    }
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


            string AsetLancar;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataAsetLancar, i).Length > 5 && getBetweenChunk(pdfToTxt, kataAsetLancar, i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, kataAsetLancar, i).ToCharArray()[1] < 58 && getBetweenChunk(pdfToTxt, kataAsetLancar, i).ToCharArray()[2] < 58)
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

        static long TotalAset(string pdfToTxt)
        {
            string[] arraykataTotalAset = GetStringFromLaporanAll2("total aset", pdfToTxt);
            //string kata2TotalAset = GetStringFromLaporan2("total aset", pdfToTxt);
            //int iKeTotalAset = getIfromDB("total aset");

            string kata1TotalAset = arraykataTotalAset[0];
            string kata2TotalAset = arraykataTotalAset[1];

            string TotalAset;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunkSubPenyusutan(pdfToTxt, kata1TotalAset, kata2TotalAset, i).Length > 5 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1TotalAset, kata2TotalAset, i).ToCharArray()[0] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1TotalAset, kata2TotalAset, i).ToCharArray()[1] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1TotalAset, kata2TotalAset, i).ToCharArray()[2] < 58)
                {
                    TotalAset = getBetweenChunkSubPenyusutan(pdfToTxt, kata1TotalAset, kata2TotalAset, i);
                    break;
                }

                //else if (getBetweenChunk(pdfToTxt, kata1TotalAset, i).Length > 5 && getBetweenChunk(pdfToTxt, kata1TotalAset, i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, kata1TotalAset, i).ToCharArray()[1] < 58 && getBetweenChunk(pdfToTxt, kata1TotalAset, i).ToCharArray()[2] < 58)
                //{
                //    TotalAset = getBetweenChunk(pdfToTxt, kata1TotalAset, i);
                //    break;
                //}
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

        static long Liabilitas(string pdfToTxt)
        {
            string[] arraykataLiabilitas = GetStringFromLaporanAll2("liabilitas jangka pendek", pdfToTxt);
            //int iKeLiabilitas = getIfromDB("liabilitas jangka pendek");

            string kata1Liabilitas = arraykataLiabilitas[0];
            string kata2Liabilitas = arraykataLiabilitas[1];

            string Liabilitas;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunkSubPenyusutan(pdfToTxt, kata1Liabilitas, kata2Liabilitas, i).Length > 5 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Liabilitas, kata2Liabilitas, i).ToCharArray()[0] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Liabilitas, kata2Liabilitas, i).ToCharArray()[1] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Liabilitas, kata2Liabilitas, i).ToCharArray()[2] < 58)
                {
                    Liabilitas = getBetweenChunkSubPenyusutan(pdfToTxt, kata1Liabilitas, kata2Liabilitas, i);
                    break;
                }

                else if (getBetweenChunk(pdfToTxt, kata1Liabilitas, i).Length > 5 && getBetweenChunk(pdfToTxt, kata1Liabilitas, i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, kata1Liabilitas, i).ToCharArray()[1] < 58 && getBetweenChunk(pdfToTxt, kata1Liabilitas, i).ToCharArray()[2] < 58)
                {
                    Liabilitas = getBetweenChunk(pdfToTxt, kata1Liabilitas, i);
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

        static long Ekuitas(string pdfToTxt)
        {
            string[] arraykataEkuitas = GetStringFromLaporanAll2("total ekuitas", pdfToTxt);
            string kata1Ekuitas = arraykataEkuitas[0];
            string kata2Ekuitas = arraykataEkuitas[1];
            //string kata3Ekuitas = arraykataEkuitas[2];

            string Ekuitas;
            for (int i = 2; ; i++)
            {
                //if (getBetweenChunk3nama(pdfToTxt, kata1Ekuitas, kata2Ekuitas, kata3Ekuitas, i).Length > 5 && getBetweenChunk3nama(pdfToTxt, kata1Ekuitas, kata2Ekuitas, kata3Ekuitas, i).ToCharArray()[0] < 58 && getBetweenChunk3nama(pdfToTxt, kata1Ekuitas, kata2Ekuitas, kata3Ekuitas, i).ToCharArray()[1] < 58 && getBetweenChunk3nama(pdfToTxt, kata1Ekuitas, kata2Ekuitas, kata3Ekuitas, i).ToCharArray()[2] < 58)
                //{
                //    Ekuitas = getBetweenChunk3nama(pdfToTxt, kata1Ekuitas, kata2Ekuitas, kata3Ekuitas, i);
                //}

                if (getBetweenChunkSubPenyusutan(pdfToTxt, kata1Ekuitas, kata2Ekuitas, i).Length > 5 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Ekuitas, kata2Ekuitas, i).ToCharArray()[0] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Ekuitas, kata2Ekuitas, i).ToCharArray()[1] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Ekuitas, kata2Ekuitas, i).ToCharArray()[2] < 58)
                {
                    Ekuitas = getBetweenChunkSubPenyusutan(pdfToTxt, kata1Ekuitas, kata2Ekuitas, i);
                    break;
                }

                //else if (getBetweenChunk(pdfToTxt, kata1Ekuitas, i).Length > 5 && getBetweenChunk(pdfToTxt, kata1Ekuitas, i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, kata1Ekuitas, i).ToCharArray()[1] < 58 && getBetweenChunk(pdfToTxt, kata1Ekuitas, i).ToCharArray()[2] < 58)
                //{
                //    Ekuitas = getBetweenChunk(pdfToTxt, kata1Ekuitas, i);
                //    break;
                //}
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

        static long Pendapatan(string pdfToTxt)
        {
            string[] arraykataPendapatan = GetStringFromLaporanAll2("total pendapatan", pdfToTxt);
            //int iKePendapatan = getIfromDB("total pendapatan");

            string kata1Pendapatan = arraykataPendapatan[0];
            string kata2Pendapatan = arraykataPendapatan[1];
            //string kata3Pendapatan = arraykataPendapatan[2];

            string Pendapatan;
            for (int i = 2; ; i++)
            {
                //if (getBetweenChunk3nama(pdfToTxt, kata1Pendapatan, kata2Pendapatan, kata3Pendapatan, i).Length > 5 && getBetweenChunk3nama(pdfToTxt, kata1Pendapatan, kata2Pendapatan, kata3Pendapatan, i).ToCharArray()[0] < 58 && getBetweenChunk3nama(pdfToTxt, kata1Pendapatan, kata2Pendapatan, kata3Pendapatan, i).ToCharArray()[1] < 58 && getBetweenChunk3nama(pdfToTxt, kata1Pendapatan, kata2Pendapatan, kata3Pendapatan, i).ToCharArray()[2] < 58)
                //{
                //    Pendapatan = getBetweenChunk3nama(pdfToTxt, kata1Pendapatan, kata2Pendapatan, kata3Pendapatan, i);
                //}

                if (getBetweenChunkSubPenyusutan(pdfToTxt, kata1Pendapatan, kata2Pendapatan, i).Length > 5 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Pendapatan, kata2Pendapatan, i).ToCharArray()[0] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Pendapatan, kata2Pendapatan, i).ToCharArray()[1] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1Pendapatan, kata2Pendapatan, i).ToCharArray()[2] < 58)
                {
                    Pendapatan = getBetweenChunkSubPenyusutan(pdfToTxt, kata1Pendapatan, kata2Pendapatan, i);
                    break;
                }

                else if (getBetweenChunk(pdfToTxt, kata1Pendapatan, i).Length > 5 && getBetweenChunk(pdfToTxt, kata1Pendapatan, i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, kata1Pendapatan, i).ToCharArray()[1] < 58 && getBetweenChunk(pdfToTxt, kata1Pendapatan, i).ToCharArray()[2] < 58)
                {
                    Pendapatan = getBetweenChunk(pdfToTxt, kata1Pendapatan, i);
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

        static long PihakBerelasi(string pdfToTxt)
        {
            string[] arraykataPihakBerelasi = GetStringFromLaporanAll("pihak berelasi", pdfToTxt);
            //int iKePihakBerelasi = getIfromDB("pihak berelasi");
            string kata1PihakBerelasi = arraykataPihakBerelasi[0];
            string kata2PihakBerelasi = arraykataPihakBerelasi[1];
            string kata3PihakBerelasi = arraykataPihakBerelasi[2];

            string PihakBerelasi = "0";

            if (kata1PihakBerelasi == "Not Found")
            {
                PihakBerelasi = "0";
            }
            else
            {
                for (int i = 2; ; i++)
                {
                    if (getBetweenChunk3nama(pdfToTxt, kata1PihakBerelasi, kata2PihakBerelasi, kata3PihakBerelasi, i).Length > 5 && getBetweenChunk3nama(pdfToTxt, kata1PihakBerelasi, kata2PihakBerelasi, kata3PihakBerelasi, i).ToCharArray()[0] < 58 && getBetweenChunk3nama(pdfToTxt, kata1PihakBerelasi, kata2PihakBerelasi, kata3PihakBerelasi, i).ToCharArray()[1] < 58 && getBetweenChunk3nama(pdfToTxt, kata1PihakBerelasi, kata2PihakBerelasi, kata3PihakBerelasi, i).ToCharArray()[2] < 58)
                    {
                        PihakBerelasi = getBetweenChunk3nama(pdfToTxt, kata1PihakBerelasi, kata2PihakBerelasi, kata3PihakBerelasi, i);
                        break;
                    }

                    else if (getBetweenChunkSubPenyusutan(pdfToTxt, kata1PihakBerelasi, kata2PihakBerelasi, i).Length >= 5 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1PihakBerelasi, kata2PihakBerelasi, i).ToCharArray()[0] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1PihakBerelasi, kata2PihakBerelasi, i).ToCharArray()[1] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1PihakBerelasi, kata2PihakBerelasi, i).ToCharArray()[2] < 58)
                    {
                        PihakBerelasi = getBetweenChunkSubPenyusutan(pdfToTxt, kata1PihakBerelasi, kata2PihakBerelasi, i);
                        break;
                    }
                    else if (i > 60)
                    {
                        PihakBerelasi = "0";
                    }
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
            string[] arraykataPihakKetiga = GetStringFromLaporanAll("pihak ketiga", pdfToTxt);
            //int iKePihakBerelasi = getIfromDB("pihak berelasi");
            string kata1PihakKetiga = arraykataPihakKetiga[0];
            string kata2PihakKetiga = arraykataPihakKetiga[1];
            string kata3PihakKetiga = arraykataPihakKetiga[2];

            string PihakKetiga = "0";

            if (kata1PihakKetiga == "Not Found")
            {
                PihakKetiga = "0";
            }
            else
            {
                for (int i = 2; ; i++)
                {
                    if (getBetweenChunk3nama(pdfToTxt, kata1PihakKetiga, kata2PihakKetiga, kata3PihakKetiga, i).Length > 5 && getBetweenChunk3nama(pdfToTxt, kata1PihakKetiga, kata2PihakKetiga, kata3PihakKetiga, i).ToCharArray()[0] < 58 && getBetweenChunk3nama(pdfToTxt, kata1PihakKetiga, kata2PihakKetiga, kata3PihakKetiga, i).ToCharArray()[1] < 58 && getBetweenChunk3nama(pdfToTxt, kata1PihakKetiga, kata2PihakKetiga, kata3PihakKetiga, i).ToCharArray()[2] < 58)
                    {
                        PihakKetiga = getBetweenChunk3nama(pdfToTxt, kata1PihakKetiga, kata2PihakKetiga, kata3PihakKetiga, i);
                        break;
                    }

                    else if (getBetweenChunkSubPenyusutan(pdfToTxt, kata1PihakKetiga, kata2PihakKetiga, i).Length > 5 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1PihakKetiga, kata2PihakKetiga, i).ToCharArray()[0] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kata1PihakKetiga, kata2PihakKetiga, i).ToCharArray()[1] < 58)
                    {
                        PihakKetiga = getBetweenChunkSubPenyusutan(pdfToTxt, kata1PihakKetiga, kata2PihakKetiga, i);
                        break;
                    }
                    else if (i > 60)
                    {
                        PihakKetiga = "0";
                    }
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
            string arrayKataInvestasi = GetStringFromLaporan("investasi jangka pendek", pdfToTxt);
            string kata1Investasi = arrayKataInvestasi;

            string Investasi;

            if (kata1Investasi == "Not Found")
            {
                Investasi = "0";
            }
            else
            {
                for (int i = 2; ; i++)
                {
                    if (getBetweenChunk(pdfToTxt, kata1Investasi, i).Length > 4 && getBetweenChunk(pdfToTxt, kata1Investasi, i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, kata1Investasi, i).ToCharArray()[1] < 58 && getBetweenChunk(pdfToTxt, kata1Investasi, i).ToCharArray()[2] < 58)
                    {
                        Investasi = getBetweenChunk(pdfToTxt, kata1Investasi, i);
                        break;
                    }
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

        static void nominal(long lngKasSetara, long lngLabaBersih, long lngLabaUsaha, long lngLiabilitas, long lngPendapatan, long lngPenyusutan, long lngPersediaan, long lngAsetLancar, long lngTotalAset, long lngEkuitas, long lngPihakBerelasi, long lngPihakKetiga, long lngInvestasi, string pdfToTxt)
        {
            string[] jutaanRupiah = GetStringFromLaporanAll("jutaan rupiah", pdfToTxt);
            string kataJutaanRupiah = jutaanRupiah[0];

            if (jutaanRupiah[0] == "Not Found")
            {
                lngKasSetara = lngKasSetara * 1;
                lngLabaBersih = lngLabaBersih * 1;
                lngLabaUsaha = lngLabaUsaha * 1;
                lngLiabilitas = lngLiabilitas * 1;
                lngPendapatan = lngPendapatan * 1;
                lngPenyusutan = lngPenyusutan * 1;
                lngPersediaan = lngPersediaan * 1;
                lngAsetLancar = lngAsetLancar * 1;
                lngTotalAset = lngTotalAset * 1;
                lngEkuitas = lngEkuitas * 1;
                lngPihakBerelasi = lngPihakBerelasi * 1;
                lngPihakKetiga = lngPihakKetiga * 1;
                lngInvestasi = lngInvestasi * 1;
            }
            else
            {
                lngKasSetara = lngKasSetara * 1000000;
                lngKasSetara = lngKasSetara * 1000000;
                lngLabaBersih = lngLabaBersih * 1000000;
                lngLabaUsaha = lngLabaUsaha * 1000000;
                lngLiabilitas = lngLiabilitas * 1000000;
                lngPendapatan = lngPendapatan * 1000000;
                lngPenyusutan = lngPenyusutan * 1000000;
                lngPersediaan = lngPersediaan * 1000000;
                lngAsetLancar = lngAsetLancar * 1000000;
                lngTotalAset = lngTotalAset * 1000000;
                lngEkuitas = lngEkuitas * 1000000;
                lngPihakBerelasi = lngPihakBerelasi * 1000000;
                lngPihakKetiga = lngPihakKetiga * 1000000;
                lngInvestasi = lngInvestasi * 1000000;
            }

            string[] ribuanRupiah = GetStringFromLaporanAll("ribuan rupiah", pdfToTxt);
            string kataRibuanRupiah = ribuanRupiah[0];

            if (ribuanRupiah[0] == "Not Found")
            {
                lngKasSetara = lngKasSetara * 1;
                lngLabaBersih = lngLabaBersih * 1;
                lngLabaUsaha = lngLabaUsaha * 1;
                lngLiabilitas = lngLiabilitas * 1;
                lngPendapatan = lngPendapatan * 1;
                lngPenyusutan = lngPenyusutan * 1;
                lngPersediaan = lngPersediaan * 1;
                lngAsetLancar = lngAsetLancar * 1;
                lngTotalAset = lngTotalAset * 1;
                lngEkuitas = lngEkuitas * 1;
                lngPihakBerelasi = lngPihakBerelasi * 1;
                lngPihakKetiga = lngPihakKetiga * 1;
                lngInvestasi = lngInvestasi * 1;
            }
            else
            {
                lngKasSetara = lngKasSetara * 1000;
                lngLabaBersih = lngLabaBersih * 1000;
                lngLabaUsaha = lngLabaUsaha * 1000;
                lngLiabilitas = lngLiabilitas * 1000;
                lngPendapatan = lngPendapatan * 1000;
                lngPenyusutan = lngPenyusutan * 1000;
                lngPersediaan = lngPersediaan * 1000;
                lngAsetLancar = lngAsetLancar * 1000;
                lngTotalAset = lngTotalAset * 1000;
                lngEkuitas = lngEkuitas * 1000;
                lngPihakBerelasi = lngPihakBerelasi * 1000;
                lngPihakKetiga = lngPihakKetiga * 1000;
                lngInvestasi = lngInvestasi * 1000;
            }
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
            int Start3 = 0;
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

        public static string[] GetStringFromLaporanAll(string kode, string laporanKeuangan)
        {
            string[] result = new string[3]{ "Not Found", "Not Found", "Not Found" };
            List<string> namalist1 = new List<string>();
            List<string> namaList2 = new List<string>();
            List<string> namaList3 = new List<string>();
            namalist1 = parameterGet(kode);
            namaList2 = parameterGet2(kode);
            namaList3 = parameterGet3(kode);
            int index = 0;
            foreach (string nama_1 in namalist1)
            {
                if (laporanKeuangan.Contains(nama_1))
                {                   
                    int j = 0;
                    foreach (string nama_2 in namaList2)
                    {
                        if (laporanKeuangan.Contains(nama_2) && (j == index))
                        {
                            int k = 0;
                            foreach (string nama_3 in namaList3)
                            {
                             
                                //jika kalimat nama ini ada di laporan keuangan
                                if (laporanKeuangan.Contains(nama_3) && (j == index) && (k == index))
                                {
                                    result[0] = nama_1;
                                     result[1] = nama_2;
                                    result[2] =nama_3;
                                    return result;
                                    //break;
                                }
                                k++;
                            }
                        }
                        j++;
                    }
                }
                index++;
            }

            return result;
        }

        public static string[] GetStringFromLaporanAll2(string kode, string laporanKeuangan)
        {
            string[] result = new string[2] { "Not Found", "Not Found"};
            List<string> namalist1 = new List<string>();
            List<string> namaList2 = new List<string>();
            namalist1 = parameterGet(kode);
            namaList2 = parameterGet2(kode);
            int index = 0;
            foreach (string nama_1 in namalist1)
            {
                if (laporanKeuangan.Contains(nama_1))
                {
                    int j = 0;
                    foreach (string nama_2 in namaList2)
                    {
                        if (laporanKeuangan.Contains(nama_2) && (j == index))
                        {

                            result[0] = nama_1;
                            result[1] = nama_2;
                        }
                        j++;
                    }
                }
                index++;
            }

            return result;
        }
    }
}