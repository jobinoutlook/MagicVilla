﻿using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using static MagicVilla_Utility.SD;
using System;

namespace MagicVilla_Web.Services
{
    public class VillaService : BaseService, IVillaService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string _villaUrl;

        public VillaService(IHttpClientFactory clientFactory,IConfiguration configuration ) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            _villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> CreateAsync<T>(VillaCreateDTO dto)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.POST,
                Data = dto,
                Url = _villaUrl + "/api/VillaAPI"
            });
        }

        public Task<T> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.DELETE,
                
                Url = _villaUrl + "/api/VillaAPI/" + id
            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.GET,

                Url = _villaUrl + "/api/VillaAPI/"
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.GET,

                Url = _villaUrl + "/api/VillaAPI/" + id
            });
        }

        public Task<T> UpdateAsync<T>(VillaUpdateDTO dto)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.PUT,
                Data = dto,
                Url = _villaUrl + "/api/VillaAPI/"+ dto.Id
            });
        }
    }
}
