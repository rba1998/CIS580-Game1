using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
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

        KeyboardState oldKeyboardState;
        KeyboardState newKeyboardState;

        UInt32 enemySpawnDelay;
        UInt32 enemySpawnCounter;

        List<EnemyBasic> enemyBasics;

        private SpriteFont font;

        public bool Gameover;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            player = new Player( this );
            enemySpawnDelay = 100;
            enemySpawnCounter = 0;
            enemyBasics = new List<EnemyBasic>();
            score = 0;
            Gameover = false;
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

            font = Content.Load<SpriteFont>( "FontArial" );

            player.LoadContent( Content );
            // TODO: use this.Content to load your game content here
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
                    enemyBasics.RemoveAt( i );
                    score++;
                }
            }

            oldKeyboardState = newKeyboardState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            foreach (EnemyBasic eb in enemyBasics)
            {
                eb.Draw(spriteBatch);
            }

            if (!Gameover) // If the game is still in play
            {
                player.Draw(spriteBatch);
                spriteBatch.DrawString(font, "Score: " + score, new Vector2(10, 10), Color.Black);
            }
            else // If the game has ended
            {
                spriteBatch.DrawString( font, "Game over! Final Score: " + score, new Vector2( 100, 100 ), Color.Black );
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
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
