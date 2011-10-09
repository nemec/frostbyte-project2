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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Frostbyte;
using System.IO;
using System.Xml.Linq;
using Microsoft.Win32;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<TileGroup> TileGroups { get; set; }

        public LevelPart SelectedObject { get; set; }

        public Vector Grid_Size
        {
            get
            {
                return gridSize;
            }
            set
            {
                gridSize = value;
                CreateGrid();
            }
        }

        private Vector gridSize;

        public Vector GridCell = new Vector(-1, -1);

        public Point StartCell { get; set; }
        public Point EndCell { get; set; }

        public int Cellsize = 64;

        public Tile SelectedTile { get; set; }

        public static Frostbyte.TileList TileMap = new TileList();

        public MainWindow()
        {
            this.InitializeComponent();

            This.mainWindow = this;

            ObservableCollection<Tile> tiles = new ObservableCollection<Tile>(){
                new Tile(){
                    Name="Floor",
                    Traversable=true,
                    Type=TileTypes.Floor,
                    FloorType = FloorTypes.Themed,
                    Orientation= Orientations.Down,
                    Active=true
                },
                new Tile(){
                    Name="Wall_Top",
                    Traversable=true,
                    Type=TileTypes.Wall,
                    Orientation = Orientations.Down,
                    Active=true
                },

                //new Tile(){
                //    Name="Wall_Left",
                //    Traversable=false,
                //    Type=TileTypes.SideWall,
                //    Orientation = Orientations.Right,
                //    Active=true
                //},

                new Tile(){
                    Name="Wall_Right",
                    Traversable=false,
                    Type=TileTypes.SideWall,
                    Orientation = Orientations.Left,
                    Active=true
                },

                //new Tile(){
                //    Name="Wall_Bottom",
                //    Traversable=false,
                //    Type=TileTypes.Bottom,
                //    Active=true
                //},

                new Tile(){
                    Name="Concave_Corner",
                    Traversable=false,
                    Type=TileTypes.Corner,
                    Orientation=Orientations.Right,
                    Active=true
                },
                
                new Tile(){
                    Name="Convex_Corner",
                    Traversable=false,
                    Type=TileTypes.ConvexCorner,
                    Orientation=Orientations.Right,
                    Active=true
                },

                //new Tile(){
                //    Name="Corner_TR",
                //    Traversable=false,
                //    Type=TileTypes.Corner,
                //    Orientation=Orientations.Down,
                //    Active=true
                //},

                //new Tile(){
                //    Name="Corner_BR",
                //    Traversable=false,
                //    Type=TileTypes.Corner,
                //    Orientation=Orientations.Up,
                //    Active=true
                //}
                //,

                //new Tile(){
                //    Name="Corner_BL",
                //    Traversable=false,
                //    Type=TileTypes.Corner,
                //    Orientation = Orientations.Up_Right,
                //    Active=true
                //}
            };
            ObservableCollection<Tile> rooms = new ObservableCollection<Tile>()
            {
                new Tile()
                {
                    Name="Room",
                    Traversable=false,
                    Type = TileTypes.Room,
                    Orientation = Orientations.Down,
                    FloorType = FloorTypes.Themed,
                    IsSpecialObject=true,
                    Active=true
                },
                new Tile()
                {
                    Name="Walls",
                    Traversable=false,
                    Type = TileTypes.Wall,
                    Orientation = Orientations.Down,
                    FloorType = FloorTypes.Themed,
                    IsSpecialObject=true,
                    Active=true
                },
                new Tile()
                {
                    Name="Wall",
                    Traversable=false,
                    Type = TileTypes.Wall,
                    Orientation = Orientations.Down,
                    FloorType = FloorTypes.Themed,
                    IsSpecialObject=true,
                    Active=true
                },
                new Tile()
                {
                    Name="Floor",
                    Traversable=false,
                    Type = TileTypes.Floor,
                    Orientation = Orientations.Down,
                    FloorType = FloorTypes.Themed,
                    IsSpecialObject=true,
                    Active=true
                }
            };
            var stuff = new ObservableCollection<TileGroup>(){
                
                new TileGroup(rooms){
                    GroupName="Rooms"
                },
                new TileGroup(tiles){
                    GroupName="Tiles"
                },
            };

            //clears selected items
            foreach (var tg in stuff)
            {
                tg.Tiles.SelectedIndex = -1;
            }

            Objects.ItemsSource = stuff;

            Level.MouseDown += new MouseButtonEventHandler(Level_MouseDown);
            Level.MouseUp += new MouseButtonEventHandler(Level_MouseUp);
            Level.MouseMove += new MouseEventHandler(Level_MouseMove);

            Grid_Size = new Vector(100, 100);
            GridSize.DataContext = this;


            SaveMap.MouseUp += new MouseButtonEventHandler(SaveMap_MouseUp);
            LoadLevel.MouseUp += new MouseButtonEventHandler(LoadLevel_MouseUp);

        }


        void LoadLevel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ClearSelection();
            OpenFileDialog d = new OpenFileDialog();
            d.FileName = "Level#";
            d.DefaultExt = ".xml";
            d.Filter = "Level files (.xml)|*.xml";
            if (d.ShowDialog() == true)
            {
                LoadGrid(new TileList(XDocument.Load(d.FileName)));
            }
        }

        void SaveMap_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //obsolete
        }
        void SaveMap_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ClearSelection();
            SaveFileDialog d = new SaveFileDialog();
            d.FileName = "Level#";
            d.DefaultExt = ".xml";
            d.Filter = "Level files (.xml)|*.xml";
            if (d.ShowDialog() == true)
            {
                //open save box and then create all the crap that needs to get saved
                List<LevelObject> objs = TileMap.GenerateSaveObjects();

                XDocument doc = new XDocument(new XElement("Level"));
                foreach (LevelObject l in objs)
                {
                    doc.Root.Add(l.ToXML());
                }

                TileMap.Save(d.FileName, doc);
            }

        }

        void LoadGrid(TileList tm)
        {
            TileMap = tm;
            var l = tm.Data;
            var tiles = l.Item2;
            foreach (var list in tiles)
            {
                foreach (var tile in list)
                {
                    if (tile.Type != TileTypes.DEFAULT)
                    {
                        Tile t = new Tile(tile);
                        Grid.SetColumn(t, tile.GridCell.X);
                        Grid.SetRow(t, tile.GridCell.Y);

                        Level.Children.Add(t);
                    }
                }
            }
        }

        private void CreateGrid()
        {
            Level.RowDefinitions.Clear();
            Level.ColumnDefinitions.Clear();
            while (Level.RowDefinitions.Count < Grid_Size.Y)
            {
                Level.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Cellsize) });
            }
            while (Level.ColumnDefinitions.Count < Grid_Size.X)
            {
                Level.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Cellsize) });
            }
            Level.Width = Level.RowDefinitions.Count * Cellsize;
            Level.Height = Level.ColumnDefinitions.Count * Cellsize;
        }

        void Level_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (CancelSelection && e.MouseDevice.RightButton == MouseButtonState.Released)
            {
                ClearSelection();
            }

            Moving = false;
        }

        private void ClearSelection()
        {
            //clear selections
            foreach (TileGroup elem in Objects.Items)
            {
                elem.Tiles.SelectedIndex = -1;
            }
            SelectedObject = null;
            SelectedTile = null;
            StartCell = new Point(-1, -1);
            EndCell = new Point(-1, -1);
            ClearTile = true;
            CancelSelection = false;
        }

        void Level_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                if (SelectedTile != null)
                {
                    Moving = true;
                    if (!SelectedTile.IsSpecialObject)
                    {
                        EndCell = e.GetPosition(Level);
                        AddTile(GetCell(EndCell));
                    }
                    else
                    {
                        //do something for special objects when dragging
                    }
                }
            }
            if (ClearTile && e.MouseDevice.RightButton == MouseButtonState.Pressed)
            {
                RemoveTile(GetCell(e.GetPosition(Level)));
            }
        }

        private Vector GetCell(Point point)
        {
            return new Vector((int)(point.X / Cellsize), (int)(point.Y / Cellsize));
        }

        void Level_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //do some handling here for end of room that skips the rest
            if (SelectedTile != null && SelectedTile.IsSpecialObject)
            {
                if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {
                    if (SelectedObject == null)
                    {
                        StartCell = e.GetPosition(Level);
                        GridCell = GetCell(StartCell);
                        Index2D i = new Index2D(GridCell.X, GridCell.Y);
                        // fill the pieces
                        if (SelectedTile.Name == "Room")
                        {
                            SelectedObject = new Room(i)
                            {
                                FloorType = SelectedTile.FloorType,
                            };
                        }
                        else if (SelectedTile.Name == "Walls")
                        {
                            SelectedObject = new BorderWalls(i)
                            {
                                FloorType = SelectedTile.FloorType,
                            };
                        }
                        else if (SelectedTile.Name == "Wall")
                        {
                            SelectedObject = new Wall(i)
                            {
                                FloorType = SelectedTile.FloorType,
                            };
                        }
                        else if (SelectedTile.Name == "Floor")
                        {
                            SelectedObject = new Floor(i)
                            {
                                Type = TileTypes.Floor,
                                FloorType = SelectedTile.FloorType,
                            };
                        }
                    }
                    else//here we complete things for the object
                    {
                        EndCell = e.GetPosition(Level);
                        GridCell = GetCell(EndCell);

                        SelectedObject.EndCell = new Index2D(GridCell.X, GridCell.Y);

                        //determine orientation
                        Point change = EndCell - (Vector)StartCell;
                        Index2D diff = new Index2D(change.X, change.Y);
                        if (SelectedObject.Type == TileTypes.Wall)
                        {
                            if (diff.MagX > diff.MagY)
                            {
                                SelectedObject.Type = diff.Y >= 0 ? TileTypes.Wall : TileTypes.Bottom;
                            }
                            else
                            {
                                SelectedObject.Orientation = diff.X >= 0 ? Orientations.Right : Orientations.Left;
                            }
                        }
                        //determine what it is
                        List<Tile> tiles = ToListTile(TileMap.Add(SelectedObject));

                        foreach (Tile t in tiles)
                        {
                            Grid.SetColumn(t, t.GridCell.X);
                            Grid.SetRow(t, t.GridCell.Y);
                            This.mainWindow.Level.Children.Add(t);
                        }

                        SelectedObject = null;
                    }
                }
                else if (e.MouseDevice.RightButton == MouseButtonState.Pressed)
                {
                    if (ClearTile)
                    {
                        GridCell = GetCell(e.GetPosition(Level));
                        RemoveTile(GridCell);
                        FirstClick = true;
                    }
                    CancelSelection = true;
                }
            }
            else
            {
                if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {
                    if (SelectedTile != null)
                    {
                        StartCell = e.GetPosition(Level);
                        GridCell = GetCell(StartCell);
                        FirstClick = true;
                        AddTile(GridCell);
                    }
                }
                else if (e.MouseDevice.RightButton == MouseButtonState.Pressed)
                {
                    if (ClearTile)
                    {
                        GridCell = GetCell(e.GetPosition(Level));
                        RemoveTile(GridCell);
                        FirstClick = true;
                    }
                    CancelSelection = true;
                }
            }
        }

        private List<Tile> ToListTile(List<Frostbyte.Tile> list)
        {
            List<Tile> ts = new List<Tile>();
            foreach (var t in list)
            {
                ts.Add(new Tile(t));
            }
            return ts;
        }

        private void AddTile(Vector newpt)
        {
            if (newpt != GridCell || FirstClick)
            {
                if (GridCell.X < 0 || GridCell.Y < 0 || GridCell.X > gridSize.X || GridCell.Y > gridSize.Y)
                {
                    GridCell = newpt;
                }

                //deterimine which coord changed more
                Vector diff = newpt - GridCell;

                bool Horiz = (diff.X < 0 ? -diff.X : diff.X) > (diff.Y < 0 ? -diff.Y : diff.Y);

                Vector dir = Horiz ? new Vector(diff.X, 0) : new Vector(0, diff.Y);
                dir.Normalize();

                while (FirstClick || (Horiz ? GridCell.X != (Moving ? newpt.X + dir.X : newpt.X) : GridCell.Y != (Moving ? newpt.Y + dir.Y : newpt.Y)))
                {
                    if (GridCell.X < 0 || GridCell.Y < 0)
                        GridCell = newpt;
                    RemoveTile(GridCell);

                    //draw the selecteditem
                    Tile toadd = Tile.DeepCopy(SelectedTile);

                    //orient it
                    Point p = EndCell - (Vector)StartCell;
                    Index2D orientation = new Index2D(p.X, p.Y);
                    if (orientation.X > 0 && orientation.Y < 0)
                    {
                        toadd.Orientation = Orientations.Up_Right;
                        SelectedTile.Orientation = Orientations.Up_Right;
                    }
                    else if (orientation.MagX > orientation.MagY)
                    {
                        toadd.Orientation = orientation.X > 0 ? Orientations.Right : Orientations.Left;
                        SelectedTile.Orientation = orientation.X > 0 ? Orientations.Right : Orientations.Left;
                    }
                    else
                    {
                        toadd.Orientation = orientation.Y < 0 ? Orientations.Up : Orientations.Down;
                        SelectedTile.Orientation = orientation.Y < 0 ? Orientations.Up : Orientations.Down;
                    }


                    if (toadd.GridCell == null)
                        toadd.GridCell = new Index2D(GridCell.X, GridCell.Y);

                    //set the cell
                    int y = (int)GridCell.Y;
                    int x = (int)GridCell.X;
                    Grid.SetRow(toadd, y);
                    Grid.SetColumn(toadd, x);

                    Level.Children.Add(toadd);
                    if (!TileMap.Add(toadd.TileValue))
                    {
                        TileMap.Add(toadd.TileValue, x, y);
                    }

                    if ((Horiz ? GridCell.X != (Moving ? newpt.X + dir.X : newpt.X) : GridCell.Y != (Moving ? newpt.Y + dir.Y : newpt.Y)))
                        GridCell += dir;
                    else
                    {
                        FirstClick = false;
                    }
                }



                //set the new last grid point
                GridCell = newpt;
            }
        }

        private void RemoveTile(Vector newpt)
        {
            //remove old element
            List<Tile> toRemove = new List<Tile>();

            if (newpt != GridCell || FirstClick)
            {
                if (GridCell.X < 0 || GridCell.Y < 0 || GridCell.X > gridSize.X || GridCell.Y > gridSize.Y)
                {
                    GridCell = newpt;
                }
                //deterimine which coord changed more
                Vector diff = newpt - GridCell;

                bool Horiz = (diff.X < 0 ? -diff.X : diff.X) > (diff.Y < 0 ? -diff.Y : diff.Y);

                Vector dir = Horiz ? new Vector(diff.X, 0) : new Vector(0, diff.Y);
                dir.Normalize();

                while (FirstClick || (Horiz ? GridCell.X != (Moving ? newpt.X + dir.X : newpt.X) : GridCell.Y != (Moving ? newpt.Y + dir.Y : newpt.Y)))
                {
                    if (GridCell.X < 0 || GridCell.Y < 0)
                        GridCell = newpt;
                    foreach (Tile item in Level.Children)
                    {
                        int x = Grid.GetColumn(item);
                        int y = Grid.GetRow(item);
                        if ((int)GridCell.X == x && (int)GridCell.Y == y)
                        {
                            toRemove.Add(item);
                        }
                    }
                    if ((Horiz ? GridCell.X != (Moving ? newpt.X + dir.X : newpt.X) : GridCell.Y != (Moving ? newpt.Y + dir.Y : newpt.Y)))
                        GridCell += dir;
                    FirstClick = false;
                }
                //set the new last grid point
                GridCell = newpt;
            }
            foreach (var elem in toRemove)
            {
                Level.Children.Remove(elem);
                if (!TileMap.Remove(elem.TileValue))
                {
                    int x = Grid.GetColumn(elem);
                    int y = Grid.GetRow(elem);
                    TileMap.Remove(elem.TileValue, x, y);
                }
            }
        }

        /// <summary>
        /// Wheter we can clear tiles or not
        /// </summary>
        public bool ClearTile { get; set; }
        /// <summary>
        /// We can cancel our current selectin
        /// </summary>
        public bool CancelSelection { get; set; }
        /// <summary>
        /// This is the first left click
        /// </summary>
        public bool FirstClick { get; set; }
        /// <summary>
        /// The mouse is moving, keep drawing
        /// </summary>
        public bool Moving { get; set; }
    }


    public class TransformConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                Orientations o = (Orientations)value;
                if (o == Orientations.Up_Right)
                {
                    return new ScaleTransform(-1, -1);
                }
                else if (o == Orientations.Up)
                {
                    return new ScaleTransform(1, -1);
                }
                else if (o == Orientations.Right)
                {
                    return new ScaleTransform(-1, 1);
                }

                //return new RotateTransform(90 * (int)o);

            }
            return new RotateTransform(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class TileConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                TileTypes tt = (TileTypes)value;
                string file = "error.png";
                switch (tt)
                {
                    case TileTypes.Wall:
                        file = "wall.png";
                        break;
                    case TileTypes.Bottom:
                        file = "wall.png";
                        break;
                    case TileTypes.Corner:
                        file = "corner.png";
                        break;
                    case TileTypes.ConvexCorner:
                        file = "convex-coner.png";
                        break;
                    case TileTypes.Floor:
                        file = "floor.png";
                        break;
                    case TileTypes.Lava:
                        file = "lava.png";
                        break;
                    case TileTypes.Water:
                        file = "water.png";
                        break;
                    case TileTypes.SideWall:
                        file = "side.png";
                        break;
                    case TileTypes.Room:
                        //do some magic to show pic for the walls etc
                        file = "room.png";
                        break;
                    default:
                        file = "error.png";
                        break;
                }
                return new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
            }
            return new RotateTransform(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}