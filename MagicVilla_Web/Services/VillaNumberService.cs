using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace MagicVilla_Web.Services
{
    public class VillaNumberService :  IVillaNumberService
    {
        private IHttpClientFactory _httpClientFactory;
        private string? _villaUrl;
        private readonly IBaseService _baseService;
        public VillaNumberService(IHttpClientFactory httpClient, IConfiguration configuration, IBaseService  baseService)
        {
            _httpClientFactory = httpClient;
            _villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
            _baseService = baseService;

        }

        public async Task<T> CreateAsync<T>(VillaNumberCreateDTO dto)
        {
            var apiRequest = new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.POST,
                Data = dto,
                Url = _villaUrl+ "/api/VillaNumberAPI"
                

            };

            //var baseService = new BaseService(_httpClientFactory);
            return await _baseService.SendAsync<T>(apiRequest);
        }
        

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.DELETE,

                Url = _villaUrl + "/api/VillaNumberAPI/" + id
                
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.GET,

                Url = _villaUrl + "/api/VillaNumberAPI/"
                
            });
        }

        public async Task<T> GetAllAsync<T>(int pageSize, int? pageNumber)
        {
            PaginationDTO dto = new PaginationDTO() { PageSize = pageSize, PageNumber = pageNumber };

            return await GetAllAsync<T>(dto);
        }
        public async Task<T> GetAllAsync<T>(PaginationDTO dto)
        {
            return await _baseService.SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.GET,

                Url = _villaUrl + "/api/VillaNumberAPI/?pageNumber="+ dto.PageNumber + "&pageSize="+dto.PageSize,

               // Data = new PaginationDTO { PageSize = pageSize, PageNumber = pageNumber }

            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.GET,

                Url = _villaUrl + "/api/VillaNumberAPI/" + id,
                
            });
        }

        public async Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto)
        {
            return await _baseService.SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.PUT,
                Data = dto,
                Url = _villaUrl + "/api/VillaNumberAPI/" + dto.VillaNo,
                
            });
        }
    }
}
