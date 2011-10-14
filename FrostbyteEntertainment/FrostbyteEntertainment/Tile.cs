using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

/// \file Tile.cs This is Shared with the Level Editor
namespace Frostbyte
{
    // REST OF CLASS LOCATED IN Shared/Tile.cs

    public partial class Tile : LevelObject
    {
        Texture2D Image { get; set; }

        /// <summary>
        /// The tiles's width
        /// </summary>
        internal int Width { get; set; }

        /// <summary>
        ///  The tiles's height
        /// </summary>
        internal int Height { get; set; }

        internal void Draw()
        {
            if (Image == null)
            {

            }

            //BitmapImage image = new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
            Texture2D image = This.Game.Content.Load<Texture2D>("corner");
            if (GridCell != null)
            {
                This.Game.spriteBatch.Draw(
                        image,
                        GridCell.Pos,
                        null,
                        Microsoft.Xna.Framework.Color.White,
                        0,
                        new Vector2(),
                        1,
                        SpriteEffects.None,
                        0
                    );
            }
        }
    }
}


