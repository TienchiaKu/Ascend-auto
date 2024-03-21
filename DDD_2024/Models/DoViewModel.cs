using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class DoViewModel
    {
        //ProjectM
        [StringLength(8)]
        [Display(Name = "客戶資料庫")]
        public string? Cus_DB { get; set; }

        [StringLength(10)]
        [Display(Name = "客戶代碼")]
        public string? CusID { get; set; }

        [StringLength(20)]
        [Display(Name = "產品應用")]
        public string? ProApp { get; set; }

        //ProjectD
        [StringLength(10)]
        [Display(Name = "供應商")]
        public string? VendorID { get; set; }

        [StringLength(25)]
        [Display(Name = "料號")]
        public string? PartNo { get; set; }

        //Project_DO
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "建立時間")]
        public DateTime CreateDate { get; set; }

        //公用
        [Display(Name = "申請者")]
        public string? ApplicantName { get; set; }

        //讀取資料用
        public string? DoID { get; set; }

        [Display(Name = "建立時間")]
        public string? vmCreateDate { get; set; }

        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [Display(Name = "供應商")]
        public string? VendorName { get; set; }

        [Display(Name = "客戶")]
        public string? CusName { get; set; }

        [Display(Name = "Do狀態")]
        public string? StatusName { get; set; }

        [Display(Name = "Do狀態")]
        public string? DOStatus { get; set; }

        [Display(Name = "Excel路徑")]
        public string? ExcelPath { get; set; }
    }
}
