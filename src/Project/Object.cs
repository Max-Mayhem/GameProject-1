using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    class Object : DrawableGameComponent
    {
        Matrix translation, rotation, scaling;        
        
        public Object(Game game) : base(game)
        {

        }

        public void setMesh() 
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
 	         base.Draw(gameTime);
        }
    }
}
