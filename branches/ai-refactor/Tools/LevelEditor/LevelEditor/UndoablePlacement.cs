using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frostbyte;

namespace LevelEditor
{
    public class UndoableAction
    {
        /// <summary>
        /// objects that were added/removed
        /// </summary>
        public List<LevelObject> Objects { get; set; }

        /// <summary>
        /// determines whether or not the item was added when put on undo or removed
        /// </summary>
        public bool Added { get; set; }

        public List<Frostbyte.Tile> OldState { get; set; }
    }
}
