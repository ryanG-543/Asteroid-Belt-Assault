using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroid_Belt_Assault
{
    class PlayerManager
    {
        public Sprite playerSprite;
        private float playerSpeed = 160.0f;
        private Rectangle playerAreaLimit;

        public int PlayerScore = 0;
        public int LivesRemaining = 3;
        public int healthRemaining = 100;
        public bool Destroyed = false;
        public int CurrentLevel = 1;

        private Vector2 gunOffset = new Vector2(25, 10);
        private float shotTimer = 0.0f;
        public float minShotTimer = 0.2f;
        private int playerRadius = 15;
        public ShotManager PlayerShotManager;       

        public PlayerManager(
            Texture2D texture,  
            Rectangle initialFrame,
            int frameCount,
            Rectangle screenBounds)
        {
            playerSprite = new Sprite(
                new Vector2(500, 500),
                texture,
                initialFrame,
                Vector2.Zero);

            PlayerShotManager = new ShotManager(
                texture,
                new Rectangle(0, 300, 5, 5),
                4,
                2,
                250f,
                screenBounds);

            playerAreaLimit =
                new Rectangle(
                    0,
                    screenBounds.Height / 2,
                    screenBounds.Width,
                    screenBounds.Height / 2);

            for (int x = 1; x < frameCount; x++)
            {
                playerSprite.AddFrame(
                    new Rectangle(
                        initialFrame.X + (initialFrame.Width * x),
                        initialFrame.Y,
                        initialFrame.Width,
                        initialFrame.Height));
            }
            playerSprite.CollisionRadius = playerRadius;
        }

        private void FireShot()
        {
            if (shotTimer >= minShotTimer)
            {
                if (CurrentLevel == 1)
                {
                    PlayerShotManager.FireShot(
                        playerSprite.Location + gunOffset,
                        new Vector2(0, -1),
                        true);
                }
                else if (CurrentLevel == 2)
                {

                    Vector2 direction = new Vector2(5, -100);
                    direction.Normalize();

                    PlayerShotManager.FireShot(
                    playerSprite.Location + gunOffset,
                    direction,
                    true);


                    direction = new Vector2(-5, -100);
                    direction.Normalize();

                    PlayerShotManager.FireShot(
                    playerSprite.Location + gunOffset,
                    direction,
                    true);
                }
                else
                {
                    PlayerShotManager.FireShot(
                    playerSprite.Location + gunOffset,
                    new Vector2(0, -1),
                    true);

                    Random rand = new Random(System.Environment.TickCount);
                    for (int i = 1; i < CurrentLevel-1; i++)
                    {
                        Vector2 direction = new Vector2(-8 * i, -100 + rand.Next(-50,50));
                        direction.Normalize();
                       

                        PlayerShotManager.FireShot(
                        playerSprite.Location + gunOffset,
                        direction,
                        true);

                        direction = new Vector2(8 * i, -100 + rand.Next(-50, 50));
                        direction.Normalize();

                        PlayerShotManager.FireShot(
                        playerSprite.Location + gunOffset,
                        direction,
                        true);
                    }

                }

                shotTimer = 0.0f;
            }
        }

        private void HandleKeyboardInput(KeyboardState keyState)
        {
        // WASD Controls
            if (keyState.IsKeyDown(Keys.W))
            {
                playerSprite.Velocity += new Vector2(0, -1);
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                playerSprite.Velocity += new Vector2(0, 1);
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                playerSprite.Velocity += new Vector2(-1, 0);
            }

            if (keyState.IsKeyDown(Keys.D))
            {
                playerSprite.Velocity += new Vector2(1, 0);
            }
            if (keyState.IsKeyDown(Keys.J))
            {
                FireShot();
            }

            //DEBUG BUTTON
            if (keyState.IsKeyDown(Keys.LeftControl))
            {
                PlayerScore =+ 500;
            }
//          if (Mouse.LeftButton == MouseButtonState.Pressed);
//          {
//              FireShot();
//          }
        }

        private void HandleGamepadInput(GamePadState gamePadState)
        {
            playerSprite.Velocity +=
                new Vector2(
                    gamePadState.ThumbSticks.Left.X,
                    -gamePadState.ThumbSticks.Left.Y);

            if (gamePadState.Buttons.A == ButtonState.Pressed)
            {
                FireShot();
            }
        }

        private void imposeMovementLimits()
        {
            Vector2 location = playerSprite.Location;

            if (location.X < playerAreaLimit.X)
                location.X = playerAreaLimit.X;

            if (location.X >
                (playerAreaLimit.Right - playerSprite.Source.Width))
                location.X =
                    (playerAreaLimit.Right - playerSprite.Source.Width);

            if (location.Y < playerAreaLimit.Y)
                location.Y = playerAreaLimit.Y;

            if (location.Y >
                (playerAreaLimit.Bottom - playerSprite.Source.Height))
                location.Y =
                    (playerAreaLimit.Bottom - playerSprite.Source.Height);

            playerSprite.Location = location;
        }

        public void Update(GameTime gameTime)
        {
            PlayerShotManager.Update(gameTime);

            if (!Destroyed)
            {
                playerSprite.Velocity = Vector2.Zero;

                shotTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                HandleKeyboardInput(Keyboard.GetState());
                HandleGamepadInput(GamePad.GetState(PlayerIndex.One));

                playerSprite.Velocity.Normalize();
                playerSprite.Velocity *= playerSpeed;

                playerSprite.Update(gameTime);
                imposeMovementLimits();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerShotManager.Draw(spriteBatch);

            if (!Destroyed)
            {
                playerSprite.Draw(spriteBatch);
            }
        }

    }
}
