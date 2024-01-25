using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class DDD_DoM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DoID { get; set; }

        [StringLength(15)]
        [Display(Name = "產品線")]
        public string VendorName { get; set; }

        [StringLength(25)]
        [Display(Name = "料號")]
        public string PartNo { get; set; }

        [StringLength(20)]
        [Display(Name = "客戶名稱(中文)")]
        public string? CusName_CN { get; set; }

        [StringLength(20)]
        [Display(Name = "客戶名稱(英文)")]
        public string? CusName_EN { get; set; }

        [StringLength(20)]
        [Display(Name = "應用")]
        public string? ProApplication { get; set; }

        [StringLength(40)]
        [Display(Name = "Status")]
        public string? ProjectNoted { get; set; }

        [StringLength(40)]
        [Display(Name = "Action")]
        public string? ProjectAction { get; set; }

        [StringLength(10)]
        [Display(Name = "申請日期")]
        public string CreateDate { get; set; }

        [Display(Name = "Owner")]
        public int Applicant { get; set; }

        [Display(Name = "Approved by")]
        public int Approver { get; set; }

        [Display(Name = "審核日期")]
        public DateTime ApproveDate { get; set; }
    }
}
