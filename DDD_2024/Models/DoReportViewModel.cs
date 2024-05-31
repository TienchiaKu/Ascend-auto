using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class DoReportViewModel
    {
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [Display(Name = "Date")]
        public string? ApplicationDate { get; set; }

        [Display(Name = "客戶名稱")]
        public string? CusName { get; set; }

        [StringLength(1)]
        [Display(Name = "New/Active")]
        public string? TradeStatus { get; set; }

        [Display(Name = "Product")]
        public string? VendorName { get; set; }

        [StringLength(25)]
        [Display(Name = "PartNumber")]
        public string? PartNo { get; set; }

        [StringLength(20)]
        [Display(Name = "Application")]
        public string? ProApp { get; set; }

        [StringLength(500)]
        [Display(Name = "Status(Latest)")]
        [Required]
        public string? DoUStatus { get; set; }

        [Display(Name = "Owner")]
        public string? Applicant { get; set; }

        [StringLength(20)]
        [Display(Name = "Approved By")]
        public string? Approver{ get; set; }

        [StringLength(500)]
        [Display(Name = "Action(Latest)")]
        public string? DoUAction { get; set; }
    }

    public class DoReportFilterViewModel
    {
        [Display(Name = "Do狀態")]
        public string? DoStatus { get; set; }

        [Display(Name = "申請者")]
        public int applicantID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyyMMdd}", ApplyFormatInEditMode = true)]
        [Display(Name = "申請時間(起)")]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyyMMdd}", ApplyFormatInEditMode = true)]
        [Display(Name = "申請時間(迄)")]
        public DateTime? EndDate { get; set; }
    }
}
