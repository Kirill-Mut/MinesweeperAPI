using Microsoft.AspNetCore.Mvc;
using MinesweeperAPI.Models;
using MinesweeperAPI.Services;

namespace MinesweeperAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("new")]
        public IActionResult StartNewGame([FromBody] NewGameRequest request)
        {
            try
            {
                var response = _gameService.StartNewGame(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message });
            }
        }

        [HttpPost("turn")]
        public IActionResult MakeTurn([FromBody] GameTurnRequest request)
        {
            try
            {
                var response = _gameService.MakeTurn(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message });
            }
        }
    }
}