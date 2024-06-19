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

    public class DOASU_Upload_ViewModel
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

        [StringLength(500)]
        [Display(Name = "進度")]
        [Required]
        public string? DoUStatusCurrent { get; set; }

        [StringLength(500)]
        [Display(Name = "進度5月")]
        [Required]
        public string? DoUStatus5 { get; set; }

        [StringLength(500)]
        [Display(Name = "進度4月")]
        [Required]
        public string? DoUStatus4 { get; set; }

        [StringLength(12)]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [Display(Name = "上傳狀態")]
        public string? UploadStatus { get; set; }
    }
}