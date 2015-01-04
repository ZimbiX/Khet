using System;

namespace Khet
{
    public class PlayerActionMove: PlayerAction
    {
        public Direction Direction { get; private set; }

        public PlayerActionMove(GamePiece piece, Direction direction): base(piece)
        {
            Direction = direction;
        }
    }
}