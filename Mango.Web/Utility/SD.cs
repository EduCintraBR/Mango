﻿namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public static string ProductAPIBase { get; set; }
        public static string ShoppingCartAPIBase { get; set; }
        public static string OrderAPIBase { get; set; }

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "JWTToken";
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            PATCH,
            DELETE
        }

        public const string Status_Pending = "Pendente";
        public const string Status_Approved = "Aprovado";
        public const string Status_ReadyForPickup = "Pronto para coleta";
        public const string Status_Completed = "Concluído";
        public const string Status_Refunded = "Reembolsado";
        public const string Status_Cancelled = "Cancelado";

        public enum ContentType
        {
            Json,
            MultipartFormData
        }
    }
}
