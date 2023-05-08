using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Assignment01
{
    public class Assignment01 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        // *** Lab2 Variables
        Matrix world;
        Matrix view;
        Matrix projection;

        float angle = 0;
        float angle2 = 0;
        float distance = 20;
        float middleMouseX, middleMouseY;
        float lightAngle1, lightAngle2;
        Model model;
        Effect effect;
        SpriteFont font;
        Vector4 ambientColor = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.1f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 lightPosition = new Vector3(1, 1, 1);
        float diffuseIntensity = 1.0f;

        MouseState previousMouseState;
        KeyboardState previousKeyboardState;

        // *** Lab 4
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        float specularIntensity = 1.0f;
        float shininess = 20.0f;
        Vector3 cameraPosition;

        int technique = 0;
        string ShaderType = "Gauraud";
        bool boolHelp = false;
        bool boolInformation = false;


        public Assignment01()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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

            model = Content.Load<Model>("Torus");
            effect = Content.Load<Effect>("SimpleShading");
            font = Content.Load<SpriteFont>("Font");

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f, 100
                );

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //partB-GeometryLoder
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                model = Content.Load<Model>("box");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                model = Content.Load<Model>("Sphere");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                model = Content.Load<Model>("Torus");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                model = Content.Load<Model>("Teapot");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D5))
            {
                model = Content.Load<Model>("bunny");
            }

            //partC-Shader/Lighting Models
            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                technique = 0;
                ShaderType = "Gouraud";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                technique = 1;
                ShaderType = "Phong";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                technique = 2;
                ShaderType = "PhongBlinn";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F4))
            {

                technique = 3;
                ShaderType = "Schlick";
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                technique = 4;
                ShaderType = "Toon";

            }
            if (Keyboard.GetState().IsKeyDown(Keys.F6))
            {
                technique = 5;
                ShaderType = "HalfLife";
            }

            //partC- light properties
            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                ambientIntensity += 0.02f;

                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    ambientIntensity -= 0.04f;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                ambientColor += new Vector4(0.02f, 0, 0, 0);

                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    ambientColor -= new Vector4(0.04f, 0, 0, 0);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                ambientColor += new Vector4(0, 0.02f, 0, 0);

                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    ambientColor -= new Vector4(0, 0.04f, 0, 0);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                ambientColor += new Vector4(0, 0, 0.02f, 0);

                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    ambientColor -= new Vector4(0, 0, 0.04f, 0);
                }
            }

            //partC-specular & shininess
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    shininess += 0.02f;
                }
                else
                {
                    specularIntensity += 0.02f;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    shininess -= 0.02f;
                }
                else
                {
                    specularIntensity -= 0.02f;
                }
            }

            //partC - text information
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

            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    boolInformation = false;
                }
                else
                {
                    boolInformation = true;
                }
            }

            //partA*************************************************************
            MouseState currentMouseState = Mouse.GetState();
            KeyboardState currentKeyboardState = Keyboard.GetState();

            //partA - Mouse Left Drag
            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Pressed)
            {
                angle += (previousMouseState.X - currentMouseState.X) / 100f;
                angle2 += (previousMouseState.Y - currentMouseState.Y) / 100f;
            }

            //partA - Mouse Right Drag
            if (currentMouseState.RightButton == ButtonState.Pressed &&
                previousMouseState.RightButton == ButtonState.Pressed)
            {
                distance += (previousMouseState.Y - currentMouseState.Y) / 100f;
            }

            //partA - mouse Middle Drag
            if (currentMouseState.MiddleButton == ButtonState.Pressed &&
                previousMouseState.MiddleButton == ButtonState.Pressed)
            {
                middleMouseX += (previousMouseState.X - currentMouseState.X) / 100f;
                middleMouseY += (previousMouseState.Y - currentMouseState.Y) / 100f;
            }

            //partA - arrow keys
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                lightAngle1 -= 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                lightAngle1 += 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                lightAngle2 += 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                lightAngle2 -= 0.02f;
            }

            //partA - Reset camera and light
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                view = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
                lightPosition = new Vector3(0.5f, 1, 1);
                angle = 0;
                angle2 = 0;
                lightAngle1 = 0;
                lightAngle2 = 0;
                distance = 0f;
                middleMouseX = 0f;
                middleMouseY = 0f;
                ambientColor = new Vector4(0, 0, 0, 0);
            }

            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                  Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));

            view = Matrix.CreateLookAt(
                cameraPosition,
                Vector3.Zero,
                Vector3.Transform(
                    Vector3.Up,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle))
                );

            // *** Update Mouse State
            previousMouseState = Mouse.GetState();
            previousKeyboardState = currentKeyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            effect.CurrentTechnique = effect.Techniques[technique]; // first technique
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(
                            Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect.Parameters["AmbientColor"].SetValue(ambientColor);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);

                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);

                        effect.Parameters["LightPosition"].SetValue(lightPosition);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
                        effect.Parameters["Shininess"].SetValue(shininess);

                        pass.Apply(); // send the data to GPU
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

            _spriteBatch.Begin();
            if (boolInformation)
            {
            //   _spriteBatch.DrawString(font, "midMouseX: " + middleMouseX + "\nmidMouseY: " + middleMouseY, new Vector2(25, 5), Color.White);
            //_spriteBatch.DrawString(font, "camera angle: " + angle + "\ncamera angle2: " + angle2 , new Vector2(25, 40), Color.White);
            //_spriteBatch.DrawString(font, "light angle: " + lightAngle1 + "\nlight angle2: " + lightAngle2, new Vector2(25, 75), Color.White);
            //_spriteBatch.DrawString(font, "Shader Type: " + ShaderType, new Vector2(25, 110), Color.White);
            //_spriteBatch.DrawString(font, "ambientIntensity: " + ambientIntensity + "\nspecularIntensity: " + specularIntensity, new Vector2(25, 130), Color.White);
            //_spriteBatch.DrawString(font, "shininess: " + shininess, new Vector2(25, 165), Color.White);
                int i = 10;
                _spriteBatch.DrawString(font, "A", Vector2.UnitY * 15 * (i++), Color.White);
                _spriteBatch.DrawString(font, "B", Vector2.UnitY * 15 * (i++), Color.White);
                _spriteBatch.DrawString(font, "C", Vector2.UnitY * 15 * (i++), Color.White);
            }

            if (boolHelp)
            {
                _spriteBatch.DrawString(font, "Rotate Camera: Mouse Left Drag", new Vector2(25, 5), Color.White);
                _spriteBatch.DrawString(font, "Change the distance of camera to the center: Mouse Right Drag", new Vector2(25, 25), Color.White);
                _spriteBatch.DrawString(font, "Translate the camera: Mouse Middle Drag", new Vector2(25, 45), Color.White);
                _spriteBatch.DrawString(font, "Rotate the light: Arrow keys", new Vector2(25, 65), Color.White);
                _spriteBatch.DrawString(font, "Reset camera and light: S Key", new Vector2(25, 85), Color.White);
                _spriteBatch.DrawString(font, "1: Box\n2: Sphere \n3: Torus \n4: Tea Pot \n5: Bunny", new Vector2(25, 125), Color.White);
                _spriteBatch.DrawString(font, "F1: Gouraud (Phong per vertex) \nF2: Phong per pixel\nF3: PhongBlinn\nF4: Schlick\nF5: Toon \nF6: HalfLife", new Vector2(25, 225), Color.White);
                _spriteBatch.DrawString(font, "L: Increase the intensity of light (+ Shift key: decrease)\nR: Increase the red value of light (+ Shift key: decrease)\nG: Increase the green value of light (+ Shift key: decrease)\nB: Increase the blue value of light (+ Shift key: decrease)", new Vector2(25, 350), Color.White);
                _spriteBatch.DrawString(font, "+ (plus):  Increases the intensity\n- (minus): Decreases the intensity", new Vector2(25, 425), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}