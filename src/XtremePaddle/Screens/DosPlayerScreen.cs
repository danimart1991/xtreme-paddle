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
    /// Esta pantalla implementa la logica actual para 2 jugadores.
    /// Contiene todo el gameplay incorporando varias clases.
    /// </summary>
    class DosPlayerScreen : GameScreen
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
        Vector2 posScoreJ2;

        // Rectangulos de Colision para 1 jugador
        Rectangle recZonaPlayer;

        // Sonidos
        SoundEffect soundPlink;
        SoundEffect soundScore;
        SoundEffect soundPause;
        SoundEffect soundPowerup;
        SoundEffect soundWin;

        // Objetos Basicos de juego
        Paddle paletaJ1;
        Paddle paletaJ2;
        Ball ball;

        //PowerUps y temporizador para cargarlos
        PowerUps[] powerUps = new PowerUps[11];
        float tempoPower = 10;

        // Puntos de inicio
        int scoreJ1 = 0;
        int scoreJ2 = 0;

        // Generador de numeros aleatorios
        Random random;

        #endregion

        #region Inicializacion


        /// <summary>
        /// Constructor.
        /// </summary>
        public DosPlayerScreen(int difficulty)
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
            Song song = content.Load<Song>("songs/J2");
            GameStateManagementGame.MusicManager.Play(song);

            // Creamos un random con semilla para que sea aleatorio
            int semilla = (int)DateTime.Now.Ticks;
            random = new Random(semilla);

            // Cargamos las paletas y bola para juego de 2 Jugadores
            paletaJ1 = new Paddle(1, 0);    // 0 por poner algo, nos da igual.
            paletaJ2 = new Paddle(2, 0);    // 0 por poner algo, nos da igual.
            ball = new Ball(difficulty);

            // Carga de texturas y fuentes dependiendo de las opciones
            texBackground = content.Load<Texture2D>("themes/" + GameStateManagementGame.Settings.Theme + "/background");
            gameFont = content.Load<SpriteFont>("gameFont");

            // Cargamos el contenido de los objetos
            ball.LoadContent(content);
            paletaJ1.LoadContent(content);
            paletaJ2.LoadContent(content);

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
            paletaJ2.Colocar();
            paletaJ1.Colocar();

            // Inicializamos los sonidos
            soundPlink = content.Load<SoundEffect>("sounds/plink");
            soundScore = content.Load<SoundEffect>("sounds/score");
            soundPause = content.Load<SoundEffect>("sounds/pause");
            soundPowerup = content.Load<SoundEffect>("sounds/powerup");
            soundWin = content.Load<SoundEffect>("sounds/win");

            //Coordenadas mínima y máxima de la pantalla
            posTopLeft = new Vector2(0, 0);
            posBottomRight = new Vector2(ScreenManager.GraphicsDevice.DisplayMode.Height, ScreenManager.GraphicsDevice.DisplayMode.Width);

            // Colocamos los marcadores.
            Vector2 tamFuente = gameFont.MeasureString("0");
            posScoreJ2 = new Vector2(posBottomRight.X / 2 - 20 - tamFuente.X, posTopLeft.Y + 10);
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

                // Obtenemos los toques en la pantalla
                TouchCollection touchs = TouchPanel.GetState();

                for (int i = 0; i < touchs.Count; i++)
                {
                    // Solo necesitamos usar uno de los gestos
                    TouchLocation touch = touchs[i];

                    Point ClickPlayer = new Point((int)touch.Position.X, (int)touch.Position.Y);

                    // Movemos al jugador si se encuentra en su posicion.
                    if (recZonaPlayer.Contains(ClickPlayer) && paletaJ1.congelado == false)
                    {
                        // Centramos la paleta horizontalmente a donde toque el usuario
                        paletaJ1.Position.Y = touch.Position.Y - paletaJ1.Bounds.Height / 2;
                        paletaJ1.ClampToScreen();
                    }
                    else if (!recZonaPlayer.Contains(ClickPlayer) && paletaJ2.congelado == false)
                    {    //Sino al J2
                        // Centramos la paleta horizontalmente a donde toque el usuario
                        paletaJ2.Position.Y = touch.Position.Y - paletaJ2.Bounds.Height / 2;
                        paletaJ2.ClampToScreen();
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
                    if (paletaJ2.Collide(ball)) GameStateManagementGame.MusicManager.Play(soundPlink);
                }

                // Comprobamos las colisiones de la bola (con las porterias)
                int fueraBola = ball.IsOffscreen();

                if (fueraBola == 1)
                {           // Si marca el J2
                    GameStateManagementGame.MusicManager.Play(soundScore);
                    if (GameStateManagementGame.Settings.Vibration) vibration.Start(vibraTime);
                    scoreJ2++;
                    ball.Reset();
                    ball.Colocar();
                    paletaJ1.Reset();
                    paletaJ2.Reset();
                }
                else if (fueraBola == -1)
                {   // Si marca el J1
                    GameStateManagementGame.MusicManager.Play(soundScore);
                    if (GameStateManagementGame.Settings.Vibration) vibration.Start(vibraTime);
                    scoreJ1++;
                    ball.Reset();
                    ball.Colocar();
                    paletaJ1.Reset();
                    paletaJ2.Reset();
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
                        if (i.Collide(ball, paletaJ1, paletaJ2, gameTime.ElapsedGameTime))
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
                    ScreenManager.AddScreen(new WinnerMenuScreen(true, 0));
                }
                else if (scoreJ2 == 10)
                {
                    GameStateManagementGame.MusicManager.Stop();
                    GameStateManagementGame.MusicManager.Play(soundWin);
                    ScreenManager.AddScreen(new WinnerMenuScreen(false, 0));
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
            paletaJ2.Draw(spriteBatch);

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
            spriteBatch.DrawString(gameFont, "" + (int)scoreJ2, new Vector2(posScoreJ2.X + 2, posScoreJ2.Y + 2), Color.Black);
            spriteBatch.DrawString(gameFont, "" + (int)scoreJ2, posScoreJ2, Color.White);

            // Finalizamos el batch dando la orden de dibujarlo.
            spriteBatch.End();

            // Si la pantalla esta transicionando lo dibujamos.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
        }

        #endregion
    }
}
