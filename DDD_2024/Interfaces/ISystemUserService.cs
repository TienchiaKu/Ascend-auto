using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Interfaces
{
    public interface ISystemUserService
    {
        bool IsCorrect(int userID, string userPWD);
        int NewUserID { get; }

        List<SelectListItem> GetSystemList { get; }
    }
}