using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class ProjectD
    {
        [Key]
        [StringLength(12)]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [StringLength(10)]
        [Display(Name = "供應商")]
        public string? VendorID { get; set; }

        [StringLength(25)]
        [Display(Name = "料號")]
        public string? PartNo { get; set; }

        [Display(Name = "預估三年銷售額")]
        public double ELTR { get; set; }

        [Display(Name = "預估毛利率")]
        public double EGP { get; set; }

        [Display(Name = "預估第一年數量")]
        public int EFirstYQty { get; set; }

        [Display(Name = "預估第二年數量")]
        public int ESecondYQty { get; set; }

        [Display(Name = "預估第三年數量")]
        public int EThirdYQty { get; set; }

        [Display(Name = "第一年單價")]
        public double UFirstYPrice { get; set; }

        [Display(Name = "第二年單價")]
        public double USecondYPrice { get; set; }

        [Display(Name = "第三年單價")]
        public double UThirdYPrice { get; set; }
    }
}
