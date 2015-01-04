using System;

namespace Khet
{
    public class Djed: RotatableGamePiece
    {
        public Djed(Tile inhabitedTile, Colour colour, Direction direction):
            base(inhabitedTile, colour, direction) {}

        public override bool Move(Direction direction)
        {
            Tile destinationTile = InhabitedTile.AdjacentTile(direction);
            if (destinationTile == null)
                return false;
            // Remove self from board
            Tile originTile = InhabitedTile;
            InhabitedTile.TakeGamePiece();
            if (destinationTile.HasInhabitingPiece) {
                // Swap was requested - check if legal to swap with piece type
                if (destinationTile.InhabitingPiece.GetType().IsIn(typeof(Pyramid), typeof(Obelisk))) {
                    // Move other piece into our old position
                    GamePiece otherPiece = destinationTile.InhabitingPiece;
                    Direction oppositeDirection = KhetGame.ReverseDirection(direction);
                    otherPiece.Move(oppositeDirection);
                    otherPiece.PieceWidget.Move(oppositeDirection);
                } else {
                    // Swap with this piece type is illegal
                    originTile.LandGamePiece(this); // Re-place this piece in its old position
                    return false;
                }
            }
            // Place self in new position
            destinationTile.LandGamePiece(this);
            return true;
        }

        public override void Rotate(bool clockwise)
        {
            // Only valid values are 1 and 7
            if (Direction == (Direction)1)
                Direction = (Direction)7;
            else
                Direction = (Direction)1;
        }
    }
}