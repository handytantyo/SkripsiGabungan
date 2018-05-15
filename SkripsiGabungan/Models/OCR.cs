using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SkripsiGabungan.Models
{
    public class OCR
    {
        public double ROE { get; set; }
        public double ROI { get; set; }
        public double cash_ratio { get; set; }
        public double current_ratio { get; set; }
        public double CP { get; set; }
        public double PP { get; set; }
        public double TATO { get; set; }
        public double TMS_TA { get; set; }
        public double target { get; set; }
        [DisplayName("Tingkat Kesehatan")]
        public string tingkat_kesehatan { get; set; }
        [DisplayName("Grade")]
        public string grade { get; set; }
    }
}