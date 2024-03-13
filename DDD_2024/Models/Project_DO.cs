using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "提出者")]
        [Required]
        public int ApplicantID { get; set; }

        [StringLength(1)]
        [Display(Name = "Do狀態")]
        [Required]
        public string? Status { get; set; }
    }
}
