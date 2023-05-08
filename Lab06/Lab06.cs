using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleEngine;

namespace Lab06
{
    public class Lab06 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //*** Lab06
        Model model; // 3d model
        Skybox skybox;
        Texture2D texture;
        Effect effect;// for Reflection.fx

        // *** Basic Variables for 3D
        float angle = 0; // X rotation
        float angle2 = 0; // Y rotation
        Vector3 cameraPosition;
        Matrix world, view, projection;
        MouseState preMouseState;

        public Lab06()
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

            model = Content.Load<Model>("Helicopter/Helicopter");
            texture = Content.Load<Texture2D>("Helicopter/HelicopterTexture");
            effect = Content.Load<Effect>("Reflection");

            string[] SkyboxTextures = {
                "skybox/SunsetPNG2",
                "skybox/SunsetPNG1",
                "skybox/SunsetPNG4",
                "skybox/SunsetPNG3",
                "skybox/SunsetPNG6",
                "skybox/SunsetPNG5",
            };
            skybox = new Skybox(SkyboxTextures, Content, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState currentMouseState = Mouse.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                preMouseState.LeftButton == ButtonState.Pressed)
            {
                angle -= (preMouseState.X - currentMouseState.X) / 100f;
                angle2 -= (preMouseState.Y - currentMouseState.Y) / 100f;
            }

            preMouseState = currentMouseState;

            world = Matrix.Identity;
            cameraPosition = Vector3.Transform(
                new Vector3(0, 0, 3),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(
                cameraPosition,
                new Vector3(),
                Vector3.Transform(
                    Vector3.Up,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle))
                );
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                1.33f,
                0.1f, 100);

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            RasterizerState originalRasterizerState = _graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            _graphics.GraphicsDevice.RasterizerState = rasterizerState;
            skybox.Draw(view, projection, cameraPosition);
            _graphics.GraphicsDevice.RasterizerState = originalRasterizerState;
            
            DrawModelWithEffect();

            base.Draw(gameTime);
        }

        private void DrawModelWithEffect()
        {
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
                        Matrix worldInverseTranspose =
                            Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["decalMap"].SetValue(texture);
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["environmentMap"].SetValue(skybox.skyBoxTexture);


                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList, part.VertexOffset,
                        part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }
    }
}