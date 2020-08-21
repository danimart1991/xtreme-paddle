using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace XtremePaddle {
    /// <summary>
    /// Enumeración que describe los estados de las pantallas.
    /// </summary>
    public enum ScreenState {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }


    /// <summary>
    /// Una pantalla es una capa unica que tiene logica update y draw, y que
    /// puede ser combinado con otras capas para construir un sistema de menus complejo.
    /// Por ejemplo el menu principal, opciones, el juego en si, acerca de... son todo
    /// implementaciones de pantallas.
    /// </summary>
    public abstract class GameScreen {
        #region Atributos


        /// <summary>
        /// Normalmente cuando una pantalla es traida encima de otra,
        /// la primera debe desvanecerse a oscuro, para hacer hueco a la
        /// nueva. Esta propiedad indica si nuestra pantalla es un simple
        /// Popup, en este caso la pantalla de debajo no debe hacer nada.
        /// </summary>
        public bool IsPopup {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        bool isPopup = false;


        /// <summary>
        /// Indica cuanto tiempo la pantalla debe hacer
        /// la transicion en el caso de ser activada.
        /// </summary>
        public TimeSpan TransitionOnTime {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        TimeSpan transitionOnTime = TimeSpan.Zero;


        /// <summary>
        /// Indica cuanto tiempo la pantalla debe hacer
        /// la transicion en el caso de ser desactivada.
        /// </summary>
        public TimeSpan TransitionOffTime {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        TimeSpan transitionOffTime = TimeSpan.Zero;


        /// <summary>
        /// Obtiene la posicion actual de la transicion de la pantalla, entre
        /// cero (activa totalmente, sin transicion) a uno (con la transicion
        /// completa).
        /// </summary>
        public float TransitionPosition {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        float transitionPosition = 1;


        /// <summary>
        /// Obtiene la transparencia actual de la transicion de la pantalla,
        /// entre cero (activa totalmente, sin transicion) a uno (con la
        /// transicion completa).
        /// </summary>
        public float TransitionAlpha {
            get { return 1f - TransitionPosition; }
        }


        /// <summary>
        /// Obtiene el estado actual de la transicion de la pantalla.
        /// </summary>
        public ScreenState ScreenState {
            get { return screenState; }
            protected set { screenState = value; }
        }

        ScreenState screenState = ScreenState.TransitionOn;


        /// <summary>
        /// Hay dos posibles razones por las que la pantalla debe cargar una
        /// transicion. Puede desaparecer temporalmante para dejar sitio a otra
        /// pantalla que este por encima de el, o puede desaparecer por que quiere.
        /// Esta propiedad indica si la pantalla debe desaparecer de verdad:
        /// si esta activo, la pantalla debe automaticamente eliminarse, tan pronto
        /// como la transicion finalice.
        /// </summary>
        public bool IsExiting {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        bool isExiting = false;


        /// <summary>
        /// Comprueba si la pantalla esta activa y responde a la entrada.
        /// </summary>
        public bool IsActive {
            get {
                return !otherScreenHasFocus &&
                       (screenState == ScreenState.TransitionOn ||
                        screenState == ScreenState.Active);
            }
        }

        bool otherScreenHasFocus;


        /// <summary>
        /// Obtiene el screen manager al que pertenece la pantalla.
        /// </summary>
        public ScreenManager ScreenManager {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        ScreenManager screenManager;

        /// <summary>
        /// Obtiene los gestos de la pantalla en la que estamos interesados. Las
        /// pantallas deben sermtan especificos como sea posible con los gestos 
        /// para incrementar la precision del sistema de gestos.
        /// Por ejemplo, la mayoria de los menus solo necesitan un Tap o quizas
        /// Tap y VerticalDrag para funcionar. Estos gestos son manejados por el
        /// ScreenManager cuando la pantalla cambia y todos los gestos son 
        /// almacenados en el InputState pasados al metodo HandleInput
        /// </summary>
        public GestureType EnabledGestures {
            get { return enabledGestures; }
            protected set {
                enabledGestures = value;

                // el screen manager controla esto durante los cambios de pantalla,
                // pero si la pantalla esta activa y los tipos de gestos estan cambiando,
                // necesitamos actualizar el TouchPanel por nosotros mismos.
                if (ScreenState == ScreenState.Active) {
                    TouchPanel.EnabledGestures = value;
                }
            }
        }

        GestureType enabledGestures = GestureType.None;

        /// <summary>
        /// Indica si la pantalla es serializable o no. Si es cierto,
        /// la pantalla sera guardada en el estado del screen manager y
        /// sus metodos Serialize y Deserialize seran llamados cuando corresponda.
        /// Si esto es falso, la pantalla sera ignorada durante la serializacion.
        /// Por defecto, se asume que todas las pantallas son serializables.
        /// </summary>
        public bool IsSerializable {
            get { return isSerializable; }
            protected set { isSerializable = value; }
        }

        bool isSerializable = true;

        #endregion

        #region Inicializacion


        /// <summary>
        /// Carga el contenido necesario de la pantalla.
        /// </summary>
        public virtual void LoadContent() { }


        /// <summary>
        /// Descarga el contenido necesario de la pantalla.
        /// </summary>
        public virtual void UnloadContent() { }


        #endregion

        #region Update y Draw


        /// <summary>
        /// Permite a la pantalla cargar logica, como actualizar la posicion de la
        /// transicion. Al contrario que HandleInput, este metodo es llamado sin tener
        /// en cuenta si la pantalla esta activa, escondida, o en medio de una transicion.
        /// </summary>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                      bool coveredByOtherScreen) {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (isExiting) {
                // Si la pantalla va a morir, la apagamos.
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionOffTime, 1)) {
                    //Cuando la transicion acaba, quitamos la pantalla.
                    ScreenManager.RemoveScreen(this);
                }
            } else if (coveredByOtherScreen) {
                // Si la pantalla esta cubierta por otra la apagamos.
                if (UpdateTransition(gameTime, transitionOffTime, 1)) {
                    // Ocupados en una transicion.
                    screenState = ScreenState.TransitionOff;
                } else {
                    // ¡Transicion acabada!
                    screenState = ScreenState.Hidden;
                }
            } else {
                // En otro caso la pantalla debe encenderse y ser activada.
                if (UpdateTransition(gameTime, transitionOnTime, -1)) {
                    // Ocupados en una transicion.
                    screenState = ScreenState.TransitionOn;
                } else {
                    // ¡Transicion acabada!
                    screenState = ScreenState.Active;
                }
            }
        }


        /// <summary>
        /// Ayudante para actualizar la posicion de la transicion.
        /// </summary>
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction) {
            // ¿Cuanto debe moverse?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Actualizamos la posicion de la transicion.
            transitionPosition += transitionDelta * direction;

            // ¿Hemos alcanzado el final de la transicion?
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1))) {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            // En otro caso, estamos todavia ocupados transicionando.
            return true;
        }


        /// <summary>
        /// Permite a la pantalla la entrada del usuario. A diferencia de
        /// Update este metodo solo es llamado cuando la pantalla esta
        /// activa, y no cuando cualqueir otra esta enfocada (por encima).
        /// </summary>
        public virtual void HandleInput(InputState input) { }


        /// <summary>
        /// Llamado cuando la pantalla debe dibujarse a si misma.
        /// </summary>
        public virtual void Draw(GameTime gameTime) { }


        #endregion

        #region Metodos

        /// <summary>
        /// Pide a la pantalla que serialice su estado del stream dado.
        /// </summary>
        /// <param name="stream">Stream del que queremos sealizar</param>
        public virtual void Serialize(Stream stream) { }

        /// <summary>
        /// Pide a la pantalla que deserialice su estado del stream dado.
        /// </summary>
        /// <param name="stream">Stream del que queremos desealizar</param>
        public virtual void Deserialize(Stream stream) { }

        /// <summary>
        /// Pide a la pantalla que muera. Al contrario que ScreenManager.RemoveScreen,
        /// el cual mata instantaneamente a la pantalla, este metodo respeta los tiempos
        /// de transicion y da a la pantalla la opción de desactivar la transicion.
        /// </summary>
        public void ExitScreen() {
            if (TransitionOffTime == TimeSpan.Zero) {
                // Si la pantalla tiene un tiempoDeTransicion=0, la matamos instantaneamente.
                ScreenManager.RemoveScreen(this);
            } else {
                // En el otro caso, activamos el flag de salida y salimos.
                isExiting = true;
            }
        }

        #endregion
    }
}
