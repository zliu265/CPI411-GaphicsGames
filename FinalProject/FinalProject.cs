using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject
{
    public class FinalProject : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Effect effect;
        Model[] models;
        SpriteFont font;

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(
            new Vector3(0, 0, 20),
            new Vector3(0, 0, 0),
            Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45),
            800f / 600f,
            0.1f,
            100f);
        Vector3 cameraPosition, cameraTarget, lightPosition;
        Matrix lightView, lightProjection;

        float angle = 0;
        float angle2 = 0;
        float angleL = 0;
        float angleL2 = 0;
        float distance = 20;
        MouseState preMouse;

        Texture2D shadowMap;
        RenderTarget2D renderTarget;
        Texture2D planeTexture;
        Vector3 shijiaopro;
        Texture2D defaultTexture;
        Matrix shijiao = Matrix.CreateLookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.UnitY);

        Vector4 shadowColor = new Vector4(0.5f, 0, 0.5f, 1);
        bool boolHelp = false;


        public FinalProject()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");

            models = new Model[2];
            models[0] = Content.Load<Model>("Plane");
            models[1] = Content.Load<Model>("triangle");

            effect = Content.Load<Effect>("ShadowShader");
            planeTexture = Content.Load<Texture2D>("Picture");

            defaultTexture = new Texture2D(GraphicsDevice, 1, 1);
            defaultTexture.SetData(new Color[] { Color.White });

            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.N)) { models[1] = Content.Load<Model>("heng"); }
            if (Keyboard.GetState().IsKeyDown(Keys.P)) { models[1] = Content.Load<Model>("triangle"); }

            if (Keyboard.GetState().IsKeyDown(Keys.R)) { shadowColor = new Vector4(1, 0, 0, 1); }
            if (Keyboard.GetState().IsKeyDown(Keys.G)) { shadowColor = new Vector4(0, 1, 0, 1); }
            if (Keyboard.GetState().IsKeyDown(Keys.B)) { shadowColor = new Vector4(0, 0, 1, 1); }
            if (Keyboard.GetState().IsKeyDown(Keys.Y)) { shadowColor = new Vector4(1, 1, 0, 1); }
            if (Keyboard.GetState().IsKeyDown(Keys.C)) { shadowColor = new Vector4(0, 1, 1, 1); }

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { angle = angle2 = angleL = angleL2 = 0; distance = 20; cameraTarget = Vector3.Zero; }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                angle -= (Mouse.GetState().X - preMouse.X) / 100f;
                angle2 += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                distance += (Mouse.GetState().X - preMouse.X) / 100f;
            }

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                cameraTarget -= ViewRight * (Mouse.GetState().X - preMouse.X) / 10f;
                cameraTarget += ViewUp * (Mouse.GetState().Y - preMouse.Y) / 10f;
            }
            preMouse = Mouse.GetState();

            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(cameraTarget));
            view = Matrix.CreateLookAt(
                cameraPosition,
                cameraTarget,
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            lightPosition = Vector3.Transform(
                new Vector3(0, 0, 20),
                Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));
            lightView = Matrix.CreateLookAt(
                lightPosition,
                Vector3.Zero,
                Vector3.Transform(
                    Vector3.UnitY,
                    Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL)));
            lightProjection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver2, 1f, 1f, 50f);

            // Update Light
            shijiaopro = Vector3.Transform(
                new Vector3(0, 0, 10),
                Matrix.CreateRotationX(-1.85f) * Matrix.CreateRotationY(angleL));
            // Update LightMatrix
            shijiao = Matrix.CreateLookAt(
                shijiaopro, Vector3.Zero,
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(-1.85f) * Matrix.CreateRotationY(angleL)));
            // ******************************//

            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    boolHelp = false;
                }
                else
                {
                    boolHelp = true;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TEMPLATE
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            //********************
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            DrawShadowMap();
            //********************

            GraphicsDevice.SetRenderTarget(null);
            shadowMap = (Texture2D)renderTarget;

            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            DrawShadowScene();

            shadowMap = null;
            //********************
            _spriteBatch.Begin();
            if (boolHelp)
            {
                int i = 0;
                _spriteBatch.DrawString(font, "Rotate Camera: Mouse Left Drag", Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                _spriteBatch.DrawString(font, "Change the distance of camera to the center: Mouse Right Drag", Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                _spriteBatch.DrawString(font, "Adjust the light position " + angleL, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                _spriteBatch.DrawString(font, "Adjust the brightness " + angleL2, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                _spriteBatch.DrawString(font, "'S' Key reset ", Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);

                _spriteBatch.DrawString(font, "Press R become red " , Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                _spriteBatch.DrawString(font, "Press R become green  " , Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                _spriteBatch.DrawString(font, "Press R become Blue  " , Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                _spriteBatch.DrawString(font, "Press Y become yellow  ", Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                _spriteBatch.DrawString(font, "Press C become Cyan  ", Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);

                _spriteBatch.DrawString(font, "Press P become Plane  ", Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                _spriteBatch.DrawString(font, "Press N become teapot  ", Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);

            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        //Private Mathod
        private void DrawShadowScene()
        {
            effect.CurrentTechnique = effect.Techniques["ShadowedScene"];

            foreach (Model model in models)
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                            effect.Parameters["View"].SetValue(view);
                            effect.Parameters["Projection"].SetValue(projection);
                            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                            effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                            effect.Parameters["LightViewMatrix"].SetValue(lightView);
                            effect.Parameters["LightProjectionMatrix"].SetValue(lightProjection);
                            effect.Parameters["LightPosition"].SetValue(lightPosition);
                            effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                            effect.Parameters["ShadowMap"].SetValue(shadowMap);
                            effect.Parameters["ShadowColor"].SetValue(shadowColor);

                            if (model == models[0])
                            {
                                effect.Parameters["ModelTexture"].SetValue(planeTexture);
                            }
                            else if (model == models[1])
                            {
                                effect.Parameters["ModelTexture"].SetValue(defaultTexture);
                            }

                            pass.Apply();

                            GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                            GraphicsDevice.Indices = part.IndexBuffer;
                            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                        }
                    }
                }

            }
        }

        private void DrawShadowMap()
        {
            effect.CurrentTechnique = effect.Techniques["ShadowMapShader"];
            foreach (Model model in models)
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                            effect.Parameters["View"].SetValue(view);
                            effect.Parameters["Projection"].SetValue(projection);
                            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                            effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                            effect.Parameters["LightViewMatrix"].SetValue(lightView);
                            effect.Parameters["LightProjectionMatrix"].SetValue(lightProjection);
                            effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                            effect.Parameters["LightPosition"].SetValue(lightPosition);

                            //effect.Parameters["ProjectiveTexture"].SetValue(texture);

                            pass.Apply();
                            GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                            GraphicsDevice.Indices = part.IndexBuffer;
                            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                        }
                    }
                }
            }
        }


    }
}