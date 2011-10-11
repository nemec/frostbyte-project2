using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Frostbyte
{
    /// <summary>
    /// Do anything required for Game-specific code here
    /// to avoid cluttering up the Engine
    /// </summary>
    static class GameData
    {
        internal static int Score { get; set; }
        internal static int NumberOfLives { get; set; }
        internal static readonly int DefaultNumberOfLives = 4;
        internal static int livesAwarded = 0;
    }

    /// <summary>
    /// Enables sorting Sprite lists by distance from an origin Sprite
    /// </summary>
    internal class DistanceSort : IComparer<Sprite>
    {
        Sprite origin;

        internal DistanceSort(Sprite origin)
        {
            this.origin = origin;
        }

        int IComparer<Sprite>.Compare(Sprite x, Sprite y)
        {
            double lx = (x.Pos - origin.Pos).LengthSquared();
            double ly = (y.Pos - origin.Pos).LengthSquared();
            if (lx > ly)
            {
                return 1;
            }
            else if (lx < ly)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Add Game-specific level code here to avoid cluttering up the Engine
    /// </summary>
    class FrostbyteLevel : Level
    {
        internal Vector2[] PlayerSpawnPoint = new Vector2[2]{
            new Vector2(50, 50),
            new Vector2(60, 50)
        };

        internal FrostbyteLevel(string n, Behavior loadBehavior, Behavior updateBehavior,
            Behavior endBehavior, Condition winCondition)
            : base(n, loadBehavior, updateBehavior, endBehavior, winCondition)
        {
        }

        /// <summary>
        /// Target lists
        /// </summary>
        internal List<Sprite> allies = new List<Sprite>();
        internal List<Sprite> enemies = new List<Sprite>();
        internal List<Sprite> obstacles = new List<Sprite>();

        internal TileList TileMap = new TileList();
        private Vector3 StartDraw;
        private Vector3 EndDraw;
        private Polygon viewportPolygon = null;

        /// <summary>
        /// A list of levels in the order they should be played through
        /// </summary>
        internal static List<string> LevelProgression = new List<string>()
        {
            "Earth",
            "Wind",
            "Lightning",
            "Fire",
            "Heart"
        };

        /// <summary>
        /// Retains progress through our levels
        /// </summary>
        internal static int CurrentStage = 0;

        internal void RealignViewport()
        {
            if (viewportPolygon == null)
            {
                Viewport viewport = This.Game.GraphicsDevice.Viewport;
                viewportPolygon = new Polygon("viewport", Color.DarkRed, new Vector3[5]{
                    new Vector3(200, 200, 0), 
                    new Vector3(viewport.Width - 200, 200, 0), 
                    new Vector3(viewport.Width - 200, viewport.Height - 200, 0), 
                    new Vector3(200, viewport.Height - 200, 0),
                    new Vector3(200, 200, 0)
                });
                viewportPolygon.Static = true;
            }

            List<Sprite> players = GetSpritesByType(typeof(Player));
            if (players != null && players.Count > 0)
            {
                Viewport viewport = This.Game.GraphicsDevice.Viewport;
                Vector2 cameraPos = This.Game.CurrentLevel.Camera.Pos;
                int borderWidth = 200;//viewport.Width/2;
                int borderHeight = 200;//viewport.Height/2;

                Vector2 min = players[0].Pos;
                Vector2 max = players[0].Pos;
                foreach(Sprite player in players){
                    if (player.Pos.X < min.X)
                    {
                        min.X = player.Pos.X;
                    }
                    if (player.Pos.Y < min.Y)
                    {
                        min.Y = player.Pos.Y;
                    }
                    if (player.Pos.X > max.X)
                    {
                        max.X = player.Pos.X + player.GetAnimation().Width;
                    }
                    if (player.Pos.Y > max.Y)
                    {
                        max.Y = player.Pos.Y + player.GetAnimation().Height;
                    }
                }

                Vector2 spread = max - min;
                spread.X = Math.Abs(spread.X);
                spread.Y = Math.Abs(spread.Y);
                int w = viewport.Width - 2 * borderWidth;
                int h = viewport.Height - 2 * borderHeight;
                float scale = This.Game.CurrentLevel.Camera.Zoom;
                if (spread.X < w * scale)
                {
                    spread.X = w * scale;
                }
                if (spread.Y < h * scale)
                {
                    spread.Y = h * scale;
                }

                scale = Math.Min(w / spread.X * scale, h / spread.Y * scale);
                This.Game.CurrentLevel.Camera.Zoom = scale;

                Vector2 minDiff = min - cameraPos;
                Vector2 maxDiff = max - cameraPos;

                if (minDiff.X < viewport.X + borderWidth)
                {
                    cameraPos.X -= borderWidth - (minDiff.X);
                }
                else if (maxDiff.X > viewport.X + viewport.Width - borderWidth)
                {
                    cameraPos.X += borderWidth - (viewport.Width - (maxDiff.X));
                }
                if (minDiff.Y < viewport.Y + borderHeight)
                {
                    cameraPos.Y -= borderHeight - (minDiff.Y);
                }
                else if (maxDiff.Y > viewport.Y + viewport.Height - borderHeight)
                {
                    cameraPos.Y += borderHeight - (viewport.Height - (maxDiff.Y));
                }
                This.Game.CurrentLevel.Camera.Pos = cameraPos;
            }
        }

        internal override void Update()
        {
            base.Update();

            //RealignViewport();

            /*KeyboardState k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.K))
            {
                Camera.Pos = new Vector2(Camera.Pos.X, Camera.Pos.Y + 5);
            }
            else if (k.IsKeyDown(Keys.I))
            {
                Camera.Pos = new Vector2(Camera.Pos.X, Camera.Pos.Y - 5);
            }

            if (k.IsKeyDown(Keys.L))
            {
                Camera.Pos = new Vector2(Camera.Pos.X + 5, Camera.Pos.Y);
            }
            if (k.IsKeyDown(Keys.J))
            {
                Camera.Pos = new Vector2(Camera.Pos.X - 5, Camera.Pos.Y);
            }

            if (k.IsKeyDown(Keys.PageUp))
            {
                Camera.Zoom += 0.05f;
            }
            else if (k.IsKeyDown(Keys.PageDown))
            {
                Camera.Zoom -= 0.05f;
            }
            else if (k.IsKeyDown(Keys.Home))
            {
                Camera.Zoom = 1f;
            }*/
            

            Matrix drawTransformation = Camera.GetTransformation(This.Game.GraphicsDevice);
            Viewport viewport = This.Game.GraphicsDevice.Viewport;
            Vector3 cameraPosition = new Vector3(Camera.Pos, 0);

            StartDraw = (cameraPosition) / Tile.TileSize;
            EndDraw = (cameraPosition + new Vector3(viewport.Width, viewport.Height, 0)) / Tile.TileSize;
        }
        
        internal override void Draw(GameTime gameTime)
        {
            /// \todo draw base tiles
            This.Game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.GetTransformation(This.Game.GraphicsDevice));

            for (int x = (int)Math.Floor(StartDraw.X); x < (int)Math.Ceiling(EndDraw.X); x++)
            {
                for (int y = (int)Math.Floor(StartDraw.Y); y < (int)Math.Ceiling(EndDraw.Y); y++)
                {
                    Tile toDraw;
                    TileMap.TryGetValue(x, y, out toDraw);

                    toDraw.Draw();
                }
            }
            This.Game.spriteBatch.End();

            base.Draw(gameTime);

            /// \todo draw bottom tiles
        }
    }
}
