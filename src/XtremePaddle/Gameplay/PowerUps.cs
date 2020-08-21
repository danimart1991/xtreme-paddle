using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XtremePaddle {
    /// <summary>
    /// Las Paletas, controladas por el jugador 1, 2 o por la IA.
    /// </summary>
    class PowerUps : Sprite {
        #region Variables

        // Random y su semilla utilizados para calcular posiciones aleatorias.
        int semilla;
        Random random;

        #endregion

        #region Atributos

        /// <summary>
        /// Tipo de PowerUp que vamos a usar.
        /// </summary>
        /// <remarks>
        /// Int. Nombre: Descripción    (Colores disponibles)
        /// 1. TamBola: Aumenta o disminuye el tamaño de la bola (0)
        /// 2. InviBola: Transparenta la bola (0)
        /// 3. VelBola: Aumenta o disminuye la velocidad de la bola (0)
        /// 4. Freeze: Congela a uno de los jugadores (1,2)
        /// 5. TamMasPlayer: Aumenta la paleta de uno de los jugadores (1,2)
        /// 6. TamMenosPlayer: Disminuye la paleta de uno de los jugadores (1,2)
        /// 7. InviPlayer: Transparenta a uno de los dos jugadores (1,2)
        /// </remarks>
        private int tipo;

        /// <summary>
        /// Color que tiene nuestro Powerup.
        /// 1 = Verde = Beneficia
        /// 2 = Rojo = Perjudica
        /// 0 = Naraja = Aleatorio/Indiferente
        /// </summary>
        private int color;

        /// <summary>
        /// Rotación del powerup para darle un toque más dinamico.
        /// </summary>
        private int rotacion;

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tipo">¿Que tipo de PowerUp es?</param>
        /// <param name="color">¿Que color es?/¿A quien da ventaja?</param>
        public PowerUps(int tipo, int color) {
            this.tipo = tipo;
            this.color = color;
        }

        /// <summary>
        /// Carga el contenido necesario de los powerups.
        /// </summary>
        public override void LoadContent(ContentManager content) {
            // Cargamos la textura del tipo del constructor
            texture = content.Load<Texture2D>(Convert.ToString("powerups/" + tipo));

            // Establecemos los límites de colisión para una detección de colisiones precisa
            // darse cuenta de las sombras y el espacio vacío en la textura
            collisionBounds = new Rectangle(6, 6, 52, 52);

            // Cargamos la semilla a partir del momento actual y el random.
            semilla = (int)DateTime.Now.Ticks;
            random = new Random(semilla);
        }

        #endregion

        #region Update y Draw

        /// <summary>
        /// Actualizamos el powerup haciendolo rotar.
        /// </summary>
        public void Update() {
            // Sumamos 1 a la rotación y si da una vuelta completa lo ponemos a 0
            rotacion += 1;
            if (rotacion >= 360)
                rotacion = 0;
        }

        /// <summary>
        /// Dibuja el Sprite dependiendo del color.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch) {
            // Detallamos la posición a la que queremos dibujar el Sprite
            // Se debe realizar esto debido a la rotación existente
            int posX = (int)Position.X + (texture.Width / 2);
            int posY = (int)Position.Y + (texture.Height / 2);

            // Dependiendo del color escogido elegimos entre los 3 posibles
            // 0 = naranga, 1 = verde, 2 = rojo
            switch (color) {
                case 0:
                    spriteBatch.Draw(texture, new Vector2(posX, posY), null, Color.Orange, MathHelper.ToRadians(rotacion),
                                    new Vector2(Bounds.Width / 2, Bounds.Height / 2), 1, SpriteEffects.None, 0);
                    break;
                case 1:
                    spriteBatch.Draw(texture, new Vector2(posX, posY), null, Color.Green, MathHelper.ToRadians(rotacion),
                                    new Vector2(Bounds.Width / 2, Bounds.Height / 2), 1, SpriteEffects.None, 0);
                    break;
                case 2:
                    spriteBatch.Draw(texture, new Vector2(posX, posY), null, Color.Red, MathHelper.ToRadians(rotacion),
                                    new Vector2(Bounds.Width / 2, Bounds.Height / 2), 1, SpriteEffects.None, 0);
                    break;
                default:
                    Console.WriteLine("Selección invalida. Escoge 0, 1 o 2.");
                    break;
            }
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Comprobamos y reaccionamos ante una colisión con la pelota
        /// </summary>
        /// <param name="ball">Bola con la que puede haber colisionado.</param>
        /// <param name="paleta1">Paleta del Jugador 1</param>
        /// <param name="paleta2">Paleta del Jugador 2</param>
        /// <param name="elapsedTime">Tiempo de juego transcurrido</param>
        /// <returns>true si hay colisión, false si no lo hay.</returns>
        public bool Collide(Ball ball, Paddle paleta1, Paddle paleta2, TimeSpan elapsedTime) {
            // Comprobamos si los rectangulos de colisión de powerup y pelota interseccionan.
            if (Bounds.Intersects(ball.Bounds)) {
                switch (tipo) {
                    case 1: //TamBola
                        bool masOMenos = random.Next() % 2 == 0;
                        ball.Escala = ((masOMenos) ? ball.Escala + 0.25f : ball.Escala - 0.25f);
                        ball.UpdateCollisionBounds();
                        break;
                    case 2: //InviBola
                        ball.Transparentar();
                        break;
                    case 3: //VelBola
                        masOMenos = random.Next() % 2 == 0;
                        ball.Velocidad = ((masOMenos) ? (ball.Velocidad * 1.50f) : (ball.Velocidad * 0.75f));
                        break;
                    default:
                        if (ball.Velocidad.X > 0) { //Lo toca el Jugador2 o la IA
                            switch (tipo) {
                                case 4: //Congelacion
                                    if (color == 1) paleta1.Congelar();
                                    else paleta2.Congelar();
                                    break;
                                case 5: //TamMasPlayer
                                    if (color == 1) {
                                        paleta2.escala += 0.25f;
                                        paleta2.UpdateCollisionBounds();
                                    } else {
                                        paleta1.escala += 0.25f;
                                        paleta1.UpdateCollisionBounds();
                                    }
                                    break;
                                case 6: //TamMenosPlayer
                                    if (color == 1) {
                                        paleta1.escala -= 0.25f;
                                        paleta1.UpdateCollisionBounds();
                                    } else {
                                        paleta2.escala -= 0.25f;
                                        paleta2.UpdateCollisionBounds();
                                    }
                                    break;
                                case 7: //InviPlayer
                                    if (color == 1) paleta1.Transparentar();
                                    else paleta2.Transparentar();
                                    break;
                            }
                            return true;
                        } else if (ball.Velocidad.X < 0) {  //Lo toca el Jugador1
                            switch (tipo) {
                                case 4: //Congelacion
                                    if (color == 1) paleta2.Congelar();
                                    else paleta1.Congelar();
                                    break;
                                case 5: //TamMasPlayer
                                    if (color == 1) {
                                        paleta1.escala += 0.25f;
                                        paleta1.UpdateCollisionBounds();
                                    } else {
                                        paleta2.escala += 0.25f;
                                        paleta2.UpdateCollisionBounds();
                                    }
                                    break;
                                case 6: //TamMenosPlayer
                                    if (color == 1) {
                                        paleta2.escala -= 0.25f;
                                        paleta2.UpdateCollisionBounds();
                                    } else {
                                        paleta1.escala -= 0.25f;
                                        paleta1.UpdateCollisionBounds();
                                    }
                                    break;
                                case 7: //InviPlayer
                                    if (color == 1) paleta2.Transparentar();
                                    else paleta1.Transparentar();
                                    break;
                            }
                            return true;
                        }
                        break;
                }
                return true;
            }

            return false;
        }

        ///<summary>
        /// Colocamos el powerup centrado entre las paletas y dentro de la altura de la pantalla.
        /// </summary>
        public void Colocar() {
            CenterAtLocation(new Vector2(random.Next(200, 600), random.Next(20 + texture.Width, 460 - texture.Width)));
        }


        ///<summary>
        /// Colocamos el powerup, fuera de la pantalla a una distancia prudente,
        /// porque no se van a usar, pero pueden usarse en el futuro.
        /// </summary>
        public void Descolocar() {
            Position.X = -500;
            Position.Y = -500;
        }

        #endregion
    }
}
