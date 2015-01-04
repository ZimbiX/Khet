using System;
using SwinGame;
using System.Collections.Generic;

namespace Khet
{
    public class PieceWidget: Widget
    {
        public GamePiece Piece { get; private set; }

        public bool Selected { get; set; }

        private KhetGame _khetGame;

        public PieceWidget(Dictionary<string, Bitmap> allImages, GamePiece piece, int zIndex, KhetGame khetGame):
            base(allImages, zIndex)
        {
            Piece = piece;
            Piece.PieceWidget = this;
            Selected = false;
            if (piece is Obelisk)
            if ((piece as Obelisk).BelowInStack != null)
                piece = (piece as Obelisk).BelowInStack;
            Position = piece.InhabitedTile.TileWidget.Position;
            UpdateImage();
            _khetGame = khetGame;
        }

        public void Move(Direction direction)
        {
            Point2D originPosition = Position;

            int xDelta, yDelta;
            int[] directionDeltas = GamePiece.GetDirectionDeltas(direction);
            xDelta = directionDeltas[0];
            yDelta = directionDeltas[1];

            int destinationX = (int)originPosition.X + xDelta*(_allImages["Tile"].Width);
            int destinationY = (int)originPosition.Y + yDelta*(_allImages["Tile"].Height);

            Point2D destinationPosition = KhetGame.Point2D(destinationX, destinationY);
            Position = destinationPosition;
        }

        public override void Revive()
        {
            base.Revive();
            if (Piece.InhabitedTile != null)
                Position = Piece.InhabitedTile.TileWidget.Position;
            else
                Position = (Piece as Obelisk).BelowInStack.InhabitedTile.TileWidget.Position;
        }

        public void UpdateImage()
        {
            _currentImage = _allImages[Piece.ToString()];
        }

        public override void Draw()
        {
            if (Active) {
                DrawWidgetImage(_currentImage);
                if (Selected) {
                    DrawWidgetImage(_allImages["GamePiece_Selected"]);
                }
                Piece.InhabitedTile.TileWidget.DrawIfMoveOption(); // TODO: fix hover over piece when move option
                if (MouseHover/* && PointIntersecting(Input.MousePosition())*/) { // WHY THE NEED TO RE-CHECK THE POINT?!?!?!?!?
                    if ((_khetGame.Controller.Selected == null && _khetGame.CurrentPlayer.Colour == Piece.Colour) ||
                        (Piece.InhabitedTile.TileWidget.MoveOption))
                        DrawWidgetImage(_allImages["Tile_Hover"]);
    //                Console.WriteLine("Drawing hover image since MoveOption and MouseHover: " +
    //                                  System.DateTime.Now.ToLongTimeString());
                }
            }
        }
    }
}