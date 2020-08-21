using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Advertising.Mobile.Xna;
using System.Device.Location;

namespace XtremePaddle {
    /// <summary>
    /// ScreenManager es un componente que controla una o mas instancias de
    /// GameScreen. Mantiene una pila de pantallas, llama a sus metodos Update 
    /// y Draw en el tiempo oportuno, y automaticamente enruta la entrada a la
    /// pantalla activa más alta.
    /// </summary>
    public class ScreenManager : DrawableGameComponent {
        #region Variables

        ContentManager content;

        // Lista de pantallas y pantallas a actualizar
        List<GameScreen> screens = new List<GameScreen>();
        List<GameScreen> screensToUpdate = new List<GameScreen>();

        InputState input = new InputState();

        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D blankTexture;
        Texture2D splashTexture;

        Song songMenu;

        bool isInitialized;

        #endregion

        #region propiedades

        private DrawableAd bannerAd;

        public DrawableAd BannerAd
        {
            get { return bannerAd; }
            set { bannerAd = value; }
        }

        private AdGameComponent adGameComponent;

        public AdGameComponent AdGameComponent
        {
            get { return adGameComponent; }
            set { adGameComponent = value; }
        }

        private static readonly string AdUnitId = "94647";
        private GeoCoordinateWatcher gcw = null;

        #endregion

        #region atributos

        /// <summary>
        /// SpriteBatch predeterminado, compartido por todas las pantallas.
        /// Esto evita que cada pantalla cree su propia instancia local.
        /// </summary>
        public SpriteBatch SpriteBatch {
            get { return spriteBatch; }
        }

        /// <summary>
        /// Fuente predeterminada, compartida por todas las pantallas.
        /// Esto evita que cada pantalla cree su propia copia local.
        /// </summary>
        public SpriteFont Font {
            get { return font; }
        }

        /// <summary>
        /// Content predeterminado, compartido por todas las pantallas.
        /// Esto evita que cada pantalla cree su propia instancia local.
        /// </summary>
        public ContentManager Content {
            get { return content; }
        }

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        public ScreenManager(Game game)
            : base(game) {
            // debemos activar EnabledGestures antes de que podamos
            // usarlo, pero no asumimos que el juego quiera leerlo.
            TouchPanel.EnabledGestures = GestureType.None;
        }

        /// <summary>
        /// Inicializamos el componente screen manager.
        /// </summary>
        public override void Initialize() {
            base.Initialize();

            isInitialized = true;
        }

        /// <summary>
        /// Carga el contenido necesario de screen manager.
        /// </summary>
        protected override void LoadContent() {
            content = Game.Content;

            // Cargamos el resto de contenido.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("menufont");
            blankTexture = content.Load<Texture2D>("blank");

            // Cargamos el Splash Screen de inicio.
            splashTexture = content.Load<Texture2D>("SplashScreenImage");
            spriteBatch.Begin();
            spriteBatch.Draw(splashTexture, new Vector2(0, 0), Color.White);
            spriteBatch.End();
            GraphicsDevice.Present();

            // Cargamos la cancion del menu y la reproducimos
            songMenu = content.Load<Song>("songs/menu");
            GameStateManagementGame.MusicManager.Play(songMenu);

            // Pedimos a cada pantalla que cargue su contenido.
            // Por si por algún motivo hemos sobreescrito.
            foreach (GameScreen screen in screens) {
                screen.LoadContent();
            }
        }

        /// <summary>
        /// Descarga los contenidos cargados.
        /// </summary>
        protected override void UnloadContent() {
            // Pedimos a cada pantalla que descargue su contenido.
            // Por si por algún motivo hemos sobreescrito.
            foreach (GameScreen screen in screens) {
                screen.UnloadContent();
            }
        }

        #endregion

        #region Update y Draw

        /// <summary>
        /// Actualizamos cada Pantalla y su logica.
        /// </summary>
        public override void Update(GameTime gameTime) {
            // Leemos los clicks o del teclado.
            input.Update();

            // Hacemos una copia de la lista maestra de pantallas, para evitar confusiones
            // si el proceso de actualizar una pantalla añade o quita otras.
            screensToUpdate.Clear();

            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Repetir mientras haya pantallas a la espera de ser actualizadas.
            while (screensToUpdate.Count > 0) {
                // Lanzá la pantalla más alta fuera de la lista de espera.
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Actualizamos la pantalla.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active) {
                    // Si esta es la primera pantalla activa que nos encontramos
                    // le damos la oportunidad de usar la entrada de gestos.
                    if (!otherScreenHasFocus) {
                        screen.HandleInput(input);

                        otherScreenHasFocus = true;
                    }

                    // En el caso de que no sea un Popup, informamos a las
                    // pantallas siguientes de que estan cubiertas por esta.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
        }

        /// <summary>
        /// Pedimos a cada pantalla que se dibuje a si misma.
        /// </summary>
        public override void Draw(GameTime gameTime) {
            foreach (GameScreen screen in screens) {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Añade una nueva pantalla al screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen) {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // Pedimos a la pantalla que cargue su contenido.
            if (isInitialized) {
                screen.LoadContent();
            }

            screens.Add(screen);

            // Actualizamos el TouchPanel para capturar los gestos de esta pantalla.
            TouchPanel.EnabledGestures = screen.EnabledGestures;
        }

        /// <summary>
        /// Quita una pantalla del screen manager. Normalmente deberia usarse
        /// GameScreen.ExitScreen en vez de esta llamada directamente, para que
        /// la pantalla pueda gradualmente apagarse, y no quitarse instantaneamente.
        /// </summary>
        public void RemoveScreen(GameScreen screen) {
            // Pedimos a la pantalla que descargue el contenido.
            if (isInitialized) {
                screen.UnloadContent();
            }

            screens.Remove(screen);
            screensToUpdate.Remove(screen);

            // si todavia hay alguna pantalla en el manager, actualizamos el
            // TouchPanel para responder a los gestos de la pantalla siguiente.
            if (screens.Count > 0) {
                TouchPanel.EnabledGestures = screens[screens.Count - 1].EnabledGestures;
            }
        }

        /// <summary>
        /// Mostramos un array de todas las pantallas. Devuelve una copia de
        /// la lista maestra real, debido a que las pantallas solo deben añadirse
        /// o removerse usando los metodos AddScreen y RemoveScreen.
        /// </summary>
        public GameScreen[] GetScreens() {
            return screens.ToArray();
        }

        /// <summary>
        /// Ayudante que dibuja un sprite fullscreen traslucido negro, usado para
        /// difuminar al introducir y sacar pantallas, y para oscurecer los fondos
        /// a la hora de usar popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha) {
            Viewport viewport = GraphicsDevice.Viewport;

            spriteBatch.Begin();

            spriteBatch.Draw(blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black * alpha);

            spriteBatch.End();
        }

        /// <summary>
        /// Informamos al screen manager que guarde su estado al disco.
        /// </summary>
        public void SerializeState() {
            // abrimos el isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication()) {
                // si nuestro directorio para el screen manager ya existe, borramos su contenido
                if (storage.DirectoryExists("ScreenManager")) {
                    DeleteState(storage);
                }

                // sino, creamos el directorio
                else {
                    storage.CreateDirectory("ScreenManager");
                }

                // creamos un fichero que usaremos para almacenar la lista de pantallas como una pila
                using (IsolatedStorageFileStream stream = storage.CreateFile("ScreenManager\\ScreenList.dat")) {
                    using (BinaryWriter writer = new BinaryWriter(stream)) {
                        // escribimos el nombre completo de todos los tipos en nuestra pila,
                        // para poder recrearlos si lo necesitamos.
                        foreach (GameScreen screen in screens) {
                            if (screen.IsSerializable) {
                                writer.Write(screen.GetType().AssemblyQualifiedName);
                            }
                        }
                    }
                }

                // ahora creamos un nuevo archivo para cada pantalla, donde podamos grabar su estado
                // si lo necesitamos. nombramos a este archivo "ScreenX.dat" donde X es el indice
                // la pantalla en la pila, para asegurarnos de que los archivos tienen nombre unico
                int screenIndex = 0;
                foreach (GameScreen screen in screens) {
                    if (screen.IsSerializable) {
                        string fileName = string.Format("ScreenManager\\Screen{0}.dat", screenIndex);

                        // abrimos el stream y dejamos a la pantalla que serialize su estado si quiere
                        using (IsolatedStorageFileStream stream = storage.CreateFile(fileName)) {
                            screen.Serialize(stream);
                        }

                        screenIndex++;
                    }
                }
            }
        }

        public bool DeserializeState() {
            // abrimos el isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication()) {
                // comprobamos si nuestra carpeta de guardado existe
                if (storage.DirectoryExists("ScreenManager")) {
                    try {
                        // comprobamos si tenemos pantallas
                        if (storage.FileExists("ScreenManager\\ScreenList.dat")) {
                            // cargamos la lista de pantallas
                            using (IsolatedStorageFileStream stream = storage.OpenFile("ScreenManager\\ScreenList.dat", FileMode.Open, FileAccess.Read)) {
                                using (BinaryReader reader = new BinaryReader(stream)) {
                                    while (reader.BaseStream.Position < reader.BaseStream.Length) {
                                        // leemos una linea de nuestro archivo
                                        string line = reader.ReadString();

                                        // si no está en blanco, creamos una pantalla para el.
                                        if (!string.IsNullOrEmpty(line)) {
                                            Type screenType = Type.GetType(line);
                                            GameScreen screen = Activator.CreateInstance(screenType) as GameScreen;
                                            AddScreen(screen);
                                        }
                                    }
                                }
                            }
                        }

                        // despues damos la opcion a cada pantalla para que desesialice del disco
                        for (int i = 0; i < screens.Count; i++) {
                            string filename = string.Format("ScreenManager\\Screen{0}.dat", i);
                            using (IsolatedStorageFileStream stream = storage.OpenFile(filename, FileMode.Open, FileAccess.Read)) {
                                screens[i].Deserialize(stream);
                            }
                        }

                        return true;
                    } catch (Exception) {
                        // si alguna excepcion ocurre mientras leemos, posiblemente no podamos recuperarlo
                        // del estado guardado, asi que lo borramos para que el juego pueda cargarse correctamente.
                        DeleteState(storage);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Borramos el archivo del isolated storage.
        /// </summary>
        private void DeleteState(IsolatedStorageFile storage) {
            // cogemos todos los archivos de la carpeta y los borramos
            string[] files = storage.GetFileNames("ScreenManager\\*");
            foreach (string file in files) {
                storage.DeleteFile(Path.Combine("ScreenManager", file));
            }
        }

        /// <summary>
        /// Create a DrawableAd with desired properties.
        /// </summary>
        public void CreateAd()
        {
            // Create a banner ad for the game.
            int width = 300;
            int height = 50;
            int x = (800 - width) / 2; // centered on the display
            int y = 430;

            bannerAd = adGameComponent.CreateAd(AdUnitId, new Rectangle(x, y, width, height), true);

            // Set some visual properties (optional). 
            //bannerAd.BorderEnabled = true; // default is true 
            //bannerAd.BorderColor = Color.White; // default is White 
            //bannerAd.DropShadowEnabled = true; // default is true
            
            // Provide the location to the ad for better targeting (optional). 
            // This is done by starting a GeoCoordinateWatcher and waiting for the location to 
            // available.
            // The callback will set the location into the ad.
            // Note: The location may not be available in time for the first ad request.
            adGameComponent.Enabled = false;
            this.gcw = new GeoCoordinateWatcher();
            this.gcw.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(gcw_PositionChanged);
            this.gcw.Start();
        }

        public void RemoveAd()
        {
            adGameComponent.RemoveAd(bannerAd);
            //adGameComponent.RemoveAll(); 
        }

        private void gcw_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // Stop the GeoCoordinateWatcher now that we have the device location. 
            this.gcw.Stop();
            bannerAd.LocationLatitude = e.Position.Location.Latitude;
            bannerAd.LocationLongitude = e.Position.Location.Longitude;
            AdGameComponent.Current.Enabled = true;
        }

        #endregion
    }
}
