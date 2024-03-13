using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class CusVendorViewModel
    {
        public WD2SU01 CusVendor { get; set; }
        public CusVendorViewModel()
        {
            CusVendor = new WD2SU01();
        }

        public string CusVendors { get; set; }

        public List<SelectListItem> listCusVendor { get; } = new List<SelectListItem>
        {
            new SelectListItem {Text = "ASCEND" , Value = "ASCEND"},
            new SelectListItem {Text = "ATI" , Value = "ATI"},
            new SelectListItem {Text = "INTERTEK" , Value = "INTERTEK"},
            new SelectListItem {Text = "KIR1N" , Value = "KIR1N"},
            new SelectListItem {Text = "TEST-B" , Value = "TEST-B"}
        };
    }
}
