using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XtremePaddle
{
    /// <summary>
    /// Clase Paleta, controladas por el J1=1, J2=2 o por la IA=0.
    /// </summary>
    class Paddle : Sprite
    {
        #region Variables

        // Random utilizados para calcular posiciones aleatorias.
        Random random = new Random();

        // Definición de los sonidos que toman parte en la clase.
        SoundEffect soundFreeze;
        SoundEffect soundVisible;

        // Definimos la textura y el color para transparentarlo o no.
        Texture2D texFreeze;
        Color colorFreeze = Color.Transparent;


        #endregion

        #region Atributos

        /// <summary>
        /// Tipo de paleta que vamos a usar: J1=1, J2=2, IA=0.
        /// </summary>
        private int tipoPaleta;

        /// <summary>
        /// Float usado para dar mayor o menor velocidad a la paleta de tipo IA
        /// dependiendo del nivel de dificultad.
        /// </summary>
        private int difficulty;

        /// <summary>
        /// Puntuación del jugador 1 en modo supervivencia.
        /// </summary>
        private int scoreSuperv;

        /// <summary>
        /// Escala de la paleta a la hora de dibujar el sprite y de definir sus rectangulos.
        /// </summary>
        public float escala = 1.0f;

        /// <summary>
        /// Define si la paleta puede moverse o no.
        /// </summary>
        public bool congelado = false;

        /// <summary>
        /// Color que tiene nuestra paleta, importado de las opciones del juego.
        /// </summary>
        public Color color;

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tipoPaleta">¿Que tipo de paleta es?</param>
        /// <param name="difficulty">Dificultad que proporciona mayor/menor velocidad a la paleta IA.</param>
        public Paddle(int tipoPaleta, int difficulty)
        {
            this.tipoPaleta = tipoPaleta;
            this.difficulty = difficulty;
        }

        /// <summary>
        /// Cargamos el contenido necesario de la paleta.
        /// </summary>
        public override void LoadContent(ContentManager content)
        {
            // Cargamos la textura a partir del tema usado en las opciones
            texture = content.Load<Texture2D>("themes/" + GameStateManagementGame.Settings.Theme + "/paddle");
            texFreeze = content.Load<Texture2D>("effects/freeze");

            // Dependiendo de que paleta sea, cargamos un color u otro.
            // Recogiendo el valor de las opciones.
            if (tipoPaleta == 1) color = GameStateManagementGame.Settings.PaddleJ1Color;
            else color = GameStateManagementGame.Settings.PaddleJ2Color;

            // Cargamos los sonidos para los efectos
            soundFreeze = content.Load<SoundEffect>("sounds/powerups/freeze");
            soundVisible = content.Load<SoundEffect>("sounds/powerups/visible");

            // Establecemos los límites de colisión para una detección de colisiones precisa
            // darse cuenta de las sombras y el espacio vacío en la textura
            collisionBounds = new Rectangle(17, 11, 13, 75);
        }

        #endregion

        #region Update y Draw

        /// <summary>
        /// Actualiza el rectangulo de colisiones de la paleta, para aquellos
        /// casos en los que cambiemos la Escala de la paleta.
        /// </summary>
        public void UpdateCollisionBounds()
        {
            collisionBounds = new Rectangle((int)(17 * escala), (int)(11 * escala), (int)(13 * escala), (int)(75 * escala));
        }

        /// <summary>
        /// Hacemos que en el caso de la IA persiga a la pelota.
        /// </summary>
        /// <param name="elapsedTime">Tiempo de juego necesario</param>
        /// <param name="ball">Pelota a la que queremos seguir</param>
        public void UpdateAI(TimeSpan elapsedTime, Ball ball)
        {
            // Intentamos que el centro de la paleta persiga al centro de la pelota
            float speed = 150f;
            float objetivoY = ball.Position.Y + ball.Bounds.Height / 2 - ball.BaseCollisionBounds.Height;

            // Si estamos en supervivencia, sumamos +1 a la puntuacion J1 para en la siguiente linea
            // no se haga una división a 0, y si se coloca la paleta, significa que se ha marcado, y
            // en supervivencia, aumentamos la dificultad con scoreJ1++, sino no aumenta.
            switch (difficulty)
            {
                case 0: // Supervivencia
                    scoreSuperv++;

                    // Generamos la velocidad a partir de la dificultad, la puntuación y un estabilizador.
                    speed = 150f * (2 + (int)(scoreSuperv / 2));
                    break;

                case 1: // Facil
                    // Generamos la velocidad a partir de la dificultad, la puntuación y un estabilizador.
                    speed = 250f;
                    break;

                case 2: // Medio
                    // Generamos la velocidad a partir de la dificultad, la puntuación y un estabilizador.
                    speed = 375f;
                    break;

                case 3: // Dificil
                    // Generamos la velocidad a partir de la dificultad, la puntuación y un estabilizador.
                    speed = 475f;
                    break;
            }

            // En vez de que la paleta se teletransporte hasta el sitio, queremos que se deslice.
            //float speed = 100 * difficulty / 2;
            float delta = objetivoY - Position.Y;

            // Para que no ocurra teletransporte, encajamos la paleta para que no pueda ir rapido.
            // Haciendo que la paleta tenga una velocidad de cambio.
            // Si estamos cerca del objetivo nos quedamos esperandole.
            if (Math.Abs(delta) < speed * (float)elapsedTime.TotalSeconds)
            {
                Position.Y = objetivoY;
            }
            // En el otro caso, calculamos la direccion de movimiento y movemos la paleta.
            else
            {
                int direction = Math.Sign(delta);
                Position.Y += direction * speed * (float)elapsedTime.TotalSeconds;
            }

            // Y nos aseguramos de que la paleta no salga de la pantalla.
            ClampToScreen();
        }

        /// <summary>
        /// Dibujamos la paleta segun sea de un tipo u otro.
        /// </summary>
        /// <param name="spriteBatch">Zona de dibujo importada del juego.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Si es el jugador 1 la dibuja en un sentido, y J2 o IA damos la vuelta al dibujo.
            if (tipoPaleta == 1)
            {
                spriteBatch.Draw(texture, Position, null, color, 0, Vector2.Zero, escala, SpriteEffects.None, 0);
                spriteBatch.Draw(texFreeze, Position, null, colorFreeze, 0, Vector2.Zero, escala, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(texture, Position, null, color, 0, Vector2.Zero, escala, SpriteEffects.FlipHorizontally, 0);
                spriteBatch.Draw(texFreeze, Position, null, colorFreeze, 0, Vector2.Zero, escala, SpriteEffects.FlipHorizontally, 0);
            }
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Comprobamos y reaccionamos ante una colisión con la pelota
        /// </summary>
        /// <param name="ball">Bola con la que puede haber colisionado.</param>
        /// <returns>true si hay colision, false si no</returns>
        public bool Collide(Ball ball)
        {
            // Comprobamos si los rectangulos de colisión de paleta y pelota interseccionan.
            if (Bounds.Intersects(ball.Bounds))
            {
                // Recojemos la magnitud de la velocidad de la bola y la direccion de X en la que se mueve.
                float magnitudVelBall = ball.Velocidad.Length();
                int direccionXBall = Math.Sign(ball.Velocidad.X);

                // Calculamos un vamos en el rango [-1, 1] indicando el punto de colisión con
                // la paleta, -1 es la parte de arriba y 1 es la parte de abajo
                float centroPaleta = Bounds.Center.Y;
                float centroBall = ball.Bounds.Center.Y;
                float diferencia = (centroBall - centroPaleta) / (Bounds.Height / 2);

                // Generamos el anguno nuevo de la pelota basandonos en el cuadrado de las diferencias,
                // simulando una especie de superficie curva, que hace que el rebote dependa de donde
                // ha colisionado la pelota con la paleta.
                float angulo = (diferencia * diferencia) * MathHelper.ToRadians(30) * Math.Sign(diferencia);

                // Usamos este angulo para generar la nueva direccion de la pelota.
                ball.Velocidad = new Vector2((float)Math.Cos(angulo) * direccionXBall, (float)Math.Sin(angulo));

                // Escalamos la velocidad de vuelta a la original y da la vuelta a la pelota.
                ball.Velocidad *= magnitudVelBall;
                ball.Velocidad.X *= -1;

                // Colocamos la pelota tanto en una paleta como en otra, en el borde,
                // quitando la interseccion para que de sensación de limpieza.
                if (ball.Velocidad.X < 0)
                    ball.Position.X = Bounds.Left - ball.BaseCollisionBounds.Width;
                else
                    ball.Position.X = Bounds.Right - ball.BaseCollisionBounds.X;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Obligamos a la paleta a que se mantenga en la pantalla por arriba y abajo.
        /// </summary>
        public void ClampToScreen()
        {
            if (Bounds.Top < 0)
            {                           // Si damos por arriba
                Position.Y = 0 - collisionBounds.Y;         // colocamos la paleta en el borde.
            }
            else if (Bounds.Bottom > 480)
            {               // Si damos por abajo
                Position.Y = 460 - collisionBounds.Height;  // colocamos la pelota en el borde.
            }
        }

        /// <summary>
        /// Resetea los cambios introducidos en la paleta.
        /// </summary>
        public void Reset()
        {
            escala = 1.0f;
            congelado = false;
            if (tipoPaleta == 1) color = GameStateManagementGame.Settings.PaddleJ1Color;
            else color = GameStateManagementGame.Settings.PaddleJ2Color;
            colorFreeze = Color.Transparent;
            UpdateCollisionBounds();
        }

        ///<summary>
        /// Colocamos la paleta centrada en el lado que le corresponda.
        /// </summary>
        public void Colocar()
        {
            if (tipoPaleta == 1) CenterAtLocation(new Vector2(665, 240));
            else CenterAtLocation(new Vector2(100, 240));
        }

        /// <summary>
        /// Paramos la paleta para congelarla y reproducimos efectos
        /// </summary>
        public void Congelar()
        {
            GameStateManagementGame.MusicManager.Play(soundFreeze);
            congelado = true;
            colorFreeze = Color.White;
            color = Color.White;
        }

        /// <summary>
        /// Transparentamos la paleta para volverla invisible y reproducimos efectos.
        /// </summary>
        public void Transparentar()
        {
            GameStateManagementGame.MusicManager.Play(soundVisible);
            color = Color.Transparent;
        }

        #endregion
    }
}
