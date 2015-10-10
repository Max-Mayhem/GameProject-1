using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Camera variables
        Camera camera;
        Vector3 camPos;
        Vector3 camView;
        Vector3 camUp;

        //All 3D objects go in the world
        GameWorld world;

        //GUI variables
        SpriteFont debugfont;
        
        //Rendering objects
        public BasicEffect effect;       

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            
            graphics.ApplyChanges();

            Mouse.SetPosition(400, 300); //Center mouse
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
            camPos = new Vector3(20, 20, 20);
            camView = -Vector3.UnitZ - Vector3.UnitX - Vector3.UnitY;
            camUp = Vector3.UnitY;
            camera = new Camera(this, camPos, camView, camUp);
            Components.Add(camera);

            world = new GameWorld(this, Content, camera);
            Components.Add(world);

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
            effect = new BasicEffect(GraphicsDevice);
            debugfont = Content.Load<SpriteFont>(@"Fonts/Courier New");

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 5.0f);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            //Need to add the following code before drawing 3D objects if sprite batch is used
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            base.Draw(gameTime);

            //For debugging purposes. At the moment, the world draws over the text...
            spriteBatch.Begin();
            
            spriteBatch.DrawString(debugfont, "Player Score: " + (10 - world.enemy.health).ToString(), new Vector2(0, 20), Color.White);
            spriteBatch.DrawString(debugfont, "Enemy Score : " + (10 - world.player.health).ToString(), new Vector2(0, 40), Color.Red);
            spriteBatch.DrawString(debugfont, "WASD to move, Left click to Attack", new Vector2(400, 20), Color.White);
            spriteBatch.DrawString(debugfont, "Beat up the enemy!", new Vector2(500, 40), Color.Red);


            spriteBatch.End();
        }
    }
}
