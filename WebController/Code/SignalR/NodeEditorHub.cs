/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Threading.Tasks;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace MyNodes.WebController.Code
{
    public class NodeEditorHub : Hub
    {
        public void Join(string panelId)
        {
            this.Groups.Add(this.Context.ConnectionId, panelId);
        }
    }

}