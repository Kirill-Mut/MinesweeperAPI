namespace MinesweeperAPI.Models
{
    public class GameTurnRequest
    {
        public Guid GameId { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }
}