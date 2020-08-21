using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XtremePaddle {
    /// <summary>
    /// La pantalla de carga coordina la transicion entre el sistema de menus y el
    /// juego en si. Normalmente una pantalla debera oscurecerse al mismo tiempo de que
    /// la siguiente pantala debe encenderse, pero para transiciones largas, esto puede
    /// tomar un tiempo de carga largo para los datos, queremos que nuestro sistema de 
    /// menus desaparezca completamente antes de que empiece a cargar el juego.
    /// Esto se consigue:
    /// 
    /// - Pidiendo salir a todas las pantallas oscureciendose.
    /// - Activando la pantalla de carga, y encendiendo al mismo tiempo.
    /// - La pantalla de carga controla el estado de las pantallas anteriores.
    /// - Cuando todas han acabado, activa la siguiente pantalla real, que se
    ///   tomara, un tiempo para cargar sus datos. La pantalla de carga sera 
    ///   lo unico que se muestre mientras cargamos estos datos.
    /// </summary>
    class LoadingScreen : GameScreen {
        #region Variables

        // Si queremos o no, Carga lenta
        bool loadingIsSlow;
        // El resto de pantallas han muerto o no
        bool otherScreensAreGone;

        // Pantallas a cargar con el loading
        GameScreen[] screensToLoad;

        #endregion

        #region Inicializacion


        /// <summary>
        /// El constructor es privado: las pantallas de carga deben
        /// ser activadas mediante el metodo estatico Load.
        /// </summary>
        private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow,
                              GameScreen[] screensToLoad) {
            this.loadingIsSlow = loadingIsSlow;
            this.screensToLoad = screensToLoad;

            // No queremos serializar las pantallas de carga. Si el usuario sale
            // mientras se está cargando un juego, el juego se cargara en la pantalla
            // anterior a la carga.
            IsSerializable = false;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Activamos la pantalla de carga.
        /// </summary>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
                                params GameScreen[] screensToLoad) {
            // Pedimos a todas las pantallas que se oscurezcan.
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Creamos y activamos la pantalla de carga.
            LoadingScreen loadingScreen = new LoadingScreen(screenManager,
                                                            loadingIsSlow,
                                                            screensToLoad);

            screenManager.AddScreen(loadingScreen);
        }


        #endregion

        #region Update y Draw


        /// <summary>
        /// Actualizamos la pantalla de carga.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Si todas las pantallas anteriores han acabado,
            // es el momento para activar la carga.
            if (otherScreensAreGone) {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in screensToLoad) {
                    if (screen != null) {
                        ScreenManager.AddScreen(screen);
                    }
                }

                // Una vez que la carga ha finalizado, usamos ResetElapsedTime
                // para decirle al mecanismo de tiempo del juego que hemos
                // terminado un frame largo y que no deberia tenerlo en cuenta.
                ScreenManager.Game.ResetElapsedTime();
            }
        }


        /// <summary>
        /// Dibujamos la pantalla de carga.
        /// </summary>
        public override void Draw(GameTime gameTime) {
            // Si somos la unica pantalla activa, significa que las pantallas anteriores
            // han muerto. Nos aseguramos en el metodo Draw, y no en Update, porque
            // no solo es necesario que las pantallas se hayan desactivado: necesitamos
            // que hayan desaparecido completamente antes de cargar.
            if ((ScreenState == ScreenState.Active) &&
                (ScreenManager.GetScreens().Length == 1)) {
                otherScreensAreGone = true;
            }

            // Si el juego tarda un poco en cargar, mostramos un mensaje de carga, 
            // mientras el juego se inicializa, sin embargo los menus cargan muy rapido, 
            // asique no necesitamos una descarga desde el juego a los menus. Este
            // Parametro, nos dice si debemos mostrar el mensaje y esperar mucho tiempo, 
            // o si al contrario estamos cargando los menus y no necesitamos esperar
            // tanto tiempo para su carga.
            if (loadingIsSlow) {
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                SpriteFont font = ScreenManager.Font;

                string message = CatStrings.loadingText;

                // Centramos el texto en la vista.
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                Color color = Color.White * TransitionAlpha;

                // Dibujamos el Texto.
                spriteBatch.Begin();
                spriteBatch.DrawString(font, message, textPosition, color);
                spriteBatch.End();
            }
        }

        #endregion
    }
}
