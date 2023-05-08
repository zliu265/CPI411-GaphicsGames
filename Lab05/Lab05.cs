using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleEngine;

namespace Lab05
{
    public class Lab05 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont font;
        Effect effect;

        MouseState preMouseState;
        float angle = 0; // X rotation
        float angle2 = 0; // Y rotation
        Vector3 cameraPosition;
        Matrix world, view, projection;
        Skybox skybox;
        string[] SkyboxTextures = {
                "skybox/SunsetPNG1",
                "skybox/SunsetPNG2",
                "skybox/SunsetPNG3",
                "skybox/SunsetPNG4",
                "skybox/SunsetPNG5",
                "skybox/SunsetPNG6",
            };


        public Lab05()
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

            skybox = new Skybox(SkyboxTextures, Content, GraphicsDevice);
            effect = Content.Load<Effect>("skybox/skybox");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                angle -= 0.03f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                angle += 0.03f;
            }

            MouseState currentMouseState = Mouse.GetState();
            preMouseState = currentMouseState;
            world = Matrix.Identity;
            cameraPosition = Vector3.Transform(
    new Vector3(0, 0, 20),
    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(
                cameraPosition,
                new Vector3(),
                Vector3.UnitY);
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

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            skybox.Draw(view, projection, cameraPosition);

            _spriteBatch.Begin();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}