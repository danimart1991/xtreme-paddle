using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace XtremePaddle {
    /// <summary>
    /// La clase Settings maneja la carga y guardado de las opciones del juego.
    /// </summary>
    public class Settings {
        #region Atributos

        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        /// <summary>
        /// Numero de tema usado en el juego.
        /// </summary>
        public int Theme = 0;

        /// <summary>
        /// Puntuación máxima de supervivencia
        /// </summary>
        public int PuntMaxSuperv = 0;

        /// <summary>
        /// Si queremos usar o no el modo extremo;
        /// </summary>
        public bool Xtreme = true;

        /// <summary>
        /// Si queremos reproducir musica;
        /// </summary>
        public bool Music = true;

        /// <summary>
        /// Si queremos que el juego vibre;
        /// </summary>
        public bool Vibration = true;

        /// <summary>
        /// Si queremos reproducir efectos sonoros;
        /// </summary>
        public bool Sound = true;

        /// <summary>
        /// El color de la paleta del Jugador1 y su numero;
        /// </summary>
        public Color PaddleJ1Color = Color.White;
        public int PaddleJ1ColorInt = 0;

        /// <summary>
        /// El color de la paleta del Jugador2 y su numero;
        /// </summary>
        public Color PaddleJ2Color = Color.White;
        public int PaddleJ2ColorInt = 0;

        /// <summary>
        /// El color de la bola y su numero;
        /// </summary>
        public Color BallColor = Color.White;
        public int BallColorInt = 0;

        #endregion

        #region Carga/Guardado

        /// <summary>
        /// Guarda un valor en la configuracion
        /// </summary>
        /// <param name="dato">Clave a guardar en el archivo</param>
        /// <param name="valor">Valor a guardar en la clave</param>
        public void Save(string dato, object valor) {
            // Si no esta el dato lo creamos, si está lo sobrescribimos
            if (!settings.Contains(dato)) {
                settings.Add(dato, valor);
            } else {
                settings[dato] = valor;
            }

            //Guardamos el archivo de configuración
            settings.Save();
        }

        /// <summary>
        /// Carga todos los datos del archivo de configuración
        /// </summary>
        public void LoadAll() {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("Theme")) {
                int? a = IsolatedStorageSettings.ApplicationSettings["Theme"] as int?;
                Theme = a.HasValue ? a.Value : 0;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("Xtreme")) {
                bool? b = IsolatedStorageSettings.ApplicationSettings["Xtreme"] as bool?;
                Xtreme = b.HasValue ? b.Value : true;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("Music")) {
                bool? d = IsolatedStorageSettings.ApplicationSettings["Music"] as bool?;
                Music = d.HasValue ? d.Value : true;
                MediaPlayer.Volume = Music ? 100 : 0;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("Vibration")) {
                bool? c = IsolatedStorageSettings.ApplicationSettings["Vibration"] as bool?;
                Vibration = c.HasValue ? c.Value : true;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("Sound")) {
                bool? d = IsolatedStorageSettings.ApplicationSettings["Sound"] as bool?;
                Sound = d.HasValue ? d.Value : true;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("PaddleJ1Color")) {
                Color? e = IsolatedStorageSettings.ApplicationSettings["PaddleJ1Color"] as Color?;
                PaddleJ1Color = e.HasValue ? e.Value : Color.White;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("PaddleJ1ColorInt")) {
                int? f = IsolatedStorageSettings.ApplicationSettings["PaddleJ1ColorInt"] as int?;
                PaddleJ1ColorInt = f.HasValue ? f.Value : 0;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("PaddleJ2Color")) {
                Color? g = IsolatedStorageSettings.ApplicationSettings["PaddleJ2Color"] as Color?;
                PaddleJ2Color = g.HasValue ? g.Value : Color.White;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("PaddleJ2ColorInt")) {
                int? h = IsolatedStorageSettings.ApplicationSettings["PaddleJ2ColorInt"] as int?;
                PaddleJ2ColorInt = h.HasValue ? h.Value : 0;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("BallColor")) {
                Color? g = IsolatedStorageSettings.ApplicationSettings["BallColor"] as Color?;
                BallColor = g.HasValue ? g.Value : Color.White;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("BallColorInt")) {
                int? h = IsolatedStorageSettings.ApplicationSettings["BallColorInt"] as int?;
                BallColorInt = h.HasValue ? h.Value : 0;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("PuntMaxSuperv"))
            {
                int? z = IsolatedStorageSettings.ApplicationSettings["PuntMaxSuperv"] as int?;
                PuntMaxSuperv = z.HasValue ? z.Value : 0;
            }
        }

        #endregion
    }
}
