using System;
using System.Collections.Generic;
using SwinGame;

namespace Khet
{
    public class LaserEmitterWidget: Widget
    {
        public LaserEmitter LaserEmitter { get; private set; }

        public LaserEmitterWidget(Dictionary<string, Bitmap> allImages, GamePiece piece, int zIndex, KhetGame khetGame):
            base(allImages, zIndex)
        {

        }

        public override void Draw()
        {

        }
    }
}

