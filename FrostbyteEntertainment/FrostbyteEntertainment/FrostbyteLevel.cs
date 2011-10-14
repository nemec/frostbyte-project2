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
        #region Constructors
        internal Vector2[] PlayerSpawnPoint = new Vector2[2]{
            new Vector2(50, 50),
            new Vector2(60, 50)
        };

        internal FrostbyteLevel(string n, Behavior loadBehavior, Behavior updateBehavior,
            Behavior endBehavior, Condition winCondition)
            : base(n, loadBehavior, updateBehavior, endBehavior, winCondition)
        {
        }
        #endregion

        #region Variables
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
        #endregion

        #region Constants
        private readonly float MAX_ZOOM = 1.0f;
        private readonly float MIN_ZOOM = 0.8f;
        private readonly int BORDER_WIDTH = 200;
        private readonly int BORDER_HEIGHT = 200;
        #endregion

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

        /// <summary>
        /// Iterates through every player onscreen to gather the minimum and maximum X and Y coordinates
        /// for any of the players. The new zoom/scale factor is calculated and then the viewport is shifted
        /// if need be.
        /// </summary>
        internal void RealignViewport()
        {
            #region Create Viewport
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
            #endregion

            List<Sprite> players = GetSpritesByType(typeof(Player));
            if (players != null && players.Count > 0)
            {
                Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
                Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

                #region Find Min and Max
                foreach (Sprite player in players)
                {
                    if (player.Pos.X < min.X)
                    {
                        min.X = player.Pos.X;
                    }
                    if (player.Pos.Y < min.Y)
                    {
                        min.Y = player.Pos.Y;
                    }
                    if (player.Pos.X + player.GetAnimation().Width > max.X)
                    {
                        max.X = player.Pos.X + player.GetAnimation().Width;
                    }
                    if (player.Pos.Y + player.GetAnimation().Height > max.Y)
                    {
                        max.Y = player.Pos.Y + player.GetAnimation().Height;
                    }
                }
                #endregion

                #region Calculate Zoom Factor
                Viewport viewport = This.Game.GraphicsDevice.Viewport;
                float zoom = This.Game.CurrentLevel.Camera.Zoom;
                Vector2 span = max - min;
                float scaleX = (viewport.Width - 2 * BORDER_WIDTH) / span.X;
                float scaleY = (viewport.Height - 2 * BORDER_HEIGHT) / span.Y;
                zoom = Math.Min(scaleX, scaleY);

                // Normalize values if necessary
                zoom = zoom > MAX_ZOOM ? MAX_ZOOM : zoom;
                zoom = zoom < MIN_ZOOM ? MIN_ZOOM : zoom;
                #endregion

                Vector2 cameraPos = This.Game.CurrentLevel.Camera.Pos;
                #region Shift Viewport
                Vector2 topLeftCorner = min - cameraPos;
                Vector2 bottomRightCorner = max - cameraPos;
                if (scaleX >= MIN_ZOOM)
                {
                    if (topLeftCorner.X < viewport.X + BORDER_WIDTH / zoom)
                    {
                        cameraPos.X += topLeftCorner.X - (viewport.X + BORDER_WIDTH) / zoom;
                    }
                    else if (bottomRightCorner.X > viewport.X + (viewport.Width - BORDER_WIDTH) / zoom)
                    {
                        cameraPos.X += bottomRightCorner.X - (viewport.X + (viewport.Width - BORDER_WIDTH) / zoom);
                    }
                }
                if (scaleY >= MIN_ZOOM)
                {
                    if (topLeftCorner.Y < viewport.Y + BORDER_HEIGHT / zoom)
                    {
                        cameraPos.Y += topLeftCorner.Y - (viewport.Y + BORDER_HEIGHT) / zoom;
                    }
                    else if (bottomRightCorner.Y > viewport.Y + (viewport.Height - BORDER_HEIGHT) / zoom)
                    {
                        cameraPos.Y += bottomRightCorner.Y - (viewport.Y + (viewport.Height - BORDER_HEIGHT) / zoom);
                    }
                }
                #endregion

                This.Game.CurrentLevel.Camera.Pos = cameraPos;
                This.Game.CurrentLevel.Camera.Zoom = zoom;
            }
        }

        internal override void Update()
        {
            base.Update();

            RealignViewport();

            Vector3 cameraPosition = new Vector3(Camera.Pos, 0);
            Viewport viewport = This.Game.GraphicsDevice.Viewport;
            float zoom = This.Game.CurrentLevel.Camera.Zoom;

            StartDraw = (cameraPosition + new Vector3(viewport.X, viewport.Y, 0)) / Tile.TileSize;
            EndDraw = (cameraPosition + new Vector3(viewport.X + viewport.Width / zoom,
                                                viewport.Y + viewport.Height / zoom, 0)) / Tile.TileSize;
            foreach (ParticleEmitter emitter in mWorldObjects.FindAll(delegate(WorldObject o) { return o is ParticleEmitter; }))
            {
                createParticlesInCircle(emitter, 10, 150, new Vector2(200, 200));
                //createParticlesLikeFlamingRing(emitter2, 10, 150, new Vector2(150, 100));
                //createParticlesLikeFlame(emitter3, 10, new Vector2(-150, 100));
                //createParticlesInCircle(emitter4, 10, 125, new Vector2(200, -200));
            }
        }

        internal override void Draw(GameTime gameTime, bool drawAfter = false)
        {
            #region Draw Base Tiles
            //draw base tiles
            This.Game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.GetTransformation(This.Game.GraphicsDevice));

            List<Tile> drawLater = new List<Tile>();
            for (int y = (int)Math.Floor(StartDraw.Y); y < (int)Math.Ceiling(EndDraw.Y); y++)
            {
                for (int x = (int)Math.Floor(StartDraw.X); x < (int)Math.Ceiling(EndDraw.X); x++)
                {
                    Tile toDraw;
                    TileMap.TryGetValue(x, y, out toDraw);

                    toDraw.Draw();
                    if (!(toDraw.Type == TileTypes.Bottom || toDraw.Type == TileTypes.BottomConvexCorner))
                        toDraw.Draw();
                    else
                        drawLater.Add(toDraw);
                }
            }
            This.Game.spriteBatch.End();
            #endregion

            base.Draw(gameTime, true);

            This.Game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.GetTransformation(This.Game.GraphicsDevice));

            //draw bottom tiles
            foreach (Tile tile in drawLater)
            {
                tile.Draw();
            }

            This.Game.spriteBatch.End();

            #region Draw Static Sprites
            if (staticSprites.Count > 0)
            {
                This.Game.spriteBatch.Begin();

                foreach (var sprite in staticSprites)
                {
                    sprite.Draw(gameTime);
                }

                This.Game.spriteBatch.End();
            }
            #endregion

        }


        private void createParticlesInCircle(ParticleEmitter emitter, int maxNumToCreate, float radius, Vector2 circleOrigin)
        {
            Random rand = new Random(This.gameTime.TotalGameTime.Milliseconds);
            double positionAngle = ((This.gameTime.TotalGameTime.TotalMilliseconds % 8000.0) / 8000.0) * Math.PI * 2;
            Vector2 position = new Vector2((float)Math.Cos(positionAngle) * radius, (float)Math.Sin(positionAngle) * radius) + circleOrigin;

            for (int i = 0; i < maxNumToCreate; i++)
            {
                double velocityAngle = rand.NextDouble() * Math.PI * 2;
                float velocitySpeed = rand.Next(2, 15);
                double accelAngle = rand.NextDouble() * Math.PI * 2;
                float accelSpeed = rand.Next(2, 15);
                emitter.createParticles(new Vector2((float)Math.Cos(velocityAngle) * velocitySpeed, (float)Math.Sin(velocityAngle) * velocitySpeed),
                                new Vector2((float)Math.Cos(accelAngle) * accelSpeed, (float)Math.Sin(accelAngle) * accelSpeed),
                                position,
                                rand.Next(5, 20),
                                rand.Next(1000, 4000));
            }
        }

        private void createParticlesLikeFlame(ParticleEmitter emitter, int maxNumToCreate, Vector2 flameOrigin)
        {
            Random rand = new Random(This.gameTime.TotalGameTime.Milliseconds);
            Vector2 position = new Vector2((float)rand.Next(0, 20), (float)rand.Next(0, 20)) + flameOrigin;

            for (int i = 0; i < maxNumToCreate; i++)
            {
                double velocityAngle = rand.NextDouble() * Math.PI * .5 + Math.PI / 4;
                float velocitySpeed = rand.Next(2, 15);
                double accelAngle = rand.NextDouble() * Math.PI * .25 + (Math.PI * 3) / 8;
                float accelSpeed = rand.Next(2, 15);
                emitter.createParticles(new Vector2((float)Math.Cos(velocityAngle) * velocitySpeed, (float)Math.Sin(velocityAngle) * velocitySpeed),
                                new Vector2((float)Math.Cos(accelAngle) * accelSpeed, (float)Math.Sin(accelAngle) * accelSpeed),
                                position,
                                rand.Next(5, 20),
                                rand.Next(1000, 4500));
            }
        }

        private void createParticlesLikeFlamingRing(ParticleEmitter emitter, int maxNumToCreate, float radius, Vector2 flameOrigin)
        {
            Random rand = new Random(This.gameTime.TotalGameTime.Milliseconds);

            for (int i = 0; i < maxNumToCreate; i++)
            {
                double positionAngle = rand.NextDouble() * Math.PI * 2;
                Vector2 position = new Vector2((float)Math.Cos(positionAngle) * radius, (float)Math.Sin(positionAngle) * radius) + flameOrigin;
                double velocityAngle = rand.NextDouble() * Math.PI * .25 + (Math.PI * 3) / 8;
                float velocitySpeed = rand.Next(2, 15);
                double accelAngle = rand.NextDouble() * Math.PI * .25 + (Math.PI * 3) / 8;
                float accelSpeed = rand.Next(2, 15);
                emitter.createParticles(new Vector2((float)Math.Cos(velocityAngle) * velocitySpeed, (float)Math.Sin(velocityAngle) * velocitySpeed),
                                new Vector2((float)Math.Cos(accelAngle) * accelSpeed, (float)Math.Sin(accelAngle) * accelSpeed),
                                position,
                                rand.Next(5, 20),
                                rand.Next(1000, 4000));
            }
        }

    }
}
