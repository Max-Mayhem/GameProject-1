using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Project
{
    class Player : GameComponent
    {
        #region attributes
        public Vector3 position;
        int forward, back, left, right;

        public float damage, health;

        Vector2 moveVector = Vector2.Zero;
        public GameObject body, fist;

        float fistX, fistY, fistZ;
        Vector3 fistPosition;
        Vector3 prevFistPos;
        const float radius = 1.5f;
        public bool isAttacking;   //This is for the actual attacking animation
        bool canAttack;     //Conditions for whether the user can attack

        float angle, speed;
        float fistDefaultAngle;
        float acc = 100;    //Applied acceleration when moving
        #endregion

        #region camera attributes
        Camera camera;
        Vector3 cameraPosition;
        Vector3 positionReference = new Vector3(0, 2, -8);  //Offset of camera with reference to player
        Vector3 headOffset = new Vector3(0, 1, 0);          //Camera target
        Vector3 cameraTarget;

        MouseState prevMouseState;
        float mouseSpeed = 10f / 1000f;
        float cameraAngle;
        float playerYaw;

        float camX, camY, camZ;
        #endregion

        float attackCooldown = 0;

        public Player(Game game, GameObject body, GameObject fist, Vector3 pos, Camera cam)
            : base(game)
        {
            this.body = body;
            this.fist = fist;
            
            damage = 2;
            health = 10;
            forward = back = left = right = 0;

            fistX = 0;
            fistY = 0;
            fistZ = radius;

            camX = 0;
            camY = 0;
            camZ = radius * 10;
            isAttacking = false;
            canAttack = true;
            fistDefaultAngle = 0.5f;
            angle = fistDefaultAngle;
            speed = (2 * MathHelper.Pi) * 2;

            //Set body GameObject parameters
            setPosition(pos);            
            body.physobj.coltype = PhysicsObject.ColliderType.player;
            
            //Set fist GameObject parameters
            fist.setScale(0.5f);
            new Vector3(fistX + body.physobj.position.X, fistY + body.physobj.position.Y, fistZ + body.physobj.position.Z);
            fist.setPosition(fistPosition);            
            fist.physobj.coltype = PhysicsObject.ColliderType.player;
            prevFistPos = fistPosition;
            fist.physobj.isStatic = true;
            
            camera = cam;
            setCamera();
        }

        public void setPosition(Vector3 pos)
        {
            position = pos;
            body.setPosition(pos);
        }

        /// <summary>
        /// Reposition camera based on player rotation
        /// </summary>
        public void setCamera()
        {
            camX = -(float)Math.Cos(cameraAngle - MathHelper.PiOver2) * radius * 10;
            camZ = (float)Math.Sin(cameraAngle - MathHelper.PiOver2) * radius * 10;
            cameraPosition = new Vector3(camX + body.position.X, camY + body.position.Y + 5, camZ + body.position.Z);
            camera.position = cameraPosition;
            camera.direction = cameraTarget - cameraPosition;
        }

        public override void Update(GameTime gameTime)
        {
            int mouseTempX = Mouse.GetState().X;
            int mouseTempY = Mouse.GetState().Y;
            attackCooldown -= 1;
            
            #region camera
            cameraTarget = body.physobj.position + headOffset;                               
                        
            int dx = -(Mouse.GetState().Position.X - prevMouseState.X);
            int dy = Mouse.GetState().Position.Y - prevMouseState.Y;
                        
            playerYaw += dx*mouseSpeed;
            setCamera();
            if(Game.Window != null)
            {
                Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);     //Recenter mouse
            }
            body.basicmodel.rotation = Matrix.CreateRotationY(playerYaw);          

            prevMouseState = Mouse.GetState();
            #endregion

            #region movement
            forward = back = left = right = 0;

            //Player motion
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {//backward = 1;
                body.physobj.acceleration.X = acc * (float)Math.Cos(cameraAngle - MathHelper.PiOver2);
                body.physobj.acceleration.Z = acc * -(float)Math.Sin(cameraAngle - MathHelper.PiOver2);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {//forward = 1;  
                body.physobj.acceleration.X = acc * -(float)Math.Cos(cameraAngle - MathHelper.PiOver2);
                body.physobj.acceleration.Z = acc * (float)Math.Sin(cameraAngle - MathHelper.PiOver2);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {//right = 1;
                body.physobj.acceleration.X = acc * (float)Math.Cos(cameraAngle);
                body.physobj.acceleration.Z = acc * -(float)Math.Sin(cameraAngle);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {//left = 1;
                body.physobj.acceleration.X = acc * -(float)Math.Cos(cameraAngle);
                body.physobj.acceleration.Z = acc * (float)Math.Sin(cameraAngle);
            }

            /*
            if (Keyboard.GetState().IsKeyDown(Keys.P))
                setPosition(Vector3.Zero);
            */
            position = body.position;

            #endregion

            #region attack
            //Only check if user wants to attack if the user CAN attack
            if (canAttack)
            {
                //If so, the attack animation is active
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && attackCooldown < 1)
                {
                    isAttacking = true;
                    canAttack = false;
                    attackCooldown = 50;
                }
            }

            //User can only attack again is the attack animation is over && the attack button is not being pressed
            if (Mouse.GetState().LeftButton != ButtonState.Pressed && !isAttacking) canAttack = true;
            if (isAttacking)
            {
                attack((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            fistX = -(float)Math.Cos(angle) * radius; 
            fistZ = (float)Math.Sin(angle) * radius;
            fistPosition = new Vector3(fistX + body.position.X, fistY + body.position.Y, fistZ + body.position.Z);
            fist.setPosition(fistPosition);
            if (isAttacking == false)
            {
                fistDefaultAngle = playerYaw;
                angle = fistDefaultAngle;
            }

            cameraAngle = playerYaw;
            #endregion

            base.Update(gameTime);
        }

        //Function to animate fist around body. This is done by giving the fist tangential velocity around the body.
        //Cool down checks are done outside this function.
        public void attack(float dt)
        {
            if (angle < 2 * Math.PI + fistDefaultAngle)    //Proceed with animation
            { angle += 2 * speed * dt;}
            else    //Animation complete
            { angle = fistDefaultAngle; isAttacking = false; }
        }
    }
}