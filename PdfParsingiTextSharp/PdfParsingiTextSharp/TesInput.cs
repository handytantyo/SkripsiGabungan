using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Text;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace PdfParsingiTextSharp
{
    class TesInput
    {        

        static void Tes(string[] args)
        {
            string filepdf = "Laporan Keuangan/adhi karya/2016/1ar-adhi2016.pdf";
            string pdfToTxt = ExtractTextFromPdf(filepdf);
            string[] words = filepdf.Split('/', '.');
            string kataLabaUsaha = GetStringFromLaporan("laba usaha", pdfToTxt);
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

                string stm = "SELECT source_ocr.nama FROM [source_ocr] JOIN [mapping] ON source_ocr.id_mapping = mapping.id";
                //SqlCommand cmd = new SqlCommand(stm, conn);
                //SqlDataReader rdr = cmd.ExecuteReader();

                SqlCommand command = new SqlCommand(stm, conn);
                SqlDataReader rdr = command.ExecuteReader();

                while (rdr.Read())
                {
                    NamaPenganti.Add(rdr.GetString(0)); //pada kolom ke 0 ambil string
                    Console.WriteLine(rdr);
                    Console.WriteLine("bdasd");
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
    }
}
