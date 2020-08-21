// NO SE USA; POR SI ACASO LO VAMOS A USAR

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace XtremePaddle
{
    /// <summary>
    /// Una pantalla Popup para mostrar el menu pausa con
    /// opciones varias y boton salir y resumir partida.
    /// </summary>
    class SeguroSalirScreen : MenuScreen
    {
        #region Variables

        // Musica del menu inicializada si salimos de la partida
        Song songMenu;

        // Sonido a reproducir al salir del menu pausa
        SoundEffect soundNoPause;

        //Objetos y Textos a los que daremos uso
        MenuEntryObject music;
        MenuEntryObject sound;
        MenuEntryObject vibration;

        // boobleanos para cambiar botones a sus contrarios
        bool musicBool = GameStateManagementGame.Settings.Music;
        bool soundBool = GameStateManagementGame.Settings.Sound;
        bool vibrationBool = GameStateManagementGame.Settings.Vibration;

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        public SeguroSalirScreen()
            : base(CatStrings.pausedTittle)
        {

            //La pantalla es un popup
            IsPopup = true;

            // TODO: ¿Cambiar por CreateAd?
            //GameStateManagementGame.BannerAd.Visible = true;

            // Entradas del Menu.
            MenuEntryObject pausePage = new MenuEntryObject("screens/pausePage", Vector2.Zero);
            music = new MenuEntryObject(string.Empty, new Vector2(215, 205));
            sound = new MenuEntryObject(string.Empty, new Vector2(345, 205));
            vibration = new MenuEntryObject(string.Empty, new Vector2(475, 205));
            MenuEntryText resumeMenuEntry = new MenuEntryText(CatStrings.resumeMenu, new Vector2(400, 165), true);
            MenuEntryText resumeMenuEntryShadow = new MenuEntryText(CatStrings.resumeMenu, new Vector2(403, 168), Color.Black, true);
            MenuEntryText quitMenuEntry = new MenuEntryText(CatStrings.quitMenu, new Vector2(400, 350), true);
            MenuEntryText quitMenuEntryShadow = new MenuEntryText(CatStrings.quitMenu, new Vector2(403, 353), Color.Black, true);

            // Ponemos los strings vacios a su correspondiente valor
            SetMenuEntryText();

            // Eventos del menu al seleccionar.
            music.Selected += MusicSelected;
            sound.Selected += SoundSelected;
            vibration.Selected += VibrationSelected;
            resumeMenuEntry.Selected += resumeMenuEntrySelected;
            quitMenuEntry.Selected += quitMenuEntrySelected;

            // Añadimos las entradas al Menu.
            MenuEntriesObject.Add(pausePage);
            MenuEntriesObject.Add(music);
            MenuEntriesObject.Add(sound);
            MenuEntriesObject.Add(vibration);
            MenuEntriesText.Add(resumeMenuEntryShadow);
            MenuEntriesText.Add(resumeMenuEntry);
            MenuEntriesText.Add(quitMenuEntryShadow);
            MenuEntriesText.Add(quitMenuEntry);
        }

        /// <summary>
        /// Rellenamos los valores de las entradas si necesitan ser actualizadas.
        /// </summary>
        void SetMenuEntryText()
        {
            music.TexName = musicBool ? "screens/options/music" : "screens/options/noMusic";
            sound.TexName = soundBool ? "screens/options/sound" : "screens/options/noSound";
            vibration.TexName = vibrationBool ? "screens/options/vibration" : "screens/options/noVibration";
        }

        /// <summary>
        /// Cargamos el contenido necesaio para el menu de pausa
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            soundNoPause = content.Load<SoundEffect>("sounds/noPause");

            songMenu = content.Load<Song>("songs/menu");
        }
        #endregion

        #region Entradas

        /// <summary>
        /// Evento que surge al presionar el boton resumir/resume.
        /// </summary>
        void resumeMenuEntrySelected(object sender, EventArgs e)
        {
            GameStateManagementGame.MusicManager.Play(soundNoPause);
            ExitScreen();
        }

        /// <summary>
        /// Evento que surge al presionar el boton quit/salir.
        /// </summary>
        void quitMenuEntrySelected(object sender, EventArgs e)
        {
            GameStateManagementGame.MusicManager.Play(soundNoPause);
            GameStateManagementGame.MusicManager.Stop();
            GameStateManagementGame.MusicManager.Play(songMenu);
            GameStateManagementGame.Aros.Visible = true;
            LoadingScreen.Load(ScreenManager, false, new BackgroundScreen("background"), new MainMenuScreen());
        }

        /// <summary>
        /// Evento que surge al seleccionar el boton de musica.
        /// </summary>
        void MusicSelected(object sender, EventArgs e)
        {
            musicBool = !musicBool;
            GameStateManagementGame.Settings.Save("Music", musicBool);
            GameStateManagementGame.Settings.LoadAll();
            SetMenuEntryText();
        }

        /// <summary>
        /// Evento que surge al seleccionar el boton de sonido.
        /// </summary>
        void SoundSelected(object sender, EventArgs e)
        {
            soundBool = !soundBool;
            GameStateManagementGame.Settings.Save("Sound", soundBool);
            GameStateManagementGame.Settings.LoadAll();
            SetMenuEntryText();
        }

        /// <summary>
        /// Evento que surge al seleccionar el boton de vibracion.
        /// </summary>
        void VibrationSelected(object sender, EventArgs e)
        {
            vibrationBool = !vibrationBool;
            GameStateManagementGame.Settings.Save("Vibration", vibrationBool);
            GameStateManagementGame.Settings.LoadAll();
            SetMenuEntryText();
        }

        #endregion
    }
}
