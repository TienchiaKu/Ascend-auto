using System;
using Microsoft.AspNetCore.Mvc;
using DDD_2024.Data;
using DDD_2024.Models;
using DDD_2024.Interfaces;
using MiniExcelLibs;
using DDD_2024.Services;

namespace DDD_2024.Controllers
{
    public class CusVendorController : Controller
    {
        private readonly ICusVendorService _cusVendoeService;
        private readonly CusVendorContext _cusVendorContext;

        public CusVendorController(ICusVendorService cusVendoeService, CusVendorContext cusVendorContext)
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

        [HttpGet]
        public async Task<IActionResult> AllCus_Selector()
        {
            var selectListItems_Cus = await _cusVendoeService.GetAllCus_Selector();

            return Content("");
        }

        // GET: CusVendor/Upload
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile Excelfile)
        {
            var stream = new MemoryStream();
            if (Excelfile != null)
            {
                Excelfile.CopyTo(stream);

                // 讀取stream中的所有資料
                var streamData = MiniExcel.Query(stream, true, startCell: "A1").ToList();

                // 檢查是否有資料
                if (streamData.Count > 0)
                {
                    List<CusUploadViewModel> list_CusViewModels = new List<CusUploadViewModel>();

                    for (int i = 0; i < streamData.Count; i++)
                    {
                        var rowData = streamData[i];

                        var cusModel = new CusUploadViewModel();

                        foreach (var cellValue in rowData)
                        {
                            if (cellValue.Key == "客戶代碼")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    cusModel.CusID = cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "客戶名稱")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    cusModel.CusName = cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "使用?")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    cusModel.CusStatus = cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "文中客戶代碼")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    cusModel.ERPCusID = cellValue.Value;
                                }
                            }
                        }
                        list_CusViewModels.Add(cusModel);
                    }
                    _cusVendoeService.ImportCusVen(list_CusViewModels);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}