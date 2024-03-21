using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class BonusCalViewModel
    {
        [Key]
        [StringLength(12)]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [StringLength(4)]
        [Display(Name = "專案狀態")]
        public string? Status { get; set; }

        [StringLength(8)]
        [Display(Name = "客戶資料庫")]
        public string? Cus_DB { get; set; }

        [StringLength(10)]
        [Display(Name = "客戶代碼")]
        public string? CusID { get; set; }

        [StringLength(10)]
        [Display(Name = "供應商")]
        public string? VendorID { get; set; }

        [StringLength(25)]
        [Display(Name = "料號")]
        public string? PartNo { get; set; }

        [Display(Name = "供應商")]
        public string? VendorName { get; set; }

        [Display(Name = "客戶")]
        public string? CusName { get; set; }
    }
}
