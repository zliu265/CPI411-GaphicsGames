using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411.SimpleEngine;

namespace Assignment04
{
    public class Assignment04 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Model model;
        Texture2D[] texture;
        Effect effect;
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 30), Vector3.Zero, Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        Matrix faceCamera;

        Vector3 cameraPosition, cameraTarget, lightPosition;
        float angle, angle2, angleL, angleL2, distance;
        MouseState preMouse;
        KeyboardState preKeyboard;

        ParticleManager particleManager;
        Vector3 particlePosition = new Vector3(0, 0, 0);
        Vector3 particleVelocity = new Vector3(0, 0, 0);
        Vector3 particleAcceleration = new Vector3(0, -3f, 0);
        float particleWind = 0f;
        float particleWindDirection = 0f;
        float particleSpeed = 1f;
        float particleResilience = 1.0f;
        float particleFriction = 1.0f;
        float particleGroundY = -2.0f;
        int particleMaxNum = 10000; //Total particles
        int particleEmissionNum = 10; //# particles per Emission
        int particleEmissionSpan = 50;
        int particleTotalTime = 0;
        int particleMaxAge = 1;
        bool particleBoundON = false;
        float emissionSize = 1.0f;
        bool showHelp = true;

        System.Random random;

        enum EmissionShape { Square, Curve, Ring };
        int emissionShape = 0;

        enum EmissionType { F1, F2, F3 };
        int emissionType = 0;
        int leiXing = 0;

        public Assignment04()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            base.Initialize();
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");
            effect = Content.Load<Effect>("ParticleShader");

            texture = new Texture2D[4]
            {
                Content.Load<Texture2D>("Square"),
                Content.Load<Texture2D>("smoke"),
                Content.Load<Texture2D>("water"),
                Content.Load<Texture2D>("fire"),
            };

            model = Content.Load<Model>("Plane");
            angle = angle2 = 0;
            distance = 30;
            random = new System.Random();

            particleManager = new ParticleManager(GraphicsDevice, 120);
            particlePosition = Vector3.Zero;
        }

        protected override void UnloadContent() { }

        private Vector3 _getParticlePosition()
        {
            if (emissionShape == (int)EmissionShape.Square)
            {
                return new Vector3(
                    (float)(emissionSize * (random.NextDouble() - 0.5)),
                    0,
                    (float)(emissionSize * (random.NextDouble() - 0.5)));
            }
            else if (emissionShape == (int)EmissionShape.Curve)
            {
                double randomAngle = Math.PI * (random.NextDouble() * 2.0 - 1.0);
                return new Vector3(
                    (float)randomAngle / 3.0f * emissionSize,
                    0,
                    emissionSize * (float)Math.Sin(randomAngle));
            }
            else if (emissionShape == (int)EmissionShape.Ring)
            {
                double randomAngle = MathHelper.Pi * (random.NextDouble() * 2.0 - 1.0);
                return new Vector3(
                    emissionSize * (float)Math.Sin(randomAngle),
                    0,
                    emissionSize * (float)Math.Cos(randomAngle));
            }
            else return new Vector3(0, 0, 0);
        }

        private Vector3 _getParticleVelocity()
        {
            if (emissionType == (int)EmissionType.F1)
                return new Vector3(0, 2, 0);
            else if (emissionType == (int)EmissionType.F2)
                return new Vector3((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1);
            else return particleVelocity;
        }

        private Vector3 _getParticleAcceleration()
        {
            if (emissionType == (int)EmissionType.F1)
                return new Vector3(0, 0, 0);
            else return particleAcceleration;
        }

        private void _generateParticles()
        {
            if (particleTotalTime % particleEmissionSpan == 0)
            {
                for (int i = 0; i < particleEmissionNum; i++)
                {
                    double angle = System.Math.PI * (i * 6) / 180.0;
                    Particle particle = particleManager.getNext();
                    particle.Position = _getParticlePosition(); //position based on shape
                    particle.Velocity = particleSpeed * _getParticleVelocity(); //velocity determined
                    particle.Acceleration = _getParticleAcceleration(); //gravity
                    particle.MaxAge = particleMaxAge; //lifespan of particles
                    particle.Init();
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // NormalMaps
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) leiXing = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) leiXing = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) leiXing = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) leiXing = 3;

            if (Keyboard.GetState().IsKeyDown(Keys.N) && !Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleEmissionNum = MathHelper.Clamp(particleEmissionNum + 1, 1, 100);
            if (Keyboard.GetState().IsKeyDown(Keys.N) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleEmissionNum = MathHelper.Clamp(particleEmissionNum - 1, 1, 100);

            if (Keyboard.GetState().IsKeyDown(Keys.E) && !Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleEmissionSpan = MathHelper.Clamp(particleEmissionNum + 1, 1, 100);
            if (Keyboard.GetState().IsKeyDown(Keys.E) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleEmissionSpan = MathHelper.Clamp(particleEmissionNum - 1, 1, 100);

            if (Keyboard.GetState().IsKeyDown(Keys.A) && !Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleMaxAge = MathHelper.Clamp(particleEmissionNum + 1, 1, 10);
            if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleMaxAge = MathHelper.Clamp(particleEmissionNum - 1, 1, 10);

            if (Keyboard.GetState().IsKeyDown(Keys.S) && !Keyboard.GetState().IsKeyDown(Keys.LeftShift)) emissionSize = MathHelper.Clamp(particleEmissionNum + 0.01f, 0.01f, 10.0f);
            if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) emissionSize = MathHelper.Clamp(particleEmissionNum - 0.01f, 0.01f, 10.0f);

            if (Keyboard.GetState().IsKeyDown(Keys.R) && !Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleResilience = MathHelper.Clamp(particleEmissionNum + 0.01f, 0f, 10.0f);
            if (Keyboard.GetState().IsKeyDown(Keys.R) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleResilience = MathHelper.Clamp(particleEmissionNum - 0.01f, 0f, 10.0f);

            if (Keyboard.GetState().IsKeyDown(Keys.F) && !Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleFriction = MathHelper.Clamp(particleEmissionNum + 0.01f, 0f, 1.0f);
            if (Keyboard.GetState().IsKeyDown(Keys.F) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleFriction = MathHelper.Clamp(particleEmissionNum - 0.01f, 0f, 1.0f);

            if (Keyboard.GetState().IsKeyDown(Keys.W) && !Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleWind = MathHelper.Clamp(particleEmissionNum + 0.01f, 0f, 10.0f);
            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleWind = MathHelper.Clamp(particleEmissionNum - 0.01f, 0f, 10.0f);

            if (Keyboard.GetState().IsKeyDown(Keys.V) && !Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleSpeed += 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.V) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) particleSpeed -= 0.01f;

            if (Keyboard.GetState().IsKeyDown(Keys.B) && !preKeyboard.IsKeyDown(Keys.B)) particleBoundON = !particleBoundON;

            if (Keyboard.GetState().IsKeyDown(Keys.F1) && preKeyboard.IsKeyDown(Keys.F1))
            {
                emissionShape = (int)EmissionShape.Square;
                emissionType = (int)EmissionType.F1;
                _generateParticles();
                particleTotalTime++;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F1) && preKeyboard.IsKeyDown(Keys.F1)) particleTotalTime = 0;

            if (Keyboard.GetState().IsKeyDown(Keys.F2) && preKeyboard.IsKeyDown(Keys.F2))
            {
                emissionShape = (int)EmissionShape.Square;
                emissionType = (int)EmissionType.F2;
                _generateParticles();
                particleTotalTime++;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F2) && preKeyboard.IsKeyDown(Keys.F2)) particleTotalTime = 0;

            if (Keyboard.GetState().IsKeyDown(Keys.F3) && preKeyboard.IsKeyDown(Keys.F3))
            {
                emissionShape = (int)EmissionShape.Square;
                emissionType = (int)EmissionType.F3;
                _generateParticles();
                ++particleTotalTime;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F3) && preKeyboard.IsKeyDown(Keys.F3)) particleTotalTime = 0;

            if (Keyboard.GetState().IsKeyDown(Keys.F4) && !preKeyboard.IsKeyDown(Keys.F4))
            {
                if (emissionShape == (int)EmissionShape.Square) emissionShape = (int)EmissionShape.Curve;
                else if (emissionShape == (int)EmissionShape.Curve) emissionShape = (int)EmissionShape.Ring;
                else if (emissionShape == (int)EmissionShape.Ring) emissionShape = (int)EmissionShape.Square;
            }

            if (emissionType == (int)EmissionType.F3)
                particleManager.Bounce(particleGroundY, particleResilience, particleFriction);
            particleManager.WindEffect(particleWind * 0.1f, particleWindDirection);
            particleManager.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f);


            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { angle = angle2 = angleL = angleL2 = 0; distance = 30; cameraTarget = Vector3.Zero; }
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !preKeyboard.IsKeyDown(Keys.OemQuestion)) showHelp = !showHelp;


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
            preKeyboard = Keyboard.GetState();

            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(cameraTarget));
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            faceCamera = Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(cameraTarget);
            lightPosition = Vector3.Transform(new Vector3(0, 0, 10), Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));

            // *** Lab 10
            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                Particle particle = particleManager.getNext();
                particle.Position = particlePosition;
                particle.Velocity = new Vector3(0, 0, 0);
                particle.Acceleration = new Vector3(0, 0, 0);
                particle.MaxAge = 1;
                particle.Init();
            }
            // ****** Update all particles
            particleManager.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f);

            base.Update(gameTime);

        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            model.Draw(world * Matrix.CreateTranslation(0, particleGroundY, 0), view, projection);

            effect.CurrentTechnique = effect.Techniques["particle"];
            effect.CurrentTechnique.Passes[0].Apply();
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["Texture"].SetValue(texture[leiXing]);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["InverseCamera"].SetValue(faceCamera);
            particleManager.Draw(GraphicsDevice);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            spriteBatch.Begin();
            if (showHelp)
            {
                int i = 0;
                spriteBatch.DrawString(font, "Particle Total Time " + particleTotalTime, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                spriteBatch.DrawString(font, "Emission Shape (F4) " + emissionShape, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                spriteBatch.DrawString(font, "Paticle Max number " + particleMaxNum, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                spriteBatch.DrawString(font, "Paticle Emission Num (N) " + particleEmissionNum, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                spriteBatch.DrawString(font, "Paticle Emission Span (E) " + particleEmissionSpan, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                spriteBatch.DrawString(font, "Paticle Max Age (A) " + particleMaxAge, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                spriteBatch.DrawString(font, "Particle Bounce Option (B) " + particleBoundON, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                spriteBatch.DrawString(font, "Paticle Resilience (R) " + particleResilience, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                spriteBatch.DrawString(font, "Particle Friction " + particleFriction, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                spriteBatch.DrawString(font, "Wind Direction (D) " + particleWindDirection, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                spriteBatch.DrawString(font, "Particle Speed (W) " + particleSpeed, Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
                spriteBatch.DrawString(font, "Emission Size (S) " + emissionSize.ToString("0.000"), Vector2.UnitX * 20 + Vector2.UnitY * 15 * (i++), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
