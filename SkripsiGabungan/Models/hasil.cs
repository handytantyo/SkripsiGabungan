//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SkripsiGabungan.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class hasil
    {
        public long id { get; set; }
        public Nullable<long> id_perusahaan { get; set; }
        public Nullable<long> id_tahun { get; set; }
        public Nullable<double> ROE { get; set; }
        public Nullable<double> ROI { get; set; }
        public Nullable<double> cash_ratio { get; set; }
        public Nullable<double> current_ratio { get; set; }
        public Nullable<double> CP { get; set; }
        public Nullable<double> PP { get; set; }
        public Nullable<double> TATO { get; set; }
        public Nullable<double> TMS_TA { get; set; }
        public Nullable<double> target { get; set; }
        public Nullable<double> target_2 { get; set; }
        public string tingkat_kesehatan { get; set; }
        public string grade { get; set; }
    
        public virtual perusahaan perusahaan { get; set; }
        public virtual sumber sumber { get; set; }
        public virtual tahun tahun { get; set; }

        public string nama_perusahaan { get; set; }
        public Nullable<int> tahun_1 { get; set; }
    }
}
