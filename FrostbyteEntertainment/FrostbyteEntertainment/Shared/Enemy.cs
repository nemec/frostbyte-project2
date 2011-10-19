using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Xml.Linq;

namespace Frostbyte
{
#if LEVELEDITOR
    //seen by editor
    internal class Enemy
    {
        
    
#else
    //seen by other things
    internal abstract partial class Enemy : Sprite
    {
        
#endif
    //seen by both
    }
}
