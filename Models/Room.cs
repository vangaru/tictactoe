using System.Collections.Generic;

namespace TicTacToeOnlineGame.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int FirstMoveCellId { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public string Status { get; set; } = "Open";
    }
}