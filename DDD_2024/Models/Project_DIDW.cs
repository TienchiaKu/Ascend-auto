using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class Project_DIDW
    {
        [StringLength(12)]
        [Display(Name = "專案編號")]
        [Required]
        public string? ProjectID { get; set; }

        [StringLength(13)]
        [Display(Name = "DoID")]
        public string? DoID { get; set; }

        [StringLength(10)]
        [Display(Name = "Din建立時間")]
        [Required]
        public string? DinDate { get; set; }

        [StringLength(1)]
        [Display(Name = "DIn狀態")]
        public string? DinStatus { get; set; }

        [StringLength(10)]
        [Display(Name = "Dwin建立時間")]
        [Required]
        public string? DwinDate { get; set; }

        [StringLength(1)]
        [Display(Name = "Dwin狀態")]
        public string? DwinStatus { get; set; }
    }
}
