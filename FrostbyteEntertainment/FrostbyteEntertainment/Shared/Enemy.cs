using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;

namespace Frostbyte
{
#if LEVELEDITOR
    //seen by editor
    internal partial class Enemy
    {
        public string Name { get; set; }

        /// <summary>
        /// the sprite's speed
        /// </summary>
        internal float Speed { get; set; }

#else
    //seen by other things
    internal abstract partial class Enemy : Sprite
    {
        
#endif
        //seen by both

        /// <summary>
        /// Health of the enemy
        /// </summary>
        internal float Health { get; set; }

        /// <summary>
        /// Turns the object into a line of xml
        /// </summary>
        /// <returns>XML representing the object</returns>
        internal virtual XElement ToXML()
        {
            return new XElement("Enemy");
        }
    }
}
