using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameWindowsStarter
{
    class Explosion
    {
        Game1 game;

        public BoundingRectangle Bounds;

        private AnimationManager _am;

        private Dictionary<string, Animation> _a;

        public int timer;

        public Explosion( Game1 game, Dictionary<string, Animation> animations, float x, float y )
        {
            this.game = game;
            _a = animations;
            _am = new AnimationManager(_a.First().Value);
            timer = 100;
            Bounds.X = x;
            Bounds.Y = y;
            Bounds.Height = _a[ "Explosion" ].FrameHeight;
            Bounds.Width = _a[ "Explosion" ].FrameWidth;
        }

        public void Update( GameTime gameTime )
        {
            _am.Play( _a[ "Explosion" ] );
            _am.Update( gameTime );
        }

        public void Draw( SpriteBatch spriteBatch )
        {
            _am.Draw( spriteBatch );
        }
    }
}
