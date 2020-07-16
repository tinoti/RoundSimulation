using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Model
{
    class Entity
    {
        public string EntityId { get; set; }

        public int Health { get; set; }

        public int Attack { get; set; }

        public bool AttackReady { get; set; }

        public int EntityType { get; set; }

        public List<Modifiers> Modifiers { get; set; }
    }
}
