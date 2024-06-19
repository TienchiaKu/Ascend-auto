using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Interfaces
{
    public interface ICusVendorService
    {
        List<SelectListItem> DBSource { get; set; }
        List<SelectListItem> cusVendor { get; set; }
        List<SelectListItem> GetCusVenType { get; }
        List<SelectListItem> GetDBSource { get; }

        Task EditAutoCusVen(CusVendor model);
        Task SuspendCusVen(CusVendor model);
        Task<List<CusReportViewModel>> GetSuspendList();

        Task<List<CusReportViewModel>> GetAutoCusVen(string type);
        Task<List<CusReportViewModel>> GetAllCus();
        Task<List<CusReportViewModel>> GetAllVendor();

        Task<List<SelectListItem>> GetAllCus_Selector();
        Task<List<SelectListItem>> GetAllVendor_Selector();

        Task<string> GetCusDBName(string CusID);
        Task<string> GetCusName(string CusID);

        string GetvendorName(string dbSource, string vendorID);
        (string DBScource, string CusCode) GetCusCode(string CusName);
        (string DBScource, string CusID) GetCusID(string CusName);
        string GetVendorID(string VendorName);
        string GetNewCusID();
        string GetNewVenID();
        string CreateNewCusID(string CusVenName);
        string CreateNewVenID(string CusVenName);
        string GetVenID(string VendorName);
        bool CheckCusVenName(string cusVenName);
        string GetVendorName(string CusID);

        void ImportCusVen(List<CusUploadViewModel> list_CusViewModels);
    }
}