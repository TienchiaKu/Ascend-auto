using DDD_2024.Models;

namespace DDD_2024.Interfaces
{
    public interface IDwinService
    {
        List<DwinViewModel> ReadExcel(IFormFile Excelfile);

        Task<List<DwinViewModel>> GetDwinsAsync();
        Task<DwinViewModel> GetDwinAsync(string? ProjectID);

        string RejectDwin(string ProjectID);
        string ConfirmDwin(string ProjectID);
    }
}
