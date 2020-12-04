using Bloxx.Source;
using Bloxx.Source.Map;
using Bloxx.Source.Menus;
using Bloxx.Source.NPCs;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoExt;
using MonoNet;
using System;

namespace Bloxx
{
    public class Main : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Component screen;

        //public static readonly int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
        //                           screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

#if DEBUG
        public const int screenWidth = 720,
                          screenHeight = 480;
#else
        public const int screenWidth = 1920,
                          screenHeight = 1080;
#endif

        //public const int screenWidth = 720,
        //                 screenHeight = 480;

        public static Rectangle GetRectangle => new Rectangle(0, 0, screenWidth, screenHeight);

        public static Main main;

        public static SpriteFont SmallFont { get; private set; }
        public static SpriteFont MediumFont { get; private set; }

        public static readonly Random random = new Random();

        public Main()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = screenWidth,
                PreferredBackBufferHeight = screenHeight,
#if DEBUG
                IsFullScreen = false
#else
                IsFullScreen = true
#endif
            };
            
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            main = this;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            SmallFont = Content.Load<SpriteFont>("Fonts/SmallFont");
            MediumFont = Content.Load<SpriteFont>("Fonts/MediumFont");

            SpecialContent.LoadContent(GraphicsDevice);
            Input.LoadData(this);
            TileData.LoadTiles(Content);
            Player.LoadSprites(Content);

#if DEBUG
            screen = new MainMenu();
#else
            screen = new SplashScreen(Content, screenWidth, screenHeight, () => { screen = new MainMenu(); });
#endif
        }

        protected override void UnloadContent()
        {
            SpecialContent.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();

            if (Input.KeyDown(Keys.Escape)) Exit();
            else if (Input.KeyDown(Keys.F11))
            {
                graphics.IsFullScreen ^= true;

                graphics.ApplyChanges();
            }

            Networking.Update();

            if (Networking.Connected && screen is ICanReceive cr)
            {
                NetIncomingMessage message;
                while ((message = Networking.ReceiveMessage()) != null)
                {
                    cr.ReceiveMessage(message);
                }
                /*
                foreach (string s in Connection.connection.ReceiveMessages())
                {
                    if (screen is ICanReceive cr) cr.RelayMessage(s);
                }
                */
            }

            screen?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (screen != null)
            {
                screen.Show(gameTime, spriteBatch);
            }

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            Input.DrawCursor(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        protected override void OnExiting(object sender, EventArgs args)
        {
            if (Networking.Connected) Networking.Stop();

            base.OnExiting(sender, args);
        }

    }
}
