using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project
{
    class GameObject : GameComponent
    {
        public BasicModel basicmodel;
        public PhysicsObject physobj;        

        public Vector3 position;
        public Quaternion rotation;
        public float scale;
        public Matrix world;

        public GameObject(Game game, Vector3 pos, BasicModel mdl, PhysicsObject pobj)
            : base(game)
        {
            basicmodel = mdl;
            physobj = pobj;

            //Default values
            
            setPosition(pos);
            rotation = Quaternion.CreateFromYawPitchRoll(0, 0, 0);
            scale = 1f;
            world = Matrix.Identity;
        }

        public void setPosition(Vector3 pos)
        {
            position = pos;
            physobj.setPosition(pos);
        }

        public void setScale(float s)
        {
            scale = s;
            physobj.setScale(s);
        }

        public Matrix getWorldTransform()
        {
            return world;
        }

        public override void Update(GameTime gameTime)
        {            
            position = physobj.position;
            world = Matrix.CreateScale(scale, scale, scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);

            basicmodel.world = getWorldTransform();
            base.Update(gameTime);
        }
    }
}
