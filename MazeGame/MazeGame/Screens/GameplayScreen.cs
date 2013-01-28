using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Microsoft.Xna.Framework;

namespace MazeGame.Screens
{
    /// <summary>
    /// This screen implements teh actual game logic.
    /// </summary>
    class GameplayScreen : GameScreen
    {
        ContentManager content;
        SpriteFont gameFont;

        // The maze itself
        Maze maze;

        // Maze textures
        Texture2D threeWalls;
        Texture2D twoWallsCorner;
        Texture2D twoWallsOpposite;
        Texture2D oneWall;
        Texture2D noWalls;

        // Shortest Path
        Texture2D shortestPathOpposite;
        Texture2D shortestPathCorner;
        bool drawShortestPath = false;
        bool drawBreadcrumbs = false;
        bool drawScore = true;

        // The current cell the player is in
        Cell playerPosition;
        Texture2D player;
        TimeSpan elapsedPlayTime = new TimeSpan();

        float pauseAlpha;

        InputAction pauseAction;
        InputAction toggleShortestPathAction;
        InputAction toggleBreadcrumbsAction;
        InputAction toggleScoreAction;

        InputAction playerMoveUpAction;
        InputAction playerMoveDownAction;
        InputAction playerMoveLeftAction;
        InputAction playerMoveRightAction;


        InputAction showHintAction;


        public GameplayScreen(int mazeSize)
        {
            maze = new Maze(mazeSize);
            playerPosition = maze.Cells[0, 0];

            TransitionOnTime  = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            toggleShortestPathAction = new InputAction(
                new Buttons[] { Buttons.RightTrigger },
                new Keys[] { Keys.P },
                true);

            toggleBreadcrumbsAction = new InputAction(
                new Buttons[] { Buttons.LeftTrigger },
                new Keys[] { Keys.B },
                true);

            toggleScoreAction = new InputAction(
                new Buttons[] { Buttons.Y },
                new Keys[] { Keys.Y },
                true);

            playerMoveUpAction = new InputAction(
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new Keys[] { Keys.Up, Keys.W },
                true);

            playerMoveDownAction = new InputAction(
                new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new Keys[] { Keys.Down, Keys.S },
                true);

            playerMoveLeftAction = new InputAction(
                new Buttons[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft },
                new Keys[] { Keys.Left, Keys.A },
                true);

            playerMoveRightAction = new InputAction(
                new Buttons[] { Buttons.DPadRight, Buttons.LeftThumbstickRight },
                new Keys[] { Keys.Right, Keys.D },
                true);

            showHintAction = new InputAction(
                new Buttons[] { Buttons.X },
                new Keys[] { Keys.H }, 
                false);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("gamefont");

                threeWalls = content.Load<Texture2D>("threewalls");
                twoWallsCorner = content.Load<Texture2D>("twowallscorner");
                twoWallsOpposite = content.Load<Texture2D>("twowallsopposite");
                oneWall = content.Load<Texture2D>("onewall");
                noWalls = content.Load<Texture2D>("nowalls");

                shortestPathCorner = content.Load<Texture2D>("shortestpathcorner");
                shortestPathOpposite = content.Load<Texture2D>("shortestpathopposite");

                player = content.Load<Texture2D>("player");

                // A real game would probably have more content than this sample, so
                // it would take longer to load. We simulate that by delaying for awhile.
                // Thread.Sleep(1000);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            content.Unload();
        }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                elapsedPlayTime += gameTime.ElapsedGameTime;
                if (playerPosition == maze.LastCell)
                {
                    ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                }
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                if (toggleShortestPathAction.Evaluate(input, ControllingPlayer, out player))
                    drawShortestPath =  drawShortestPath ?  false : true;

                if (toggleBreadcrumbsAction.Evaluate(input, ControllingPlayer, out player))
                    drawBreadcrumbs = drawBreadcrumbs ? false : true;

                if (toggleScoreAction.Evaluate(input, ControllingPlayer, out player))
                    drawScore = drawScore ? false : true;

                // TODO: add player movement actions
                if (playerMoveUpAction.Evaluate(input, ControllingPlayer, out player))
                {
                    playerPosition = (playerPosition.up != null) ? playerPosition.up : playerPosition;
                }
                if (playerMoveDownAction.Evaluate(input, ControllingPlayer, out player))
                {
                    playerPosition = (playerPosition.down != null) ? playerPosition.down : playerPosition;
                }
                if (playerMoveLeftAction.Evaluate(input, ControllingPlayer, out player))
                {
                    playerPosition = (playerPosition.left != null) ? playerPosition.left : playerPosition;
                }
                if (playerMoveRightAction.Evaluate(input, ControllingPlayer, out player))
                {
                    playerPosition = (playerPosition.right != null) ? playerPosition.right : playerPosition;
                }
                
            }

        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);
            
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            string message = maze.Size.ToString();
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);

            // Center a square the size of the viewport height
            Rectangle mazeRectangle = new Rectangle((viewport.Width / 2) - (viewport.Height) / 2, 0, viewport.Height, viewport.Height);
                 
            spriteBatch.Begin();

            DrawShortestPath(spriteBatch, mazeRectangle);
            DrawPlayer(spriteBatch, mazeRectangle);
            DrawCells(spriteBatch, mazeRectangle);
            DrawElapsedPlayTime(spriteBatch);
            DrawScore(spriteBatch, viewportSize);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        private Rectangle SetCellRectangle(Rectangle mazeRectangle, Cell cell, int mazeSize)
        {
            Rectangle cellRectangle = new Rectangle();
            int cellSize = mazeRectangle.Width / mazeSize;          // mazeRectangle should always be a square so width vs. height doesn't matter

            cellRectangle.X = mazeRectangle.X + (int)cell.position.X * cellSize;
            cellRectangle.Y = mazeRectangle.Y + (int)cell.position.Y * cellSize;
            cellRectangle.Width = cellSize;
            cellRectangle.Height = cellSize;

            return cellRectangle;
        }

        private void DrawElapsedPlayTime(SpriteBatch spriteBatch)
        {
            string timeDisplay = elapsedPlayTime.ToString("mm':'ss");

            Vector2 textPos = new Vector2(0, 0);

            spriteBatch.DrawString(gameFont, timeDisplay, textPos, Color.White, 0, textPos, 0.85f, SpriteEffects.None, 0);

            return;
        }

        private void DrawScore(SpriteBatch spriteBatch, Vector2 viewportSize)
        {
            string score = "9999";
            int textWidth = (int)gameFont.MeasureString(score).X;
            Vector2 textPos = new Vector2(viewportSize.X - textWidth, 0);

            if (drawScore)
                spriteBatch.DrawString(gameFont, score, textPos, Color.White);
        }

        private void DrawPlayer(SpriteBatch spriteBatch, Rectangle mazeRectangle)
        {
            Rectangle cellRectangle = SetCellRectangle(mazeRectangle, playerPosition, maze.Size);

            spriteBatch.Draw(player, cellRectangle, Color.White);
        }

        private void DrawCells(SpriteBatch spriteBatch, Rectangle mazeRectangle)
        {
            Rectangle cellRectangle = new Rectangle();
            Texture2D drawTexture = ScreenManager.BlankTexture;
            float drawRotation = 0;
            Vector2 drawOrigin = new Vector2(0, 0);

            foreach (Cell currentCell in maze.Cells)
            {
                switch (currentCell.Type)
                {
                    // Three walls
                    case Cell.CellType.ThreeWallsOpenUp:
                        drawTexture = threeWalls;
                        drawRotation = 0;
                        break;
                    case Cell.CellType.ThreeWallsOpenRight:
                        drawTexture = threeWalls;
                        drawRotation = (float)Math.PI / 2;
                        break;
                    case Cell.CellType.ThreeWallsOpenDown:
                        drawTexture = threeWalls;
                        drawRotation = (float)Math.PI;
                        break;
                    case Cell.CellType.ThreeWallsOpenLeft:
                        drawTexture = threeWalls;
                        drawRotation = (float)Math.PI / 2 * -1;
                        break;
                    // Two walls, corners
                    case Cell.CellType.TwoWallsUpRight:
                        drawTexture = twoWallsCorner;
                        drawRotation = 0;
                        break;
                    case Cell.CellType.TwoWallsRightDown:
                        drawTexture = twoWallsCorner;
                        drawRotation = (float)Math.PI / 2;
                        break;
                    case Cell.CellType.TwoWallsDownLeft:
                        drawTexture = twoWallsCorner;
                        drawRotation = (float)Math.PI;
                        break;
                    case Cell.CellType.TwoWallsLeftUp:
                        drawTexture = twoWallsCorner;
                        drawRotation = (float)Math.PI * 3 / 2;
                        break;

                    // Two walls, opposites
                    case Cell.CellType.TwoWallsUpDown:
                        drawTexture = twoWallsOpposite;
                        drawRotation = 0;
                        break;
                    case Cell.CellType.TwoWallsLeftRight:
                        drawTexture = twoWallsOpposite;
                        drawRotation = (float)Math.PI / 2;
                        break;

                    // One wall
                    case Cell.CellType.OneWallUp:
                        drawTexture = oneWall;
                        drawRotation = 0;
                        break;
                    case Cell.CellType.OneWallRight:
                        drawTexture = oneWall;
                        drawRotation = (float)Math.PI / 2;
                        break;
                    case Cell.CellType.OneWallDown:
                        drawTexture = oneWall;
                        drawRotation = (float)Math.PI;
                        break;
                    case Cell.CellType.OneWallLeft:
                        drawTexture = oneWall;
                        drawRotation = (float)Math.PI / 2 * -1;
                        break;

                    // No walls
                    case Cell.CellType.AllOpen:
                        drawTexture = noWalls;
                        drawRotation = 0;
                        break;
                }
                cellRectangle = SetCellRectangle(mazeRectangle, currentCell, maze.Size);
                cellRectangle.X += (mazeRectangle.Width / maze.Size) / 2;
                cellRectangle.Y += (mazeRectangle.Height / maze.Size) / 2;
                drawOrigin = new Vector2(drawTexture.Width / 2, drawTexture.Height / 2);
                spriteBatch.Draw(drawTexture, cellRectangle, null, Color.White, drawRotation, drawOrigin, SpriteEffects.None, 0);
            }

            return;
        }

        public void DrawShortestPath(SpriteBatch spriteBatch, Rectangle mazeRectangle)
        {
            Rectangle cellRectangle = new Rectangle();
            Texture2D drawTexture = ScreenManager.BlankTexture;
            float drawRotation = 0;
            Vector2 drawOrigin = new Vector2(0, 0);

            if (drawShortestPath)
            {
                foreach (Cell pathCell in maze.ShortestPath)
                {
                    if (pathCell.previous == null)      // This is the first cell
                    {
                        if (pathCell.next == pathCell.right)
                        {
                            drawTexture = shortestPathOpposite;
                            drawRotation = (float)Math.PI / 2;
                        }
                        if (pathCell.next == pathCell.down)
                        {
                            drawTexture = shortestPathCorner;
                            drawRotation = (float)Math.PI;
                        }
                    }
                    else if (pathCell.next == null)          // This is the last cell
                    {
                        if (pathCell.previous == pathCell.up)
                        {
                            drawTexture = shortestPathCorner;
                            drawRotation = 0;
                        }
                        if (pathCell.previous == pathCell.left)
                        {
                            drawTexture = shortestPathOpposite;
                            drawRotation = (float)Math.PI / 2;
                        }
                    }
                    else                                    // Somewhere in the middle
                    {
                        if (pathCell.previous == pathCell.up)
                        {
                            if (pathCell.next == pathCell.right)
                            {
                                drawTexture = shortestPathCorner;
                                drawRotation = 0;
                            }
                            if (pathCell.next == pathCell.down)
                            {
                                drawTexture = shortestPathOpposite;
                                drawRotation = 0;
                            }
                            if (pathCell.next == pathCell.left)
                            {
                                drawTexture = shortestPathCorner;
                                drawRotation = (float)Math.PI / 2 * -1;
                            }
                        }

                        if (pathCell.previous == pathCell.right)
                        {
                            if (pathCell.next == pathCell.down)
                            {
                                drawTexture = shortestPathCorner;
                                drawRotation = (float)Math.PI / 2;
                            }
                            if (pathCell.next == pathCell.left)
                            {
                                drawTexture = shortestPathOpposite;
                                drawRotation = (float)Math.PI / 2;
                            }

                            if (pathCell.next == pathCell.up)
                            {
                                drawTexture = shortestPathCorner;
                                drawRotation = 0;
                            }
                        }

                        if (pathCell.previous == pathCell.down)
                        {
                            if (pathCell.next == pathCell.left)
                            {
                                drawTexture = shortestPathCorner;
                                drawRotation = (float)Math.PI;
                            }
                            if (pathCell.next == pathCell.up)
                            {
                                drawTexture = shortestPathOpposite;
                                drawRotation = 0;
                            }
                            if (pathCell.next == pathCell.right)
                            {
                                drawTexture = shortestPathCorner;
                                drawRotation = (float)Math.PI / 2;
                            }
                        }
                        if (pathCell.previous == pathCell.left)
                        {
                            if (pathCell.next == pathCell.up)
                            {
                                drawTexture = shortestPathCorner;
                                drawRotation = (float)Math.PI / 2 * -1;
                            }
                            if (pathCell.next == pathCell.right)
                            {
                                drawTexture = shortestPathOpposite;
                                drawRotation = (float)Math.PI / 2;
                            }
                            if (pathCell.next == pathCell.down)
                            {
                                drawTexture = shortestPathCorner;
                                drawRotation = (float)Math.PI;
                            }
                        }
                    }

                    cellRectangle = SetCellRectangle(mazeRectangle, pathCell, maze.Size);
                    cellRectangle.X += (mazeRectangle.Width / maze.Size) / 2;
                    cellRectangle.Y += (mazeRectangle.Height / maze.Size) / 2;
                    drawOrigin = new Vector2(drawTexture.Width / 2, drawTexture.Height / 2);

                    spriteBatch.Draw(drawTexture, cellRectangle, null, Color.White, drawRotation, drawOrigin, SpriteEffects.None, 0);
                }
            }

            return;
        }

    }
}
