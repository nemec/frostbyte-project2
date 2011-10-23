using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal class ProgressBar : Sprite
    {
        internal ProgressBar(string name, int maxValue, Color borderColor, Color fillColor, Color backgroundColor)
            : this(name, maxValue, borderColor, fillColor, backgroundColor, new Vector2(100, 20))
        {
        }

        internal ProgressBar(string name, int maxValue, Color borderColor, Color fillColor, Color backgroundColor, Vector2 size)
            : base(name, new Actor(new DummyAnimation()))
        {
            this.MaxValue = maxValue;
            this.size = size;
            this.innerSize = size - Vector2.One * 2 * borderSize;

            Viewport viewport = This.Game.GraphicsDevice.Viewport;
            basicEffect.View = Matrix.CreateLookAt(
                new Vector3(viewport.X + viewport.Width / 2,
                    viewport.Y + viewport.Height / 2, -1),
                new Vector3(
                    viewport.X + viewport.Width / 2,
                    viewport.Y + viewport.Height / 2,
                    0),
                Vector3.Down);

            basicEffect.Projection = Matrix.CreateOrthographic(
                viewport.Width,
                viewport.Height, 1, 2);

            basicEffect.VertexColorEnabled = true;

            border = new VertexPositionColor[5] {
                new VertexPositionColor(new Vector3(0, 0, 0), borderColor),
                    new VertexPositionColor(new Vector3(size.X, 0, 0), borderColor),
                    new VertexPositionColor(new Vector3(0, size.Y, 0), borderColor),
                    new VertexPositionColor(new Vector3(size.X, size.Y, 0), borderColor),
                    new VertexPositionColor(new Vector3(size.X, 0, 0), borderColor)
            };
            this.fillColor = fillColor;

            background = new VertexPositionColor[5]{
                new VertexPositionColor(new Vector3(borderSize, borderSize, 0), backgroundColor),
                    new VertexPositionColor(new Vector3(innerSize.X + borderSize, borderSize, 0), backgroundColor),
                    new VertexPositionColor(new Vector3(borderSize, innerSize.Y + borderSize, 0), backgroundColor),
                    new VertexPositionColor(new Vector3(innerSize.X + borderSize, innerSize.Y + borderSize, 0), backgroundColor),
                    new VertexPositionColor(new Vector3(innerSize.X + borderSize, borderSize, 0), backgroundColor)
            };
        }

        private int borderSize = 2;
        private Vector2 size;
        private Vector2 innerSize;

        internal int MaxValue { get; set; }
        private int mValue;
        internal int Value {
            get { return mValue; }
            set {
                mValue = value < 0 ? 0 :
                    (value > MaxValue ? MaxValue :
                        value);
            } 
        }
        private BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);

        Color fillColor;
        VertexPositionColor[] border;
        VertexPositionColor[] fill;
        VertexPositionColor[] background;

        /// <summary>
        /// Add current position to each point's offset
        /// </summary>
        /// <param name="points"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private VertexPositionColor[] positionVertices(VertexPositionColor[] points, Vector3 position)
        {
            VertexPositionColor[] positionedPoints = points.Clone() as VertexPositionColor[];
            for (int x = 0; x < positionedPoints.Length; x++)
            {
                positionedPoints[x].Position += position;
            }
            return positionedPoints;
        }

        internal override void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                basicEffect.World = Matrix.CreateTranslation(new Vector3(Pos, 0));
                if (!Static)
                {
                    basicEffect.World *= This.Game.CurrentLevel.Camera.GetTransformation(This.Game.GraphicsDevice);
                }

                // Needs to be updated depending on the current Value
                Vector2 fillProportion = new Vector2(innerSize.X * mValue / MaxValue, innerSize.Y);
                fill = new VertexPositionColor[5]{
                    new VertexPositionColor(new Vector3(borderSize, borderSize, 0), fillColor),
                    new VertexPositionColor(new Vector3(fillProportion.X + borderSize, borderSize, 0), fillColor),
                    new VertexPositionColor(new Vector3(borderSize, fillProportion.Y + borderSize, 0), fillColor),
                    new VertexPositionColor(new Vector3(fillProportion.X + borderSize, fillProportion.Y + borderSize, 0), fillColor),
                    new VertexPositionColor(new Vector3(fillProportion.X + borderSize, borderSize, 0), fillColor)
                };

                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Vector3 Pos3d = new Vector3(Pos, 0);
                    VertexPositionColor[] positionedPoints;

                    positionedPoints = positionVertices(border, Pos3d);
                    This.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.TriangleStrip,
                        positionedPoints, 0, 2);

                    positionedPoints = positionVertices(background, Pos3d);
                    This.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.TriangleStrip,
                        positionedPoints, 0, 2);

                    positionedPoints = positionVertices(fill, Pos3d);
                    This.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.TriangleStrip,
                        positionedPoints, 0, 2);
                }
            }
        }
    }
}
