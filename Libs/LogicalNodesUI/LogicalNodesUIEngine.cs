using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public void ButtonClick(string nodeId)
        {
            LogicalNode n = engine.GetNode(nodeId);
            if (!(n is LogicalNodeUIButton))
                return;

            LogicalNodeUIButton node = (LogicalNodeUIButton)n;
            node.Click();
        }

        public void ToggleButtonClick(string nodeId)
        {
            LogicalNode n = engine.GetNode(nodeId);
            if (!(n is LogicalNodeUIToggleButton))
                return;

            LogicalNodeUIToggleButton node = (LogicalNodeUIToggleButton)n;
            node.Toggle();
        }

        public void SliderChange(string nodeId, int value)
        {
            LogicalNode n = engine.GetNode(nodeId);
            if (!(n is LogicalNodeUISlider))
                return;

            LogicalNodeUISlider node = (LogicalNodeUISlider)n;
            node.SetValue(value);
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
            LogicalNode inNode = engine.GetInputOwner(link.InputId);

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
