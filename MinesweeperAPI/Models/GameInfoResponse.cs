namespace MinesweeperAPI.Models
{
    public class GameInfoResponse
    {
        public Guid GameId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int MinesCount { get; set; }
        public bool Completed { get; set; }
        public char[][] Field { get; set; }
    }
}