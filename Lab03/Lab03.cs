using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab03
{
    public class Lab03 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Lab03
        Model model;
        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 lightPosition = new Vector3(1, 1, 1);

        MouseState previousMouseState;
        float angle2;
        SpriteFont font;

        //Lab02
        Effect effect;
        float angle;
        Vector3 CameraPosition;
        Matrix world;
        Matrix view;
        Matrix projection;

        //Vector4 ambient = new Vector4(0, 0, 0, 0);
        //float ambientIntensity = 0.1f;
        //float diffuseIntensity = 1.0f;
        //Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        //Vector3 diffuseLightDirection = new Vector3(1, 1, 1);

        //Vector3 cameraOffset = new Vector3(0, 0, 0);
        //Vector3 cameraPosition = new Vector3(0, 0, 10);

        public Lab03()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("bunny");
            effect = Content.Load<Effect>("SimpleShading");
            font = Content.Load<SpriteFont>("Font");

            world = Matrix.Identity;
            view = Matrix.CreateLookAt(
                10.0f * CameraPosition, //new Vector3(0, 0, 3),
                new Vector3(),
                new Vector3(0, 1, 0));
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                1.33f,
                0.1f, 100);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                float offsetx = 0.01f * (Mouse.GetState().X - previousMouseState.X);
                float offsety = 0.01f * (Mouse.GetState().Y - previousMouseState.Y);
                angle += offsetx;
                angle2 += offsety;
            }

            previousMouseState = Mouse.GetState();

            Vector3 camera = Vector3.Transform(new Vector3(0, 0, 20), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);

            //if (Keyboard.GetState().IsKeyDown(Keys.Left))
            //{
            //    angle += 0.02f;

            //    CameraPosition = new Vector3(
            //         (float)System.Math.Cos(angle),
            //        0,
            //         (float)System.Math.Sin(angle)
            //        );
            //}

            //world = Matrix.Identity;
            //view = Matrix.CreateLookAt(
            //    10.0f * CameraPosition, //new Vector3(0, 0, 3),
            //    new Vector3(),
            //    new Vector3(0, 1, 0));
            //projection = Matrix.CreatePerspectiveFieldOfView(
            //    MathHelper.ToRadians(90),
            //    1.33f,
            //    0.1f, 100);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            //Built-in Draw Method without shader
            //model.Draw(world, view, projection);


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
                        effect.Parameters["DiffuseLightDirection"].SetValue(lightPosition);
                        effect.Parameters["DiffuseIntensity"].SetValue(1f);
                        Matrix worldInverseTranspose = 
                            Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);

                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;

                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, 
                            part.PrimitiveCount);
                    }
                }
            }
            //debug
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "angle:" + angle, Vector2.UnitX + Vector2.UnitY * 12, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}