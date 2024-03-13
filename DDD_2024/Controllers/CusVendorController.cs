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

namespace DDD_2024.Controllers
{
    public class CusVendorController : Controller
    {
        private readonly ICusVendoeService _cusVendoeService;

        public CusVendorController(ICusVendoeService cusVendoeService)
        {
            _cusVendoeService = cusVendoeService;
        }

        // GET: CusVendor
        public async Task<IActionResult> Index()
        {
            var model =  await _cusVendoeService.vendorlistAscend();

            if (model != null)
            {
                return View(model);
            }
            else
            {
                return Problem("客供商無資料");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetASCENDvendor()
        {
            var data = await _cusVendoeService.vendorlistAscend();
            return PartialView("_VendorListPartial", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetATIvendor()
        {
            var data = await _cusVendoeService.vendorlistATI();
            return PartialView("_VendorListPartial", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetKIR1Nvendor()
        {
            var data = await _cusVendoeService.vendorlistKIR1N();
            return PartialView("_VendorListPartial", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetINTERTEKvendor()
        {
            var data = await _cusVendoeService.vendorlistINTERTEK();
            return PartialView("_VendorListPartial", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetTESTBvendor()
        {
            var data = await _cusVendoeService.vendorlistTESTB();
            return PartialView("_VendorListPartial", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetASCENDcustomer()
        {
            var data = await _cusVendoeService.cuslistAscend();
            return PartialView("_VendorListPartial", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetATIcustomer()
        {
            var data = await _cusVendoeService.cuslistATI();
            return PartialView("_VendorListPartial", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetKIR1NIcustomer()
        {
            var data = await _cusVendoeService.cuslistKIR1N();
            return PartialView("_VendorListPartial", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetINTERTEKcustomer()
        {
            var data = await _cusVendoeService.cuslistINTERTEK();

            return PartialView("_VendorListPartial", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetTESTBcustomer()
        {
            var data = await _cusVendoeService.cuslistTESTB();
            return PartialView("_VendorListPartial", data);
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> KIR1Nvendor_SelectList()
        {
            var vendors = await _cusVendoeService.vendorlistKIR1N();

            var selectList = vendors.Select(vendor => new SelectListItem
            {
                Value = vendor.SU01001,
                Text = vendor.SU01003
            }).ToList();

            return selectList;
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> TESTBvendor_SelectList()
        {
            var vendors = await _cusVendoeService.vendorlistTESTB();

            var selectList = vendors.Select(vendor => new SelectListItem
            {
                Value = vendor.SU01001,
                Text = vendor.SU01003
            }).ToList();

            return selectList;
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> ATIvendor_SelectList()
        {
            var vendors = await _cusVendoeService.vendorlistATI();

            var selectList = vendors.Select(vendor => new SelectListItem
            {
                Value = vendor.SU01001,
                Text = vendor.SU01003
            }).ToList();

            return selectList;
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> INTERTEKvendor_SelectList()
        {
            var vendors = await _cusVendoeService.vendorlistINTERTEK();

            var selectList = vendors.Select(vendor => new SelectListItem
            {
                Value = vendor.SU01001,
                Text = vendor.SU01003
            }).ToList();

            return selectList;
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> Ascendvendor_SelectList()
        {
            var vendors = await _cusVendoeService.vendorlistAscend();

            var selectList = vendors.Select(vendor => new SelectListItem
            {
                Value = vendor.SU01001,
                Text = vendor.SU01003
            }).ToList();

            return selectList;
        }

        [HttpGet]
        public List<SelectListItem> GetAscendvendor_SelectList()
        {
            var vendors = _cusVendoeService.GetAscendvendorList();

            return vendors;
        }
    }
}