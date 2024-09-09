using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.Extensions.Configuration;

namespace MagicVilla_Web.Services
{
    public class VillaNumberService : BaseService, IVillaNumberService
    {
        private IHttpClientFactory _httpClientFactory;
        private string? _villaUrl;
        public VillaNumberService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClientFactory = httpClient;
            _villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> CreateAsync<T>(VillaNumberCreateDTO dto)
        {
            var apiRequest = new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.POST,
                Data = dto,
                Url = _villaUrl+ "/api/VillaNumberAPI"

            };

            var baseService = new BaseService(_httpClientFactory);
            return baseService.SendAsync<T>(apiRequest);
        }
        public Task<T> CreateAsync<T>(VillaCreateDTO dto)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.POST,
                Data = dto,
                Url = _villaUrl + "/api/VillaNumberAPI"
            });
        }

        public Task<T> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.DELETE,

                Url = _villaUrl + "/api/VillaNumberAPI/" + id
            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.GET,

                Url = _villaUrl + "/api/VillaNumberAPI/"
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.GET,

                Url = _villaUrl + "/api/VillaNumberAPI/" + id
            });
        }

        public Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.PUT,
                Data = dto,
                Url = _villaUrl + "/api/VillaNumberAPI/" + dto.VillaNo
            });
        }
    }
}
