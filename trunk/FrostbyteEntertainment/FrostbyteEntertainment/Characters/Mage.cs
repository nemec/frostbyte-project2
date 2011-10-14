using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Frostbyte.Characters
{
    class Mage : Player
    {
        enum TargetAlignment
        {
            Ally,
            Enemy,
            None
        }

        #region Constructors
        public Mage(string name, Actor actor)
            : this(name, actor, PlayerIndex.One)
        {

        }

        internal Mage(string name, Actor actor, PlayerIndex input)
            : base(name, actor)
        {
            //controller = new GamePadController(input);
            controller = new KeyboardController();
            keyboard = new KeyboardController();
            currentTargetAlignment = TargetAlignment.None;
            target = new Frostbyte.Levels.Target("target");
            target.mVisible = false;
            sortType = new DistanceSort(this);

            UpdateBehavior = mUpdate;
        }
        #endregion

        #region Variables
        private Sprite currentTarget = null;
        private TargetAlignment currentTargetAlignment;
        private IController controller;
        private IController keyboard;
        private Sprite target;
        BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);
        private IComparer<Sprite> sortType;
        #endregion

        #region Methods
        /// <summary>
        /// Finds the closest enemy sprite to the player that's further than the current target
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Sprite findMinimum(List<Sprite> list)
        {
            if (list.Contains(this))
            {
                list.Remove(this);
            }
            list.Sort(sortType);

            int next = list.IndexOf(currentTarget);
            for (int x = 0; x < list.Count; x++)
            {
                Sprite target = list[(next + 1 + x) % list.Count];
                if (target.mVisible)
                {
                    return target;
                }
            }

            cancelTarget(); // Every sprite in list is invisible, there's nothing to target
            return null;
        }

        private void cancelTarget()
        {
            target.mVisible = false;
            currentTarget = null;
            currentTargetAlignment = TargetAlignment.None;
        }

        public void mUpdate()
        {
            controller.Update();

            if (currentTarget != null && !currentTarget.mVisible)
            {
                cancelTarget();
            }

            if (controller.IsConnected)
            {
                #region Targeting
                if (controller.TargetEnemies)
                {
                    if (currentTargetAlignment != TargetAlignment.Enemy)
                    {
                        currentTarget = null;
                    }

                    currentTarget = findMinimum((This.Game.CurrentLevel as FrostbyteLevel).enemies);

                    if (currentTarget != null)
                    {
                        currentTargetAlignment = TargetAlignment.Enemy;
                    }
                }
                else if (controller.TargetAllies)
                {
                    if (currentTargetAlignment != TargetAlignment.Ally)
                    {
                        currentTarget = null;
                    }

                    currentTarget = findMinimum((This.Game.CurrentLevel as FrostbyteLevel).allies.Concat(
                        (This.Game.CurrentLevel as FrostbyteLevel).obstacles).ToList());
                    if (currentTarget != null)
                    {
                        currentTargetAlignment = TargetAlignment.Ally;
                    }
                }

                if (controller.CancelTargeting == ReleasableButtonState.Clicked)
                {
                    cancelTarget();
                }

                if (currentTarget != null)
                {
                    target.mVisible = true;
                    target.CenterOn(currentTarget);
                }
                #endregion Targeting

                #region Movement

                Pos.X += controller.Movement.X * 3;
                Pos.Y -= controller.Movement.Y * 3;

                #endregion Movement
            }
            else
            {
                #region Movement

                keyboard.Update();
                Pos.X += keyboard.Movement.X * 3;
                Pos.Y += keyboard.Movement.Y * 3;

                #endregion Movement
            }
        }

        /*internal override void Draw(GameTime gameTime)
        {

            float height = This.Game.GraphicsDevice.Viewport.Height;
            float width = This.Game.GraphicsDevice.Viewport.Width;
            basicEffect.View = Matrix.CreateLookAt(new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, -10),
                                                   new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, 0), new Vector3(0, -1, 0));
            basicEffect.Projection = Matrix.CreateOrthographic(This.Game.GraphicsDevice.Viewport.Width, This.Game.GraphicsDevice.Viewport.Height, 1, 20);
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.CreateTranslation(new Vector3(Pos, 0)) * This.Game.CurrentLevel.Camera.GetTransformation(This.Game.GraphicsDevice);
            int size = 10;
            VertexPositionColor[] points = new VertexPositionColor[4]{
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.AliceBlue),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y, 0), Color.AliceBlue),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y + size, 0), Color.AliceBlue),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y + size, 0), Color.AliceBlue)};
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                This.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, points, 0, 1);
            }
        }*/
        #endregion
    }
}
