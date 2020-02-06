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

        public BoundingRectangle Bounds;

        Texture2D texture;

        public BulletLeft( Game1 game, Texture2D texture2d, Player player )
        {
            this.game = game;
            this.texture = texture2d;
            Bounds.Width = 16;
            Bounds.Height = 87;
            Bounds.X = player.Bounds.X + 30;
            Bounds.Y = player.Bounds.Y - 32;
        }

        public void Draw( SpriteBatch spriteBatch )
        {
            spriteBatch.Draw( texture, Bounds, Color.White );
        }

        public void Update( GameTime gameTime )
        {
            Bounds.Y -= 20;
        }
    }
}
