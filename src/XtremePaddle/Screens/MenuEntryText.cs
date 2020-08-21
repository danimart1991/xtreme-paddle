using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XtremePaddle
{
    /// <summary>
    /// Clase ayudante que representa un texto a modo de boton en MenuScreen.
    /// Dibuja el boton dependiendo del constructor y ofrece un sistema de seleccion.
    /// </summary>
    class MenuEntryText
    {
        #region Variables

        ScreenManager screenManager;

        // Dibujamos el texto con la fuente seleccionada, sino con menufont.
        string fontAux = "menufont";

        // Dibujamos el texto en el color seleccionado, sino en blanco.
        public Color colorAux = Color.White;

        // Posicion que nos da el usuario y luego apañaremos para que se centre.
        Vector2 auxPosition;

        // Si esta creciendo o no
        bool isGrowing;

        // El sonido al hacer click en el texto
        SoundEffect soundClick;

        #endregion

        #region Properties

        /// <summary>
        /// Obtiene o pone la fuente del texto a dibujar.
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        SpriteFont font;

        /// <summary>
        /// El usuario nos indica si quiere centrar el texto o no.
        /// </summary>
        public bool Center
        {
            get { return center; }
            set { center = value; }
        }

        bool center;

        /// <summary>
        /// Rotation que se le da al texto.
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        float rotation = 0f;

        /// <summary>
        /// Obtiene o pone el texto a dibujar.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        string text;

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
        /// Obtiene o Pone la posicion donde se dibujara el texto.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        Vector2 position;

        /// <summary>
        /// Obtiene o Pone el color del texto.
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        Color color = Color.White;

        /// <summary>
        /// Para cuando se quiere escalar el objeto.
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        float scale = 1;

        /// <summary>
        /// Obtiene o Pone el origen donde se dibujara el objeto.
        /// </summary>
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        Vector2 origin = Vector2.Zero;

        #endregion

        #region Eventos

        /// <summary>
        /// Evento que ocurre cuando el objeto es seleccionado.
        /// </summary>
        public event EventHandler Selected;

        /// <summary>
        /// Metodo para controlar el texto seleccionado.
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
        public MenuEntryText(string text, Vector2 auxPosition, bool center)
        {
            this.text = text;
            this.auxPosition = auxPosition;
            this.center = center;
        }

        /// <summary>
        /// Constructor más simple con rotacion.
        /// </summary>
        public MenuEntryText(string text, Vector2 auxPosition, bool center, float rotation)
        {
            this.text = text;
            this.auxPosition = auxPosition;
            this.center = center;
            this.rotation = rotation;
        }

        /// <summary>
        /// Constructor más simple con fuente propia.
        /// </summary>
        public MenuEntryText(string text, Vector2 auxPosition, string font, bool center)
        {
            this.text = text;
            this.auxPosition = auxPosition;
            fontAux = font;
            this.center = center;
        }

        /// <summary>
        /// Constructor para cambiar el color al texto.
        /// </summary>
        public MenuEntryText(string text, Vector2 auxPosition, Color color, bool center)
        {
            this.text = text;
            this.auxPosition = auxPosition;
            colorAux = color;
            this.center = center;
        }

        /// <summary>
        /// Constructor para cambiar el color al texto y rotacion.
        /// </summary>
        public MenuEntryText(string text, Vector2 auxPosition, Color color, bool center, float rotation)
        {
            this.text = text;
            this.auxPosition = auxPosition;
            colorAux = color;
            this.center = center;
            this.rotation = rotation;
        }

        /// <summary>
        /// Constructor con añadidos de color y fuente propia.
        /// </summary>
        public MenuEntryText(string text, Vector2 auxPosition, Color color, string font, bool center)
        {
            this.text = text;
            this.auxPosition = auxPosition;
            fontAux = font;
            colorAux = color;
            this.center = center;
        }

        /// <summary>
        /// Constructor para cambiar el color al texto y rotacion, fuente y todo.
        /// </summary>
        public MenuEntryText(string text, Vector2 auxPosition, Color color, string font, bool center, float rotation)
        {
            this.text = text;
            this.auxPosition = auxPosition;
            fontAux = font;
            colorAux = color;
            this.center = center;
            this.rotation = rotation;
        }

        /// <summary>
        /// Constructor para cambiar el color al texto y rotacion, fuente y hasta animacion.
        /// </summary>
        public MenuEntryText(string text, Vector2 auxPosition, Color color, string font, bool center, float rotation, string animation)
        {
            this.text = text;
            this.auxPosition = auxPosition;
            fontAux = font;
            colorAux = color;
            this.center = center;
            this.rotation = rotation;
            this.animation = animation;
        }

        /// <summary>
        /// Cargamos el contenido necesario para el texto.
        /// </summary>
        /// <param name="content">El ContentManager de donde cargaremos el contenido.</param>
        public virtual void LoadContent(ContentManager content)
        {
            soundClick = content.Load<SoundEffect>("sounds/click");
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
                origin = new Vector2(screenManager.Font.MeasureString(Text).X / 2, screenManager.Font.LineSpacing / 2);
                scale = scale + (isGrowing ? 0.05f : -0.05f);
                if (scale > 1.5f)
                    isGrowing = false;
                else if (scale < 0.5f)
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
        /// Dibujamos el texto con los datos que tenemos.
        /// </summary>
        public virtual void Draw(MenuScreen screen, GameTime gameTime)
        {
            // Debido a un error con los colores necesitamos un colorAux y scaleAux por scale
            color = colorAux;

            // Modificamos el alpha para las transiciones.
            color *= screen.TransitionAlpha;
            if (center)
            {
                position = new Vector2(auxPosition.X - (GetWidth(screen) / 2), auxPosition.Y - (GetHeight(screen) / 2));
            }
            else
            {
                position = auxPosition;
            }

            // Cargamos el sistema básico para dibujar y la fuente.
            screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            LoadContent(screen.ScreenManager.Content);
            font = screenManager.Content.Load<SpriteFont>(fontAux);

            // Dibujamos el texto
            spriteBatch.DrawString(font, text, position, color, MathHelper.ToRadians(rotation), origin, scale, SpriteEffects.None, 0);
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Obtiene la altura del texto escrito.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }

        /// <summary>
        /// Obtiene el ancho del texto escrito.
        /// </summary>
        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }

        #endregion
    }
}
