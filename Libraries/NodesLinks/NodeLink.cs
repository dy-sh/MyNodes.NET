/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodesLinks
{
    public class NodeLink
    {
        [Key]
        public int db_Id { get; set; }

        public int inSensorDbId { get; set; }
        public int inNodeId { get; set; }
        public int inSensorId { get; set; }

        public int outSensorDbId { get; set; }
        public int outNodeId { get; set; }
        public int outSensorId { get; set; }
    }
}
