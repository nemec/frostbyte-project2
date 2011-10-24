using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace AnimationEditor
{
    public class AnimFile
    {
        static int count = 0;

        public AnimFile()
        {
            Filename = string.Format("Untitled{0}", count++);
            Frames = new ObservableCollection<Frame>();
        }

        public AnimFile(ObservableCollection<Frame> frames)
        {
            Filename = string.Format("Untitled{0}", count++);
            Frames = frames;
        }

        public AnimFile(System.Windows.Controls.ItemCollection itemCollection)
        {
            Filename = string.Format("Untitled{0}", count++);
            Frames = new ObservableCollection<Frame>();
            foreach (var frame in itemCollection)
            {
                Frame f = frame as Frame;
                if (f != null)
                    Frames.Add(f);
            }
        }

        public AnimFile(ObservableCollection<Frame> frames, string filename)
        {
            Frames = frames;
            Filename = filename;
        }

        /// <summary>
        /// Name of anim file
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Animation's frames
        /// </summary>
        public ObservableCollection<Frame> Frames { get; set; }

        #region Methods
        public XDocument CreateAnimFile(string filename)
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("Animation"));
            foreach (Frame frame in Frames)
            {
                XElement elem = new XElement("Frame");
                string file = frame.File.EndsWith(".anim") || frame.File.EndsWith(".png") || frame.File.EndsWith(".jpg") || frame.File.EndsWith(".bmp") ? frame.File.Remove(frame.File.LastIndexOf('.')) : filename;
                elem.SetAttributeValue("SpriteSheet", file);
                elem.SetAttributeValue("FrameDelay", frame.Pause);
                elem.SetAttributeValue("Width", frame.Width);
                elem.SetAttributeValue("Height", frame.Height);
                elem.SetAttributeValue("TLPos", frame.StartPos);
                elem.SetAttributeValue("AnimationPeg", frame.AnimationPeg);
                //add collision data here
                foreach (var item in frame.Collisions)
                {
                    elem.Add(item.GetLine());
                }

                doc.Root.Add(elem);
            }
            return doc;
        }
        #endregion Methods
    }

    //public class FramesToViews : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        List<Frame> m = value as List<Frame>;
    //        if (m != null && m.Count != 0)
    //        {
    //            List<AnimationFrame> frames = new List<AnimationFrame>();
    //            foreach (var frame in m)
    //            {
    //                frames.Add(new AnimationFrame(frame));
    //            }
    //            return frames;
    //        }
    //        else
    //            return null;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        List<AnimationFrame> m = value as List<AnimationFrame>;
    //        if (m != null)
    //        {
    //            return m;
    //        }
    //        else
    //            return value;
    //    }
    //}
}
