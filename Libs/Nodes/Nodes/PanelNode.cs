/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNodes.Nodes
{
    public class PanelNode : Node
    {
        public PanelNode() : base("Main", "Panel")
        {
            Settings.Add("Name", new NodeSetting(NodeSettingType.Text, "Name", ""));
        }


        public override void OnInputChange(Input input)
        {
            if (engine != null)
                engine.GetNode(input.Id).Outputs[0].Value = input.Value;
        }

        public void AddInputNode(PanelInputNode node)
        {
            if (Inputs.Any(x => x.Id == node.Id))
                return;

            var input = new Input
            {
                Id = node.Id,
                Name = GenerateNewInputName()
            };

            node.Settings["Name"].Value = input.Name;
            AddInput(input);

            UpdateMeInEditor();
            UpdateMeInDb();
        }

        public void AddOutputNode(PanelOutputNode node)
        {
            if (Outputs.Any(x => x.Id == node.Id))
                return;


            var output = new Output
            {
                Id = node.Id,
                Name = GenerateOutputName()
            };

            node.Settings["Name"].Value = output.Name;
            AddOutput(output);

            UpdateMeInEditor();
            UpdateMeInDb();
        }


        public bool RemovePanelInput(PanelInputNode node)
        {
            var input = engine.GetInput(node.Id);

            RemoveInput(input);

            UpdateMeInEditor();
            UpdateMeInDb();
            return true;
        }

        public bool RemovePanelOutput(PanelOutputNode node)
        {
            var output = engine.GetOutput(node.Id);

            RemoveOutput(output);

            UpdateMeInEditor();
            UpdateMeInDb();
            return true;
        }


        public override bool OnAddToEngine(NodesEngine engine)
        {
            this.engine = engine;

            if (string.IsNullOrEmpty(Settings["Name"].Value))
                Settings["Name"].Value = GeneratePanelName();

            base.OnAddToEngine(engine);
            return true;
        }


        private string GeneratePanelName()
        {
            //auto naming
            var panels = engine.GetPanelNodes();
            var names = panels.Select(x => x.Settings["Name"].Value).ToList();
            for (var i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"Panel {i}"))
                    return $"Panel {i}";
            }
            return null;
        }


        public override void OnRemove()
        {
            var nodesList = engine.GetNodesForPanel(Id, false);
            foreach (var n in nodesList)
            {
                engine.RemoveNode(n);
            }
        }

        private string GenerateNewInputName()
        {
            //auto naming
            var names = Inputs.Select(x => x.Name).ToList();
            for (var i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"In {i}"))
                    return $"In {i}";
            }
            return null;
        }

        private string GenerateOutputName()
        {
            //auto naming
            var names = Outputs.Select(x => x.Name).ToList();
            for (var i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"Out {i}"))
                    return $"Out {i}";
            }
            return null;
        }


        public override bool SetSettings(Dictionary<string, string> data)
        {
            bool result = base.SetSettings(data);
            UpdateMeOnDashboard();
            return result;
        }


        public override string GetJsListGenerationScript()
        {
            return @"

            //PanelNode
            function PanelNode() {
                this.properties = {
                    'ObjectType': 'MyNodes.Nodes.PanelNode',
                    'Assembly': 'Nodes'
                };
            }
            PanelNode.title = 'Panel';
            PanelNode.prototype.getExtraMenuOptions = function (graphcanvas) {
                var that = this;
                return [
                    { content: 'Open', callback: function () { window.location = '/NodeEditor/Panel/' + that.id; } },
                    null, //null for horizontal line
                    { content: 'Show on Dashboard', callback: function () { var win = window.open('/Dashboard/Panel/' + that.id, '_blank'); win.focus(); } },
                    null,
                    { content: 'Export to file', callback: function () { var win = window.open('/NodeEditorAPI/SerializePanelToFile/' + that.id, '_blank'); win.focus(); } },
                    { content: 'Export to script', callback: function () { editor.exportPanelToScript(that.id) } },
                    { content: 'Export URL', callback: function () { editor.exportPanelURL(that.id) } },
                    null
                ];
            }
            LiteGraph.registerNodeType('Main/Panel', PanelNode);

            ";
        }

        public override string GetNodeDescription()
        {
            return "This node can contain other nodes. <br/>" +
                   "You can add any number of inputs/outputs to this node, " +
                   "and connect them with the nodes inside. <br/>" +
                   "Thus you can create your own nodes, which are named Panels.<br/> " +
                   "You can export the panels to save them on disk and import later. " +
                   "Or, you can share your panels with other people. <br/>" +
                   "Each panel has its own dashboard. <br/>" +
                   "You can add UI nodes, to create a separate control interface. <br/>" +
                   "Each UI panel will be available in the browser via a separate link. ";
        }
    }
}