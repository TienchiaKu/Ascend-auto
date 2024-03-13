using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DDD_2024.Models;

namespace DDD_2024.Data
{
    public class DDD_EmployeeContext : DbContext
    {
        public DDD_EmployeeContext (DbContextOptions<DDD_EmployeeContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.EmployeeM> DDD_Employee { get; set; } = default!;
    }
}
