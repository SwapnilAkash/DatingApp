using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class ClaimsPrincpleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user){
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static int GetUserId(this ClaimsPrincipal user){
            return Convert.ToInt32(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}