using DDD_2024.Models;

namespace DDD_2024.Interfaces
{
    public interface IDinService
    {
        List<DinViewModel> ImportDin(IFormFile Excelfile);

        Task<List<DinViewModel>> GetDinsAsync();
        Task<DinViewModel> GetDinAsync(string? ProjectID);

        string RejectDin(string ProjectID);
        string ConfirmDin(string ProjectID);
    }
}
