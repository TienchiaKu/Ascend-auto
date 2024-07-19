using MiniExcelLibs.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    //獎金計算模組
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

    #region BonusCal-DoReport
    public class BDoReportViewModel
    {
        [ExcelColumn(Name = "No.")]
        public int SEQ { get; set; }

        [ExcelColumn(Name = "Date")]
        [Display(Name = "Date")]
        public string? ApplicationDate { get; set; }

        [ExcelColumn(Name = "專案編號")]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [ExcelColumn(Name = "Customer(中文)")]
        [Display(Name = "客戶名稱")]
        public string? CusName { get; set; }

        [ExcelColumn(Name = "New/Active")]
        [Display(Name = "New/Active")]
        public string? NewActive { get; set; }

        [ExcelColumn(Name = "Product")]
        [Display(Name = "Product")]
        public string? VendorName { get; set; }

        [ExcelColumn(Name = "Part Number")]
        [Display(Name = "PartNumber")]
        public string? PartNo { get; set; }

        [ExcelColumn(Name = "Application", Width = 20)]
        [Display(Name = "Application")]
        public string? ProApp { get; set; }

        [ExcelColumn(Name = "進度更新月份")]
        public string? DoStatusDate { get; set; }

        [ExcelColumn(Name = "Status Update(最新)", Width = 40)]
        [Display(Name = "Status(Latest)")]
        public string? DoStatus1 { get; set; }

        [ExcelColumn(Ignore = true)]
        public int ApplicantID { get; set; }

        [ExcelColumn(Name = "Owner")]
        [Display(Name = "Owner")]
        public string? Applicant { get; set; }

        [ExcelColumn(Name = "Approved By")]
        [StringLength(20)]
        [Display(Name = "Approved By")]
        public string? Approver { get; set; }

        [ExcelColumn(Name = "Action", Width = 20)]
        [Display(Name = "Action(Latest)")]
        public string? DoUAction { get; set; }

        [ExcelColumn(Name = "發放獎金?")]
        public string? Noted1 { get; set; }
    }
    public class BDoReportFilter
    {
        [Display(Name = "區域")]
        [Required]
        public string? Region { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyyMMdd}", ApplyFormatInEditMode = true)]
        [Display(Name = "申請時間(起)")]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyyMMdd}", ApplyFormatInEditMode = true)]
        [Display(Name = "申請時間(迄)")]
        public DateTime? EndDate { get; set; }
    }
    #endregion

    #region DoController-DoReport
    public class DoReport_ViewModel
    {
        [ExcelColumn(Name = "No.")]
        public int SEQ { get; set; }

        [ExcelColumn(Name = "Date")]
        [Display(Name = "Date")]
        public string? ApplicationDate { get; set; }

        [ExcelColumn(Name = "專案編號")]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [ExcelColumn(Name = "Customer(中文)")]
        [Display(Name = "客戶名稱")]
        public string? CusName { get; set; }

        [ExcelColumn(Name = "New/Active")]
        [Display(Name = "New/Active")]
        public string? NewActive { get; set; }

        [ExcelColumn(Name = "Product")]
        [Display(Name = "Product")]
        public string? VendorName { get; set; }

        [ExcelColumn(Name = "Part Number")]
        [Display(Name = "PartNumber")]
        public string? PartNo { get; set; }

        [ExcelColumn(Name = "Application", Width = 20)]
        [Display(Name = "Application")]
        public string? ProApp { get; set; }

        [ExcelColumn(Name = "結案")]
        public string? Noted1 { get; set; }

        [ExcelColumn(Name = "結案原因")]
        public string? Noted2 { get; set; }

        [ExcelColumn(Name = "Status Update(最新)", Width = 40)]
        [Display(Name = "Status(Latest)")]
        public string? DoStatus1 { get; set; }

        [ExcelColumn(Name = "Status Update", Width = 40)]
        [Display(Name = "Status(Latest)")]
        public string? DoStatus2 { get; set; }

        [ExcelColumn(Name = "Status Update", Width = 40)]
        [Display(Name = "Status(Latest)")]
        public string? DoStatus3 { get; set; }

        [ExcelColumn(Name = "Status Update", Width = 40)]
        [Display(Name = "Status(Latest)")]
        public string? DoStatus4 { get; set; }

        [ExcelColumn(Name = "Status Update", Width = 40)]
        [Display(Name = "Status(Latest)")]
        public string? DoStatus5 { get; set; }

        [ExcelColumn(Name = "Status Update", Width = 40)]
        [Display(Name = "Status(Latest)")]
        public string? DoStatus6 { get; set; }

        [ExcelColumn(Ignore = true)]
        public int ApplicantID { get; set; }

        [ExcelColumn(Name = "Owner")]
        [Display(Name = "Owner")]
        public string? Applicant { get; set; }

        [ExcelColumn(Name = "Approved By")]
        [StringLength(20)]
        [Display(Name = "Approved By")]
        public string? Approver { get; set; }

        [ExcelColumn(Name = "Action", Width = 20)]
        [Display(Name = "Action(Latest)")]
        public string? DoUAction { get; set; }
    }
    public class DoReportFilterViewModel
    {
        [Display(Name = "取結案資料?")]
        public string? IsFinsih { get; set; }

        [Display(Name = "申請者")]
        public int applicantID { get; set; }

        [Display(Name = "區域")]
        public string? Region { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyyMMdd}", ApplyFormatInEditMode = true)]
        [Display(Name = "申請時間(起)")]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyyMMdd}", ApplyFormatInEditMode = true)]
        [Display(Name = "申請時間(迄)")]
        public DateTime? EndDate { get; set; }
    }
    public class DoReportStatus_ViewModel
    {
        [ExcelColumn(Name = "狀態")]
        public string? Status { get; set; }

        [ExcelColumn(Name = "代表文字")]
        public string? Text { get; set; }
    }
    #endregion

    #region DoController-DoMaintain
    //更新結案情況
    public class DoMaintainModel_F
    {
        [StringLength(12)]
        [Required]
        public string? ProjectID { get; set; }

        [StringLength(13)]
        public string? DoID { get; set; }

        public string? DoUDate { get; set; }

        [StringLength(1)]
        [Required]
        [Display(Name = "結案")]
        public string? IsFinish { get; set; }

        [StringLength(500)]
        [Display(Name = "結案原因")]
        public string? FinishReason { get; set; }
    }
    //更新Status,Action
    public class DoMaintainModel_U
    {
        [StringLength(12)]
        [Required]
        public string? ProjectID { get; set; }

        [StringLength(13)]
        public string? DoID { get; set; }

        [Display(Name = "更新時間")]
        public string? DoUDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Action")]
        public string? DoUAction { get; set; }

        [StringLength(500)]
        [Display(Name = "Status")]
        public string? DoUStatus { get; set; }

    }
    //儲存Do維護結果
    public class DoMaintainModel_Result
    {
        [StringLength(12)]
        [Required]
        public string? ProjectID { get; set; }

        [Display(Name = "上傳狀態")]
        public string? UploadStatus { get; set; }
    }
    #endregion

    public class DiDwReportViewModel
    {
        [ExcelColumn(Name = "立項日期")]
        [Display(Name = "Date")]
        public string? ApplicationDate { get; set; }

        [ExcelColumn(Name = "專案編號")]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [ExcelColumn(Name = "供應商")]
        [Display(Name = "供應商")]
        public string? VendorName { get; set; }

        [ExcelColumn(Name = "品名料號")]
        [Display(Name = "品名料號")]
        public string? PartName { get; set; }

        [ExcelColumn(Name = "客戶名稱")]
        [Display(Name = "客戶名稱")]
        public string? CusName { get; set; }

        [ExcelColumn(Name = "最終客戶")]
        [Display(Name = "最終客戶")]
        public string? EndCusName { get; set; }

        [ExcelColumn(Name = "產品應用")]
        [Display(Name = "產品應用")]
        public string? ProApp { get; set; }

        [ExcelColumn(Name = "產品型號")]
        [Display(Name = "產品型號")]
        public string? ProModel { get; set; }

        [ExcelColumn(Name = "預估量產年度")]
        [Display(Name = "預估量產年度")]
        public string? PYear { get; set; }

        [ExcelColumn(Name = "預估量產季度")]
        [Display(Name = "預估量產季度")]
        public string? PSeason { get; set; }

        [ExcelColumn(Name = "預估第一年\r\n( 數量 )")]
        [Display(Name = "預估第一年(數量)")]
        public double FirstQty { get; set; }

        [ExcelColumn(Name = "預估第二年\r\n( 數量 )")]
        [Display(Name = "預估第二年(數量)")]
        public double SecondQty { get; set; }

        [ExcelColumn(Name = "預估第三年\r\n( 數量 )")]
        [Display(Name = "預估第三年(數量)")]
        public double ThirdQty { get; set; }

        [ExcelColumn(Name = "單價_第一年")]
        [Display(Name = "單價_第一年")]
        public double FirstPrice { get; set; }

        [ExcelColumn(Name = "單價_第二年")]
        [Display(Name = "單價_第二年")]
        public double SecondPrice { get; set; }

        [ExcelColumn(Name = "單價_第三年")]
        [Display(Name = "單價_第三年")]
        public double ThirdPrice { get; set; }

        [ExcelColumn(Name = "預估三年銷售額\r\n( LTR )")]
        [Display(Name = "預估三年銷售額")]
        public double LTR { get; set; }

        [ExcelColumn(Name = "毛利率\r\n( 預估 )")]
        [Display(Name = "毛利率")]
        public double GP { get; set; }

        [ExcelColumn(Name = "預估三年銷售毛利\r\n( LTP )")]
        [Display(Name = "預估三年銷售毛利")]
        public double LTP { get; set; }

        [ExcelColumn(Name = "DIDWType")]
        [Display(Name = "DIDWType")]
        public string? DiDw { get; set; }

        [ExcelColumn(Name = "Integration")]
        [Display(Name = "Integration")]
        public string? inte { get; set; }

        [ExcelColumn(Name = "PM")]
        [Display(Name = "PM")]
        public string? PMName { get; set; }

        [ExcelColumn(Name = "PM%")]
        [Display(Name = "PM%")]
        public double PMPer { get; set; }

        [ExcelColumn(Name = "Sales")]
        [Display(Name = "Sales")]
        public string? SalesName { get; set; }

        [ExcelColumn(Name = "Sales%")]
        [Display(Name = "Sales%")]
        public double SalesPer { get; set; }

        [ExcelColumn(Name = "FAE_Total")]
        [Display(Name = "FAE_Total")]
        public double FAE_Total { get; set; }

        [ExcelColumn(Name = "FAE1")]
        [Display(Name = "FAE1")]
        public string? FAE1Name { get; set; }

        [ExcelColumn(Name = "FAE1%")]
        [Display(Name = "FAE1%")]
        public double FAE1Per { get; set; }

        [ExcelColumn(Name = "FAE2")]
        [Display(Name = "FAE2")]
        public string? FAE2Name { get; set; }

        [ExcelColumn(Name = "FAE2%")]
        [Display(Name = "FAE2%")]
        public double FAE2Per { get; set; }

        [ExcelColumn(Name = "FAE3")]
        [Display(Name = "FAE3")]
        public string? FAE3Name { get; set; }

        [ExcelColumn(Name = "FAE3%")]
        [Display(Name = "FAE3%")]
        public double FAE3Per { get; set; }

        [ExcelColumn(Name = "獎金Base")]
        [Display(Name = "獎金Base")]
        public double Bbase { get; set; }

        [ExcelColumn(Name = "獎金總額")]
        [Display(Name = "獎金總額")]
        public double BTotal { get; set; }

        [ExcelColumn(Name = "PM$")]
        [Display(Name = "PM$")]
        public double PMMoney { get; set; }

        [ExcelColumn(Name = "SALES$")]
        [Display(Name = "Sales$")]
        public double SalesMoney { get; set; }

        [ExcelColumn(Name = "FAE1$")]
        [Display(Name = "FAE1%")]
        public double FAE1Money { get; set; }

        [ExcelColumn(Name = "FAE2$")]
        [Display(Name = "FAE2%")]
        public double FAE2Money { get; set; }

        [ExcelColumn(Name = "FAE3$")]
        [Display(Name = "FAE3%")]
        public double FAE3Money { get; set; }
    }
}
