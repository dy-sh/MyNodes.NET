/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/


using Microsoft.AspNetCore.SignalR;

namespace MyNodes.WebController.Code
{
    public class DashboardHub : Hub
    {
        public void Join(string panelId)
        {
            this.Groups.Add(this.Context.ConnectionId, panelId);
        }
    }

}