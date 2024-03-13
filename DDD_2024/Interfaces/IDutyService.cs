using Microsoft.AspNetCore.Mvc.Rendering;

namespace DDD_2024.Interfaces
{
    public interface IDutyService
    {
        int NewDutyID { get; }

        List<SelectListItem> GetDutyList { get; set; }
    }
}
