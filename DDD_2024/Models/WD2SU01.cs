using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class WD2SU01
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "客供商代號")]
        public string? SU01001 { get; set; }

        [StringLength(1)]
        [Display(Name = "客供商類別")]
        public string? SU01002 { get; set; }

        [StringLength(1)]
        [Display(Name = "客供商名稱")]
        public string? SU01003 { get; set; }
    }
}