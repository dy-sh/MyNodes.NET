/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public class PanelNode : Node
    {
        public string Name { get; set; }

        public PanelNode() : base(0, 0)
        {
            this.Title = "Panel";
            this.Type = "Main/Panel";
        }

        public override void Loop()
        {
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

            Input input = new Input
            {
                Id = node.Id,
                Name = GenerateNewInputName()
            };

            node.Name = input.Name;
            AddInput(input);

            engine.UpdateNode(node);
            engine.UpdateNodeInDb(node);

            UpdateMe();
            UpdateMeInDb();
        }

        public void AddOutputNode(PanelOutputNode node)
        {
            if (Outputs.Any(x => x.Id == node.Id))
                return;


            Output output = new Output
            {
                Id = node.Id,
                Name = GenerateOutputName()
            };

            node.Name = output.Name;
            AddOutput(output);

            engine.UpdateNode(node);
            engine.UpdateNodeInDb(node);

            UpdateMe();
            UpdateMeInDb();
        }



        public bool RemoveInput(PanelInputNode node)
        {
            Input input = engine.GetInput(node.Id);

            Link link = engine.GetLinkForInput(input);
            if (link != null)
                engine.RemoveLink(link);

            Inputs.Remove(input);
            UpdateMe();
            UpdateMeInDb();
            return true;
        }

        public bool RemoveOutput(PanelOutputNode node)
        {
            Output output = engine.GetOutput(node.Id);

            List<Link> links = engine.GetLinksForOutput(output);
            foreach (var link in links)
                engine.RemoveLink(link);

            Outputs.Remove(output);
            UpdateMe();
            UpdateMeInDb();
            return true;
        }




        public override bool OnAddToEngine(NodesEngine engine)
        {
            this.engine = engine;

            if (Name==null)
            Name = GeneratePanelName();

            base.OnAddToEngine(engine);
            return true;
        }


        private string GeneratePanelName()
        {
            //auto naming
            List<PanelNode> panels = engine.GetPanelNodes();
            List<string> names = panels.Select(x => x.Name).ToList();
            for (int i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"Panel {i}"))
                    return $"Panel {i}";
            }
            return null;
        }


        public override void OnRemove()
        {
            List<Node> nodesList = engine.GetNodesForPanel(Id, false);
            foreach (var n in nodesList)
            {
                engine.RemoveNode(n);
            }
        }

        private string GenerateNewInputName()
        {
            //auto naming
            List<string> names = Inputs.Select(x => x.Name).ToList();
            for (int i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"In {i}"))
                    return $"In {i}";
            }
            return null;
        }

        private string GenerateOutputName()
        {
            //auto naming
            List<string> names = Outputs.Select(x => x.Name).ToList();
            for (int i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"Out {i}"))
                    return $"Out {i}";
            }
            return null;
        }
    }
}
