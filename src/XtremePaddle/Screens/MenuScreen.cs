using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace XtremePaddle
{
    /// <summary>
    /// Clase base para las pantallas que contengan menu. El usuario puede elegir
    /// una de las opciones del menu o presionar atras para volver.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        #region Variables

        // El sonido al hacer click atras
        SoundEffect soundBack;

        // Lista de textos y objetos a representar
        List<MenuEntryText> menuEntriesText = new List<MenuEntryText>();
        List<MenuEntryObject> menuEntriesObject = new List<MenuEntryObject>();

        // Titulo del menu
        string menuTitle;

        #endregion

        #region Atributos

        /// <summary>
        /// Obtiene la lista de entradas de texto del menu, para que las clases
        /// derivadas puedan añadir o cambiar el contenido del menu.
        /// </summary>
        protected IList<MenuEntryText> MenuEntriesText
        {
            get { return menuEntriesText; }
        }

        /// <summary>
        /// Obtiene la lista de entradas de objetos del menu, para que las clases
        /// derivadas puedan añadir o cambiar el contenido del menu.
        /// </summary>
        protected IList<MenuEntryObject> MenuEntriesObject
        {
            get { return menuEntriesObject; }
        }

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            // Los Menus solo necesitan Tap para hacer click
            EnabledGestures = GestureType.Tap;

            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Cargamos el contenido necesario para el menu.
        /// </summary>
        /// <param name="content">El ContentManager de donde cargaremos el contenido.</param>
        public virtual void LoadContent(ContentManager content)
        {
            soundBack = content.Load<SoundEffect>("sounds/back");
        }

        #endregion

        #region Entradas

        /// <summary>
        /// Creamos un rectangulo para que podamos hacer click en el texto dibujado
        /// como si se tratase de un boton.
        /// </summary>
        protected virtual Rectangle GetMenuEntryTextHitBounds(MenuEntryText entry)
        {
            return new Rectangle((int)entry.Position.X, (int)entry.Position.Y,
                                    entry.GetWidth(this), entry.GetHeight(this));
        }

        /// <summary>
        /// Respondemos a los gestos del usuario, entrando en otros menus,
        /// saliendo del menu o no haciendo nada.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            // Si presionamos el boton atras volvemos en el menu con un sonido.
            if (input.IsMenuCancel())
            {
                GameStateManagementGame.MusicManager.Play(soundBack);
                OnCancel();
            }

            // Comprobamos si hemos hecho click en algúna entrada de menu.
            foreach (GestureSample gesture in input.Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    // Convertimos el click a un punto para ver si toca un rectangulo.
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                    // Recorremos las entradas del texto para ver si alguna ha sido tocada
                    for (int i = 0; i < menuEntriesText.Count; i++)
                    {
                        MenuEntryText menuEntry = menuEntriesText[i];

                        if (GetMenuEntryTextHitBounds(menuEntry).Contains(tapLocation))
                        {
                            // Al seleccionar alguna entrada pasamos a su accion.
                            OnSelectEntryText(i);
                        }
                    }

                    // Recorremos las entradas de objetos para ver si alguna ha sido tocada
                    for (int i = 0; i < menuEntriesObject.Count; i++)
                    {
                        MenuEntryObject menuEntry = menuEntriesObject[i];

                        if (menuEntry.Rectangle.Contains(tapLocation))
                        {
                            // Al seleccionar alguna entrada pasamos a su accion.
                            OnSelectEntryObject(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Evento cuando el usuario selecciona alguna entrada de texto.
        /// </summary>
        protected virtual void OnSelectEntryText(int entryIndex)
        {
            menuEntriesText[entryIndex].OnSelectEntry();
        }

        /// <summary>
        /// Evento cuando el usuario selecciona alguna entrada de objeto.
        /// </summary>
        protected virtual void OnSelectEntryObject(int entryIndex)
        {
            menuEntriesObject[entryIndex].OnSelectEntry();
        }

        /// <summary>
        /// Evento cuando el usuario cancela el menu dando atras.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        /// <summary>
        /// Ayudamos al definir textos u objetos por si incluimos un salir o atras.
        /// </summary>
        protected void OnCancel(object sender)
        {
            OnCancel();
        }

        #endregion

        #region Update
        /// <summary>
        /// Actualizamos los componentes
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Actualizamos cada objeto.
            for (int i = 0; i < menuEntriesObject.Count; i++)
            {
                MenuEntryObject menuEntry = menuEntriesObject[i];
                menuEntry.Update();
            }

            // Actualizamos cada texto.
            for (int i = 0; i < menuEntriesText.Count; i++)
            {
                MenuEntryText menuEntry = menuEntriesText[i];
                menuEntry.Update();
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        #endregion

        #region Draw

        /// <summary>
        /// Dibuja el menu completo.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Cargamos el contenido basico de dibujo
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            LoadContent(ScreenManager.Content);
            spriteBatch.Begin();

            // Dibujamos cada objeto.
            for (int i = 0; i < menuEntriesObject.Count; i++)
            {
                MenuEntryObject menuEntry = menuEntriesObject[i];
                menuEntry.Draw(this, gameTime);
            }

            // Dibujamos cada texto.
            for (int i = 0; i < menuEntriesText.Count; i++)
            {
                MenuEntryText menuEntry = menuEntriesText[i];
                menuEntry.Draw(this, gameTime);
            }

            // Hacemos que el menu se deslice durante la transicion, usando using a
            // una curva de velocidad que haga que la entrada no sea, a la misma
            // velocidad constantemente.
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Dibujamos el titulo de la pantalla centrado
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 40);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;

            // Con su color.
            Color titleColor = new Color(255, 255, 255) * TransitionAlpha;
            float titleScale = 1.25f;

            // Con su sombra correspondiente.
            Vector2 titlePositionShadow = new Vector2((graphics.Viewport.Width / 2) + 3, 42);
            Color titleColorShadow = new Color(0, 0, 0) * TransitionAlpha;

            // Los posicionamos
            titlePosition.Y -= transitionOffset * 100;
            titlePositionShadow.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePositionShadow, titleColorShadow, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                       titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }


        #endregion
    }
}
