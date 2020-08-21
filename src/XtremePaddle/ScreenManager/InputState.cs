using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace XtremePaddle {
    /// <summary>
    /// Ayudante que lee la entrada tactil y del teclado. Esta clase  
    /// traza entre el estado actual y pasado de los dispositivos de entrada.
    /// </summary>
    public class InputState {
        #region Variables

        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        public InputState() { }

        #endregion

        #region Metodos

        /// <summary>
        /// Lee el último estado de la entrada.
        /// </summary>
        public void Update() {
            TouchState = TouchPanel.GetState();

            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable) {
                Gestures.Add(TouchPanel.ReadGesture());
            }
        }

        /// <summary>
        /// Comprueba si se ha realizado un "click en atras".
        /// </summary>
        public bool IsMenuCancel() {
            return GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed;
        }

        #endregion
    }
}
