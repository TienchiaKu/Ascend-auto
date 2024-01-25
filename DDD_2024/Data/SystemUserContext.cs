using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DDD_2024.Models;

namespace DDD_2024.Data
{
    public class SystemUserContext : DbContext
    {
        public SystemUserContext (DbContextOptions<SystemUserContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.DDD_SystemUser> DDD_SystemUser { get; set; } = default!;
    }
}
