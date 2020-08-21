using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XtremePaddle
{
    /// <summary>
    /// La background screen dibuja un fondo detras de todas las pantallas
    /// con el fin de ahorrar recursos y mejorar el aspecto visual del 
    /// juego en si. Haciendo que las transiciones solo sucedan en las pantallas
    /// superiores a ella.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        #region Variables

        ContentManager content;
        Texture2D backgroundTexture;
        String background;

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen(String background)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.background = background;
        }


        /// <summary>
        /// Cargamos el contenido necesario para la pantalla a partir del
        /// content predeterminado si existe y un fondo.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>(background);
            ScreenManager.CreateAd();
        }


        /// <summary>
        /// Descargamos el contenido de la pantalla si es necesario.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
            ScreenManager.RemoveAd();
        }


        #endregion

        #region Update y Draw


        /// <summary>
        /// A diferencia del resto de pantallas la actualizacion de esta pantalla,
        /// no debe incluir transiciones ni nada por el estilo, debido a que es una
        /// pantalla de fondo y las pantallas superiores realizan esta funciones por el.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Dibujamos la pantalla.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();
        }

        #endregion
    }
}
