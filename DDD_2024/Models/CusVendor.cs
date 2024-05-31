﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class CusVendor
    {
        [Key]
        [StringLength(5)]
        [Display(Name = "客供商ID")]
        [Required]
        public string? CusVenID { get; set; }

        [StringLength(25)]
        [Display(Name = "客供商名稱")]
        [Required]
        public string? CusVenName { get; set; }

        [StringLength(8)]
        [Display(Name = "資料庫來源")]
        public string? DBSource { get; set; }

        [StringLength(10)]
        [Display(Name = "文中客供商代碼")]
        public string? CusVenCode { get; set; }

        [StringLength(1)]
        [Display(Name = "是否使用")]
        public string? IsUse { get; set; }

        [Display(Name = "更新時間")]
        public DateTime UpdateDate { get; set; }    
    }

    public class CusReportViewModel
    {
        [StringLength(8)]
        [Display(Name = "資料庫來源")]
        public string? DBSource { get; set; }

        [StringLength(10)]
        [Display(Name = "客戶代碼")]
        public string? CusID { get; set; }

        [StringLength(25)]
        [Display(Name = "客戶名稱")]
        [Required]
        public string? CusName { get; set; }
    }

    public class VendorReportViewModel
    {
        [StringLength(8)]
        [Display(Name = "資料庫來源")]
        public string? DBSource { get; set; }

        [StringLength(10)]
        [Display(Name = "供應商代碼")]
        public string? VendorID { get; set; }

        [StringLength(25)]
        [Display(Name = "供應商名稱")]
        [Required]
        public string? VendorName { get; set; }
    }
}
