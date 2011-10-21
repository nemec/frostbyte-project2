using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte
{
    interface HUDTheme
    {
        Color TextColor { get; }

    }

    internal class GenericTheme : HUDTheme
    {
        public Color TextColor { get { return Color.White; } }
    }

    internal class EarthTheme : HUDTheme
    {
        public Color TextColor { get { return Color.BurlyWood; } }
    }

    internal class HUD
    {
        #region Constructors
        internal HUD()
            : this(new GenericTheme())
        {
        }

        internal HUD(HUDTheme theme)
        {

        }
        #endregion

        #region Variables
        private int mHealth;
        internal int Health { 
            get { return mHealth; }
            set { mHealth = value; }
        }
        #endregion
    }
}
