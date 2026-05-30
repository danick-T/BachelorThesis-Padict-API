using CoachAssist.Api.Authorization;
using CoachAssist.Api.Middleware;
using CoachAssist.Core.DTOs.Terrein;
using CoachAssist.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TerreinEntity = CoachAssist.Core.Entities.Terrein;

namespace CoachAssist.Api.Controllers;

[ApiController]
[Route("api/terreinen")]
public class TerreinsController : ControllerBase
{
    private readonly ITerreinRepository _terreinen;
    private readonly IClubRepository _clubs;

    public TerreinsController(ITerreinRepository terreinen, IClubRepository clubs)
    {
        _terreinen = terreinen;
        _clubs = clubs;
    }

    [Authorize(Policy = Policies.AdminOfCoach)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TerreinResponse>>> Lijst([FromQuery] int? clubId)
    {
        var terreinen = clubId.HasValue
            ? await _terreinen.GetByClubIdAsync(clubId.Value)
            : await _terreinen.GetAllAsync();

        var response = terreinen.Select(t => new TerreinResponse
        {
            TerreinId = t.TerreinId,
            Naam = t.Naam,
            Adres = t.Adres,
            ClubId = t.ClubId,
        });
        return Ok(response);
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPost]
    public async Task<ActionResult<TerreinResponse>> Maak([FromBody] MaakTerreinRequest req)
    {
        if (!await _clubs.ExistsAsync(req.ClubId))
            throw new NietGevondenException($"Club met id {req.ClubId} bestaat niet.");

        if (await _terreinen.BestaatNaamInClubAsync(req.Naam, req.ClubId))
            return Conflict(new { melding = "Er bestaat al een terrein met deze naam binnen deze club." });

        var terrein = new TerreinEntity
        {
            Naam = req.Naam,
            Adres = req.Adres,
            ClubId = req.ClubId,
        };
        await _terreinen.AddAsync(terrein);
        await _terreinen.SaveChangesAsync();

        var response = new TerreinResponse
        {
            TerreinId = terrein.TerreinId,
            Naam = terrein.Naam,
            Adres = terrein.Adres,
            ClubId = terrein.ClubId,
        };
        return CreatedAtAction(nameof(Lijst), new { clubId = terrein.ClubId }, response);
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TerreinResponse>> Update(int id, [FromBody] WerkTerreinBijRequest req)
    {
        var terrein = await _terreinen.GetByIdAsync(id);
        if (terrein is null)
            throw new NietGevondenException($"Terrein met id {id} bestaat niet.");

        if (!string.Equals(terrein.Naam, req.Naam, StringComparison.OrdinalIgnoreCase)
            && await _terreinen.BestaatNaamInClubAsync(req.Naam, terrein.ClubId))
        {
            return Conflict(new { melding = "Er bestaat al een terrein met deze naam binnen deze club." });
        }

        terrein.Naam = req.Naam;
        terrein.Adres = req.Adres;
        _terreinen.Update(terrein);
        await _terreinen.SaveChangesAsync();

        return Ok(new TerreinResponse
        {
            TerreinId = terrein.TerreinId,
            Naam = terrein.Naam,
            Adres = terrein.Adres,
            ClubId = terrein.ClubId,
        });
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Verwijder(int id)
    {
        var terrein = await _terreinen.GetByIdAsync(id);
        if (terrein is null)
            throw new NietGevondenException($"Terrein met id {id} bestaat niet.");

        // Hard delete is alleen veilig wanneer geen wedstrijden/trainingen ernaar verwijzen,
        // want de DB-relaties zijn op Restrict gezet.
        if (terrein.Wedstrijden.Any() || terrein.Trainingen.Any())
        {
            return Conflict(new { melding = "Terrein kan niet verwijderd worden zolang er wedstrijden of trainingen aan gekoppeld zijn." });
        }

        _terreinen.Remove(terrein);
        await _terreinen.SaveChangesAsync();
        return NoContent();
    }
}
