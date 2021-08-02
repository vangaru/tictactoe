using System.ComponentModel.DataAnnotations;

namespace TicTacToeOnlineGame.Models
{
    public class CreateRoomModel
    {
        [Required(ErrorMessage = "Name of the game not set")]
        [StringLength(30, ErrorMessage = "Name is too long")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Tags not set")]
        [StringLength(30, ErrorMessage = "String is too long")]
        [Range(typeof(string), "a", "z", ErrorMessage = "Tags can contain only lowercase letters")]
        public string Tags { get; set; }
        
        [Required(ErrorMessage = "You must make first move")]
        public string FirstMoveCellId { get; set; }
    }
}