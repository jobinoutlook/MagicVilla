
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private IHttpClientFactory _httpClientFactory;
        private string? _villaUrl;
        public AuthService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClientFactory = httpClient;
            _villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> LoginAsync<T>(LoginRequestDTO obj)
        {
            var apiRequest = new APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.POST,
                Data = obj,
                Url = _villaUrl + "/api/UsersAuth/login"

            };

           
            return SendAsync<T>(apiRequest);
        }

        public Task<T> RegisterAsync<T>(RegisterationRequestDTO obj)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = MagicVilla_Utility.SD.ApiType.POST,
                Data = obj,
                Url = _villaUrl + "/api/UsersAuth/register"
            });
        }
    }
}
