using System;
using System.Collections.Generic;

namespace Khet
{
    public class Tile
    {
        public GamePiece InhabitingPiece { get; private set; }

        private Board _board;

        public Colour PieceColourRestriction { get; private set; }

        public TileWidget TileWidget { get; set; }

        public Tile(Board board = null, Colour pieceColourRestriction = (Colour)(-1))
        {
            InhabitingPiece = null;
            _board = board;
            TileWidget = null;
            PieceColourRestriction = pieceColourRestriction;
        }

        public void LandGamePiece(GamePiece piece)
        {
            if (HasInhabitingPiece) {
                if (InhabitingPiece.GetType() == typeof(Obelisk))
                if ((InhabitingPiece as Obelisk).HasObeliskAbove == false)
                    (piece as Obelisk).StackMeOnTopOf(InhabitingPiece as Obelisk);
            } else {
                InhabitingPiece = piece; // Set this tile's InhabitingPiece to the piece
                piece.LandOnTile(this); // Get the piece to set its InhabitedTile to this tile
            }
        }

        /// <summary>
        /// Takes the game piece from this tile.
        /// </summary>
        /// <returns>
        /// The game piece.
        /// </returns>
        public GamePiece TakeGamePiece()
        {
            GamePiece piece = InhabitingPiece;
            InhabitingPiece = null; // Set this tile's InhabitingPiece to null
            piece.LeaveCurrentTile(); // Get the piece to set its InhabitedTile to null
            return piece;
        }

        public bool HasInhabitingPiece
        {
            get { return (InhabitingPiece != null); }
        }

        public Tile AdjacentTile(Direction direction)
        {
            if (_board != null)
                return _board.AdjacentTile(this, direction);
            return null;
        }

        public bool CanMove(GamePiece piece)
        {
            if (!PieceColourRestriction.IsIn((Colour)(-1), piece.Colour))
                return false;
            if (piece is Djed) {
                if (HasInhabitingPiece)
                    return (InhabitingPiece.GetType().IsIn(typeof(Pyramid), typeof(Obelisk)));
                return true;
            } else if (piece is Obelisk) {
                if (HasInhabitingPiece == false)
                    return true;
                if (InhabitingPiece.GetType() == typeof(Obelisk))
                if ((InhabitingPiece as Obelisk).HasObeliskAbove == false)
                if ((piece as Obelisk).HasObeliskAbove == false)
                if (piece.Colour == InhabitingPiece.Colour)
                    return true;
                return false;
            } else {
                return (HasInhabitingPiece == false);
            }
        }

        public override string ToString()
        {
            if (HasInhabitingPiece) {
                return InhabitingPiece.ToString();
            } else {
                return "---";
            }
        }
    }
}