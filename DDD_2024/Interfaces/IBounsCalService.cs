using DDD_2024.Data;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;

namespace DDD_2024.Interfaces
{
    public interface IBounsCalService
    {
        Task<List<BonusCalViewModel>> GetProjects_Do();
        Task<List<BonusCalViewModel>> GetProjects_DoFilter(List<string> list_months);
        Task<List<BonusCalViewModel>> GetProjects_DINWIN();
        List<DoBonusViewModel> GetEmpBonus(List<BDoReportViewModel> list);
        Task<List<BDoReportViewModel>> GetDoReport(BDoReportFilter filterModel);

        List<ProjectBonusViewModel> BonusConfirm(string[] projectIds);
        List<EmployeeBonusViewMode> GetDIDWBonusbyEmployee(List<ProjectBonusViewModel> list_ProjectBonus);
        List<EmployeeBonusViewMode> GetDOBonusbyEmployee(List<ProjectBonusViewModel> list_ProjectBonus);
        List<SelectListItem> GetRegion();
    }
}
