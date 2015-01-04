using System;
using System.Collections.Generic;
using SwinGame;

namespace Khet
{
    public class RotateWidget: Widget
    {
        private bool _rotateClockwise;

        public RotateWidget(Dictionary<string, Bitmap> allImages, GamePiece piece, int zIndex, bool rotateClockwise):
            base(allImages, zIndex)
        {
            _rotateClockwise = rotateClockwise;
        }

        public override void Draw()
        {
            if (Active) {
                if (MouseHover) {
                    if (_rotateClockwise)
                        DrawWidgetImage(_allImages["RotateAntiClockwise_Hover"]);
                    else
                        DrawWidgetImage(_allImages["RotateClockwise_Hover"]);
                } else {
                    DrawWidgetImage(_currentImage);
                }
            }
        }
    }
}

