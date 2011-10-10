using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

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
        internal Vector2 PlayerSpawnPoint = new Vector2(50, 50);

        internal FrostbyteLevel(string n, Behavior loadBehavior, Behavior updateBehavior, Behavior endBehavior, Condition winCondition)
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
        internal Vector3 StartDraw;
        internal Vector3 EndDraw;

        /// <summary>
        /// A list of levels in the order they should be played through
        /// </summary>
        internal static List<string> LevelProgression = new List<string>()
        {
            "Stomach",
            "Lungs",
            "Credits"
        };

        /// <summary>
        /// Retains progress through our levels
        /// </summary>
        internal static int CurrentStage = 0;

        internal void RealignViewport()
        {
            List<Sprite> players = GetSpritesByType("Player");
            Sprite player = players[0];
            if (players != null)
            {
                Viewport viewport = This.Game.GraphicsDevice.Viewport;
                Vector2 cameraPos = This.Game.CurrentLevel.Camera.Pos;
                int borderWidth = 200;//viewport.Width/2;
                int borderHeight = 200;//viewport.Height/2;

                Vector2 difference = player.Pos - cameraPos;
                if (difference.X < viewport.X + borderWidth)
                {
                    cameraPos.X -= borderWidth - (difference.X);
                }
                else if (difference.X > viewport.X + viewport.Width - borderWidth)
                {
                    cameraPos.X += borderWidth - (viewport.Width - (difference.X));
                }
                if (difference.Y < viewport.Y + borderWidth)
                {
                    cameraPos.Y -= borderHeight - (difference.Y);
                }
                else if (difference.Y > viewport.Y + viewport.Height - borderWidth)
                {
                    cameraPos.Y += borderHeight - (viewport.Height - (difference.Y));
                }
                This.Game.CurrentLevel.Camera.Pos = cameraPos;
            }
        }

        internal override void Update()
        {
            base.Update();

            //RealignViewport();

            KeyboardState k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.Down))
            {
                Camera.Pos = new Vector2(Camera.Pos.X, Camera.Pos.Y + 5);
            }
            else if (k.IsKeyDown(Keys.Up))
            {
                Camera.Pos = new Vector2(Camera.Pos.X, Camera.Pos.Y - 5);
            }

            if (k.IsKeyDown(Keys.Right))
            {
                Camera.Pos = new Vector2(Camera.Pos.X + 5, Camera.Pos.Y);
            }
            if (k.IsKeyDown(Keys.Left))
            {
                Camera.Pos = new Vector2(Camera.Pos.X - 5, Camera.Pos.Y);
            }


            Matrix drawTransformation = Camera.GetTransformation(This.Game.GraphicsDevice);
            Viewport viewport = This.Game.GraphicsDevice.Viewport;
            Vector3 cameraPosition = new Vector3(Camera.Pos, 0);

            StartDraw = (cameraPosition +  drawTransformation.Translation) / Tile.TileSize;
            EndDraw = (cameraPosition + new Vector3(viewport.Width, viewport.Height, 0) +
                        drawTransformation.Translation) / Tile.TileSize;
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

    public partial class Tile : LevelObject
    {
        internal void Draw()
        {
            /*string file = "error.png";
            switch (Type)
            {
                case TileTypes.Wall:
                    file = "wall.png";
                    break;
                case TileTypes.Bottom:
                    file = "top-grass.png";
                    break;
                case TileTypes.Corner:
                    file = "corner.png";
                    break;
                case TileTypes.ConvexCorner:
                    file = "convex-coner.png";
                    break;
                case TileTypes.Floor:
                    file = "floor.png";
                    break;
                case TileTypes.Lava:
                    file = "lava.png";
                    break;
                case TileTypes.Water:
                    file = "water.png";
                    break;
                case TileTypes.SideWall:
                    file = "side.png";
                    break;
                case TileTypes.Room:
                    //do some magic to show pic for the walls etc
                    file = "room.png";
                    break;
                case TileTypes.Empty:
                    file = "";
                    break;
                default:
                    file = "error.png";
                    break;
            }*/
            //BitmapImage image = new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
            Texture2D image = This.Game.Content.Load<Texture2D>("corner");
            if (GridCell != null)
            {
                Vector2 gridCell = new Vector2(GridCell.X, GridCell.Y);
                This.Game.spriteBatch.Draw(image, gridCell, new Rectangle(GridCell.X, GridCell.Y, TileSize, TileSize),
                    Microsoft.Xna.Framework.Color.White, 0, new Vector2(), Orientation.Scale(), SpriteEffects.None, 0);
            }
        }
    }

    public static class EnumExtensions
    {
        public static Vector2 Scale(this Orientations o)
        {
            /*if (o == Orientations.Up_Left)
            {
                return new ScaleTransform(-1, -1);
            }
            else if (o == Orientations.Up)
            {
                return new ScaleTransform(1, -1);
            }
            else if (o == Orientations.Right)
            {
                return new ScaleTransform(-1, 1);
            }
            return new ScaleTransform(1, 1);*/
            if (o == Orientations.Up_Left)
            {
                return new Vector2(-1, -1);
            }
            else if (o == Orientations.Up)
            {
                return new Vector2(1, -1);
            }
            else if (o == Orientations.Right)
            {
                return new Vector2(-1, 1);
            }
            return new Vector2(1, 1);
        }
    }
}
