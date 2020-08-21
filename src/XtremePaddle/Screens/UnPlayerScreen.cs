using System;
using System.Threading;
using Microsoft.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace XtremePaddle
{
    /// <summary>
    /// Esta pantalla implementa la logica actual para 1 jugador.
    /// Contiene todo el gameplay incorporando varias clases.
    /// </summary>
    class UnPlayerScreen : GameScreen
    {
        #region Variables

        ContentManager content;

        // Vibracion de 1seg.
        VibrateController vibration = VibrateController.Default;
        TimeSpan vibraTime = new TimeSpan(00, 00, 01);

        // Dificultad
        float difficulty;

        // Texturas y Fuentes
        Texture2D texBackground;
        SpriteFont gameFont;

        // Posiciones por defecto
        Vector2 posTopLeft;
        Vector2 posBottomRight;

        // Posiciones de los marcadores
        Vector2 posScoreJ1;
        Vector2 posScoreIA;

        // Rectangulos de Colision para 1 jugador
        Rectangle recZonaPlayer;

        // Sonidos
        SoundEffect soundPlink;
        SoundEffect soundScore;
        SoundEffect soundPause;
        SoundEffect soundWin;
        SoundEffect soundLose;
        SoundEffect soundPowerup;

        // Objetos Basicos de juego
        Paddle paletaJ1;
        Paddle paletaIA;
        Ball ball;

        //PowerUps y temporizador para cargarlos
        PowerUps[] powerUps = new PowerUps[11];
        float tempoPower = 10;

        // Puntos de inicio
        int scoreJ1 = 0;
        int scoreIA = 0;

        // Generador de numeros aleatorios
        Random random;

        #endregion

        #region Inicializacion


        /// <summary>
        /// Constructor.
        /// </summary>
        public UnPlayerScreen(float difficulty)
        {
            this.difficulty = difficulty;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            // TODO: ¿Cambiar por RemoveAd?
            //GameStateManagementGame.BannerAd.Visible = false;
        }

        /// <summary>
        /// Cargamos el contenido necesario para la partida.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Paramos el efecto de los aros
            GameStateManagementGame.Aros.Visible = false;

            // Paramos cualquier cancion que este sonando
            GameStateManagementGame.MusicManager.Stop();

            // Cargamos la nueva dependiendo de la dificultad y la reproducimos
            Song song = content.Load<Song>("songs/J1_" + difficulty);
            GameStateManagementGame.MusicManager.Play(song);

            // Creamos un random con semilla para que sea aleatorio
            int semilla = (int)DateTime.Now.Ticks;
            random = new Random(semilla);

            // Cargamos las paletas y bola para juego de 1 Jugador
            paletaJ1 = new Paddle(1, 0);    // 0 por poner algo, nos da igual.
            paletaIA = new Paddle(0, (int)difficulty);
            ball = new Ball(difficulty);

            // Carga de texturas y fuentes dependiendo de las opciones
            texBackground = content.Load<Texture2D>("themes/" + GameStateManagementGame.Settings.Theme + "/background");
            gameFont = content.Load<SpriteFont>("gameFont");

            // Cargamos el contenido de los objetos
            ball.LoadContent(content);
            paletaJ1.LoadContent(content);
            paletaIA.LoadContent(content);

            //Cargamos los PowerUps y su contenido
            powerUps[0] = new PowerUps(1, 0);
            powerUps[1] = new PowerUps(2, 0);
            powerUps[2] = new PowerUps(3, 0);
            powerUps[3] = new PowerUps(4, 1);
            powerUps[4] = new PowerUps(4, 2);
            powerUps[5] = new PowerUps(5, 1);
            powerUps[6] = new PowerUps(5, 2);
            powerUps[7] = new PowerUps(6, 1);
            powerUps[8] = new PowerUps(6, 2);
            powerUps[9] = new PowerUps(7, 1);
            powerUps[10] = new PowerUps(7, 2);
            foreach (PowerUps i in powerUps)
            {
                i.LoadContent(content);
                i.Descolocar();
            }

            // Colocamos la bola y las paletas inicialmente.
            ball.Colocar();
            paletaIA.Colocar();
            paletaJ1.Colocar();

            // Inicializamos los sonidos
            soundPlink = content.Load<SoundEffect>("sounds/plink");
            soundScore = content.Load<SoundEffect>("sounds/score");
            soundPause = content.Load<SoundEffect>("sounds/pause");
            soundWin = content.Load<SoundEffect>("sounds/win");
            soundLose = content.Load<SoundEffect>("sounds/lose");
            soundPowerup = content.Load<SoundEffect>("sounds/powerup");

            //Coordenadas mínima y máxima de la pantalla
            posTopLeft = new Vector2(0, 0);
            posBottomRight = new Vector2(ScreenManager.GraphicsDevice.DisplayMode.Height, ScreenManager.GraphicsDevice.DisplayMode.Width);

            // Colocamos los marcadores.
            Vector2 tamFuente = gameFont.MeasureString("0");
            posScoreIA = new Vector2(posBottomRight.X / 2 - 20 - tamFuente.X, posTopLeft.Y + 10);
            posScoreJ1 = new Vector2(posBottomRight.X / 2 + 20, posTopLeft.Y + 10);

            //Coordenadas rectangulos fijos de click
            recZonaPlayer = new Rectangle((int)posBottomRight.X / 2, 0, (int)posBottomRight.X, (int)posBottomRight.Y);


            // Simulamos la carga creando un Thread que duerme 1 segundo para que
            // de tiempo a cargar todo el contenido del juego.
            Thread.Sleep(1000);

            // Despues de que carge usamos ResetElapsedTime(), para que
            // el load nos pase al juego real y podamos jugar.
            ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Descargamos graficos que no se usen en el juego.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update y Draw

        /// <summary>
        /// Actualizamos el estado del juego. Este metodo está conexionado a screen,
        /// por tanto se "pausara" el juego si se pone por encima una pantalla,
        /// aunque solo si es popup, sino se cerrara.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                // Calculamos movimiento de la bola
                ball.Update(gameTime.ElapsedGameTime);

                // Si somos paleta IA vamos por la ball. 
                if (paletaIA.congelado == false)
                    paletaIA.UpdateAI(gameTime.ElapsedGameTime, ball);

                // Obtenemos los toques en la pantalla
                TouchCollection touchs = TouchPanel.GetState();

                if (touchs.Count > 0)
                {
                    // Solo necesitamos usar uno de los gestos
                    TouchLocation touch = touchs[0];

                    Point ClickPlayer = new Point((int)touch.Position.X, (int)touch.Position.Y);

                    // Movemos al jugador si se encuentra en su posicion.
                    if (recZonaPlayer.Contains(ClickPlayer) && paletaJ1.congelado == false)
                    {
                        // Centramos la paleta horizontalmente a donde toque el usuario
                        paletaJ1.Position.Y = touch.Position.Y - paletaJ1.Bounds.Height / 2;
                        paletaJ1.ClampToScreen();
                    }
                }

                // Hacemos que la bola se quede dentro del campo.
                ball.ClampToScreen();

                // Comprobamos las colisiones con las paletas, basandonos en la dirección de la bola.
                if (ball.Velocidad.X > 0)
                {
                    if (paletaJ1.Collide(ball)) GameStateManagementGame.MusicManager.Play(soundPlink);
                }
                else if (ball.Velocidad.X < 0)
                {
                    if (paletaIA.Collide(ball)) GameStateManagementGame.MusicManager.Play(soundPlink);
                }

                // Comprobamos las colisiones de la bola (con las porterias)
                int fueraBola = ball.IsOffscreen();

                if (fueraBola == 1)
                {               // Si marca la IA
                    GameStateManagementGame.MusicManager.Play(soundScore);
                    if (GameStateManagementGame.Settings.Vibration) vibration.Start(vibraTime);
                    scoreIA++;
                    ball.Reset();
                    ball.Colocar();
                    paletaJ1.Reset();
                    paletaIA.Reset();
                }
                else if (fueraBola == -1)
                {       // Si marca el J1
                    GameStateManagementGame.MusicManager.Play(soundScore);
                    if (GameStateManagementGame.Settings.Vibration) vibration.Start(vibraTime);
                    scoreJ1++;
                    ball.Reset();
                    ball.Colocar();
                    paletaJ1.Reset();
                    paletaIA.Reset();
                }

                // Si tenemos activado el modo xtremo jugamos con powerups
                if (GameStateManagementGame.Settings.Xtreme)
                {
                    if (tempoPower > 0)
                    {
                        tempoPower -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (tempoPower < 0)
                        {
                            int elegido = random.Next(0, powerUps.Length);
                            powerUps[elegido].Colocar();
                            tempoPower = random.Next(10, 15);
                        }
                    }

                    foreach (PowerUps i in powerUps)
                    {
                        i.Update();
                        if (i.Collide(ball, paletaJ1, paletaIA, gameTime.ElapsedGameTime))
                        {
                            i.Descolocar();
                            GameStateManagementGame.MusicManager.Play(soundPowerup);
                        }
                    }
                }

                // Si el jugador o la IA llega a 10 puntos paramos el juego
                // y preguntamos al usuario si quiere reiniciar o salir.
                if (scoreJ1 == 10)
                {
                    GameStateManagementGame.MusicManager.Stop();
                    GameStateManagementGame.MusicManager.Play(soundWin);
                    ScreenManager.AddScreen(new WinnerMenuScreen(true, (int)difficulty));
                }
                else if (scoreIA == 10)
                {
                    GameStateManagementGame.MusicManager.Stop();
                    GameStateManagementGame.MusicManager.Play(soundLose);
                    ScreenManager.AddScreen(new WinnerMenuScreen(false, (int)difficulty));
                }
            }
        }

        /// <summary>
        /// Dejamos al juego que controle la entrada de teclas. A diferencia
        /// del metodo update, este solo actua cuando la pantalla, esta activa.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            // Si el usuario presiona el boton atras, cargamos el menu pausa
            if (input.IsMenuCancel())
            {
                GameStateManagementGame.MusicManager.Play(soundPause);
                ScreenManager.AddScreen(new PauseMenuScreen());
            }
        }


        /// <summary>
        /// Dibujamos la pantalla de juego.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Comenzamos el batch de dibujado
            spriteBatch.Begin();

            // Dibujamos el fondo
            spriteBatch.Draw(texBackground, Vector2.Zero, Color.White);

            // Dibujamos el resto de elementos
            ball.Draw(spriteBatch);
            paletaJ1.Draw(spriteBatch);
            paletaIA.Draw(spriteBatch);

            // Dibujamos todos los powerups aunque no se usen
            if (GameStateManagementGame.Settings.Xtreme)
            {
                foreach (PowerUps i in powerUps)
                {
                    i.Draw(spriteBatch);
                }
            }

            // Dibujamos los soundScores con sus sombras
            spriteBatch.DrawString(gameFont, "" + (int)scoreJ1, new Vector2(posScoreJ1.X + 2, posScoreJ1.Y + 2), Color.Black);
            spriteBatch.DrawString(gameFont, "" + (int)scoreJ1, posScoreJ1, Color.White);
            spriteBatch.DrawString(gameFont, "" + (int)scoreIA, new Vector2(posScoreIA.X + 2, posScoreIA.Y + 2), Color.Black);
            spriteBatch.DrawString(gameFont, "" + (int)scoreIA, posScoreIA, Color.White);

            // Finalizamos el batch dando la orden de dibujarlo.
            spriteBatch.End();

            // Si la pantalla esta transicionando lo dibujamos.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
        }

        #endregion
    }
}
