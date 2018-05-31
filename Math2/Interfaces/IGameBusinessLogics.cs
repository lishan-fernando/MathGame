#region usings

using System.Net.WebSockets;
using System.Threading.Tasks;
using Math2.Models;
#endregion

namespace Math2.Interfaces
{
    public interface IGameBusinessLogics
    {
        Game Game { get; set; }
        void AddPlayer(string player, WebSocket webSocket);
        Task NewGame();
        Task EvaluateAnswer(Answer answer);
    }
}
