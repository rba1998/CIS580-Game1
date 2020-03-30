using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using System.Windows;

namespace MonoGameWindowsStarter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        UInt32 score;
        Texture2D textureEnemy;
        Texture2D textureExplosion;
        KeyboardState oldKeyboardState;
        KeyboardState newKeyboardState;
        UInt32 enemySpawnDelay;
        UInt32 enemySpawnCounter;
        List<EnemyBasic> enemyBasics;
        List<Explosion> explosions;
        private SpriteFont font;
        public bool Gameover;
        List<SoundEffect> sfx;
        private Dictionary<string, Animation> animations;
        ParticleSystem particleSystem;
        ParticleSystem particleSystemEnemy;
        Texture2D particleTexture;
        Texture2D particleTextureEnemy;
        Random random;
        float enemySpawnX;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            sfx = new List<SoundEffect>();
            SoundEffect.MasterVolume = 0.1f;
            player = new Player( this, sfx );
            enemySpawnDelay = 100;
            enemySpawnCounter = 0;
            enemyBasics = new List<EnemyBasic>();
            explosions = new List<Explosion>();
            score = 0;
            Gameover = false;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            random = new Random();
            enemySpawnX = 0;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 540;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            textureEnemy = Content.Load<Texture2D>( "pug" );
            textureExplosion = Content.Load<Texture2D>( "explosion" );

            font = Content.Load<SpriteFont>( "FontArial" );

            sfx.Add( Content.Load<SoundEffect>( "shootLaser" ) );
            sfx.Add( Content.Load<SoundEffect>( "shootEnemyLaser" ) );
            sfx.Add( Content.Load<SoundEffect>( "explosionEnemy" ) );

            animations = new Dictionary<string, Animation>()
            {
                { "Explosion", new Animation( textureExplosion, 100 ) }
            };

            player.LoadContent( Content );

            // Particle System: Engine Exhaust
            particleTexture = Content.Load<Texture2D>("particle");
            particleSystem = new ParticleSystem(this.GraphicsDevice, 1000, particleTexture);
            particleSystem.Emitter = new Vector2(100, 100);
            particleSystem.SpawnPerFrame = 4;

            // Set the SpawnParticle method
            particleSystem.SpawnParticle = (ref Particle particle) =>
            {
                particle.Position = new Vector2(player.Bounds.X + (player.Bounds.Width / 2) - 10, player.Bounds.Y + player.Bounds.Height - 20);
                particle.Velocity = new Vector2(
                    MathHelper.Lerp(-50, 50, (float)random.NextDouble()), // X between -50 and 50
                    MathHelper.Lerp(0, 100, (float)random.NextDouble()) // Y between 0 and 100
                    );
                particle.Acceleration = 0.1f * new Vector2(0, (float)-random.NextDouble());
                particle.Color = Color.Red;
                particle.Scale = 1f;
                particle.Life = 1.0f;
            };

            // Set the UpdateParticle method
            particleSystem.UpdateParticle = (float deltaT, ref Particle particle) =>
            {
                particle.Velocity += deltaT * particle.Acceleration;
                particle.Position += deltaT * particle.Velocity;
                particle.Scale -= deltaT;
                particle.Life -= deltaT;
            };

            // Particle System: Enemy Spawn Location
            particleTextureEnemy = Content.Load<Texture2D>( "particle" );
            particleSystemEnemy = new ParticleSystem(this.GraphicsDevice, 1000, particleTextureEnemy);
            particleSystemEnemy.Emitter = new Vector2(100, 100);
            particleSystemEnemy.SpawnPerFrame = 1;

            // Set the SpawnParticle method
            particleSystemEnemy.SpawnParticle = (ref Particle particle) =>
            {
                particle.Position = new Vector2(enemySpawnX, -25);
                particle.Velocity = new Vector2(
                    MathHelper.Lerp(-50, 50, (float)random.NextDouble()), // X between -50 and 50
                    MathHelper.Lerp(0, 200, (float)random.NextDouble()) // Y between 0 and 100
                    );
                particle.Acceleration = 0.3f * new Vector2(0, (float)-random.NextDouble());
                particle.Color = Color.DarkRed;
                particle.Scale = 3.0f;
                particle.Life = 0.5f;
            };

            // Set the UpdateParticle method
            particleSystemEnemy.UpdateParticle = (float deltaT, ref Particle particle) =>
            {
                particle.Velocity += deltaT * particle.Acceleration;
                particle.Position += deltaT * particle.Velocity;
                particle.Scale -= deltaT;
                particle.Life -= deltaT;
            };
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            newKeyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            player.Update( gameTime );

            // Enemy spawning logic
            if ( enemySpawnCounter <= 0 )
            {
                EnemyBasic enemy = new EnemyBasic( this );
                enemy.LoadContent( Content );
                enemyBasics.Add( enemy );
                enemySpawnX = enemy.Bounds.X + (enemy.Bounds.Width / 2);
                enemySpawnCounter = enemySpawnDelay;
                if ( enemySpawnDelay >= 1 )
                {
                    enemySpawnDelay -= 5;
                }
            }
            else if ( enemySpawnCounter > 0 )
            {
                enemySpawnCounter--;
            }
            for ( int i = 0; i < enemyBasics.Count; i++ )
            {
                enemyBasics[ i ].Update( gameTime );
                if ( enemyBasics[ i ].Bounds.Y > graphics.PreferredBackBufferHeight )
                {
                    enemyBasics.RemoveAt( i );
                }
            }

            // Check bullet/enemy and player/enemy collisions
            for ( int i = 0; i < enemyBasics.Count; i++ )
            {
                if (CollidesWith(enemyBasics[i].Bounds, player.Bounds))
                {
                    if ( !Gameover )
                    {
                        sfx[ 1 ].Play();
                    }
                    Gameover = true;
                }
                for ( int j = 0; j < player.bulletLefts.Count; j++ )
                {
                    if ( CollidesWith( enemyBasics[ i ].Bounds, player.bulletLefts[ j ].Bounds ) )
                    {
                        enemyBasics[i].Health--;
                        player.bulletLefts.RemoveAt( j );
                    }
                }
                for (int k = 0; k < player.bulletRights.Count; k++)
                {
                    if (CollidesWith(enemyBasics[i].Bounds, player.bulletRights[k].Bounds))
                    {
                        enemyBasics[i].Health--;
                        player.bulletRights.RemoveAt(k);
                    }
                }
                if( enemyBasics[ i ].Health <= 0 )
                {
                    SoundEffect.MasterVolume = 0.5f;
                    sfx[ 2 ].Play();
                    SoundEffect.MasterVolume = 0.1f;
                    explosions.Add( new Explosion( this, animations, enemyBasics[ i ].Bounds.X, enemyBasics[ i ].Bounds.Y ) );
                    score++;
                    enemyBasics.RemoveAt(i);
                }
            }

            // Explosion timers
            for ( int i = 0; i < explosions.Count; i++ )
            {
                explosions[ i ].timer--;
                if ( explosions[ i ].timer <= 0 )
                {
                    explosions.RemoveAt( i );
                }
            }

            particleSystem.Update(gameTime);
            particleSystemEnemy.Update(gameTime);

            oldKeyboardState = newKeyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            foreach ( EnemyBasic eb in enemyBasics )
            {
                eb.Draw( spriteBatch );
            }

            foreach ( Explosion e in explosions )
            {
                e.Draw( spriteBatch );
            }

            if ( !Gameover ) // If the game is still in play
            {
                player.Draw(spriteBatch);
                spriteBatch.DrawString( font, "Score: " + score, new Vector2(10, 10), Color.Black );
                particleSystem.Draw();
            }
            else // If the game has ended
            {
                spriteBatch.DrawString( font, "Game over! Final Score: " + score, new Vector2( 100, 100 ), Color.Black );
            }
            
            spriteBatch.End();

            particleSystemEnemy.Draw();

            base.Draw( gameTime );
        }

        public static bool CollidesWith( BoundingRectangle a, BoundingRectangle b )
        {
            return ( ( a.X + a.Width >= b.X ) &&
                     ( b.X + b.Width >= a.X ) &&
                     ( a.Y + a.Height >= b.Y ) &&
                     ( b.Y + b.Height >= a.Y ) );
        }
    }
}
