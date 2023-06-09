﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab09
{
    public class Lab09 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        SpriteFont font;
        Effect effect;
        Effect lighting;
        Effect depthmap;
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
        Matrix lightView;
        Matrix lightProjection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver2, 1f, 1f, 50f);
        float angle = 0;
        float angle2 = 0;
        float angleL = 0;
        float angleL2 = 0;
        float distance = 20;
        MouseState preMouse;
        Model plane;
        Model torus;
        Texture2D texture;

        RenderTarget2D shadowMap;

        public Lab09()
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
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            plane = Content.Load<Model>("Plane");
            torus = Content.Load<Model>("torus");
            lighting = Content.Load<Effect>("Lighting");
            depthmap = Content.Load<Effect>("DepthMap");

            shadowMap = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            lighting.Parameters["ShadowMap"].SetValue((Texture2D)shadowMap);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { angle = angle2 = angleL = angleL2 = 0; distance = 30; cameraTarget = Vector3.Zero; }
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
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
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
                new Vector3(0, 0, 10),
                Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));
            lightView = Matrix.CreateLookAt(
                lightPosition,
                Vector3.Zero,
                Vector3.Transform(
                    Vector3.UnitY,
                    Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL)));
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.SetRenderTarget(shadowMap);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            effect = depthmap;
            effect.CurrentTechnique = effect.Techniques[0];

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {

                effect.Parameters["View"].SetValue(view);
                effect.Parameters["Projection"].SetValue(projection);
                effect.Parameters["LightViewMatrix"].SetValue(lightView);
                effect.Parameters["LightProjectionMatrix"].SetValue(lightProjection);
                effect.Parameters["LightPosition"].SetValue(lightPosition);
                effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                pass.Apply();
                foreach (ModelMesh mesh in plane.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            part.VertexOffset,
                            part.StartIndex,
                            part.PrimitiveCount);
                    }
                }
                foreach (ModelMesh mesh in torus.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        Matrix world = mesh.ParentBone.Transform * Matrix.CreateTranslation(new Vector3(0.0f, 2.25f, 0.0f));
                        effect.Parameters["World"].SetValue(world);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(world));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            part.VertexOffset,
                            part.StartIndex,
                            part.PrimitiveCount);
                    }
                }
            }
            // ************************************* //
            // ************ LIGHTED SCENE ************ //
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            effect = lighting;
            effect.CurrentTechnique = effect.Techniques[0];

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {

                effect.Parameters["View"].SetValue(view);
                effect.Parameters["Projection"].SetValue(projection);
                effect.Parameters["LightViewMatrix"].SetValue(lightView);
                effect.Parameters["LightProjectionMatrix"].SetValue(lightProjection);
                effect.Parameters["LightPosition"].SetValue(lightPosition);
                effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                pass.Apply();
                foreach (ModelMesh mesh in plane.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            part.VertexOffset,
                            part.StartIndex,
                            part.PrimitiveCount);
                    }
                }
                foreach (ModelMesh mesh in torus.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        Matrix world = mesh.ParentBone.Transform * Matrix.CreateTranslation(new Vector3(0.0f, 2.25f, 0.0f));
                        effect.Parameters["World"].SetValue(world);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(world));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            part.VertexOffset,
                            part.StartIndex,
                            part.PrimitiveCount);
                    }
                }
            }
            base.Draw(gameTime);
        }
    }
}