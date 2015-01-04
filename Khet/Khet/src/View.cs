using System;
using System.Collections.Generic;
using SwinGame;
using System.Drawing;
using Bitmap = SwinGame.Bitmap;
using Graphics = SwinGame.Graphics;

namespace Khet
{
    public class View
    {
        // TODO: Convert to using the one Dictionary<string, Bitmap> with the same standardised piece naming as
        // used in the layouts, with another character:
        //   N - Normal
        //   H - MouseHover
        //   D - MouseDown
        // Extra images:
        //   
        //public Dictionary<Type, Dictionary<string, Bitmap[]>> AllImages { get; private set; }
        public Dictionary<string, Bitmap> AllImages { get; private set; }

        public List<Widget> Widgets;

        public Point2D BoardBorder;

        public Board Board { get; private set; }

        private KhetGame _khetGame;

        private int _zIndexMin;
        private int _zIndexMax;

        //private SwinGame.Font _font;

        public View(KhetGame khetGame, Board board, Point2D boardBorder)
        {
            _zIndexMin = 0;
            _zIndexMax = 0;

            BoardBorder = boardBorder;
            LoadImages();

            //_font = 
            Text.LoadFontNamed("arial", "arial.ttf", 14);

            Board = board;
            _khetGame = khetGame;
            Widgets = new List<Widget>();

            Bitmap tileImage = AllImages["Tile"];
            int tileWidth = tileImage.Width;
            int tileHeight = tileImage.Height;

            for (int tileY = 0; tileY < 8; tileY++) {
                for (int tileX = 0; tileX < 10; tileX++) {
                    Tile tile = board.TileAt(tileX, tileY);
                    float x = BoardBorder.X + tileX * tileWidth + tileWidth/2;
                    float y = BoardBorder.Y + tileY * tileHeight + tileHeight/2;
                    Point2D position = KhetGame.Point2D(x, y);
                    Widgets.Add(new TileWidget(AllImages, tile, position, -1));
                    if (tile.HasInhabitingPiece) {
                        Widgets.Add(new PieceWidget(AllImages, tile.InhabitingPiece, -2, khetGame));
                        if (tile.InhabitingPiece is Obelisk) {
                            Obelisk obeliskAbove = (tile.InhabitingPiece as Obelisk).AboveInStack;
                            if (obeliskAbove != null) {
                                PieceWidget obeliskWidget = new PieceWidget(AllImages,
                                                                     obeliskAbove, -2, khetGame);
                                Widgets.Add(obeliskWidget);
                                obeliskAbove.PieceWidget = obeliskWidget;
                                obeliskAbove.PieceWidget.Kill();
                            }
                        }
                    }
                }
            }
        }

        private void LoadImages()
        {
            AllImages = new Dictionary<string, Bitmap>();

            foreach (string pieceImageTypeAndDirection in new string[] {
                "P0", "P2", "P4", "P6",
                "T1", "T3", "T5", "T7",
                "D1", "D7",
                "O-", "OO"
            })
                foreach (string colour in new string[] {"S", "R"})
                    AllImages.Add(colour + pieceImageTypeAndDirection, LoadBitmap(colour + pieceImageTypeAndDirection));

            foreach (string otherImageID in new string[] {
                "Tile", "Tile_Red", "Tile_Silver", "Tile_MoveOption", "Tile_Hover", "Tile_MoveOption_Highlighted",
                "GamePiece_Selected"
            })
                AllImages.Add(otherImageID, LoadBitmap(otherImageID));
        }

        private Bitmap LoadBitmap(string filename)
        {
            filename += ".png";
            Bitmap bitmap = null;
            try {
                bitmap = Images.LoadBitmap(filename);
            } catch {
                Console.Error.WriteLine("Bitmap file not found: " + filename);
            }
            return bitmap;
        }

        public void Draw()
        {
            for (int zIndex = _zIndexMax; zIndex >= _zIndexMin; zIndex--) {
                foreach (Widget widget in Widgets) {
                    // Adjust min and max zIndex values for the next draw/rest of this one
                    if (widget.ZIndex < _zIndexMin)
                        _zIndexMin = widget.ZIndex;
                    else if (widget.ZIndex > _zIndexMax)
                        _zIndexMax = widget.ZIndex;
                    // If the widget is at the current zIndex level, draw it
                    if (widget.ZIndex == zIndex)
                        widget.Draw();
                }
            }
//            foreach (Widget widget in Widgets)
//                if (widget.GetType() == typeof(TileWidget))
//                    widget.Draw();
//            foreach (Widget widget in Widgets)
//                if (widget.GetType() == typeof(PieceWidget))
//                    widget.Draw();
            if (_khetGame.Players.Count > 0) {
                Color silverPlayerTextColour = ColorTranslator.FromHtml("#BBBBBB");
                Color redPlayerTextColour    = ColorTranslator.FromHtml("#FF0000");
                if (!_khetGame.GameFinished) {
                    // Display who's turn it is
                    Color textColour = (_khetGame.CurrentPlayer == _khetGame.Players[0]) ?
                                        silverPlayerTextColour : redPlayerTextColour;
                    Text.DrawText("Current turn:", Color.Black,
                                  KhetGame.Point2D(20, 20));
                    Text.DrawText(_khetGame.CurrentPlayer.Colour.ToString(), textColour,
                                  KhetGame.Point2D(135, 20));
                    // Display key info
                    string keyInfo = "Rotate selected piece:\n" +
                        "    Z: Anti-clockwise\n" +
                        "    X: Clockwise\n" +
                        "Unstack selected obelisk:\n" +
                        "    Ctrl + click destination tile\n" +
                        "Start a new game:\n" +
                        "    1: Classic\n" +
                        "    2: Imhotep\n" +
                        "    3: Dynasty";
                    //Text.DrawText(keyInfo, Color.Black, KhetGame.Point2D(20, AllImages["Tile"].Height*8 + 85));
                    Text.DrawTextLines(keyInfo, Color.Black, Color.Transparent, Text.FontNamed("arial"),
                                       FontAlignment.AlignLeft, 30, AllImages["Tile"].Height*8 + 75, 1000, 500);
                } else {
                    Color textColour = (_khetGame.Victor == _khetGame.Players[0]) ?
                                        silverPlayerTextColour : redPlayerTextColour;
                    Text.DrawText("Winner:", Color.Black,
                                  KhetGame.Point2D(20, 20));
                    Text.DrawText(_khetGame.Victor.Colour.ToString(), textColour,
                                  KhetGame.Point2D(85, 20));
                }
            }

            if (_khetGame.Players.Count > 0 && Board.LaserEmitters.Count > 0) {
                LaserEmitter laserEmitter = (_khetGame.CurrentPlayer == _khetGame.Players[0] ? Board.LaserEmitters[1] :
                                             Board.LaserEmitters[0]);
                DrawLaser(laserEmitter);
            }
        }

        public void DrawLaser(LaserEmitter laserEmitter)
        {
            if (laserEmitter.LaserPath.Count > 0) {
                int xDelta, yDelta;
                int[] directionDeltas = GamePiece.GetDirectionDeltas(KhetGame.ReverseDirection(laserEmitter.Direction));
                xDelta = directionDeltas[0];
                yDelta = directionDeltas[1];

                xDelta *= AllImages["Tile"].Width / 2;
                yDelta *= AllImages["Tile"].Height / 2;

                Point2D laserEmitterPosition = KhetGame.Point2D(laserEmitter.AttachedTile.TileWidget.Position.X + xDelta, 
                                                                laserEmitter.AttachedTile.TileWidget.Position.Y + yDelta);

                List<Point2D[]> laserLinesForeground = new List<Point2D[]>();

                laserLinesForeground.Add(DrawLaserLineBGAndGetFG(laserEmitterPosition,
                    laserEmitter.AttachedTile.TileWidget.Position));
                
                for (int i = 0; i < laserEmitter.LaserPath.Count-1; i++) {
                    Point2D p1 = laserEmitter.LaserPath[i].TileWidget.Position;
                    Point2D p2 = laserEmitter.LaserPath[i+1].TileWidget.Position;
                    laserLinesForeground.Add(DrawLaserLineBGAndGetFG(p1, p2));
                }
                Tile finalDestination = laserEmitter.LaserPath[laserEmitter.LaserPath.Count-1];
                if (finalDestination.InhabitingPiece == null &&
                    finalDestination.AdjacentTile(laserEmitter.FinalLaserDirection) == null) {
                    Point2D p1 = finalDestination.TileWidget.Position;

                    directionDeltas = GamePiece.GetDirectionDeltas(laserEmitter.FinalLaserDirection);
                    xDelta = directionDeltas[0];
                    yDelta = directionDeltas[1];

                    xDelta *= AllImages["Tile"].Width / 2;
                    yDelta *= AllImages["Tile"].Height / 2;

                    Point2D p2 = KhetGame.Point2D(p1.X + xDelta, p1.Y + yDelta);

                    laserLinesForeground.Add(DrawLaserLineBGAndGetFG(p1, p2));
                }
                DrawLaserLinesFG(laserLinesForeground);
            }
        }

        public Point2D[] DrawLaserLineBGAndGetFG(Point2D p1, Point2D p2)
        {
            Color laserColourEdge = ColorTranslator.FromHtml("#FF0000");
            if (p1.X == p2.X) { // if laser line is vertical
                Graphics.DrawLine(laserColourEdge, p1.X+1, p1.Y, p2.X+1, p2.Y);
                Graphics.DrawLine(laserColourEdge, p1.X-1, p1.Y, p2.X-1, p2.Y);
            } else { // laser line is horizontal
                Graphics.DrawLine(laserColourEdge, p1.X, p1.Y+1, p2.X, p2.Y+1);
                Graphics.DrawLine(laserColourEdge, p1.X, p1.Y-1, p2.X, p2.Y-1);
            }
            return new Point2D[] {p1, p2};
        }

        public void DrawLaserLinesFG(List<Point2D[]> laserLinesForeground)
        {
            Color laserColourCentre = ColorTranslator.FromHtml("#AC1315");
            foreach (Point2D[] laserLineForeground in laserLinesForeground) {
                Graphics.DrawLine(laserColourCentre, laserLineForeground[0], laserLineForeground[1]);
            }
        }
    }
}