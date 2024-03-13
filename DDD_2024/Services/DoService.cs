using DDD_2024.Data;
using DDD_2024.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DDD_2024.Services
{
    public class DoService: IDoService
    {
        private readonly DoContext _context;

        public DoService(DoContext context)
        {
            _context = context;
        }

        public string GetStatusName(string? doStatus)
        {
            if (string.IsNullOrEmpty(doStatus))
            {
                return string.Empty;
            }
            else if (doStatus == "N")
            {
                return "新單";
            }
            else if (doStatus == "C")
            {
                return "審核通過";
            }
            else if (doStatus == "D")
            {
                return "作廢";
            }
            else if (doStatus == "R")
            {
                return "審核未通過";
            }
            else if (doStatus == "X")
            {
                return "結案(獎金未發)";
            }
            else if (doStatus == "F")
            {
                return "結案(獎金已發)";
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetProjectID(string date)
        {
            var projectID = _context.ProjectM.Where(e => e.ProjectID != null && e.ProjectID.Substring(1,8) == date)
                .Select(e => e.ProjectID).ToList();

            if (projectID.Count == 0)
            {
                return "P" + date + "001";
            }
            else
            {
                var maxProjectID = projectID.Max();
                int maxID;
                if (int.TryParse(maxProjectID.Substring(9), out maxID))
                {
                    return "P" + date + (maxID + 1).ToString().PadLeft(3, '0');
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string GetDOID(string date)
        {
            var DoID = _context.Project_DO.Where(e => e.DoID != null && e.DoID.Substring(2, 8) == date)
                .Select(e => e.DoID).ToList();

            if (DoID.Count == 0)
            {
                return "DO" + date + "001";
            }
            else
            {
                var maxDoID = DoID.Max();
                int maxID;
                if (int.TryParse(maxDoID.Substring(10), out maxID))
                {
                    return "DO" + date + (maxID + 1).ToString().PadLeft(3, '0');
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}