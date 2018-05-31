using System.Collections.Generic;
using Math2.Enums;

namespace Math2.Models
{
    public class Game
    {
        public EnumGameStatus Status { get; set; }

        public string StatusText { get; set; }

        public Question Question { get; set; }

        public List<Player> Players { get; set; }

        public Game()
        {
            Question = new Question();
            Players = new List<Player>();
        }
    }
}