using System;
using Microsoft.Xna.Framework;
using Microsoft.Phone.Tasks;

namespace XtremePaddle
{
    /// <summary>
    /// La pantalla MainMenu, es la primera cosa que se muestra al iniciar el juego
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Variables

        ShareLinkTask sharetask = new ShareLinkTask();
        
        
        MenuEntryObject shareEntryObject;
        MenuEntryObject moreEntryObject;
        MenuEntryObject aboutEntryObject;
        MenuEntryObject musicEntryObject;
        MenuEntryObject vibrationEntryObject;
        MenuEntryObject soundEntryObject;

        bool musicBool = GameStateManagementGame.Settings.Music;
        bool vibrationBool = GameStateManagementGame.Settings.Vibration;
        bool soundBool = GameStateManagementGame.Settings.Sound;

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainMenuScreen()
            : base("")
        { //No queremos que tenga un titulos
                
            // Programamos los links para compartir el juego
            sharetask.LinkUri = new Uri("http://www.windowsphone.com/es-ES/apps/592edbc4-9039-4e7d-a43e-46721c7716cf");
            sharetask.Message = CatStrings.ShareMessage;
            sharetask.Title = CatStrings.ShareTittle;

            // Entradas del Menu.
            MenuEntryObject mainPage = new MenuEntryObject("screens/mainPage", Vector2.Zero);
            MenuEntryObject unPlayerEntryObject = new MenuEntryObject("screens/mainPage/player", new Vector2(200, 252));
            MenuEntryObject dosPlayersEntryObject = new MenuEntryObject("screens/mainPage/players", new Vector2(345, 252));
            MenuEntryObject achievementsEntryObject = new MenuEntryObject("screens/mainPage/achievements", new Vector2(500, 252));
            shareEntryObject = new MenuEntryObject("screens/mainPage/shareButton", new Vector2(730, 410));
            moreEntryObject = new MenuEntryObject("screens/mainPage/moreClose", new Vector2(0, 410));
            aboutEntryObject = new MenuEntryObject("null", new Vector2(4, 340));
            musicEntryObject = new MenuEntryObject("null", new Vector2(4, 270));
            vibrationEntryObject = new MenuEntryObject("null", new Vector2(4, 200));
            soundEntryObject = new MenuEntryObject("null", new Vector2(4, 130));

            // Eventos del menu al seleccionar.
            unPlayerEntryObject.Selected += UnPlayerObjectSelected;
            dosPlayersEntryObject.Selected += DosPlayersObjectSelected;
            achievementsEntryObject.Selected += AchievementsObjectSelected;
            shareEntryObject.Selected += ShareEntryObjectSelected;
            moreEntryObject.Selected += MoreEntryObjectSelected;
            aboutEntryObject.Selected += AboutEntrySelected;
            musicEntryObject.Selected += MusicEntrySelected;
            vibrationEntryObject.Selected += VibrationEntrySelected;
            soundEntryObject.Selected += SoundEntrySelected;

            // Añadimos las entradas al Menu.
            MenuEntriesObject.Add(mainPage);
            MenuEntriesObject.Add(unPlayerEntryObject);
            MenuEntriesObject.Add(dosPlayersEntryObject);
            MenuEntriesObject.Add(achievementsEntryObject);
            MenuEntriesObject.Add(shareEntryObject);
            MenuEntriesObject.Add(moreEntryObject);
            MenuEntriesObject.Add(aboutEntryObject);
            MenuEntriesObject.Add(musicEntryObject);
            MenuEntriesObject.Add(vibrationEntryObject);
            MenuEntriesObject.Add(soundEntryObject);
        }

        #endregion

        #region Entradas

        /// <summary>
        /// Evento que surge al seleccionar jugar a 1 Jugador.
        /// </summary>
        void UnPlayerObjectSelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new Options1MenuScreen());
        }

        /// <summary>
        /// Evento que surge al seleccionar jugar a 2 Jugadores.
        /// </summary>
        void DosPlayersObjectSelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new Options2MenuScreen());
        }

        /// <summary>
        /// Evento que surge al seleccionar logros/achievements.
        /// </summary>
        void AchievementsObjectSelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new PopupBoxScreen(CatStrings.comingAchiev));
        }

        /// <summary>
        /// Evento que surge al seleccionar share.
        /// </summary>
        void ShareEntryObjectSelected(object sender, EventArgs e)
        {
            sharetask.Show();
        }

        /// <summary>
        /// Evento que surge al seleccionar more.
        /// </summary>
        void MoreEntryObjectSelected(object sender, EventArgs e)
        {
            if (moreEntryObject.TexName == "screens/mainPage/moreClose")
            {
                // Cargamos la configuración y todos los datos necesarios.
                GameStateManagementGame.Settings.LoadAll();
                musicBool = GameStateManagementGame.Settings.Music;
                vibrationBool = GameStateManagementGame.Settings.Vibration;
                soundBool = GameStateManagementGame.Settings.Sound;

                moreEntryObject.TexName = "screens/mainPage/moreOpen";
                aboutEntryObject.TexName = "screens/mainPage/aboutButton";
                musicEntryObject.TexName = musicBool ? "screens/mainPage/musicButton" : "screens/mainPage/noMusicButton";
                vibrationEntryObject.TexName = vibrationBool ? "screens/mainPage/vibrationButton" : "screens/mainPage/noVibrationButton";
                soundEntryObject.TexName = soundBool ? "screens/mainPage/soundButton" : "screens/mainPage/noSoundButton";
            }
            else
            {
                moreEntryObject.TexName = "screens/mainPage/moreClose";
                aboutEntryObject.TexName = "null";
                musicEntryObject.TexName = "null";
                vibrationEntryObject.TexName = "null";
                soundEntryObject.TexName = "null";
            }
        }

        /// <summary>
        /// Evento que surge al seleccionar about/acerca de.
        /// </summary>
        void AboutEntrySelected(object sender, EventArgs e)
        {
            // Cerramos more para que quede bonito.
            moreEntryObject.TexName = "screens/mainPage/moreClose";
            aboutEntryObject.TexName = "null";
            musicEntryObject.TexName = "null";
            vibrationEntryObject.TexName = "null";
            soundEntryObject.TexName = "null";
            ScreenManager.AddScreen(new AboutMenuScreen());
        }

        /// <summary>
        /// Evento que surge al seleccionar el boton musica.
        /// </summary>
        void MusicEntrySelected(object sender, EventArgs e)
        {
            musicBool = !musicBool;

            // Actualizamos la textura y guardamos
            musicEntryObject.TexName = musicBool ? "screens/mainPage/musicButton" : "screens/mainPage/noMusicButton";
            GameStateManagementGame.Settings.Save("Music", musicBool);
            GameStateManagementGame.Settings.LoadAll();
        }

        /// <summary>
        /// Evento que surge al seleccionar el boton vibracion.
        /// </summary>
        void VibrationEntrySelected(object sender, EventArgs e)
        {
            vibrationBool = !vibrationBool;

            // Actualizamos la textura y guardamos
            vibrationEntryObject.TexName = vibrationBool ? "screens/mainPage/vibrationButton" : "screens/mainPage/noVibrationButton";
            GameStateManagementGame.Settings.Save("Vibration", vibrationBool);
            GameStateManagementGame.Settings.LoadAll();
        }

        /// <summary>
        /// Evento que surge al seleccionar el boton sonido.
        /// </summary>
        void SoundEntrySelected(object sender, EventArgs e)
        {
            soundBool = !soundBool;

            // Actualizamos la textura y guardamos
            soundEntryObject.TexName = soundBool ? "screens/mainPage/soundButton" : "screens/mainPage/noSoundButton";
            GameStateManagementGame.Settings.Save("Sound", soundBool);
            GameStateManagementGame.Settings.LoadAll();
        }

        /// <summary>
        /// Al dar al boton atras, salimos del juego.
        /// </summary>
        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
        }

        #endregion
    }
}