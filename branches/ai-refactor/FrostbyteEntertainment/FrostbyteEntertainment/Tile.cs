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
                This.Game.spriteBatch.Draw(image, GridCell.Pos, null,
                    Microsoft.Xna.Framework.Color.White, 0, new Vector2(), 1, SpriteEffects.None, 0);
            }
        }
    }
}


