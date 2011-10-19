using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frostbyte.Enemies
{
    internal partial class IceGolem : Golem
    {
        internal IceGolem(string name, float speed, int health)
            : base(name, speed, health)
        {
            ElementType = Element.Water;
        }
    }
}