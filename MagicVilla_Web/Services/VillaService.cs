using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using static MagicVilla_Utility.SD;
using System;

namespace MagicVilla_Web.Services
{
    public class VillaService : IVillaService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string? _villaUrl;
        private readonly IBaseService _baseService;
        public VillaService(IHttpClientFactory clientFactory,IConfiguration configuration,IBaseService baseService ) 
        {
            _clientFactory = clientFactory;
            _villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
            _baseService = baseService;
        }

        public async Task<T> CreateAsync<T>(VillaCreateDTO dto)
        {
            return await _baseService.SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.POST,
                Data = dto,
                Url = _villaUrl + "/api/VillaAPI",
                
                ContentType = ContentType.MultipartFormData
            });
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.DELETE,
                
                Url = _villaUrl + "/api/VillaAPI/" + id,
                
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.GET,

                Url = _villaUrl + "/api/VillaAPI/",
                
            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.GET,

                Url = _villaUrl + "/api/VillaAPI/" + id,
                
            });
        }

        public async Task<T> UpdateAsync<T>(VillaUpdateDTO dto)
        {
            return await _baseService.SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.PUT,
                Data = dto,
                Url = _villaUrl + "/api/VillaAPI/"+ dto.Id,
                
                ContentType = ContentType.MultipartFormData
            });
        }
    }
}
