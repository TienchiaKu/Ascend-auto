using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Services
{
    public class ProjectEmpService : IProjectEmpService
    {
        private readonly Project_EmpContext _project_EmpContext;

        public ProjectEmpService(Project_EmpContext project_EmpContext)
        {
            _project_EmpContext = project_EmpContext;
        }

        public int NewSEQ
        {
            get
            {
                if (_project_EmpContext.Project_Emp.Count() == 0)
                {
                    return 1;
                }
                else
                {
                    return _project_EmpContext.Project_Emp.Max(e => e.SEQ) + 1;
                }
            }
        }
    }
}
