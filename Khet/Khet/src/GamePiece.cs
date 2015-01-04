using System;

namespace Khet
{
    public enum Direction {
        North = 0,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

    public abstract class GamePiece
    {
        public Tile InhabitedTile { get; protected set; }

        public Colour Colour { get; protected set; }

        public Direction Direction { get; protected set; }

        public PieceWidget PieceWidget { get; set; }

        public GamePiece(Tile inhabitedTile, Colour colour)
        {
            InhabitedTile = inhabitedTile;
            if (InhabitedTile != null)
                InhabitedTile.LandGamePiece(this);
            Colour = colour;
        }

        public GamePiece(Tile inhabitedTile, Colour colour, Direction direction)
        {
            InhabitedTile = inhabitedTile;
            if (InhabitedTile != null)
                InhabitedTile.LandGamePiece(this);
            Colour = colour;
            Direction = direction;
        }

        public void LandOnTile(Tile tile)
        {
            InhabitedTile = tile;
        }

        public void LeaveCurrentTile()
        {
            InhabitedTile = null;
        }

        public virtual bool Move(Direction direction)
        {
            Tile destinationTile = InhabitedTile.AdjacentTile(direction);
            if (destinationTile == null)
                return false;
            if (!destinationTile.CanMove(this))
                return false;
            destinationTile.LandGamePiece(InhabitedTile.TakeGamePiece());
            return true;
        }

        public static GamePiece CreateFromString(string pieceString, Tile location)
        {
            if (pieceString.Length != 3)
                // Invalid piece string length, so do not create a piece here
                return null;

            char colourChar = pieceString[0];
            char pieceTypeChar = pieceString[1];
            char directionChar = pieceString[2];

            // Set up colour

            Colour colour;
            if (colourChar == 'R') {
                colour = Colour.Red;
            } else if (colourChar == 'S') {
                colour = Colour.Silver;
            } else {
                // Invalid colour, so do not create a piece here
                return null;
            }
            
            // Set up direction

            int direction = 0;
            // Ignore direction setup for an obelisk
            if (pieceTypeChar != 'O') {
                try {
                    direction = Convert.ToInt32(directionChar - '0');
                    if (direction < 0 || direction > 7)
                        // Invalid direction (out of range), so do not create a piece here
                        return null;
                } catch {
                    // Invalid direction (not a number), so do not create a piece here
                    return null;
                }
            }

            // Create the piece based on the piece type

            switch (pieceTypeChar) {
                case 'P':
                    if (direction.IsIn(0, 2, 4, 6))
                        return new Pharaoh(location, colour, (Direction)direction);
                    break;
                case 'T': // 't' for 'triangle', as 'p' is already taken
                    if (direction.IsIn(1, 3, 5, 7))
                        return new Pyramid(location, colour, (Direction)direction);
                    break;
                case 'D':
                    if (direction.IsIn(1, 7))
                        return new Djed(location, colour, (Direction)direction);
                    break;
                case 'O':
                    if (directionChar.IsIn('O', '-'))
                        // Create the obelisk, and if the piece string specifies a second obelisk, make it 
                        // create and stack another one on top of itself
                        return new Obelisk(location, colour, (directionChar == 'O'));
                    break;
            }
            // Invalid piece type or direction for that piece type, so do not create a piece here
            return null;
        }

        public static int[] GetDirectionDeltas(Direction direction)
        {
            int xDelta, yDelta;
            switch ((int)direction) {
                case 0:
                    xDelta = 0;
                    yDelta = -1;
                    break;
                case 1:
                    xDelta = 1;
                    yDelta = -1;
                    break;
                case 2:
                    xDelta = 1;
                    yDelta = 0;
                    break;
                case 3:
                    xDelta = 1;
                    yDelta = 1;
                    break;
                case 4:
                    xDelta = 0;
                    yDelta = 1;
                    break;
                case 5:
                    xDelta = -1;
                    yDelta = 1;
                    break;
                case 6:
                    xDelta = -1;
                    yDelta = 0;
                    break;
                case 7:
                    xDelta = -1;
                    yDelta = -1;
                    break;
                default:
                    Console.Error.WriteLine("Error: invalid direction for finding adjacent tile: " + direction);
                    return null;
            }
            return new int[] {xDelta, yDelta};
        }

        public override string ToString()
        {
            string description = "";

            // Position 1: Colour
            switch (this.Colour) {
                case Colour.Red:
                    description += "R";
                    break;
                case Colour.Silver:
                    description += "S";
                    break;
                default:
                    description += "?";
                    break;
            }

            // Position 2: Piece type
            if (this is Pharaoh) {
                description += "P";
            } else if (this is Pyramid) {
                description += "T";
            } else if (this is Djed) {
                description += "D";
            } else if (this is Obelisk) {
                description += "O";
            } else {
                description += "?";
            }

            // Position 3: Direction
            // Check if the piece is an obelisk. These do not require a direction, and so store stack
            // information in this position
            if (this.GetType() == typeof(Obelisk)) {
                if ((this as Obelisk).AboveInStack != null)
                    description += "O";
                else
                    description += "-";
            } else { // The piece is not an obelisk, so add the direction as normal
                description += (int)(this as GamePiece).Direction;
            }

            return description;
        }
    }
}