using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DDD_2024.Services
{
    public class CusVendorService : ICusVendoeService
    {
        private readonly ASCENDContext _contextAscend;
        private readonly ATIContext _contextATI;
        private readonly KIR1NContext _contextKIR1N;
        private readonly INTERTEKContext _contextINTERTEK;
        private readonly TESTBContext _contextTESTB;
        public List<SelectListItem> DBSource { get; set; }
        public List<SelectListItem> cusVendor { get; set; }

        public CusVendorService(ASCENDContext contextASCEND, ATIContext contextATI, KIR1NContext kIR1NContext, INTERTEKContext iNTERTEKContext, TESTBContext tESTBContext)
        {
            _contextAscend = contextASCEND;
            _contextATI = contextATI;
            _contextKIR1N = kIR1NContext;
            _contextINTERTEK = iNTERTEKContext;
            _contextTESTB = tESTBContext;

            DBSource = new List<SelectListItem>
            {
            new SelectListItem {Text = "ASCEND" , Value = "ASCEND"},
            new SelectListItem {Text = "ATI" , Value = "ATI"},
            new SelectListItem {Text = "INTERTEK" , Value = "INTERTEK"},
            new SelectListItem {Text = "KIR1N" , Value = "KIR1N"},
            new SelectListItem {Text = "TEST-B" , Value = "TEST-B"}
            };

            cusVendor = new List<SelectListItem>
            {
            new SelectListItem {Text = "客戶" , Value = "cus"},
            new SelectListItem {Text = "供應商" , Value = "vendor"}
            };
        }

        //取Ascend供應商資料:V開頭
        public async Task<List<WD2SU01>> vendorlistAscend()
        {           
            var model = await _contextAscend.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && item.SU01001.StartsWith("V"))
                .ToListAsync();

            return model;
        }

        //取Ascend客戶資料:B開頭
        public async Task<List<WD2SU01>> cuslistAscend()
        {
            var model = await _contextAscend.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && item.SU01001.StartsWith("B"))
                .ToListAsync();

            return model;
        }

        //取ATI供應商資料:V開頭
        public async Task<List<WD2SU01>> vendorlistATI()
        {
            var model = await _contextATI.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && item.SU01001.StartsWith("V"))
                .ToListAsync();

            return model;
        }

        //取ATI客戶資料:B開頭
        public async Task<List<WD2SU01>> cuslistATI()
        {
            var model = await _contextATI.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && item.SU01001.StartsWith("T"))
                .ToListAsync();

            return model;
        }

        //取KIR1N供應商資料:V開頭
        public async Task<List<WD2SU01>> vendorlistKIR1N()
        {
            var model = await _contextKIR1N.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && item.SU01001.StartsWith("V"))
                .ToListAsync();

            return model;
        }

        //取KIR1N客戶資料:T開頭
        public async Task<List<WD2SU01>> cuslistKIR1N()
        {
            var model = await _contextKIR1N.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && item.SU01001.StartsWith("T"))
                .ToListAsync();

            return model;
        }

        //取INTERTEK供應商資料:V開頭
        public async Task<List<WD2SU01>> vendorlistINTERTEK()
        {
            var model = await _contextINTERTEK.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && item.SU01001.StartsWith("V"))
                .ToListAsync();

            return model;
        }

        //取INTERTEK客戶資料:B開頭
        public async Task<List<WD2SU01>> cuslistINTERTEK()
        {
            var model = await _contextINTERTEK.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && item.SU01001.StartsWith("T"))
                .ToListAsync();

            return model;
        }

        //取TESTB供應商資料:V開頭
        public async Task<List<WD2SU01>> vendorlistTESTB()
        {
            var model = await _contextTESTB.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && item.SU01001.StartsWith("V"))
                .ToListAsync();

            return model;
        }

        //取TESTB客戶資料:B開頭
        public async Task<List<WD2SU01>> cuslistTESTB()
        {
            var model = await _contextTESTB.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && item.SU01001.StartsWith("T"))
                .ToListAsync();

            return model;
        }

        public async Task<IEnumerable<SelectListItem>> Ascendvendor_SelectList()
        {
            var vendors = await _contextAscend.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && item.SU01001.StartsWith("V"))
                .ToListAsync();

            var selectList = vendors.Select(vendor => new SelectListItem
            {
                Value = vendor.SU01001, 
                Text = vendor.SU01003
            }).ToList();

            return selectList;
        }

        public List<SelectListItem> GetAscendvendorList()
        {
            var vendors = _contextAscend.WD2SU01.Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && item.SU01001.StartsWith("V")).ToList();
            return vendors.Select(e => new SelectListItem
            {
                Value = e.SU01001,
                Text = e.SU01003
            }).ToList();
        }

        public List<SelectListItem> GetTESTBvendorList()
        {
            var vendors = _contextTESTB.WD2SU01.Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && item.SU01001.StartsWith("V")).ToList();
            return vendors.Select(e => new SelectListItem
            {
                Value = e.SU01001,
                Text = e.SU01003
            }).ToList();
        }

        public List<SelectListItem> GetAscendcusList()
        {
            var vendors = _contextAscend.WD2SU01.Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && item.SU01001.StartsWith("B")).ToList();

            return vendors.Select(e => new SelectListItem
            {
                Value = e.SU01001,
                Text = e.SU01003
            }).ToList();
        }

        public List<SelectListItem> GetIntertekcusList()
        {
            var vendors = _contextINTERTEK.WD2SU01.Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && item.SU01001.StartsWith("T")).ToList();

            return vendors.Select(e => new SelectListItem
            {
                Value = e.SU01001,
                Text = e.SU01003
            }).ToList();
        }

        public string GetvendorName(string dbSource,string vendorID)
        {
            string vendorName = string.Empty;
            
            if (!string.IsNullOrEmpty(dbSource) && !string.IsNullOrEmpty(vendorID))
            {
                switch (dbSource)
                {
                    case "ASCEND":
                        vendorName = _contextAscend.WD2SU01.Where(item => item.SU01001 == vendorID).Select(item => item.SU01003).FirstOrDefault() ?? "";
                        break;
                    case "ATI":
                        vendorName = _contextATI.WD2SU01.Where(item => item.SU01001 == vendorID).Select(item => item.SU01003).FirstOrDefault() ?? "";
                        break;
                    case "INTERTEK":
                        vendorName = _contextINTERTEK.WD2SU01.Where(item => item.SU01001 == vendorID).Select(item => item.SU01003).FirstOrDefault() ?? "";
                        break;
                    case "KIR1N":
                        vendorName = _contextKIR1N.WD2SU01.Where(item => item.SU01001 == vendorID).Select(item => item.SU01003).FirstOrDefault() ?? "";
                        break;
                    case "TEST-B":
                        vendorName = _contextTESTB.WD2SU01.Where(item => item.SU01001 == vendorID).Select(item => item.SU01003).FirstOrDefault() ?? "";
                        break;
                    default: break;
                }
            }
            return vendorName;
        }
    }
}
