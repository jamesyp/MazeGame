using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MazeGame.Screens
{
    /// <summary>
    /// the main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen()
            : base("Main Menu")
        {
            // Create the menu entries
            MenuEntry playEntry = new MenuEntry("Play");
            MenuEntry highScoresEntry = new MenuEntry("High Scores");
            MenuEntry creditsEntry = new MenuEntry("Credits");
            MenuEntry exitEntry = new MenuEntry("Exit");

            // Hook up menu event handlers
            playEntry.Selected       += PlayEntrySelected;
            highScoresEntry.Selected += HighScoresEntrySelected;
            creditsEntry.Selected    += CreditsEntrySelected;
            exitEntry.Selected       += OnCancel;

            // Add entries to the menu
            MenuEntries.Add(playEntry);
            MenuEntries.Add(highScoresEntry);
            MenuEntries.Add(creditsEntry);
            MenuEntries.Add(exitEntry);
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new GameMenuScreen(), e.PlayerIndex);
        }

        void HighScoresEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //ScreenManager.AddScreen(new HighScoresScreen(), e.PlayerIndex);
        }

        void CreditsEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new CreditsScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        /// <param name="playerIndex"></param>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}
