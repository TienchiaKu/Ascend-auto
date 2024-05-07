using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class Project_Emp
    {
        [Key]
        [Display(Name = "序號")]
        public int SEQ { get; set; }

        [StringLength(12)]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [Display(Name = "員工")]
        public int EmpID { get; set; }

        [StringLength(5)]
        [Display(Name = "職務")]
        public string? Duty { get; set; }

        [Display(Name = "獎金比例")]
        public double BonusP { get; set; }
    }
}