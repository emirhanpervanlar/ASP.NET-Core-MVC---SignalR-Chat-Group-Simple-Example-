using LearningCoreExample.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningCoreExample.Socket
{
    public class ChatHub : Hub
    {
        public static Dictionary<string,HubUser> allUser = new Dictionary<string, HubUser>();

        public override Task OnConnectedAsync()
        {
            string id = Context.ConnectionId;
            allUser.Add(id,new HubUser());
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Clients.All.SendAsync("Send", allUser[Context.ConnectionId].userName, "<u>Oturumu kapattı !</u> ");
            // Kullanıcının listede olup omadığını kontrol ediyoruz
            if (allUser.ContainsKey(Context.ConnectionId))
            {
                // Kullanıcıyı çıkarıyoruz
                allUser.Remove(Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }


        public async Task setUser(string userName)
        {

            

            // Kullanıcının listede olup omadığını kontrol ediyoruz
            if (allUser.ContainsKey(Context.ConnectionId))
            {
                // Kullanıcının adını tanımlıyoruz
                allUser[Context.ConnectionId].userName = userName;
                await Clients.All.SendAsync("Send", "", $"{allUser[Context.ConnectionId].userName} oturum açtı.");
            }
        }

        public async Task AddToGroup(string groupName)
        { 
            // Kullanıcının listede olup omadığını kontrol ediyoruz
            if (allUser.ContainsKey(Context.ConnectionId))
            {
                // Kullanıcıyı gruba dahil ediyoruz ve kendi grup listesini güncelliyoruz
                allUser[Context.ConnectionId].groupList.Add(groupName);
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                await Clients.Group(groupName).SendAsync("SendGroup", "", $"{allUser[Context.ConnectionId].userName} has joined the group {groupName}.");
            }
        }


        public async Task RemoveFromGroup(string groupName)
        {
            // Kullanıcının listede olup omadığını kontrol ediyoruz
            if (allUser.ContainsKey(Context.ConnectionId))
            {
                // Kullanıcıyı gruptan çıkarıyoruz ve kendi grup listesini güncelliyoruz
                allUser[Context.ConnectionId].groupList.Remove(groupName);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                await Clients.Group(groupName).SendAsync("SendGroup", "", $"{allUser[Context.ConnectionId].userName} has left the group {groupName}.");
            }
        }

        public async Task SendPublicRoom(string message)
        {
            // Kullanıcının listede olup omadığını kontrol ediyoruz
            if (allUser.ContainsKey(Context.ConnectionId))
            {
                await Clients.All.SendAsync("Send", allUser[Context.ConnectionId].userName, message);
            }
        }

        public async Task SendGroupMessage(string groupName,string message)
        {
            await Clients.Group(groupName).SendAsync("SendGroup", allUser[Context.ConnectionId].userName, message);
        }
        
    }
}
