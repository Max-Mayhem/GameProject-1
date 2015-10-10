using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    class ModelManager : DrawableGameComponent
    {
        public List<BasicModel> models;        
        Camera camera;

        public ModelManager(Game game, Camera cam)
            : base(game)
        {
            camera = cam;
        }

        public override void Initialize()
        {
            models = new List<BasicModel>();
            base.Initialize();
        }

        public void addBasicModel(BasicModel mdl) 
        {
            models.Add(mdl);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (BasicModel mdl in models)
            {
                mdl.Draw(Game.GraphicsDevice, camera);
            }
            base.Draw(gameTime);
        }
    }
}
