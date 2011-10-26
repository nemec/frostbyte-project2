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
using Frostbyte;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for GameObject.xaml
    /// </summary>
    public partial class GameObject : UserControl
    {
        Enemy e = new Enemy();

        public bool InMenu { get; set; }

        public string InstanceName
        {
            get
            {
                return e.Name;
            }
            set
            {
                e.Name = value;
            }
        }

        public float Speed
        {
            get
            {
                return e.Speed;
            }
            set
            {
                e.Speed = value;
            }
        }

        public int Health
        {
            get
            {
                return e.Health;
            }
            set
            {
                e.Health = value;
            }
        }

        public Type Type
        {
            get
            {
                return e.EnemyType;
            }
            set
            {
                e.EnemyType = value;
            }
        }

        public int MaxHealth
        {
            get
            {
                return e.MaxHealth;
            }
            set
            {
                e.MaxHealth = value;
            }
        }

        public Point Pos
        {
            get
            {
                return new Point(e.Pos.X, e.Pos.Y);
            }
            set
            {
                e.Pos = new Index2D(value.X, value.Y);
                Canvas.SetTop(this, e.Pos.Y);
                Canvas.SetLeft(this, e.Pos.X);
            }
        }

        internal Enemy Object
        {
            get { return e; }
            set
            {
                e = value;
                Canvas.SetTop(this, e.Pos.Y);
                Canvas.SetLeft(this, e.Pos.X);
            }
        }

        public GameObject()
        {
            InitializeComponent();
            InMenu = true;
            MouseUp += new MouseButtonEventHandler(Tile_MouseUp);
            MouseDown += new MouseButtonEventHandler(GameObject_MouseDown);
        }

        void GameObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.RightButton == MouseButtonState.Pressed)
                DownOnMe = true;
            else if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                Dragging = true;
        }

        public GameObject(string p)
            : this()
        {
            TileImage.Source = new BitmapImage(new Uri(p, UriKind.RelativeOrAbsolute));
        }

        void Tile_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (InMenu)
            {
                This.mainWindow.SelectedGameObject = this;
                This.mainWindow.ClearTile = false;
                This.mainWindow.StartCell = new Point(-1, -1);
                This.mainWindow.EndCell = new Point(-1, -1);
            }
            else if (DownOnMe && e.MouseDevice.RightButton == MouseButtonState.Released)
            {
                EditFrame ef = new EditFrame() { DataContext = this };
                Canvas.SetTop(ef, Pos.Y);
                Canvas.SetLeft(ef, Pos.X);

                //pop up an edit frame for it
                This.mainWindow.OtherThings.Children.Add(ef);
                DownOnMe = false;
            }
            else if (Dragging && e.MouseDevice.LeftButton == MouseButtonState.Released)
            {
                Dragging = false;
            }
        }

        public GameObject Clone()
        {
            GameObject o = new GameObject()
               {
                   Health = Health,
                   Pos = Pos,
                   Speed = Speed,
                   InstanceName = InstanceName,
                   MaxHealth = MaxHealth,
                   Type = Type,
                   InMenu = false,
               };
            o.TileImage.Source = TileImage.Source;
            return o;
        }

        public bool DownOnMe { get; set; }

        public bool Dragging { get; set; }
    }
}
