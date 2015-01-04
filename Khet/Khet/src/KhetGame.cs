using System;
using SwinGame;
using System.Collections.Generic;
//using System.Timers;
//using Timer = System.Timers.Timer;
using Timer = SwinGame.Timer;

namespace Khet
{
    public class KhetGame
    {
        public Board Board { get; private set; }

        public View View { get; private set; }

        public Controller Controller { get; private set; }

        public List<Player> Players { get; private set; }

        public Player CurrentPlayer { get; private set; }

        private Timer _laserFinishTimer;

        private int _laserDuration;

        public Player Victor { get; private set; }

        public bool GameFinished
        {
            get {
                return (Victor != null);
            }
        }

        public bool Interactible { get; private set; }

        public static Dictionary<Type, Dictionary<string, int>> ReflectionResults { get; private set; }

        public KhetGame(int laserDuration = 3000)
        {
            Board = new Board(this);
            Players = new List<Player>();
            Interactible = true;
            _laserDuration = laserDuration;
            _laserFinishTimer = Timers.CreateTimer();

            // Set up reflection results for use by a LaserEmitter when calculating the path of its laser.
            // piece type, piece direction, laser approach direction, laser departure direction
            ReflectionResults = new Dictionary<Type, Dictionary<string, int>>();
            ReflectionResults.Add(typeof(Pyramid), new Dictionary<string, int>());
            ReflectionResults.Add(typeof(Djed), new Dictionary<string, int>());

            Type pieceType = typeof(Pyramid);
            ReflectionResults[pieceType].Add("1,0", 2);
            ReflectionResults[pieceType].Add("1,2", 0);
            ReflectionResults[pieceType].Add("1,4", -1);
            ReflectionResults[pieceType].Add("1,6", -1);
            ReflectionResults[pieceType].Add("3,0", -1);
            ReflectionResults[pieceType].Add("3,2", 4);
            ReflectionResults[pieceType].Add("3,4", 2);
            ReflectionResults[pieceType].Add("3,6", -1);
            ReflectionResults[pieceType].Add("5,0", -1);
            ReflectionResults[pieceType].Add("5,2", -1);
            ReflectionResults[pieceType].Add("5,4", 6);
            ReflectionResults[pieceType].Add("5,6", 4);
            ReflectionResults[pieceType].Add("7,0", 6);
            ReflectionResults[pieceType].Add("7,2", -1);
            ReflectionResults[pieceType].Add("7,4", -1);
            ReflectionResults[pieceType].Add("7,6", 0);

            pieceType = typeof(Djed);
            ReflectionResults[pieceType].Add("1,0", 2);
            ReflectionResults[pieceType].Add("1,2", 0);
            ReflectionResults[pieceType].Add("1,4", 6);
            ReflectionResults[pieceType].Add("1,6", 4);
            ReflectionResults[pieceType].Add("7,0", 6);
            ReflectionResults[pieceType].Add("7,2", 4);
            ReflectionResults[pieceType].Add("7,4", 2);
            ReflectionResults[pieceType].Add("7,6", 0);
        }

        public static Point2D Point2D(float x, float y)
        {
            Point2D p = new Point2D();
            p.X = x;
            p.Y = y;
            return p;
        }

        public static Direction ReverseDirection(Direction initialDirection)
        {
            Direction oppositeDirection = initialDirection + 4;
            if (oppositeDirection > (Direction)7)
                oppositeDirection -= (Direction)8;
            return oppositeDirection;
        }

        public void InitialiseViewAndController(Point2D boardBorder)
        {
            View = new View(this, Board, boardBorder);
            Controller = new Controller(this, View);
        }

        public bool AddPlayer(string name)
        {
            if (Players.Count < 2) {
                Colour colour = (Players.Count == 0 ? Colour.Silver : Colour.Red);
                Players.Add(new Player(name, colour/*, new LaserEmitter(new Tile())*/));
                CurrentPlayer = Players[0];
                return true;
            }
            return false;
        }

        public bool PerformPlayerAction(PlayerAction playerAction)
        {
            bool playerActionSuccess = false;
            if (playerAction is PlayerActionRotate) {
                // Piece rotation
                if (playerAction.Piece is RotatableGamePiece) {
                    bool clockwise = (playerAction as PlayerActionRotate).Clockwise;
                    if (playerAction.Piece.Colour == CurrentPlayer.Colour) {
                        (playerAction.Piece as RotatableGamePiece).Rotate(clockwise);
                        playerActionSuccess = true;
                    } else {
                        playerActionSuccess = false;
                    }
                }
            } else {
                // Piece move
                Direction direction = (playerAction as PlayerActionMove).Direction;
                if (playerAction.Piece.Colour == CurrentPlayer.Colour)
                    playerActionSuccess = playerAction.Piece.Move(direction);
                else
                    playerActionSuccess = false;
            }
            if (playerActionSuccess)
                FinaliseTurn();
            return playerActionSuccess;
        }

        public void FinaliseTurn()
        {
            // Fire teh laser
            Board.CurrentLaserEmitter.Fire();
            Interactible = false;
            _laserFinishTimer.Start();
        }

        public void Run()
        {
            if (_laserFinishTimer.Ticks > 0)
            if (_laserFinishTimer.Ticks >= _laserDuration) {
                EndLaser();
            }
        }

        public void EndLaser()
        {
            _laserFinishTimer.Stop(); // Also resets the timer to 0
            EndTurn();
        }

        public void EndTurn()
        {
            // Remove any illuminated piece(s)
            if (Board.CurrentLaserEmitter.FinalDestination != null) {
                GamePiece piece = Board.CurrentLaserEmitter.FinalDestination.InhabitingPiece;
                if (piece is Obelisk) {
                    if ((piece as Obelisk).HasObeliskAbove) {
                        piece = (piece as Obelisk).AboveInStack;
                        (piece as Obelisk).BelowInStack.UnstackFromOnTopOfMe();
                    }
                }
                if (piece.InhabitedTile != null)
                    piece.InhabitedTile.TakeGamePiece();
                if (piece.PieceWidget != null)
                    piece.PieceWidget.Kill();
                if (piece.GetType() == typeof(Pharaoh))
                    // Declare the other player victor (the player who does not own the pharoah just killed)
                    Victor = Players[(piece.Colour == Colour.Silver) ? 1 : 0];
            }

            // Turn off the laser
            Board.CurrentLaserEmitter.Clear();

            // Resume play, or end the game
            if (GameFinished) {
                GameEnd();
            } else {
                // Alternate the current player
                CurrentPlayer = Players[(CurrentPlayer == Players[0]) ? 1 : 0];
                Interactible = true;
            }
        }

        public void GameEnd()
        {
            // TODO ..............................
        }
    }
}