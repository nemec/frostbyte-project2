using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            this.theme = theme;
        }
        #endregion

        #region Variables
        private HUDTheme theme;
        private TextScroller scroller;
        List<PlayerHUD> playerHUDS = new List<PlayerHUD>();
        private static Vector2 barSize = new Vector2(100, 20);
        private static Vector2 barSpacing = new Vector2(10, 2);
        #endregion

        #region Methods
        internal void LoadCommon()
        {
            scroller = new TextScroller("scroller", theme);
            scroller.Pos = new Vector2(FrostbyteLevel.BORDER_WIDTH / 2,
                This.Game.GraphicsDevice.Viewport.Height - scroller.GetAnimation().Height);
            scroller.Static = true;
        }

        internal void AddPlayer(Player p)
        {
            int xoffset = 80 + playerHUDS.Count * (int)(barSize.X + barSpacing.X);
            playerHUDS.Add(new PlayerHUD(theme, p, xoffset, 10));
        }

        internal void ScrollText(string s)
        {
            if (scroller != null)
            {
                scroller.ScrollText(s);
            }
        }
        #endregion

        private class PlayerHUD
        {
            internal PlayerHUD(HUDTheme theme, Player p, int xOffset, int yOffset)
            {
                Text name = new Text("player_name_" + p.Name, "Text", p.Name);
                name.DisplayColor = theme.TextColor;
                name.Pos = new Vector2(xOffset, yOffset);
                name.Static = true;

                healthBar = new ProgressBar("Health_" + p.Name, p.MaxHealth,
                    Color.MidnightBlue, Color.Blue, Color.Black, barSize);
                healthBar.Pos = new Vector2(xOffset, yOffset + name.GetAnimation().Height);
                healthBar.Static = true;
                healthBar.Value = p.MaxHealth;

                p.HealthChanged += delegate(object obj, int value)
                {
                    healthBar.Value = value;
                };
                
                manaBar = new ProgressBar("Mana_" + p.Name, p.MaxMana,
                    Color.DarkRed, Color.Firebrick, Color.Black, barSize);
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

        private class TextScroller : Sprite
        {
            internal TextScroller(string name, HUDTheme theme)
                : base(name, new Actor(new DummyAnimation(name,
                    This.Game.GraphicsDevice.Viewport.Width - FrostbyteLevel.BORDER_WIDTH,
                    This.Game.GraphicsDevice.Viewport.Height - 2 * FrostbyteLevel.BORDER_HEIGHT)))
            {
                ZOrder = 100;
                UpdateBehavior = update;
                this.theme = theme;
                background = new Texture2D(This.Game.GraphicsDevice, 1, 1);
                Color transp = Color.Black;
                transp.A = alpha;
                background.SetData(new Color[] { transp });
            }

            internal int MaxCharactersPerLine = 72;
            internal int TextSpacing = 2;
            internal bool SplitOnWhitespace = true;
            private byte alpha = 90;
            private HUDTheme theme;
            private List<char> buffer = new List<char>();
            private List<Text> onScreen = new List<Text>();
            private Texture2D background;

            private int tickCount = 0;
            internal int TicksPerScroll = 2;

            internal void ScrollText(string s)
            {
                buffer.AddRange(s);
                buffer.AddRange("\n\n");
            }

            internal void update(){
                tickCount = (tickCount + 1) % TicksPerScroll;
                if (onScreen.Count > 0 && tickCount == 0)
                {
                    foreach (Text t in onScreen)
                    {
                        t.Pos.Y -= 1;
                    }

                    Sprite fst = onScreen.First();
                    if (fst.Pos.Y < Pos.Y)
                    {
                        This.Game.CurrentLevel.RemoveSprite(fst);
                        onScreen.RemoveAt(0);
                    }
                }
                if (buffer.Count != 0)
                {
                    // We have room to scroll another line of text
                    if (onScreen.Count == 0 ||
                        onScreen.Last().Pos.Y + onScreen.Last().GetAnimation().Height + TextSpacing < 
                            Pos.Y + GetAnimation().Height)
                    {
                        buffer = buffer.SkipWhile(x => char.IsWhiteSpace(x)).ToList();
                        IEnumerable<char> pendingDisplay = buffer.Take(MaxCharactersPerLine + 1).
                            Reverse();
                        if (SplitOnWhitespace)
                        {
                            // Find first instance of whitespace at end
                            pendingDisplay = pendingDisplay.SkipWhile(x => !char.IsWhiteSpace(x));
                        }

                        string toDisplay = pendingDisplay.Reverse().
                            Aggregate("", (s, c) => s + c).Trim(); // Convert to string and trim whitespace
                        buffer.RemoveRange(0, pendingDisplay.Count());

                        Text line = new Text("text", "Text", toDisplay);
                        line.DisplayColor = theme.TextColor;
                        line.Pos = new Vector2(Pos.X, Pos.Y + GetAnimation().Height);
                        line.Static = true;
                        line.ZOrder = 101;
                        onScreen.Add(line);
                    }
                }
            }

            internal override void Draw(GameTime gameTime)
            {
                base.Draw(gameTime);
                if (onScreen.Count > 0)
                {
                    This.Game.spriteBatch.Draw(background, new Rectangle(
                            (int)Pos.X,
                            (int)Pos.Y,
                            (int)GetAnimation().Width,
                            (int)GetAnimation().Height), Color.White);
                }
            }
        }
    }
}
