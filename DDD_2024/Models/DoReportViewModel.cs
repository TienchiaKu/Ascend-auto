using MiniExcelLibs.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class DoReportViewModel
    {
        [ExcelColumn(Name = "專案編號")]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [ExcelColumn(Name = "Date")]
        [Display(Name = "Date")]
        public string? ApplicationDate { get; set; }

        [ExcelColumn(Name = "客戶名稱")]
        [Display(Name = "客戶名稱")]
        public string? CusName { get; set; }

        [ExcelColumn(Name = "New/Active")]
        [StringLength(1)]
        [Display(Name = "New/Active")]
        public string? TradeStatus { get; set; }

        [ExcelColumn(Name = "Product")]
        [Display(Name = "Product")]
        public string? VendorName { get; set; }

        [ExcelColumn(Name = "PartNumber")]
        [StringLength(25)]
        [Display(Name = "PartNumber")]
        public string? PartNo { get; set; }

        [ExcelColumn(Name = "Application",Width = 20)]
        [StringLength(20)]
        [Display(Name = "Application")]
        public string? ProApp { get; set; }

        [ExcelColumn(Name = "Status(Latest)", Width = 60)]
        [StringLength(500)]
        [Display(Name = "Status(Latest)")]
        [Required]
        public string? DoUStatus { get; set; }

        [ExcelColumn(Ignore = true)]
        public int ApplicantID { get; set; }

        [ExcelColumn(Name = "Owner")]
        [Display(Name = "Owner")]
        public string? Applicant { get; set; }

        [ExcelColumn(Name = "Approved By")]
        [StringLength(20)]
        [Display(Name = "Approved By")]
        public string? Approver{ get; set; }

        [ExcelColumn(Name = "Action(Latest)", Width = 20)]
        [StringLength(500)]
        [Display(Name = "Action(Latest)")]
        public string? DoUAction { get; set; }

        [ExcelColumn(Name = "獎金/狀態更新")]
        public string? StatusUpdate { get; set; }
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
