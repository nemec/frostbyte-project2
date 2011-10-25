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

namespace LevelEditor
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
        }

        #endregion Methods

        #region Event Handlers

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
            Canvas layer = (Parent as Canvas);
            if (layer != null)
            {
                layer.Children.Remove(this);
            }
        }

       #endregion Event Handlers
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