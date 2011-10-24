using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace AnimationEditor
{
    public class CollisionCircle : Collision
    {
        public Point center;
        public double radius;

        public CollisionCircle(Point p, Ellipse circ)
        {
            center = p;
            radius = circ.Width;
        }

        public CollisionCircle(Point p, float r)
        {
            center = p;
            radius = r;
        }

        public override XElement GetLine()
        {
            XElement e = new XElement("Collision");
            e.SetAttributeValue("Type", "Circle");
            e.SetAttributeValue("Pos", center);
            e.SetAttributeValue("Radius", radius);
            return e;
        }
    }
}
