using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Advertising.Mobile.Xna;

namespace XtremePaddle
{
    /// <summary>
    /// Clase principal del juego que carga los menus del propio juego
    /// Inicializa los componentes basicos y los carga en el ScreenManager.
    /// </summary>
    public class GameStateManagementGame : Microsoft.Xna.Framework.Game
    {
        #region Variables

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        // Datos para la publicidad
        private static readonly string ApplicationId = "432abda9-c080-4fd6-9dfe-978fc5e67ae5";

        // Añadimos el sistema de aros que explosionan y sus datos
        const float TimeBetweenAros = 2.0f;
        float timeTillAro = 0.0f;

        #endregion

        #region Propiedades

        // Sistema de particulas de aros
        public static ParticleSystem Aros
        {
            get { return aros; }
        }

        private static ParticleSystem aros;

        // Cargamos las opciones del juego
        public static Settings Settings
        {
            get { return settings; }
        }

        private static Settings settings = new Settings();

        // Cargamos el manejador de musica y sonidos
        public static MusicManager MusicManager
        {
            get { return musicManager; }
        }

        private static MusicManager musicManager;

        #endregion

        #region Inicializacion

        /// <summary>
        ///Constructor.
        /// </summary>
        public GameStateManagementGame()
        {
            Content.RootDirectory = "Content";

            // Cargamos toda la configuración guardada
            settings.LoadAll();

            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Elegimos jugar con la pantalla horizontal
            InitializeLandscapeGraphics();

            // Creamos el manejador de pantallas 
            // y lo añadimos a componentes
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);


            // intentamos deserializar el screen manager del disco. Si
            // esto falla, añadimos nuestras pantallas propias.
            if (!screenManager.DeserializeState())
            {
                // Activamos nuestras pantallas propias.
                screenManager.AddScreen(new BackgroundScreen("background"));
                screenManager.AddScreen(new MainMenuScreen());
            }

            // Creamos nuestro manejador de musica y lo añadimos como componente.
            musicManager = new MusicManager(this);
            musicManager.PreguntaGameHasControl += MusicManagerPreguntaGameHasControl;
            musicManager.FalloReproduccion += MusicManagerFalloReproduccion;
            Components.Add(musicManager);

            // Initialize the AdGameComponent with your ApplicationId and add it to the game.
            AdGameComponent.Initialize(this, ApplicationId);
            Components.Add(AdGameComponent.Current);
            screenManager.AdGameComponent = AdGameComponent.Current;

            aros = new ParticleSystem(this, "ArosSettings") { DrawOrder = ParticleSystem.AdditiveDrawOrder };
            Components.Add(aros);
        }

        /// <summary>
        /// Serializamos el screen manager cuando salimos del juego.
        /// </summary>
        protected override void OnExiting(object sender, System.EventArgs args)
        {
            screenManager.SerializeState();

            base.OnExiting(sender, args);
        }

        /// <summary>
        /// Ayudante que inicializa el juego en modo panoramico.
        /// </summary>
        private void InitializeLandscapeGraphics()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
        }

        #endregion

        #region Update y Draw

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateAros(dt);

            // the base update will handle updating the particle systems themselves,
            // because we added them to the components collection.
            base.Update(gameTime);
        }

        // this function is called when we want to demo the explosion effect. it
        // updates the timeTillExplosion timer, and starts another explosion effect
        // when the timer reaches zero.
        private void UpdateAros(float dt)
        {
            timeTillAro -= dt;
            if (timeTillAro < 0)
            {
                Vector2 where = Vector2.Zero;
                // create the explosion at some random point on the screen.
                where.X = ParticleHelpers.RandomBetween(0, graphics.GraphicsDevice.Viewport.Width);
                where.Y = ParticleHelpers.RandomBetween(0, graphics.GraphicsDevice.Viewport.Height);

                // the overall explosion effect is actually comprised of two particle
                // systems: the fiery bit, and the smoke behind it. add particles to
                // both of those systems.
                aros.AddParticles(where, Vector2.Zero);

                // reset the timer.
                timeTillAro = TimeBetweenAros;
            }
        }

        /// <summary>
        /// Metodo cuando el juego debe dibujarse a si mismo.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // El verdadero draw ocurre dentro del componente screen manager.
            base.Draw(gameTime);
        }

        #endregion

        #region Eventos Music Manager

        /// <summary>
        /// Invocado si el usuario esta escuchando musica cuando pedimos al musicManager que 
        /// reproduzca una cancion. Podemos responder preguntando al usuario si quiere apagar su musica
        /// y en tal caso se reproduzca nuestra musica.
        /// </summary>
        private void MusicManagerPreguntaGameHasControl(object sender, EventArgs e)
        {
            // Mostrar un message box para ver si el usuario quiere apagar su propia musica.
            Guide.BeginShowMessageBox(
                "Use game music?",
                "Would you like to turn off your music to listen to the game's music?",
                new[] { "Yes", "No" },
                0, MessageBoxIcon.Alert,
                result =>
                {
                    // Obtenemos la eleccion del resultado
                    int? choice = Guide.EndShowMessageBox(result);

                    // Si el usuario dice SI, paramos el mediaplayer. Nuestro musicManager deberia
                    // ver que tenemos una cancion que queremos reproducir y que el juego ahora tiene
                    // el control para reproducir automaticamente esta cancion.
                    if (choice.HasValue && choice.Value == 0) MediaPlayer.Stop();
                },
                null);
        }

        /// <summary>
        /// Invocado si la reproduccion de musica falla. El caso mas comun es que el telefono
        /// este conectado al pc con Zune abierto, o en modo depuracion. La mayoria de los juegos
        /// suelen ignorar este evento, pero podemos decirle al usuario para que sepa el porque, 
        /// da este error.
        /// </summary>
        private void MusicManagerFalloReproduccion(object sender, EventArgs e)
        {
            // Mostramos un mensaje que avisa al usuario de que l musica no se reproduce.
            Guide.BeginShowMessageBox(
                "Music playback failed",
                "Music playback cannot begin if the phone is connected to a PC running Zune.",
                new[] { "OK" },
                0,
                MessageBoxIcon.Error,
                null,
                null);
        }

        #endregion

    }

}
