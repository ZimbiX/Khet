using System;

namespace Khet
{
    public class Obelisk: GamePiece
    {
        public Obelisk AboveInStack { get; private set; }
        public Obelisk BelowInStack { get; private set; }

        public bool HasObeliskAbove
        {
            get {
                return (AboveInStack != null);
            }
        }

        public Obelisk(Tile inhabitedTile, Colour colour, bool createObeliskAbove = false):
            base(inhabitedTile, colour)
        {
            AboveInStack = null;
            BelowInStack = null;
            if (createObeliskAbove)
                StackOnTopOfMe(new Obelisk(null, colour, this));
        }

        public Obelisk(Tile inhabitedTile, Colour colour, Obelisk belowInStack):
            base(inhabitedTile, colour)
        {
            StackMeOnTopOf(belowInStack);
        }

        public void StackMeOnTopOf(Obelisk toStackMeOnTopOf)
        {
            BelowInStack = toStackMeOnTopOf;
            BelowInStack.StackOnTopOfMe(this);
            if (PieceWidget != null)
                PieceWidget.Kill();
            //else { Console.WriteLine("Obelisk has null PieceWidget in StackThisOnTopOf"); }
        }

        public void StackOnTopOfMe(Obelisk toStackOnTopOfMe)
        {
            AboveInStack = toStackOnTopOfMe;
            if (PieceWidget != null)
                PieceWidget.UpdateImage();
        }

        public void UnstackThisFromTop()
        {
            if (PieceWidget != null)
                PieceWidget.Revive();
            BelowInStack = null;
            //else { Console.WriteLine("Obelisk has null PieceWidget in UnstackThisFromTop"); }
        }

        public void UnstackFromOnTopOfMe()
        {
            AboveInStack = null;
            if (PieceWidget != null)
                PieceWidget.UpdateImage();
        }

        public override bool Move(Direction direction)
        {
            Tile inhabitedTile = (InhabitedTile != null) ? InhabitedTile : BelowInStack.InhabitedTile;
            Tile destinationTile = inhabitedTile.AdjacentTile(direction);
            if (destinationTile == null)
                return false;
            if (!destinationTile.CanMove(this))
                return false;

            // If moving a single obelisk, or whole stack
            if (InhabitedTile != null) {
                InhabitedTile.TakeGamePiece();
            } else { // Unstacking the top obelisk
                BelowInStack.UnstackFromOnTopOfMe();
                if (BelowInStack.PieceWidget != null)
                    BelowInStack.PieceWidget.UpdateImage();
                UnstackThisFromTop();
            }
            destinationTile.LandGamePiece(this);
            return true;
        }
    }
}