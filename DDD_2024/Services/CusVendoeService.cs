using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        private readonly CusVendorContext _cusVendorContext;
        public List<SelectListItem> DBSource { get; set; }
        public List<SelectListItem> cusVendor { get; set; }

        public CusVendorService(ASCENDContext contextASCEND, ATIContext contextATI, KIR1NContext kIR1NContext, 
            INTERTEKContext iNTERTEKContext, TESTBContext tESTBContext, CusVendorContext cusVendorContext)
        {
            _contextAscend = contextASCEND;
            _contextATI = contextATI;
            _contextKIR1N = kIR1NContext;
            _contextINTERTEK = iNTERTEKContext;
            _contextTESTB = tESTBContext;
            _cusVendorContext = cusVendorContext;

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
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("V"))
                .ToListAsync();

            return model;
        }

        //取Ascend客戶資料:B開頭
        public async Task<List<WD2SU01>> cuslistAscend()
        {
            var model = await _contextAscend.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("B"))
                .ToListAsync();

            return model;
        }

        //取ATI供應商資料:VAM開頭
        public async Task<List<WD2SU01>> vendorlistATI()
        {
            var model = await _contextATI.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("VAM"))
                .ToListAsync();

            return model;
        }

        //取ATI客戶資料:CAM開頭
        public async Task<List<WD2SU01>> cuslistATI()
        {
            var model = await _contextATI.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("CAM"))
                .ToListAsync();

            return model;
        }

        //取KIR1N供應商資料:VKI開頭
        public async Task<List<WD2SU01>> vendorlistKIR1N()
        {
            var model = await _contextKIR1N.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("VKI"))
                .ToListAsync();

            return model;
        }

        //取KIR1N客戶資料:CKI開頭
        public async Task<List<WD2SU01>> cuslistKIR1N()
        {
            var model = await _contextKIR1N.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("CKI"))
                .ToListAsync();

            return model;
        }

        //取INTERTEK供應商資料:VIT開頭
        public async Task<List<WD2SU01>> vendorlistINTERTEK()
        {
            var model = await _contextINTERTEK.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("VIT"))
                .ToListAsync();

            return model;
        }

        //取INTERTEK客戶資料:CIT開頭
        public async Task<List<WD2SU01>> cuslistINTERTEK()
        {
            var model = await _contextINTERTEK.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("CIT"))
                .ToListAsync();

            return model;
        }

        //取TESTB供應商資料:V開頭
        public async Task<List<WD2SU01>> vendorlistTESTB()
        {
            var model = await _contextTESTB.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("V"))
                .ToListAsync();

            return model;
        }

        //取TESTB客戶資料:B開頭
        public async Task<List<WD2SU01>> cuslistTESTB()
        {
            var model = await _contextTESTB.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("T"))
                .ToListAsync();

            return model;
        }

        public async Task<IEnumerable<SelectListItem>> Ascendvendor_SelectList()
        {
            var vendors = await _contextAscend.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("V"))
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
            var vendors = _contextAscend.WD2SU01.Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("V")).ToList();
            return vendors.Select(e => new SelectListItem
            {
                Value = e.SU01001,
                Text = e.SU01003
            }).ToList();
        }

        public List<SelectListItem> GetTESTBvendorList()
        {
            var vendors = _contextTESTB.WD2SU01.Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("V")).ToList();
            return vendors.Select(e => new SelectListItem
            {
                Value = e.SU01001,
                Text = e.SU01003
            }).ToList();
        }

        public List<SelectListItem> GetCusItem_Amico()
        {
            var vendors = _contextATI.WD2SU01.Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("CAM")).ToList();

            return vendors.Select(e => new SelectListItem
            {
                Value = e.SU01001,
                Text = e.SU01003
            }).ToList();
        }

        public List<SelectListItem> GetCusItem_Ascend()
        {
            var vendors = _contextAscend.WD2SU01.Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("B")).ToList();

            return vendors.Select(e => new SelectListItem
            {
                Value = e.SU01001,
                Text = e.SU01003
            }).ToList();
        }

        public List<SelectListItem> GetCusItem_Intetek()
        {
            var vendors = _contextINTERTEK.WD2SU01.Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("CIT")).ToList();

            return vendors.Select(e => new SelectListItem
            {
                Value = e.SU01001,
                Text = e.SU01003
            }).ToList();
        }

        public List<SelectListItem> GetCusItem_Kir1n()
        {
            var vendors = _contextKIR1N.WD2SU01.Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("CKI")).ToList();

            return vendors.Select(e => new SelectListItem
            {
                Value = e.SU01001,
                Text = e.SU01003
            }).ToList();
        }

        public List<SelectListItem> GetCusItem_TestB()
        {
            var vendors = _contextTESTB.WD2SU01.Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("T")).ToList();

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
                    case "Auto":
                        vendorName = _cusVendorContext.CusVendor.Where(item => item.CusVenID == vendorID).Select(item => item.CusVenName).FirstOrDefault() ?? "";
                        break;

                    default: break;
                }
            }
            return vendorName;
        }

        public (string DBScource, string CusCode) GetCusCode(string CusName)
        {
            string DBScource = string.Empty;
            string CusCode = string.Empty;

            var code = _contextAscend.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01004) && e.SU01004.Contains(CusName) && 
            !string.IsNullOrEmpty(e.SU01001) && e.SU01001.StartsWith("B"))?.SU01001;
            
            if (!string.IsNullOrEmpty(code))
            {
                DBScource = "ASCEND";
                CusCode = code;
                return (DBScource, CusCode);
            }

            if (string.IsNullOrEmpty(code))
            {
                code = _contextINTERTEK.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01004) && e.SU01004.Contains(CusName) &&
                !string.IsNullOrEmpty(e.SU01001) && e.SU01001.StartsWith("CIT"))?.SU01001;

                if (!string.IsNullOrEmpty(code))
                {
                    DBScource = "INTERTEK";
                    CusCode = code;
                    return (DBScource, CusCode);
                }
            }

            if (!string.IsNullOrEmpty(code))
            {
                code = _contextTESTB.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01004) && e.SU01004.Contains(CusName) &&
                !string.IsNullOrEmpty(e.SU01001) && e.SU01001.StartsWith("T"))?.SU01001;

                if (!string.IsNullOrEmpty(code))
                {
                    DBScource = "TEST-B";
                    CusCode = code;
                    return (DBScource, CusCode);
                }
            }

            if (!string.IsNullOrEmpty(code))
            {
                code = _contextATI.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01004) && e.SU01004.Contains(CusName) &&
                !string.IsNullOrEmpty(e.SU01001) && e.SU01001.StartsWith("CAM"))?.SU01001;
                if (!string.IsNullOrEmpty(code))
                {
                    DBScource = "ATI";
                    CusCode = code;
                    return (DBScource, CusCode);
                }
            }

            if (!string.IsNullOrEmpty(code))
            {
                code = _contextKIR1N.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01004) && e.SU01004.Contains(CusName) &&
                !string.IsNullOrEmpty(e.SU01001) && e.SU01001.StartsWith("CKI"))?.SU01001;

                if (!string.IsNullOrEmpty(code))
                {
                    DBScource = "KIR1N";
                    CusCode = code;
                    return (DBScource, CusCode);
                }
            }

            if (!string.IsNullOrEmpty(code))
            {
                code = _cusVendorContext.CusVendor.FirstOrDefault(e => e.CusVenName == CusName)?.CusVenID;

                if (!string.IsNullOrEmpty(code))
                {
                    DBScource = "Auto";
                    CusCode = code;
                    return (DBScource, CusCode);
                }
            }

            // 如果没有找到 CusCode，則返回空字串符
            return (DBScource, CusCode);
        }

        public string GetVendorID(string VendorName)
        {
            if (!string.IsNullOrEmpty(VendorName))
            {
                var VendorCode = _contextAscend.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01003) && e.SU01003.ToUpper() == VendorName.ToUpper() &&
                !string.IsNullOrEmpty(e.SU01001) && e.SU01001.ToUpper().StartsWith("V"))?.SU01001 ?? string.Empty;

                if (string.IsNullOrEmpty(VendorCode))
                {
                    VendorCode = _cusVendorContext.CusVendor.FirstOrDefault(e => !string.IsNullOrEmpty(e.CusVenName) && e.CusVenName.ToUpper() == VendorName.ToUpper())?.CusVenID ?? string.Empty;
                }

                return VendorCode;
            }
            else
            {
                return string.Empty;
            }
        }

        public string CreateNewCusID(string CusVenName)
        {
            //1.比對有無重複
            //2.無重複
            //3.取最新的代號
            //4.InsertTo table cusVendor

            string OldCusName = string.Empty;
            string nextCusVenID = string.Empty;

            //比對有無重複的客戶名稱
            var chk_CusID = _cusVendorContext.CusVendor.Where(e => e.CusVenName == CusVenName.Trim()).Select(e => e.CusVenID).FirstOrDefault();

            //無重複客戶名稱
            if (chk_CusID == null)
            {
                //檢查是否有客戶資料
                var Chk_Cus = _cusVendorContext.CusVendor.Where(e => e.CusVenID != null && e.CusVenID.StartsWith("C")).ToList();

                if (Chk_Cus.Count == 0)
                {
                    nextCusVenID = "C0001";
                }
                else
                {
                    string maxCusVenID = Chk_Cus.Max(e => e.CusVenID);

                    if (!string.IsNullOrEmpty(maxCusVenID))
                    {
                        // 提取數字部分
                        string numberPart = maxCusVenID.Substring(1);

                        // 將數字部分轉換為整數並加1
                        int nextNumber = int.Parse(numberPart) + 1;

                        // 生成下一個CusVenID
                        nextCusVenID = "C" + nextNumber.ToString("0000");
                    }
                }
            }
            else
            {
                nextCusVenID = chk_CusID;
            }

            if (!string.IsNullOrEmpty(nextCusVenID))
            {
                //InsertTo table cusVendor
                var model = new CusVendor
                {
                    CusVenID = nextCusVenID,
                    CusVenName = CusVenName.Trim(),
                    DBSource = "Auto",
                    IsUse = "Y",
                    UpdateDate = DateTime.Now
                };

                _cusVendorContext.Add(model);
                _cusVendorContext.SaveChanges();
            }

            return nextCusVenID;
        }

        public string CreateNewVenID(string CusVenName)
        {
            //1.比對有無重複
            //2.無重複
            //3.取最新的代號
            //4.InsertTo table cusVendor

            string OldCusName = string.Empty;
            string nextCusVenID = string.Empty;

            //比對有無重複的客戶名稱
            var chk_CusName = _cusVendorContext.CusVendor.Where(e => e.CusVenName == CusVenName.Trim()).Select(e => e.CusVenID).FirstOrDefault();

            if(chk_CusName == null)
            {
                //檢查是否有重複的客戶資料
                var Chk_Cus = _cusVendorContext.CusVendor.Where(e => e.CusVenID != null && e.CusVenID.StartsWith("V")).ToList();

                if (Chk_Cus.Count == 0)
                {
                    nextCusVenID = "V0001";
                }
                else
                {
                    string maxCusVenID = Chk_Cus.Max(e => e.CusVenID);

                    if (!string.IsNullOrEmpty(maxCusVenID))
                    {
                        // 提取數字部分
                        string numberPart = maxCusVenID.Substring(1);

                        // 將數字部分轉換為整數並加1
                        int nextNumber = int.Parse(numberPart) + 1;

                        // 生成下一個CusVenID
                        nextCusVenID = "V" + nextNumber.ToString("0000");
                    }
                }
            }

            if (!string.IsNullOrEmpty(nextCusVenID))
            {
                //InsertTo table cusVendor
                var model = new CusVendor
                {
                    CusVenID = nextCusVenID,
                    CusVenName = CusVenName.Trim(),
                    DBSource = "Auto",
                    IsUse = "Y",
                    UpdateDate = DateTime.Now
                };

                _cusVendorContext.Add(model);
                _cusVendorContext.SaveChanges();
            }

            return nextCusVenID;
        }

        public (string DBScource, string CusID) GetCusID(string CusName)
        {
            /*
                1.用CusName比對文中5個DB以及自建的客供商Table，有找到對應的CusDB與CusID，則回填
                2a.有找到對應的CusDB與CusID，回傳DBScource及CusCode
                
                2b.未找到對應的CusDB與CusID，在自建的Table寫入一筆客戶資料
                3b.回傳新建的DBScource及CusCode
            */

            string DBScource = string.Empty;
            string CusID = string.Empty;

            (DBScource, CusID) = SearchCusID(CusName);

            if(string.IsNullOrEmpty(DBScource) && string.IsNullOrEmpty(CusID))
            {
                string maxCusVenID = _cusVendorContext.CusVendor.Where(e => e.CusVenID.StartsWith("C")).Max(e => e.CusVenID) ?? string.Empty;
                string nextCusVenID = string.Empty;

                if (string.IsNullOrEmpty(maxCusVenID))
                {
                    nextCusVenID = "C0001";
                }
                else
                {
                    // 提取數字部分
                    string numberPart = maxCusVenID.Substring(1);

                    // 將數字部分轉換為整數並加1
                    int nextNumber = int.Parse(numberPart) + 1;

                    // 生成下一個CusVenID
                    nextCusVenID = "C" + nextNumber.ToString("0000");
                }

                if (!string.IsNullOrEmpty(nextCusVenID))
                {
                    //InsertTo table cusVendor
                    var model = new CusVendor
                    {
                        CusVenID = nextCusVenID,
                        CusVenName = CusName.Trim(),
                        DBSource = "Auto",
                        IsUse = "Y",
                        UpdateDate = DateTime.Now
                    };

                    _cusVendorContext.Add(model);
                    _cusVendorContext.SaveChanges();
                }

                return ("Auto", nextCusVenID);
            }
            else
            {
                return (DBScource, CusID);
            }
        }

        public (string DBScource, string CusID) SearchCusID (string CusName)
        {
            string DBScource = string.Empty;
            string CusID = string.Empty;
            string CusCode = string.Empty;

            CusCode = _contextAscend.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01004) && e.SU01004.Contains(CusName) &&
           !string.IsNullOrEmpty(e.SU01001) && e.SU01001.StartsWith("B"))?.SU01001 ?? string.Empty;

            if (!string.IsNullOrEmpty(CusCode))
            {
                DBScource = "ASCEND";
                CusID = CusCode;
                return (DBScource, CusID);
            }

            if (string.IsNullOrEmpty(CusCode))
            {
                CusCode = _contextINTERTEK.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01004) && e.SU01004.Contains(CusName) &&
                !string.IsNullOrEmpty(e.SU01001) && e.SU01001.StartsWith("CIT"))?.SU01001 ?? string.Empty;

                if (!string.IsNullOrEmpty(CusCode))
                {
                    DBScource = "INTERTEK";
                    CusID = CusCode;
                    return (DBScource, CusID);
                }
            }

            if (string.IsNullOrEmpty(CusCode))
            {
                CusCode = _contextTESTB.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01004) && e.SU01004.Contains(CusName) &&
                !string.IsNullOrEmpty(e.SU01001) && e.SU01001.StartsWith("T"))?.SU01001 ?? string.Empty;

                if (!string.IsNullOrEmpty(CusCode))
                {
                    DBScource = "TEST-B";
                    CusID = CusCode;
                    return (DBScource, CusID);
                }
            }

            if (string.IsNullOrEmpty(CusCode))
            {
                CusCode = _contextATI.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01004) && e.SU01004.Contains(CusName) &&
                !string.IsNullOrEmpty(e.SU01001) && e.SU01001.StartsWith("CAM"))?.SU01001 ?? string.Empty;

                if (!string.IsNullOrEmpty(CusCode))
                {
                    DBScource = "ATI";
                    CusID = CusCode;
                    return (DBScource, CusID);
                }
            }

            if (string.IsNullOrEmpty(CusCode))
            {
                CusCode = _contextKIR1N.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01004) && e.SU01004.Contains(CusName) &&
                !string.IsNullOrEmpty(e.SU01001) && e.SU01001.StartsWith("CKI"))?.SU01001 ?? string.Empty;

                if (!string.IsNullOrEmpty(CusCode))
                {
                    DBScource = "KIR1N";
                    CusID = CusCode;
                    return (DBScource, CusID);
                }
            }

            if (string.IsNullOrEmpty(CusCode))
            {
                CusCode = _cusVendorContext.CusVendor.FirstOrDefault(e => !string.IsNullOrEmpty(e.CusVenName) && e.CusVenName.ToUpper() == CusName.ToUpper())?.CusVenID ?? string.Empty;

                if (!string.IsNullOrEmpty(CusCode))
                {
                    DBScource = "Auto";
                    CusID = CusCode;
                    return (DBScource, CusID);
                }
            }
            return (DBScource, CusID);
        }

        public string GetVenID(string VendorName)
        {
            string VenID = string.Empty;

            VenID = SearchVenID(VendorName);

            if (string.IsNullOrEmpty(VenID))
            {
                string maxCusVenID = _cusVendorContext.CusVendor.Where(e => e.CusVenID.StartsWith("V")).Max(e => e.CusVenID) ?? string.Empty;
                string nextCusVenID = string.Empty;

                if (string.IsNullOrEmpty(maxCusVenID))
                {
                    nextCusVenID = "V0001";
                }
                else
                {
                    // 提取數字部分
                    string numberPart = maxCusVenID.Substring(1);

                    // 將數字部分轉換為整數並加1
                    int nextNumber = int.Parse(numberPart) + 1;

                    // 生成下一個CusVenID
                    nextCusVenID = "V" + nextNumber.ToString("0000");
                }

                if (!string.IsNullOrEmpty(nextCusVenID))
                {
                    //InsertTo table cusVendor
                    var model = new CusVendor
                    {
                        CusVenID = nextCusVenID,
                        CusVenName = VendorName.Trim(),
                        DBSource = "Auto",
                        IsUse = "Y",
                        UpdateDate = DateTime.Now
                    };

                    _cusVendorContext.Add(model);
                    _cusVendorContext.SaveChanges();
                }

                return nextCusVenID;
            }
            else
            {
                return VenID;
            }
        }

        public string SearchVenID(string VendorName)
        {
            if (!string.IsNullOrEmpty(VendorName))
            {
                var VendorCode = _contextAscend.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01003) && e.SU01003.ToUpper() == VendorName.ToUpper() &&
                !string.IsNullOrEmpty(e.SU01001) && e.SU01001.ToUpper().StartsWith("V"))?.SU01001 ?? string.Empty;

                if (string.IsNullOrEmpty(VendorCode))
                {
                    VendorCode = _cusVendorContext.CusVendor.FirstOrDefault(e => !string.IsNullOrEmpty(e.CusVenName) && e.CusVenName.ToUpper() == VendorName.ToUpper())?.CusVenID ?? string.Empty;
                }

                return VendorCode;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
