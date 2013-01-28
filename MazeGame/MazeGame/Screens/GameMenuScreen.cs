using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeGame.Screens
{
    class GameMenuScreen : MenuScreen
    {
        public GameMenuScreen()
            : base("Maze Size")
        {
            // Create the menu entries
            MenuEntry fiveEntry    = new MenuEntry("5x5");
            MenuEntry tenEntry     = new MenuEntry("10x10");
            MenuEntry fifteenEntry = new MenuEntry("15x15");
            MenuEntry twentyEntry  = new MenuEntry("20x20");

            // Hook up event handlers
            fiveEntry.Selected    += fiveEntrySelected;
            tenEntry.Selected     += tenEntrySelected;
            fifteenEntry.Selected += fifteenEntrySelected;
            twentyEntry.Selected  += twentyEntrySelected;

            // Add entries to the menu
            MenuEntries.Add(fiveEntry);
            MenuEntries.Add(tenEntry);
            MenuEntries.Add(fifteenEntry);
            MenuEntries.Add(twentyEntry);
        }

        void fiveEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(5));
        }

        void tenEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // Create and load a 10x10 maze screen
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                   new GameplayScreen(10));
        }

        void fifteenEntrySelected(object sender, PlayerIndexEventArgs e)
        { 
            // Create and load a 15x15 maze screen
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                   new GameplayScreen(15));
        }

        void twentyEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // Create and load a 20x20 maze screen
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(20));
        }
    }
}
