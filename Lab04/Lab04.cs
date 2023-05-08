using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Lab04
{
    public class Lab04 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Vector3 diffuseLightDirection = new Vector3(1, 1, 1);
        //float diffuseIntensity = 1.0f;
        //*** Lab03
        Model model; // **** FBX file
        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 lightPosition = new Vector3(1, 1, 1);
        // *** Lab3 Mouse Event
        MouseState preMouseState;

        //*** Lab02
        Effect effect;
        float angle = 0; // X rotation
        float angle2 = 0; // Y rotation
        Vector3 cameraPosition;
        Matrix world, view, projection;

        //*** Lab04
        Vector4 specularColor = new Vector4(1,1,1,1);
        float shininess = 10.0f;
        int shader = 0;

        public Lab04()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // *********************************************
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //**********************************************
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            effect = Content.Load<Effect>("SimpleShadingLab04");
            model = Content.Load<Model>("Torus");
        }
         
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                shininess -= 0.05f;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                shininess += 0.05f;

            }

            if (Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                shader = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                shader = 1;
            }
            // *** Lab03 Mouse Event
            MouseState currentMouseState = Mouse.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed && 
                preMouseState.LeftButton == ButtonState.Pressed)
            {
                angle -= (preMouseState.X - currentMouseState.X) / 100f;
                angle2 -= (preMouseState.Y - currentMouseState.Y) / 100f;
            }
            preMouseState = currentMouseState;

            world = Matrix.Identity;
            //view = Matrix.CreateRotationY(angle) *
            //    Matrix.CreateRotationX(angle2) *
            //    Matrix.CreateTranslation(new Vector3(0, 0, -20));
            cameraPosition = Vector3.Transform(
                new Vector3(0, 0, 20),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(
                cameraPosition,
                new Vector3(),
                Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                1.33f,
                0.1f, 100);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            effect.CurrentTechnique = effect.Techniques[0];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["AmbientColor"].SetValue(ambient);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);

                        // **** Solution of Lab3
                        effect.Parameters["DiffuseIntensity"].SetValue(1f);
                        Matrix worldInverseTranspose =
                            Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);
                        //**** Lab04
                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["Shininess"].SetValue(20f);
                        effect.Parameters["LightPosition"].SetValue(lightPosition);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);

                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;

                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, 
                            0,
                            part.StartIndex, 
                            part.PrimitiveCount);

                    }
                }
            }

            base.Draw(gameTime);
        }
    }
}