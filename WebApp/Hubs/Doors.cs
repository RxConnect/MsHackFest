using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WebApp.Hubs
{
    public class Doors : Hub
    {
        public Task Send(string message)
        {
            return Clients.All.InvokeAsync("Send", message + " to the world!");
        }
    }
}