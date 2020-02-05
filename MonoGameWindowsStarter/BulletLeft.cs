using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameWindowsStarter
{
    public class BulletLeft
    {
        Game1 game;

        public BoundingRectangle bounds;

        Texture2D texture;

        public BulletLeft( Game1 game, Texture2D texture2d, Player player )
        {
            this.game = game;
            this.texture = texture2d;
            bounds.Width = 16;
            bounds.Height = 87;
            bounds.X = player.bounds.X + 30;
            bounds.Y = player.bounds.Y - 32;
        }

        public void Draw( SpriteBatch spriteBatch )
        {
            spriteBatch.Draw( texture, bounds, Color.White );
        }

        public void Update( GameTime gameTime )
        {
            bounds.Y -= 20;
        }
    }
}
