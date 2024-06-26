﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DDD_2024.Models;

namespace DDD_2024.Data
{
    public class DDD_DutyContext : DbContext
    {
        public DDD_DutyContext (DbContextOptions<DDD_DutyContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.DutyM> DutyM { get; set; } = default!;
    }
}
