using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Win32;
using System.IO;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace AnimationEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<SpriteSheetVisual> SpriteFrameData { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            storage.mainWindow = this;
            SpriteFrameData = new ObservableCollection<SpriteSheetVisual>();
            CloseFrames.MouseDown += new MouseButtonEventHandler(CloseFrames_MouseDown);
            CopyFrames.MouseUp += new MouseButtonEventHandler(CopyFrames_MouseUp);
        }

        void CopyFrames_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SpriteSheetItems.SelectAll();
        }

        void ButtonClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public void ButtonClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var result = MessageBox.Show("Save File?", "Save", MessageBoxButton.YesNoCancel);
            if (result != MessageBoxResult.Cancel)
            {
                //save
                if (result == MessageBoxResult.Yes)
                {
                    AnimFile file = Files.SelectedItem as AnimFile;
                    if (file != null)
                        Save(file);
                }
                Files.Items.Remove((sender as Grid).DataContext);
            }
        }

        void CloseFrames_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SpriteSheets.SelectedIndex = -1;
        }

        public readonly static RoutedUICommand CommandCreateSpriteSheet;
        public readonly static RoutedUICommand CommandCreateAnimation;
        public readonly static RoutedUICommand CommandSaveAndExit;
        public readonly static RoutedUICommand CommandImportSpriteSheet;
        public readonly static RoutedUICommand CommandImportImage;

        static MainWindow()
        {
            CommandCreateSpriteSheet = new RoutedUICommand("CreateSpriteSheet",
                "Create Sprite Sheet", typeof(MainWindow));
            CommandCreateSpriteSheet.InputGestures.Add(
                new KeyGesture(Key.E, ModifierKeys.Control));

            CommandCreateAnimation = new RoutedUICommand("CreateAnimation",
                "Create Animation", typeof(MainWindow));
            CommandCreateAnimation.InputGestures.Add(
                new KeyGesture(Key.N, ModifierKeys.Control));

            CommandSaveAndExit = new RoutedUICommand("SaveAndExit",
                "Exit", typeof(MainWindow));
            CommandSaveAndExit.InputGestures.Add(
                new KeyGesture(Key.Q, ModifierKeys.Control));

            CommandImportSpriteSheet = new RoutedUICommand("ImportSpriteSheet",
                "Import Sprite Sheet", typeof(MainWindow));
            CommandImportSpriteSheet.InputGestures.Add(
                new KeyGesture(Key.U, ModifierKeys.Control));

            CommandImportImage = new RoutedUICommand("ImportImage",
                "Import Sprite Sheet", typeof(MainWindow));
            CommandImportImage.InputGestures.Add(
                new KeyGesture(Key.I, ModifierKeys.Control));
        }

        #region MenuFunctions
        private void Yes(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CanSave(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Files.HasItems;
        }

        public void LoadFromAnimFile(Object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog loadAnimDialog = new OpenFileDialog();
            loadAnimDialog.Title = "Select animation file to load.";
            loadAnimDialog.Multiselect = false;
            loadAnimDialog.Filter = "Animation files (*.anim)|*.anim|All files (*.*)|*.*";
            if (loadAnimDialog.ShowDialog() == true)
            {
                ParseAnimfile(loadAnimDialog.OpenFile(), loadAnimDialog.SafeFileName);
            }
        }

        public void SaveFile(Object sender, ExecutedRoutedEventArgs e)
        {
            AnimFile file = Files.SelectedItem as AnimFile;
            if (file != null)
                Save(file);
        }

        public void SaveFileAs(Object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Animation files(*.anim)|*.anim";
            AnimFile file = Files.SelectedItem as AnimFile;
            if (file != null && sfd.ShowDialog() == true)
            {
                XDocument doc = file.CreateAnimFile(sfd.SafeFileName);
                doc.Save(sfd.FileName);
            }
        }

        public void CreateAnimation(Object sender, ExecutedRoutedEventArgs e)
        {
            Files.Items.Add(new AnimFile());
        }

        public void CreateSpriteSheet(Object sender, ExecutedRoutedEventArgs e)
        {
            Files.Items.Add(new AnimFile());
        }

        public void SaveAndExit(Object sender, ExecutedRoutedEventArgs e)
        {
            foreach (TabItem file in Files.Items)
            {
                AnimFile animfile = (file.DataContext as AnimFile);
                if (file != null)
                    Save(animfile);
            }
            Application.Current.Shutdown();
        }

        public void ImportSpriteSheet(Object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog loadAnimDialog = new OpenFileDialog();
            loadAnimDialog.InitialDirectory = @"C:\Users\Ivan Lloyd\Dropbox\GameDesign\BodilyInfection\BodilyInfection\BodilyInfectionContent\Sprites";
            loadAnimDialog.Title = "Select image file to load.";
            loadAnimDialog.Multiselect = true;
            loadAnimDialog.Filter = "Image Files(*.spsh)|*.spsh|All files (*.*)|*.*";
            if (loadAnimDialog.ShowDialog() == true)
            {
                ParseSpriteSheet(loadAnimDialog.FileName);
            }
        }

        public void ImportImage(Object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog loadAnimDialog = new OpenFileDialog();
            loadAnimDialog.Title = "Select image file(s) to load.";
            loadAnimDialog.Multiselect = true;
            loadAnimDialog.Filter = "Image Files(*.BMP;*PNG;*.JPG;*.GIF)|*.BMP;*.PNG;*.JPG;*.GIF|All files (*.*)|*.*";
            if (loadAnimDialog.ShowDialog() == true)
            {
                List<Frame> frames = new List<Frame>();
                string[] fileNames = loadAnimDialog.SafeFileNames;
                int count = 0;
                foreach (var file in loadAnimDialog.OpenFiles())
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = file;
                    img.EndInit();
                    frames.Add(new Frame()
                    {
                        File = fileNames[count],
                        Image = img,
                        AnimationPeg = new Point(0, 0),
                        Height = img.PixelHeight,
                        Width = img.PixelWidth,
                        StartPos = new Point(0, 0),
                        Pause = 20,
                        ClearColor = Colors.Magenta
                    }
                    );
                    count++;
                }
                //add them to a new frameset
                AddSpriteSheet(frames, "images");
            }
        }
        #endregion

        #region Methods

        private void Save(AnimFile file)
        {
            if (file != null)
            {
                if (File.Exists(file.Filename))
                {
                    string name = file.Filename.EndsWith(".anim") ? file.Filename : string.Format("{0}.anim", file.Filename);
                    XDocument doc = file.CreateAnimFile(name);
                    doc.Save(name);
                    file.Filename = name;
                }
                else
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Animation files(*.anim)|*.anim";
                    if (sfd.ShowDialog() == true)
                    {
                        XDocument doc = file.CreateAnimFile(sfd.SafeFileName);
                        doc.Save(sfd.FileName);
                        file.Filename = sfd.FileName;
                    }
                }
            }
        }

        private void ParseAnimfile(Stream stream, string filename)
        {

            ObservableCollection<Frame> frames = new ObservableCollection<Frame>();
            XDocument doc = XDocument.Load(stream);
  //          try
            {
                string spritesheetfile;
                string spritesheetfilelast = "";
                string safename = "";
                foreach (var elem in doc.Descendants("Frame"))
                {
                    string file = elem.Attribute("SpriteSheet").Value;
                    spritesheetfile = string.Format("{0}.png", elem.Attribute("SpriteSheet").Value);

                    //find the file
                    if (!File.Exists(spritesheetfile))
                    {
                        if (spritesheetfile != safename)
                        {
                            OpenFileDialog ofd = new OpenFileDialog();
                            ofd.InitialDirectory = @"C:\Users\Ivan Lloyd\Dropbox\GameDesign\BodilyInfection\BodilyInfection\BodilyInfectionContent\Sprites";
                            ofd.Title = string.Format("Select Spritesheet for \"{0}\"", filename);
                            ofd.Multiselect = true;
                            ofd.Filter = "Image Files(*.BMP;*PNG;*.JPG;*.GIF)|*.BMP;*.PNG;*.JPG;*.GIF|All files (*.*)|*.*";
                            if (ofd.ShowDialog() == true)
                            {
                                spritesheetfile = ofd.FileName;
                                spritesheetfilelast = ofd.FileName;
                                safename = ofd.SafeFileName;
                            }
                        }
                        else
                        {
                            spritesheetfile = spritesheetfilelast;
                        }
                    }

                    BitmapImage spritesheet = new BitmapImage();
                    spritesheet.BeginInit();
                    spritesheet.StreamSource = new StreamReader(spritesheetfile).BaseStream;
                    spritesheet.EndInit();

                    Point TL = Point.Parse(elem.Attribute("TLPos").Value);
                    int w = int.Parse(elem.Attribute("Width").Value);
                    int h = int.Parse(elem.Attribute("Height").Value);

                    CroppedBitmap img = new CroppedBitmap(spritesheet, new Int32Rect((int)TL.X, (int)TL.Y, w, h));

                    ObservableCollection<Collision> cols = new ObservableCollection<Collision>();

                    foreach (var collision in elem.Descendants("Collision"))
                    {
                        if (collision.Attribute("Type").Value == "Circle")
                        {
                            cols.Add( 
                                new CollisionCircle(
                                    Point.Parse(collision.Attribute("Pos").Value),
                                    float.Parse(collision.Attribute("Radius").Value)
                                )
                            );
                        }
                        else if (collision.Attribute("Type").Value == "Rectangle")
                        {
                            cols.Add(
                                new CollisionRect(
                                    Point.Parse(collision.Attribute("TLPos").Value),
                                    Point.Parse(collision.Attribute("BRPos").Value)
                                )
                            );
                        }
                    }

                    frames.Add(new Frame()
                        {
                            File = file,
                            Pause = long.Parse(elem.Attribute("FrameDelay").Value),
                            Width = w,
                            Height = h,
                            StartPos = TL,
                            AnimationPeg = Point.Parse(elem.Attribute("AnimationPeg").Value),
                            Image = img,
                            Collisions = cols
                        }
                    );
                }
                Files.Items.Add(new AnimFile(frames, filename));
            }
            //catch (Exception e)
            //{
            //    MessageBox.Show(string.Format("Exception: {0}\n", e.GetBaseException().Message));
            //}
        }

        private void ParseSpriteSheet(string FileName)
        {
            List<Frame> frames = new List<Frame>();
            string spritesheetfile = FileName.Remove(FileName.LastIndexOf("."));
            int count = 0;

            BitmapImage spritesheet = new BitmapImage();
            spritesheet.BeginInit();
            spritesheet.StreamSource = new StreamReader(spritesheetfile).BaseStream;
            spritesheet.EndInit();

            XDocument doc = XDocument.Load(FileName);
            foreach (var frame in doc.Descendants("Frame"))
            {
                int h = int.Parse(frame.Attribute("Height").Value);
                int w = int.Parse(frame.Attribute("Width").Value);
                Point TL = Point.Parse(frame.Attribute("TLPos").Value);
                CroppedBitmap img = new CroppedBitmap(spritesheet, new Int32Rect((int)TL.X, (int)TL.Y, w, h));
                string file = FileName.Substring(FileName.LastIndexOf('\\') + 1);
                file = file.Remove(file.LastIndexOf('.'));
                frames.Add(new Frame()
                {
                    File = file,
                    Image = img,
                    AnimationPeg = new Point(0, 0),
                    Height = h,
                    Width = w,
                    StartPos = TL,
                    Pause = 20,
                    ClearColor = Colors.Magenta
                }
                );
                count++;
            }
            //if (Files.HasItems && Files.Items.Count > 0)
            //{
            //    Files.Items.MoveCurrentToFirst();
            //    (Files.SelectedItem as TabItem).DataContext = new AnimFile(frames);
            //}
            //else
            //{
            //    Files.Items.Clear();
            //    FileAnimationEditor fae = new FileAnimationEditor();
            //    fae.DataContext = new AnimFile(frames);
            //    Files.ItemsSource = new List<FileAnimationEditor>() { fae };
            //    Files.SelectedIndex = 0;
            //}

            /*SpriteFrameData.Add*/
            AddSpriteSheet(frames, spritesheetfile);
        }

        private void AddSpriteSheet(List<Frame> frames, string spritesheetfile)
        {
            List<SpriteSheetVisual> lss = new List<SpriteSheetVisual>();
            SpriteSheetVisual ssv = new SpriteSheetVisual() { Frames = frames, FileName = spritesheetfile };
            lss.Add(ssv);
            foreach (var item in SpriteSheets.Items)
            {
                SpriteSheetVisual s = item as SpriteSheetVisual;
                if (s != null)
                {
                    lss.Add(s);
                }
            }
            SpriteSheets.ItemsSource = lss;
            SpriteSheets.SelectedIndex = -1;
        }

        #region PropertyChanged

        #endregion
        #endregion Methods

        #region Image
        void SaveToBmp(FrameworkElement visual, string fileName)
        {
            var encoder = new BmpBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                (int)visual.ActualWidth,
                (int)visual.ActualHeight,
                96,
                96,
                PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }
        #endregion Image

    }

    public class ShowItems : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value.GetType() == typeof(int))
            {
                int index = (int)value;
                return index == -1 ? Visibility.Collapsed : Visibility.Visible;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class NameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string name = value as string;
                return name.Substring(name.LastIndexOf('\\') + 1);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class SpriteSheetConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<Frame> ss = value as List<Frame>;
            if (ss != null && ss.Count > 0)
            {
                return ss[0].Image;
            }
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
