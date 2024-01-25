using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DDD_2024.Models
{
    public class DDD_Employee
    {
        [Key]
        [Display(Name = "員工編號")]
        public int EmpID { get; set; }

        [StringLength(20)]
        [Display(Name = "姓名")]
        public string EmpName { get; set; }

        [Display(Name = "在職與否")]
        public string OnDuty { get; set; }

        public string? PM { get; set; }
        public string? Sales { get; set; }
        public string? FAE { get; set; }
        public string? RBU { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateDate { get; set; }
    }
}
