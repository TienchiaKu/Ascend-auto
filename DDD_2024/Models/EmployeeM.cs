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
        public string? EmpName { get; set; }

        [Display(Name = "在職與否")]
        public string? OnDuty { get; set; }

        [StringLength(10)]
        [Display(Name = "密碼")]
        public string? userPWD { get; set; }

        [Display(Name = "更新時間")]
        public DateTime UpdateDate { get; set; }

        [Display(Name = "區域")]
        public string? Region { get; set; }
    }

    public class EmpIndexViewModel
    {
        [Display(Name = "員工編號")]
        public int EmpID { get; set; }

        [Display(Name = "姓名")]
        public string? EmpName { get; set; }

        [Display(Name = "區域")]
        public string? Region { get; set; }

        [Display(Name = "Sales")]
        public bool isSales { get; set; }

        [Display(Name = "PM")]
        public bool isPM { get; set; }

        [Display(Name = "FAE")]
        public bool isFAE { get; set; }

        [Display(Name = "RBU")]
        public bool isRBU { get; set; }

        [Display(Name = "權限")]
        public string? Auth { get; set; }
    }

    public class EmpCreateViewModel
    {
        [Display(Name = "姓名")]
        public string? EmpName { get; set; }

        [Display(Name = "區域")]
        public string? Region { get; set; }

        [Display(Name = "密碼")]
        public string? userPWD { get; set; }

        [Display(Name = "在職?")]
        public string? OnDuty { get; set; }

        [Display(Name = "Sales")]
        public string? isSales { get; set; }

        [Display(Name = "PM")]
        public string? isPM { get; set; }

        [Display(Name = "FAE")]
        public string? isFAE { get; set; }

        [Display(Name = "RBU")]
        public string? isRBU { get; set; }

        [Display(Name = "權限")]
        public string? Auth { get; set; }
    }

    public class EmpEditViewModel
    {
        public int EmpId { get; set; }

        [Display(Name = "姓名")]
        public string? EmpName { get; set; }

        [Display(Name = "區域")]
        public string? Region { get; set; }

        [Display(Name = "密碼")]
        public string? userPWD { get; set; }

        [Display(Name = "在職?")]
        public string? OnDuty { get; set; }

        [Display(Name = "Sales")]
        public string? isSales { get; set; }

        [Display(Name = "PM")]
        public string? isPM { get; set; }

        [Display(Name = "FAE")]
        public string? isFAE { get; set; }

        [Display(Name = "RBU")]
        public string? isRBU { get; set; }

        [Display(Name = "權限")]
        public string? Auth { get; set; }
    }
}
