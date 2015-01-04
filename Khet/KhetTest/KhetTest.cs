using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Khet
{
    [TestFixture()]
    public class KhetTest
    {
        // Test invalid; enums are, in fact, treated as regular integers
//        [Test()]
//        public void TestEnums()
//        {
//            Direction direction = (Direction)0;
//            direction--;
//            Assert.AreEqual((Direction)7, (Direction)direction, "Direction 0 minus 1 equals Direction 7");
//
//            Direction direction = (Direction)7;
//            direction++;
//            Assert.AreEqual((Direction)0, (Direction)direction, "Direction 7 plus 1 equals Direction 0");
//        }

        [Test()]
        public void TestGamePieceReturningInhabitedTile()
        {
            Tile tile = new Tile();
            GamePiece piece = new Pharaoh(tile, Colour.Red, Direction.North);
            Assert.AreSame(tile, piece.InhabitedTile, "Piece can return its inhabited tile");
        }

        [Test()]
        public void TestGamePieceReturningColour()
        {
            Tile tile = new Tile();
            GamePiece piece = new Pharaoh(tile, Colour.Red, Direction.North);
            Assert.AreEqual(Colour.Red, piece.Colour, "Piece can return its colour");
        }

        [Test()]
        public void TestGamePieceReturningDirection()
        {
            Tile tile = new Tile();
            GamePiece piece = new Pharaoh(tile, Colour.Red, Direction.North);
            Assert.AreEqual(Direction.North, piece.Direction, "Piece can return its direction");
        }

        [Test()]
        public void TestGamePieceReturningDescription()
        {
            Tile tile = new Tile();
            GamePiece piece = new Pharaoh(tile, Colour.Red, Direction.North);
            Assert.AreEqual("RP0", piece.ToString(), "Piece can return its description");
        }

        [Test()]
        public void TestGamePieceReturningDescriptionPieceType()
        {
            Tile tile = new Tile();
            Colour colour = Colour.Red;
            Direction direction = Direction.North;
            Dictionary<char, GamePiece> tests = new Dictionary<char, GamePiece>() {
                {'P', new Pharaoh(tile, colour, direction)},
                {'T', new Pyramid(tile, colour, direction)},
                {'D', new Djed(tile, colour, direction)},
                {'O', new Obelisk(tile, colour, false)}
            };
            foreach (var test in tests)
                Assert.AreEqual(test.Key, test.Value.ToString()[1], "Piece can return description with the correct " +
                    "piece type as a " + test.Value.GetType().ToString().Split('.')[1]);
        }

        [Test()]
        public void TestGamePieceReturningDescriptionColour()
        {
            foreach (Colour colour in new Colour[] {Colour.Silver, Colour.Red}) {
                char colourChar = colour.ToString()[0];
                Tile tile = new Tile();
                Direction direction = Direction.North;
                GamePiece[] tests = new GamePiece[] {
                    new Pharaoh(tile, colour, direction),
                    new Pyramid(tile, colour, direction),
                    new Djed(tile, colour, direction),
                    new Obelisk(tile, colour, false)
                };
                foreach (var test in tests)
                    Assert.AreEqual(colourChar, test.ToString()[0], "Piece can return description with the correct " +
                        "colour (" + colourChar + ") as a " + test.GetType().ToString().Split('.')[1]);
            }
        }

        [Test()]
        public void TestGamePieceReturningDescriptionDirection()
        {
            for (Direction direction = (Direction)2; direction < (Direction)7; direction++) {
                char directionChar = (char)(direction + '0');
                Tile tile = new Tile();
                Colour colour = Colour.Silver;
                GamePiece[] tests = new GamePiece[] {
                    new Pharaoh(tile, colour, direction),
                    new Pyramid(tile, colour, direction),
                    new Djed(tile, colour, direction),
                    new Obelisk(tile, colour, false)
                };
                foreach (var test in tests) {
                    if (test is Obelisk)
                        directionChar = '-';
                    Assert.AreEqual(directionChar, test.ToString()[2], "Piece can return description with the " +
                        "correct direction (" + directionChar + ") as a " + test.GetType().ToString().Split('.')[1]);
                }
            }
        }

        [Test()]
        public void TestObeliskReturningDescription()
        {
            Obelisk obelisk = new Obelisk(new Tile(), Colour.Silver, false);
            Assert.AreEqual("SO-", obelisk.ToString(), "Obelisk can return correct description with no obelisk " +
                "on top");
            obelisk = new Obelisk(new Tile(), Colour.Red, true);
            Assert.AreEqual("ROO", obelisk.ToString(), "Obelisk can return correct description with an obelisk " +
                "on top");
        }

        [Test()]
        public void TestGamePieceCreateFromString()
        {
            foreach (string pieceString in new string[] {"SP4", "RT7", "SD1", "RO-", "ROO", "RO-"}) {
                GamePiece piece = GamePiece.CreateFromString(pieceString, new Tile());
                Assert.IsNotNull(piece, "Can create piece from string using piece string: " + pieceString);
                Assert.AreEqual(pieceString, GamePiece.CreateFromString(pieceString, new Tile()).ToString(),
                                "Can create piece from string properly, using the piece string: " + pieceString);
            }
        }

        [Test()]
        public void TestGamePieceCreateFromStringInvalid()
        {
            foreach (string pieceString in new string[] {
                // Length:
                "", "T", "4", "RP", "P4", "ROOO", "RT11", "SO--",
                // Colour:
                "-P4", "XP4", 
                // Piece type:
                "SD0", "S-2", "S55", "R  ", "S0O",
                // Direction:
                "SP1", "SP3", "SP5", "SP7",                             // Pharaoh
                "RT0", "RT2", "RT4", "RT6",                             // Pyramid
                "SD0", "SD2", "SD3", "SD4", "SD5", "SD6",               // Djed
                "RO0", "RO1", "RO2", "RO3", "RO4", "RO5", "RO6", "RO7", // Obelisk
                // Obelisk character 3:
                "SOP", "SO ",
                // Misc:
                "   ", "---"
            }) {
                GamePiece piece = GamePiece.CreateFromString(pieceString, new Tile());
                Assert.IsNull(piece, "Correctly returns null when asked to create piece from string using " +
                    "piece string: " + pieceString);
            }
        }

        [Test()]
        public void TestBoardLoadLayoutFromString() // Classic
        {
            string layoutString =
                "--- --- --- --- ROO RP4 ROO RT3 --- ---\n" +
                "--- --- RT5 --- --- --- --- --- --- ---\n" +
                "--- --- --- ST7 --- --- --- --- --- ---\n" +
                "RT1 --- ST5 --- RD1 RD7 --- RT3 --- ST7\n" +
                "RT3 --- ST7 --- SD7 SD1 --- RT1 --- ST5\n" +
                "--- --- --- --- --- --- RT3 --- --- ---\n" +
                "--- --- --- --- --- --- --- ST1 --- ---\n" +
                "--- --- ST7 SOO SP0 SOO --- --- --- ---";
            Board board = new Board();
            Assert.IsTrue(board.LoadLayoutFromString(BoardLayoutNames.Classic, layoutString), "Board can load " +
                "a layout from a string");
            board.ArrangePieces(BoardLayoutNames.Classic);
            Assert.AreEqual(layoutString, board.ToString(), "Board can generate and return a description that is " +
                "the same layout string that it loaded");
        }

        [Test()]
        public void TestBoardLoadLayoutFromStringFiles()
        {
            for (BoardLayoutNames layoutName = 0;
                 layoutName < (BoardLayoutNames)Enum.GetNames(typeof(BoardLayoutNames)).Length; layoutName++) {
                string filename = "Layout_" + layoutName.ToString() + ".txt";
                DirectoryInfo mainPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent;
                string path = Path.Combine(mainPath.ToString(), filename);
                string layoutString = File.ReadAllText(path);
                Board board = new Board();
                Assert.IsTrue(board.LoadLayoutFromString(BoardLayoutNames.Classic, layoutString), "Board can load " +
                    "a layout from a string (manually from file: " + filename + ")");
                board.ArrangePieces(BoardLayoutNames.Classic);
                Assert.AreEqual(layoutString, board.ToString(), "Board can generate and return a description that is " +
                    "the same layout string that it loaded (manually from file: " + filename + ")");
            }
        }

        [Test()]
        public void TestBoardLoadLayoutFromFile()
        {
            string filename = "Layout_Classic.txt";
            Board board = new Board();
            Assert.IsTrue(board.LoadLayoutFromFile(BoardLayoutNames.Classic, filename), "Board can load a layout " +
                "from file");
            board.ArrangePieces(BoardLayoutNames.Classic);
            DirectoryInfo mainPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent;
            string path = Path.Combine(mainPath.ToString(), filename);
            Assert.AreEqual(System.IO.File.ReadAllText(path), board.ToString(), "Board can return layout " +
                "that was loaded from file");
        }

        public Dictionary<Direction, int[]> GetDirectionDeltas()
        {
            return new Dictionary<Direction, int[]> {
                { (Direction)0, new int[] { 0, -1} },
                { (Direction)1, new int[] { 1, -1} },
                { (Direction)2, new int[] { 1,  0} },
                { (Direction)3, new int[] { 1,  1} },
                { (Direction)4, new int[] { 0,  1} },
                { (Direction)5, new int[] {-1,  1} },
                { (Direction)6, new int[] {-1,  0} },
                { (Direction)7, new int[] {-1, -1} }
            };
        }

        [Test()]
        public void TestBoardAndTileAdjacentTile()
        {
            int startTileX = 4;
            int startTileY = 5;
            Dictionary<Direction, int[]> directionDeltas = GetDirectionDeltas();
            foreach (var directionDelta in directionDeltas) {
                Board board = new Board();
                Tile startTile = board.TileAt(startTileX, startTileY);
                int endTileX = startTileX+directionDelta.Value[0];
                int endTileY = startTileY+directionDelta.Value[1];
                Tile endTile = board.TileAt(endTileX, endTileY);
                Assert.AreSame(endTile, startTile.AdjacentTile(directionDelta.Key), "Board (through start tile) can " +
                    "return the tile in the given direction for coordinates: start:"+startTileX+","+startTileY+
                    " end:"+endTileX+","+endTileY+" with direction: "+directionDelta.Key.ToString());
            }
        }

        public void TestBoadAndTileAdjacentTileInvalidCheckTileInDirection(Board board, int x, int y,
                                                                           Direction edgeDirection)
        {
            Tile startTile = board.TileAt(x, y);
            foreach (Direction directon in new Direction[] {
                // Also get the direction to either side of the given one, correcting for value boundaries
                ((int)edgeDirection > 0 ? edgeDirection : edgeDirection + 8) - 1,
                edgeDirection,
                ((int)edgeDirection < 7 ? edgeDirection : edgeDirection - 8) + 1
            }) {
                Assert.IsNull(startTile.AdjacentTile(directon), "No tile returned for edge tile ("+x+","+y+") " +
                    "in direction "+directon.ToString());
            }
        }

        [Test()]
        public void TestBoardAndTileAdjacentTileInvalid()
        {
            Board board = new Board();
            int x, y;
            Direction edgeDirection;
            // Top:
            y = 0;
            edgeDirection = Direction.North;
            for (x = 0; x < 10; x++) {
                TestBoadAndTileAdjacentTileInvalidCheckTileInDirection(board, x, y, edgeDirection);
            }
            // Bottom:
            y = 7;
            edgeDirection = Direction.South;
            for (x = 0; x < 10; x++) {
                TestBoadAndTileAdjacentTileInvalidCheckTileInDirection(board, x, y, edgeDirection);
            }
            // Left:
            x = 0;
            edgeDirection = Direction.West;
            for (y = 0; y < 8; y++) {
                TestBoadAndTileAdjacentTileInvalidCheckTileInDirection(board, x, y, edgeDirection);
            }
            // Right:
            x = 9;
            edgeDirection = Direction.East;
            for (y = 0; y < 8; y++) {
                TestBoadAndTileAdjacentTileInvalidCheckTileInDirection(board, x, y, edgeDirection);
            }
        }

        [Test()]
        public void TestTile()
        {
            Tile tile = new Tile();
            Assert.AreEqual(typeof(Tile), tile.GetType(), "Tile can be created");
        }

        [Test()]
        public void TestObeliskStackedMove()
        {
            string layoutString =
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- SOO --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---";
            Board board = new Board();
            board.LoadLayoutFromString(BoardLayoutNames.Classic, layoutString);
            board.ArrangePieces(BoardLayoutNames.Classic);

            Assert.IsTrue(board.TileAt(1,1).HasInhabitingPiece, "Tile recognises stacked obelisks as something " +
                "inhabiting it");
            Assert.AreEqual("SOO", board.TileAt(1,1).InhabitingPiece.ToString(), "Tile describes stacked obelisks " +
                "as stacked obelisks");
            board.TileAt(1,1).InhabitingPiece.Move(Direction.SouthEast);
            Assert.AreEqual("---", board.TileAt(1,1).ToString(), "Tile is vacant after having stacked obelisks " +
                "leave it");
            Assert.AreEqual("SOO", board.TileAt(2,2).ToString(), "Tile moves stacked obelisks correctly");
        }

        [Test()]
        public void TestObeliskUnstackedMove()
        {
            string layoutString =
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- SO- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---";
            Board board = new Board();
            board.LoadLayoutFromString(BoardLayoutNames.Classic, layoutString);
            board.ArrangePieces(BoardLayoutNames.Classic);

            Assert.IsTrue(board.TileAt(1,1).HasInhabitingPiece && board.TileAt(1,1).HasInhabitingPiece, "Tile " +
                "recognises unstacked obelisk as something inhabiting it");
            Assert.AreEqual(null, (board.TileAt(1,1).InhabitingPiece as Obelisk).AboveInStack, "Unstacked obelisk " +
                "knows the obelisk above it is null");
            Assert.AreEqual("SO-", board.TileAt(1,1).InhabitingPiece.ToString(), "Tile describes an unstacked obelisk " +
                "as an unstacked obelisk");
            board.TileAt(1,1).InhabitingPiece.Move(Direction.North);
            Assert.AreEqual("---", board.TileAt(1,1).ToString(), "Tile is vacant after having stacked obelisks " +
                "leave it");
            Assert.AreEqual("SO-", board.TileAt(1,0).ToString(), "Tile moves unstacked obelisks correctly");
        }

        [Test()]
        public void TestObeliskUnstack()
        {
            string layoutString =
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- SOO --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---";
            Board board = new Board();
            board.LoadLayoutFromString(BoardLayoutNames.Classic, layoutString);
            board.ArrangePieces(BoardLayoutNames.Classic);

            (board.TileAt(2,2).InhabitingPiece as Obelisk).AboveInStack.Move(Direction.West);
            Assert.AreEqual("SO-", board.TileAt(2,2).ToString(), "A single obelisk remains after having the one " +
                "above it unstacked from on top");
            Assert.IsFalse((board.TileAt(2,2).InhabitingPiece as Obelisk).HasObeliskAbove, "Single obelisk remaining " +
                "after unstack recognises that there is no obelisk above it");
            Assert.IsNull((board.TileAt(2,2).InhabitingPiece as Obelisk).AboveInStack, "Single obelisk remaining " +
                "after unstack recognises that the obelisk above is null");

            Assert.AreEqual("SO-", board.TileAt(1,2).ToString(), "Obelisk unstacked from top moves to its new tile " +
                "correctly");
            Assert.IsNull((board.TileAt(1,2).InhabitingPiece as Obelisk).BelowInStack, "Obelisk unstacked from top " +
                "knows that the obelisk below it is null");
        }

        [Test()]
        public void TestObeliskStack()
        {
            string layoutString =
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- SO- --- --- --- --- --- --- --- ---\n" +
                "--- SO- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---\n" +
                "--- --- --- --- --- --- --- --- --- ---";
            Board board = new Board();
            board.LoadLayoutFromString(BoardLayoutNames.Classic, layoutString);
            board.ArrangePieces(BoardLayoutNames.Classic);

            Obelisk obeliskBottom = (board.TileAt(1,1).InhabitingPiece as Obelisk);
            Obelisk obeliskTop = (board.TileAt(1,2).InhabitingPiece as Obelisk);
            (board.TileAt(1,2).InhabitingPiece as Obelisk).Move(Direction.North);

            Assert.AreEqual("SOO", board.TileAt(1,1).ToString(), "An obelisk can be stacked on top of another");

            Assert.IsNull((board.TileAt(1,1).InhabitingPiece as Obelisk).BelowInStack, "Bottom obelisk knows that " +
                "the one below it is null");
            Assert.IsNull((board.TileAt(1,1).InhabitingPiece as Obelisk).AboveInStack.AboveInStack, "Top obelisk " +
                "knows that the one above it is null");

            Assert.AreSame(obeliskTop, (board.TileAt(1,1).InhabitingPiece as Obelisk).AboveInStack, "Bottom obelisk " +
                "correctly sets the one above it");
            Assert.AreSame(obeliskBottom, obeliskTop.BelowInStack, "Top obelisk correctly sets the one below it");
        }
    }
}