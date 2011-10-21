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
        internal ProgressBar(string name, int maxValue, Color barcolor, Color borderColor)
            : this(name, maxValue, barcolor, borderColor, new Vector2(100, 30))
        {
        }

        internal ProgressBar(string name, int maxValue, Color fillColor, Color borderColor, Vector2 size)
            : base(name, new Actor(new DummyAnimation()))
        {
            this.maxValue = maxValue;
            this.size = size;

            Viewport viewport = This.Game.GraphicsDevice.Viewport;
            basicEffect.View = Matrix.CreateLookAt(
                new Vector3(
                    viewport.X + viewport.Width / 2,
                    viewport.Y + viewport.Height / 2,
                    -10),
                new Vector3(
                    viewport.X + viewport.Width / 2,
                    viewport.Y + viewport.Height / 2, 0),
                    new Vector3(0, -1, 0));

            basicEffect.Projection = Matrix.CreateOrthographic(viewport.Width,
                viewport.Height, 1, 20);

            border = new VertexPositionColor[5] {
                new VertexPositionColor(new Vector3(0, 0, 0), borderColor),
                new VertexPositionColor(new Vector3(size.X, 0, 0), borderColor),
                new VertexPositionColor(new Vector3(size.X, size.Y, 0), borderColor),
                new VertexPositionColor(new Vector3(0, size.Y, 0), borderColor),
                new VertexPositionColor(new Vector3(0, 0, 0), borderColor)
            };
            this.fillColor = fillColor;
        }

        private Vector2 size;
        private int maxValue;
        private int mValue;
        internal int Value {
            get { return mValue; }
            set {
                mValue = value < 0 ? 0 :
                    (value > maxValue ? maxValue :
                        value);
                
            } 
        }
        private BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);

        VertexPositionColor[] border;
        Color fillColor;
        VertexPositionColor[] fill;

        internal override void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                basicEffect.World = Matrix.CreateTranslation(new Vector3(Pos, 0));
                if (!Static)
                {
                    basicEffect.World *= This.Game.CurrentLevel.Camera.GetTransformation(This.Game.GraphicsDevice);
                }

                Vector2 fillProportion = new Vector2(size.X * mValue / maxValue, size.Y);
                fill = new VertexPositionColor[7]{
                    new VertexPositionColor(new Vector3(0, 0, 0), fillColor),
                    new VertexPositionColor(new Vector3(fillProportion.X, fillProportion.Y, 0), fillColor),
                    new VertexPositionColor(new Vector3(0, fillProportion.Y, 0), fillColor),
                    new VertexPositionColor(new Vector3(0, 0, 0), fillColor),
                    new VertexPositionColor(new Vector3(fillProportion.X, 0, 0), fillColor),
                    new VertexPositionColor(new Vector3(fillProportion.X, fillProportion.Y, 0), fillColor),
                    new VertexPositionColor(new Vector3(0, 0, 0), fillColor),
                };

                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Vector3 Pos3d = new Vector3(Pos, 0);
                    List<VertexPositionColor> positionedPoints = new List<VertexPositionColor>();
                    foreach (VertexPositionColor point in border)
                    {
                        positionedPoints.Add(new VertexPositionColor(point.Position + Pos3d, point.Color));
                    }
                    This.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip,
                        positionedPoints.ToArray(),
                        0, border.Length - 1);

                    positionedPoints = new List<VertexPositionColor>();
                    foreach (VertexPositionColor point in fill)
                    {
                        positionedPoints.Add(new VertexPositionColor(point.Position + Pos3d, point.Color));
                    }
                    This.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip,
                        positionedPoints.ToArray(),
                        0, 5);//fill.Length - 1);
                }
            }
        }
    }
}
