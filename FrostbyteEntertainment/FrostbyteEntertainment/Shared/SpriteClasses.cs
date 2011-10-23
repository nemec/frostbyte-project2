using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frostbyte
{
    internal partial class OurSprite
    {
        public delegate void HealthChangedHandler(object obj, int value);
        public event HealthChangedHandler HealthChanged = delegate { };

        internal int MaxHealth { get { return 100; } }

        /// <summary>
        /// Health of the Sprite
        /// </summary>
        private int mHealth;
        internal int Health
        {
            get
            {
                return mHealth;
            }
            set
            {
                mHealth = value < 0 ? 0 :
                    (value > MaxHealth ? MaxHealth :
                        value);
                HealthChanged(this, mHealth);
            }
        }
    }
}
