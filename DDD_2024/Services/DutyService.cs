using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Services
{
    public class DutyService : IDutyService
    {
        private readonly DDD_DutyContext _context;
        public List<SelectListItem> GetDutyList { get; set; }

        public DutyService(DDD_DutyContext context)
        {
            _context = context;

            GetDutyList = new List<SelectListItem>
            {
                new SelectListItem{ Text = "---請選擇---", Value =""},
                new SelectListItem{ Text = "PM", Value ="PM"},
                new SelectListItem{ Text = "Sales", Value ="Sales"},
                new SelectListItem{ Text = "FAE", Value ="FAE"},
                new SelectListItem{ Text = "RBU", Value ="RBU"},
            };
        }

        public int NewDutyID
        {
            get
            {
                if (_context.DutyM == null)
                {
                    return 1;
                }

                if (_context.DutyM.Count() == 0)
                {
                    return 1;
                }
                else
                {
                    return _context.DutyM.Max(e => e.DutyID) + 1;
                }
            }
        }
    }
}
