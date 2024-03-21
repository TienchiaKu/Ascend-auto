using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DDD_2024.Models;
using MiniExcelLibs;

namespace DDD_2024.Data
{
    public class DoContext : DbContext
    {
        public DoContext (DbContextOptions<DoContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.ProjectM> ProjectM { get; set; } = default!;
        public DbSet<DDD_2024.Models.ProjectD> ProjectD { get; set; } = default!;
        public DbSet<DDD_2024.Models.Project_DO> Project_DO { get; set; } = default!;
        public DbSet<DDD_2024.Models.Project_DIDW> Project_DIDW { get; set; } = default!;
        public DbSet<DDD_2024.Models.Project_Emp> Project_Emp { get; set; } = default!;
    }

    public class ProjectMContext : DbContext
    {
        public ProjectMContext(DbContextOptions<ProjectMContext> options)
            : base(options)
        {
        }
        public DbSet<DDD_2024.Models.ProjectM> ProjectM { get; set; } = default!;
        public DbSet<DDD_2024.Models.BonusCalViewModel> BonusCalViewModel { get; set; } = default!;
    }

    public class ProjectDContext : DbContext
    {
        public ProjectDContext(DbContextOptions<ProjectDContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.ProjectD> ProjectD { get; set; } = default!;
    }

    public class Project_DIDWContext : DbContext
    {
        public Project_DIDWContext(DbContextOptions<Project_DIDWContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.Project_DIDW> Project_DIDW { get; set; } = default!;

        public DbSet<DDD_2024.Models.DwinViewModel> DwinViewModel { get; set; } = default!;
    }

    public class Project_EmpContext : DbContext
    {
        public Project_EmpContext(DbContextOptions<Project_EmpContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.Project_Emp> Project_Emp { get; set; } = default!;
    }
}
