using System;
using System.Collections.Generic;
using System.IO;

namespace Khet
{
    public enum BoardLayoutNames {
        Classic = 0,
        Imhotep,
        Dynasty
    }

    public class Board
    {
        private Tile[,] Tiles { get; set; }

        public List<LaserEmitter> LaserEmitters { get; private set; }

        private Dictionary<BoardLayoutNames, string[,]> BoardLayouts { get; set; }

        private KhetGame _khetGame;

        public Board(KhetGame khetGame = null)
        {
            BoardLayouts = new Dictionary<BoardLayoutNames, string[,]>();
            Tiles = new Tile[10,8]; // TODO: is this required?
            LaserEmitters = new List<LaserEmitter>();
            _khetGame = khetGame;
            SetUpTiles();
            LoadLayoutFiles();
            SetUpLaserEmitters();
        }

        private void SetUpTiles()
        {
            char[,] tileColourRestrictions = LoadTileColourRestrictions();
            for (int x = 0; x < Tiles.GetLength(0); x++)
                for (int y = 0; y < Tiles.GetLength(1); y++)
                    Tiles[x,y] = new Tile(this, CharToColour(tileColourRestrictions[x,y]));
        }

        private void SetUpLaserEmitters()
        {
            LaserEmitters.Add(new LaserEmitter(Tiles[0,0], false));
            LaserEmitters.Add(new LaserEmitter(Tiles[Tiles.GetLength(0)-1, Tiles.GetLength(1)-1], true));
        }

        private Colour CharToColour(char character)
        {
            switch (character) {
                case 'S':
                    return Colour.Silver;
                case 'R':
                    return Colour.Red;
                default:
                    return (Colour)(-1);
            }
        }

        private char[,] LoadTileColourRestrictions()
        {
			string path = Utils.ResourcePath("khet-layouts/Tile_Colours.txt");
            // Read and split the file content into lines
			string[] tileColourRestrictionLines = Utils.ReadFile(path).Split('\n');
            char[,] tileColourRestrictions = new char[10,8];
            // For each line
            for (int y = 0; y < tileColourRestrictionLines.Length; y++) {
                // Split the line into characters
                string[] tileColourRestrictionLineChars = tileColourRestrictionLines[y].Split(' ');
                // For each character in the line
                for (int x = 0; x < tileColourRestrictionLineChars.Length; x++)
                    // Put it in the main array, in char form
                    tileColourRestrictions[x,y] = tileColourRestrictionLineChars[x][0];
            }
            return tileColourRestrictions;
        }

        private void LoadLayoutFiles()
        {
            for (BoardLayoutNames layoutName = 0;
                 layoutName < (BoardLayoutNames)Enum.GetNames(typeof(BoardLayoutNames)).Length; layoutName++) {
                string filename = "Layout_" + layoutName.ToString() + ".txt";
                LoadLayoutFromFile(layoutName, filename);
            }
        }

        public bool LoadLayoutFromFile(BoardLayoutNames layoutName, string filename)
        {
			string path = Utils.ResourcePath("khet-layouts/" + filename);
			return LoadLayoutFromString(layoutName, Utils.ReadFile(path));
        }

        /// <summary>
        /// Loads a layout from a given string and assigns it to the given layout name.
        /// </summary>
        /// <param name='layoutName'>
        /// Layout name.
        /// </param>
        /// <param name='layoutString'>
        /// Layout string.
        /// </param>
        public bool LoadLayoutFromString(BoardLayoutNames layoutName, string layoutString)
        {
            // Load the layout string into an array of each tile

            string[,] layoutBoard = new string[Tiles.GetLength(0), Tiles.GetLength(1)];
            // Split the layout string into an array of its lines
            string[] layoutLines = layoutString.Split('\n');
            // Check for invalid number of rows
            if (layoutLines.Length != Tiles.GetLength(1)) {
                Console.Error.WriteLine("Invalid number of rows encountered when attempting to load layout " +
                    "from string into " + layoutName.ToString());
                return false;
            }
            // For each layout line
            for (int y = 0; y < Tiles.GetLength(1); y++) {
                // Make an array of the line's tiles
                string[] layoutLineTiles = layoutLines[y].Split(' ');
                // Check for invalid number of columns on this line
                if (layoutLineTiles.Length != Tiles.GetLength(0)){
                    Console.Error.WriteLine("Invalid number of columns encountered when attempting to load layout " +
                        "from string into " + layoutName.ToString() + ", at line " + y+1);
                    return false;
                }
                // Add this line's tiles to the board layout array
                for (int x = 0; x < Tiles.GetLength(0); x++)
                    layoutBoard[x, y] = layoutLineTiles[x];
            }

            // Put the layout in the board layouts array
            BoardLayouts[layoutName] = layoutBoard;
            return true;
        }

        /// <summary>
        /// Arranges the pieces on the board into the loaded layout with the given name.
        /// </summary>
        /// <param name='layoutName'>
        /// Layout name.
        /// </param>
        public void ArrangePieces(BoardLayoutNames layoutName)
        {
            string[,] layout = BoardLayouts[layoutName];
            // For every tile in the board, create an appropriate piece for it from the layout's corresponding
            // piece string
            for (int y = 0; y < Tiles.GetLength(1); y++) {
                for (int x = 0; x < Tiles.GetLength(0); x++) {
                    string pieceString = layout[x, y];
                    // Create the piece
                    GamePiece piece = GamePiece.CreateFromString(pieceString, Tiles[x, y]);
//                    // Place the piece
//                    if (piece != null)
//                        Tiles[x, y].LandGamePiece(piece);
                }
            }
        }

        public override string ToString()
        {
            string description = "";
            for (int y = 0; y < Tiles.GetLength(1); y++) {
                for (int x = 0; x < Tiles.GetLength(0); x++) {
                    // Add a newline to separate rows
                    if (y > 0 && x == 0)
                        description += "\n";
                    else if (x > 0)
                        description += " ";
                    description += Tiles[x, y].ToString();
                }
            }
            return description;
        }

        public Tile TileAt(int x, int y)
        {
            if (x >= 0 && x < Tiles.GetLength(0) && y >= 0 && y < Tiles.GetLength(1))
                return Tiles[x, y];
            return null;
        }

        private int[] FindTileCoordinate(Tile tileToFind)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++) {
                for (int x = 0; x < Tiles.GetLength(0); x++) {
                    if (Tiles[x, y] == tileToFind)
                        return new int[] {x, y};
                }
            }
            return null;
        }

        public Tile AdjacentTile(Tile startTile, Direction direction)
        {
            int[] startTileCoordinate = FindTileCoordinate(startTile);
            if (startTileCoordinate == null)
                return null;
            int x = startTileCoordinate[0];
            int y = startTileCoordinate[1];

            int xDelta, yDelta;
            int[] directionDeltas = GamePiece.GetDirectionDeltas(direction);
            xDelta = directionDeltas[0];
            yDelta = directionDeltas[1];

            int endTileX = x + xDelta;
            int endTileY = y + yDelta;
            if (endTileX >= 0 && endTileX < Tiles.GetLength(0) && endTileY >= 0 && endTileY < Tiles.GetLength(1))
                return Tiles[endTileX, endTileY];
            return null;
        }

        public LaserEmitter CurrentLaserEmitter
        {
            get {
                if (_khetGame.Players.Count > 0 && LaserEmitters.Count > 0)
                    // TODO: does this need to be opposite??
                    return (_khetGame.CurrentPlayer == _khetGame.Players[0] ? LaserEmitters[1] : LaserEmitters[0]);
                return null;
            }
        }
    }
}