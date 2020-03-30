using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace MonoGameWindowsStarter
{
    public class Player
    {
        Game1 game;
        public BoundingRectangle Bounds;
        Texture2D texturePlayer;
        Texture2D textureBullet;
        public List<BulletLeft> bulletLefts;
        public List<BulletRight> bulletRights;
        List<SoundEffect> sfx;
        ParticleSystem particleSystem;
        Texture2D particleTexture;
        Random random;

        // The higher this variable, the slower the ship fires
        UInt16 bulletDelay;

        UInt16 bulletDelayCounter;

        public Player( Game1 game, List<SoundEffect> sfx )
        {
            this.game = game;
            this.sfx = sfx;
            bulletLefts = new List<BulletLeft>();
            bulletRights = new List<BulletRight>();
            bulletDelay = 3;
            bulletDelayCounter = 0;
            random = new Random();
        }

        public void LoadContent( ContentManager content )
        {
            texturePlayer = content.Load<Texture2D>("Player");
            textureBullet = content.Load<Texture2D>("Bullet");
            Bounds.Width = 96;
            Bounds.Height = 98;
            Bounds.X = game.GraphicsDevice.Viewport.Width / 2 - Bounds.Width / 2;
            Bounds.Y = game.GraphicsDevice.Viewport.Height - Bounds.Height;

            // Firing particle system
            particleTexture = content.Load<Texture2D>("particle");
            particleSystem = new ParticleSystem(game.GraphicsDevice, 1000, particleTexture);
            particleSystem.Emitter = new Vector2(100, 100);
            particleSystem.SpawnPerFrame = 4;
            particleSystem.boolSpawn = false;

            // Set the SpawnParticle method
            particleSystem.SpawnParticle = (ref Particle particle) =>
            {
                if (particleSystem.boolSpawn)
                {
                    particle.Position = new Vector2(Bounds.X + (Bounds.Width / 2) - 10, Bounds.Y);
                    particle.Velocity = new Vector2(
                        MathHelper.Lerp(-50, 50, (float)random.NextDouble()), // X between -50 and 50
                        MathHelper.Lerp(-100, -200, (float)random.NextDouble()) // Y between 0 and 100
                        );
                    particle.Acceleration = -0.1f * new Vector2(0, (float)-random.NextDouble());
                    particle.Color = Color.Yellow;
                    particle.Scale = 1f;
                    particle.Life = 0.2f;
                }
            };

            // Set the UpdateParticle method
            particleSystem.UpdateParticle = (float deltaT, ref Particle particle) =>
            {
                particle.Velocity += deltaT * particle.Acceleration;
                particle.Position += deltaT * particle.Velocity;
                particle.Scale -= deltaT;
                particle.Life -= deltaT;
            };
        }

        public void Update( GameTime gameTime )
        {
            if (!game.Gameover)
            {
                /*-------------------------
                 * Player ship movement
                -------------------------*/
                var keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    Bounds.X -= (float)gameTime.ElapsedGameTime.TotalMilliseconds - 5;
                }

                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    Bounds.X += (float)gameTime.ElapsedGameTime.TotalMilliseconds - 5;
                }

                /*-------------------------
                 * Player ship main firing
                -------------------------*/
                if (keyboardState.IsKeyDown(Keys.Z) && bulletDelayCounter == 0)
                {
                    BulletLeft bulletl = new BulletLeft(game, textureBullet, this);
                    BulletRight bulletr = new BulletRight(game, textureBullet, this);
                    bulletLefts.Add(bulletl);
                    bulletRights.Add(bulletr);
                    bulletDelayCounter = bulletDelay;
                    sfx[ 0 ].Play();
                    particleSystem.boolSpawn = true;
                }
                else if (bulletDelayCounter > 0)
                {
                    // Ensures shots aren't produced every frame while key is held.
                    bulletDelayCounter--;
                }

                /*-------------------------
                 * Keep ship on screen
                -------------------------*/
                if (Bounds.X < 0)
                {
                    Bounds.X = 0;
                }
                if (Bounds.X > game.GraphicsDevice.Viewport.Width - Bounds.Width)
                {
                    Bounds.X = game.GraphicsDevice.Viewport.Width - Bounds.Width;
                }

                /*-------------------------
                 * Update bullets, removing them if necessary
                -------------------------*/
                for (int i = 0; i < bulletLefts.Count; i++)
                {
                    bulletLefts[i].Update(gameTime);

                    if (bulletLefts[i].Bounds.Y <= -87)
                    {
                        bulletLefts.RemoveAt(i);
                    }
                }
                for (int i = 0; i < bulletRights.Count; i++)
                {
                    bulletRights[i].Update(gameTime);

                    if (bulletRights[i].Bounds.Y <= -87)
                    {
                        bulletRights.RemoveAt(i);
                    }
                }
            }

            particleSystem.Update( gameTime );
            particleSystem.boolSpawn = false;
        }

        public void Draw( SpriteBatch spriteBatch )
        {
            /*-------------------------
             * Draw player ship
            -------------------------*/
            spriteBatch.Draw( texturePlayer, Bounds, Color.White );

            /*-------------------------
             * Draw left and right bullets
            -------------------------*/
            for ( int i = 0; i < Math.Min( bulletLefts.Count, bulletRights.Count ); i++ )
            {
                bulletLefts[ i ].Draw( spriteBatch );
                bulletRights[ i ].Draw( spriteBatch );
            }

            particleSystem.Draw();
        }
    }
}
