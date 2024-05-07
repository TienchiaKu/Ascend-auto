using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DDD_2024.Interfaces
{
    public interface IDoService
    {
        int NewProjectEmpSEQ { get; }
        List<SelectListItem> GetStatus { get; }

        string GetStatusName(string? doStatus);
        string GetProjectID(string date);
        string GetDOID(string date);
        bool chk_DoTransDin(string projectID);
        string ConfirmDo(string DoId);
        string ConfiirmDos(string[] DoIds);
        string RejectDO(string DoId);
        string RejectDos(string[] DoIds);

        List<SelectListItem> GetTradingStatus();
        string GetTradingStatusName(string TradingStatus);

        Task<List<DoViewModel>> GetDOsAsync();
        Task<List<DoViewModel>> GetDOsFilterAsync(string projectStatus, string applicant);
        Task<DoViewModel> GetDoAsync(string ProjectID);
    }
}
