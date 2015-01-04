using System;

namespace Khet
{
    public abstract class RotatableGamePiece: GamePiece
    {
        public RotatableGamePiece(Tile inhabitedTile, Colour colour, Direction direction):
            base(inhabitedTile, colour)
        {
            Direction = direction;
        }

        public virtual void Rotate(bool clockwise)
        {
            int directionDelta = clockwise ? 2 : -2;
            Direction += directionDelta;
            if (Direction < (Direction)0)
                Direction += 8;
            else if (Direction > (Direction)7)
                Direction -= 8;
        }

        public void RotateClockwise()
        {
            Rotate(true);
        }

        public void RotateAnticlockwise()
        {
            Rotate(false);
        }
    }
}