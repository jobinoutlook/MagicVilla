﻿using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.VM;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Reflection;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        private int _pageSize = 10;

        public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService, IMapper mapper,IConfiguration configuration)
        {
            _villaNumberService = villaNumberService;
            _villaService = villaService;
            _mapper = mapper;
            _pageSize = _pageSize = configuration.GetValue<int>("PageSize:Value");
        }

        //public async Task<IActionResult> IndexVillaNumber()
        //{
        //    List<VillaNumberDTO> lstVilladto = new();

        //    var response = await _villaNumberService.GetAllAsync<APIResponse>();
        //    if (response != null && response.IsSuccess)
        //    {
        //        lstVilladto = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
        //    }

        //    return View(lstVilladto);
        //}

        public async Task<IActionResult> IndexVillaNumberPaged(int? pageNumber)
        {


            APIResponse response = await _villaNumberService.GetAllAsync<APIResponse>(_pageSize, pageNumber ?? 1);
            if (response != null && response.IsSuccess)
            {
                List<VillaNumberDTO> lst = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
                var queryable = lst.AsQueryable();

                var paginatedResult = await PaginatedList<VillaNumberDTO>.CreateAsync(queryable, pageNumber ?? 1, _pageSize);

                return View(paginatedResult);

            }


            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> CreateVillaNumber()
        {
            VillaNumberCreateVM villaNumberVM = new();

            APIResponse response = await _villaService.GetAllAsync<APIResponse>();
//====================================================================================================
            //this is a hack
            //if(response!=null && response.IsSuccess && response.ErrorMessages.Count==0)
//====================================================================================================
            if(response!=null && response.IsSuccess)
            {
               var villaDTOs = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));

               villaNumberVM.VillaList = villaDTOs.Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });

            }

            return View(villaNumberVM);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumber);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumberPaged));
                }
                else
                {
                    TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0) ?
                        response.ErrorMessages[0] : "Error Encountered";
                }

            }

            APIResponse resp = await _villaService.GetAllAsync<APIResponse>();
            if(resp!=null && resp.IsSuccess)
            {
                var villaDTOs = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(resp.Result));
                model.VillaList = villaDTOs.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }


            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> UpdateVillaNumber(int villaNo)
        {
            VillaNumberUpdateVM villaNumberVM = new(); 
            var response = await _villaNumberService.GetAsync<APIResponse>(villaNo);

            if(response != null && response.IsSuccess)
            {
                VillaNumberDTO model  = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result)); 
                villaNumberVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model);
            }

            response = await _villaService.GetAllAsync<APIResponse>();
            if(response != null && response.IsSuccess)
            {
                var villaDTOs = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
                villaNumberVM.VillaList = villaDTOs.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });

                return View(villaNumberVM);
            }
            else
            {
                TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0) ?
                    response.ErrorMessages[0] : "Error Encountered";
            }

            return NotFound();

        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber);
                if(response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumberPaged));
                }
                else
                {
                    TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0) ?
                        response.ErrorMessages[0] : "Error Encountered";
                }
            }


            APIResponse resp = await _villaService.GetAllAsync<APIResponse>();
            if (resp != null && resp.IsSuccess)
            {
                var villaDTOs = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(resp.Result));
                model.VillaList = villaDTOs.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }


            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteVillaNumber(int villaNo)
        {
            VillaNumberDeleteVM villaNumberVM = new();
            var response = await _villaNumberService.GetAsync<APIResponse>(villaNo);

            if (response != null && response.IsSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                villaNumberVM.VillaNumber = _mapper.Map<VillaNumberDTO>(model);
            }

            response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                var villaDTOs = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
                villaNumberVM.VillaList = villaDTOs.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });

                return View(villaNumberVM);
            }

            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
        {
            var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumber.VillaNo);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexVillaNumberPaged));
            }
            else
            {
                TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0) ?
                    response.ErrorMessages[0] : "Error Encountered";
            }
            return View(model);
        }

    }
}
