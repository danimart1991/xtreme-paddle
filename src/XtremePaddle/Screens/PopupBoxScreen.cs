using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XtremePaddle {
    /// <summary>
    /// Una pantalla Popup para mostrar textos simples de mensaje
    /// en forma de pantalla superpuesta sobre otra.
    /// </summary>
    class PopupBoxScreen : GameScreen {
        #region Variables

        // Sonido al hacer click atras.
        SoundEffect soundBack;

        // Mensaje que queremos mostrar.
        string message;

        Texture2D gradientTexture;

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        public PopupBoxScreen(string message) {
            this.message = message;

            //La pantalla es un popup
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }


        /// <summary>
        /// Cargamos el contenido necesario para la pantalla.
        /// </summary>
        public override void LoadContent() {
            ContentManager content = ScreenManager.Game.Content;

            soundBack = content.Load<SoundEffect>("sounds/back");
            gradientTexture = content.Load<Texture2D>("gradient");
        }


        #endregion

        #region Entrada


        /// <summary>
        /// Respuesta a la introduccion del usuario de entradas.
        /// </summary>
        public override void HandleInput(InputState input) {
            // Si presionamos atras, salimos de la pantalla.
            if (input.IsMenuCancel()) {
                GameStateManagementGame.MusicManager.Play(soundBack);
                ExitScreen();
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// Dibujamos el cuadro de dialogo.
        /// </summary>
        public override void Draw(GameTime gameTime) {
            // Importamos fuentes y SpriteBatch comunes.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // Oscurecemos las otras pantallas para mostrar el popup mejor.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Centramos el mensaje en la vista predeterminada.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            // Calculamos cuando ocupara el rectangulo dependiendo del texto.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            // Introducimos el popup en las transiciones.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Dibujamos el rectangulo de fondo.
            spriteBatch.Draw(gradientTexture, backgroundRectangle, color);

            // Dibujamos el texto.
            spriteBatch.DrawString(font, message, textPosition, color);

            spriteBatch.End();
        }


        #endregion
    }
}
