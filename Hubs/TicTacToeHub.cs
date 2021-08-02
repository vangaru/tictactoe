using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TicTacToeOnlineGame.Hubs
{
    public class TicTacToeHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveClientConnectionId", Context.ConnectionId);
            await Clients.Others.SendAsync("ReceiveConnectedUser");
            
            await base.OnConnectedAsync();
        }

        public async Task EnableEventListeners(string url)
        {
            await Clients.Others.SendAsync("ReceiveEventListeners", url);
        }

        public async Task SendMove(string currentClass, int cellId, string url)
        {
            await Clients.Others.SendAsync("ReceiveMove", currentClass, cellId, url);
        }

        public async Task SendGameResult(bool draw, string url)
        {
            await Clients.Others.SendAsync("ReceiveGameResult", draw, url);
        }
    }
}