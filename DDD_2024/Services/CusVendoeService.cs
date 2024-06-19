using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DDD_2024.Services
{
    public class CusVendorService : ICusVendorService
    {
        private readonly ASCENDContext _contextAscend;
        private readonly ATIContext _contextATI;
        private readonly KIR1NContext _contextKIR1N;
        private readonly TESTBContext _contextTESTB;
        private readonly INTERTEKContext _contextINTERTEK;
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

        public string GetvendorName(string dbSource,string vendorID)
        {
            string vendorName = string.Empty;             
            
            if (!string.IsNullOrEmpty(dbSource) && !string.IsNullOrEmpty(vendorID))
            {
                switch (dbSource)
                {
                    case "Ascend":
                        vendorName = _contextAscend.WD2SU01.Where(item => item.SU01001 == vendorID).Select(item => item.SU01003).FirstOrDefault() ?? "";
                        break;
                    case "ATI":
                        vendorName = _contextATI.WD2SU01.Where(item => item.SU01001 == vendorID).Select(item => item.SU01003).FirstOrDefault() ?? "";
                        break;
                    case "Intertek":
                        vendorName = _contextINTERTEK.WD2SU01.Where(item => item.SU01001 == vendorID).Select(item => item.SU01003).FirstOrDefault() ?? "";
                        break;
                    case "KIR1N":
                        vendorName = _contextKIR1N.WD2SU01.Where(item => item.SU01001 == vendorID).Select(item => item.SU01003).FirstOrDefault() ?? "";
                        break;
                    case "TestB":
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
            !string.IsNullOrEmpty(e.SU01001) && (e.SU01001.StartsWith("A") || e.SU01001.StartsWith("B")))?.SU01001;
            
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

        public string GetNewCusID()
        {
            string nextCusVenID = string.Empty;

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

            return nextCusVenID;
        }

        public string CreateNewCusID(string CusVenName)
        {
            //1.比對有無重複
            //2.無重複
            //3.取最新的代號
            //4.InsertTo table cusVendor

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
            string CusCode = string.Empty;

            CusCode = _contextAscend.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01003) && e.SU01003.Contains(CusName) &&
           !string.IsNullOrEmpty(e.SU01001) && (e.SU01001.StartsWith("A") || e.SU01001.StartsWith("B")))?.SU01001 ?? string.Empty;

            if (!string.IsNullOrEmpty(CusCode))
            {
                DBScource = "Ascend";
                CusID = CusCode;
                return (DBScource, CusID);
            }

            if (string.IsNullOrEmpty(CusCode))
            {
                CusCode = _contextINTERTEK.WD2SU01.FirstOrDefault(e => !string.IsNullOrEmpty(e.SU01004) && e.SU01004.Contains(CusName) &&
                !string.IsNullOrEmpty(e.SU01001) && e.SU01001.StartsWith("CIT"))?.SU01001 ?? string.Empty;

                if (!string.IsNullOrEmpty(CusCode))
                {
                    DBScource = "Intertek";
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
                    DBScource = "TestB";
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

        public string GetNewVenID()
        {
            string nextCusVenID = string.Empty;

            //檢查是否有客戶資料
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

            return nextCusVenID;
        }

        public bool CheckCusVenName(string cusVenName)
        {
            var vendors = _cusVendorContext.CusVendor.ToList();
            return vendors.Any(v => v.CusVenName == cusVenName);
        }

        public async Task<List<CusReportViewModel>> GetAllCus()
        {
            List<CusReportViewModel> list_AllCus = new List<CusReportViewModel>();
            List<WD2SU01>list_WD2SU01 = new List<WD2SU01>();
            List<CusVendor>list_CusVendor = new List<CusVendor>();
            List<CusVendor>list_CusVendorSuspend = new List<CusVendor>();

            list_CusVendorSuspend = await _cusVendorContext.CusVendor.Where(e => !string.IsNullOrEmpty(e.CusVenID) && e.CusVenID.Substring(0,1) == "D").ToListAsync();

            //取Ascend-Cus
            list_WD2SU01 = _contextAscend.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && (item.SU01001.StartsWith("A") || item.SU01001.StartsWith("B")))
                .ToList();

            if(list_WD2SU01.Count > 0)
            {
                foreach(var item in list_WD2SU01)
                {
                    CusReportViewModel modelVM = new CusReportViewModel()
                    {
                        DBSource = "Ascend",
                        CusID = item.SU01001,
                        CusName = item.SU01003
                    };

                    bool exists = list_CusVendorSuspend.Any(e => e.DBSource == modelVM.DBSource && e.CusVenCode == modelVM.CusID);
                    if (!exists)
                    {
                        list_AllCus.Add(modelVM);
                    }
                    else
                    {
                        continue;
                    }
                }
                list_WD2SU01.Clear();
            }

            //取ATI-Cus
            list_WD2SU01 = _contextATI.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("CAM"))
                .ToList();

            if (list_WD2SU01.Count > 0)
            {
                foreach (var item in list_WD2SU01)
                {
                    CusReportViewModel modelVM = new CusReportViewModel()
                    {
                        DBSource = "ATI",
                        CusID = item.SU01001,
                        CusName = item.SU01003
                    };

                    bool exists = list_CusVendorSuspend.Any(e => e.DBSource == modelVM.DBSource && e.CusVenCode == modelVM.CusID);
                    if (!exists)
                    {
                        list_AllCus.Add(modelVM);
                    }
                    else
                    {
                        continue;
                    }
                }
                list_WD2SU01.Clear();
            }

            //取Kirin-Cus
            list_WD2SU01 = _contextKIR1N.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("CKI"))
                .ToList();

            if (list_WD2SU01.Count > 0)
            {
                foreach (var item in list_WD2SU01)
                {
                    CusReportViewModel modelVM = new CusReportViewModel()
                    {
                        DBSource = "Kirin",
                        CusID = item.SU01001,
                        CusName = item.SU01003
                    };

                    bool exists = list_CusVendorSuspend.Any(e => e.DBSource == modelVM.DBSource && e.CusVenCode == modelVM.CusID);
                    if (!exists)
                    {
                        list_AllCus.Add(modelVM);
                    }
                    else
                    {
                        continue;
                    }
                }
                list_WD2SU01.Clear();
            }

            //取Intertek-Cus
            list_WD2SU01 = _contextINTERTEK.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("CIT"))
                .ToList();

            if (list_WD2SU01.Count > 0)
            {
                foreach (var item in list_WD2SU01)
                {
                    CusReportViewModel modelVM = new CusReportViewModel()
                    {
                        DBSource = "Intertek",
                        CusID = item.SU01001,
                        CusName = item.SU01003
                    };

                    bool exists = list_CusVendorSuspend.Any(e => e.DBSource == modelVM.DBSource && e.CusVenCode == modelVM.CusID);
                    if (!exists)
                    {
                        list_AllCus.Add(modelVM);
                    }
                    else
                    {
                        continue;
                    }
                }
                list_WD2SU01.Clear();
            }

            //取TestB-Cus
            list_WD2SU01 = _contextKIR1N.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "2") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("T"))
                .ToList();

            if (list_WD2SU01.Count > 0)
            {
                foreach (var item in list_WD2SU01)
                {
                    CusReportViewModel modelVM = new CusReportViewModel()
                    {
                        DBSource = "TestB",
                        CusID = item.SU01001,
                        CusName = item.SU01003
                    };

                    bool exists = list_CusVendorSuspend.Any(e => e.DBSource == modelVM.DBSource && e.CusVenCode == modelVM.CusID);
                    if (!exists)
                    {
                        list_AllCus.Add(modelVM);
                    }
                    else
                    {
                        continue;
                    }
                }
                list_WD2SU01.Clear();
            }

            //取Auto-Cus
            list_CusVendor = _cusVendorContext.CusVendor.Where(e => e.IsUse == "Y" && !string.IsNullOrEmpty(e.CusVenID) && e.CusVenID.StartsWith("C")).ToList();

            if (list_CusVendor.Count > 0)
            {
                foreach (var item in list_CusVendor)
                {
                    CusReportViewModel modelVM = new CusReportViewModel()
                    {
                        DBSource = "Auto",
                        CusID = item.CusVenID,
                        CusName = item.CusVenName
                    };
                    list_AllCus.Add(modelVM);
                }
                list_CusVendor.Clear();
            }

            return list_AllCus;
        }
        public async Task<List<CusReportViewModel>> GetAllVendor()
        {
            List<CusReportViewModel> list_AllVendor = new List<CusReportViewModel>();
            List<WD2SU01> list_WD2SU01 = new List<WD2SU01>();
            List<CusVendor> list_CusVendor = new List<CusVendor>();
            List<CusVendor> list_CusVendorSuspend = new List<CusVendor>();

            list_CusVendorSuspend = await _cusVendorContext.CusVendor.Where(e => !string.IsNullOrEmpty(e.CusVenID) && e.CusVenID.Substring(0, 1) == "D").ToListAsync();

            //取Ascend-Cus
            list_WD2SU01 = _contextAscend.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("V"))
                .ToList();

            if (list_WD2SU01.Count > 0)
            {
                foreach (var item in list_WD2SU01)
                {
                    CusReportViewModel modelVM = new CusReportViewModel()
                    {
                        DBSource = "Ascend",
                        CusID = item.SU01001,
                        CusName = item.SU01003
                    };

                    bool exists = list_CusVendorSuspend.Any(e => e.DBSource == modelVM.DBSource && e.CusVenCode == modelVM.CusID);
                    if (!exists)
                    {
                        list_AllVendor.Add(modelVM);
                    }
                    else
                    {
                        continue;
                    }
                }
                list_WD2SU01.Clear();
            }

            //取Intertek-Cus
            list_WD2SU01 = _contextINTERTEK.WD2SU01
                .Where(item => (item.SU01002 == "0" || item.SU01002 == "1") && !string.IsNullOrEmpty(item.SU01001) && item.SU01001.StartsWith("VIT"))
                .ToList();

            if (list_WD2SU01.Count > 0)
            {
                foreach (var item in list_WD2SU01)
                {
                    CusReportViewModel modelVM = new CusReportViewModel()
                    {
                        DBSource = "Intertek",
                        CusID = item.SU01001,
                        CusName = item.SU01003
                    };

                    bool exists = list_CusVendorSuspend.Any(e => e.DBSource == modelVM.DBSource && e.CusVenCode == modelVM.CusID);
                    if (!exists)
                    {
                        list_AllVendor.Add(modelVM);
                    }
                    else
                    {
                        continue;
                    }
                }
                list_WD2SU01.Clear();
            }

            //取Auto-Cus
            list_CusVendor = _cusVendorContext.CusVendor.Where(e => e.IsUse == "Y" && !string.IsNullOrEmpty(e.CusVenID) && e.CusVenID.StartsWith("V")).ToList();

            if (list_CusVendor.Count > 0)
            {
                foreach (var item in list_CusVendor)
                {
                    CusReportViewModel modelVM = new CusReportViewModel()
                    {
                        DBSource = "Auto",
                        CusID = item.CusVenID,
                        CusName = item.CusVenName
                    };
                    list_AllVendor.Add(modelVM);
                }
                list_CusVendor.Clear();
            }

            return list_AllVendor;
        }

        public async Task <List<SelectListItem>> GetAllCus_Selector()
        {
            var model = await GetAllCus();

            return model.Select(e => new SelectListItem
            {
                Value = e.CusID,
                Text = e.CusName
            }).ToList();
        }
        public async Task<List<SelectListItem>> GetAllVendor_Selector()
        {
            var model = await GetAllVendor();

            return model.Select(e => new SelectListItem
            {
                Value = e.CusID,
                Text = e.CusName
            }).ToList();
        }

        public async Task<List<CusReportViewModel>> GetAutoCusVen(string type)
        {
            if(type == "C")
            {
                return await GetAllCus();                               
            }
            else
            {
                return await GetAllVendor();          
            }                            
        }
        public List<CusVendor> GetAutoVen()
        {
            return _cusVendorContext.CusVendor.Where(e => e.IsUse == "Y" && !string.IsNullOrEmpty(e.CusVenName) && e.CusVenName.StartsWith("V")).ToList();
        }

        public List<SelectListItem> GetCusVenType
        {
            get
            {
                List<SelectListItem> CusVenType = new List<SelectListItem>
        {
            new SelectListItem {Text = "客戶" , Value = "C"},
            new SelectListItem {Text = "供應商" , Value = "V"},
        };
                return CusVenType;
            }
        }

        public List<SelectListItem> GetDBSource
        {
            get
            {
                List<SelectListItem> Region = new List<SelectListItem>
        {
            new SelectListItem {Text = "Ascend" , Value = "Ascend"},
            new SelectListItem {Text = "ATI" , Value = "ATI"},
            new SelectListItem {Text = "Kirin" , Value = "Kirin"},
            new SelectListItem {Text = "TestB" , Value = "TestB"},
            new SelectListItem {Text = "Auto" , Value = "Auto"}
        };
                return Region;
            }
        }

        public async Task EditAutoCusVen(CusVendor model)
        {
            if(model != null)
            {
                if(!string.IsNullOrEmpty(model.DBSource) && model.DBSource != "Auto")
                {
                    model.IsUse = "N";
                }
                
                model.UpdateDate = DateTime.Now;
                
                _cusVendorContext.Update(model);
                await _cusVendorContext.SaveChangesAsync();
            }
        }

        public async Task SuspendCusVen(CusVendor model)
        {
            if (model != null)
            {
                model.CusVenID = GetSuspendCusVenID();
                model.IsUse = "N";
                model.UpdateDate = DateTime.Now;

                _cusVendorContext.Add(model);
                await _cusVendorContext.SaveChangesAsync();
            }
        }

        public string GetSuspendCusVenID()
        {
            var CusVenID = _cusVendorContext.CusVendor.Where(e => !string.IsNullOrEmpty(e.CusVenID) && e.CusVenID.Substring(0,1) == "D").Select(e => e.CusVenID).ToList();

            if (CusVenID.Count == 0)
            {
                return "D001";
            }
            else
            {
                var maxCusVenID = CusVenID.Max();
                int maxID;

                if (!string.IsNullOrEmpty(maxCusVenID) && int.TryParse(maxCusVenID.Substring(1), out maxID))
                {
                    return "D" + (maxID + 1).ToString().PadLeft(4, '0');
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public async Task<List<CusReportViewModel>> GetSuspendList()
        {
            List<CusReportViewModel> list_CusVendorSuspend = new List<CusReportViewModel>();
            List<CusVendor> list_CusVendor = new List<CusVendor>();

            list_CusVendor = await _cusVendorContext.CusVendor.Where(e => !string.IsNullOrEmpty(e.IsUse) && e.IsUse == "N").
                ToListAsync();

            foreach(var item in list_CusVendor)
            {
                var model = new CusReportViewModel()
                {
                    DBSource = item.DBSource,
                    CusID = item.CusVenCode,
                    CusName = item.CusVenName
                };
                list_CusVendorSuspend.Add(model);
            }

            return list_CusVendorSuspend;
        }

        public async Task<string> GetCusDBName(string CusID)
        {
            var model = await GetAllCus();

            var DBSource = model.Where(e => e.CusID == CusID).Select(e => e.DBSource).FirstOrDefault();
            if (!string.IsNullOrEmpty(DBSource))
            {
                return DBSource;
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<string> GetCusName(string CusID)
        {
            var model = await GetAllCus();

            var CusName = model.Where(e => e.CusID == CusID).Select(e => e.CusName).FirstOrDefault();
            if (!string.IsNullOrEmpty(CusName))
            {
                return CusName;
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetVendorName(string CusID)
        {
            string CusName = string.Empty;

            if(string.IsNullOrEmpty(CusID))
            {
                return CusName;
            }
            
            //搜尋Ascend 資料庫
            CusName = _contextAscend.WD2SU01.Where(e => e.SU01001 == CusID).Select(e => e.SU01003).FirstOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(CusName))
            {
                //搜尋BizAuto資料庫
                CusName  = _cusVendorContext.CusVendor.Where(e => e.CusVenID == CusID).Select(e => e.CusVenName).FirstOrDefault() ?? string.Empty;
            }

            return CusName;

        }

        public void ImportCusVen(List<CusUploadViewModel> list_CusViewModels)
        {
            if(list_CusViewModels.Count == 0)
            {
                return;
            }

            foreach(var item in list_CusViewModels)
            {
                string DBSource = string.Empty;

                if (!string.IsNullOrEmpty(item.CusID))
                {
                    DBSource = GetDBSourcebyCusID(item.CusID);
                }

                switch (item.CusStatus)
                {
                    //使用
                    case "Y":
                        if (DBSource == "Auto")
                        {
                            var OriModel = _cusVendorContext.CusVendor.Where(e => !string.IsNullOrEmpty(e.CusVenID) && e.CusVenID.StartsWith("C") &&
                                           e.CusVenID == item.CusID && string.IsNullOrEmpty(e.CusVenCode) && 
                                           !string.IsNullOrEmpty(e.IsUse) && e.IsUse == "N").FirstOrDefault();
                            //檢查是否存在且未對應到文中資料庫且已被停用
                            if (OriModel != null)
                            {
                                OriModel.IsUse = "Y";
                                OriModel.UpdateDate = DateTime.Now;

                                _cusVendorContext.Update(OriModel);
                                _cusVendorContext.SaveChanges();
                            }
                        }
                        else
                        {
                            var OriModel = _cusVendorContext.CusVendor.Where(e => !string.IsNullOrEmpty(e.CusVenID) && e.CusVenID.StartsWith("D") &&
                                           e.CusVenCode == item.CusID).FirstOrDefault();

                            if(OriModel != null)
                            {
                                _cusVendorContext.Remove(OriModel);
                                _cusVendorContext.SaveChanges();
                            }
                        }
                        break;
                    //停用客戶/供應商
                    case "N":
                        if(DBSource == "Auto")
                        {
                            var OriModel = _cusVendorContext.CusVendor.Where(e => !string.IsNullOrEmpty(e.CusVenID) && e.CusVenID.StartsWith("C") &&
                                        e.CusVenID == item.CusID &&!string.IsNullOrEmpty(e.IsUse) && e.IsUse != "N").FirstOrDefault();
                            //檢查是否存在且尚未被停用
                            if(OriModel != null)
                            {
                                OriModel.IsUse = "N";
                                OriModel.UpdateDate = DateTime.Now;

                                _cusVendorContext.Update(OriModel);
                                _cusVendorContext.SaveChanges();
                            }
                        }
                        else
                        {
                            //檢查是否已經停用
                            var ChkModel = _cusVendorContext.CusVendor.Where(e => !string.IsNullOrEmpty(e.CusVenID) && e.CusVenID.StartsWith("D") &&
                                        e.CusVenCode == item.CusID).FirstOrDefault();
                            //尚未被停用
                            if (ChkModel == null)
                            {
                                var model = new CusVendor()
                                {
                                    CusVenID = GetSuspendCusVenID(),
                                    CusVenName = item.CusName,
                                    DBSource = DBSource,
                                    CusVenCode = item.CusID,
                                    IsUse = "N",
                                    UpdateDate = DateTime.Now
                                };
                                _cusVendorContext.Add(model);
                                _cusVendorContext.SaveChanges();
                            }
                        }
                        break;
                    //DBSource = Auto, 修改客戶名稱
                    case "E":
                        if (DBSource == "Auto")
                        {
                            var OriModel = _cusVendorContext.CusVendor.Where(e => !string.IsNullOrEmpty(e.CusVenID) && e.CusVenID.StartsWith("C") &&
                                           e.CusVenID == item.CusID).FirstOrDefault();
                            //檢查是否存在且尚未被停用
                            if (OriModel != null && OriModel.IsUse != "N")
                            {
                                OriModel.CusVenName = item.CusName;
                                OriModel.UpdateDate = DateTime.Now;

                                _cusVendorContext.Update(OriModel);
                                _cusVendorContext.SaveChanges();
                            }
                        }
                        break;
                    //DBSource = Auto, 停用Auto的資料,並寫入對應的文中DBSource及CusID
                    case "C":
                        if (DBSource == "Auto")
                        {
                            var OriModel = _cusVendorContext.CusVendor.Where(e => !string.IsNullOrEmpty(e.CusVenID) && e.CusVenID.StartsWith("C") &&
                                           e.CusVenID == item.CusID).FirstOrDefault();
                            //檢查是否存在且尚未被停用
                            if (OriModel != null)
                            {
                                if (!string.IsNullOrEmpty(item.ERPCusID))
                                {
                                    OriModel.DBSource = GetDBSourcebyCusID(item.ERPCusID);
                                    OriModel.CusVenCode = item.ERPCusID;
                                    OriModel.IsUse = "N";
                                    OriModel.UpdateDate = DateTime.Now;

                                    _cusVendorContext.Update(OriModel);
                                    _cusVendorContext.SaveChanges();
                                }             
                            }
                        }
                        break;
                }
            }

        }
        private string GetDBSourcebyCusID(string CusID)
        {
            if (string.IsNullOrEmpty(CusID))
            {
                return string.Empty;
            }
            else
            {
                switch (CusID.Substring(0, 1))
                {
                    case "A":
                        return "Ascend";
                    case "B":
                        return "Ascend";
                    case "C":
                        if(CusID.Length >= 3)
                        {
                            switch (CusID.Substring(0, 3))
                            {
                                case "CAM":
                                    return "ATI";
                                case "CKI":
                                    return "Kirin";
                                case "CIT":
                                    return "Intertek";
                            }
                            // 檢查第二位或第三位是否為數字
                            if (char.IsDigit(CusID[1]) && char.IsDigit(CusID[2]))
                            {
                                return "Auto";
                            }
                        }
                        return string.Empty;
                    case "T":
                        return "TestB";
                    default: 
                        return string.Empty;
                }               
            }
        }
    }
}