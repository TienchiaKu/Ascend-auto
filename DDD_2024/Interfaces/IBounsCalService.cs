using DDD_2024.Data;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DDD_2024.Interfaces
{
    public interface IBounsCalService
    {
        Task<List<BonusCalViewModel>> GetProjects_DO();
        Task<List<BonusCalViewModel>> GetProjects_DINWIN();

        List<ProjectBonusViewModel> BonusConfirm(string[] projectIds);
        List<EmployeeBonusViewMode> GetDIDWBonusbyEmployee(List<ProjectBonusViewModel> list_ProjectBonus);
        List<EmployeeBonusViewMode> GetDOBonusbyEmployee(List<ProjectBonusViewModel> list_ProjectBonus);
    }
}
