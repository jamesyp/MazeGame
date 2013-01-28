using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MazeGame.Screens
{
    class CreditsScreen : MenuScreen
    {
        private string[] jobTitles;
        private int currentTitle;
        private string name = "James Patton";
        public CreditsScreen()
            : base("Credits")
        {
            jobTitles = new string[] { "Written By", "Produced By", "Directed By",
                                       "Lead Designer", "Lead Artist", "QA Director",
                                        "Marketing", "Special Thanks To"};

            currentTitle = 0;
        }

        protected override void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            currentTitle++;
            if (currentTitle >= jobTitles.Length)
                currentTitle = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            Color color = Color.Teal;            
            // Modify the alpha to fade out text during transitions.
            color *= TransitionAlpha;

            string jobTitle = jobTitles[currentTitle];

            // Calculate text positions
            Vector2 origin = new Vector2(0, font.LineSpacing / 2);
            Vector2 titlePosition = new Vector2(0f, 150f);
            titlePosition.X = graphics.Viewport.Width / 2 - font.MeasureString(jobTitle).X;

            Vector2 namePosition = new Vector2(0, titlePosition.Y + ScreenManager.Font.LineSpacing);
            namePosition.X = graphics.Viewport.Width / 2 - font.MeasureString(name).X;

            spriteBatch.Begin();

            spriteBatch.DrawString(font, jobTitle, titlePosition, color);
            spriteBatch.DrawString(font, name, namePosition, color);

            spriteBatch.End();
        }
    }
}
