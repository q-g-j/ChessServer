using System.Text.Json.Serialization;

namespace ChessServer.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int WhiteId { get; set; }
        public int BlackId { get; set; }
        public string? LastMoveStart { get; set; }
        public string? LastMoveEnd  { get; set; }
        public bool PawnMovedTwoSquares { get; set; } = false;
        public char PromotePawnType { get; set; }
        public bool IsCheckMate { get; set; } = false;
        public string? MoveInfo { get; set; }
        public bool HasPlayerQuit { get; set; } = false;
        [JsonIgnore]
        public int WhiteInactivityCounter { get; set; }
        [JsonIgnore]
        public int BlackInactivityCounter { get; set; }
    }
}
