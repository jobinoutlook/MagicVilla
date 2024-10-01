
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class AuthService : IAuthService
    {
        private IHttpClientFactory _httpClientFactory;
        private string? _villaUrl;
        private readonly IBaseService _baseService;
        public AuthService(IHttpClientFactory httpClient, IConfiguration configuration, IBaseService baseService)
        {
            _httpClientFactory = httpClient;
            _villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
            _baseService = baseService;
        }

        public async Task<T> LoginAsync<T>(LoginRequestDTO obj)
        {
            var apiRequest = new APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.POST,
                Data = obj,
                Url = _villaUrl + "/api/UsersAuth/login"

            };

           
            return await _baseService.SendAsync<T>(apiRequest,withBearer:false);
        }

        public async Task<T> RegisterAsync<T>(RegisterationRequestDTO obj)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.POST,
                Data = obj,
                Url = _villaUrl + "/api/UsersAuth/register"
            }, withBearer:false);
        }

        public async Task<T> LogoutAsync<T>(TokenDTO obj)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.POST,
                Data = obj,
                Url = _villaUrl + "/api/UsersAuth/revoke"
            });
        }
    }
}
