using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DDD_2024.Models;

namespace DDD_2024.Data
{
    public class DDD_DoMContext : DbContext
    {
        public DDD_DoMContext (DbContextOptions<DDD_DoMContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.DDD_DoM> DDD_DoM { get; set; } = default!;
    }
}
