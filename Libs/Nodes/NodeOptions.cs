﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public class NodeOptions
    {
        public bool LogOutputChanges = true;
        public bool ProtectedAccess = false;
        public bool ResetOutputsIfAnyInputIsNull=false;
    }
}