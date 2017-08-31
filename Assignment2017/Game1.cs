using System;
using Microsoft.CSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.AspNet.SignalR.Client;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Assignment2017
{

    public class Game1 : Game
    {

        static HttpClient client = new HttpClient();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Vector3 cameraPosition = new Vector3(0.0f, 0.0f, GameConstants.CameraHeight);
        Matrix projectionMatrix;
        Matrix viewMatrix;
        SoundEffect soundEngine;
        SoundEffectInstance soundEngineInstance;
        SoundEffect soundHyperspaceActivation;
        SoundEffect soundExplosion2;
        SoundEffect soundExplosion3;
        SoundEffect soundWeaponsFire;

        enum GameState
        {
            Title, Game, HiScores
        }

        private GameState gameState = GameState.Title;
        private bool connected = false;
        HubConnection hubConnection;
        IHubProxy gameHubProxy;

        Model asteroidModel;
        Model bulletModel;
        Model shipModel;

        Matrix[] asteroidTransforms;
        Matrix[] bulletTransforms;
        Matrix[] shipTransforms;

        Player player = new Player();
        GamePadState currentState;
        GamePadState lastState = GamePad.GetState(PlayerIndex.One);

        Texture2D stars;
        SpriteFont font;
        Vector2 scorePosition = new Vector2(100, 50);

        Viewport mainViewport;
        Viewport leftViewport;
        Viewport rightViewport;
        float aspectRatio;
        string[] messages = new string[10];
        
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            aspectRatio = (float)GraphicsDeviceManager.DefaultBackBufferWidth / (2 * GraphicsDeviceManager.DefaultBackBufferHeight);
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i] = "...";
            }
        }

        private async System.Threading.Tasks.Task ConnectAsync()
        {
            client.BaseAddress = new Uri("http://localhost:53241");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            hubConnection = new HubConnection("http://localhost:53241/");
            gameHubProxy = hubConnection.CreateHubProxy("GameHub");
            gameHubProxy.On<string>("notify", text => NewMessage(text));
            gameHubProxy.On("accept", () => connected = true);
            await hubConnection.Start();
            await gameHubProxy.Invoke("AddPlayer", "Player");
        }
        
        private void NewMessage(string message)
        {
            for (int i = 1; i < messages.Length; i++)
            {
                messages[i - 1] = messages[i];
            }
            messages[messages.Length - 1] = message;
        }

        protected override void Initialize()
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), GraphicsDevice.DisplayMode.AspectRatio, GameConstants.CameraHeight - 1000f, GameConstants.CameraHeight + 1000f);
            viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            NewMessage("Init");
            ConnectAsync();
            base.Initialize();

        }
        
        private Matrix[] SetupEffectDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = projectionMatrix;
                    effect.View = viewMatrix;
                }
            }
            return absoluteTransforms;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            shipModel = Content.Load<Model>("Models\\SmallShip");
            shipTransforms = SetupEffectDefaults(shipModel);
            soundEngine = Content.Load<SoundEffect>("Audio\\engine_2");
            soundEngineInstance = soundEngine.CreateInstance();
            soundHyperspaceActivation = Content.Load<SoundEffect>("Audio\\hyperspace_activate");
            asteroidModel = Content.Load<Model>("Models\\asteroid1");
            asteroidTransforms = SetupEffectDefaults(asteroidModel);
            bulletModel = Content.Load<Model>("Models\\pea_proj");
            bulletTransforms = SetupEffectDefaults(bulletModel);
            stars = Content.Load<Texture2D>("Textures\\B1_stars");
            font = Content.Load<SpriteFont>("default");
            soundExplosion2 = Content.Load<SoundEffect>("Audio\\explosion2");
            soundExplosion3 = Content.Load<SoundEffect>("Audio\\explosion3");
            soundWeaponsFire = Content.Load<SoundEffect>("Audio\\tx0_fire1");

            mainViewport = GraphicsDevice.Viewport;
            leftViewport = mainViewport;
            rightViewport = mainViewport;
            leftViewport.Width = leftViewport.Width / 2;
            rightViewport.Width = rightViewport.Width / 2;
            rightViewport.X = leftViewport.Width + 1;
        }


        protected override void UnloadContent()
        {
        }

        Vector3 modelVelocity = Vector3.Zero;

        protected override void Update(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.Title:
                    UpdateTitle(gameTime);
                    break;
                case GameState.Game:
                    UpdateGame(gameTime);
                    break;
                case GameState.HiScores:
                    UpdateHiScores(gameTime);
                    break;
                default:
                    break;
            }
            base.Update(gameTime);
        }

        private void UpdateHiScores(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        private void UpdateTitle(GameTime gameTime)
        {
            if (connected)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
                if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    gameState = GameState.Game;
                    NewMessage("Start");
                } 
            }
        }

        private void UpdateGame(GameTime gameTime)
        {
            player.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                gameState = GameState.Title;

            UpdateInput();
            
            if (player.CheckForBulletAsteroidCollision(bulletModel.Meshes[0].BoundingSphere.Radius, asteroidModel.Meshes[0].BoundingSphere.Radius))
            {
                soundExplosion2.Play();
            }

            bool shipDestroyed = player.CheckForShipAsteroidCollision(shipModel.Meshes[0].BoundingSphere.Radius, asteroidModel.Meshes[0].BoundingSphere.Radius);
            if (shipDestroyed)
            {
                soundExplosion3.Play();
            }
            
        }

        private void UpdateInput()
        {
            currentState = GamePad.GetState(PlayerIndex.One);

            if (currentState.IsConnected)
            {
                if (player.ship.isActive)
                {
                    player.ship.Update(currentState);
                    PlayEngineSound(currentState);
                }

                if (IsButtonPressed(Buttons.B))
                {
                    player.WarpToCenter();
                    soundHyperspaceActivation.Play();
                }

                if (player.ship.isActive && IsButtonPressed(Buttons.A))
                {
                    player.ShootBullet();
                    soundWeaponsFire.Play();
                    bool isFiring = true;
                }
            }
            lastState = currentState;
        }
        
        

        void PlayEngineSound(GamePadState currentState)
        {
            if (currentState.Triggers.Right >0)
            {
                if (soundEngineInstance.State == SoundState.Stopped)
                {
                    soundEngineInstance.Volume = 0.75f;
                    soundEngineInstance.IsLooped = true;
                    soundEngineInstance.Play();
                }
                else
                {
                    soundEngineInstance.Resume();
                }
            }
            else if (currentState.Triggers.Right == 0)
            {
                if (soundEngineInstance.State==SoundState.Playing)
                    soundEngineInstance.Pause();
            }
        }

        bool IsButtonPressed(Buttons button)
        {
            switch (button)
            {
                case Buttons.A:
                    return (currentState.Buttons.A == ButtonState.Pressed && lastState.Buttons.A == ButtonState.Released);
                case Buttons.B:
                    return (currentState.Buttons.B == ButtonState.Pressed && lastState.Buttons.B == ButtonState.Released);
                case Buttons.X:
                    return (currentState.Buttons.X == ButtonState.Pressed && lastState.Buttons.X == ButtonState.Released);
                case Buttons.Back:
                    return (currentState.Buttons.Back == ButtonState.Pressed && lastState.Buttons.Back == ButtonState.Released);
                case Buttons.DPadDown:
                    return (currentState.DPad.Down == ButtonState.Pressed && lastState.DPad.Down == ButtonState.Released);
                case Buttons.DPadUp:
                    return (currentState.DPad.Up == ButtonState.Pressed && lastState.DPad.Up == ButtonState.Released);
                case Buttons.Start:
                    return (currentState.Buttons.Start == ButtonState.Pressed && lastState.Buttons.Start == ButtonState.Released);
            }
            return false;
        }


        protected override void Draw(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.Title:
                    DrawTitle();
                    break;
                case GameState.Game:
                    DrawGame(gameTime);
                    break;
                case GameState.HiScores:
                    DrawHiScores();
                    break;
                default:
                    break;
            }

            spriteBatch.Begin();
            for (int i = 0; i < messages.Length; i++)
            {
                spriteBatch.DrawString(font, messages[i], scorePosition + new Vector2(0, 20 * i), Color.LightBlue);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawHiScores()
        {
            throw new NotImplementedException();
        }

        private void DrawTitle()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(stars, new Rectangle(0, 0, 800, 600), Color.White);
            if (connected)
            {
                spriteBatch.DrawString(font, "Press Start to begin", (new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2)), Color.White);
                spriteBatch.DrawString(font, gameState.ToString(), scorePosition, Color.Red);
            }
            else
                spriteBatch.DrawString(font, "Waiting for connection", scorePosition, Color.White);
            spriteBatch.End();
        }

        void DrawPlayer(Player player, Viewport viewport)
        {
            graphics.GraphicsDevice.Viewport = viewport;

            spriteBatch.Begin();
            spriteBatch.Draw(stars, new Rectangle(0, 0, 800, 600), Color.White);
            //spriteBatch.DrawString(font, "Score: " +player.score, scorePosition, Color.LightGreen);
            spriteBatch.End();
            if (player.ship.isActive)
            {
                Matrix shipTransformMatrix = player.ship.RotationMatrix * Matrix.CreateTranslation(player.ship.Position);
                DrawModel(shipModel, shipTransformMatrix, shipTransforms); 
            }

            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (player.asteroidList[i].isActive)
                {
                    Matrix asteroidTransform = Matrix.CreateTranslation(player.asteroidList[i].position);
                    DrawModel(asteroidModel, asteroidTransform, asteroidTransforms); 
                }
            }

            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                if (player.bulletList[i].isActive)
                {
                    Matrix bulletTransform = Matrix.CreateTranslation(player.bulletList[i].position);
                    DrawModel(bulletModel, bulletTransform, bulletTransforms);
                }
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Score: " + player.score, scorePosition, Color.LightGreen);
            spriteBatch.End();
        }

        private void DrawGame(GameTime gameTime)
        {
            DrawPlayer(player, leftViewport);
        }

        public static void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                }
                mesh.Draw();
            }
            
        }
    }
}
