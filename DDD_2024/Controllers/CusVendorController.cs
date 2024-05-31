using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DDD_2024.Data;
using DDD_2024.Models;
using DDD_2024.Interfaces;
using DDD_2024.Services;
using MiniExcelLibs;

namespace DDD_2024.Controllers
{
    public class CusVendorController : Controller
    {
        private readonly ICusVendoeService _cusVendoeService;
        private readonly CusVendorContext _cusVendorContext;

        public CusVendorController(ICusVendoeService cusVendoeService, CusVendorContext cusVendorContext)
        {
            _cusVendoeService = cusVendoeService;
            _cusVendorContext = cusVendorContext;
        }

        // GET: CusVendor
        public async Task<IActionResult> Index()
        {
            var model =  await _cusVendoeService.GetAllCus();

            if (model != null)
            {
                return View(model);
            }
            else
            {
                return Problem("客供商無資料");
            }
        }

        // GET: Do
        public async Task<IActionResult> IndexFilter([FromQuery] string type)
        {
            var model = await _cusVendoeService.GetAutoCusVen(type);

            if (model != null)
            {
                return PartialView("_CusVenPartial", model);
            }
            else
            {
                return Problem("Entity set 'CusVen' is null.");
            }
        }

        public async Task<IActionResult> CusVendorReport_Excel()
        {
            var cusModel = await _cusVendoeService.GetAllCus();
            var venModel = await _cusVendoeService.GetAllVendor();

            if (cusModel != null && venModel != null)
            {
                var sheets = new Dictionary<string, object>
                {
                    ["客戶"] = cusModel,
                    ["供應商"] = venModel
                };

                var memoryStream = new MemoryStream();
                memoryStream.SaveAs(sheets);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "客供商報表_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"
                };
            }
            else
            {
                return Content("匯出錯誤");
            }
        }

        // GET: CusVendor/Create
        public IActionResult Create(string type)
        {
            var model = new CusVendor();

            if (string.IsNullOrEmpty(type) )
            {
                return Content("");
            }
            else
            {
                if(type == "Cus")
                {
                    model.CusVenID = _cusVendoeService.GetNewCusID();
                }
                else if(type == "Ven")
                {
                    model.CusVenID = _cusVendoeService.GetNewVenID();
                }
                else
                {
                    return Content("");
                }
            }

            return View(model);
        }

        // GET: CusVendor/Edit
        [HttpGet]
        public IActionResult Edit(string DBSource, string CusID,string CusName)
        {
            var model = new CusVendor()
            {
                CusVenID = CusID,
                CusVenName = CusName,
                DBSource = DBSource
            };

            return View(model);
        }

        public IActionResult Suspend([Bind("DBSource,CusID,CusName")] CusReportViewModel cusReportViewModel)
        {
            var model = new CusVendor()
            {
                CusVenCode = cusReportViewModel.CusID,
                CusVenName = cusReportViewModel.CusName,
                DBSource = cusReportViewModel.DBSource
            };

            _cusVendoeService.SuspendCusVen(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: Do
        public async Task<IActionResult> Recovery()
        {
            var model = await _cusVendoeService.GetSuspendList();

            if (model != null && model.Count > 0)
            {
                return PartialView("_CusVenPartial", model);
            }
            else
            {
                return Problem("Entity set 'Suspend CusVen' is null.");
            }
        }

        // POST: CusVendor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CusVenID,CusVenName,DBSource,CusVenCode,IsUse")] CusVendor cusVendor)
        {                      
            if (cusVendor == null)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                await _cusVendoeService.EditAutoCusVen(cusVendor);
                return RedirectToAction(nameof(Index));
            }
            
            return View();
        }


        // POST: CusVendor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CusVenID,CusVenName,IsUse")] CusVendor cusVendor)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(cusVendor.CusVenName))
                {
                    //檢查名稱是否重複
                    if (_cusVendoeService.CheckCusVenName(cusVendor.CusVenName))
                    {
                        ModelState.AddModelError(string.Empty, "客供商資料重複");
                    }
                    else
                    {
                        cusVendor.DBSource = "Auto";
                        cusVendor.UpdateDate = DateTime.Now;
                        _cusVendorContext.Add(cusVendor);
                        await _cusVendorContext.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }              
            }
            return View();
        }
    }
}