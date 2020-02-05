using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoGameWindowsStarter
{
    public class Player
    {
        Game1 game;

        public BoundingRectangle bounds;

        Texture2D texturePlayer;
        Texture2D textureBullet;

        List<BulletLeft> bulletLefts;
        List<BulletRight> bulletRights;

        // The higher this variable, the slower the ship fires
        UInt16 bulletDelay;

        UInt16 bulletDelayCounter;

        public Player( Game1 game )
        {
            this.game = game;
            bulletLefts = new List<BulletLeft>();
            bulletRights = new List<BulletRight>();
            bulletDelay = 3;
            bulletDelayCounter = 0;
        }

        public void LoadContent( ContentManager content )
        {
            texturePlayer = content.Load<Texture2D>("Player");
            textureBullet = content.Load<Texture2D>("Bullet");
            bounds.Width = 96;
            bounds.Height = 98;
            bounds.X = game.GraphicsDevice.Viewport.Width / 2 - bounds.Width / 2;
            bounds.Y = game.GraphicsDevice.Viewport.Height - bounds.Height;
        }

        public void Update( GameTime gameTime )
        {
            /*-------------------------
             * Player ship movement
            -------------------------*/
            var keyboardState = Keyboard.GetState();
            if ( keyboardState.IsKeyDown( Keys.Left ) )
            {
                bounds.X -= (float)gameTime.ElapsedGameTime.TotalMilliseconds - 5;
            }

            if ( keyboardState.IsKeyDown( Keys.Right ) )
            {
                bounds.X += (float)gameTime.ElapsedGameTime.TotalMilliseconds - 5;
            }

            /*-------------------------
             * Player ship main firing
            -------------------------*/
            if ( keyboardState.IsKeyDown( Keys.Z ) && bulletDelayCounter == 0 )
            {
                BulletLeft bulletl = new BulletLeft( game, textureBullet, this );
                BulletRight bulletr = new BulletRight( game, textureBullet, this );
                bulletLefts.Add( bulletl );
                bulletRights.Add( bulletr );
                bulletDelayCounter = bulletDelay;
            }
            else if ( bulletDelayCounter != 0 )
            {
                // Ensures shots aren't produced every frame while key is held.
                bulletDelayCounter--;
            }

            /*-------------------------
             * Keep ship on screen
            -------------------------*/
            if ( bounds.X < 0 )
            {
                bounds.X = 0;
            }
            if ( bounds.X > game.GraphicsDevice.Viewport.Width - bounds.Width )
            {
                bounds.X = game.GraphicsDevice.Viewport.Width - bounds.Width;
            }

            /*-------------------------
             * Update bullets, removing them if necessary
            -------------------------*/
            for ( int i = 0; i < bulletLefts.Count; i++ )
            {
                bulletLefts[i].Update( gameTime );
                
                if ( bulletLefts[i].bounds.Y <= -87 )
                {
                    bulletLefts.RemoveAt( i );
                }
            }
            for ( int i = 0; i < bulletRights.Count; i++ )
            {
                bulletRights[ i ].Update( gameTime );

                if ( bulletRights[ i ].bounds.Y <= -87 )
                {
                    bulletRights.RemoveAt( i );
                }
            }
        }

        public void Draw( SpriteBatch spriteBatch )
        {
            /*-------------------------
             * Draw player ship
            -------------------------*/
            spriteBatch.Draw( texturePlayer, bounds, Color.White );

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
