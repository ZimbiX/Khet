using System;

namespace Khet
{
    public abstract class PlayerAction
    {
        public GamePiece Piece { get; private set; }

        public PlayerAction(GamePiece piece)
        {
            Piece = piece;
        }
    }
}