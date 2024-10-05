using AutoMapper;
using AutoMapper.Configuration.Annotations;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager
            , IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _config = configuration;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x=>x.UserName == username);
            if (user == null)
            {
                return true;
            }

            return false;
        }

        public async Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            //added by Jobin
          // RemoveInvalidRefreshTokens(user);


            bool IsValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (user == null || IsValid == false)
            {
                return new TokenDTO()
                {
                    AccessToken = ""
                };
            }

            var jwtTokenId = $"JITI{Guid.NewGuid()}";
            var accessToken = await GetAccessToken(user,jwtTokenId);
            var refreshToken = await CreateNewRefreshToken(user.Id, jwtTokenId);
            
            TokenDTO tokenDTO = new TokenDTO()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
                                
            };

            return tokenDTO;
        }

        private async void RemoveInvalidRefreshTokens(ApplicationUser user)
        {
            List<RefreshToken> invalidRefreshTokens = await _db.RefreshTokens.Where(u=>u.UserId == user.Id).ToListAsync();

            var invalidTokens = invalidRefreshTokens.Where(u => u.IsValid == false).ToArray();
            _db.RefreshTokens.RemoveRange(invalidTokens);


            //invalidRefreshTokens = await _db.RefreshTokens.Where(u => u.UserId == user.Id).ToListAsync();

            //if (invalidRefreshTokens.Count>1)
            //{

            //    for (int i = 0; i < invalidRefreshTokens.Count-1; i++)
            //    {
                    
            //        _db.RefreshTokens.Remove(invalidRefreshTokens[i]);
                    
            //    }

                
            //}


           
        }


        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            ApplicationUser user = new ()
            {
                UserName = registerationRequestDTO.UserName,
                Email = registerationRequestDTO.UserName,
                NormalizedEmail = registerationRequestDTO.Name.ToUpper(),
                Name = registerationRequestDTO.Name
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
                if(result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(registerationRequestDTO.Role).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(registerationRequestDTO.Role));
                    }
                    await _userManager.AddToRoleAsync(user, registerationRequestDTO.Role);
                    var userToReturn = _db.ApplicationUsers
                        .FirstOrDefault(u => u.UserName == registerationRequestDTO.UserName);
                    return _mapper.Map<UserDTO>(userToReturn);
                }
            }
            catch (Exception ex)
            {
            }

            return new UserDTO();

           
        }

        private async Task<string> GetAccessToken(ApplicationUser user,string jwtTokenId)
        {
            var roles = await _userManager.GetRolesAsync(user);
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(secretKey);

            ClaimsIdentity identity = new();

            foreach (var role in roles)
            {
                identity.AddClaims([
                    new Claim(ClaimTypes.Name,user.UserName.ToString()),
                    new Claim(ClaimTypes.Role,role),
                    new Claim(JwtRegisteredClaimNames.Jti,jwtTokenId),
                    new Claim(JwtRegisteredClaimNames.Sub,user.Id),
                    new Claim(JwtRegisteredClaimNames.Aud,_config.GetValue<string>("Audience:Name")),
                    new Claim(JwtRegisteredClaimNames.Iss,_config.GetValue<string>("Issuer:Name"))
                    ]);
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                //Subject = new ClaimsIdentity(new Claim[]
                //{
                //    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                //    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                //}),
                Expires = DateTime.UtcNow.AddDays(7),
                //Issuer = _config.GetValue<string>("Issuer:Name"),
                //Audience = _config.GetValue<string>("Audience:Name"),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenStr = tokenHandler.WriteToken(token);
            return tokenStr;
        }

        public async Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO)
        {
            //Find existing refresh token
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(u => u.Refresh_Token == tokenDTO.RefreshToken);
            if (existingRefreshToken == null)
            {
                return new TokenDTO();
            }
            //compare data from existing refresh and access token provided
            var isTokenValid = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            
            if (!isTokenValid)
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new TokenDTO();
            }

            //when someone tries to use not valid refresh token, fraud possible
            if (!existingRefreshToken.IsValid) 
            {
                await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
                return new TokenDTO();
            }
            
            //If just expired then mark as invalid and return empty
            if (existingRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                existingRefreshToken.IsValid = false;
                _db.SaveChanges();
                return new TokenDTO();

            }
            //replace old refresh with a new one with updated expire date
            var newRefreshToken = await CreateNewRefreshToken(existingRefreshToken.UserId,
                existingRefreshToken.JwtTokenId);

            //revoke existing refresh Token 
            await MarkTokenAsInvalid(existingRefreshToken);
            

            //generate new access token
            var applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == existingRefreshToken.UserId);
            if (applicationUser == null)
            {
                return new TokenDTO();
            }

            var newAccessToken = await GetAccessToken(applicationUser, existingRefreshToken.JwtTokenId);

            return new TokenDTO()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };



        }

        public async Task RevokeRefreshToken(TokenDTO tokenDTO)
        {
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(_ => _.Refresh_Token == tokenDTO.RefreshToken);

            if (existingRefreshToken == null) 
                return;

            var isTokenValid = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);

            if (!isTokenValid)
            {

                return;
            }

            await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
        
        
        }


        private async Task<string> CreateNewRefreshToken(string userId,string tokenId)
        {
            RefreshToken refreshToken = new()
            {
                IsValid = true,
                UserId = userId,
                JwtTokenId = tokenId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                Refresh_Token = Guid.NewGuid() + "-" + Guid.NewGuid()
            };

            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();
            return refreshToken.Refresh_Token;
        }
        private bool GetAccessTokenData(string accessToken, string expectedUserId, string expectedTokenId)
        {
            try
            {
                //var tokenhandler = new JwtSecurityTokenHandler();
                //var jwt = tokenhandler.CreateJwtSecurityToken(accessToken);
                //var jwtTokenId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Jti).Value;
                //var userId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value;
                //return userId == expectedUserId && jwtTokenId == expectedTokenId;

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(accessToken);
                var jwtTokenId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Jti).Value;
                var userId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value;
                return userId == expectedUserId && jwtTokenId == expectedTokenId;

            }
            catch
            {
                return false;
            }
        }
        private async Task MarkAllTokenInChainAsInvalid(string userId, string tokenId)
        {
            var chainRecords = _db.RefreshTokens.Where(u => u.UserId == userId
                && u.JwtTokenId == tokenId);

            foreach (var item in chainRecords)
            {
                item.IsValid = false;
            }
            _db.UpdateRange(chainRecords);
            _db.SaveChanges();
        }

        private Task MarkTokenAsInvalid(RefreshToken refreshToken)
        {
            refreshToken.IsValid = false;
            
            return _db.SaveChangesAsync();
        }

       
    }
}
