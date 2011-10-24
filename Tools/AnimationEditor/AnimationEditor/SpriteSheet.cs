using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AnimationEditor
{
    public class SpriteSheet
    {
        public string Name { get; set; }

        public List<Frame> Frames { get; set; }

        public int FrameCount { get { return Frames != null ? Frames.Count : 0; } }
    }
}
