using System.Security.Claims;

namespace FirstAPI.Extensions
{
    public static class ClaimsExtension
    {
        public static IEnumerable<Claim> GetClaims(this User user)
        {
            var result = new List<Claim>
        {
            new(ClaimTypes.Name, user.Id.ToString())
        };

            return result;
        }

        public static int GetUserId(this ClaimsPrincipal principal)
        {
            var claim = principal?.FindFirst(ClaimTypes.Name);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
