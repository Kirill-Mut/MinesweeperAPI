namespace MinesweeperAPI.Models
{
    public class GameInfoResponse
    {
        public Guid Game_id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Mines_count { get; set; }
        public bool Completed { get; set; }
        public char[][] Field { get; set; }
    }
}