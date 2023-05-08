using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;

namespace Lab02
{
    public class Lab02 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // *** Lab2
        float angle;
        float distance = 1.0f;
        Vector3 CameraPos;
        Matrix world;
        Matrix view;
        Matrix projection;

        Effect effect;
        VertexPositionTexture[] vertices =
        {
            new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0.5f,0)),
            new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1,1)),
            new VertexPositionTexture(new Vector3(-1, 0, 0), new Vector2(0,1))
        };

        public Lab02()
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

            effect = Content.Load<Effect>("SimpleTexture");
            effect.Parameters["MyTexture"].SetValue(Content.Load<Texture2D>("logo_mg"));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                //Debug.WriteLine("TESTING");
                angle += 0.02f;
                Vector3 offset = new Vector3(
                    (float)System.Math.Cos(angle),
                    (float)System.Math.Sin(angle),
                    0);

                CameraPos = new Vector3(
                     (float)System.Math.Cos(angle),
                    0,
                     (float)System.Math.Sin(angle)
                    );
                effect.Parameters["offset"].SetValue(offset);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                angle -= 0.02f;
                Vector3 offset = new Vector3(
                    (float)System.Math.Cos(angle),
                    (float)System.Math.Sin(angle),
                    0);
                effect.Parameters["offset"].SetValue(offset);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                distance += 0.02f;
                Vector3 offset = new Vector3(
                    (float)System.Math.Cos(angle),
                    (float)System.Math.Sin(angle),
                    0);
                effect.Parameters["offset"].SetValue(offset);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                distance -= 0.02f;
                Vector3 offset = new Vector3(
                    (float)System.Math.Cos(angle),
                    (float)System.Math.Sin(angle),
                    0);
                effect.Parameters["offset"].SetValue(offset);
            }



            Vector3 cameraPosition = distance * new Vector3(
                (float)System.Math.Sin(angle),
                0,
                (float)System.Math.Cos(angle));

            Matrix world = Matrix.Identity;
            Matrix view = Matrix.CreateLookAt(
                cameraPosition,
                new Vector3(),
                new Vector3(0, 1, 0));

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                GraphicsDevice.Viewport.AspectRatio,
                //1.33f,
                0.1f, 100);

            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.AlphaBlend; // Activate Alpha mode

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
            }

            base.Draw(gameTime);
        }
    }
}