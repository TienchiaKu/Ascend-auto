using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DDD_2024.Models;

namespace DDD_2024.Data
{
    public class ASCENDContext : DbContext
    {
        public ASCENDContext(DbContextOptions<ASCENDContext> options)
            : base(options)
        {
        }
        public DbSet<DDD_2024.Models.WD2SU01> WD2SU01 { get; set; } = default!;
    }

    public class ATIContext : DbContext
    {
        public ATIContext(DbContextOptions<ATIContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.WD2SU01> WD2SU01 { get; set; } = default!;
    }

    public class KIR1NContext : DbContext
    {
        public KIR1NContext(DbContextOptions<KIR1NContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.WD2SU01> WD2SU01 { get; set; } = default!;
    }

    public class INTERTEKContext : DbContext
    {
        public INTERTEKContext(DbContextOptions<INTERTEKContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.WD2SU01> WD2SU01 { get; set; } = default!;
    }

    public class TESTBContext : DbContext
    {
        public TESTBContext(DbContextOptions<TESTBContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.WD2SU01> WD2SU01 { get; set; } = default!;
    }

    public class BizAutoContext : DbContext
    {
        public BizAutoContext(DbContextOptions<BizAutoContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.EmployeeM> employeeM { get; set; } = default!;
        public DbSet<DDD_2024.Models.DutyM> DutyM { get; set; } = default!;
    }

    public class CusVendorContext: DbContext
    {
        public CusVendorContext(DbContextOptions<CusVendorContext> options)
            : base(options)
        {
        }

        public DbSet<DDD_2024.Models.CusVendor> CusVendor { get; set; } = default!;
    }
}
