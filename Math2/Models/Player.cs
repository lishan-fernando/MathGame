using System.Collections.Generic;
using System.Net.WebSockets;
using Newtonsoft.Json;

namespace Math2.Models
{
    public class Player
    {
        public string Name { get; set; }

        public int Points { get; set; }

        public List<Answer> Answers { get; set; }

        [JsonIgnore]
        public WebSocket playerSoket { get; set; }

        public Player()
        {
            Answers = new List<Answer>();
        }
    }
}