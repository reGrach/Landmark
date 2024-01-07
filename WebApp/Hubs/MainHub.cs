using Microsoft.AspNetCore.SignalR;

namespace Landmark.WebApp.Hubs
{
    public class MainHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}