using System;
using System.Collections.Generic;
using SwinGame;
using System.Drawing;
using Graphics = SwinGame.Graphics;

namespace Khet
{
    public class LaserEmitter
    {
        public Tile AttachedTile { get; private set; }

        public Direction Direction { get; private set; } // doesn't change after initialisation

        public List<Tile> LaserPath { get; private set; }

        public Direction FinalLaserDirection { get; private set; }

        public Tile FinalDestination { get; private set; }

        public LaserEmitter(Tile attachedTile, bool laserFiresNorth)
        {
            AttachedTile = attachedTile;
            Direction = (laserFiresNorth ? Direction.North : Direction.South);
            LaserPath = new List<Tile>();
        }

        public void Fire()
        {
            LaserPath.Clear();
            LaserPath.Add(AttachedTile);
            Tile laserCurrentTile = AttachedTile;
            Direction laserCurrentDirection = Direction;
            Direction directionToNextTile = GetDirectionToNextTile(laserCurrentTile, laserCurrentDirection);
            while (directionToNextTile != KhetGame.ReverseDirection(laserCurrentDirection)) {
                laserCurrentDirection = directionToNextTile;
                laserCurrentTile = laserCurrentTile.AdjacentTile(directionToNextTile);
                if (laserCurrentTile == null)
                    break;
                LaserPath.Add(laserCurrentTile);
                directionToNextTile = GetDirectionToNextTile(laserCurrentTile, laserCurrentDirection);
            }
            FinalLaserDirection = laserCurrentDirection;
            FinalDestination = laserCurrentTile;
        }

        public Direction GetDirectionToNextTile(Tile tileBeingApproached, Direction approachDirection)
        {
            GamePiece piece = tileBeingApproached.InhabitingPiece;
            if (piece == null)
                return approachDirection;

            int reflectionResult = -9999;
            if ((piece is Pyramid) || (piece is Djed)) {
                reflectionResult = KhetGame.ReflectionResults[piece.GetType()][(int)piece.Direction + "," +
                                                              (int)KhetGame.ReverseDirection(approachDirection)];
            } else if (piece is Obelisk) {
                reflectionResult = -1;
            } else if (piece is Pharaoh) {
                reflectionResult = -1;
            }

            // If a valid reflected direction has been determined (if the laser hit a mirrored side)
            if (reflectionResult != -1)
                return (Direction)reflectionResult;
            else // If it hit a non-mirrored side, return the opposite direction as a signal of reflection failure
                return KhetGame.ReverseDirection((Direction)approachDirection);
        }

        public void Clear()
        {
            LaserPath.Clear();
            FinalDestination = null;
        }
    }
}