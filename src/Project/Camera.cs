using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Project
{
    class Camera : GameComponent
    {
        public Matrix view { protected set; get; }
        public Matrix projection { protected set; get; }

        public Vector3 position;
        public Vector3 direction, up, right;    //To be used for camera rotation and movement each frame
        public Vector3 worldUp;                 //This will control the roll of the camera (Vector3.UnitY)

        public Vector3 velocity;
        public PhysicsObject physobj;

        Point mpos;
        float ar;

        public Camera(Game game, Vector3 pos, Vector3 dir, Vector3 up)
            : base(game)
        {
            position = pos;
            worldUp = up;
            worldUp.Normalize();
            direction = dir;
            dir.Normalize();

            ar = (float)game.Window.ClientBounds.Width / game.Window.ClientBounds.Height;
            view = Matrix.CreateLookAt(position, direction, worldUp);
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                ar,
                1.0f,
                1000.0f);
        }

        public override void Initialize()
        {
            mpos = Mouse.GetState().Position;
            base.Initialize();
        }

        void move()
        {            
            physobj.acceleration = velocity.X * right + velocity.Z * direction;
        }

        public override void Update(GameTime gameTime)
        {
            //position = physobj.position;
            
            /*
            int keyup = 0, keydown = 0, keyleft = 0, keyright = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                keyup = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                keydown = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                keyleft = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                keyright = 1;

            
            velocity.X = (-keyleft + keyright)*15;
            velocity.Z = (keyup - keydown)*15;

            move();

            float dx = 0.002f * (Mouse.GetState().Position.X - mpos.X);
            float dy = 0.002f * (Mouse.GetState().Position.Y - mpos.Y);

            //Prevent gimbal lock
            if (direction.Y > 0.95 || direction.Y < -0.95)
                direction.Y *= 0.99f;   //Bring very slightly closer to the horizon

            //Console.WriteLine(dx.ToString() + " " + dy.ToString());                                  

            right = Vector3.Cross(direction, worldUp);
            up = Vector3.Cross(direction, right);
            direction += dx * right + dy * up;
            direction.Normalize();

            mpos = Mouse.GetState().Position;
            */

            view = Matrix.CreateLookAt(position, position + direction, worldUp);

            base.Update(gameTime);
        }
    }
}
