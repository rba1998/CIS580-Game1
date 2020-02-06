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
    class EnemyBasic
    {
        Game1 game;

        public BoundingRectangle Bounds;

        Texture2D texture;

        Random rand;

        public int Health;

        public EnemyBasic( Game1 game )
        {
            this.game = game;
            rand = new Random();
            Health = 1;
        }

        public void LoadContent( ContentManager content )
        {
            texture = content.Load<Texture2D>("pug");
            Bounds.Width = 128;
            Bounds.Height = 89;
            Bounds.X = (float)rand.NextDouble() * game.graphics.PreferredBackBufferWidth - ( Bounds.Width / 2 );
            Bounds.Y = -100;
        }

        public void Update( GameTime gametime )
        {
            Bounds.Y += 5;
        }

        public void Draw( SpriteBatch spriteBatch )
        {
            spriteBatch.Draw( texture, Bounds, Color.White );
        }
    }
}
