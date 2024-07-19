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
        string QueryDoID(string projectID);

        List<SelectListItem> GetTradingStatus(string? TradingStatus);
        List<SelectListItem> GetIsFinsih();
        List<SelectListItem> GetRegion();
        string GetTradingStatusName(string TradingStatus);

        Task<List<DoViewModel>> GetDOsAsync();
        Task<List<DoViewModel>> GetDOsFilterAsync(string projectStatus, string applicant, List<string> list_months);
        Task<DoViewModel> GetDoAsync(string ProjectID);
        Task<List<DoReport_ViewModel>> GetDosReport(DoReportFilterViewModel model);
        Task<List<DOASUViewModel>> GetDOASUsAsync(string DoID);
        Task<DoEditViewModel> GetEditDo(string ProjectID);

        Task EditDo(DoEditViewModel model);
        Task CreateDoAS(DOASUViewModel model);
        Task CreateDO(DoCreateViewModel model);

        List<DoViewModel> ImportDo(List<DoViewModel> list_doViewModels);
        List<DoMaintainModel_Result> DoMaintain(List<DoMaintainModel_F> list_finish,List<DoMaintainModel_U>list_update);
        List<DoReportUpload> UpdateDoStatus(List<DoReportUpload> list_DoUpload);
    }
}
