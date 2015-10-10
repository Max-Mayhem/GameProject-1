using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    class PhysicsWorld : GameComponent
    {
        public List<PhysicsObject> physobjs;
        public List<int[]> collisionEvents;             //Use to check of collision between specific physobjs
        public List<PhysicsObject[]> registeredCollisionEvents;   //The collisions in collisionEvents that have actually occured
        public float simtime, simspeed;
        public float groundHeight;

        public PhysicsWorld(Game game)
            : base(game)
        {
            simtime = 0;
            simspeed = 1f;
            physobjs = new List<PhysicsObject>();
            collisionEvents = new List<int[]>();
            groundHeight = -2;
        }

        public void addPhysicsObject(PhysicsObject pobj) 
        {
            physobjs.Add(pobj);
        }

        public void addCollisionEvent(PhysicsObject p1, PhysicsObject p2)
        {
            int[] pindex = new int[2] {physobjs.IndexOf(p1), physobjs.IndexOf(p2)};           
            collisionEvents.Add(pindex);    //Add list of physics objects to collision events
        }

        public override void Update(GameTime gameTime)
        {
            registeredCollisionEvents = new List<PhysicsObject[]>();  //Reset registered collisions

            simtime += simspeed * (float) gameTime.ElapsedGameTime.TotalSeconds;

            //Simulate motion of all PhysicsObjects
            foreach (PhysicsObject pobj in physobjs)
            {
                pobj.Update(simspeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                
                //Check ground penetration
                if (pobj.position.Y - pobj.bsphere.Radius < groundHeight)
                {
                    pobj.velocity.Y *= -0.8f;
                    pobj.position.Y = groundHeight + pobj.bsphere.Radius;
                }
            }

            //Check collisions            
            for (int i = 0; i < physobjs.Count; i++)
            {
                for (int j = i + 1; j < physobjs.Count; j++)
                {                        
                    if (checkCollision(physobjs[i], physobjs[j]))
                    {
                        if (isCollisionEvent(i, j)) //Check if we want special behaviour from objects i & j
                            registeredCollisionEvents.Add(new PhysicsObject[2] { physobjs[i], physobjs[j] });
                        else //If not, resolve collision as normal
                            resolveCollision(physobjs[i], physobjs[j]);
                            continue;
                    }                        
                }
            }
            

            base.Update(gameTime);
        }

        //Return true if these two physics objects belong to a collisionEvent
        private bool isCollisionEvent(int i, int j)
        {
            foreach (int[] pindex in collisionEvents)
            {             
                if (pindex.Contains<int>(i) && pindex.Contains<int>(j)) //Collision event found
                {                         
                    return true;
                }
            }
            //No collision events found
            return false;
        }

        private bool checkCollision(PhysicsObject p1, PhysicsObject p2)
        {
            if (p1.bsphere.Intersects(p2.bsphere))
                return true;                
            
            return false;
        }

        //There is an issue if p1 is moving perpendicularly to p.
        private void resolveCollision(PhysicsObject p1, PhysicsObject p2)
        {
            Vector3 p = p1.position - p2.position;
            
            float r1 = p1.bsphere.Radius;
            float r2 = p2.bsphere.Radius;
            float R = p.Length();

            p.Normalize();            

            float vd = -Vector3.Dot(p, p1.velocity) + Vector3.Dot(p, p2.velocity);
            float pd = 0;   //Distance for each object to be moved

            //To stop it from recolliding each frame, give objects a distance to move = penetration distance. 
            //Value is increased slightly to stop objects getting stuck to each other
            if (p1.isStatic || p2.isStatic)         //Move the non-static object the full distance
                pd = 1.00f * Math.Abs(r1 + r2 - R);
            else
                pd = 0.50f * Math.Abs(r1 + r2 - R); //Move both objects half the distance

            
            p1.velocity += 0.8f * vd * p;
            p2.velocity += 0.8f * vd * -p;

            if (!p1.isStatic)
            {                
                p1.position += p * pd;
                p2.velocity += 0.8f * vd * p;
            }

            if (!p2.isStatic)
            {                
                p2.position += -p * pd;
                p1.velocity += 0.8f * vd * -p;
            }
        }
    }
}
