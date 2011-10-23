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
        Dictionary<string, PlayerHUD> playerHUDS = new Dictionary<string, PlayerHUD>();
        private static Vector2 barSize = new Vector2(100, 20);
        private static Vector2 barSpacing = new Vector2(10, 2);
        #endregion

        #region Methods
        internal void AddPlayer(Player p)
        {
            int xoffset = 80 + playerHUDS.Count * (int)(barSize.X + barSpacing.X);
            playerHUDS[p.Name] = new PlayerHUD(p, xoffset, 10);
        }

        internal void RemovePlayer(string playerID)
        {
            if (playerHUDS.ContainsKey(playerID))
            {
                playerHUDS.Remove(playerID);
            }
        }
        #endregion

        private class PlayerHUD
        {
            internal PlayerHUD(Player p, int xOffset, int yOffset)
            {
                Text name = new Text("player_name_" + p.Name, "Text", p.Name);
                name.Pos = new Vector2(xOffset, yOffset);
                name.Static = true;

                healthBar = new ProgressBar("Health_" + p.Name, p.MaxHealth,
                    Color.DarkBlue, Color.Blue, Color.Black, barSize);
                healthBar.Pos = new Vector2(xOffset, yOffset + name.GetAnimation().Height + barSpacing.Y);
                healthBar.Static = true;
                healthBar.Value = p.MaxHealth;

                p.HealthChanged += delegate(object obj, int value)
                {
                    healthBar.Value = value;
                };
                
                manaBar = new ProgressBar("Mana_" + p.Name, p.MaxMana,
                    Color.DarkRed, Color.Red, Color.Black, barSize);
                manaBar.Pos = new Vector2(xOffset,
                    yOffset + name.GetAnimation().Height + barSize.Y + barSpacing.Y);
                manaBar.Static = true;
                manaBar.Value = p.MaxMana;

                p.ManaChanged += delegate(object obj, int value)
                {
                    manaBar.Value = value;
                };
            }

            ~PlayerHUD()
            {
                This.Game.CurrentLevel.RemoveSprite(healthBar);
                This.Game.CurrentLevel.RemoveSprite(manaBar);
            }

            #region Variables
            internal ProgressBar healthBar;
            internal ProgressBar manaBar;


            internal int Health
            {
                get
                {
                    return healthBar.Value;
                }
                set
                {
                    healthBar.Value = value;
                }
            }
            internal int Mana
            {
                get
                {
                    return manaBar.Value;
                }
                set
                {
                    manaBar.Value = value;
                }
            }
            #endregion
        }
    }
}
