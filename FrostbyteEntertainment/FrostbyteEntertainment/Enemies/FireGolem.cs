using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frostbyte.Enemies
{
    internal partial class FireGolem : Golem
    {
        internal FireGolem(string name, float speed, int health)
            : base(name, speed, health)
        {
            ElementType = Element.Fire;
        }
    }
}
