using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class DutyM
    {
        [Key]
        public int DutyID { get; set; }

        [Display(Name = "員工編號")]
        [Required]
        public int EmpID { get; set; }

        [StringLength(5)]
        [Display(Name = "角色")]
        [Required]
        public string? Role { get; set; }

        [StringLength(1)]
        [Display(Name = "是否使用")]
        [Required]
        public string? IsUse { get; set; }

        [Display(Name = "更新時間")]
        public DateTime UpdateDate { get; set; }
    }

    public class DutyIndexViewModel
    {
        [Display(Name = "員工編號")]
        public int EmpID { get; set; }

        [Display(Name = "姓名")]
        public string? EmpName {  get; set; }

        [Display(Name = "Sales")]
        public bool isSales { get; set; }

        [Display(Name = "PM")]
        public bool isPM { get; set; }

        [Display(Name = "FAE")]
        public bool isFAE { get; set; }

        [Display(Name = "RBU")]
        public bool isRBU { get; set; }

        [Display(Name = "權限")]
        public string? Auth { get; set; }
    }
}
