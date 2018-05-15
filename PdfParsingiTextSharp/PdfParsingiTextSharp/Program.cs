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

    class Program
    {
        static void Main(string[] args)
        {
            //Console.Write("Masukan nama file : ");
            //string namaFile = Console.ReadLine();

            
            string filepdf = "Laporan Keuangan/adhi karya/2016/1ar-adhi2016.pdf";
            string pdfToTxt=ExtractTextFromPdf(filepdf);
            string[] words = filepdf.Split('/', '.');

            string kataLabaUsaha = GetStringFromLaporan("laba usaha", pdfToTxt);
            //int iKeLabaUsaha = getIfromDB("laba usaha");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            Console.WriteLine(kataLabaUsaha);
            Console.ReadLine();

            string labaUsaha;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataLabaUsaha, i).Length > 7 && getBetweenChunk(pdfToTxt, kataLabaUsaha, i).ToCharArray()[0] < 58)
                {
                    labaUsaha = getBetweenChunk(pdfToTxt, kataLabaUsaha, i);
                    break;
                }
            }

            Console.WriteLine(labaUsaha);
            Console.WriteLine();

            string kataKasSetara = GetStringFromLaporan("kas dan setara kas", pdfToTxt);
            //int iKeKasSetara = getIfromDB("kas dan setara kas");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            Console.WriteLine(kataKasSetara);
            Console.ReadLine();

            string KasSetara;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataKasSetara, i).Length > 7 && getBetweenChunk(pdfToTxt, kataKasSetara, i).ToCharArray()[0] < 58)
                {
                    KasSetara = getBetweenChunk(pdfToTxt, kataKasSetara, i);
                    break;
                }
            }
            Console.WriteLine(KasSetara);
            Console.WriteLine();

            string kataBebanUmum = GetStringFromLaporan("penyusutan", pdfToTxt);
            string kataPenyusutan = GetStringFromLaporan2("penyusutan", pdfToTxt);
            //int iKePenyusutan = getIfromDB("penyusutan");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            Console.WriteLine(kataBebanUmum);
            Console.ReadLine();

            string Penyusutan;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunkSubPenyusutan(pdfToTxt, kataBebanUmum, kataPenyusutan, i).Length > 7 && getBetweenChunkSubPenyusutan(pdfToTxt, kataBebanUmum, kataPenyusutan, i).ToCharArray()[0] < 58 && getBetweenChunkSubPenyusutan(pdfToTxt, kataBebanUmum, kataPenyusutan, i).ToCharArray()[1] < 58)
                {
                    Penyusutan = getBetweenChunkSubPenyusutan(pdfToTxt, kataBebanUmum, kataPenyusutan, i);
                    break;
                }
            }

            Console.WriteLine(Penyusutan);

            string kataPersediaan = GetStringFromLaporan("persediaan", pdfToTxt);
            //int iKePersediaan = getIfromDB("persediaan");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            Console.WriteLine(kataPersediaan);
            Console.ReadLine();

            string Persediaan;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataPersediaan, i).Length > 7 && getBetweenChunk(pdfToTxt, kataPersediaan, i).ToCharArray()[0] < 58)
                {
                    Persediaan = getBetweenChunk(pdfToTxt, kataPersediaan, i);
                    break;
                }
            }

            string kataAsetLancar = GetStringFromLaporan("aset lancar", pdfToTxt);
            //int iKeAsetLancar = getIfromDB("aset lancar");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            Console.WriteLine(kataAsetLancar);
            Console.ReadLine();

            string AsetLancar;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataAsetLancar, i).Length > 7 && getBetweenChunk(pdfToTxt, kataAsetLancar, i).ToCharArray()[0] < 58)
                {
                    AsetLancar = getBetweenChunk(pdfToTxt, kataAsetLancar, i);
                    break;
                }
            }
            //Console.WriteLine(KasSetara);
            Console.ReadLine();

            string kataTotalAset = GetStringFromLaporan("total aset", pdfToTxt);
            //int iKeTotalAset = getIfromDB("total aset");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            Console.WriteLine(kataTotalAset);
            Console.ReadLine();

            string TotalAset;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataTotalAset, i).Length > 7 && getBetweenChunk(pdfToTxt, kataTotalAset, i).ToCharArray()[0] < 58)
                {
                    TotalAset = getBetweenChunk(pdfToTxt, kataTotalAset, i);
                    break;
                }
            }

            string kataLiabilitas = GetStringFromLaporan("liabilitas jangka pendek", pdfToTxt);
           // int iKeLiabilitas = getIfromDB("liabilitas jangka pendek");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            Console.WriteLine(kataLiabilitas);
            Console.ReadLine();

            string Liabilitas;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataLiabilitas, i).Length > 7 && getBetweenChunk(pdfToTxt, kataLiabilitas, i).ToCharArray()[0] < 58)
                {
                    Liabilitas = getBetweenChunk(pdfToTxt, kataLiabilitas, i);
                    break;
                }
            }

            string kataEkuitas = GetStringFromLaporan("total ekuitas", pdfToTxt);
            //int iKeEkuitas = getIfromDB("total ekuitas");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            Console.WriteLine(kataEkuitas);
            Console.ReadLine();

            string Ekuitas;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataEkuitas, i).Length > 7 && getBetweenChunk(pdfToTxt, kataEkuitas, i).ToCharArray()[0] < 58)
                {
                    Ekuitas = getBetweenChunk(pdfToTxt, kataEkuitas, i);
                    break;
                }
            }

            string kataPendapatan = GetStringFromLaporan("total pendapatan", pdfToTxt);
            //int iKePendapatan = getIfromDB("total pendapatan");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            Console.WriteLine(kataPendapatan);
            Console.ReadLine();

            string Pendapatan;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataPendapatan, i).Length > 7 && getBetweenChunk(pdfToTxt, kataPendapatan, i).ToCharArray()[0] < 58)
                {
                    Pendapatan = getBetweenChunk(pdfToTxt, kataPendapatan, i);
                    break;
                }
            }

            string kataPihakBerelasi = GetStringFromLaporan("pihak berelasi", pdfToTxt);
            //int iKePihakBerelasi = getIfromDB("pihak berelasi");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            Console.WriteLine(kataPihakBerelasi);
            Console.ReadLine();

            string PihakBerelasi;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataPihakBerelasi, i).Length > 7 && getBetweenChunk(pdfToTxt, kataPihakBerelasi, i).ToCharArray()[0] < 58)
                {
                    PihakBerelasi = getBetweenChunk(pdfToTxt, kataPihakBerelasi, i);
                    break;
                }
            }

            string kataPihakKetiga = GetStringFromLaporan("pihak ketiga", pdfToTxt);
            //int iKePihakKetiga = getIfromDB("pihak ketiga");

            //getBetweenChunk11(pdfToTxt, kata, int, iKe);
            Console.WriteLine(kataPihakKetiga);
            Console.ReadLine();

            string PihakKetiga;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, kataPihakKetiga, i).Length > 7 && getBetweenChunk(pdfToTxt, kataPihakKetiga, i).ToCharArray()[0] < 58)
                {
                    PihakKetiga = getBetweenChunk(pdfToTxt, kataPihakKetiga, i);
                    break;
                }
            }

            //Id Perusahaan
            int id_perusahaan = 0;
            string perusahaan = words[1];
            if (perusahaan == "biofarma")
            {
                id_perusahaan = 1;
            }
            else if (perusahaan == "krakatau steel")
            {
                id_perusahaan = 2;
            }
            else if (perusahaan == "ninda karya")
            {
                id_perusahaan = 3;
            }
            else if (perusahaan == "perkebunan nusantara v")
            {
                id_perusahaan = 4;
            }
            else if (perusahaan == "perum peruri")
            {
                id_perusahaan = 5;
            }
            else if (perusahaan == "pos indonesia")
            {
                id_perusahaan = 6;
            }
            else if (perusahaan == "pupuk sriwidjaya")
            {
                id_perusahaan = 7;
            }
            else if (perusahaan == "rajawali nusantara indonesia")
            {
                id_perusahaan = 8;
            }
            else if (perusahaan == "sarinah")
            {
                id_perusahaan = 9;
            }
            else if (perusahaan == "wijaya karya")
            {
                id_perusahaan = 10;
            }
            else if (perusahaan == "adhi karya")
            {
                id_perusahaan = 11;
            }
            else if (perusahaan == "brantas abipraya")
            {
                id_perusahaan = 12;
            }
            else if (perusahaan == "garuda indonesia")
            {
                id_perusahaan = 13;
            }
            else if (perusahaan == "hutama karya")
            {
                id_perusahaan = 14;
            }
            else if (perusahaan == "industri telekomunikasi indonesia")
            {
                id_perusahaan = 15;
            }
            else if (perusahaan == "jasa marga")
            {
                id_perusahaan = 16;
            }
            else if (perusahaan == "len")
            {
                id_perusahaan = 17;
            }
            else if (perusahaan == "perkebunan nusantara x")
            {
                id_perusahaan = 18;
            }
            else if (perusahaan == "semen baturaja")
            {
                id_perusahaan = 19;
            }
            else if (perusahaan == "waskita karya")
            {
                id_perusahaan = 20;
            }
            Console.WriteLine(id_perusahaan);

            //Id Tahun

            char[] id_tahun1 = words[2].ToCharArray();
            int arrayTahun = 4;
            char[] id_tahun2 = new char[arrayTahun];
            
            for (int i = words[2].Length - 1; i >= words[2].Length - 4; i--)
            {
                id_tahun2[arrayTahun - 1] = id_tahun1[i];
                arrayTahun--;
            }
            Console.WriteLine(id_tahun1);

            int id_tahun = 0;
            if (id_tahun2[0] == '2' && id_tahun2[1] == '0' && id_tahun2[2] == '1' && id_tahun2[3] == '2')
            {
                id_tahun = 1;
            }
            else if (id_tahun2[0] == '2' && id_tahun2[1] == '0' && id_tahun2[2] == '1' && id_tahun2[3] == '3')
            {
                id_tahun = 2;
            }
            else if (id_tahun2[0] == '2' && id_tahun2[1] == '0' && id_tahun2[2] == '1' && id_tahun2[3] == '4')
            {
                id_tahun = 3;
            }
            else if (id_tahun2[0] == '2' && id_tahun2[1] == '0' && id_tahun2[2] == '1' && id_tahun2[3] == '5')
            {
                id_tahun = 4;
            }
            else if (id_tahun2[0] == '2' && id_tahun2[1] == '0' && id_tahun2[2] == '1' && id_tahun2[3] == '6')
            {
                id_tahun = 5;
            }
            //string tahun = new string(id_tahun2);
            //int id_tahun = Int32.Parse(tahun)-2011;
            //Console.WriteLine(id_tahun);

            #region Pdf to TXT
            //string strKas;
            //Console.Write("Input Kas dan Setara Kas: ");
            //strKas = Console.ReadLine();

            //string strBank;
            //Console.Write("Input Bank: ");
            //strBank = Console.ReadLine();

            //string file = @"Laporan Keuangan/adhi karya/adhi16.txt";
            //using (StreamWriter writer = new StreamWriter(file))
            //{
            //    writer.Write(pdfToTxt);
            //}
            //Console.WriteLine("Data Saved Successfully!");

            // string textUji =getBetween1(pdfToTxt, "Kas dan Setara Kas");

            // Console.WriteLine("Hasil : {0}",textUji);
            //string source = "Aku adalah anak gembala gembala selalu riang serta gembala gembira";

            //1 Word
            //string[] arraySource = pdfToTxt.Split();

            //var matchQuery = from word in arraySource
            //                 where word == "Kas"
            //                 select word;
            //int countWord = matchQuery.Count();

            //Console.WriteLine(countWord);

            ////1 Sentence
            //countWord = 0;
            //foreach (Match m in Regex.Matches(pdfToTxt, "Kas dan Setara Kas"))
            //{
            //    countWord++;
            //}

            //Console.WriteLine(countWord);
            #endregion

            //string Coba = getBeforeChunk(pdfToTxt, "Jumlah Bank Total", 2);
            //Console.WriteLine(Coba);
            //Console.ReadLine();

            //Data Kas dan Setara Kas

            string hasilKasSetara;
            
            for (int i = 2; ; i++)
            {
                if(getBetweenChunk(pdfToTxt, "Kas dan Setara Kas", i).Length > 7 && getBetweenChunk(pdfToTxt, "Kas dan Setara Kas", i).ToCharArray()[0] < 58)
                {
                    hasilKasSetara = getBetweenChunk(pdfToTxt, "Kas dan Setara Kas", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "Kas dan setara kas", i).Length > 6 && getBetweenChunk(pdfToTxt, "Kas dan setara kas", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Kas dan setara kas", i).ToCharArray()[1]< 58)
                {
                    hasilKasSetara = getBetweenChunk(pdfToTxt, "Kas dan setara kas", i);
                    break;
                }
                   

            }
            

            char[] arrayKasSetara = hasilKasSetara.ToCharArray();
            char[] arrayKasSetara2 = new char[arrayKasSetara.Length];
            int nextKasSetara = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for(int i = 0; i < arrayKasSetara.Length; i++)
            {
                char letterKasSetara = arrayKasSetara[i];
                //Console.WriteLine(letter);
                if(arrayKasSetara[i] != '.' && arrayKasSetara[i] != ',')
                {
                    arrayKasSetara2[nextKasSetara] = arrayKasSetara[i];
                    nextKasSetara++;
                }
                else
                {

                }
            }

            //Data Penyusutan

            string hasilPenyusutan;

            for (int i = 2; ; i++)
            {
                
                if (getBetweenChunk(pdfToTxt, "Penyusutan, amortisasi dan rugi", i).Length > 4 && getBetweenChunk(pdfToTxt, "Penyusutan, amortisasi dan rugi", i).ToCharArray()[0] > 40 && getBetweenChunk(pdfToTxt, "Penyusutan, amortisasi dan rugi", i).ToCharArray()[0] < 58)
                {
                    hasilPenyusutan = getBetweenChunk(pdfToTxt, "Penyusutan, amortisasi dan rugi", i);
                    break;
                }
                else if (getBetweenChunk2(pdfToTxt, "Penyusutan dan amortisasi", i).Length > 4 && getBetweenChunk2(pdfToTxt, "Penyusutan dan amortisasi", i).ToCharArray()[0] > 40 && getBetweenChunk2(pdfToTxt, "Penyusutan dan amortisasi", i).ToCharArray()[0] < 58)
                {
                    hasilPenyusutan = getBetweenChunk2(pdfToTxt, "Penyusutan dan amortisasi", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "Penyusutan dan amortisasi", i).Length > 4 && getBetweenChunk(pdfToTxt, "Penyusutan dan amortisasi", i).ToCharArray()[0] > 40 && getBetweenChunk(pdfToTxt, "Penyusutan dan amortisasi", i).ToCharArray()[0] < 58)
                {
                    hasilPenyusutan = getBetweenChunk(pdfToTxt, "Penyusutan dan amortisasi", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "Penyusutan", i).Length > 7 && getBetweenChunk(pdfToTxt, "Penyusutan", i).ToCharArray()[0] > 40 && getBetweenChunk(pdfToTxt, "Penyusutan", i).ToCharArray()[0] < 58)
                {
                    hasilPenyusutan = getBetweenChunk(pdfToTxt, "Penyusutan", i);
                    break;
                }
            }

            char[] arrayPenyusutan = hasilPenyusutan.ToCharArray();
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
                else
                {

                }
            }

            //Data Persediaan            
            string hasilPersediaan;
            for (int i = 2; ; i++)
            {
                //if (getBetweenChunk(pdfToTxt, "Jumlah aset lancar", i).Length > 7 && getBetweenChunk(pdfToTxt, "Jumlah aset lancar", i).ToCharArray()[0] < 58)
                //{
                //    hasilPersediaan = getBetweenChunk(pdfToTxt, "Jumlah aset lancar", i);
                //    break;
                //}
                if (getBetweenChunk(pdfToTxt, "Persediaan", i).Length > 6 && getBetweenChunk(pdfToTxt, "Persediaan", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Persediaan", i).ToCharArray()[1] < 58)
                {
                        hasilPersediaan = getBetweenChunk(pdfToTxt, "Persediaan", i);
                        break;
                   
                }
                else if (getBetweenChunk2(pdfToTxt, "Persediaan", i).Length > 7 && getBetweenChunk2(pdfToTxt, "Persediaan", i).ToCharArray()[1] < 58) 
                {
                    hasilPersediaan = getBetweenChunk2(pdfToTxt, "Persediaan", i);
                    break;
                }
                else if (i > 40)
                {
                    hasilPersediaan = "0";
                    break;
                }

            }
            

            //string hasilPersediaan;
            //for (int i = 2; ; i++)
            //{
            //    if (getBetweenChunk(pdfToTxt, "Persediaan", i).Length > 7 && getBetweenChunk(pdfToTxt, "Persediaan", i).ToCharArray()[0] < 58)
            //    {
            //        hasilPersediaan = getBetweenChunk(pdfToTxt, "Persediaan", i);
            //        break;
            //    }
            //    else if(getBetweenChunk(pdfToTxt, "Jumlah aset lancar", i).Length > 7 && getBetweenChunk(pdfToTxt, "Jumlah aset lancar", i).ToCharArray()[0] < 58)
            //    {
            //        hasilPersediaan = getBetweenChunk(pdfToTxt, "Jumlah aset lancar", i);
            //        break;
            //    }
            //    else
            //    {
            //        hasilPersediaan = i.ToString();//getBetweenChunk2(pdfToTxt, "Persediaan", i);//getBetweenChunk(pdfToTxt, "Jumlah aset lancar", 8);
            //        //hasilPersediaan = "0";
            //        break;
            //    }
            //}

            char[] arrayPersediaan = hasilPersediaan.ToCharArray();
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
                else
                {

                }
            }

            //Data Total Aset Lancar
            string hasilAsetLancar;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, "Total Aset Lancar", i).Length > 6 && getBetweenChunk(pdfToTxt, "Total Aset Lancar", i).ToCharArray()[0] < 58) 
                {
                    hasilAsetLancar = getBetweenChunk(pdfToTxt, "Total Aset Lancar", i);
                    break;
                }
                else if(getBetweenChunk(pdfToTxt, "Jumlah Aset Lancar", i).Length > 7 && getBetweenChunk(pdfToTxt, "Jumlah Aset Lancar", i).ToCharArray()[0] < 58)
                {
                    hasilAsetLancar = getBetweenChunk(pdfToTxt, "Jumlah Aset Lancar", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "Jumlah aset lancar", i).Length > 7 && getBetweenChunk(pdfToTxt, "Jumlah aset lancar", i).ToCharArray()[0] < 58)
                {
                    hasilAsetLancar = getBetweenChunk(pdfToTxt, "Jumlah aset lancar", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "JUMLAH ASET LANCAR", i).Length > 7 && getBetweenChunk(pdfToTxt, "JUMLAH ASET LANCAR", i).ToCharArray()[0] < 58)
                {
                    hasilAsetLancar = getBetweenChunk(pdfToTxt, "JUMLAH ASET LANCAR", i);
                    break;
                }
            }
            
            char[] arrayAsetLancar = hasilAsetLancar.ToCharArray();
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
                else
                {

                }
            }

            //Data Total Aset
            string hasilTotalAset;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, "JUMLAH ASET", i).Length > 7 && getBetweenChunk(pdfToTxt, "JUMLAH ASET", i).ToCharArray()[0] < 58)
                {
                    hasilTotalAset = getBetweenChunk(pdfToTxt, "JUMLAH ASET", i);
                    
                    break; //disini
                }
                else if (getBetweenChunk(pdfToTxt, "TOTAL ASET", i).Length > 7 && getBetweenChunk(pdfToTxt, "TOTAL ASET", i).ToCharArray()[0] < 58)
                {
                    hasilTotalAset = getBetweenChunk(pdfToTxt, "TOTAL ASET", i);
                    break; //disini
                }
            }

            char[] arrayTotalAset = hasilTotalAset.ToCharArray();
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
                else
                {

                }
            }
            
            //Data Liabilitas Jangka Pendek
            string hasilLiabilitas;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, "Total Liabilitas a ngka Pendek", i).Length > 5 && getBetweenChunk(pdfToTxt, "Total Liabilitas a ngka Pendek", i).ToCharArray()[0] < 58)
                {
                    hasilLiabilitas = getBetweenChunk(pdfToTxt, "Total Liabilitas a ngka Pendek", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "Total Liabilitas Jangka Pendek", i).Length > 7 && getBetweenChunk(pdfToTxt, "Total Liabilitas Jangka Pendek", i).ToCharArray()[0] < 58)
                {
                    hasilLiabilitas = getBetweenChunk(pdfToTxt, "Total Liabilitas Jangka Pendek", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "Jumlah Liabilitas Jangka Pendek", i).Length > 7 && getBetweenChunk(pdfToTxt, "Jumlah Liabilitas Jangka Pendek", i).ToCharArray()[0] < 58)
                {
                    hasilLiabilitas = getBetweenChunk(pdfToTxt, "Jumlah Liabilitas Jangka Pendek", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "Jumlah liabilitas jangka pendek", i).Length > 7 && getBetweenChunk(pdfToTxt, "Jumlah liabilitas jangka pendek", i).ToCharArray()[0] < 58)
                {
                    hasilLiabilitas = getBetweenChunk(pdfToTxt, "Jumlah liabilitas jangka pendek", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "TOTAL LIABILITAS JANGKA PENDEK", i).Length > 7 && getBetweenChunk(pdfToTxt, "TOTAL LIABILITAS JANGKA PENDEK", i).ToCharArray()[0] < 58)
                {
                    hasilLiabilitas = getBetweenChunk(pdfToTxt, "TOTAL LIABILITAS JANGKA PENDEK", i);
                    break;
                }
                //else if (getBetweenChunk(pdfToTxt, "Jumlah Jangka Pendek", i).Length > 7 && getBetweenChunk(pdfToTxt, "Jumlah Jangka Pendek", i).ToCharArray()[0] < 58)
                //{
                //    hasilLiabilitas = getBetweenChunk(pdfToTxt, "Jumlah Jangka Pendek", i);
                //    break;
                //}
            }
            //Console.WriteLine("Benar yah");

            char[] arrayLiabilitas = hasilLiabilitas.ToCharArray();
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
                else
                {

                }
            }

            //Data Ekuitas (Ada dua match found)
            string hasilEkuitas;
            for (int i = 2; ; i++)
            {

                if (getBetweenChunk(pdfToTxt, "Non-controllin Interests", i).Length > 5 && getBetweenChunk(pdfToTxt, "Non-controllin Interests", i).ToCharArray()[0] < 58) //Krakatau Steel 16
                {
                    hasilEkuitas = getBetweenChunk(pdfToTxt, "Non-controllin Interests", i);
                    //Console.WriteLine(hasilEkuitas);                        
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "EKUITAS, NETO", i).Length > 6 && getBetweenChunk(pdfToTxt, "EKUITAS, NETO", i).ToCharArray()[0] < 58) //Krakatau Steel
                {
                    hasilEkuitas = getBetweenChunk(pdfToTxt, "EKUITAS, NETO", i);
                    //Console.WriteLine(hasilEkuitas);                        
                    break;
                }
                else if (getBetweenChunk2(pdfToTxt, "Total Ekuitas", i).Length > 7 && getBetweenChunk2(pdfToTxt, "Total Ekuitas", i).ToCharArray()[0] < 58)
                {
                    hasilEkuitas = getBetweenChunk2(pdfToTxt, "Total Ekuitas", i);
                    //Console.WriteLine(hasilEkuitas);                        
                    break;
                }

                else if (getBetweenChunk(pdfToTxt, "Non Controlling Interest", i).Length > 7 && getBetweenChunk(pdfToTxt, "Non Controlling Interest", i).ToCharArray()[0] < 58)
                {
                    hasilEkuitas = getBetweenChunk(pdfToTxt, "Non Controlling Interest", i);
                    //Console.WriteLine(hasilEkuitas);
                    break;
                }
                //else if (getBetweenChunk(pdfToTxt, "Jumlah Ekuitas", i).Length > 7 && getBetweenChunk(pdfToTxt, "Jumlah Ekuitas", i).ToCharArray()[0] < 58) //Inti 16
                //{
                //    Console.WriteLine("tes");
                //    hasilEkuitas = getBetweenChunk(pdfToTxt, "Jumlah Ekuitas", i);
                //    //Console.WriteLine(hasilEkuitas);
                //    break;

                //}
                else if (getBetweenChunk2(pdfToTxt, "Jumlah Ekuitas", i).Length > 7)
                {
                    
                   /* if (getBetweenChunk2(pdfToTxt, "Jumlah Ekuitas", i).ToCharArray()[0] < 58)
                    {
                        Console.WriteLine("tes");
                        hasilEkuitas = getBetweenChunk2(pdfToTxt, "Jumlah Ekuitas", i);
                        //Console.WriteLine(hasilEkuitas);
                        break;

                    }*/
                    hasilEkuitas = getBetweenChunk(pdfToTxt, "Jumlah Ekuitas", 7);
                    //Console.WriteLine("123");
                    /* else if (getBetweenChunk(pdfToTxt, "Jumlah Ekuitas", i).ToCharArray()[0] < 58) //Inti 2016
                     {
                         hasilEkuitas = getBetweenChunk(pdfToTxt, "Jumlah Ekuitas", 8);
                         //Console.WriteLine(hasilEkuitas);
                         break;
                     }*/

                }
                    else if (getBetweenChunk(pdfToTxt, "JUMLAH EKUITAS", i).Length > 7 && getBetweenChunk(pdfToTxt, "JUMLAH EKUITAS", i).ToCharArray()[0] < 58)
                    {
                        hasilEkuitas = getBetweenChunk(pdfToTxt, "JUMLAH EKUITAS", i);
                        //Console.WriteLine(hasilEkuitas);
                        break;
                    }
                    else if (getBetweenChunk(pdfToTxt, "Ekuitas yang dapat", i).Length > 7 && getBetweenChunk(pdfToTxt, "Ekuitas yang dapat", i).ToCharArray()[0] < 58)
                    {
                        hasilEkuitas = getBetweenChunk(pdfToTxt, "Ekuitas yang dapat", i);
                        //Console.WriteLine(hasilEkuitas);
                        break;
                    }                

            }
            

            char[] arrayEkuitas = hasilEkuitas.ToCharArray();
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
                else
                {

                }
            }

            //Data Pendapatan Usaha
            string hasilPendapatan;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, "PENDAPATAN NETO", i).Length > 7 && getBetweenChunk(pdfToTxt, "PENDAPATAN NETO", i).ToCharArray()[0] < 58)

                {
                    hasilPendapatan = getBetweenChunk(pdfToTxt, "PENDAPATAN NETO", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "PENDAPATAN", i).Length > 7 && getBetweenChunk(pdfToTxt, "PENDAPATAN", i).ToCharArray()[0] < 58)

                {
                    hasilPendapatan = getBetweenChunk(pdfToTxt, "PENDAPATAN", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "Total Pendapatan Usaha", i).Length > 7 && getBetweenChunk(pdfToTxt, "Total Pendapatan Usaha", i).ToCharArray()[0] < 58)
                {
                    hasilPendapatan = getBetweenChunk(pdfToTxt, "Total Pendapatan Usaha", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "Pendapatan Usaha", i).Length > 7 && getBetweenChunk(pdfToTxt, "Pendapatan Usaha", i).ToCharArray()[0] < 58)
                {
                    hasilPendapatan = getBetweenChunk(pdfToTxt, "Pendapatan Usaha", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "Pendapatan", i).Length > 7 && getBetweenChunk(pdfToTxt, "Pendapatan", i).ToCharArray()[0] < 58)
                {
                    hasilPendapatan = getBetweenChunk(pdfToTxt, "Pendapatan", i);
                    break;
                }
            }
            
            char[] arrayPendapatan = hasilPendapatan.ToCharArray();
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
                else
                {

                }
            }
            //Data Laba Usaha
            string hasilLabaUsaha;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, "Laba Sebelum Pajak dan Beban Keuangan", i).Length > 7 && getBetweenChunk(pdfToTxt, "Laba Sebelum Pajak dan Beban Keuangan", i).ToCharArray()[0] < 58)
                {
                    hasilLabaUsaha = getBetweenChunk(pdfToTxt, "Laba Sebelum Pajak dan Beban Keuangan", i);
                    break;

                }
                else if (getBetweenChunk(pdfToTxt, "LABA USAHA", i).Length > 7 && getBetweenChunk(pdfToTxt, "LABA USAHA", i).ToCharArray()[0] < 58)
                {
                    hasilLabaUsaha = getBetweenChunk(pdfToTxt, "LABA USAHA", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "Laba Sebelum Pajak", i).Length > 7 && getBetweenChunk(pdfToTxt, "Laba Sebelum Pajak", i).ToCharArray()[0] < 58)
                {
                    hasilLabaUsaha = getBetweenChunk(pdfToTxt, "Laba Sebelum Pajak", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "LABA (RUGI) SEBELUM PAJAK", i).Length > 7)
                {                    
                    if (getBetweenChunk(pdfToTxt, "LABA (RUGI) SEBELUM PAJAK", i).ToCharArray()[0] == 40) 
                    {
                        hasilLabaUsaha = "-" + getBetweenChunk(pdfToTxt, "LABA (RUGI) SEBELUM PAJAK", i);
                        break;
                    }
                    else if (getBetweenChunk(pdfToTxt, "LABA (RUGI) SEBELUM PAJAK", i).ToCharArray()[0] < 58)
                    {
                        hasilLabaUsaha = getBetweenChunk(pdfToTxt, "LABA (RUGI) SEBELUM PAJAK", i);
                        break;
                    }
                }
                else if (getBetweenChunk(pdfToTxt, "LABA (RUGI) SEBELUM BEBAN", i).Length > 6) //Krakatau Steel 12
                {
                    if (getBetweenChunk(pdfToTxt, "LABA (RUGI) SEBELUM BEBAN", i).ToCharArray()[0] == 40)
                    {
                        if (getBetweenChunk(pdfToTxt, "LABA (RUGI) SEBELUM BEBAN", i).ToCharArray()[1] < 58)
                        {
                            hasilLabaUsaha = "-" + getBetweenChunk(pdfToTxt, "LABA (RUGI) SEBELUM BEBAN", i);
                            break;
                        }
                        
                    }
                    else if (getBetweenChunk(pdfToTxt, "LABA (RUGI) SEBELUM BEBAN", i).ToCharArray()[0] < 58)
                    {
                        hasilLabaUsaha = getBetweenChunk(pdfToTxt, "LABA (RUGI) SEBELUM BEBAN", i);
                        break;
                    }
                    //Console.WriteLine("ahai");
                }
                else if (getBetweenChunk(pdfToTxt, "RUGI SEBELUM BEBAN", i).Length > 5) //Krakatau Steel 13, 14
                {
                    if (getBetweenChunk(pdfToTxt, "RUGI SEBELUM BEBAN", i).ToCharArray()[0] == 40)
                    {
                        if (getBetweenChunk(pdfToTxt, "RUGI SEBELUM BEBAN", i).ToCharArray()[1] < 58)
                        {
                            hasilLabaUsaha = "-" + getBetweenChunk(pdfToTxt, "RUGI SEBELUM BEBAN", i);
                            break;
                        }

                    }
                    else if (getBetweenChunk(pdfToTxt, "RUGI SEBELUM BEBAN", i).ToCharArray()[0] < 58)
                    {
                        hasilLabaUsaha = getBetweenChunk(pdfToTxt, "RUGI SEBELUM BEBAN", i);
                        break;
                    }
                }
                else if (getBetweenChunk(pdfToTxt, "RUGI SEBELUM PAJAK FINAL", i).Length > 5) //Krakatau Steel 16
                {
                    if (getBetweenChunk(pdfToTxt, "RUGI SEBELUM PAJAK FINAL", i).ToCharArray()[0] == 40)
                    {
                        hasilLabaUsaha = "-" + getBetweenChunk(pdfToTxt, "RUGI SEBELUM PAJAK FINAL", i);
                        break;
                    }
                    else if (getBetweenChunk(pdfToTxt, "RUGI SEBELUM PAJAK FINAL", i).ToCharArray()[0] < 58)
                    {
                        hasilLabaUsaha = getBetweenChunk(pdfToTxt, "RUGI SEBELUM PAJAK FINAL", i);
                        break;
                    }
                }
                else if (getBetweenChunk(pdfToTxt, "RUGI SE ELU PAJAK     ", i).Length > 5) //Krakatau Steel 15
                {
                    if (getBetweenChunk(pdfToTxt, "RUGI SE ELU PAJAK     ", i).ToCharArray()[0] == 40)
                    {
                        hasilLabaUsaha = "-" + getBetweenChunk(pdfToTxt, "RUGI SE ELU PAJAK     ", i);
                        break;
                    }
                    else if (getBetweenChunk(pdfToTxt, "RUGI SE ELU PAJAK     ", i).ToCharArray()[0] < 58)
                    {
                        hasilLabaUsaha = getBetweenChunk(pdfToTxt, "RUGI SE ELU PAJAK     ", i);
                        break;
                    }
                }
                else if (getBetweenChunk(pdfToTxt, "Rugi Sebelum Pajak", i).Length > 7 && getBetweenChunk(pdfToTxt, "Rugi Sebelum Pajak", i).ToCharArray()[0] < 58)
                {
                    hasilLabaUsaha = "-" + getBetweenChunk(pdfToTxt, "Rugi Sebelum Pajak", i);
                    break;
                }
            }
            

            char[] arrayLabaUsaha = hasilLabaUsaha.ToCharArray();
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
            //Data Laba Bersih
            string hasilLabaBersih;
            for (int i = 2; ; i++)
            {
                if (getBetweenChunk(pdfToTxt, "LABA BERSIH TAHUN BERJALAN", i).Length > 7 && getBetweenChunk(pdfToTxt, "LABA BERSIH TAHUN BERJALAN", i).ToCharArray()[0] < 58)
                {
                    hasilLabaBersih = getBetweenChunk(pdfToTxt, "LABA BERSIH TAHUN BERJALAN", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "LABA TAHUN BERJALAN", i).Length > 7 && getBetweenChunk(pdfToTxt, "LABA TAHUN BERJALAN", i).ToCharArray()[0] < 58)
                {
                    hasilLabaBersih = getBetweenChunk(pdfToTxt, "LABA TAHUN BERJALAN", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "LABA PERIODE BERJALAN", i).Length > 7 && getBetweenChunk(pdfToTxt, "LABA PERIODE BERJALAN", i).ToCharArray()[0] < 58)
                {
                    hasilLabaBersih = getBetweenChunk(pdfToTxt, "LABA PERIODE BERJALAN", i);
                    break;
                }
                else if (getBetweenChunk(pdfToTxt, "LABA (RUGI) TAHUN BERJALAN", i).Length > 5)
                {
                    if (getBetweenChunk(pdfToTxt, "LABA (RUGI) TAHUN BERJALAN", i).ToCharArray()[0] == 40)
                    {
                        hasilLabaBersih = "-" + getBetweenChunk(pdfToTxt, "LABA (RUGI) TAHUN BERJALAN", i);
                        break;
                    }
                    else if (getBetweenChunk(pdfToTxt, "LABA (RUGI) TAHUN BERJALAN", i).ToCharArray()[0] < 58)
                    {
                        hasilLabaBersih = getBetweenChunk(pdfToTxt, "LABA (RUGI) TAHUN BERJALAN", i);
                        break;
                    }
                }
                else if (getBetweenChunk(pdfToTxt, "RUGI TAHUN BERJALAN", i).Length > 5) //Krakatau Steel 13, 14, 16
                {
                    if (getBetweenChunk(pdfToTxt, "RUGI TAHUN BERJALAN", i).ToCharArray()[0] == 40)
                    {
                        hasilLabaBersih = "-" + getBetweenChunk(pdfToTxt, "RUGI TAHUN BERJALAN", i);
                        break;
                    }
                    else if (getBetweenChunk(pdfToTxt, "RUGI TAHUN BERJALAN", i).ToCharArray()[0] < 58)
                    {
                        hasilLabaBersih = getBetweenChunk(pdfToTxt, "RUGI TAHUN BERJALAN", i);
                        break;
                    }
                }
                else if (getBetweenChunk(pdfToTxt, "RUGI TA UN  ERJALAN", i).Length > 5) //Krakatau Steel 15
                {
                    if (getBetweenChunk(pdfToTxt, "RUGI TA UN  ERJALAN", i).ToCharArray()[0] == 40)
                    {
                        hasilLabaBersih = "-" + getBetweenChunk(pdfToTxt, "RUGI TA UN  ERJALAN", i);
                        break;
                    }
                    else if (getBetweenChunk(pdfToTxt, "RUGI TA UN  ERJALAN", i).ToCharArray()[0] < 58)
                    {
                        hasilLabaBersih = getBetweenChunk(pdfToTxt, "RUGI TA UN  ERJALAN", i);
                        break;
                    }
                }
                else if (getBetweenChunk(pdfToTxt, "Rugi Tahun Berjalan", i).Length > 7 && getBetweenChunk(pdfToTxt, "Rugi Tahun Berjalan", i).ToCharArray()[0] < 58)
                {
                    hasilLabaBersih = getBetweenChunk(pdfToTxt, "Rugi Tahun Berjalan", i);
                    break;
                }
            }
            

            char[] arrayLabaBersih = hasilLabaBersih.ToCharArray();
            char[] arrayLabaBersih2 = new char[arrayLabaBersih.Length];
            int nextLabaBersih = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayLabaBersih.Length; i++)
            {
                char letterLabaBersih = arrayLabaBersih[i];
                //Console.WriteLine(letter);
                if (arrayLabaBersih[i] != '.' && arrayLabaBersih[i] != ',' && arrayLabaBersih[i] != '(' && arrayLabaBersih[i] != ')')
                {
                    arrayLabaBersih2[nextLabaBersih] = arrayLabaBersih[i];
                    nextLabaBersih++;
                }
                else
                {

                }
            }
            ////Data Bank
            //string hasilBank;
            //if (getBeforeChunk(pdfToTxt, "Jumlah Bank Total", 2).Length > 7 && getBeforeChunk(pdfToTxt, "Jumlah Bank Total", 2).ToCharArray()[0] < 58)
            //{
            //    hasilBank = getBeforeChunk(pdfToTxt, "Jumlah Bank Total", 2);
            //}
            
            //else
            //{
            //    for (int i = 2; ; i++)
            //    {

            //        if (getBetweenChunkSubKas(pdfToTxt, "Bank", "Sub Jumlah", i).Length > 7 && getBetweenChunkSubKas(pdfToTxt, "Bank", "Sub Jumlah", i).ToCharArray()[0] < 58) //Inti 15
            //        {
            //            hasilBank = getBetweenChunkSubKas(pdfToTxt, "Bank", "Sub Jumlah", i);
            //            //Console.WriteLine(i);
            //            break;
            //        }
            //        else if (getBetweenChunk(pdfToTxt, "Jumlah bank", i).Length > 7 && getBetweenChunk(pdfToTxt, "Jumlah bank", i).ToCharArray()[0] < 58)
            //        {
            //            hasilBank = getBetweenChunk(pdfToTxt, "Jumlah bank", i);
            //            break;
            //        }
            //        //else if (getBetweenChunkKas(pdfToTxt, "Bank ", i).Length > 7 && getBetweenChunkKas(pdfToTxt, "Bank ", i).ToCharArray()[0] < 58)
            //        //{
            //        //    hasilBank = getBetweenChunkKas(pdfToTxt, "Bank ", i);
            //        //    Console.WriteLine(i);
            //        //    break;
            //        //}
                    
            //        else if(i>40)
            //        {
            //            for(int j=2; ; j++)
            //            {
            //                if (getBetweenChunkKas(pdfToTxt, "Bank ", j).Length > 7 && getBetweenChunkKas(pdfToTxt, "Bank ", j).ToCharArray()[0] < 58)
            //                {
            //                    hasilBank = getBetweenChunkKas(pdfToTxt, "Bank ", j);
            //                    Console.WriteLine(j);
            //                    break;
            //                }                            
            //            }
            //            break;
            //        }

            //        //else if (getBetweenChunk(pdfToTxt, "Penerimaan Pinjaman Bank", i).Length > 7 && getBetweenChunk(pdfToTxt, "Penerimaan Pinjaman Bank", i).ToCharArray()[0] < 58)
            //        //{
            //        //    hasilBank = getBetweenChunk(pdfToTxt, "Penerimaan Pinjaman Bank", i);
            //        //    break;
            //        //}

            //    }
            //}
            
            ////if ( && getBetweenChunkBank(pdfToTxt, "Bank", 2).ToCharArray()[0] < 58)
            ////{
            ////    hasilBank = getBetweenChunkBank(pdfToTxt, "Bank", 2);
            ////}
            ////else
            ////{
            ////    hasilBank = "0";
            ////}

            //char[] arrayBank = hasilBank.ToCharArray();
            //char[] arrayBank2 = new char[arrayBank.Length];
            //int nextBank = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            //for (int i = 0; i < arrayBank.Length; i++)
            //{
            //    char letterBank = arrayBank[i];
            //    //Console.WriteLine(letter);
            //    if (arrayBank[i] != '.' && arrayBank[i] != ',' && arrayBank[i] != '(' && arrayBank[i] != ')')
            //    {
            //        arrayBank2[nextBank] = arrayBank[i];
            //        nextBank++;
            //    }
            //    else
            //    {

            //    }
            //}
            ////Data Kas
            //string hasilKas;
            //for (int i = 2; ; i++)
            //{
            //    if (getBetweenChunk11(pdfToTxt, "Kas", i).Length > 7 && getBetweenChunk11(pdfToTxt, "Kas", i).ToCharArray()[0] < 58 && getBetweenChunk11(pdfToTxt, "Kas", i).ToCharArray()[1] < 58)
            //    {
            //        hasilKas = getBetweenChunk11(pdfToTxt, "Kas", i);
            //        //Console.WriteLine("ahai");
            //        break;
            //    }
            //    else if (getBetweenChunkSubKas(pdfToTxt,"Kas", "Sub Jumlah", i).Length > 7 && getBetweenChunkSubKas(pdfToTxt, "Kas", "Sub Jumlah",i).ToCharArray()[0] < 58)
            //    {
            //        hasilKas = getBetweenChunkSubKas(pdfToTxt, "Kas", "Sub Jumlah", i);
            //        //Console.WriteLine(i);
            //        break;
            //    }
            //}
            
            //char[] arrayKas = hasilKas.ToCharArray();
            //char[] arrayKas2 = new char[arrayKas.Length];
            //int nextKas = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            //for (int i = 0; i < arrayKas.Length; i++)
            //{
            //    char letterKas = arrayKas[i];
            //    //Console.WriteLine(letter);
            //    if (arrayKas[i] != '.' && arrayKas[i] != ',' && arrayKas[i] != '(' && arrayKas[i] != ')')
            //    {
            //        arrayKas2[nextKas] = arrayKas[i];
            //        nextKas++;
            //    }
            //    else
            //    {

            //    }
            //}
            //Console.WriteLine("Benar");
            ////Data Pihak Berelasi
            ////Console.WriteLine((char)65);
            
            ////char a = ')';
            ////if((int)a != 40)
            ////{
            ////    Console.WriteLine("benar");
            ////}
            ////else
            ////{
            ////    Console.WriteLine("salah"); 
            ////}
            Console.Out.Flush();
            //hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi", 10);
            string hasilPihakBerelasi = "0";
            int cekPihakBerelasi = 0;
            abovePihakBerelasi:

            switch(cekPihakBerelasi)
            {

                case 0:
                    for (int i = 2; i < 40; i++)
                    {
                        //if (getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Related Parties", 2).ToCharArray()[0] != 40)
                        //{
                        //    hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi", i);
                        //    break;
                        //}
                        if (getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i).ToCharArray()[0] < 58)
                        {
                            hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i);
                            break;
                        }
                    }
                    if (hasilPihakBerelasi == "0")
                    {
                        cekPihakBerelasi++;
                        goto abovePihakBerelasi;
                    }
                    break;
                case 1:
                    for (int i = 2; i < 40; i++)
                    {
                        //if (getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Related Parties", 2).ToCharArray()[0] != 40)
                        //{
                        //    hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi", i);
                        //    break;
                        //}
                        if (getBetweenChunk(pdfToTxt, "Pihak berelasi", i).Length > 5 && getBetweenChunk(pdfToTxt, "Pihak berelasi", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Pihak berelasi", i).ToCharArray()[1] < 58)
                        {
                            hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak berelasi", i);
                            break;
                        }
                        //else if (getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i).ToCharArray()[0] < 58)
                        //{
                        //    hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i);
                        //    break;
                        //}
                    }
                    if (hasilPihakBerelasi == "0")
                    {
                        cekPihakBerelasi++;
                        goto abovePihakBerelasi;
                    }
                    break;

                case 2:
                    for (int i = 2; i < 40; i++)
                    {
                        //if (getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Related Parties", 2).ToCharArray()[0] != 40)
                        //{
                        //    hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi", i);
                        //    break;
                        //}
                        if (getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Related Parties", 2).ToCharArray()[0] != 40)
                        {
                            hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi", i);
                            break;
                        }
                        //if (getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i).ToCharArray()[0] < 58)
                        //{
                        //    hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i);
                        //    break;
                        //}
                    }
                    if (hasilPihakBerelasi == "0")
                    {
                        cekPihakBerelasi++;
                        goto abovePihakBerelasi;
                    }
                    break;
                case 3:
                    for (int i = 2; i < 100; i++)
                    {
                        if (getBetweenChunk(pdfToTxt, "Pihak Berelasi Related Parties", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Berelasi Related Parties", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Pihak Berelasi Related Parties", i + 1).Length > 4)
                        {
                            hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi Related Parties", i);
                            break;
                        }
                    }
                    if (hasilPihakBerelasi == "0")
                    {
                        cekPihakBerelasi++;
                        goto abovePihakBerelasi;
                    }
                    break;
                case 4:
                    for (int i = 2; i < 100; i++)
                    {
                        if (getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i + 1).Length > 4)
                        {
                            hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi", i);
                            break;
                        }
                    }
                    if (hasilPihakBerelasi == "0")
                    {
                        cekPihakBerelasi++;
                        goto abovePihakBerelasi;
                    }
                    break;

                
                
                

                //case 3:
                //    for (int i = 2; i < 40; i++)
                //    {
                //        if (getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Related Parties", 2).ToCharArray()[0] != 40)
                //        {
                //            hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi", i);
                //            break;
                //        }
                //    }
                //    if (hasilPihakBerelasi == "0")
                //    {
                //        cekPihakBerelasi++;
                //        goto abovePihakBerelasi;
                //    }
                //    break;
                //case 3:
                //    for (int i = 2; i < 40; i++)
                //    {
                //        if (getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i).ToCharArray()[0] < 58)
                //        {
                //            hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i);
                //        }
                //    }
                //    if (hasilPihakBerelasi == "0")
                //    {
                //        cekPihakBerelasi++;
                //        goto abovePihakBerelasi;
                //    }
                //    break;
                case 5:
                    hasilPihakBerelasi = "0";
                    break;


            }
            //Console.WriteLine("benar");
            //for (int i = 2; ; i++)
            //{
            //    if (getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i).ToCharArray()[0] < 58)
            //    {
            //        hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i);
            //        break;
            //    }
            //    else if (getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Related Parties", 2).ToCharArray()[0] != 40)
            //    {
            //        hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi", i);
            //        break;
            //    }
            //    else if (getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i + 1).Length > 4)
            //    {
            //        hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi", i);
            //        break;
            //    }//try
            //     //{
            //     //    if (getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i).ToCharArray()[0] < 58)
            //     //    {
            //     //        hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak-pihak Berelasi", i);
            //     //        break;
            //     //    }
            //     //    else if (getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Related Parties", 2).ToCharArray()[0] != 40)
            //     //    {
            //     //        hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi", i);
            //     //        break;
            //     //    }
            //     //    else if (getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i + 1).Length > 4)
            //     //    {
            //     //        hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi", i);
            //     //        break;
            //     //    }
            //     //}
            //     //catch
            //     //{
            //     //    if (getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Berelasi", i).ToCharArray()[0] < 58)
            //     //    {
            //     //        hasilPihakBerelasi = getBetweenChunk(pdfToTxt, "Pihak Berelasi", i);
            //     //        break;
            //     //    }

            //    //    else if (i > 40)
            //    //    {
            //    //        hasilPihakBerelasi = "0";
            //    //        break;
            //    //    }
            //    //}

            //}

            //Console.WriteLine("benar");
            char[] arrayPihakBerelasi = hasilPihakBerelasi.ToCharArray();
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
                else
                {

                }
            }
            long lngHasilPihakBerelasi;
            string strHasilPihakBerelasi = new string(arrayPihakBerelasi2);
            try
            {
                lngHasilPihakBerelasi = Int64.Parse(strHasilPihakBerelasi);
            }
            catch
            {
                cekPihakBerelasi++;
                goto abovePihakBerelasi;
            }

            string hasilPihakKetiga = "0";
            int cekPihakKetiga = 0;
            abovePihakKetiga:

            switch (cekPihakKetiga)
            {
                case 0:
                    for (int i = 2; i < 40; i++)
                    {
                        if (getBetweenChunk(pdfToTxt, "Pihak Ketiga 4", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Ketiga 4", i).ToCharArray()[0] < 58)
                        {
                            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak Ketiga 4", i);
                            break;
                        }

                    }
                    if (hasilPihakKetiga == "0")
                    {
                        cekPihakKetiga++;
                        goto abovePihakKetiga;
                    }
                    break;
                case 1:
                    for (int i = 2; i < 40; i++)
                    {
                        if (getBetweenChunk(pdfToTxt, "Pihak-pihak Ketiga", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak-pihak Ketiga", i).ToCharArray()[0] < 58)
                        {
                            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak-pihak Ketiga", i);
                            break;
                        }
                        //else if (getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).ToCharArray()[0] < 58)
                        //{
                        //    hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak Ketiga", i);
                        //    break;
                        //}


                        //else if (getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Third Parties", 2).ToCharArray()[0] != 40)
                        //{
                        //    hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak Ketiga", i);
                        //    break;
                        //}

                    }
                    if (hasilPihakKetiga == "0")
                    {
                        cekPihakKetiga++;
                        goto abovePihakKetiga;
                    }
                    break;
                case 2:
                    for (int i = 2; i < 40; i++)
                    {
                        if (getBetweenChunk(pdfToTxt, "Pihak ketiga", i).Length > 5 && getBetweenChunk(pdfToTxt, "Pihak ketiga", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Pihak ketiga", i).ToCharArray()[1] < 58 && getBetweenChunk(pdfToTxt, "Pihak ketiga", i).ToCharArray()[2] != 44) //Krakatau Steel 15
                        {
                            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak ketiga", i);
                            break;
                        }
                        
                    }
                    if (hasilPihakKetiga == "0")
                    {
                        cekPihakKetiga++;
                        goto abovePihakKetiga;
                    }
                    break;
                case 3:
                    for (int i = 2; i < 100; i++)
                    {
                        if (getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i + 1).Length > 4)
                        {
                            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak Ketiga", i);
                            break;
                        }
                    }
                    if (hasilPihakKetiga == "0")
                    {
                        cekPihakKetiga++;
                        goto abovePihakKetiga;
                    }
                    break;
                case 4:
                    for (int i = 2; i < 40; i++)
                    {
                        if (getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Third Parties", 2).ToCharArray()[0] != 40)
                        {
                            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak Ketiga", i);
                            break;
                        }

                    }
                    if (hasilPihakKetiga == "0")
                    {
                        cekPihakKetiga++;
                        goto abovePihakKetiga;
                    }
                    break;
                case 5:
                    for (int i = 2; i < 40; i++)
                    {
                        //if (getBetweenChunk(pdfToTxt, "Pihak-pihak Ketiga", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak-pihak Ketiga", i).ToCharArray()[0] < 58)
                        //{
                        //    hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak-pihak Ketiga", i);
                        //    break;
                        //}

                        if (getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Third Parties", 2).ToCharArray()[0] != 40)
                        {
                            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak Ketiga", i);
                            break;
                        }

                    }
                    if (hasilPihakKetiga == "0")
                    {
                        cekPihakKetiga++;
                        goto abovePihakKetiga;
                    }
                    break;
                
                case 6:
                    for (int i = 2; i < 40; i++)
                    {
                        if (getBetweenChunk(pdfToTxt, "Piutang usaha", i).Length > 8 && getBetweenChunk(pdfToTxt, "Piutang usaha", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Piutang usaha", i).ToCharArray()[1] < 58)
                        {
                            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Piutang usaha", i);
                            //hasilPihakKetiga = i.ToString();
                            break;
                        }
                    }
                    if (hasilPihakKetiga == "0")
                    {
                        cekPihakKetiga++;
                        goto abovePihakKetiga;
                    }
                    break;

                case 7:
                    hasilPihakKetiga = "0";
                    break;
            }
            //Data Pihak Ketiga
            //string hasilPihakKetiga;
            //    for (int i = 2; ; i++)
            //    {
            //        if (getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Third Parties", 2).ToCharArray()[0] != 40)
            //        {
            //            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak Ketiga", i);
            //            break;
            //        }
            //        else if (getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i + 1).Length > 4)
            //        {
            //            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak Ketiga", i);
            //            break;
            //        }

            //    }//try
            //    {
            //        if (getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Third Parties", 2).ToCharArray()[0] != 40)
            //        {
            //            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak Ketiga", i);
            //            break;
            //        }
            //    }
            //        catch
            //    {
            //        if (getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i + 1).Length > 4)
            //        {
            //            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak Ketiga", i);
            //            break;
            //        }

            //        else if (getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).Length > 8 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Pihak Ketiga", i).ToCharArray()[1] < 58)
            //        {
            //            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Pihak Ketiga", i);
            //            break;
            //        }
            //        else if (getBetweenChunk(pdfToTxt, "Piutang usaha", i).Length > 8 && getBetweenChunk(pdfToTxt, "Piutang usaha", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Piutang usaha", i).ToCharArray()[1] < 58)
            //        {
            //            hasilPihakKetiga = getBetweenChunk(pdfToTxt, "Piutang usaha", i);
            //            //hasilPihakKetiga = i.ToString();
            //            break;
            //        }
            //    }
            //    Console.WriteLine("Benar");

            //}
            //Console.WriteLine("Benar");

            char[] arrayPihakKetiga = hasilPihakKetiga.ToCharArray();
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
                else
                {

                }
            }
            long lngHasilPihakKetiga;
            string strHasilPihakKetiga = new string(arrayPihakKetiga2);
            try
            {
                lngHasilPihakKetiga = Int64.Parse(strHasilPihakKetiga);
            }
            catch
            {
                cekPihakKetiga++;
                goto abovePihakKetiga;
            }

            string hasilInvestasi = "0";
            int cekInvestasi = 0;
            aboveInvestasi:

            switch (cekInvestasi)
            {
                case 0:
                    for (int i = 2; i < 40; i++)
                    {
                        if (getBetweenChunk(pdfToTxt, "Investasi jangka pendek 2e,2g,5,32,36", i).Length > 3 && getBetweenChunk(pdfToTxt, "Investasi jangka pendek 2e,2g,5,32,36", i).ToCharArray()[0] < 58) //Krakatau Steel 12
                        {
                            hasilInvestasi = getBetweenChunk(pdfToTxt, "Investasi jangka pendek 2e,2g,5,32,36", i);
                            break;
                        }

                    }
                    if (hasilInvestasi == "0")
                    {
                        cekInvestasi++;
                        goto aboveInvestasi;
                    }
                    break;
                
                case 1:
                    for (int i = 2; i < 40; i++)
                    {
                        if (getBetweenChunk(pdfToTxt, "Investasi jangka pendek", i) == "-" && getBetweenChunk(pdfToTxt, "Investasi jangka pendek", i + 3).ToCharArray()[0] < 58) //Krakatau Steel 14, 15
                        {
                            
                                hasilInvestasi = "00";
                                break;
                            
                        }

                    }
                    if (hasilInvestasi == "0")
                    {
                        cekInvestasi++;
                        goto aboveInvestasi;
                    }
                    break;
                case 2:
                    for (int i = 2; i < 40; i++)
                    {
                        if (getBetweenChunk(pdfToTxt, "Investasi jangka pendek", i).Length > 3 && getBetweenChunk(pdfToTxt, "Investasi jangka pendek", i).ToCharArray()[0] < 58 && getBetweenChunk(pdfToTxt, "Investasi jangka pendek", i).ToCharArray()[1] < 58) //Krakatau Steel 15
                        {
                            hasilInvestasi = getBetweenChunk(pdfToTxt, "Investasi jangka pendek", i);
                            break;
                        }

                    }
                    if (hasilInvestasi == "0")
                    {
                        cekInvestasi++;
                        goto aboveInvestasi;
                    }
                    break;
                //case 1:
                //    for (int i = 2; i < 40; i++)
                //    {
                //        if (getBetweenChunk(pdfToTxt, "Investasi jangka pendek", i).Length > 5 && getBetweenChunk(pdfToTxt, "Investasi jangka pendek", i).ToCharArray()[0] < 58) //Krakatau Steel 15
                //        {
                //            hasilInvestasi = getBetweenChunk(pdfToTxt, "Investasi jangka pendek", i);
                //            break;
                //        }

                //    }
                //    if (hasilInvestasi == "0")
                //    {
                //        cekInvestasi++;
                //        goto aboveInvestasi;
                //    }
                //    break;
                
                case 3:
                    hasilPihakKetiga = "0";
                    break;
            }

            char[] arrayInvestasi = hasilInvestasi.ToCharArray();
            char[] arrayInvestasi2 = new char[arrayInvestasi.Length];
            int nextInvestasi = 0; //Untuk Menghilangkan titik (.) dan koma (,)

            for (int i = 0; i < arrayInvestasi.Length; i++)
            {
                char letterInvestasi = arrayInvestasi[i];
                //Console.WriteLine(letter);
                if (arrayInvestasi[i] != '.' && arrayInvestasi[i] != ',' && arrayInvestasi[i] != '(' && arrayInvestasi[i] != ')')
                {
                    arrayInvestasi2[nextInvestasi] = arrayInvestasi[i];
                    nextInvestasi++;
                }
                else
                {

                }
            }
            long lngInvestasi;
            string strInvestasi = new string(arrayInvestasi2);
            try
            {
                lngInvestasi = Int64.Parse(strInvestasi);
            }
            catch
            {
                cekInvestasi++;
                goto aboveInvestasi;
            }

            //Data Capital Employed
            string stringTotalAset = new string(arrayTotalAset2);
            long longTotalAset = Int64.Parse(stringTotalAset);
            Convert.ToInt64(longTotalAset);
            string stringLiabilitas = new string(arrayLiabilitas2);
            long longLiabilitas = Int64.Parse(stringLiabilitas);
            Convert.ToInt64(longLiabilitas);
            long CapitalEmployed = longTotalAset - longLiabilitas;

            //Data PiutangUsaha
            //string strPihakBerelasi = new string(arrayPihakBerelasi2);
            //long lngPihakBerelasi = Int64.Parse(strPihakBerelasi);
            //Convert.ToInt64(lngPihakBerelasi);
            //string strPihakKetiga = new string(arrayPihakKetiga2);
            //long lngPihakKetiga = Int64.Parse(strPihakKetiga);
            //Convert.ToInt64(lngPihakKetiga);
            long PiutangUsaha = lngHasilPihakBerelasi + lngHasilPihakKetiga;

            //Data ROE
            string strLabaBersih = new string(arrayLabaBersih2);
            long lngLabaBersih = Int64.Parse(strLabaBersih);
            Convert.ToInt64(lngLabaBersih);
            string strEkuitas = new string(arrayEkuitas2);
            long lngEkuitas = Int64.Parse(strEkuitas);
            Convert.ToInt64(lngEkuitas);
            float ROE = ((float)lngLabaBersih / lngEkuitas) * 100;

            //Data ROI --> rumus aslinya (Laba Bersih + penyusutan) / Capital Employed
            string strTotalAset = new string(arrayTotalAset2);
            long lngTotalAset = Int64.Parse(strTotalAset);
            Convert.ToInt64(lngTotalAset);
            string strPenyusutan = new string(arrayPenyusutan2);
            long lngPenyusutan = Int64.Parse(strPenyusutan);
            string strLabaUsaha = new string(arrayLabaUsaha2);
            long lngLabaUsaha = Int64.Parse(strLabaUsaha);
            float ROI = (((float)lngLabaUsaha + lngPenyusutan) / CapitalEmployed) * 100;

            //Data Cash Ratio

            string strKasSetara = new string(arrayKasSetara2);
            long lngKasSetara = Int64.Parse(strKasSetara);
            Convert.ToInt64(lngKasSetara);
            string strLiabilitas = new string(arrayLiabilitas2);
            long lngLiabilitas = Int64.Parse(strLiabilitas);
            Convert.ToInt64(lngLiabilitas);
            float CashRatio = (((float)lngKasSetara+lngInvestasi) / lngLiabilitas) * 100;

            //Data Current Ratio

            string strAsetLancar = new string(arrayAsetLancar2);
            long lngAsetLancar = Int64.Parse(strAsetLancar);
            Convert.ToInt64(lngAsetLancar);
            float CurrentRatio = ((float)lngAsetLancar / lngLiabilitas) * 100;

            //Data Collections Period

            string strPendapatan = new string(arrayPendapatan2);
            long lngPendapatan = Int64.Parse(strPendapatan);
            Convert.ToInt64(lngPendapatan);
            float CP = ((float)PiutangUsaha / lngPendapatan) * 365;

            //Data Inventory Turn Over

            string strPersediaan = new string(arrayPersediaan2);
            long lngPersediaan = Int64.Parse(strPersediaan);
            Convert.ToInt64(lngPersediaan);
            float PP = ((float)lngPersediaan / lngPendapatan) * 365;

            //Total Assets Turn Over

            float TATO = ((float)lngPendapatan / CapitalEmployed) * 100;

            //TMS TA

            float TMS_TA = ((float)lngEkuitas / lngTotalAset) * 100;

            //Console.WriteLine();

            Console.Write("Capital Employed: ");
            Console.WriteLine(CapitalEmployed);
            Console.Write("Kas dan Setara Kas: ");
            Console.WriteLine(arrayKasSetara2);
            Console.Write("Penyusutan: ");
            Console.WriteLine(arrayPenyusutan2);
            Console.Write("Persediaan: ");
            Console.WriteLine(arrayPersediaan2);
            Console.Write("Aset Lancar: ");
            Console.WriteLine(arrayAsetLancar2);
            Console.Write("Total Aset: ");
            Console.WriteLine(arrayTotalAset2);
            Console.Write("Liabilitas Jangka Pendek: ");
            Console.WriteLine(arrayLiabilitas2);
            Console.Write("Total Ekuitas: ");
            Console.WriteLine(arrayEkuitas2);
            Console.Write("Total Pendapatan: ");
            Console.WriteLine(arrayPendapatan2);
            Console.Write("Laba Usaha: ");
            Console.WriteLine(arrayLabaUsaha2);
            Console.Write("Laba Bersih: ");
            Console.WriteLine(arrayLabaBersih2);
            //Console.Write("Bank: ");
            //Console.WriteLine(arrayBank2);
            //Console.Write("Kas: ");
            //Console.WriteLine(arrayKas2);
            Console.Write("Piutang Usaha Pihak Berelasi: ");
            Console.WriteLine(arrayPihakBerelasi2);
            Console.Write("Piutang Usaha Pihak Ketiga: ");
            Console.WriteLine(arrayPihakKetiga2);
            Console.Write("Total Piutang Usaha: ");
            Console.WriteLine(PiutangUsaha);
            Console.Write("Surat Berharga Jangka Pendek: ");
            Console.WriteLine(arrayInvestasi2);
            Console.WriteLine();
            Console.Write("ROE: ");
            Console.WriteLine(ROE);
            Console.Write("ROI: ");
            Console.WriteLine(ROI);
            Console.Write("Cash Ratio: ");
            Console.WriteLine(CashRatio);
            Console.Write("Current Ratio: ");
            Console.WriteLine(CurrentRatio);
            Console.Write("Collection Periods: ");
            Console.WriteLine(CP);
            Console.Write("Inventory Turnover: ");
            Console.WriteLine(PP);
            Console.Write("Total Assets Turnover: ");
            Console.WriteLine(TATO);
            Console.Write("Ratio Of Total Equity To Total Assets ");
            Console.WriteLine(TMS_TA);
            //nextPersediaan = 0;

            //Cari ASCII
            //byte[] ascii = Encoding.ASCII.GetBytes(hasil);
            //foreach (byte element in ascii)
            //{
            //    Console.WriteLine("{0}={1}", element, (char)element);

            //}
            //char[] a = new char[] { 'a' };
            //Console.Write((int)a[0]);
            // Console.WriteLine(hasil);

            //string hasil2 = getBetweenChunk(pdfToTxt, "Jumlah Aset Lancar", 2);
            //Console.WriteLine(hasil);
            //Console.WriteLine(hasil2);

            //Connection To Database

            //, penyusutan, kas, bank, surat_berharga_jangka_pendek, current_aset, current_liabilities, total_piutang_usaha, total_persediaan, total_pendapatan_usaha, total_aset, kas_setara_kas

        //    string connStr = "server=localhost;user=root;database=tugasakhir;port=3306;password=";
        //    MySqlConnection conn = new MySqlConnection(connStr);
        //    try
        //    {
        //        Console.WriteLine("Connecting to MySQL...");
        //        conn.Open();

        //        string sql = "INSERT INTO sumber (id_perusahaan, id_tahun, laba_setelah_pajak, EBIT, penyusutan, surat_berharga_jangka_pendek, current_aset, current_liabilities, total_piutang_usaha, total_persediaan, total_pendapatan_usaha, total_aset, kas_setara_kas) VALUES (@idperusahaan, @idtahun, @LabaBersih, @LabaUsaha, @Penyusutan, @Investasi, @AsetLancar, @Liabilitas, @PiutangUsaha, @Persediaan, @Pendapatan, @TotalAset, @KasSetaraKas)";

        //        MySqlCommand cmd = new MySqlCommand(sql, conn);
        //        cmd.Parameters.AddWithValue("@idperusahaan", id_perusahaan);
        //        cmd.Parameters.AddWithValue("@idtahun", id_tahun);
        //        cmd.Parameters.AddWithValue("@LabaBersih", arrayLabaBersih2);
        //        cmd.Parameters.AddWithValue("@LabaUsaha", arrayLabaUsaha2);
        //        cmd.Parameters.AddWithValue("@Penyusutan", arrayPenyusutan2);
        //        cmd.Parameters.AddWithValue("@Investasi", arrayInvestasi2);
        //        cmd.Parameters.AddWithValue("@AsetLancar", arrayAsetLancar2);
        //        cmd.Parameters.AddWithValue("@Liabilitas", arrayLiabilitas2);
        //        cmd.Parameters.AddWithValue("@PiutangUsaha", PiutangUsaha);
        //        cmd.Parameters.AddWithValue("@Persediaan", arrayPersediaan2);
        //        cmd.Parameters.AddWithValue("@Pendapatan", arrayPendapatan2);
        //        cmd.Parameters.AddWithValue("@TotalAset", arrayTotalAset2);
        //        cmd.Parameters.AddWithValue("@KasSetaraKas", arrayKasSetara2);
                
        //        cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }

        //    conn.Close();
            
        //    Console.WriteLine("Done.");

            Console.ReadLine();
        }


        public static string GetStringFromLaporan(string kode, string laporanKeuangan) {
            string result ="Not Found";
            List<string> namaList = new List<string>();
            namaList = parameterGet(kode);

            foreach (string nama in namaList)
            {

                //jika kalimat nama ini ada di laporan keuangan
                if (laporanKeuangan.Contains(nama)) {
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

        public static List<string> parameterGet(string NamaAsli)
        {
            List<string> NamaPenganti = new List<string>() ;
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
                    //Console.WriteLine(rdr["nama"]);
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

                string stm = "SELECT source_ocr.nama_2 FROM [source_ocr] JOIN [mapping] ON source_ocr.id_mapping = mapping.id WHERE mapping.nama_mapping = " + NamaAsli + "";
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

        /*public static int getIfromDB(string namaAsli) {
            int result = 0;
            string connStr = @"Data Source=DESKTOP-ERK0RV1\SQLEXPRESS;Initial Catalog=tugasakhir;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string stm = "SELECT mapping.i FROM mapping WHERE mapping.nama_mapping = \'"+namaAsli+"\'";
                SqlCommand cmd = new SqlCommand(stm, conn);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    result = rdr.GetInt32(0);
                }




            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();

            Console.WriteLine("Done.");
            return result;
        }
        */

        public static string getOneWord(string strSource, string strSearch)
        {
            if (strSource.Contains(strSearch))
            {
                return strSearch;
            }
            else
            {
                return "";


            }
        }

        public static string getBetween1(string strSource, string strStart)
        {
            int Start/*, End*/;
            if (strSource.Contains(strStart))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length+7;
               
               
                return strSource.Substring(Start, 17);
            }
            else
            {
                return "";
            }
        }

        //Chunk Pertama
        public static string getBeforeChunk(string strSource, string strStart, int HowManyChunck)
        {
            int Start;
            char[] whitespace = new char[] { ' ', '\t', '\n' };

            if (strSource.Contains(strStart))
            {
                Start = 0;
                string findwordChunk = strSource.Substring(Start, strSource.Length);
                string[] Chunk = findwordChunk.Split(whitespace);
                string[] needToFind = strStart.Split(whitespace);
                int FindChunk = 0;
                bool TrueBefore = true;
                for (int i = 0; i < (Chunk.Length - needToFind.Length); i++)
                {
                    if (Chunk[i] == needToFind[0]) {
                        TrueBefore = true;
                        for (int j = 1; j < needToFind.Length; j++)
                        {
                            if (Chunk[i + j] == needToFind[j] && TrueBefore == true)
                            {
                                TrueBefore = true;
                            }
                            else {
                                TrueBefore = false;
                            }
                        }

                        if (TrueBefore == true)
                        {
                            FindChunk = i;
                        }

                    }
                    
                }

                return Chunk[FindChunk-HowManyChunck];
            }
            else
            {
                return "";
            }
        }

        //Chunk Pertama
        public static string getBetweenChunk(string strSource, string strStart, int HowManyChunck)
        {
            int Start;
            char[] whitespace = new char[] {' ','\t','\n'};

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
        //Chunk Ke-2
        public static string getBetweenChunk2(string strSource, string strStart, int HowManyChunck)
        {
            int Start;
            int Start2;
            char[] whitespace = new char[] { ' ', '\t', '\n' };

            if (strSource.Contains(strStart))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                Start2 = strSource.IndexOf(strStart, Start) + strStart.Length;
                string findwordChunk = strSource.Substring(Start2, strSource.Length - Start2);
                string[] Chunk = findwordChunk.Split(whitespace);
                return Chunk[HowManyChunck - 1];
            }
            else
            {
                return "";
            }
        }
        //Chunk Ke-3
        public static string getBetweenChunk3(string strSource, string strStart, int HowManyChunck)
        {
            int Start;
            int Start2;
            int Start3;
            char[] whitespace = new char[] { ' ', '\t', '\n' };

            if (strSource.Contains(strStart))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                Start2 = strSource.IndexOf(strStart, Start) + strStart.Length;
                Start3 = strSource.IndexOf(strStart, Start2) + strStart.Length;
                string findwordChunk = strSource.Substring(Start3, strSource.Length - Start3);
                string[] Chunk = findwordChunk.Split(whitespace);
                return Chunk[HowManyChunck - 1];
            }
            else
            {
                return "";
            }
        }
        //Chunk Tengah
        public static string getBetweenChunkBank(string strSource, string strStart, int HowManyChunck)
        {
            int Start;
            int Start2 = 0;
            char[] whitespace = new char[] { ' ', '\t', '\n' };

            if (strSource.Contains(strStart))
            {
                for (int i = 0; i < 9; i++)
                {
                    Start = strSource.IndexOf(strStart, Start2) + strStart.Length;
                    Start2 = Start;
                }
                //Start2 = strSource.IndexOf(strStart, Start) + strStart.Length;

                string findwordChunk = strSource.Substring(Start2, strSource.Length - Start2);
                string[] Chunk = findwordChunk.Split(whitespace);
                return Chunk[HowManyChunck - 1];
            }
            else
            {
                return "";
            }
        }

        //Chunk Sub Kas
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

        public static string getBetweenChunk3nama(string strSource, string strStart, string next, int HowManyChunck)
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

                Start = strSource.IndexOf(next, Start2) + next.Length;
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

        //Chunk Sub Bank
        public static string getBetweenChunkSubBank(string strSource, string strStart, string next, int HowManyChunck)
        {
            int Start;
            int Start2 = 0;
            char[] whitespace = new char[] { ' ', '\t', '\n' };

            if (strSource.Contains(strStart))
            {
                for (int i = 0; i < 2; i++)
                {
                    Start = strSource.IndexOf(strStart, Start2) + strStart.Length;
                    Start2 = Start;
                }
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


        //Chunk Ke-11
        public static string getBetweenChunk11(string strSource, string strStart, int HowManyChunck, int IndexKe)
        {
            int Start;
            int Start2 = 0;
            char[] whitespace = new char[] { ' ', '\t', '\n' };

            if (strSource.Contains(strStart))
            {
                for (int i = 0; i < IndexKe; i++)
                {
                    Start = strSource.IndexOf(strStart, Start2) + strStart.Length;
                    Start2 = Start;
                }
                //Start2 = strSource.IndexOf(strStart, Start) + strStart.Length;

                string findwordChunk = strSource.Substring(Start2, strSource.Length - Start2);
                string[] Chunk = findwordChunk.Split(whitespace);
                return Chunk[HowManyChunck - 1];
            }
            else
            {
                return "";
            }
        }
        //Chunk Terakhir
        public static string getBetweenChunkKas(string strSource, string strStart, int HowManyChunck)
        {
            int Start;
            char[] whitespace = new char[] { ' ', '\t', '\n' };

            if (strSource.Contains(strStart))
            {
                Start = strSource.LastIndexOf(strStart);
                string findwordChunk = strSource.Substring(Start, strSource.Length - Start);
                string[] Chunk = findwordChunk.Split(whitespace);
                return Chunk[HowManyChunck - 1];
            }
            else
            {
                return "";
            }
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                Console.WriteLine("String Number: {0} /n strStart.Length= {1} ", strSource.IndexOf(strStart, 0),strStart.Length);
                End = strSource.IndexOf(strEnd, Start);
                Console.WriteLine("String End: {0}", strSource.IndexOf(strEnd, 0));

                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
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
    }
}
