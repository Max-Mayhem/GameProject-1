using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    /*
     * World will instantiate all the objects
     */ 
    class World : GameComponent
    {
        public Matrix world;
        
        public Ground groundPlane;
        public Player playerModel;        

        public World(Game game) : base(game)
        {
            groundPlane = new Ground(game);
            playerModel = new Player(game);
            
            world = Matrix.Identity;

            game.Components.Add(groundPlane);
            game.Components.Add(playerModel);
        }

        public override void Initialize()
        {            
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.R))
                world *= Matrix.CreateRotationY(0.05f);

            groundPlane.setParent(world);
            playerModel.setParent(world);

            base.Update(gameTime);
        }
    }


}
