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

        [StringLength(40)]
        [Display(Name = "產品應用")]
        public string? ProApp { get; set; }

        //ProjectD
        [StringLength(10)]
        [Display(Name = "供應商")]
        public string? VendorID { get; set; }

        [StringLength(25)]
        [Display(Name = "產品代號")]
        public string? PartNo { get; set; }

        //Project_DO
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "建立時間")]
        [Required]
        public DateTime CreateDate { get; set; }

        [StringLength(1)]
        [Display(Name = "交易狀態")]
        public string? TradeStatus { get; set; }

        //Project_DOASUpdate
        [Display(Name = "更新時間")]
        public string? DoUDate { get; set; }

        [StringLength(500)]
        [Display(Name = "DoAction")]
        public string? DoUAction { get; set; }

        [StringLength(500)]
        [Display(Name = "DoStatus")]
        public string? DoUStatus { get; set; }

        //公用
        [Display(Name = "申請者")]
        public string? Applicant { get; set; }

        [Display(Name = "申請者ID")]
        [Required]
        public int ApplicantID { get; set; }

        [Display(Name = "覆核者")]
        public string? Approver { get; set; }

        [Display(Name = "覆核者ID")]
        public int ApproverID { get; set; }

        //讀取資料用
        [Required]
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

        [Display(Name = "上傳狀態")]
        public string? UploadStatus { get; set; }
    }

    public class DoCreateViewModel
    {
        [Required]
        [Display(Name = "申請時間")]
        public DateTime CreateDate { get; set; }

        [Required]
        [Display(Name = "客戶")]
        public string? CusID { get; set; }

        [Required]
        [Display(Name = "供應商")]
        public string? VendorID { get; set; }

        [Required]
        [Display(Name = "產品代號")]
        public string? PartNo { get; set; }

        [Display(Name = "產品應用")]
        public string? ProApp { get; set; }

        [Required]
        [Display(Name = "是否為新客戶")]
        public string? TradeStatus { get; set; }

        [Required]
        [Display(Name = "申請者")]
        public int ApplicantID { get; set; }

        [Required]
        [Display(Name = "覆核者ID")]
        public int ApproverID { get; set; }

        [StringLength(500)]
        [Display(Name = "Action")]
        public string? DoUAction { get; set; }

        [StringLength(500)]
        [Display(Name = "Status")]
        public string? DoUStatus { get; set; }
    }
}
