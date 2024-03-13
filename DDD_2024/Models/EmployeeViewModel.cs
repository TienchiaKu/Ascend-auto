using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class EmployeeViewModel
    {
        public EmployeeM employee { get; set; }

        public EmployeeViewModel()
        {
            employee = new EmployeeM();
        }

        [Display(Name = "是否在職")]
        public string? OnDuty_CN { get; set; }
    }
}
