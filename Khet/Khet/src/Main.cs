using System;
using System.Linq;
using System.Reflection;
using SwinGame;
using Color = System.Drawing.Color;

namespace Khet
{

    // From: http://www.thecodepage.com/post/Quick-Tip-A-generic-In-method.aspx
    public static class ExtensionMethods 
    { 
        public static bool IsIn<T>(this T source, params T[] values) 
        {
            if (values == null) { return false; }
            return values.Contains(source);
        }
    }

    class MainClass
    {
        public static KhetGame CreateGame(Point2D boardBorder, BoardLayoutNames layoutName)
        {
            KhetGame game = new KhetGame(2000);
            game.Board.ArrangePieces(layoutName); // TODO: make selectable
            game.InitialiseViewAndController(boardBorder);
            return game;
        }

        public static void Main()
        {
            // Start the audio system so sound can be played
            Audio.OpenAudio();

            int tileWidth = 92;
            int tileHeight = 92;
            Point2D boardBorder = KhetGame.Point2D(50, 50);
            int bottomBorder = 200;
            
            // Open the game window
            Graphics.OpenGraphicsWindow("Khet",
                                        tileWidth*10 + (int)boardBorder.X*2,
                                        tileHeight*8 + (int)boardBorder.Y + bottomBorder);
            //Graphics.ShowSwinGameSplashScreen();

            KhetGame game = CreateGame(boardBorder, BoardLayoutNames.Classic);

            bool quitRequested = false;
            // Run the game loop
            while(quitRequested == false)
            {
                for (int i = 0; i < 1; i++) { // CPU test
                    // Fetch the next batch of UI interaction
                    Input.ProcessEvents();

                    // Switch layout if requested
                    if (Input.KeyTyped(KeyCode.vk_1))
                        game = CreateGame(boardBorder, BoardLayoutNames.Classic);
                    else if (Input.KeyTyped(KeyCode.vk_2))
                        game = CreateGame(boardBorder, BoardLayoutNames.Imhotep);
                    else if (Input.KeyTyped(KeyCode.vk_3))
                        game = CreateGame(boardBorder, BoardLayoutNames.Dynasty);

                    // Process the game
                    game.Run();
                    quitRequested = !game.Controller.Run();
                    
                    // Clear the screen and draw the framerate
                    Graphics.ClearScreen(Color.White);
                    game.View.Draw();
                    Text.DrawFramerate(0,0);
                }
                
                // Draw onto the screen
                Graphics.RefreshScreen(60);
            }
            
            // End the audio
            Audio.CloseAudio();
            
            // Close any resources we were using
            Resources.ReleaseAllResources();
        }
    }
}