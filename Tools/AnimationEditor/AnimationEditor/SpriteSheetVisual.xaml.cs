using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnimationEditor
{
	/// <summary>
	/// Interaction logic for SpriteSheetVisual.xaml
	/// </summary>
	public partial class SpriteSheetVisual : UserControl
	{
        public string FileName { get; set; }

        public List<Frame> Frames { get; set; }

        public int FrameCount { get { return Frames != null ? Frames.Count : 0; } }

		public SpriteSheetVisual()
		{
			this.InitializeComponent();
		}
	}
}