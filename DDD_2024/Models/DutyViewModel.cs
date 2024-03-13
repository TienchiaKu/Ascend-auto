using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class DutyViewModel
    {
        public DutyM duty { get; set; }

        public List<WD2SU01> list_WD2SU01s { get; set; }

        public DutyViewModel()
        {
            duty = new DutyM();
            list_WD2SU01s = new List<WD2SU01>();
        }      

        [StringLength(15)]
        [Display(Name = "產品線")]
        public string? VendorName { get; set; }

        [Display(Name = "員工名")]
        public string EmpName { get; set; }

        [StringLength(1)]
        [Display(Name = "是否使用")]
        public string? IsUseName { get; set; }
    }
}
