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
        Task<(List<DoReportViewModel>?, List<DoBonusViewModel>)> GetDoBonus(string[] projectIds);

        List<ProjectBonusViewModel> BonusConfirm(string[] projectIds);
        List<EmployeeBonusViewMode> GetDIDWBonusbyEmployee(List<ProjectBonusViewModel> list_ProjectBonus);
        List<EmployeeBonusViewMode> GetDOBonusbyEmployee(List<ProjectBonusViewModel> list_ProjectBonus);
    }
}
