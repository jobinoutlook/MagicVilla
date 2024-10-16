﻿using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
       
       
        private int _pageSize = 10;
        public VillaController(IVillaService villaService,IConfiguration configuration)
        {
            _villaService = villaService;

            _pageSize = configuration.GetValue<int>("PageSize:Value");


        }

        //[ResponseCache(Duration = 30)]
        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDTO> lstVilladto = new();

            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                lstVilladto = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));

            }

            return View(lstVilladto);
        }

        //[ResponseCache(Duration = 30)]
        public async Task<IActionResult> IndexVillaPaged(PaginationDTO dto)
        {


            APIResponse response = await _villaService.GetAllAsync<APIResponse>(dto);
            if (response != null && response.IsSuccess)
            {
                List<VillaDTO> lst = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
                var queryable = lst.AsQueryable();

                var paginatedResult = await PaginatedList<VillaDTO>.CreateAsync(queryable, dto.PageNumber ?? 1, _pageSize);

                return View(paginatedResult);

            }


            return NotFound();

        }



        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> CreateVilla()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVilla(VillaCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.CreateAsync<APIResponse>(model);
                if(response != null && response.IsSuccess)
                {
                   return RedirectToAction(nameof(IndexVillaPaged));
                }

                if(response != null)
                {
                    if (response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                    }
                }
                
            }
            
            return View(model);
        }



        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> UpdateVilla(int villaId)
        {
            VillaUpdateDTO villaDTO = new();
            var response = await _villaService.GetAsync<APIResponse>(villaId);
            if (response != null && response.IsSuccess)
            {
                villaDTO = JsonConvert.DeserializeObject<VillaUpdateDTO>(Convert.ToString(response.Result));

                return View(villaDTO);
            }

            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVilla(VillaUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.UpdateAsync<APIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                   return RedirectToAction(nameof(IndexVillaPaged));
                }
                else
                {
                    ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
                }

            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteVilla(int villaId)
        {
            VillaDTO? villaDTO = new();
            var response = await _villaService.GetAsync<APIResponse>(villaId);
            if (response != null && response.IsSuccess)
            {
                villaDTO = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));

                return View(villaDTO);
            }

            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVilla(VillaDTO model)
        {
            
                var response = await _villaService.DeleteAsync<APIResponse>(model.Id);
                if (response != null && response.IsSuccess)
                {
                   return RedirectToAction(nameof(IndexVillaPaged));
                }

           
            return View(model);
        }

    }
}
