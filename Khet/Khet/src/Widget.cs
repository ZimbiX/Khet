using System;
using SwinGame;
using System.Collections.Generic;

namespace Khet
{
    public abstract class Widget
    {
        protected Dictionary<string, Bitmap> _allImages;

        protected Bitmap _currentImage;

        public Point2D Position { get; set; }

        public bool MouseHover { get; set; }

        public int ZIndex { get; private set; }

        public bool Active { get; set; }

        public delegate void EventHandler(Widget widget);
        public EventHandler EventHandlerOnMouseHover { get; set; }
        public EventHandler EventHandlerOnMouseDown { get; set; }
        public EventHandler EventHandlerOnMouseUp { get; set; }
        public EventHandler EventHandlerOnMouseClick { get; set; }

        public int Width {
            get { 
                if (_currentImage != null)
                    return _currentImage.Width;
                else
                    return 0;
            }
        }

        public int Height {
            get { 
                if (_currentImage != null)
                    return _currentImage.Height;
                else
                    return 0;
            }
        }

        protected Point2D PositionForImage(Bitmap image)
        {
            float x = Position.X;
            float y = Position.Y;
            float imageWidth = (float)image.Width;
            float imageHeight = (float)image.Height;
            float imageX = x - imageWidth/2;
            float imageY = y - imageHeight/2;
            return KhetGame.Point2D(imageX, imageY);
        }

        public Widget(Dictionary<string, Bitmap> allImages, int zIndex)
        {
            _allImages = allImages;
            MouseHover = false;
            ZIndex = zIndex;
            Active = true;
        }

        public virtual bool PointIntersecting(Point2D pointToCheck)
        {
            return (pointToCheck.X >= Position.X - Width/2 &&
                    pointToCheck.X <  Position.X + Width/2 &&
                    pointToCheck.Y >= Position.Y - Height/2 &&
                    pointToCheck.Y <  Position.Y + Height/2);
        }

        public void Kill()
        {
            Active = false;
            Position = KhetGame.Point2D(-9999,-9999);
        }

        public virtual void Revive()
        {
            Active = true;
        }

        protected void DrawWidgetImage(Bitmap image)
        {
            if (image != null)
                image.Draw(PositionForImage(image));
        }

        public abstract void Draw();
    }
}