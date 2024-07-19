using DDD_2024.Models;

namespace DDD_2024.Interfaces
{
    public interface IDinService
    {
        List<DinUploadViewModel> ImportDIDW(IFormFile Excelfile);

        Task<List<DinIndexViewModel>> GetDinsAsync();
        Task<DinViewModel> GetDinAsync(string? ProjectID);
        Task<DinCreateViewModel> GetEditDin(string? ProjectID);

        string RejectDin(string ProjectID);
        string ConfirmDin(string ProjectID);
    }
}
