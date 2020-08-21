using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace XtremePaddle {
    /// <summary>
    /// Menu que se inicia al terminar una partida con uno u otro ganador.
    /// </summary>
    class WinnerMenuScreen : MenuScreen {
        #region Variables

        // Quien ha ganado: true = J1, false = J2/IA
        bool ganador;

        // Dificultad por si se requiere reiniciar la partida
        int dificultad;

        // Sonido utilizado al reiniciar la partida
        SoundEffect soundNoPause;

        // Musica del menu por si se requiere salir del juego
        Song songMenu;

        //Objetos y Textos a los que daremos uso
        MenuEntryObject winnerPage;
        MenuEntryText leftMenuEntry;
        MenuEntryText leftMenuEntryShadow;
        MenuEntryText rightMenuEntry;
        MenuEntryText rightMenuEntryShadow;

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        public WinnerMenuScreen(bool ganador, int dificultad)
            : base("") {    // No queremos titulo.

            //La pantalla es un popup
            IsPopup = true;

            // TODO: ¿Cambiar por CreateAd?
            //GameStateManagementGame.BannerAd.Visible = true;

            this.ganador = ganador;
            this.dificultad = dificultad;

            // Entradas del Menu.
            winnerPage = new MenuEntryObject(string.Empty, Vector2.Zero);
            MenuEntryText restartMenuEntry = new MenuEntryText(CatStrings.restartWinMenu, new Vector2(400, 150), true);
            MenuEntryText restartMenuEntryShadow = new MenuEntryText(CatStrings.restartWinMenu, new Vector2(403, 152), Color.Black, true);
            MenuEntryText quitMenuEntry = new MenuEntryText(CatStrings.quitWinMenu, new Vector2(400, 340), true);
            MenuEntryText quitMenuEntryShadow = new MenuEntryText(CatStrings.quitWinMenu, new Vector2(403, 342), Color.Black, true);
            leftMenuEntry = new MenuEntryText(string.Empty, new Vector2(335, 140), Color.White, "winnerfont", true, 90f);
            leftMenuEntryShadow = new MenuEntryText(string.Empty, new Vector2(337, 143), Color.Black, "winnerfont", true, 90f);
            rightMenuEntry = new MenuEntryText(string.Empty, new Vector2(665, 400), Color.White, "winnerfont", true, 270f);
            rightMenuEntryShadow = new MenuEntryText(string.Empty, new Vector2(667, 403), Color.Black, "winnerfont", true, 270f);

            // Ponemos los strings vacios a su correspondiente valor
            SetMenuEntryText();

            // Eventos del menu al seleccionar.
            restartMenuEntry.Selected += restartMenuEntrySelected;
            quitMenuEntry.Selected += quitMenuEntrySelected;

            // Añadimos las entradas al Menu.
            MenuEntriesObject.Add(winnerPage);
            MenuEntriesText.Add(restartMenuEntryShadow);
            MenuEntriesText.Add(restartMenuEntry);
            MenuEntriesText.Add(quitMenuEntryShadow);
            MenuEntriesText.Add(quitMenuEntry);
            MenuEntriesText.Add(leftMenuEntryShadow);
            MenuEntriesText.Add(leftMenuEntry);
            MenuEntriesText.Add(rightMenuEntryShadow);
            MenuEntriesText.Add(rightMenuEntry);
        }

        /// <summary>
        /// Rellenamos los valores de las entradas si necesitan ser actualizadas.
        /// </summary>
        void SetMenuEntryText() {
            winnerPage.TexName = ganador ? "screens/ganaJ1" : "screens/ganaJ2";
            leftMenuEntry.Text = ganador ? CatStrings.loseText : CatStrings.winText;
            leftMenuEntryShadow.Text = ganador ? CatStrings.loseText : CatStrings.winText;
            rightMenuEntry.Text = ganador ? CatStrings.winText : CatStrings.loseText;
            rightMenuEntryShadow.Text = ganador ? CatStrings.winText : CatStrings.loseText;
        }

        /// <summary>
        /// Cargamos el contenido necesaio para el menu de ganadores
        /// </summary>
        public override void LoadContent() {
            ContentManager content = ScreenManager.Game.Content;

            soundNoPause = content.Load<SoundEffect>("sounds/noPause");

            songMenu = content.Load<Song>("songs/menu");
        }

        #endregion

        #region Entradas


        /// <summary>
        /// Evento que surge al presionar el boton reiniciar/restart.
        /// </summary>
        void restartMenuEntrySelected(object sender, EventArgs e) {
            LoadingScreen.Load(ScreenManager, true, new UnPlayerScreen(dificultad));
            GameStateManagementGame.MusicManager.Play(soundNoPause);
            ExitScreen();
        }

        /// <summary>
        /// Evento que surge al presionar el boton quit/salir.
        /// </summary>
        void quitMenuEntrySelected(object sender, EventArgs e) {
            GameStateManagementGame.MusicManager.Stop();
            GameStateManagementGame.MusicManager.Play(songMenu);
            GameStateManagementGame.Aros.Visible = true;
            LoadingScreen.Load(ScreenManager, false, new BackgroundScreen("background"), new MainMenuScreen());
        }

        /// <summary>
        /// Al dar al boton atras, salimos de la partida.
        /// </summary>
        protected override void OnCancel() {
            GameStateManagementGame.MusicManager.Stop();
            GameStateManagementGame.MusicManager.Play(songMenu);
            GameStateManagementGame.Aros.Visible = true;
            LoadingScreen.Load(ScreenManager, false, new BackgroundScreen("background"), new MainMenuScreen());
        }

        #endregion
    }
}
