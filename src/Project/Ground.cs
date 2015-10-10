using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    class Ground : GameComponent
    {
        Matrix translation, rotation, scaling, parent;
        GameObject cube;
        
        public Ground(Game game, Camera cam)
            : base(game)
        {
            cube = new GameObject(game, "cube", game.Content, cam);
            game.Components.Add(cube);
            translation = rotation = scaling = Matrix.Identity;
            cube.translation = Matrix.CreateTranslation(0, -10, -10);
            cube.scale = Matrix.CreateScale(20);
        }

        public override void Initialize()
        {
            
            base.Initialize();
        }

        public void setParent(Matrix p) {
            parent = p;
        }

        public override void Update(GameTime gameTime)
        {            
            cube.parent = rotation * scaling * translation * parent;
            base.Update(gameTime);
        }
    }
}
