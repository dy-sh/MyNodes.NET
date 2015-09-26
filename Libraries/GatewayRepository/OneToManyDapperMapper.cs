using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.GatewayRepository
{
    public class OneToManyDapperMapper<TP, TC, TPk>
    {
        private readonly IDictionary<TPk, TP> _lookup = new Dictionary<TPk, TP>();
        public Action<TP, TC> AddChildAction { get; set; }
        public Func<TP, TPk> ParentKey { get; set; }

        public virtual TP Map(TP parent, TC child)
        {
            TP entity;
            var found = true;
            var primaryKey = ParentKey(parent);
            if (!_lookup.TryGetValue(primaryKey, out entity))
            {
                _lookup.Add(primaryKey, parent);
                entity = parent;
                found = false;
            }
            AddChildAction(entity, child);
            return !found ? entity : default(TP);
        }
    }
}
