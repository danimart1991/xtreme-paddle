using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace XtremePaddle {
    /// <summary>
    /// Componente de juego que puede ser usado para reproducir musica en background,
    /// asegurandonos que la propiedad GameHasControl esta siendo respetada y que la,
    /// musica sigue reproduciendose despues de cada video.
    /// </summary>
    /// <remarks>
    /// El MusicManager tiene dos responsabilidades primarias:
    /// 
    /// 1)  Si el juego desea reproducir una cancion, el MusicManager debe monitorizar
    ///     el MediaPlayer.GameHasControl para asegurarse de que el juego tiene permisos
    ///     para reproducir musica, sin sobrescribir la musica del usuario.
    ///     
    /// 2)  Si el juego está pausado por alguna razón, (ej. los cascos se han desconectado)
    ///     el MusicManager debe seguir reproduciendo automaticamente.
    ///     
    /// El MusicManager ayuda no solo con los Requisitos de Windows Phone Certification sobre
    /// el uso de GameHasControl, sino que hace más sencillo el reanudar media despues
    /// de ciertos eventos, como ver videos o desconectar cascos, los cuales paran la musica
    /// sin reanudacion posterior.
    /// </remarks>
    public class MusicManager : GameComponent {
        #region Variables

        // No queremos estar sondeando consntantemente, asi que elegimos un tiempo (en segundos)
        // que indica cada cuanto debemos sondear. Por defecto 1 seg.
        const float EsperaSondeo = 1f;

        // Un float simple para nuestro timer de sondeo.
        private float gameHasControlTimer;

        // Variable que nos indica si tenemos el control sobre la musica o no.
        private bool gameHasControl = false;

        // La cancion que se va a reproducir.
        private Song currentSong;

        #endregion

        #region Atributos

        /// <summary>
        /// Obtiene si se esta reproduciendo musica en el juego actualmente.
        /// </summary>
        public bool ReproduciendoMusica { get; private set; }

        /// <summary>
        /// Invocado si el juego intenta reproducir una cancion y el juego no tiene el control.
        /// Esto da al juego la oportunidad de preguntar al jugador si desea apagar su propia musica.
        /// </summary>
        public event EventHandler<EventArgs> PreguntaGameHasControl;

        /// <summary>
        /// Invocado si la reproduccion de la cancion falla, para preguntar al usuario o 
        /// responder si es necesario.
        /// </summary>
        public event EventHandler<EventArgs> FalloReproduccion;

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="game">El juego que va a usar el MusicManager.</param>
        public MusicManager(Game game)
            : base(game) {

            // Cogemos el valor incial de GameHasControl
            gameHasControl = MediaPlayer.GameHasControl;

            // Activamos el evento de juego activado para poder responder, si el juego
            // pasa a estado background o si se reproduce un video.
            game.Activated += game_Activated;
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Manejador de Eventos que es invocado cuadno el juego es activado
        /// desde background o en cualquier otro momento.
        /// </summary>
        void game_Activated(object sender, EventArgs e) {
            //Miramos si tenemos el control de la musica
            gameHasControl = MediaPlayer.GameHasControl;

            // Si tenemos el control, y queremos reproducir algo, y el mediaPlayer, no esta
            // reproduciendo, reproducimos. Esto ocurre cuando volvemos de una desactivacion con
            // ciertos lanzadores (principalmente el MediaPlayerLauncher) el cual no reproduce
            // automaticamente la cancion que queremos. Podemos detectarlo y reproducir la cancion
            // nosotros mismos, para que el usuario no se quede sin musica en el juego.
            if (gameHasControl && currentSong != null && MediaPlayer.State != MediaState.Playing)
                PlaySongSafe();
        }

        /// <summary>
        /// Reproducimos la cancion indicada como musica de fondo.
        /// </summary>
        /// <param name="song">La cancion a reproducir.</param>
        public void Play(Song song) {
            // Almacenamos son en nuestra variable global.
            currentSong = song;

            // Si tenemos el control, reproducimos la cancion inmediatamente.
            if (gameHasControl)
                PlaySongSafe();

            // En otro caso, invocamos nuestro evento para que el juego pueda chekear, si
            // el jugador quiere parar su musica para reproducir la nuestra.
            else if (PreguntaGameHasControl != null)
                PreguntaGameHasControl(this, EventArgs.Empty);
        }

        /// <summary>
        /// Reproducimos un sonido si es posible.
        /// </summary>
        /// <param name="sound">El sonido a reproducir.</param>
        public void Play(SoundEffect sound) {
            // Comprobamos si se puede reproducir sonidos.
            if (GameStateManagementGame.Settings.Sound) sound.Play();
        }

        /// <summary>
        /// Dejamos de reproducir musica de fondo.
        /// </summary>
        public void Stop() {
            // Null a la variable global
            currentSong = null;

            // Paramos la reproduccion.
            if (gameHasControl)
                MediaPlayer.Stop();
        }

        /// <summary>
        /// Permitimos al componente que maneje su propia logica de actualizacion.
        /// </summary>
        /// <param name="gameTime">Captura de los valores temporales</param>
        public override void Update(GameTime gameTime) {
            // Actualiazmos nuestro timer
            gameHasControlTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Si pasamos nuestro temporizador de sondeo, manejamos el update
            if (gameHasControlTimer >= EsperaSondeo) {
                // Reiniciamos el temporizador a cero
                gameHasControlTimer = 0f;

                // Comprobamos si tenemos control del MediaPlayer
                gameHasControl = MediaPlayer.GameHasControl;

                // Recogemos el estado actual y la cancion del MediaPlayer
                MediaState StateActual = MediaPlayer.State;
                Song songActiva = MediaPlayer.Queue.ActiveSong;

                // Si tenemos el control de la musica
                if (gameHasControl) {
                    // Si tenemos una cancion que reproducir
                    if (currentSong != null) {
                        // Si MediaPlayer no reproduce nada
                        if (StateActual != MediaState.Playing) {
                            // Si la cancion esta pausada, por ejemplo, debido a
                            // quitar cascos, llamamos a Resume() para continuar
                            if (StateActual == MediaState.Paused)
                                ResumeSongSafe();
                            // Sino reproducimos nuestra cancion.
                            else
                                PlaySongSafe();
                        }
                    } else {
                        // Si no tenemos una cancion que reproducir, queremos asegurarnos de que se 
                        // para cualquier musica que estaba sonando previamente.
                        if (StateActual != MediaState.Stopped) MediaPlayer.Stop();
                    }
                }
                // Guardamos un valor indicando si la musica esta sonando o no.
                ReproduciendoMusica = (StateActual == MediaState.Playing) && gameHasControl;
            }
        }

        /// <summary>
        /// Metodo que envuelve MediaPlayer.Play para manejar excepciones
        /// </summary>
        private void PlaySongSafe() {
            // Nos aseguramos de tener una cancion que reproducir
            if (currentSong == null) return;

            try {
                MediaPlayer.Play(currentSong);
            } catch (InvalidOperationException) {
                // La reproduccion de Media puede fallas si Zune esta conectado. No queremos
                // que el juego pete, asique, capturamos excepciones.

                // Anulamos la cancion para que no se quede intentando reproducirse. Esto causaria
                // que el jeugo se ralentizase y se petase.
                currentSong = null;

                // Invocamos nuestro evento FalloReproduccion en el caso de que el juego quiera
                // manejar este escenario a su manera.
                if (FalloReproduccion != null) FalloReproduccion(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Metodo que envuelve MediaPlayer.Resume para manejar excepciones
        /// </summary>
        private void ResumeSongSafe() {
            try {
                MediaPlayer.Resume();
            } catch (InvalidOperationException) {
                // La reproduccion de Media puede fallas si Zune esta conectado. No queremos
                // que el juego pete, asique, capturamos excepciones.

                // Anulamos la cancion para que no se quede intentando reproducirse. Esto causaria
                // que el jeugo se ralentizase y se petase.
                currentSong = null;

                // Invocamos nuestro evento FalloReproduccion en el caso de que el juego quiera
                // manejar este escenario a su manera.
                if (FalloReproduccion != null) FalloReproduccion(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
