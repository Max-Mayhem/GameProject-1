using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    //Cube class contains all the vectors for vertices, texture coordinates and face normals
    class Cube : DrawableGameComponent
    {   
        public Vector3 position;
        public Matrix parent;
        public Color[] colour;
        public VertexPositionColor[] vpc;
        public short[] indices;

        public Cube(Game game, Vector3 pos, float size) : base(game)
        {
            colour = new Color[4] {
                Color.DarkSalmon,
                Color.DarkGoldenrod,
                Color.GhostWhite,
                Color.MediumSeaGreen
            };

            position = pos;
            parent = Matrix.Identity;

            Vector3[] verts;
            Vector3[] normals;            

            //Vertices
            verts = new Vector3[8]
            {
                new Vector3(-size + position.X, size + position.Y, size + position.Z),
                new Vector3(size + position.X, size + position.Y, size + position.Z),
                new Vector3(size + position.X, -size + position.Y, size + position.Z),
                new Vector3(-size + position.X, -size + position.Y, size + position.Z),

                new Vector3(-size + position.X, size + position.Y, -size + position.Z),
                new Vector3(size + position.X, size + position.Y, -size + position.Z),
                new Vector3(size + position.X, -size + position.Y, -size + position.Z),
                new Vector3(-size + position.X, -size + position.Y, -size + position.Z)
            };
            
            normals = new Vector3[] {
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, -1, 0)
            };

            vpc = new VertexPositionColor[4*6] {
                //Front face
                new VertexPositionColor(verts[0], colour[0]),
                new VertexPositionColor(verts[1], colour[1]),
                new VertexPositionColor(verts[2], colour[2]),
                new VertexPositionColor(verts[3], colour[3]), 

                //Back face
                new VertexPositionColor(verts[5], colour[0]),
                new VertexPositionColor(verts[4], colour[1]),
                new VertexPositionColor(verts[7], colour[2]),
                new VertexPositionColor(verts[6], colour[3]),

                //Left face
                new VertexPositionColor(verts[4], colour[0]),
                new VertexPositionColor(verts[0], colour[1]),
                new VertexPositionColor(verts[3], colour[2]),
                new VertexPositionColor(verts[7], colour[3]),

                //Top face
                new VertexPositionColor(verts[4], colour[0]),
                new VertexPositionColor(verts[5], colour[1]),
                new VertexPositionColor(verts[1], colour[2]),
                new VertexPositionColor(verts[0], colour[3]),

                //Right face
                new VertexPositionColor(verts[1], colour[0]),
                new VertexPositionColor(verts[5], colour[1]),
                new VertexPositionColor(verts[6], colour[2]),
                new VertexPositionColor(verts[2], colour[3]),

                //Bottom face
                new VertexPositionColor(verts[3], colour[0]),
                new VertexPositionColor(verts[2], colour[1]),
                new VertexPositionColor(verts[6], colour[2]),
                new VertexPositionColor(verts[7], colour[3])  
            };

            indices = new short[2 * 3 * 6] {
                0, 1, 2, 
                0, 2, 3,
                4, 5, 6, 
                4, 6, 7,
                8, 9, 10, 
                8, 10, 11,
                12, 13, 14,
                12, 14, 15,
                16, 17, 18,
                16, 18, 19,
                20, 21, 22,
                20, 22, 23
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            BasicEffect effect = ((Game1)Game).effect;
            Camera camera = ((Game1)Game).camera;
            GraphicsDevice gd = ((Game1)Game).GraphicsDevice;

            VertexBuffer vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionColor), vpc.Length, BufferUsage.None);
            vertexBuffer.SetData(vpc);    //Put verts data into vertexBuffer

            IndexBuffer indexBuffer = new IndexBuffer(gd, IndexElementSize.SixteenBits, sizeof(short) * indices.Length, BufferUsage.None);
            indexBuffer.SetData(indices);

            
            gd.Indices = indexBuffer;            
            gd.SetVertexBuffer(vertexBuffer);   //Choose buffer

            effect.World = parent;
            effect.View = camera.view;
            effect.Projection = camera.projection;

            effect.VertexColorEnabled = true;

            //effect.EnableDefaultLighting();
            //effect.LightingEnabled = true;

            //Iterate over every operation 
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vpc.Length, 0, indices.Length/3);
            }

            base.Draw(gameTime);
        }
    }
}
