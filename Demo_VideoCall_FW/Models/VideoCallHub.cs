using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo_VideoCall_FW.Models
{
    public class VideoCallHub : Hub
    {
        public void SendOffer(string connectionId, string offer)
        {
            Clients.Client(connectionId).ReceiveOffer(Context.ConnectionId, offer);
        }

        public void SendAnswer(string connectionId, string answer)
        {
            Clients.Client(connectionId).ReceiveAnswer(Context.ConnectionId, answer);
        }

        public void SendIceCandidate(string connectionId, string candidate)
        {
            Clients.Client(connectionId).ReceiveIceCandidate(Context.ConnectionId, candidate);
        }
    }
}