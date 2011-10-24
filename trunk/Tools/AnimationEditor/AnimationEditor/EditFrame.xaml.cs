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
    /// Interaction logic for EditFrame.xaml
    /// </summary>
    public partial class EditFrame : UserControl
    {
        private bool mouseDownInDataPopup;
        private Point DragOffset;

        public EditFrame()
        {
            this.InitializeComponent();
            AddHandlers();
        }

        public EditFrame(Frame f)
        {
            DataContext = f;
            this.InitializeComponent();
            AddHandlers();
        }

        #region Methods
        public void AddHandlers()
        {
            //handles mouse down so mouse up can be handled
            ButtonClose.MouseLeftButtonDown += new MouseButtonEventHandler(ButtonClose_MouseLeftButtonDown);
            ButtonClose.MouseLeftButtonUp += new MouseButtonEventHandler(ButtonClose_MouseLeftButtonUp);
            DrawingScreen.MouseDown += new MouseButtonEventHandler(DrawingScreen_MouseDown);
            DrawingScreen.MouseUp += new MouseButtonEventHandler(DrawingScreen_MouseUp);
            MouseMove += new MouseEventHandler(EditFrame_MouseMove);
            SelectAnimPeg.MouseUp += new MouseButtonEventHandler(SelectAnimationPeg);
            AddRect.MouseUp += new MouseButtonEventHandler(AddRect_MouseUp);
            AddCirc.MouseUp += new MouseButtonEventHandler(AddCirc_MouseUp);
            Loaded += new RoutedEventHandler(EditFrame_Loaded);
        }

        public void DrawCollisions()
        {
            CollisionData.Children.Clear();
            Frame f = DataContext as Frame;
            foreach (var item in f.Collisions)
            {
                if (item.GetType() == typeof(CollisionCircle))
                {
                    CollisionCircle c = item as CollisionCircle;
                    Ellipse circle = new Ellipse()
                    {
                        Width = c.radius,
                        Height = c.radius,
                        Stroke = Brushes.Green,
                        StrokeThickness = 2
                    };

                    Canvas.SetTop(circle, c.center.Y - c.radius/2);
                    Canvas.SetLeft(circle, c.center.X - c.radius/2);

                    CollisionData.Children.Add(circle);
                }
                else if (item.GetType() == typeof(CollisionRect))
                {
                    CollisionRect r = item as CollisionRect;
                    Rectangle rectangle = new Rectangle
                    {
                        Width = r.Width,
                        Height = r.Height,
                        Stroke = Brushes.Green,
                        StrokeThickness = 2
                    };

                    Canvas.SetLeft(rectangle, r.TL.X);
                    Canvas.SetTop(rectangle, r.TL.Y);

                    CollisionData.Children.Add(rectangle);
                }
            }
            Ellipse el = new Ellipse
            {
                Stroke = Brushes.Yellow,
                StrokeThickness = 5,
                Width = 5,
                Height = 5
            };
            Canvas.SetLeft(el, f.AnimationPeg.X);
            Canvas.SetTop(el, f.AnimationPeg.Y);
            CollisionData.Children.Add(el);
        }
        #endregion Methods

        #region Event Handlers
        void EditFrame_Loaded(object sender, RoutedEventArgs e)
        {
            Frame f = DataContext as Frame;
            f.Collisions.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Collisions_CollectionChanged);
            DrawCollisions();
        }

        void Collisions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DrawCollisions();
        }

        void AddCirc_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AddingCircle = true;
            DrawingScreen.Visibility = Visibility.Visible;
        }

        void AddRect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AddingRectangle = true;
            DrawingScreen.Visibility = Visibility.Visible;
        }

        void EditFrame_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                return;

            var pos = e.GetPosition(DrawingScreen);

            if (rect != null)
            {
                var x = Math.Min(pos.X, StartPoint.X);
                var y = Math.Min(pos.Y, StartPoint.Y);

                var w = Math.Max(pos.X, StartPoint.X) - x;
                var h = Math.Max(pos.Y, StartPoint.Y) - y;

                rect.Width = w;
                rect.Height = h;

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
            }

            else if (circ != null)
            {
                var radius = (pos - StartPoint).Length;

                circ.Width = radius;
                circ.Height = radius;

                Canvas.SetLeft(circ, StartPoint.X - radius / 2);
                Canvas.SetTop(circ, StartPoint.Y - radius / 2);
            }
        }

        void DrawingScreen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Frame f = (DataContext as Frame);
            if (SelectPeg)
            {
                f.AnimationPeg = ActualPos;
                SelectPeg = false;
            }
            else if (rect != null)
            {
                f.Collisions.Add(new CollisionRect(ActualPos, rect.Width, rect.Height));
                rect = null;
                AddingRectangle = false;
            }
            else if (circ != null)
            {
                f.Collisions.Add(new CollisionCircle(ActualPos, circ));
                circ = null;
                AddingCircle = false;
            }
            DrawingScreen.Children.Clear();
            DrawingScreen.Visibility = Visibility.Collapsed;
            DrawCollisions();
        }

        void DrawingScreen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            StartPoint = e.GetPosition(DrawingScreen);
            ActualPos = e.GetPosition(FrameImage);
            if (AddingRectangle)
            {
                rect = new Rectangle
                {
                    Stroke = Brushes.Green,
                    StrokeThickness = 2
                };
                Canvas.SetLeft(rect, StartPoint.X);
                Canvas.SetRight(rect, StartPoint.Y);
                DrawingScreen.Children.Add(rect);
            }
            else if (AddingCircle)
            {
                circ = new Ellipse
                {
                    Stroke = Brushes.Green,
                    StrokeThickness = 2
                };
                Canvas.SetLeft(circ, StartPoint.X);
                Canvas.SetTop(circ, StartPoint.Y);
                DrawingScreen.Children.Add(circ);
            }
        }

        private void OnDataPopupClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(Border))
            {
                mouseDownInDataPopup = true;
                DragOffset = e.GetPosition(LayoutRoot);
                border1.CaptureMouse();
            }
        }

        private void OnDataPopupMoving(object sender, MouseEventArgs e)
        {
            if (mouseDownInDataPopup)
            {
                // we want to move it based on the position of the mouse
                moveUserControl(e);
            }
        }

        private void moveUserControl(MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(LayoutRoot);
            Double newX = LocalTranslateTransform.X + (mousePos.X - DragOffset.X);
            Double newY = LocalTranslateTransform.Y + (mousePos.Y - DragOffset.Y);
            LocalTranslateTransform.X = newX;
            LocalTranslateTransform.Y = newY;
        }

        private void OnDataPopupReleased(object sender, MouseButtonEventArgs e)
        {
            mouseDownInDataPopup = false;
            border1.ReleaseMouseCapture();
        }

        void ButtonClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public void ButtonClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Grid parent = (Parent as Grid);
            if (parent != null)
            {
                parent.Children.Remove(this);
            }
        }

        private void SelectAnimationPeg(object sender, MouseButtonEventArgs e)
        {
            SelectPeg = true;
            DrawingScreen.Visibility = Visibility.Visible;
        }
        #endregion Event Handlers

        public bool AddingRectangle;

        public bool AddingCircle;

        public bool SelectPeg;

        public Point StartPoint = new Point();

        public Point ActualPos = new Point();

        private Rectangle rect;

        private Ellipse circ;

    }
    public class FilenameClipper : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            if (s != null)
            {
                try
                {
                    return s.Substring(s.LastIndexOf("\\"));
                }
                catch
                {
                    return s;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}