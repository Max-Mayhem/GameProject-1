using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project
{
    /// <summary>
    /// PhysicsObject holds all the data for kinematics and collisions
    /// 
    /// </summary>

    /* NOTES:
     * Terminal velocity = applied acceleration/resistance
     * Sharpness of velocity curve is proportional to resistance  (how quickly is terminal velocity attained)
     */


    class PhysicsObject
    {
        public Vector3 position, velocity, acceleration, resistance;
        public float resistFactor;
        public Vector3 termVelocity;
        public bool isStatic;   //To stop motion intermittently
        public Vector3 gravity = new Vector3(0, -30, 0);
        public float groundHeight = -2;
        public BoundingSphere bsphere;
        public enum ColliderType { obstacle, player, enemy, noclip };
        public ColliderType coltype = ColliderType.obstacle;    //Default collider type

        public float mass;

        public PhysicsObject(Vector3 pos, BoundingSphere bs)
        {
            position = pos;
            bsphere = bs;

            velocity = acceleration = resistance = Vector3.Zero;
            termVelocity = new Vector3(999, 999, 999);
            resistFactor = 5;

            mass = (float) Math.Pow(bsphere.Radius,3);
        }

        public void setPosition(Vector3 pos)
        {
            position = pos;
            bsphere.Center = pos;
        }
        
        public void setScale(float s)
        {
            bsphere.Radius *= s;
        }

        public void Update(float dt)
        {
            bsphere.Center = position;
            
            if (isStatic) return;
                                        
            resistance = -resistFactor * velocity;

            velocity += (acceleration + resistance + gravity) * dt;

            //Clamp velocity to terminal velocity
            velocity.X = MathHelper.Clamp(velocity.X, -termVelocity.X, termVelocity.X);
            velocity.Y = MathHelper.Clamp(velocity.Y, -termVelocity.Y, termVelocity.Y);
            velocity.Z = MathHelper.Clamp(velocity.Z, -termVelocity.Z, termVelocity.Z);

            position += velocity * dt;
    
            acceleration = Vector3.Zero;            
        }

        public void applyForce(Vector3 force)
        {
            acceleration += force / mass;
        }
    }
}
