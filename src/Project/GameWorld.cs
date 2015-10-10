using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Project
{
    /// <summary>
    /// GameWorld is the base class that contains all game objects and simulation objects. 
    /// </summary>
    class GameWorld : GameComponent
    {
        //Useful objects from Game
        ContentManager content;
        Camera camera;
        GameComponentCollection components;

        //Objects in GameWorld
        public List<GameObject> gameobjs;
        PhysicsWorld physworld;
        ModelManager modelmgr;
        float groundHeight = -2;
        public Player player;
        public Enemy enemy;

        //Level info - Move to another class (maybe merge Navigation and Level)        
        Level level;

        SoundEffect sfx;
        SoundEffectInstance bgm, punch;

        public GameWorld(Game game, ContentManager cm, Camera cam)
            : base(game)
        {
            content = cm;
            camera = cam;
            components = game.Components;
            physworld = new PhysicsWorld(game);
            modelmgr = new ModelManager(game, cam);
            level = new Level();

            components.Add(physworld);
            components.Add(modelmgr);

            sfx = game.Content.Load<SoundEffect>(@"Audio/punch");
            punch = sfx.CreateInstance();
            punch.IsLooped = false;
            sfx = game.Content.Load<SoundEffect>(@"Audio/bgm");
            bgm = sfx.CreateInstance();
            bgm.IsLooped = true;
            bgm.Play();
        }

        //Initialise the objects here
        public override void Initialize()
        {
            Game game = ((Game1)Game);

            camera.physobj = createPhysicsObject(camera.position, new BoundingSphere(camera.position, 2));
            camera.physobj.gravity = Vector3.Zero;
            camera.physobj.isStatic = true;

            gameobjs = new List<GameObject>();

            loadLevel(game);

            base.Initialize();
        }

        //Initialise game objects and create the level here. Need to more camera create to GameWorld and this function.
        public void loadLevel(Game game)
        {
            //Create static objects            
            for (int i = 0; i < level.size; i++)
            {
                for (int j = 0; j < level.size; j++)
                {
                    Vector3 pos = level.centerOfCell(i, j);
                    pos.Y = groundHeight + 1;
                    switch (level.map[i, j])
                    {
                        //Player
                        case 1:
                            createPlayer(game, pos);
                            break;
                        //Enemy
                        case 2:
                            createEnemy(game, pos);
                            break;
                        //Obstacle
                        case 3:
                            createObstacle(game, pos);
                            break;
                    }
                }
            }

            BasicModel floor = createBasicModel("cube", "floor");
            floor.scale *= Matrix.CreateScale(64, 1, 64);
            floor.translation = Matrix.CreateTranslation(32, -2.5f, 32);

            //Collision events between player and enemy
            physworld.addCollisionEvent(enemy.body.physobj, player.fist.physobj);
            physworld.addCollisionEvent(player.body.physobj, enemy.fist.physobj);
        }

        public void createPlayer(Game game, Vector3 pos)
        {
            GameObject body = createGameObject(game, Vector3.Zero, "sphere", "player");   //Player body            
            GameObject fist = createGameObject(game, Vector3.Zero, "sphere", "tex_fist");   //Player fist
            gameobjs.Add(body);
            gameobjs.Add(fist);

            //Add the body and fist of player to gameobjs
            player = new Player(game, body, fist, pos, camera);
            player.body.setPosition(pos);
            //Add to Game.Components
            components.Add(player);

            physworld.addCollisionEvent(player.body.physobj, player.fist.physobj);
        }

        public void createEnemy(Game game, Vector3 pos)
        {
            GameObject body = createGameObject(game, Vector3.Zero, "sphere", "enemy");   //Enemy body            
            GameObject fist = createGameObject(game, Vector3.Zero, "sphere", "enemy_fist");   //Enemy fist
            gameobjs.Add(body);
            gameobjs.Add(fist);

            //Add the body and fist of enemy to gameobjs
            enemy = new Enemy(game, player, level, body, fist, pos);
            enemy.body.setPosition(pos);
            components.Add(enemy);

            physworld.addCollisionEvent(enemy.body.physobj, enemy.fist.physobj);
        }

        public void createObstacle(Game game, Vector3 pos)
        {
            GameObject obstacle1 = createGameObject(game, pos, "sphere", "obstacle");
            obstacle1.physobj.isStatic = true;

            GameObject obstacle2 = createGameObject(game, pos + 2 * Vector3.UnitY, "sphere", "obstacle");
            obstacle2.physobj.isStatic = true;

            gameobjs.Add(obstacle1);
            gameobjs.Add(obstacle2);
        }

        /// <summary>
        /// This will create a BasicModel and a PhysicsObject for each in game object.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="pos"></param>
        /// <param name="mdlname"></param>
        /// <param name="texname"></param>
        /// <returns></returns>
        public GameObject createGameObject(Game game, Vector3 pos, string mdlname, string texname)
        {
            BasicModel bmodel = createBasicModel(mdlname, texname);
            PhysicsObject physobj = createPhysicsObject(pos, bmodel.model.Meshes[0].BoundingSphere);
            GameObject gobj = new GameObject(game, pos, bmodel, physobj);
            components.Add(gobj);

            return gobj;
        }

        /// <summary>
        /// Load the model and texture asset. Create a BasicModel using those assets and add
        /// it to ModelManager. The function also returns the BasicModel created.
        /// </summary>
        /// <param name="mdlname"></param>
        /// <param name="texname"></param>
        /// <returns></returns>
        public BasicModel createBasicModel(string mdlname, string texname)
        {
            Model mdl = content.Load<Model>(@"Models/" + mdlname);
            Texture2D tex = content.Load<Texture2D>(@"Textures/" + texname);

            BasicModel bmodel = new BasicModel(mdl, tex);
            modelmgr.addBasicModel(bmodel);

            return bmodel;
        }

        /// <summary>
        /// Create a PhysicsObject based on position and bounding sphere. This will exist in the GameWorld and does not need a BasicModel to be simulated.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="bsphere"></param>
        /// <returns></returns>
        public PhysicsObject createPhysicsObject(Vector3 pos, BoundingSphere bsphere)
        {
            PhysicsObject pobj = new PhysicsObject(pos, bsphere);
            physworld.addPhysicsObject(pobj);
            return pobj;
        }

        public override void Update(GameTime gameTime)
        {
            //Game logic here
            //Give AI player position and path

            /*Test for special collisions
             * Go through physworld.registeredCollisionEvents and handle collisions depending on the PhysicsObject.ColliderType   
             * At the moment, if player hits enemy (or vice-versa). The player that is hit is thrown back, and takes damage.
             */

            foreach (PhysicsObject[] pindex in physworld.registeredCollisionEvents)
            {
                switch (collisionEventType(pindex))
                {
                    case 0:     //Player fist hit enemy
                        if (player.isAttacking)
                        {
                            Vector3 dPos = player.body.position - enemy.body.position;
                            dPos.Normalize();
                            enemy.body.physobj.velocity += -dPos * 10f;
                            enemy.health -= player.damage;
                            punch.Play();
                        }
                        break;

                    case 1:     //Enemy fist hit player
                        if (enemy.isAttacking)
                        {
                            Vector3 dPos = enemy.body.position - player.body.position;
                            dPos.Normalize();
                            player.body.physobj.velocity += -dPos * 10f;
                            player.health -= enemy.damage;
                            punch.Play();
                        }
                        break;
                }
            }

            //Check player and enemy health
            //Start new round if either health < 0.
            //Record who won and lost

            //Update GUI for Game1

            base.Update(gameTime);
        }

        //This method of handling the collisionEventType will be changed in the future. A collisionEvent will be an object of its own.
        //The response to a registeredCollisionEvent will be handled manually as above.
        private int collisionEventType(PhysicsObject[] pindex)
        {
            if (pindex.Contains(enemy.body.physobj) && pindex.Contains(player.fist.physobj)) return 0;
            if (pindex.Contains(player.body.physobj) && pindex.Contains(enemy.fist.physobj)) return 1;

            return -1;
        }
    }
}
