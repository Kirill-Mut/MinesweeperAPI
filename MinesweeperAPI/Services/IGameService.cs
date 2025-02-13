using MinesweeperAPI.Models;

namespace MinesweeperAPI.Services
{
    public interface IGameService
    {
        GameInfoResponse StartNewGame(NewGameRequest request);
        GameInfoResponse MakeTurn(GameTurnRequest request);
    }
}