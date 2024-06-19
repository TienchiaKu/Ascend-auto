using MiniExcelLibs.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class BonusCalViewModel
    {
        [Key]
        [StringLength(12)]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [StringLength(4)]
        [Display(Name = "專案狀態")]
        public string? Status { get; set; }

        [StringLength(8)]
        [Display(Name = "客戶資料庫")]
        public string? Cus_DB { get; set; }

        [StringLength(10)]
        [Display(Name = "客戶代碼")]
        public string? CusID { get; set; }

        [StringLength(10)]
        [Display(Name = "供應商")]
        public string? VendorID { get; set; }

        [StringLength(25)]
        [Display(Name = "料號")]
        public string? PartNo { get; set; }

        [Display(Name = "供應商")]
        public string? VendorName { get; set; }

        [Display(Name = "客戶")]
        public string? CusName { get; set; }
    }

    public class ProjectBonusViewModel
    {
        [StringLength(12)]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [StringLength(4)]
        [Display(Name = "專案階段")]
        public string? Stage { get; set; }

        [Display(Name = "申請者")]
        public string? ApplicantName { get; set; }

        [Display(Name = "申請者ID")]
        public int ApplicantID { get; set; }

        [Display(Name = "區域")]
        public string? Region { get; set; }

        [StringLength(1)]
        [Display(Name = "交易狀態")]
        public string? TradeStatus { get; set; }

        [Display(Name = "Do獎金")]
        public double DOBonus { get; set; }

        [StringLength(5)]
        [Display(Name = "PM")]
        [Required]
        public string? PM_EmpName { get; set; }

        public int PM_EmpID { get; set; }

        public double PM_Bonus { get; set; }

        [StringLength(5)]
        [Display(Name = "Sales")]
        [Required]
        public string? Sales_EmpName { get; set; }

        public int Sales_EmpID { get; set; }

        public double Sales_Bonus { get; set; }

        [StringLength(5)]
        [Display(Name = "FAE1")]
        public string? FAE1_EmpName { get; set; }

        public int FAE1_EmpID { get; set; }

        public double FAE1_Bonus { get; set; }

        [StringLength(5)]
        [Display(Name = "FAE2")]
        public string? FAE2_EmpName { get; set; }

        public int FAE2_EmpID { get; set; }

        public double FAE2_Bonus { get; set; }

        [StringLength(5)]
        [Display(Name = "FAE3")]
        public string? FAE3_EmpName { get; set; }

        public int FAE3_EmpID { get; set; }

        public double FAE3_Bonus { get; set; }
    }

    public class EmployeeBonusViewMode
    {
        [Display(Name = "申請者")]
        public string? EmployeeName { get; set; }

        [Display(Name = "申請者ID")]
        public int EmployeeID { get; set; }

        [Display(Name = "獎金")]
        public double Bonus { get; set; }
    }

    public class DoBonusViewModel
    {
        [ExcelColumn(Name ="區域")]
        public string? Region { get; set; }

        [ExcelColumn(Name = "申請人")]
        public string? Owner { get; set; }

        public decimal Active { get; set; }

        public decimal New { get; set; }

        [ExcelColumn(Name = "小計")]
        public decimal Amount { get; set; }
    }

    public class DoReportUpload
    {
        [ExcelColumn(Name = "專案編號")]
        [Display(Name = "專案編號")]
        [Required]
        public string? ProjectID { get; set; }

        [ExcelColumn(Ignore = true)]
        [Display(Name = "狀態更新")]
        [Required]
        public string? StatusUpdate { get; set; }

        [ExcelColumn(Name = "更新訊息")]
        [Display(Name = "更新訊息")]
        public string? message { get; set; }
    }
}
