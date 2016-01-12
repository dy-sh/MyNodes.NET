using System.Collections.Generic;
using System.Linq;
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
    public delegate void LogicalNodeUIEventHandler(LogicalNodeUI node);

    public class LogicalNodesUIEngine
    {
        public event LogicalNodeUIEventHandler OnNewUINodeEvent;
        public event LogicalNodeUIEventHandler OnUINodeDeleteEvent;
        public event LogicalNodeUIEventHandler OnUINodeUpdatedEvent;

        private static LogicalNodesEngine engine;

        public LogicalNodesUIEngine(LogicalNodesEngine engine)
        {
            LogicalNodesUIEngine.engine = engine;
            engine.OnNewNodeEvent += OnNewNodeEvent;
            engine.OnNodeDeleteEvent += OnNodeDeleteEvent;
            engine.OnNodeUpdatedEvent += OnNodeUpdatedEvent;
            engine.OnOutputUpdatedEvent += OnOutputUpdatedEvent;
            engine.OnInputUpdatedEvent += OnInputUpdatedEvent;
            engine.OnNewLinkEvent += OnNewLinkEvent;
        }

        public List<LogicalNodeUI> GetUINodes()
        {
            return engine.nodes
                .Where(n => n is LogicalNodeUI)
                .Cast<LogicalNodeUI>()
                .ToList();
        }

        private void OnInputUpdatedEvent(Input input)
        {
            LogicalNode node = engine.GetInputOwner(input);
            if (node is LogicalNodeUI)
                OnUINodeUpdatedEvent?.Invoke((LogicalNodeUI)node);
        }

        private void OnOutputUpdatedEvent(Output output)
        {
            LogicalNode node = engine.GetOutputOwner(output);
            if (node is LogicalNodeUI)
                OnUINodeUpdatedEvent?.Invoke((LogicalNodeUI)node);
        }

        private void OnNodeUpdatedEvent(LogicalNode node)
        {
            if (node is LogicalNodeUI)
                OnUINodeUpdatedEvent?.Invoke((LogicalNodeUI)node);
        }

        private void OnNodeDeleteEvent(LogicalNode node)
        {
            if (node is LogicalNodeUI)
                OnUINodeDeleteEvent?.Invoke((LogicalNodeUI)node);
        }

        private void OnNewNodeEvent(LogicalNode node)
        {
            if (node is LogicalNodeUI)
                OnNewUINodeEvent?.Invoke((LogicalNodeUI)node);
        }

        private void OnNewLinkEvent(LogicalLink link)
        {
            LogicalNode outNode = engine.GetOutputOwner(link.OutputId);
            LogicalNode inNode= engine.GetInputOwner(link.InputId);

            Output output = engine.GetOutput(link.OutputId);
            Input input = engine.GetInput(link.InputId);

            if (inNode is LogicalNodeUI)
            {
                LogicalNodeUI node = (LogicalNodeUI)inNode;
                node.Name = $"{outNode.Title} {output.Name}";
                engine.UpdateNode(node);
            }

            if (outNode is LogicalNodeUI)
            {
                LogicalNodeUI node = (LogicalNodeUI)outNode;
                node.Name = $"{inNode.Title} {input.Name}";
                engine.UpdateNode(node);
            }

        }


    }
}
