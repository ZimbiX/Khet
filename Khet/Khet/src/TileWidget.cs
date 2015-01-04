using System;
using SwinGame;
using System.Collections.Generic;

namespace Khet
{
    public class TileWidget: Widget
    {
        public Tile Tile { get; set; }

        private Bitmap _pieceColourRestrictionImage;

        public bool MoveOption { get; set; }

        public GamePiece MovePiece { get; set; }

        public Direction MoveOptionDirection { get; set; }

        public TileWidget(Dictionary<string, Bitmap> allImages, Tile tile, Point2D position, int zIndex):
            base(allImages, zIndex)
        {
            _currentImage = _allImages["Tile"];
            Tile = tile;
            Tile.TileWidget = this;
            MoveOption = false;
            Position = position;
            if (tile.PieceColourRestriction != (Colour)(-1))
                _pieceColourRestrictionImage = _allImages["Tile_" +
                                                          tile.PieceColourRestriction.ToString()];
        }

        public override void Draw()
        {
            if (Active) {
                DrawWidgetImage(_currentImage);
                DrawWidgetImage(_pieceColourRestrictionImage);
                if (!Tile.HasInhabitingPiece)
                    DrawIfMoveOption();
            }
        }

        public void DrawIfMoveOption()
        {
            // Drawn either:
            //   - [When inhabited:    ] By the piece, after it has drawn itself, so this is on top of it
            //   - [When not inhabited:] By this, after it has drawn itself
            if (MoveOption) {
                DrawWidgetImage(_allImages["Tile_MoveOption"]);
                if (MouseHover) {
                    DrawWidgetImage(_allImages["Tile_Hover"]);
                }
            }
        }
    }
}