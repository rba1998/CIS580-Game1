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

        // The higher this variable, the slower the ship fires
        UInt16 bulletDelay;

        UInt16 bulletDelayCounter;

        List<SoundEffect> sfx;

        public Player( Game1 game, List<SoundEffect> sfx )
        {
            this.game = game;
            this.sfx = sfx;
            bulletLefts = new List<BulletLeft>();
            bulletRights = new List<BulletRight>();
            bulletDelay = 3;
            bulletDelayCounter = 0;
        }

        public void LoadContent( ContentManager content )
        {
            texturePlayer = content.Load<Texture2D>("Player");
            textureBullet = content.Load<Texture2D>("Bullet");
            Bounds.Width = 96;
            Bounds.Height = 98;
            Bounds.X = game.GraphicsDevice.Viewport.Width / 2 - Bounds.Width / 2;
            Bounds.Y = game.GraphicsDevice.Viewport.Height - Bounds.Height;
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
        }
    }
}
