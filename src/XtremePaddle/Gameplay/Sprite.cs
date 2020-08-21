using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XtremePaddle {
    /// <summary>
    /// Clase basica para todos los Sprites usados en el juego.
    /// </summary>
    public abstract class Sprite {
        #region Atributos

        // La textura del Sprite
        public Texture2D texture;

        // Como las texturas contienen sobras y brillos, especificamos el area de colision
        protected Rectangle collisionBounds;

        // Definimos la posición del Sprite
        public Vector2 Position;

        #endregion

        #region Inicializacion

        /// <summary>
        /// Permite al Sprite cargar contenido.
        /// </summary>
        /// <param name="content">El ContentManager de donde cargaremos el contenido.</param>
        public virtual void LoadContent(ContentManager content) { }

        #endregion

        #region Update y Draw

        /// <summary>
        /// Dibuja de manera sencilla el Sprite.
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(texture, Position, Color.White);
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Obtenemos los limites de colisión para el espacio de textura del sprite.
        /// </summary>
        public Rectangle BaseCollisionBounds {
            get {
                return collisionBounds;
            }
        }

        /// <summary>
        /// Obtenemos los limites de colision del sprite en el espacio.
        /// </summary>
        public Rectangle Bounds {
            get {
                // Primero con los limites de colisión
                Rectangle bounds = collisionBounds;

                // Y los compensamos con la posicion actual
                bounds.X += (int)Position.X;
                bounds.Y += (int)Position.Y;

                return bounds;
            }
        }

        /// <summary>
        /// Posiciona el Sprite en el centro de un punto dado.
        /// </summary>
        /// <param name="center">Localizacion donde queremos centrar el sprite.</param>
        public void CenterAtLocation(Vector2 center) {
            Position = center - new Vector2(collisionBounds.Width / 2, collisionBounds.Height / 2);
        }

        #endregion
    }
}
