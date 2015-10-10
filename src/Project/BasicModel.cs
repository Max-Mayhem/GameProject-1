using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    class BasicModel
    {
        public Model model;
        public  Matrix translation, rotation, scale;
        public Matrix world;
        public Texture2D texture;
        Matrix[] boneTransforms;

        public BasicModel(Model model, Texture2D tex)
        {
            this.model = model;
            this.texture = tex;
            translation = rotation = scale = Matrix.Identity;
            world = Matrix.Identity;
        }

        public virtual void Draw(GraphicsDevice device, Camera camera)
        {
            boneTransforms = new Matrix[model.Bones.Count];
            model.CopyBoneTransformsTo(boneTransforms);
            foreach (ModelMesh modelmesh in model.Meshes)
            {
                foreach (BasicEffect effect in modelmesh.Effects)
                {
                    effect.World = modelmesh.ParentBone.Transform * rotation * scale * world * translation;
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                    effect.TextureEnabled = true;
                    effect.Texture = texture;
                    effect.EnableDefaultLighting();
                }
                
                modelmesh.Draw();
            }
        }
    }
}
