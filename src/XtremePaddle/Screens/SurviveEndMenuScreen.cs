using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace XtremePaddle
{
    /// <summary>
    /// Menu que se inicia al terminar una partida de supervivencia.
    /// </summary>
    class SurviveEndMenuScreen : MenuScreen
    {
        #region Variables

        // Objetos que daremos uso más adelante
        MenuEntryText highScoreMenuEntry;
        MenuEntryText highScoreMenuEntryShadow;
        MenuEntryObject winLoseMenuEntryObject;

        // Puntuación conseguida por el J1
        int scoreJ1;

        // Cargamos la puntuación más alta guardada en Settings
        static int puntMaxSuperv;

        // Sonido utilizado al reiniciar la partida
        SoundEffect soundNoPause;

        // Musica del menu por si se requiere salir del juego
        Song songMenu;

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        public SurviveEndMenuScreen(int scoreJ1)
            : base("GAME OVER")
        {
            // TODO: ¿Cambiar por CreateAd?
            //GameStateManagementGame.BannerAd.Visible = true;

            //La pantalla es un popup
            IsPopup = true;

            this.scoreJ1 = scoreJ1;
            
            // Entradas del Menu.
            MenuEntryObject surviveEndPage = new MenuEntryObject("screens/surviveEnd", Vector2.Zero);
            winLoseMenuEntryObject = new MenuEntryObject(string.Empty, new Vector2(300, 340), "hinchar");
            MenuEntryText scoreJ1MenuEntry = new MenuEntryText(CatStrings.scoreSurvMenu + scoreJ1, new Vector2(125, 115), false);
            MenuEntryText scoreJ1MenuEntryShadow = new MenuEntryText(CatStrings.scoreSurvMenu + scoreJ1, new Vector2(128, 117), Color.Black, false);
            highScoreMenuEntry = new MenuEntryText(string.Empty, new Vector2(125, 180), false);
            highScoreMenuEntryShadow = new MenuEntryText(string.Empty, new Vector2(128, 182), Color.Black, false);
            MenuEntryText restartMenuEntry = new MenuEntryText(CatStrings.restartWinMenu, new Vector2(590, 320), true);
            MenuEntryText restartMenuEntryShadow = new MenuEntryText(CatStrings.restartWinMenu, new Vector2(593, 322), Color.Black, true);
            MenuEntryText quitMenuEntry = new MenuEntryText(CatStrings.quitWinMenu, new Vector2(590, 370), true);
            MenuEntryText quitMenuEntryShadow = new MenuEntryText(CatStrings.quitWinMenu, new Vector2(593, 372), Color.Black, true);

            // Ponemos los strings vacios a su correspondiente valor
            SetMenuEntryText();

            // Eventos del menu al seleccionar.
            restartMenuEntry.Selected += restartMenuEntrySelected;
            quitMenuEntry.Selected += quitMenuEntrySelected;

            // Añadimos las entradas al Menu.
            MenuEntriesObject.Add(surviveEndPage);
            MenuEntriesObject.Add(winLoseMenuEntryObject);
            MenuEntriesText.Add(scoreJ1MenuEntryShadow);
            MenuEntriesText.Add(scoreJ1MenuEntry);
            MenuEntriesText.Add(highScoreMenuEntryShadow);
            MenuEntriesText.Add(highScoreMenuEntry);
            MenuEntriesText.Add(restartMenuEntryShadow);
            MenuEntriesText.Add(restartMenuEntry);
            MenuEntriesText.Add(quitMenuEntryShadow);
            MenuEntriesText.Add(quitMenuEntry);
        }

        /// <summary>
        /// Rellenamos los valores de las entradas si necesitan ser actualizadas.
        /// </summary>
        void SetMenuEntryText()
        {
            GameStateManagementGame.Settings.LoadAll();
            puntMaxSuperv = GameStateManagementGame.Settings.PuntMaxSuperv;
            if (scoreJ1 > puntMaxSuperv)
            {
                winLoseMenuEntryObject.TexName = "screens/supervLogos/goodJob";
                highScoreMenuEntry.Text = CatStrings.highScoreSurvMenu + scoreJ1;
                highScoreMenuEntryShadow.Text = CatStrings.highScoreSurvMenu + scoreJ1;
            }
            else
            {
                winLoseMenuEntryObject.TexName = "screens/supervLogos/badJob";
                highScoreMenuEntry.Text = CatStrings.highScoreSurvMenu + puntMaxSuperv;
                highScoreMenuEntryShadow.Text = CatStrings.highScoreSurvMenu + puntMaxSuperv;
            }
        }

        /// <summary>
        /// Cargamos el contenido necesario para el menu de ganadores
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
        /// Evento que surge al presionar el boton reiniciar/restart.
        /// </summary>
        void restartMenuEntrySelected(object sender, EventArgs e)
        {
            if (scoreJ1 > puntMaxSuperv)
            {
                GameStateManagementGame.Settings.Save("PuntMaxSuperv", scoreJ1);
                GameStateManagementGame.Settings.LoadAll();
            }
            LoadingScreen.Load(ScreenManager, true, new SurviveScreen());
            GameStateManagementGame.MusicManager.Play(soundNoPause);
            ExitScreen();
        }

        /// <summary>
        /// Evento que surge al presionar el boton quit/salir.
        /// </summary>
        void quitMenuEntrySelected(object sender, EventArgs e)
        {
            if (scoreJ1 > puntMaxSuperv)
            {
                GameStateManagementGame.Settings.Save("PuntMaxSuperv", scoreJ1);
                GameStateManagementGame.Settings.LoadAll();
            }
            GameStateManagementGame.MusicManager.Stop();
            GameStateManagementGame.MusicManager.Play(songMenu);
            GameStateManagementGame.Aros.Visible = true;
            LoadingScreen.Load(ScreenManager, false, new BackgroundScreen("background"), new MainMenuScreen());
        }

        /// <summary>
        /// Al dar al boton atras, salimos de la partida.
        /// </summary>
        protected override void OnCancel()
        {
            if (scoreJ1 > puntMaxSuperv)
            {
                GameStateManagementGame.Settings.Save("PuntMaxSuperv", scoreJ1);
                GameStateManagementGame.Settings.LoadAll();
            }
            GameStateManagementGame.MusicManager.Stop();
            GameStateManagementGame.MusicManager.Play(songMenu);
            GameStateManagementGame.Aros.Visible = true;
            LoadingScreen.Load(ScreenManager, false, new BackgroundScreen("background"), new MainMenuScreen());
        }

        #endregion
    }
}
