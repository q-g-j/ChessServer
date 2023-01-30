using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ChessServer.Models;
using System.Linq;
using System.Numerics;

namespace ChessServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Players : ControllerBase
    {
        private readonly ChessDBContext _dbContext;

        public Players(ChessDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<List<Player>> GetAllPlayers()
        {
            return await _dbContext.Players.ToListAsync(); ;
        }

        [HttpPost]
        public async Task<ActionResult> PostNewPlayer(Player player)
        {
            var playerInDb = await _dbContext.Players.Where(a => a.Name == player.Name).FirstOrDefaultAsync();
            if (playerInDb == null)
            {
                await _dbContext.Players.AddAsync(player);
                await _dbContext.SaveChangesAsync();

                playerInDb = await _dbContext.Players.Where(a => a.Name == player.Name).FirstOrDefaultAsync();

                if (playerInDb != null)
                {
                    return Ok(playerInDb);
                }
            }
            return Conflict("error_nameconflict");
        }

        [HttpPut("{playerId}")]
        public async Task<ActionResult> PutResetInactiveCounter(int playerId)
        {
            var playerInDb = await _dbContext.Players.Where(a => a.Id == playerId).FirstOrDefaultAsync();

            if (playerInDb != null)
            {
                playerInDb.InactiveCounter = 0;
                await _dbContext.SaveChangesAsync();
                return Ok();
            }

            return NotFound("error_resetcounterfailed");
        }

        [HttpDelete("{playerId}")]
        public async Task<ActionResult> DeletePlayer(int playerId)
        {
            var playerInDb = _dbContext.Players.Where(a => a.Id == playerId).FirstOrDefault();

            if (playerInDb != null)
            {
                _dbContext.Invitations.RemoveRange(_dbContext.Invitations.Where(a => a.PlayerId == playerInDb.Id));
                _dbContext.Players.Remove(playerInDb);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }

            return NotFound("error_resetcounterfailed");
        }
    }
}
