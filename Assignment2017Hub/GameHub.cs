using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Assignment2017Hub
{
    public class GameHub : Hub
    {
        public void Notification(string message)
        {
            Clients.All.Notify(message);
        }

        public void AddPlayer(string name)
        {

            Notification(name + " joined");
        }

        public override Task OnConnected()
        {
            Clients.Caller.accept();
            return base.OnConnected();
        }

        public void PlayerStart()
        {
            Clients.Caller.welcome();
        }
    }
}