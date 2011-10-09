using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frostbyte.Levels
{
    internal static class Test
    {
        internal static void Load()
        {
            FrostbyteLevel l = (This.Game.CurrentLevel != This.Game.NextLevel && This.Game.NextLevel != null ? This.Game.NextLevel : This.Game.CurrentLevel) as FrostbyteLevel;

            LevelFunctions.Spawn(delegate()
            {
                return new FerociousEnemy("e1", new Actor(new DummyAnimation("e1")), 1f, 10);
            }, 10, new Microsoft.Xna.Framework.Vector2(50, 50));

            LevelFunctions.Spawn(delegate()
            {
                return new TestObstacle("e1", new Actor(new DummyAnimation("e1")));
            }, 3, new Microsoft.Xna.Framework.Vector2(50, 50));

            LevelFunctions.Spawn(delegate()
            {
                return new TestAlly("e1", new Actor(new DummyAnimation("e1")));
            }, 2, new Microsoft.Xna.Framework.Vector2(50, 50));

            Characters.Mage mage = new Characters.Mage("mage", new Actor(new DummyAnimation("mage")));
            mage.Pos = new Microsoft.Xna.Framework.Vector2(50, 50);
            l.Camera.Pos = mage.Pos - new Microsoft.Xna.Framework.Vector2(This.Game.GraphicsDevice.Viewport.Width / 2,
                This.Game.GraphicsDevice.Viewport.Height / 2);

        }

        internal static void Update()
        {

        }
    }

    internal class Target : Sprite
    {
        internal Target(string name, Actor actor)
            : base(name, actor)
        {

        }

        BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);
        
        internal override void Draw(GameTime gameTime)
        {
            if (mVisible)
            {
                float height = This.Game.GraphicsDevice.Viewport.Height;
                float width = This.Game.GraphicsDevice.Viewport.Width;
                basicEffect.View = Matrix.CreateLookAt(new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, -10),
                                                       new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, 0), new Vector3(0, -1, 0));
                basicEffect.Projection = Matrix.CreateOrthographic(This.Game.GraphicsDevice.Viewport.Width, This.Game.GraphicsDevice.Viewport.Height, 1, 20);
                basicEffect.VertexColorEnabled = true;
                basicEffect.World = Matrix.CreateTranslation(new Vector3(Pos, 0)) * This.Game.CurrentLevel.Camera.GetTransformation(This.Game.GraphicsDevice);
                int size = 15;
                VertexPositionColor[] points = new VertexPositionColor[5]{
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.Red),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y, 0), Color.Red),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y + size, 0), Color.Red),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y + size, 0), Color.Red),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.Red)};
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    This.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, points, 0, points.Length - 1);
                }
            }
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
