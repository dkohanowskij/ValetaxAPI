using FxNet.Test.Api.Data;
using FxNet.Test.Api.DTOs;
using FxNet.Test.Api.Models;
using FxNet.Test.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FxNet.Test.Api.Controllers;

[ApiController]
[Produces("application/json")]
public class PartnerController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtService _jwtService;

    public PartnerController(AppDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Saves user by unique code and returns JWT token required on all other requests.
    /// </summary>
    [HttpPost("api.user.partner.rememberMe")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenInfo), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<TokenInfo>> RememberMe([FromQuery] string code)
    {
        var partner = await _db.Partners.FirstOrDefaultAsync(p => p.Code == code);
        if (partner == null)
        {
            partner = new Partner
            {
                Code = code,
                Token = string.Empty, // Will be replaced with JWT
                CreatedAt = DateTime.UtcNow
            };
            _db.Partners.Add(partner);
            await _db.SaveChangesAsync();
        }

        // Generate JWT token
        var jwtToken = _jwtService.GenerateToken(partner.Id, partner.Code);

        // Update partner with new JWT token
        partner.Token = jwtToken;
        await _db.SaveChangesAsync();

        return Ok(new TokenInfo { Token = jwtToken });
    }
}
