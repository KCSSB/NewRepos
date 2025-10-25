﻿using System.Net;
using System.Security.Claims;
using API.Constants;
using API.Exceptions;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using Microsoft.IdentityModel.Tokens;


namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var _errCreator = new ErrorContextCreator(typeof(ClaimsPrincipalExtensions).Name);
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdString == null)
                throw new AppException(_errCreator.NotFound("Ошибка при получении Claim NameIdentifier из User"));

            if (!int.TryParse(userIdString, out int userId))
                throw new AppException(_errCreator.BadRequest($"Ошибка при приведении userId {userIdString} к формату int"));

            return userId;
        }
        public static string GetDeviceId(this ClaimsPrincipal user)
        {
            string? deviceId = null;
            if (user!=null)
                deviceId = user.FindFirstValue("DeviceId");
            return deviceId;
        }
    }
}
