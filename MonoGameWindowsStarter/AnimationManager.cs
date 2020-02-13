using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameWindowsStarter
{
    class AnimationManager
    {
        private Animation _animation;

        private int _timer;

        public Vector2 Position { get; set; }

        public AnimationManager( Animation ani )
        {
            _animation = ani;
        }

        public void Play( Animation ani )
        {
            if ( _animation == ani )
            {
                return;
            }

            _animation = ani;
            _animation.CurrentFrame = 0;
            _timer = 0;
        }

        public void Stop()
        {
            _timer = 0;

            _animation.CurrentFrame = 0;
        }

        public void Update( GameTime gameTime )
        {
            _timer++;

            if( _timer > 100 )
            {
                _timer = 0;
                _animation.CurrentFrame++;
                if ( _animation.CurrentFrame >= _animation.FrameCount )
                {
                    _animation.CurrentFrame = 0;
                }
            }
        }

        public void Draw( SpriteBatch spriteBatch )
        {
            spriteBatch.Draw( _animation.Texture,
                              Position,
                              new Rectangle( _animation.CurrentFrame * _animation.FrameWidth, 0, _animation.FrameWidth, _animation.FrameHeight ),
                              Color.White );
        }
    }
}
