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

        public Point Pos
        {
            get
            {
                return e.Pos;
            }
            set
            {
                e.Pos = value;
            }
        }

        public GameObject()
        {
            InitializeComponent();
            InMenu = true;
            MouseUp += new MouseButtonEventHandler(Tile_MouseUp);
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
        }
    }
}
