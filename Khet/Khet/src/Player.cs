using System;

namespace Khet
{
    public enum Colour {
        Silver = 1,
        Red
    }

    public class Player
    {
        public string Name { get; private set; }

        public int Wins { get; private set; }

        public Colour Colour { get; private set; }

        public LaserEmitter LaserEmitter { get; private set; }

        public Player(string name, Colour colour/*, LaserEmitter laserEmitter*/)
        {
            Name = name;
            Colour = colour;
            /*LaserEmitter = laserEmitter;*/
            Wins = 0;
        }
    }
}