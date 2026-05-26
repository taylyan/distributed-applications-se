using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HuntingPermitTripManagement.Api.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected int? CurrentUserId
    {
        get
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
    }

    protected bool IsAdmin => User.IsInRole("Admin");
}