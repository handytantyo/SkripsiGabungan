using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bytescout.PDFExtractor;

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
                string filePath = "default" + Path.GetExtension(file.FileName);//Guid.NewGuid() + Path.GetExtension(file.FileName);
                //string fileapa = Server.MapPath(filePath);
                //extractor.LoadDocumentFromFile(fileapa);
                //extractor.OCRMode = OCRMode.Auto;
                //extractor.OCRLanguageDataFolder = @"C:\Users\Nabyl\Desktop\ocr1IronPDF\ocr1IronPDF\tessdata";
                //extractor.OCRLanguage = "eng";
                //extractor.OCRResolution = 300;
                //extractor.SaveTextToFile(Path.Combine(Server.MapPath("~/UploadedFiles/output.txt"), filePath));
                file.SaveAs(Path.Combine(Server.MapPath("~/UploadedFiles"), filePath));//filePath
                //Here you can write code for save this information in your database if you want
                Page_Load(Path.Combine(Server.MapPath("~/UploadedFiles"), filePath), filePath);

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
            extractor.SaveTextToFile(Path.Combine(Server.MapPath("~/UploadedFiles"), namaFIle + "TextFIle"));
            Response.Write("</pre>");

            Response.End();
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