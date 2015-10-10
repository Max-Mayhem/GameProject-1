using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    //Cube class contains all the vectors for vertices, texture coordinates and face normals
    class ShadedCube : DrawableGameComponent
    {   
        public Vector3 position;
        public Matrix parent;
        public Color colour;
        public VertexPositionNormalTexture[] vpc;
        Texture2D texture;
        public short[] indices;

        public ShadedCube(Game game, Vector3 pos, float size, Color col) : base(game)
        {
            texture = new Texture2D(game.GraphicsDevice, 1, 1);
            texture.SetData<Color>(new Color[]{col});// fill the texture with white
            colour = col;

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

            vpc = new VertexPositionNormalTexture[4*6] {
                //Front face
                new VertexPositionNormalTexture(verts[0], normals[0], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[1], normals[0], new Vector2(1, 0)),
                new VertexPositionNormalTexture(verts[2], normals[0], new Vector2(1, 1)),
                new VertexPositionNormalTexture(verts[3], normals[0], new Vector2(0, 1)), 

                //Back face
                new VertexPositionNormalTexture(verts[5], normals[1], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[4], normals[1], new Vector2(1, 0)),
                new VertexPositionNormalTexture(verts[7], normals[1], new Vector2(1, 1)),
                new VertexPositionNormalTexture(verts[6], normals[1], new Vector2(0, 1)),

                //Left face
                new VertexPositionNormalTexture(verts[4], normals[2], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[0], normals[2], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[3], normals[2], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[7], normals[2], new Vector2(0, 0)),

                //Top face
                new VertexPositionNormalTexture(verts[4], normals[3], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[5], normals[3], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[1], normals[3], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[0], normals[3], new Vector2(0, 0)),

                //Right face
                new VertexPositionNormalTexture(verts[1], normals[4], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[5], normals[4], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[6], normals[4], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[2], normals[4], new Vector2(0, 0)),

                //Bottom face
                new VertexPositionNormalTexture(verts[3], normals[5], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[2], normals[5], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[6], normals[5], new Vector2(0, 0)),
                new VertexPositionNormalTexture(verts[7], normals[5], new Vector2(0, 0))  
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

            VertexBuffer vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionNormalTexture), vpc.Length, BufferUsage.None);
            vertexBuffer.SetData(vpc);    //Put verts data into vertexBuffer

            IndexBuffer indexBuffer = new IndexBuffer(gd, IndexElementSize.SixteenBits, sizeof(short) * indices.Length, BufferUsage.None);
            indexBuffer.SetData(indices);

            
            gd.Indices = indexBuffer;            
            gd.SetVertexBuffer(vertexBuffer);   //Choose buffer

            effect.World = parent;
            effect.View = camera.view;
            effect.Projection = camera.projection;

            effect.VertexColorEnabled = false;

            effect.TextureEnabled = true;
            effect.Texture = texture;

            effect.EnableDefaultLighting();
            effect.LightingEnabled = true;

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
