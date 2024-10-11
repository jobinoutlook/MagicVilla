﻿using System.Reflection.Metadata;

namespace MagicVilla_Utility
{
    public static class SD
    {
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE

        }

        public static string AccessToken = "JWTToken";
        public static string RefreshToken = "RefreshToken";
        public const string Admin = "admin";
        public const string Customer = "customer"; 
        public enum ContentType
        {
            Json,
            MultipartFormData
        }

        public static int PageSize { get; private set; } = 10;
    }
}
