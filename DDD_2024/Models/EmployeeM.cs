using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DDD_2024.Models
{
    public class EmployeeM
    {
        [Key]
        [Display(Name = "員工編號")]
        public int EmpID { get; set; }

        [StringLength(20)]
        [Display(Name = "姓名")]
        public string EmpName { get; set; }

        [Display(Name = "在職與否")]
        public string OnDuty { get; set; }

        [StringLength(10)]
        [Display(Name = "密碼")]
        public string? userPWD { get; set; }

        [Display(Name = "建立時間")]
        public DateTime UpdateDate { get; set; }
    }
}
