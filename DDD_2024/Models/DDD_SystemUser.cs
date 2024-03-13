using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class DDD_SystemUser
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "使用者")]
        public string UserName { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "密碼")]
        public string UserPWD { get; set; }

        [StringLength(15)]
        [Display(Name = "部門")]
        public string Department { get; set; }

        [Required]
        [StringLength(1)]
        [Display(Name = "是否在職")]
        public string IsActive { get; set; }

        [Required]
        [Display(Name = "建立日期")]
        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
