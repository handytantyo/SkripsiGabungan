using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Text;
using Bytescout.PDFExtractor;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using SkripsiGabungan.Models;

namespace SkripsiGabungan.Controllers
{
    public class DragAndDropController : Controller
    {
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
            extractor.RegistrationName = "demo";
            extractor.RegistrationKey = "demo";

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

        public ActionResult Result()
        {
            //TextExtractor extractor = new TextExtractor();
            //extractor.RegistrationName = "demo";
            //extractor.RegistrationKey = "demo";
            //extractor.LoadDocumentFromFile(Server.MapPath("~/UploadedFiles/default.pdf"));
            //string pdfToTxt = extractor.ToString();            
            string pdfToTxt = System.IO.File.ReadAllText(Server.MapPath("~/UploadedFiles/default.pdf"));
            //string pdfToTxt = ExtractTextFromPdf(extractor.ToString());
            //string[] words = extractor.ToString().Split('/', '.');

            string kataLabaUsaha = GetStringFromLaporan("laba usaha", pdfToTxt);
            //int iKeLabaUsaha = getIfromDB("laba usaha");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            //Console.WriteLine(kataLabaUsaha);
            //Console.ReadLine();

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
                else
                {

                }
            }

            string strLabaUsaha = new string(arrayLabaUsaha2);
            double lngLabaUsaha = Double.Parse(strLabaUsaha);

            var list = new List<OCR>();

            list.Add(new OCR()
            {
               ROE =  lngLabaUsaha//labaUsaha;
            });

            return View(list);
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

        //public ActionResult Ocr()
        //{
        //    // Create Bytescout.PDFExtractor.TextExtractor instance
        //    TextExtractor extractor = new TextExtractor();
        //    extractor.RegistrationName = "demo";
        //    extractor.RegistrationKey = "demo";

        //    // Load sample PDF document
        //    extractor.LoadDocumentFromFile("C:/Users/Nabyl/Desktop/ocr1IronPDF/ocr1IronPDF/bin/Debug/1ar-adhi2016.pdf");

        //    // Enable Optical Character Recognition (OCR)
        //    // in .Auto mode (SDK automatically checks if needs to use OCR or not)
        //    extractor.OCRMode = OCRMode.Auto;

        //    // Set the location of "tessdata" folder containing language data files
        //    extractor.OCRLanguageDataFolder = @"C:\Users\Nabyl\Desktop\ocr1IronPDF\ocr1IronPDF\tessdata";

        //    // Set OCR language
        //    extractor.OCRLanguage = "eng"; // "eng" for english, "deu" for German, "fra" for French, "spa" for Spanish etc - according to files in /tessdata

        //    // Set PDF document rendering resolution
        //    extractor.OCRResolution = 300;

        //    // Save extracted text to file
        //    extractor.SaveTextToFile("C:/Users/Nabyl/Desktop/ocr1IronPDF/ocr1IronPDF/bin/Debug/adhi16.txt");

        //    // Open output file in default associated application
        //    System.Diagnostics.Process.Start("C:/Users/Nabyl/Desktop/ocr1IronPDF/ocr1IronPDF/bin/Debug/adhi16.txt");
        //    return Json("Success..", JsonRequestBehavior.AllowGet);

        //}
    }
}