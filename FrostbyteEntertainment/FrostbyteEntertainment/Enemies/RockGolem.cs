using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class RockGolem : Golem
    {
        internal RockGolem(string name, float speed, int health, Vector2 initialPos)
            : base(name, health, initialPos, speed)
        {
            ElementType = Element.Earth;
        }
    }
}
