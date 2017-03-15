/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/


using Microsoft.AspNetCore.SignalR;

namespace MyNodes.WebController.Code
{
    public class LogsHub : Hub
    {
        public void Join(string logSource)
        {
            this.Groups.Add(this.Context.ConnectionId, logSource);
        }
    }

}