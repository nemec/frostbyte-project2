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
        #region MapStuff
        public ObservableCollection<TileGroup> TileGroups { get; set; }

        /// <summary>
        /// selected Room etc
        /// </summary>
        public LevelPart SelectedLevelPart { get; set; }

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

        /// <summary>
        /// Currently selected tile
        /// </summary>
        public Tile SelectedTile { get; set; }

        /// <summary>
        /// Selected game object
        /// </summary>
        public GameObject SelectedGameObject { get; set; }

        public static Frostbyte.TileList TileMap = new TileList();
        #endregion MapStuff

        #region ControlTriggers
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
        #endregion ControlTriggers

        #region Constructor
        public MainWindow()
        {
            this.InitializeComponent();

            This.mainWindow = this;

            #region Tiles
            ObservableCollection<Tile> tiles = new ObservableCollection<Tile>(){
                new Tile(){
                    Name="Floor",
                    Traversable=true,
                    Type=TileTypes.Floor,
                    FloorType = TileTypes.Floor,
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

                new Tile(){
                    Name="Wall_Right",
                    Traversable=false,
                    Type=TileTypes.SideWall,
                    Orientation = Orientations.Left,
                    Active=true
                },

                new Tile(){
                    Name="Wall_Bottom",
                    Traversable=false,
                    Type=TileTypes.Bottom,
                    Active=true
                },

                new Tile(){
                    Name="Concave_Corner",
                    Traversable=false,
                    Type=TileTypes.Corner,
                    Orientation=Orientations.Left,
                    Active=true
                },
                
                new Tile(){
                    Name="Convex_Corner",
                    Traversable=false,
                    Type=TileTypes.ConvexCorner,
                    Orientation=Orientations.Left,
                    Active=true
                },

                 new Tile(){
                    Name="Bottom_Convex_Corner",
                    Traversable=false,
                    Type=TileTypes.BottomConvexCorner,
                    Orientation=Orientations.Down,
                    Active=true
                },

                new Tile(){
                    Name="TopGrass",
                    Traversable=false,
                    Type=TileTypes.TopArea,
                    Orientation=Orientations.Left,
                    Active=true
                },

                new Tile(){
                    Name="Empty",
                    Traversable=false,
                    Type=TileTypes.Empty,
                    Orientation=Orientations.Left,
                    Active=true
                },

                new Tile(){
                    Name="Rock",
                    Traversable=false,
                    Type=TileTypes.Stone,
                    Orientation=Orientations.Left,
                    Active=true
                },
            };
            #endregion Tiles

            #region Rooms
            ObservableCollection<Tile> rooms = new ObservableCollection<Tile>()
            {
                new Tile()
                {
                    Name="Room",
                    Traversable=false,
                    Type = TileTypes.Room,
                    Orientation = Orientations.Down,
                    FloorType = TileTypes.Floor,
                    IsSpecialObject=true,
                    Active=true
                },
                new Tile()
                {
                    Name="Walls",
                    Traversable=false,
                    Type = TileTypes.Wall,
                    Orientation = Orientations.Down,
                    IsSpecialObject=true,
                    Active=true
                },
                new Tile()
                {
                    Name="Wall",
                    Traversable=false,
                    Type = TileTypes.Wall,
                    Orientation = Orientations.Down,
                    IsSpecialObject=true,
                    Active=true
                },
                new Tile()
                {
                    Name="Floor",
                    Traversable=false,
                    Type = TileTypes.Floor,
                    Orientation = Orientations.Down,
                    FloorType = TileTypes.Floor,
                    IsSpecialObject=true,
                    Active=true
                }
            };
            #endregion Rooms

            #region OtherStuff
            ObservableCollection<GameObject> objects = new ObservableCollection<GameObject>()
            {
                new GameObject(){
                    InstanceName="Golem",
                    Speed=1,
                    Health=100,

                },
            };
            #endregion OtherStuff

            var stuff = new ObservableCollection<TileGroup>(){
                
                new TileGroup(rooms){
                    GroupName="Rooms"
                },
                new TileGroup(tiles){
                    GroupName="Tiles"
                },
                new TileGroup(objects){
                    GroupName="Objects"
                }
            };


            Objects.ItemsSource = stuff;
            Grid_Size = new Vector(100, 100);
            GridSize.DataContext = this;

            #region Event Handlers
            Level.MouseDown += new MouseButtonEventHandler(Level_MouseDown);
            Level.MouseUp += new MouseButtonEventHandler(Level_MouseUp);
            Level.MouseMove += new MouseEventHandler(Level_MouseMove);
            #endregion Event Handlers
        }

        static MainWindow()
        {
            CommandUndo = new RoutedUICommand("Undo",
                "Undo", typeof(MainWindow));
            CommandUndo.InputGestures.Add(
                new KeyGesture(Key.Z, ModifierKeys.Control));

            CommandRedo = new RoutedUICommand("Redo",
                "Redo", typeof(MainWindow));
            CommandRedo.InputGestures.Add(
                new KeyGesture(Key.R, ModifierKeys.Control));

            CommandSaveAndExit = new RoutedUICommand("SaveAndExit",
                "Exit", typeof(MainWindow));
            CommandSaveAndExit.InputGestures.Add(
                new KeyGesture(Key.Q, ModifierKeys.Control));
        }
        #endregion Constructor

        #region Helper fns
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
            Level.Width = Level.ColumnDefinitions.Count * Cellsize;
            Level.Height = Level.RowDefinitions.Count * Cellsize;
        }

        private void ClearSelection()
        {
            //clear selections
            //foreach (TileGroup elem in Objects.Items)
            //{
            //    foreach(TreeViewItem e in elem.Tiles.Items)
            //    {
            //        e.IsSelected = false;
            //    }
            //}
            SelectedLevelPart = null;
            SelectedTile = null;
            SelectedGameObject = null;
            StartCell = new Point(-1, -1);
            EndCell = new Point(-1, -1);
            ClearTile = true;
            CancelSelection = false;
        }

        private Vector GetCell(Point point)
        {
            return new Vector((int)(point.X / Cellsize), (int)(point.Y / Cellsize));
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

        private List<Tile> AddTile(Vector newpt)
        {
            List<Tile> tiles = new List<Tile>();
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
                        toadd.Orientation = Orientations.Up;
                        SelectedTile.Orientation = Orientations.Up;
                    }
                    else if (orientation.MagX > orientation.MagY)
                    {
                        toadd.Orientation = orientation.X > 0 ? Orientations.Left : Orientations.Right;
                        SelectedTile.Orientation = orientation.X > 0 ? Orientations.Left : Orientations.Right;
                    }
                    else
                    {
                        toadd.Orientation = orientation.Y < 0 ? Orientations.Up_Left : Orientations.Down;
                        SelectedTile.Orientation = orientation.Y < 0 ? Orientations.Up_Left : Orientations.Down;
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
                    tiles.Add(toadd);
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
            return tiles;
        }

        private List<Tile> RemoveTile(Vector newpt)
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
            return toRemove;
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

        /// <summary>
        /// adds tiles to the grid from a level obj list
        /// </summary>
        /// <param name="list"></param>
        private void AddTiles(List<LevelObject> list)
        {
            List<Tile> ts = new List<Tile>();
            foreach (Frostbyte.Tile t in list)
            {
                ts.Add(new Tile(t));
            }
            AddToGrid(ts);
        }

        /// <summary>
        /// adds tiles to the grid
        /// </summary>
        /// <param name="tiles"></param>
        private void AddToGrid(List<Tile> tiles)
        {
            foreach (Tile t in tiles)
            {
                Grid.SetColumn(t, t.GridCell.X);
                Grid.SetRow(t, t.GridCell.Y);
                This.mainWindow.Level.Children.Add(t);
            }
        }

        /// <summary>
        /// removes tiles form the grid and the list
        /// </summary>
        /// <param name="list"></param>
        private void RemoveTiles(List<Tile> list)
        {
            foreach (Tile o in list)
            {
                RemoveTile(new Vector(o.GridCell.X, o.GridCell.Y));
            }
        }

        /// <summary>
        /// Clears the tiles from the grid
        /// </summary>
        /// <param name="list"></param>
        private void ClearTiles(List<Tile> list)
        {
            List<Tile> toRemove = new List<Tile>();
            foreach (Tile t in list)
            {
                foreach (Tile item in Level.Children)
                {
                    int x = Grid.GetColumn(item);
                    int y = Grid.GetRow(item);
                    if (t.GridCell.X == x && t.GridCell.Y == y)
                    {
                        toRemove.Add(item);
                    }
                }
            }
            foreach (var elem in toRemove)
            {
                Level.Children.Remove(elem);
            }
        }
        #endregion Helper fns

        #region MouseHandlers
        void Level_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (CancelSelection && e.MouseDevice.RightButton == MouseButtonState.Released)
            {
                ClearSelection();
            }

            Moving = false;
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
                        //add to undo list
                        List<LevelObject> lo = new List<LevelObject>();
                        UndoableAction ua = new UndoableAction() { Added = true, OldState = TileMap.GetCurrentState(new Index2D(StartCell.X, StartCell.Y), new Index2D(EndCell.X, EndCell.Y)) };
                        foreach (Tile item in AddTile(GetCell(EndCell)))
                        {
                            lo.Add(item.TileValue);
                        }
                        ua.Objects = lo;
                        UndoTiles.Push(ua);
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

        void Level_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //do some handling here for end of room that skips the rest
            #region Rooms
            if (SelectedTile != null && SelectedTile.IsSpecialObject)
            {
                if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {
                    if (SelectedLevelPart == null)
                    {
                        StartCell = e.GetPosition(Level);
                        GridCell = GetCell(StartCell);
                        Index2D i = new Index2D(GridCell.X, GridCell.Y);
                        // fill the pieces
                        if (SelectedTile.Name == "Room")
                        {
                            SelectedLevelPart = new Room(i)
                            {
                                FloorType = SelectedTile.FloorType,
                            };
                        }
                        else if (SelectedTile.Name == "Walls")
                        {
                            SelectedLevelPart = new BorderWalls(i)
                            {
                                FloorType = SelectedTile.FloorType,
                            };
                        }
                        else if (SelectedTile.Name == "Wall")
                        {
                            SelectedLevelPart = new Wall(i)
                            {
                                FloorType = SelectedTile.FloorType,
                            };
                        }
                        else if (SelectedTile.Name == "Floor")
                        {
                            SelectedLevelPart = new Floor(i)
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

                        SelectedLevelPart.EndCell = new Index2D(GridCell.X, GridCell.Y);

                        //determine orientation
                        Point change = EndCell - (Vector)StartCell;
                        Index2D diff = new Index2D(change.X, change.Y);
                        if (SelectedLevelPart.Type == TileTypes.Wall)
                        {
                            if (diff.MagX > diff.MagY)
                            {
                                SelectedLevelPart.Type = diff.Y >= 0 ? TileTypes.Wall : TileTypes.Bottom;
                            }
                            else
                            {
                                SelectedLevelPart.Orientation = diff.X >= 0 ? Orientations.Left : Orientations.Right;
                            }
                        }
                        //add it to the undo list
                        UndoTiles.Push(new UndoableAction() { Added = true, Objects = new List<LevelObject>() { SelectedLevelPart }, OldState = TileMap.GetCurrentState(new Index2D(SelectedLevelPart.StartCell.X, SelectedLevelPart.StartCell.Y), new Index2D(SelectedLevelPart.EndCell.X, SelectedLevelPart.EndCell.Y)) });
                        //add it to the tile list
                        List<Tile> tiles = ToListTile(TileMap.Add(SelectedLevelPart));
                        //add it to visible grid
                        AddToGrid(tiles);

                        SelectedLevelPart = null;
                    }
                }
                else if (e.MouseDevice.RightButton == MouseButtonState.Pressed)
                {
                    if (ClearTile)
                    {
                        GridCell = GetCell(e.GetPosition(Level));
                        //add to undo list
                        List<LevelObject> lo = new List<LevelObject>();
                        UndoableAction ua = new UndoableAction() { Added = false, OldState = TileMap.GetCurrentState(new Index2D(GridCell.X, GridCell.Y), new Index2D(GridCell.X, GridCell.Y)) };
                        foreach (Tile item in RemoveTile(GridCell))
                        {
                            lo.Add(item.TileValue);
                        }
                        ua.Objects = lo;
                        UndoTiles.Push(ua);
                        FirstClick = true;
                    }
                    CancelSelection = true;
                }
            }
            #endregion Room
            #region Objects
            else if (SelectedGameObject != null)
            {
                //get sposition
                Point p = e.GetPosition(Level);
                //calcl offest for centerpoint
                p.X -= SelectedGameObject.Width / 2;
                p.Y -= SelectedGameObject.Height / 2;
                //copy the object and set attributes
                GameObject obj = SelectedGameObject.Clone();
                obj.Pos = p;
                //pop up an edit frame for it

            }
            #endregion Objects
            #region tiles
            else
            {
                if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {
                    if (SelectedTile != null)
                    {
                        StartCell = e.GetPosition(Level);
                        GridCell = GetCell(StartCell);
                        FirstClick = true;
                        //add to undo list
                        List<LevelObject> lo = new List<LevelObject>();
                        UndoableAction ua = new UndoableAction() { Added = true, OldState = TileMap.GetCurrentState(new Index2D(StartCell.X, StartCell.Y), new Index2D(EndCell.X, EndCell.Y)) };
                        foreach (Tile item in AddTile(GetCell(StartCell)))
                        {
                            lo.Add(item.TileValue);
                        }
                        ua.Objects = lo;
                        UndoTiles.Push(ua);
                    }
                }
                else if (e.MouseDevice.RightButton == MouseButtonState.Pressed)
                {
                    if (ClearTile)
                    {
                        GridCell = GetCell(e.GetPosition(Level));
                        //add to undo list
                        List<LevelObject> lo = new List<LevelObject>();
                        UndoableAction ua = new UndoableAction() { Added = false, OldState = TileMap.GetCurrentState(new Index2D(GridCell.X, GridCell.Y), new Index2D(GridCell.X, GridCell.Y)) };
                        foreach (Tile item in RemoveTile(GridCell))
                        {
                            lo.Add(item.TileValue);
                        }
                        ua.Objects = lo;
                        UndoTiles.Push(ua);
                        FirstClick = true;
                    }
                    CancelSelection = true;
                }
            }
            #endregion tiles
        }
        #endregion MouseHandlers

        #region KeyControls

        #endregion KeyControls

        #region MenuFunctions
        private void Yes(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CanSave(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TileMap.HasItems;
        }

        public void LoadFromFile(Object sender, ExecutedRoutedEventArgs e)
        {
            ClearSelection();
            OpenFileDialog d = new OpenFileDialog();
            d.FileName = "Level#";
            d.DefaultExt = ".xml";
            d.Filter = "Level files (.xml)|*.xml";
            if (d.ShowDialog() == true)
            {
                Level.Children.Clear();
                LoadGrid(new TileList(XDocument.Load(d.FileName)));
            }
        }

        public void SaveFile(Object sender, ExecutedRoutedEventArgs e)
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

        public void SaveFileAs(Object sender, ExecutedRoutedEventArgs e)
        {
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

        public void SaveAndExit(Object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileAs(sender, e);
            Application.Current.Shutdown();
        }

        private void UndoAction(object sender, ExecutedRoutedEventArgs e)
        {
            var tiles = Undo();
            ClearTiles(tiles);
            AddToGrid(tiles);
        }

        private void RedoAction(object sender, ExecutedRoutedEventArgs e)
        {
            var tiles = Redo();
            ClearTiles(tiles);
            AddToGrid(tiles);
        }
        #endregion MenuFunctions

        #region RoutedCommands
        public readonly static RoutedUICommand CommandSaveAndExit;
        public readonly static RoutedUICommand CommandUndo;
        public readonly static RoutedUICommand CommandRedo;
        #endregion RoutedCommands

        #region Execution
        private void CanUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = UndoTiles.Count > 0;
        }

        private void CanRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RedoTiles.Count > 0;
        }
        #endregion Execution

        #region Undo
        /// <summary>
        /// List of tiles that have been removed. 
        /// </summary>
        Stack<UndoableAction> UndoTiles = new Stack<UndoableAction>();

        public List<Tile> Undo()
        {
            List<Tile> tiles = new List<Tile>();
            UndoableAction placedObject = UndoTiles.Pop();
            List<LevelObject> objs = placedObject.Objects;
            foreach (LevelObject obj in objs)
            {
                if (obj is Frostbyte.Tile)
                {
                    Frostbyte.Tile t = obj as Frostbyte.Tile;
                    if (placedObject.Added)
                    {
                        //remove the tiles
                        TileMap.Remove(t);
                        //pass data out of fn
                        tiles.Add(new Tile(t));
                        //replace with old tiles
                        TileMap.SetState(placedObject.OldState);
                        //we've just been removed
                        placedObject.Added = false;
                        //now we can redo what we just undid
                        RedoTiles.Push(placedObject);
                    }
                    else
                    {
                        //copy current state
                        placedObject.OldState = TileMap.GetCurrentState(t.GridCell, t.GridCell);
                        //add new tiles
                        TileMap.Add(t);
                        //pass tiles out of fn
                        tiles.Add(new Tile(t));
                        //we've just been added
                        placedObject.Added = true;
                        //let us be able to undo this sucker
                        RedoTiles.Push(placedObject);
                    }
                }
                else if (obj is LevelPart)
                {
                    LevelPart lp = obj as LevelPart;
                    List<Frostbyte.Tile> fbts = new List<Frostbyte.Tile>();
                    if (placedObject.Added)
                    {
                        //remove the tiles
                        ClearTiles(ToListTile(TileMap.Remove(lp)));
                        //replace with old tiles
                        TileMap.SetState(placedObject.OldState);
                        //pass out the old state to draw
                        foreach (var thing in placedObject.OldState)
                        {
                            fbts.Add(thing);
                        }
                        //we've just been removed
                        placedObject.Added = false;
                        //now we can redo what we just undid
                        RedoTiles.Push(placedObject);
                    }
                    else
                    {
                        //copy current state
                        placedObject.OldState = TileMap.GetCurrentState(lp.StartCell, lp.EndCell);
                        //add new tiles and pass out the result
                        fbts = TileMap.Add(lp);
                        //we've just been added
                        placedObject.Added = true;
                        //Now we can redo this
                        RedoTiles.Push(placedObject);
                    }

                    //pass tiles to draw out of the fn
                    foreach (var item in fbts)
                    {
                        tiles.Add(new Tile(item));
                    }
                }
            }
            return tiles;
        }
        #endregion Undo

        #region Redo
        /// <summary>
        /// List of tiles that have been removed. 
        /// </summary>
        Stack<UndoableAction> RedoTiles = new Stack<UndoableAction>();

        public List<Tile> Redo()
        {
            List<Tile> tiles = new List<Tile>();
            UndoableAction placedObject = RedoTiles.Pop();
            List<LevelObject> objs = placedObject.Objects;
            foreach (LevelObject obj in objs)
            {
                if (obj is Frostbyte.Tile)
                {
                    Frostbyte.Tile t = obj as Frostbyte.Tile;
                    if (placedObject.Added)
                    {
                        //remove the tiles
                        TileMap.Remove(t);
                        //pass data out of fn
                        tiles.Add(new Tile(t));
                        //replace with old tiles
                        TileMap.SetState(placedObject.OldState);
                        //we've just been removed
                        placedObject.Added = false;
                        //now we can redo what we just undid
                        UndoTiles.Push(placedObject);
                    }
                    else
                    {
                        //copy current state
                        placedObject.OldState = TileMap.GetCurrentState(t.GridCell, t.GridCell);
                        //add new tiles
                        TileMap.Add(t);
                        //pass tiles out of fn
                        tiles.Add(new Tile(t));
                        //we've just been added
                        placedObject.Added = true;
                        //let us be able to undo this sucker
                        UndoTiles.Push(placedObject);
                    }
                }
                else if (obj is LevelPart)
                {
                    LevelPart lp = obj as LevelPart;
                    List<Frostbyte.Tile> fbts = new List<Frostbyte.Tile>();
                    if (placedObject.Added)
                    {
                        //remove the tiles
                        ClearTiles(ToListTile(TileMap.Remove(lp)));
                        //replace with old tiles
                        TileMap.SetState(placedObject.OldState);
                        //pass out the old state to draw
                        foreach (var thing in placedObject.OldState)
                        {
                            fbts.Add(thing);
                        }
                        //we've just been removed
                        placedObject.Added = false;
                        //now we can redo what we just undid
                        UndoTiles.Push(placedObject);
                    }
                    else
                    {
                        //copy current state
                        placedObject.OldState = TileMap.GetCurrentState(lp.StartCell, lp.EndCell);
                        //add new tiles and pass out the result
                        fbts = TileMap.Add(lp);
                        //we've just been added
                        placedObject.Added = true;
                        //Now we can redo this
                        UndoTiles.Push(placedObject);
                    }

                    //pass tiles to draw out of the fn
                    foreach (var item in fbts)
                    {
                        tiles.Add(new Tile(item));
                    }
                }
            }
            return tiles;
        }
        #endregion Redo
    }


    public class TransformConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                Orientations o = (Orientations)value;
                if (o == Orientations.Up_Left)
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
                        file = "bot.png";
                        break;
                    case TileTypes.Corner:
                        file = "corner.png";
                        break;
                    case TileTypes.BottomCorner:
                        file = "botcorner.png";
                        break;
                    case TileTypes.ConvexCorner:
                        file = "convex-coner.png";
                        break;
                    case TileTypes.BottomConvexCorner:
                        file = "bot-convex-coner.png";
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
                    case TileTypes.Stone:
                        file = "rock.png";
                        break;
                    case TileTypes.Empty:
                        file = "";
                        break;
                    case TileTypes.TopArea:
                        file = "top-grass.png";
                        break;
                    default:
                        file = "error.png";
                        break;
                }
                try
                {
                    return new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    Console.WriteLine("File {0} does not exist.", file);
                    return new BitmapImage(new Uri("error.png", UriKind.RelativeOrAbsolute));
                }
            }
            return new RotateTransform(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}