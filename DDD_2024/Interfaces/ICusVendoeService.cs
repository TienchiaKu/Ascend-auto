using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Interfaces
{
    public interface ICusVendoeService
    {
        List<SelectListItem> DBSource { get; set; }
        List<SelectListItem> cusVendor { get; set; }
        Task<List<WD2SU01>> vendorlistAscend();
        Task<List<WD2SU01>> cuslistAscend();
        Task<List<WD2SU01>> vendorlistATI();
        Task<List<WD2SU01>> cuslistATI();
        Task<List<WD2SU01>> vendorlistKIR1N();
        Task<List<WD2SU01>> cuslistKIR1N();
        Task<List<WD2SU01>> vendorlistINTERTEK();
        Task<List<WD2SU01>> cuslistINTERTEK();
        Task<List<WD2SU01>> vendorlistTESTB();
        Task<List<WD2SU01>> cuslistTESTB();

        Task<IEnumerable<SelectListItem>> Ascendvendor_SelectList();
        List<SelectListItem> GetCusItem_Amico();
        List<SelectListItem> GetCusItem_Ascend();
        List<SelectListItem> GetCusItem_Intetek();
        List<SelectListItem> GetCusItem_Kir1n();
        List<SelectListItem> GetCusItem_TestB();

        string GetvendorName(string dbSource, string vendorID);
        (string DBScource, string CusCode) GetCusCode(string CusName);
        (string DBScource, string CusID) GetCusID(string CusName);
        string GetVendorID(string VendorName);
        string CreateNewCusID(string CusVenName);
        string CreateNewVenID(string CusVenName);
        string GetVenID(string VendorName);
    }
}