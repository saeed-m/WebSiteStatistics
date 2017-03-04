using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace WebSiteStatistics.Hubs
{
    public class OnlineUsersHub : Hub
    {
        public static readonly ConcurrentDictionary<string, string> OnlineUsers = new ConcurrentDictionary<string, string>();

        public void UpdateUsersOnlineCount()
        {
            // آي پي معرف يك كاربر است
            // اما كانكشن آي دي معرف يك برگه جديد در مرورگر او است
            // هر كاربر مي‌تواند چندين برگه را به يك سايت گشوده يا ببندد
            var ipsCount = OnlineUsers.Select(x => x.Value).Distinct().Count();
            this.Clients.All.updateUsersOnlineCount(ipsCount);
        }

        /// <summary>
        /// اگر كاربران اعتبار سنجي شده‌اند بهتر است از
        /// this.Context.User.Identity.Name
        /// بجاي آي پي استفاده شود
        /// </summary>
        protected string GetUserIpAddress()
        {
            object serverRemoteIpAddress;
            return !Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out serverRemoteIpAddress)
                ? null : serverRemoteIpAddress.ToString();
        }

        public override Task OnConnected()
        {
            var ip = GetUserIpAddress();
            OnlineUsers.TryAdd(this.Context.ConnectionId, ip);
            UpdateUsersOnlineCount();

            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            var ip = GetUserIpAddress();
            OnlineUsers.TryAdd(this.Context.ConnectionId, ip);
            UpdateUsersOnlineCount();

            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // در اين حالت ممكن است مرورگر كاملا بسته شده باشد
            // يا حتي صرفا يك برگه مرورگر از چندين برگه متصل به سايت بسته شده باشند
            string ip;
            OnlineUsers.TryRemove(this.Context.ConnectionId, out ip);
            UpdateUsersOnlineCount();

            return base.OnDisconnected(stopCalled);
        }
    }
}