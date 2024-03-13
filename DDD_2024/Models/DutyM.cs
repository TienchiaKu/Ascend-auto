using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class DutyM
    {
        [Key]
        public int DutyID { get; set; }

        [Display(Name = "資料來源")]
        [Required]
        public string? DBSource { get; set; }

        [StringLength(5)]
        [Display(Name = "產品線")]
        [Required]
        public string? VendorID { get; set; }

        [Display(Name = "員工編號")]
        public int EmpID { get; set; }

        [StringLength(5)]
        [Display(Name = "職務")]
        [Required]
        public string? Duty { get; set; }

        [StringLength(1)]
        [Display(Name = "是否使用")]
        [Required]
        public string? IsUse { get; set; }

        [Display(Name = "更新時間")]
        public DateTime UpdateDate { get; set; }
    }
}
