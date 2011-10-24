using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte
{
    public delegate void ManaChangedHandler(object obj, int value);

    internal abstract class Player : OurSprite
    {
        public event ManaChangedHandler ManaChanged = delegate { };

        internal Player(string name, Actor actor)
            : base(name, actor)
        {
            (This.Game.CurrentLevel as FrostbyteLevel).allies.Add(this);
            Mana = MaxMana;
        }

        internal int MaxMana { get { return 100; } }

        /// <summary>
        /// Player's Mana value
        /// </summary>
        private int mMana;
        internal int Mana
        {
            get
            {
                return mMana;
            }
            set
            {
                mMana = value < 0 ? 0 :
                    (value > MaxMana ? MaxMana :
                        value);
                ManaChanged(this, mMana);
            }
        }
    }

    internal abstract class Obstacle : OurSprite
    {
        internal Obstacle(string name, Actor actor)
            : base(name, actor)
        {
            (This.Game.CurrentLevel as FrostbyteLevel).obstacles.Add(this);
        }
    }

    internal abstract partial class OurSprite : Sprite
    {
        internal OurSprite(string name, Actor actor)
            : base(name, actor) { }

        internal OurSprite(string name, Actor actor, int collisionlist)
            : base(name, actor, collisionlist) { }
    }
}
