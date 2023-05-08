using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleEngine;

namespace Assignment03
{
    public class Assignment03 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont font;

        Vector2 angle = new Vector2(0, 0);
        float dist = 5;
        Matrix dragRotLock;
        Vector3 offset = new Vector3(0, 0, 0);

        MouseState prevMouse;
        KeyboardState prevKey;

        Vector3 cameraPosition = new Vector3(0, 0, -10);
        Vector4 diffuseColor = new Vector4(0.65f, 0.65f, 0.65f, 1);
        Vector4 ambientColor = new Vector4(0.4f, 0.5f, 0.75f, 1);
        float ambientIntensity = 0.25f;
        float specularIntensity = 0.3f;
        float diffuseIntensity = 0.8f;
        float shininess = 20.0f;

        bool mipmap = true;

        float etaRatio = 0.93f;

        Vector3 uvwScale = new Vector3(3.0f, 10.0f, 1.0f);

        Effect effect;
        int technique = 0;
        string[] techniques =
        {
            "Gourad", "Phong", "Blinn", "Schlick", "Toon", "HalfLife"
        };

        Vector3 lightPos = new Vector3(5, 10, 8);
        Vector3 lightColor = new Vector3(1, 1, 1);
        float lightStrength = 200;

        Model model;

        Texture[] texture;
        int bumpMap = 0;

        Skybox skybox;

        Matrix view;
        Matrix projection;

        bool Help = false;
        bool H = false;

        public Assignment03()
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

            texture = new Texture[8]
            {
                Content.Load<Texture>("art"),
                Content.Load<Texture>("BumpTest"),
                Content.Load<Texture>("crossHatch"),
                Content.Load<Texture>("monkey"),
                Content.Load<Texture>("round"),
                Content.Load<Texture>("saint"),
                Content.Load<Texture>("science"),
                Content.Load<Texture>("square"),
            };

            effect = Content.Load<Effect>("BumpMap");
            model = Content.Load<Model>("Torus");

            string[] skyboxTextures =
            {
            "nvlobby_new_posx", "nvlobby_new_negx",
            "nvlobby_new_posy", "nvlobby_new_negy",
            "nvlobby_new_posz", "nvlobby_new_negz"
            };

            skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.1f, 100);
            prevMouse = Mouse.GetState();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState currKey = Keyboard.GetState();
            MouseState currMouse = Mouse.GetState();

            if (currMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Pressed)
            {
                angle.X -= (currMouse.X - prevMouse.X) * 0.01f;
                angle.Y -= (currMouse.Y - prevMouse.Y) * 0.01f;
            }

            if (currMouse.RightButton == ButtonState.Pressed && prevMouse.RightButton == ButtonState.Pressed)
            {
                dist += (prevMouse.X - currMouse.X) * 0.01f;
            }

            if (currMouse.MiddleButton == ButtonState.Pressed && prevMouse.MiddleButton == ButtonState.Released)
            {
                dragRotLock = Matrix.CreateRotationX(angle.Y) * Matrix.CreateRotationY(angle.X);
            }

            if (currMouse.MiddleButton == ButtonState.Pressed && prevMouse.MiddleButton == ButtonState.Pressed)
            {
                offset += Vector3.Transform(new Vector3((prevMouse.X - currMouse.X) * 0.01f, (prevMouse.Y - currMouse.Y) * -0.01f, 0), dragRotLock);
            }

            cameraPosition = offset + Vector3.Transform(new Vector3(0, 0, dist), Matrix.CreateRotationX(angle.Y) * Matrix.CreateRotationY(angle.X));
            view = Matrix.CreateLookAt(cameraPosition, offset, Vector3.Transform(new Vector3(0, 1, 0), Matrix.CreateRotationX(angle.Y) * Matrix.CreateRotationY(angle.X)));

            prevMouse = currMouse;

            if (Keyboard.GetState().IsKeyDown(Keys.D1)) bumpMap = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) bumpMap = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) bumpMap = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) bumpMap = 3;
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) bumpMap = 4;
            if (Keyboard.GetState().IsKeyDown(Keys.D6)) bumpMap = 5;
            if (Keyboard.GetState().IsKeyDown(Keys.D7)) bumpMap = 6;
            if (Keyboard.GetState().IsKeyDown(Keys.D8)) bumpMap = 7;

            if (Keyboard.GetState().IsKeyDown(Keys.F1)) technique = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.F2)) technique = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.F3)) technique = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.F4)) technique = 3;
            if (Keyboard.GetState().IsKeyDown(Keys.F5)) technique = 4;


            if (Keyboard.GetState().IsKeyDown(Keys.U))
            {
                float decrease = 1;
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    decrease = -1;
                }
                uvwScale.X += 0.1f * decrease;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.V))
            {
                float decrease = 1;
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    decrease = -1;
                }
                uvwScale.Y += 0.1f * decrease;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                float decrease = 1;
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    decrease = -1;
                }
                uvwScale.Z += 0.01f * decrease;
                uvwScale.Z = MathHelper.Clamp(uvwScale.Z, 0, 1);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) lightPos.X -= 0.5f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) lightPos.X += 0.5f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) lightPos.Z -= 0.5f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) lightPos.Z += 0.5f;
            if (Keyboard.GetState().IsKeyDown(Keys.PageDown)) lightPos.Y -= 0.5f;
            if (Keyboard.GetState().IsKeyDown(Keys.PageUp)) lightPos.Y += 0.5f;

            if (currKey.IsKeyDown(Keys.M) && !prevKey.IsKeyDown(Keys.M)) mipmap = !mipmap;

            if (currKey.IsKeyDown(Keys.OemQuestion) && !prevKey.IsKeyDown(Keys.OemQuestion)) Help = !Help;
            if (currKey.IsKeyDown(Keys.H) && !prevKey.IsKeyDown(Keys.H)) H = !H;

            if (currKey.IsKeyDown(Keys.S))
            {
                lightPos = new Vector3(5, 10, 8);
                lightColor = new Vector3(1, 1, 1);
                lightStrength = 200;
                dist = 5;
                angle = new Vector2(0, 0);
                offset = new Vector3(0, 0, 0);
            }

            prevKey = currKey;
            base.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            RasterizerState originalRasterizerState = _graphics.GraphicsDevice.RasterizerState;

            RasterizerState ras = new RasterizerState();
            ras.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = ras;

            skybox.Draw(view, projection, cameraPosition);

            effect.CurrentTechnique = effect.Techniques[technique];
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        Matrix model = mesh.ParentBone.Transform;
                        model *= Matrix.CreateScale(0.25f);
                        effect.Parameters["Model"].SetValue(model);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        Matrix modelInverseTranspose =
                        Matrix.Transpose(Matrix.Invert(model));
                        effect.Parameters["ModelInverseTranspose"].SetValue(modelInverseTranspose);
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["AmbientColor"].SetValue(ambientColor);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);
                        effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
                        effect.Parameters["Shininess"].SetValue(shininess);
                        effect.Parameters["LightPosition"].SetValue(lightPos);
                        effect.Parameters["LightStrength"].SetValue(lightStrength);
                        effect.Parameters["LightColor"].SetValue(lightColor);
                        effect.Parameters["NormalMap"].SetValue(texture[bumpMap]);
                        effect.Parameters["UvwScale"].SetValue(uvwScale);
                        effect.Parameters["SkyboxTexture"].SetValue(skybox.skyBoxTexture);
                        effect.Parameters["EtaRatio"].SetValue(etaRatio);
                        effect.Parameters["MipMap"].SetValue(mipmap ? 1 : 0);
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);

                    }
                }
            }
            _spriteBatch.Begin();
            if (Help)
            {
                _spriteBatch.DrawString(font, "Rotate Camera: Left Mouse; Change Camera Distance: Right Mouse; Translate Camera: Middle Mouse", new Vector2(10, 360), Color.Red);
                _spriteBatch.DrawString(font, "Move Light (Arrow Keys): Left/Right = X/Z axis, Up/Down = Y axis", new Vector2(10, 380), Color.Red);
                _spriteBatch.DrawString(font, "1-8 keys: Change Normal Map Image; F1-F10 keys: Change Lighting Model", new Vector2(10, 400), Color.Red);
                _spriteBatch.DrawString(font, "U/V (+Shift): Change UV of normal map; W (+Shift): Change weight of normal map", new Vector2(10, 420), Color.Red);
                _spriteBatch.DrawString(font, "M: Toggle Mipmaps", new Vector2(10, 440), Color.Red);
                _spriteBatch.DrawString(font, "?: Toggle Help; H: Toggle Debug Info; Reset Camera/Light: S", new Vector2(10, 460), Color.Red);
            }

            if (H)
            {
                _spriteBatch.DrawString(font, "Camera Angle: " + VectorToString(angle) + "; Distance: " + dist + "; Offset: " + VectorToString(offset), new Vector2(10, 10), Color.Red);
                _spriteBatch.DrawString(font, "Light Position: " + VectorToString(lightPos) + "; Intensity: " + lightStrength, new Vector2(10, 30), Color.Red);
                _spriteBatch.DrawString(font, "UV Scaling: " + uvwScale.X.ToString("0.00") + ", " + uvwScale.Y.ToString("0.00") + "; Normal Map Weight: " + uvwScale.Z + "; Mipmaps: " + (mipmap ? "On" : "Off"), new Vector2(10, 50), Color.Red);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        string VectorToString(Vector2 vec)
        {
            return "(" + vec.X.ToString("0.00") + ", " + vec.Y.ToString("0.00") + ")";
        }
        string VectorToString(Vector3 vec)
        {
            return "(" + vec.X.ToString("0.00") + ", " + vec.Y.ToString("0.00") + ", " + vec.Z.ToString("0.00") + ")";
        }
    }

    
}