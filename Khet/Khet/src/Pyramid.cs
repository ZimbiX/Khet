using System;

namespace Khet
{
    public class Pyramid: RotatableGamePiece
    {
        public Pyramid(Tile inhabitedTile, Colour colour, Direction direction):
            base(inhabitedTile, colour, direction) {}

        // Pyramid has standard movement, so no need to override GamePiece's Move method.
    }
}