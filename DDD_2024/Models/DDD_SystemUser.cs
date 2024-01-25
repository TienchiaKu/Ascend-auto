using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class DDD_SystemUser
    {
        [Key]
        [Display]
        public int UserID { get; set; }

        [Required]
        [StringLength(20)]
        public string UserName { get; set; }

        [Required]
        [StringLength(20)]
        public string UserPWD { get; set; }

        [StringLength(15)]
        public string Department { get; set; }

        [Required]
        [StringLength(1)]
        public string IsActive { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
