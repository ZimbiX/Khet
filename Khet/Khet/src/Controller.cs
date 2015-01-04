using System;
using SwinGame;
using System.Collections.Generic;

namespace Khet
{
    public class Controller
    {
        public KhetGame _khetGame;

        private View _view;

        private List<Widget> _widgets;

        public PieceWidget Selected { get; private set; }

        public Controller(KhetGame khetGame, View view)
        {
            _view = view;
            _khetGame = khetGame;
            _khetGame.AddPlayer("Player 1");
            _khetGame.AddPlayer("Player 2");
            _widgets = _view.Widgets;
            // Assign event handlers to widgets
            foreach (Widget widget in _widgets) {
                if (widget.GetType() == typeof(PieceWidget)) {
                    widget.EventHandlerOnMouseClick = PieceWidgetOnMouseClick;
                    widget.EventHandlerOnMouseHover = PieceWidgetOnMouseHover;
                }
                if (widget.GetType() == typeof(TileWidget)) {
                    widget.EventHandlerOnMouseClick = TileWidgetOnMouseClick;
                    widget.EventHandlerOnMouseHover = TileWidgetOnMouseHover;
                }
            }
        }

        public bool Run()
        {
            if (Input.WindowCloseRequested())
                return false;
            Widget widgetUnderMouse = FindWidgetUnderMouse();
            if (_khetGame.Interactible && !_khetGame.GameFinished) {
//                if (Input.KeyDown(KeyCode.vk_LCTRL))
//                    Console.WriteLine("asdljaksd");
                if (Input.MouseClicked(MouseButton.LeftButton)) {
                    Console.WriteLine("Clicked - PointIntersecting for each widget executing");
                    // If clicked on a widget
                    if (widgetUnderMouse != null)
                        RunEventHandler(widgetUnderMouse, widgetUnderMouse.EventHandlerOnMouseClick);
                    else
                        ClearMoveOptions();
                } else if (widgetUnderMouse != null) {
                    RunEventHandler(widgetUnderMouse, widgetUnderMouse.EventHandlerOnMouseHover);
                }
                if (Selected != null) {
                    PieceWidget selected = Selected;
                    if (Input.KeyTyped(KeyCode.vk_z)) {
                        Console.WriteLine("Z key typed - Checking whether anti-clockwise rotation is available");
                        RotatePieceIfSelected(false);
                        selected.UpdateImage();
                    } else if (Input.KeyTyped(KeyCode.vk_x)) {
                        Console.WriteLine("X key typed - Checking whether clockwise rotation is available");
                        RotatePieceIfSelected(true);
                        selected.UpdateImage();
                    }
                }
            }
            if (Input.KeyTyped(KeyCode.vk_ESCAPE)) {
                if (!_khetGame.Interactible && !_khetGame.GameFinished) {
                    _khetGame.EndLaser();
                } else if (Selected != null) {
                    Selected.Selected = false;
                    ClearMoveOptions();
                    Selected = null;
                } else {
                    return false;
                }
            }
            return true;
        }

        public void ClearMoveOptions()
        {
            foreach (Widget widget in _widgets) {
                if (widget.GetType() == typeof(TileWidget)) {
                    TileWidget tileWidget = (widget as TileWidget);
                    tileWidget.MoveOption = false;
                    tileWidget.MoveOptionDirection = (Direction)(-1);
                    tileWidget.MovePiece = null;
                } else if (widget.GetType() == typeof(PieceWidget)) {
                    PieceWidget pieceWidget = (widget as PieceWidget);
                    pieceWidget.Selected = false;
                    Selected = null;
                }
            }
        }

        public void SetUpMoveOptions(PieceWidget clickedPieceWidget)
        {
            Tile pieceWidgetTileOn = clickedPieceWidget.Piece.InhabitedTile;

            clickedPieceWidget.Selected = true;
            Selected = clickedPieceWidget;
            for (Direction direction = (Direction)0; direction <= (Direction)7; direction++) {
                Tile adjacentTile = pieceWidgetTileOn.AdjacentTile(direction);
                if (adjacentTile != null) {
                    if (adjacentTile.CanMove(clickedPieceWidget.Piece))
                        SetAsMoveOption(adjacentTile.TileWidget, direction, clickedPieceWidget.Piece);
                }
            }
        }

        private void SetAsMoveOption(TileWidget tileWidget, Direction direction, GamePiece piece)
        {
            if (tileWidget != null) {
                tileWidget.MoveOption = true;
                tileWidget.MoveOptionDirection = direction;
                tileWidget.MovePiece = piece;
            }
        }

        public void RotatePieceIfSelected(bool clockwise)
        {
            if (Selected != null) {
                // (rotatability checked within):
                PlayerAction playerAction = new PlayerActionRotate(Selected.Piece, clockwise);
                if (_khetGame.PerformPlayerAction(playerAction))
                    ClearMoveOptions();
            }
        }

        private Widget FindWidgetUnderMouse()
        {
            // Find the top widget that is under the mouse, then return it
            Widget widgetUnderMouse = null;
            int widgetZIndex = 999999999; // A lower (more negative) zIndex is layered on top
            foreach (Widget widget in _widgets) {
                widget.MouseHover = false;
                if (widget.PointIntersecting(Input.MousePosition()))
                    // Check if this widget is above (has a lower zIndex than) the one already chosen
                    if (widget.ZIndex < widgetZIndex) {
                        widgetUnderMouse = widget;
                        widgetZIndex = widget.ZIndex;
                    }
            }
            return widgetUnderMouse;
        }

        private void RunEventHandler(Widget widget, Widget.EventHandler eventHandler)
        {
            if (eventHandler != null)
                eventHandler(widget);
        }

        private void PerformPieceMove(PieceWidget pieceWidget, Direction direction)
        {
            Console.WriteLine("Board before move:");
            Console.WriteLine(_view.Board.ToString());

            GamePiece piece = pieceWidget.Piece;
            if (Input.KeyDown(KeyCode.vk_LCTRL)/* || Input.KeyTyped(KeyCode.vk_RSHIFT)*/)
            if (piece is Obelisk)
            if ((piece as Obelisk).HasObeliskAbove)
                piece = (piece as Obelisk).AboveInStack;
            if (_khetGame.PerformPlayerAction(new PlayerActionMove(piece, direction))) {
                piece.PieceWidget.Move(direction);
                Console.WriteLine("Move successful");
                Console.WriteLine("Board after move:");
                Console.WriteLine(_view.Board.ToString());
            } else {
                Console.WriteLine("Move denied");
            }
        }

        private void PieceWidgetOnMouseClick(Widget widget)
        {
            Console.WriteLine("Clicked on PieceWidget - PieceWidgetOnMouseClick executing");
            PieceWidget clickedPieceWidget = widget as PieceWidget;
            Tile pieceWidgetTileOn = clickedPieceWidget.Piece.InhabitedTile;

            // Check if this piece's tile is currently a move option
            if (pieceWidgetTileOn.TileWidget.MoveOption) {
                // Perform the move
                PerformPieceMove(pieceWidgetTileOn.TileWidget.MovePiece.PieceWidget,
                                 pieceWidgetTileOn.TileWidget.MoveOptionDirection);
                ClearMoveOptions();
            } else {
                if (clickedPieceWidget.Selected) {
                    // Currently selected, but clicked again, so deselect
                    ClearMoveOptions();
                } else {
                    ClearMoveOptions();
                    // Not currently a move option or a selected piece, so select this piece and set up its
                    // move options from the current tile
                    if (clickedPieceWidget.Piece.Colour == _khetGame.CurrentPlayer.Colour) {
                        SetUpMoveOptions(clickedPieceWidget);
                    }
                }
            }
        }

        private void TileWidgetOnMouseClick(Widget widget)
        {
            Console.WriteLine("Clicked on TileWidget - TileWidgetOnMouseClick executing");
            TileWidget tileWidget = widget as TileWidget;
            // Check if this tile is currently a move option
            if (tileWidget.MoveOption) {
                // Perform the move
                PerformPieceMove(tileWidget.MovePiece.PieceWidget,
                                 tileWidget.MoveOptionDirection);
            }
            ClearMoveOptions();
        }

        private void TileWidgetOnMouseHover(Widget widget)
        {
            TileWidget tileWidget = widget as TileWidget;
            // Check if this tile is currently a move option
            //tileWidget.DrawMouseHover();
            tileWidget.MouseHover = true;
        }

        private void PieceWidgetOnMouseHover(Widget widget)
        {
            PieceWidget pieceWidget = widget as PieceWidget;
            // Check that there is no piece selected yet, and this piece is owned by the current player
            //pieceWidget.DrawMouseHover();
            pieceWidget.MouseHover = true;
        }
    }
}