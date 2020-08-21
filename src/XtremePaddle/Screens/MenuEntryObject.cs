using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XtremePaddle
{
    /// <summary>
    /// Clase ayudante que representa una imagen a modo de boton en MenuScreen.
    /// Dibuja el boton dependiendo del constructor y ofrece un sistema de seleccion.
    /// </summary>
    class MenuEntryObject
    {
        #region Variables

        ScreenManager screenManager;

        // Dibujamos el objeto en el color seleccionado, sino en blanco.
        Color colorAux = Color.White;

        // La textura del objeto
        Texture2D texture;

        // Si esta creciendo o no
        bool isGrowing;

        // El sonido al hacer click en el objeto
        SoundEffect soundClick;

        #endregion

        #region Atributos

        /// <summary>
        /// Obtiene o Pone el color del objeto.
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        Color color = Color.White;

        /// <summary>
        /// Obtiene o Pone el rectangulo de este objeto al hacer click.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }

        Rectangle rectangle;

        /// <summary>
        /// Obtiene o Pone el nombre de la textura para este objeto.
        /// </summary>
        public string TexName
        {
            get { return texName; }
            set { texName = value; }
        }

        string texName;

        /// <summary>
        /// Obtiene o Pone el nombre de la animacion para este objeto.
        /// </summary>
        public string Animation
        {
            get { return animation; }
            set { animation = value; }
        }

        string animation = "";

        /// <summary>
        /// Obtiene o Pone el origen donde se dibujara el objeto.
        /// </summary>
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        Vector2 origin = Vector2.Zero;

        /// <summary>
        /// Obtiene o Pone la posicion donde se dibujara el objeto.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        Vector2 position;

        /// <summary>
        /// Para cuando se trabaja con arrays o listas sea facil de llamar.
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        int id = 0;

        /// <summary>
        /// Para cuando se quiere rotar el objeto.
        /// </summary>
        public int Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        int rotation = 0;

        /// <summary>
        /// Para cuando se quiere escalar el objeto.
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        float scale = 1;

        #endregion

        #region Eventos

        /// <summary>
        /// Evento que ocurre cuando el objeto es seleccionado.
        /// </summary>
        public event EventHandler Selected;

        /// <summary>
        /// Metodo para controlar el objeto seleccionado.
        /// </summary>
        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
            {
                GameStateManagementGame.MusicManager.Play(soundClick);
                Selected(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Inicializacion


        /// <summary>
        /// Constructor más simple.
        /// </summary>
        public MenuEntryObject(string texName, Vector2 position)
        {
            this.texName = texName;
            this.position = position;
        }

        /// <summary>
        /// Constructor más simple con animaciones.
        /// </summary>
        public MenuEntryObject(string texName, Vector2 position, string animation)
        {
            this.texName = texName;
            this.position = position;
            this.animation = animation;
        }

        /// <summary>
        /// Constructor para cambiar el color al objeto.
        /// </summary>
        public MenuEntryObject(string texName, Vector2 position, Color color)
        {
            this.texName = texName;
            this.position = position;
            this.color = color;
        }

        /// <summary>
        /// Constructor con añadidos de color y id para arrays.
        /// </summary>
        public MenuEntryObject(string texName, Vector2 position, Color color, int id)
        {
            this.texName = texName;
            this.position = position;
            this.color = color;
            this.id = id;
        }

        /// <summary>
        /// Cargamos el contenido necesario para el objeto.
        /// </summary>
        /// <param name="content">El ContentManager de donde cargaremos el contenido.</param>
        public virtual void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>(texName);
            soundClick = content.Load<SoundEffect>("sounds/click");
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        #endregion

        #region Update

        /// <summary>
        /// Update del objeto para animaciones
        /// </summary>
        public virtual void Update()
        {
            if (animation.Contains("hinchar"))
            {
                origin = new Vector2(texture.Width / 2, texture.Height / 2);
                rotation = -10;
                scale = scale + (isGrowing ? 0.05f : -0.05f);
                if (scale > 1.0f)
                    isGrowing = false;
                else if (scale < 0.25f)
                    isGrowing = true;
            }

            if (animation.Contains("girar"))
            {
                rotation += 10;
                if (rotation >= 360)
                    rotation = 0;
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// Dibujamos el objeto con los datos que tenemos.
        /// </summary>
        public virtual void Draw(MenuScreen screen, GameTime gameTime)
        {
            // Debido a un error con los colores necesitamos un colorAux
            colorAux = color;

            // Modificamos el alpha para las transiciones.
            colorAux *= screen.TransitionAlpha;

            // Cargamos el sistema básico para dibujar.
            screenManager = screen.ScreenManager;
            LoadContent(screen.ScreenManager.Content);
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            // Dibujamos el objeto
            spriteBatch.Draw(texture, position, null, colorAux, MathHelper.ToRadians(rotation), origin, scale, SpriteEffects.None, 0);
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Obtiene la altura del objeto.
        /// </summary>
        public virtual int GetHeight()
        {
            return texture.Height;
        }


        /// <summary>
        /// Obtiene el ancho del objeto.
        /// </summary>
        public virtual int GetWidth()
        {
            return texture.Width;
        }

        #endregion
    }
}
