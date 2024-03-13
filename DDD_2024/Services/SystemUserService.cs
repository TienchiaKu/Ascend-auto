using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DDD_2024.Services
{
    public class SystemUserService: ISystemUserService
    {
        private readonly SystemUserContext _context;

        public SystemUserService(SystemUserContext context)
        {
            _context = context;
        }

        public List<SelectListItem> GetSystemList
        {
            get
            {
                var systemUser = _context.DDD_SystemUser.Where(e => e.IsActive == "Y").ToList();
                return systemUser.Select(e => new SelectListItem
                {
                    Value = e.UserID.ToString(),
                    Text = e.UserName
                }).ToList();
            }
        }

        public bool IsCorrect(int userID, string userPWD) 
        {
            return _context.DDD_SystemUser.Any(e => e.UserID == userID && e.UserPWD == userPWD);
        }

        public int NewUserID
        {
            get
            {
                if (_context.DDD_SystemUser.Count() == 0)
                {
                    return 1;
                }
                else
                {
                    return _context.DDD_SystemUser.Max(e => e.UserID) + 1;
                }              
            }
        }
    }
}
