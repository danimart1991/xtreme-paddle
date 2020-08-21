using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XtremePaddle
{
    /// <summary>
    /// Clase que define la pelota que rebota en nuestro juego.
    /// </summary>
    public class Ball : Sprite
    {
        #region Variables

        // Random y su semilla utilizados para calcular posiciones aleatorias.
        int semilla;
        Random random;

        // Definición de los sonidos que toman parte en la clase.
        SoundEffect soundPlink;
        SoundEffect soundVisible;

        #endregion

        #region Atributos

        /// <summary>
        /// Vector que define la velocidad de la Pelota, es decir, el ritmo con el que cambia a la posición siguiente.
        /// </summary>
        public Vector2 Velocidad;

        /// <summary>
        /// Calcula el numero de goles, que ha marcado el jugador.
        /// </summary>
        public int scoreSuperv = 0;

        /// <summary>
        /// Color que tiene nuestra pelota, importado de las opciones del juego.
        /// </summary>
        public Color color = GameStateManagementGame.Settings.BallColor;

        /// <summary>
        /// Escala de la pelota a la hora de dibujar el sprite y de definir sus rectangulos.
        /// </summary>
        public float Escala = 1.0f;

        /// <summary>
        /// Float usado para dar mayor o menor velocidad a la pelota dependiendo del nivel de dificultad.
        /// </summary>
        private float difficulty;

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="difficulty">Dificultad que proporciona mayor/menor velocidad a la pelota.</param>
        public Ball(float difficulty)
        {
            this.difficulty = difficulty;
        }

        /// <summary>
        /// Carga el contenido necesario de pelota.
        /// </summary>
        public override void LoadContent(ContentManager content)
        {
            // Cargamos la textura a partir del tema usado en las opciones
            texture = content.Load<Texture2D>("themes/" + GameStateManagementGame.Settings.Theme + "/ball");

            // Establecemos los límites de colisión para una detección de colisiones precisa
            // darse cuenta de las sombras y el espacio vacío en la textura
            collisionBounds = new Rectangle(8, 8, 32, 32);

            // Cargamos los sonidos para los efectos
            soundPlink = content.Load<SoundEffect>("sounds/plink");
            soundVisible = content.Load<SoundEffect>("sounds/powerups/visible");

            // Cargamos la semilla a partir del momento actual y el random.
            semilla = (int)DateTime.Now.Ticks;
            random = new Random(semilla);
        }

        #endregion

        #region Update y Draw

        /// <summary>
        /// Actualiza la posición de la pelota dependiendo de su Velocidad, y el tiempo transcurrido.
        /// </summary>
        public void Update(TimeSpan elapsedTime)
        {
            // Si difficultad == 4 significa que es el modo de dos jugadores ascendente y la velocidad tiene que empezar baja e ir aumentando poco a poco.
            if (difficulty == 4) { Velocidad *= 1.001f; }

            Position += Velocidad * (float)elapsedTime.TotalSeconds;
        }

        /// <summary>
        /// Actualiza el rectangulo de colisiones de la pelota, para aquellos
        /// casos en los que cambiemos la Escala de la pelota.
        /// </summary>
        public void UpdateCollisionBounds()
        {
            collisionBounds = new Rectangle((int)(8 * Escala), (int)(8 * Escala), (int)(32 * Escala), (int)(32 * Escala));
        }

        /// <summary>
        /// Dibujamos la pelota segun todos sus atributos.
        /// </summary>
        /// <param name="spriteBatch">Zona de dibujo importada del juego.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, color, 0, Vector2.Zero, Escala, SpriteEffects.None, 0);
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Comprueba si la pelota ha salido por los laterales derecho e
        /// izquierdo de la pantalla.
        /// </summary>
        /// <returns>
        /// 1 si la bola ha salido por el lado derecho, 
        /// -1 si la bola ha salido por el lado izquierdo,
        /// 0 si la bola sigue en la pantalla.
        /// </returns>
        public int IsOffscreen()
        {
            if (Position.X >= 800)
                return 1;
            if (Position.X <= -texture.Width)
                return -1;
            return 0;
        }

        /// <summary>
        /// Resetea los cambios introducidos en la pelota.
        /// </summary>
        public void Reset()
        {
            Escala = 1.0f;
            color = GameStateManagementGame.Settings.BallColor;
            UpdateCollisionBounds();
        }

        ///<summary>
        /// Colocamos la bola en el centro y la lanzamos de nuevo
        /// </summary>
        public void Colocar()
        {
            // Colocamos la bola en en centro de la pantalla
            CenterAtLocation(new Vector2(400, 240));

            // Generamos un angulo con el que sera lanzada de 90grados
            float angulo = MathHelper.ToRadians(random.Next(-45, 46));

            // Aleatorizamos el jugador que va a recibir la pelota.
            bool recibeJugador = random.Next() % 2 == 0;

            // Si la pelota va hacia el jugador damos la vuelta al angulo.
            if (recibeJugador) angulo += MathHelper.Pi;

            // Si estamos en supervivencia, sumamos +1 a la puntuacion J1 para en la siguiente linea
            // no se haga una división a 0, y si se coloca la bola, significa que se ha marcado, y
            // en supervivencia, aumentamos la dificultad con scoreJ1++, sino no aumenta.
            if (difficulty == 0) scoreSuperv++;

            // Si difficultad == 4 significa que es el modo de dos jugadores ascendente y la velocidad tiene que empezar baja e ir aumentando poco a poco.
            if (difficulty == 4)
            {
                // Generamos la velocidad a partir del angulo que hemos calculado.
                // y sustituimos dificultad por un grado facil para ir aumentandolo,
                // y un estabilizador (para ajustarlo todo).
                Velocidad = new Vector2((float)Math.Cos(angulo), (float)Math.Sin(angulo)) * 150f * (2 + (int)(scoreSuperv / 2));
            }
            // Si no lo es, significa que es 2 jugadores normal o 1 jugador normal o supervivencia.
            else
            {
                // Generamos la velocidad a partir del angulo que hemos calculado.
                // y le añadimos la dificultad y un estabilizador (para ajustarlo todo).
                Velocidad = new Vector2((float)Math.Cos(angulo), (float)Math.Sin(angulo)) * 150f * (difficulty + 2 + (int)(scoreSuperv / 2));
            }
        }

        /// <summary>
        /// Obligamos a la pelota a que se mantenga en la pantalla por arriba y abajo.
        /// </summary>
        public void ClampToScreen()
        {
            if (Bounds.Top < 0)
            {                   // Si damos por arriba
                Position.Y = 0 - Bounds.Y;          // colocamos la pelota en el borde
                Velocidad.Y *= -1;                  // y la mandamos con velocidad contraria.
                GameStateManagementGame.MusicManager.Play(soundPlink);
            }
            else if (Bounds.Bottom > 480)
            {       // Si damos por abajo.
                Position.Y = 460 - Bounds.Height;   // colocamos la pelota en el borde
                Velocidad.Y *= -1;                  // y la mandamos con velocidad contraria.
                GameStateManagementGame.MusicManager.Play(soundPlink);
            }
        }

        /// <summary>
        /// Transparentamos la pelota para volverla invisible y reproducimos efecto.
        /// </summary>
        public void Transparentar()
        {
            GameStateManagementGame.MusicManager.Play(soundVisible);
            color = Color.Transparent;
        }

        #endregion
    }
}
