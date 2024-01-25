using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Interfaces
{
    public interface IEmployeeService
    {
        int MaxEmployeeID { get; }
        List<SelectListItem> YesNo { get; set; }
        List<SelectListItem> GetEmployeeNameList { get; }
    }
}