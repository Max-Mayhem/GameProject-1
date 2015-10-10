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
    class Enemy : GameComponent
    {
        Vector3 position;
        
        Player player;
        Level level;
        
        public float damage, health;

        Vector2 moveVector = Vector2.Zero;
        Matrix parent;
        public GameObject body, fist;
        public List<Vector3> navPath;
        Vector3 goalPosition;

        float fistX, fistY, fistZ;
        Vector3 fistPosition;
        Vector3 prevFistPos;
        const float radius = 1.5f;
        const float attackRange = 1.6f;   //Distance at which enemy can attack
        public bool isAttacking;   //This is for the actual attacking animation
        bool canAttack;     //Conditions for whether the user can attack

        float enemyYaw;
        float angle, speed;
        float fistDefaultAngle;

        Vector3 desired;
        float dist;
        float maxSpeed = 35;
        float acc = 100;

        float attackCooldown; 

        public Enemy(Game game, Player player, Level level, GameObject body, GameObject fist, Vector3 pos)
            : base(game)
        {
            this.body = body;
            this.fist = fist;

            this.player = player;
            this.level = level;

            damage = 2;
            health = 10;

            fistX = 0;
            fistY = 0;
            fistZ = radius;
            
            //Attack parameters
            
            attackCooldown = 1f;  //Time till enemy can attack again

            isAttacking = false;
            canAttack = true;
            enemyYaw = 0;
            fistDefaultAngle = 0.5f;
            angle = fistDefaultAngle;
            speed = (2 * MathHelper.Pi) * 2;

            //Set body GameObject parameters
            setPosition(pos);
            body.setPosition(position);            
            body.physobj.coltype = PhysicsObject.ColliderType.enemy;            

            //Set fist GameObject parameters
            fist.setScale(0.5f);
            fistPosition = new Vector3(fistX + body.physobj.position.X, fistY + body.physobj.position.Y, fistZ + body.physobj.position.Z);
            fist.setPosition(fistPosition);
            fist.physobj.coltype = PhysicsObject.ColliderType.enemy;
            prevFistPos = fistPosition;
            fist.physobj.isStatic = true;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            attackCooldown--;

            position = body.position;

            Vector3 newForward = Vector3.Normalize(body.position - player.position);

            enemyYaw = (float)Math.Atan2(newForward.X, newForward.Z) - MathHelper.Pi;
            body.basicmodel.rotation = Matrix.CreateRotationY(enemyYaw);

            float range = checkRange();
                        
            navPath = level.findPathBetween(position, player.position);
            
            if (range > 1.6f)
            {
                //navPath should always atleast have the enemy position and player position
                if (navPath.Count > 2)
                {
                    goalPosition = navPath[1];                    
                }
                else
                {
                    goalPosition = player.position;
                }
                steering(arrive(), dt);
            }
            else
            {
                body.physobj.acceleration = Vector3.Zero;
                if (canAttack && attackCooldown < 1)
                {
                    isAttacking = true;
                    attackCooldown = 70;
                }
            }

            if (isAttacking)
            {
                attack(dt);
            }

            if (isAttacking == false)
            {
                fistDefaultAngle = enemyYaw;
                angle = fistDefaultAngle;
            }

            fistX = -(float)Math.Cos(angle) * radius;
            fistZ = (float)Math.Sin(angle) * radius;
            fistPosition = new Vector3(fistX + body.position.X, fistY + body.position.Y, fistZ + body.position.Z);
            fist.setPosition(fistPosition);
            //prevFistPos = fistPosition;   //Remove if not required

            base.Update(gameTime);
        }

        public void setPosition(Vector3 pos)
        {
            position = pos;
            body.setPosition(pos);
        }

        //Arrive at the list of paths produced from the A* algorithm        
        public void navigate()
        {

        }
        
        //Function to animate fist around body. This is done by giving the fist tangential velocity around the body.
        //Cool down checks are done outside this function.
        public void attack(float dt)
        {
            if (angle < 2 * Math.PI + fistDefaultAngle)    //Proceed with animation
            {
                angle += 2 * speed * dt;
            }
            else    //Animation complete
            {
                angle = fistDefaultAngle;
                isAttacking = false;
            }
        }

        //apply the steering force to the acceleration
        public void steering(Vector3 force, float time)
        {
            force.Normalize();
            body.physobj.acceleration = force*acc;
        }

        //Like seek but slows down when approaching player        
        public Vector3 arrive()
        {
            float slowing = 1.0f;
            Vector3 playerOffset = goalPosition - body.position;
            dist = playerOffset.Length();
            float ramped = maxSpeed * (dist / slowing);
            float clipped = Math.Min(maxSpeed, ramped);
            desired = (clipped / dist) * playerOffset;
            return (desired - body.physobj.velocity);
        }

        //steer enemy towards player 
        //Function returns the acceleration force to be applied to the object
        public Vector3 seek(Vector3 target)
        {
            desired = target - body.position;
            desired.Normalize();
            desired *= maxSpeed;
            return (desired - body.physobj.velocity);
        }

        //predicts player's future position and pass it to seek
        public Vector3 pursue()
        {
            dist = (player.body.position - body.position).Length();
            float timeToReach = (dist / body.physobj.velocity.Length());
            Vector3 target = player.body.position + (player.body.physobj.velocity * timeToReach);
            return seek(target);
        }

        //checks distance between player and enemy
        public float checkRange()
        {
            return (float)(Math.Sqrt(Vector3.Distance(player.body.position, body.position)));
        }
    }
}

