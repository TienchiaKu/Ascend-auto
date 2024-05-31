using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DDD_2024.Interfaces
{
    public interface IDoService
    {
        int NewProjectEmpSEQ { get; }
        int NewProjectDOASSEQ { get; }
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
        Task<List<DoReportViewModel>> GetDosReport(DoReportFilterViewModel model);
        Task<List<DOASUViewModel>> GetDOASUsAsync(string DoID);

        Task EditDo(DoViewModel model);
        Task CreateDoAS(DOASUViewModel model);
        Task CreateDO(DoViewModel model);

        List<DOASU_Upload_ViewModel> ImportDOASU(List<DOASU_Upload_ViewModel> list_Upload);
    }
}
