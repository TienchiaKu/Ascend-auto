using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DDD_2024.Models
{
    public class Project_DO
    {
        [Key]
        [StringLength(13)]
        [Required]
        public string? DoID { get; set; }

        [StringLength(12)]
        [Display(Name = "專案編號")]
        [Required]
        public string? ProjectID { get; set; }

        [StringLength(10)]
        [Display(Name = "建立時間")]
        [Required]
        public string? CreateDate { get; set; }

        [Display(Name = "申請者")]
        [Required]
        public int ApplicantID { get; set; }

        [StringLength(1)]
        [Display(Name = "Do狀態")]
        [Required]
        public string? Status { get; set; }

        [StringLength(1)]
        [Display(Name = "交易狀態")]
        public string? TradeStatus { get; set; }

        [Display(Name = "覆核者")]
        public int ApproverID { get; set; }

        [StringLength(1)]
        [Display(Name = "是否結案")]
        public string? IsFinish { get; set; }
    }

    public class Project_DOASUpdate
    {
        [Key]
        [Required]
        public int SEQ { get; set; }
        
        [StringLength(13)]
        [Required]
        public string? DoID { get; set; }

        [StringLength(6)]
        [Display(Name = "更新時間")]
        [Required]
        public string? DoUDate { get; set; }

        [StringLength(500)]
        [Display(Name = "動作")]
        public string? DoUAction { get; set; }

        [StringLength(500)]
        [Display(Name = "進度")]
        public string? DoUStatus { get; set; }
    }

    public class DOASUViewModel
    {
        [StringLength(13)]
        [Required]
        public string? DoID { get; set; }

        [Display(Name = "更新時間")]
        [Required]
        public DateTime vmDoUDate { get; set; }

        [Display(Name = "更新時間")]
        public string? DoUDate { get; set; }

        [StringLength(500)]
        [Display(Name = "動作")]
        public string? DoUAction { get; set; }

        [StringLength(500)]
        [Display(Name = "進度")]
        [Required]
        public string? DoUStatus { get; set; }
    }

    public class DoEditViewModel
    {
        [Required]
        public string? DoID { get; set; }

        [Required]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [Display(Name = "申請日期")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "客戶")]
        public string? CusName { get; set; }

        [Display(Name = "供應商")]
        public string? VendorName { get; set; }

        [Display(Name = "產品代號")]
        public string? PartNo { get; set; }

        [Display(Name = "產品應用")]
        public string? ProApp { get; set; }

        [Display(Name = "申請者")]
        [Required]
        public int ApplicantID { get; set; }

        [Display(Name = "覆核者")]
        [Required]
        public int ApproverID { get; set; }

        [Display(Name = "交易狀態")]
        public string? TradeStatus { get; set; }

        [Display(Name = "更新時間")]
        public string? DoUDate { get; set; }

        [StringLength(500, ErrorMessage = "字串長度過長")]
        [Display(Name = "DoAction")]
        public string? DoUAction { get; set; }

        [StringLength(500,ErrorMessage ="字串長度過長")]
        [Display(Name = "DoStatus")]
        [Required]
        public string? DoUStatus { get; set; }
    }
}