using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace AnimationEditor
{
    public class CollisionRect : Collision
    {
        public Point TL;
        public double Width;
        public double Height;

        public CollisionRect(Point p, double w, double h)
        {
            // TODO: Complete member initialization
            TL = p;
            Width = w;
            Height = h;
        }

        public CollisionRect(Point tl, Point br)
        {
            TL = tl;
            Width = br.X - tl.X;
            Height = br.Y - tl.Y;
        }

        public override XElement GetLine()
        {
            XElement e = new XElement("Collision");
            e.SetAttributeValue("Type", "Rectangle");
            e.SetAttributeValue("TLPos", TL);
            e.SetAttributeValue("BRPos", new Point(TL.X + Width, TL.Y + Height));
            return e;
        }
    }
}
