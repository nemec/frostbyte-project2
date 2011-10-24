using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace AnimationEditor
{
    public class Frame
    {
        public Frame()
        {
            Collisions = new ObservableCollection<Collision>();
        }

        #region Properties
        /// <summary>
        /// File from which the Frame is obtained
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Image Texture
        /// </summary>
        public BitmapSource Image { get; set; }

        /// <summary>
        /// Amount of time to pause between this frame and next. (In Milliseconds)
        /// </summary>
        public long Pause { get; set; }

        /// <summary>
        /// The offeset from position to place
        /// the image. Defaults to (0,0)
        /// </summary>
        public Point AnimationPeg { get; set; }

        /// <summary>
        /// The frame's width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///  The frame's height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Position of the top left corner
        /// </summary>
        public Point StartPos { get; set; }

        /// <summary>
        /// Color that should be transparent
        /// </summary>
        public Color ClearColor { get; set; }

        public ObservableCollection<Collision> Collisions { get; set; }
        #endregion

        public override string ToString()
        {
            return string.Format("Pause:{0} milliseconds\nAnimation Peg at {1}\nClear color is {2}\nTop Left is at {3}\nWidth:{4}\nHeight:{5}\nFilename:{6}",Pause,AnimationPeg,ClearColor,StartPos,Width,Height,File);
        }
    }
}
