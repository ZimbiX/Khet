using System;

namespace Khet
{
    public class Pharaoh: GamePiece
    {
        public Pharaoh(Tile inhabitedTile, Colour colour, Direction direction):
            base(inhabitedTile, colour, direction) {}

        // Pharaoh has standard movement, so no need to override GamePiece's Move method.
    }
}