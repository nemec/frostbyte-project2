using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Input;

namespace Frostbyte.Levels
{
    internal static class Test
    {
        internal static void Load()
        {
            Collision.Lists.Add(new KeyValuePair<int, int>(0, 1));

            FrostbyteLevel l = (This.Game.CurrentLevel != This.Game.NextLevel && This.Game.NextLevel != null ? This.Game.NextLevel : This.Game.CurrentLevel) as FrostbyteLevel;

            l.TileMap = new TileList(XDocument.Load(@"Content/Level1.xml"));

            /*LevelFunctions.Spawn(delegate()
            {
                return new FerociousEnemy("e1", new Actor(new DummyAnimation("enemy", 10, 10)), 1f, 10);
            }, 10, new Microsoft.Xna.Framework.Vector2(50, 50));

            LevelFunctions.Spawn(delegate()
            {
                return new TestObstacle("e1", new Actor(new DummyAnimation("obstacle", 10, 10)));
            }, 3, new Microsoft.Xna.Framework.Vector2(50, 50));

            LevelFunctions.Spawn(delegate()
            {
                return new TestAlly("e1", new Actor(new DummyAnimation("ally", 10, 10)));
            }, 2, new Microsoft.Xna.Framework.Vector2(50, 50));*/

            Sprite ally = new TestAlly("a1", new Actor(new DummyAnimation("ally", 10, 10)));
            ally.Pos = new Vector2(250, 260);

            Characters.Mage mage = new Characters.Mage("mage", new Actor(new Animation("shield_opaque.anim")));
            mage.Pos = new Microsoft.Xna.Framework.Vector2(650, 250);
            //l.Camera.Pos = mage.Pos - new Microsoft.Xna.Framework.Vector2(This.Game.GraphicsDevice.Viewport.Width / 2,
            //    This.Game.GraphicsDevice.Viewport.Height / 2);
            /*Sprite a = new Sprite("box1", new Actor(new Animation("boxen.anim")),2);

            This.Game.CurrentLevel.AddAnimation(new Animation("boxen.anim"));
            Actor act = new Actor(l.GetAnimation("boxen.anim"));
            Sprite a = new Sprite("box1", act,0);
            a.Pos = new Vector2(15,0);
            This.Game.CurrentLevel.AddAnimation(new Animation("boxen2.anim"));
            act = new Actor(l.GetAnimation("boxen2.anim"));
            Sprite b = new Sprite("box2", act,1);
            b.UpdateBehavior = delegate()
            {
                var key = Keyboard.GetState();
                if(key.IsKeyDown(Keys.A))
                b.Pos.X -= 1;
                if(key.IsKeyDown(Keys.D))
                b.Pos.X += 1;
                if(key.IsKeyDown(Keys.W))
                b.Pos.Y -= 1;
                if(key.IsKeyDown(Keys.S))
                b.Pos.Y += 1;
            };
            Sprite b = new Sprite("box2", new Actor(new Animation("boxen.anim")),3);*/

        }

        internal static void Update()
        {

        }
    }

    internal class Target : Polygon
    {
        internal Target(string name)
            : this(name, 15)
        {
        }

        internal Target(string name, int size)
            : base(name, new Actor(new DummyAnimation("target", size, size)), Color.Red,
                new Vector3(0, 0, 0),
                new Vector3(size, 0, 0),
                new Vector3(size, size, 0),
                new Vector3(0, size, 0),
                new Vector3(0, 0, 0))
        {
        }
    }

    internal class TestAlly : Player
    {
        internal TestAlly(string name, Actor actor)
            : base(name, actor)
        {
            float height = This.Game.GraphicsDevice.Viewport.Height;
            float width = This.Game.GraphicsDevice.Viewport.Width;
            basicEffect.View = Matrix.CreateLookAt(new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, -10),
                                                   new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, 0), new Vector3(0, -1, 0));
            basicEffect.Projection = Matrix.CreateOrthographic(This.Game.GraphicsDevice.Viewport.Width, This.Game.GraphicsDevice.Viewport.Height, 1, 20);
            basicEffect.VertexColorEnabled = true;
        }

        BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);
        VertexPositionColor[] points;

        internal override void Draw(GameTime gameTime)
        {
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                int size = 10;
                points = new VertexPositionColor[5]{
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.Green),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y, 0), Color.Green),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y + size, 0), Color.Green),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y + size, 0), Color.Green),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.Green)};
                pass.Apply();
                basicEffect.World = Matrix.CreateTranslation(new Vector3(Pos, 0)) * This.Game.CurrentLevel.Camera.GetTransformation(This.Game.GraphicsDevice);
                This.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, points, 0, points.Length - 1);
            }
        }
    }

    internal class TestObstacle : Obstacle
    {
        internal TestObstacle(string name, Actor actor)
            : base(name, actor)
        {
            float height = This.Game.GraphicsDevice.Viewport.Height;
            float width = This.Game.GraphicsDevice.Viewport.Width;
            basicEffect.View = Matrix.CreateLookAt(new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, -10),
                                                   new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, 0), new Vector3(0, -1, 0));
            basicEffect.Projection = Matrix.CreateOrthographic(This.Game.GraphicsDevice.Viewport.Width, This.Game.GraphicsDevice.Viewport.Height, 1, 20);
            basicEffect.VertexColorEnabled = true;
        }

        BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);
        VertexPositionColor[] points;

        internal override void Draw(GameTime gameTime)
        {
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                int size = 10;
                points = new VertexPositionColor[5]{
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.LightCyan),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y, 0), Color.LightCyan),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y + size, 0), Color.LightCyan),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y + size, 0), Color.LightCyan),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.LightCyan)};
                pass.Apply();
                basicEffect.World = Matrix.CreateTranslation(new Vector3(Pos, 0)) * This.Game.CurrentLevel.Camera.GetTransformation(This.Game.GraphicsDevice);
                This.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, points, 0, points.Length - 1);
            }
        }
    }

    internal class FerociousEnemy : Frostbyte.Enemies.Enemy
    {
        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 0);


        #endregion Variables

        internal FerociousEnemy(string name, Actor actor, float speed, int health)
            : base(name, actor, speed, health)
        {
            //moved here because it was hiding parent variable
            movementStartTime = new TimeSpan(0, 0, 1);
            float height = This.Game.GraphicsDevice.Viewport.Height;
            float width = This.Game.GraphicsDevice.Viewport.Width;
            basicEffect.View = Matrix.CreateLookAt(new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, -10),
                                                   new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, 0), new Vector3(0, -1, 0));
            basicEffect.Projection = Matrix.CreateOrthographic(This.Game.GraphicsDevice.Viewport.Width, This.Game.GraphicsDevice.Viewport.Height, 1, 20);
            basicEffect.VertexColorEnabled = true;
        }

        BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);
        VertexPositionColor[] points;

        internal override void Draw(GameTime gameTime)
        {
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                int size = 10;
                points = new VertexPositionColor[5]{
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.BlueViolet),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y, 0), Color.BlueViolet),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y + size, 0), Color.BlueViolet),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y + size, 0), Color.BlueViolet),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.BlueViolet)};
                pass.Apply();
                basicEffect.World = Matrix.CreateTranslation(new Vector3(Pos, 0)) * This.Game.CurrentLevel.Camera.GetTransformation(This.Game.GraphicsDevice);
                if (mVisible) {
                    This.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, points, 0, points.Length - 1);
                }
            }
        }

        protected override void updateMovement()
        {
            Sprite Player1 = This.Game.CurrentLevel.GetSpritesByType("Mage")[0];
         // Sprite Player2 = This.Game.CurrentLevel.GetSpritesByType("Mage")[1];


            // Tests for Movement Patterns
            // Ram for 5 sec, charge for 5 sec, stealth charge for five sec, stealth camp 10 sec, retreat indefinitely
            if (!changeState && (This.gameTime.TotalGameTime - movementStartTime < new TimeSpan(0, 0, 5)))
                changeState = ram(Player1.Pos, Vector2.Zero, new TimeSpan(0, 0, 2), 1000f, 2.0f);

            else if (!changeState && (This.gameTime.TotalGameTime - movementStartTime < new TimeSpan(0, 0, 10)))
                changeState = charge(Player1.Pos, Vector2.Zero, new TimeSpan(0, 0, 3), 1000f, 1.1f);

            else if (!changeState && (This.gameTime.TotalGameTime - movementStartTime < new TimeSpan(0, 0, 15)))
                changeState = stealthCharge(Player1.Pos, Vector2.Zero, new TimeSpan(0, 0, 2), 1000f, 30f, 1.1f);

            else if (!changeState && (This.gameTime.TotalGameTime - movementStartTime < new TimeSpan(0, 0, 25)))
                changeState = stealthCamp(Player1.Pos, Vector2.Zero, 30f);

            else if (!changeState)
                changeState = retreat(Player1.Pos, Vector2.Zero, new TimeSpan(0, 0, 0, 0, 5), 50f, 2.0f);

            else
                changeState = !freeze(idleTime);
            
        }

        protected override void updateAttack()
        {
           // throw new NotImplementedException();
        }
    }
}
