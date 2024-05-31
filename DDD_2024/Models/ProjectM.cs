using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class ProjectM
    {
        [Key]
        [StringLength(12)]
        [Display(Name = "專案編號")]
        [Required]
        public string? ProjectID { get; set; }

        [StringLength(4)]
        [Display(Name = "專案狀態")]
        public string? Status { get; set; }

        [StringLength(10)]
        [Display(Name = "建立時間")]
        public string? CreateDate { get; set; }

        [StringLength(10)]
        [Display(Name = "更新日期")]
        public string? UpdateDate { get; set; }

        [StringLength(8)]
        [Display(Name = "客戶資料庫")]
        public string? Cus_DB { get; set; }

        [StringLength(10)]
        [Display(Name = "客戶代碼")]
        public string? CusID { get; set; }

        [StringLength(20)]
        [Display(Name = "最終客戶名稱")]
        public string? EndCus { get; set; }

        [StringLength(40)]
        [Display(Name = "產品應用")]
        public string? ProApp { get; set; }

        [StringLength(20)]
        [Display(Name = "產品型號")]
        public string? ProModel { get; set; }

        [Display(Name = "是否整合")]
        public bool IsInte { get; set; }

        [StringLength(6)]
        [Display(Name = "預估量產時間")]
        public string? EProduceYS { get; set; }      
    }
}
