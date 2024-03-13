using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DDD_2024.Models;

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
    }
}
