using System;

namespace Khet
{
    public class PlayerActionRotate: PlayerAction
    {
        public bool Clockwise { get; private set; }

        public PlayerActionRotate(GamePiece piece, bool clockwise): base(piece)
        {
            Clockwise = clockwise;
        }
    }
}