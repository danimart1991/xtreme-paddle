using System;
using Microsoft.Xna.Framework;

namespace XtremePaddle
{
    /// <summary>
    /// La pantalla de opciones es un menu con la configuracion del juego
    /// casi en su totalidad. Brinda la oportunidad al jugador, de personalizar
    /// su experiencia a la hora de jugar.
    /// </summary>
    class Options2MenuScreen : MenuScreen
    {
        #region Variables

        // Dificultad en la que vamos a jugar
        int difficult = 1;

        //Objetos y Textos a los que daremos uso
        MenuEntryText temasMenuEntry;
        MenuEntryText temasMenuEntryShadow;
        MenuEntryObject miniTema;
        MenuEntryObject offXtreme;
        MenuEntryObject onXtreme;
        MenuEntryObject xtremeMode;
        MenuEntryObject currentDifficult;
        MenuEntryObject difEasyMenuObject;
        MenuEntryObject difMedMenuObject;
        MenuEntryObject difHardMenuObject;
        MenuEntryObject difAscMenuObject;
        MenuEntryObject currentPaddleJ1;
        MenuEntryObject currentPaddleJ2;
        MenuEntryObject currentBall;
        MenuEntryObject[] arrayaux = new MenuEntryObject[1];
        MenuEntryObject[] paddlesJ1 = new MenuEntryObject[5];
        MenuEntryObject[] paddlesJ2 = new MenuEntryObject[5];
        MenuEntryObject[] balls = new MenuEntryObject[5];

        // Cargamos datos de Settings del juego
        static string[] temas = { CatStrings.classicOptions, CatStrings.glowOptions, 
                                    CatStrings.sketchOptions, CatStrings.soccerOptions, CatStrings.tennisOptions };
        static bool xtremeBool = GameStateManagementGame.Settings.Xtreme;
        static int currentTema = GameStateManagementGame.Settings.Theme;
        static Color colorPaddleJ1 = GameStateManagementGame.Settings.PaddleJ1Color;
        static int colorPaddleJ1Int = GameStateManagementGame.Settings.PaddleJ1ColorInt;
        static Color colorPaddleJ2 = GameStateManagementGame.Settings.PaddleJ2Color;
        static int colorPaddleJ2Int = GameStateManagementGame.Settings.PaddleJ2ColorInt;
        static Color colorBall = GameStateManagementGame.Settings.BallColor;
        static int colorBallInt = GameStateManagementGame.Settings.BallColorInt;

        #endregion

        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Options2MenuScreen()
            : base(CatStrings.options2Tittle)
        {

            // Entradas del Menu.
            MenuEntryObject optionsPage = new MenuEntryObject("screens/optionsPage", Vector2.Zero);
            MenuEntryText playMenuEntry = new MenuEntryText(CatStrings.playOptions, new Vector2(730, 450), "gamefont", true);
            MenuEntryText playMenuEntryShadow = new MenuEntryText(CatStrings.playOptions, new Vector2(730, 450), Color.Black, "gamefont", true);
            MenuEntryText difficultyMenuEntry = new MenuEntryText(CatStrings.difficultyOptions, new Vector2(40, 80), "aboutfont", false);
            MenuEntryText difficultyMenuEntryShadow = new MenuEntryText(CatStrings.difficultyOptions, new Vector2(42, 83), Color.Black, "aboutfont", false);
            difEasyMenuObject = new MenuEntryObject("screens/options/difEasy", new Vector2(45, 120));
            difMedMenuObject = new MenuEntryObject("screens/options/difMed", new Vector2(130, 120));
            difHardMenuObject = new MenuEntryObject("screens/options/difHard", new Vector2(215, 120));
            difAscMenuObject = new MenuEntryObject("screens/options/difAsc", new Vector2(310, 120));
            MenuEntryText paddleJ1MenuEntry = new MenuEntryText(CatStrings.playerOptions, new Vector2(415, 80), "aboutfont", false);
            MenuEntryText paddleJ1MenuEntryShadow = new MenuEntryText(CatStrings.playerOptions, new Vector2(417, 83), Color.Black, "aboutfont", false);
            MenuEntryText paddleJ2MenuEntry = new MenuEntryText(CatStrings.player2Options, new Vector2(415, 200), "aboutfont", false);
            MenuEntryText paddleJ2MenuEntryShadow = new MenuEntryText(CatStrings.player2Options, new Vector2(417, 203), Color.Black, "aboutfont", false);
            MenuEntryText ballMenuEntry = new MenuEntryText(CatStrings.ballOptions, new Vector2(415, 320), "aboutfont", false);
            MenuEntryText ballMenuEntryShadow = new MenuEntryText(CatStrings.ballOptions, new Vector2(417, 323), Color.Black, "aboutfont", false);
            temasMenuEntry = new MenuEntryText(string.Empty, new Vector2(40, 245), "gamefont", false);
            temasMenuEntryShadow = new MenuEntryText(string.Empty, new Vector2(42, 248), Color.Black, "gamefont", false);
            miniTema = new MenuEntryObject(string.Empty, new Vector2(95, 285));
            MenuEntryObject flechaDer = new MenuEntryObject("screens/options/flechaDer", new Vector2(350, 340));
            MenuEntryObject flechaIzq = new MenuEntryObject("screens/options/flechaIzq", new Vector2(35, 340));
            offXtreme = new MenuEntryObject("screens/options/offButton", new Vector2(215, 210));
            onXtreme = new MenuEntryObject("screens/options/onButton", new Vector2(305, 210));
            xtremeMode = new MenuEntryObject("screens/options/miniFlechaDer", Vector2.Zero);
            currentDifficult = new MenuEntryObject("screens/options/miniFlechaArr", new Vector2(63, 175));

            // Cargamos la seleccion de paletas y pelota y todas las opciones
            currentPaddleJ1 = new MenuEntryObject("screens/options/selection", Vector2.Zero);
            currentPaddleJ2 = new MenuEntryObject("screens/options/selection", Vector2.Zero);
            currentBall = new MenuEntryObject("screens/options/selection", Vector2.Zero);
            paddlesJ1[0] = new MenuEntryObject(string.Empty, new Vector2(445, 104), Color.White, 0);
            paddlesJ1[1] = new MenuEntryObject(string.Empty, new Vector2(510, 104), Color.Red, 1);
            paddlesJ1[2] = new MenuEntryObject(string.Empty, new Vector2(575, 104), Color.Yellow, 2);
            paddlesJ1[3] = new MenuEntryObject(string.Empty, new Vector2(640, 104), Color.Blue, 3);
            paddlesJ1[4] = new MenuEntryObject(string.Empty, new Vector2(705, 104), Color.Green, 4);
            paddlesJ2[0] = new MenuEntryObject(string.Empty, new Vector2(445, 225), Color.White, 0);
            paddlesJ2[1] = new MenuEntryObject(string.Empty, new Vector2(510, 225), Color.Red, 1);
            paddlesJ2[2] = new MenuEntryObject(string.Empty, new Vector2(575, 225), Color.Yellow, 2);
            paddlesJ2[3] = new MenuEntryObject(string.Empty, new Vector2(640, 225), Color.Blue, 3);
            paddlesJ2[4] = new MenuEntryObject(string.Empty, new Vector2(705, 225), Color.Green, 4);
            balls[0] = new MenuEntryObject(string.Empty, new Vector2(450, 355), Color.White, 0);
            balls[1] = new MenuEntryObject(string.Empty, new Vector2(515, 355), Color.Red, 1);
            balls[2] = new MenuEntryObject(string.Empty, new Vector2(580, 355), Color.Yellow, 2);
            balls[3] = new MenuEntryObject(string.Empty, new Vector2(640, 355), Color.Blue, 3);
            balls[4] = new MenuEntryObject(string.Empty, new Vector2(705, 355), Color.Green, 4);

            // Ponemos los strings vacios a su correspondiente valor
            SetMenuEntryText();

            // Eventos del menu al seleccionar.
            playMenuEntry.Selected += PlaySelected;
            flechaDer.Selected += FlechaDerSelected;
            flechaIzq.Selected += FlechaIzqSelected;
            onXtreme.Selected += OnXtremeSelected;
            offXtreme.Selected += OffXtremeSelected;
            difEasyMenuObject.Selected += DiffEasySelected;
            difMedMenuObject.Selected += DiffMedSelected;
            difHardMenuObject.Selected += DiffHardSelected;
            difAscMenuObject.Selected += DiffAscSelected;

            for (int i = 0; i < paddlesJ1.Length; i++) paddlesJ1[i].Selected += PaddleJ1Selected;
            for (int i = 0; i < paddlesJ2.Length; i++) paddlesJ2[i].Selected += PaddleJ2Selected;
            for (int i = 0; i < balls.Length; i++) balls[i].Selected += BallSelected;

            // Añadimos las entradas al Menu.
            MenuEntriesObject.Add(optionsPage);
            MenuEntriesObject.Add(flechaDer);
            MenuEntriesObject.Add(flechaIzq);
            MenuEntriesObject.Add(miniTema);
            MenuEntriesObject.Add(currentBall);
            MenuEntriesObject.Add(currentPaddleJ1);
            MenuEntriesObject.Add(currentPaddleJ2);
            MenuEntriesObject.Add(offXtreme);
            MenuEntriesObject.Add(onXtreme);
            MenuEntriesObject.Add(xtremeMode);
            MenuEntriesText.Add(playMenuEntryShadow);
            MenuEntriesText.Add(playMenuEntry);
            MenuEntriesText.Add(difficultyMenuEntryShadow);
            MenuEntriesText.Add(difficultyMenuEntry);
            MenuEntriesObject.Add(difEasyMenuObject);
            MenuEntriesObject.Add(difMedMenuObject);
            MenuEntriesObject.Add(difHardMenuObject);
            MenuEntriesObject.Add(difAscMenuObject);
            MenuEntriesObject.Add(currentDifficult);
            MenuEntriesText.Add(temasMenuEntryShadow);
            MenuEntriesText.Add(temasMenuEntry);

            MenuEntriesText.Add(paddleJ1MenuEntryShadow);
            MenuEntriesText.Add(paddleJ1MenuEntry);
            MenuEntriesText.Add(paddleJ2MenuEntryShadow);
            MenuEntriesText.Add(paddleJ2MenuEntry);
            MenuEntriesText.Add(ballMenuEntryShadow);
            MenuEntriesText.Add(ballMenuEntry);
            for (int i = 0; i < paddlesJ1.Length; i++) MenuEntriesObject.Add(paddlesJ1[i]);
            for (int i = 0; i < paddlesJ2.Length; i++) MenuEntriesObject.Add(paddlesJ2[i]);
            for (int i = 0; i < balls.Length; i++) MenuEntriesObject.Add(balls[i]);
        }

        /// <summary>
        /// Rellenamos los valores de las entradas si necesitan ser actualizadas.
        /// </summary>
        void SetMenuEntryText()
        {
            for (int i = 0; i < paddlesJ1.Length; i++) paddlesJ1[i].TexName = "themes/" + currentTema + "/paddle";
            for (int i = 0; i < paddlesJ2.Length; i++) paddlesJ2[i].TexName = "themes/" + currentTema + "/paddle";
            for (int i = 0; i < balls.Length; i++) balls[i].TexName = "themes/" + currentTema + "/ball";
            temasMenuEntryShadow.Text = CatStrings.themeOptions + temas[currentTema];
            temasMenuEntry.Text = CatStrings.themeOptions + temas[currentTema];
            xtremeMode.Position = new Vector2(xtremeBool ? (onXtreme.Position.X - 27) : (offXtreme.Position.X - 27), 207);
            currentBall.Position = new Vector2(balls[colorBallInt].Position.X - 14, balls[colorBallInt].Position.Y + 12);
            currentPaddleJ1.Position = new Vector2(paddlesJ1[colorPaddleJ1Int].Position.X - 13, paddlesJ1[colorPaddleJ1Int].Position.Y + 40);
            currentPaddleJ2.Position = new Vector2(paddlesJ2[colorPaddleJ2Int].Position.X - 13, paddlesJ2[colorPaddleJ2Int].Position.Y + 40);
            miniTema.TexName = "themes/" + currentTema + "/miniTema";
        }

        #endregion

        #region Entradas

        /// <summary>
        /// Evento que surge al presionar la flecha derecha de temas.
        /// </summary>
        void FlechaDerSelected(object sender, EventArgs e)
        {
            if (currentTema == (temas.Length - 1)) currentTema = 0; else currentTema++;
            GameStateManagementGame.Settings.Save("Theme", currentTema);
            GameStateManagementGame.Settings.LoadAll();
            SetMenuEntryText();
        }

        /// <summary>
        /// Evento que surge al presionar la flecha izquierda de temas.
        /// </summary>
        void FlechaIzqSelected(object sender, EventArgs e)
        {
            if (currentTema == 0) currentTema = (temas.Length - 1); else currentTema--;
            GameStateManagementGame.Settings.Save("Theme", currentTema);
            GameStateManagementGame.Settings.LoadAll();
            SetMenuEntryText();
        }

        /// <summary>
        /// Evento que surge al seleccionar una paleta de color del J1.
        /// </summary>
        void PaddleJ1Selected(object sender, EventArgs e)
        {
            MenuEntryObject paddleJ1 = sender as MenuEntryObject;
            colorPaddleJ1 = paddleJ1.Color;
            colorPaddleJ1Int = paddleJ1.Id;
            GameStateManagementGame.Settings.Save("PaddleJ1ColorInt", colorPaddleJ1Int);
            GameStateManagementGame.Settings.Save("PaddleJ1Color", colorPaddleJ1);
            GameStateManagementGame.Settings.LoadAll();
            SetMenuEntryText();
        }

        /// <summary>
        /// Evento que surge al seleccionar una paleta de color del J2.
        /// </summary>
        void PaddleJ2Selected(object sender, EventArgs e)
        {
            MenuEntryObject paddleJ2 = sender as MenuEntryObject;
            colorPaddleJ2 = paddleJ2.Color;
            colorPaddleJ2Int = paddleJ2.Id;
            GameStateManagementGame.Settings.Save("PaddleJ2ColorInt", colorPaddleJ2Int);
            GameStateManagementGame.Settings.Save("PaddleJ2Color", colorPaddleJ2);
            GameStateManagementGame.Settings.LoadAll();
            SetMenuEntryText();
        }

        /// <summary>
        /// Evento que surge al seleccionar una pelota de color.
        /// </summary>
        void BallSelected(object sender, EventArgs e)
        {
            MenuEntryObject ball = sender as MenuEntryObject;
            colorBall = ball.Color;
            colorBallInt = ball.Id;
            GameStateManagementGame.Settings.Save("BallColorInt", colorBallInt);
            GameStateManagementGame.Settings.Save("BallColor", colorBall);
            GameStateManagementGame.Settings.LoadAll();
            SetMenuEntryText();
        }

        /// <summary>
        /// Evento que surge al seleccionar el boton diffEasy.
        /// </summary>
        void DiffEasySelected(object sender, EventArgs e)
        {
            difficult = 1;
            currentDifficult.Position = new Vector2((difEasyMenuObject.Position.X + (difEasyMenuObject.Rectangle.Height / 2)) - 14, 175);
        }

        /// <summary>
        /// Evento que surge al seleccionar el boton diffMed.
        /// </summary>
        void DiffMedSelected(object sender, EventArgs e)
        {
            difficult = 2;
            currentDifficult.Position = new Vector2((difMedMenuObject.Position.X + (difMedMenuObject.Rectangle.Height / 2)) - 14, 175);
        }

        /// <summary>
        /// Evento que surge al seleccionar el boton diffHard.
        /// </summary>
        void DiffHardSelected(object sender, EventArgs e)
        {
            difficult = 3;
            currentDifficult.Position = new Vector2((difHardMenuObject.Position.X + (difHardMenuObject.Rectangle.Height / 2)) - 14, 175);
        }

        /// <summary>
        /// Evento que surge al seleccionar el boton diffAsc.
        /// </summary>
        void DiffAscSelected(object sender, EventArgs e)
        {
            difficult = 4;
            currentDifficult.Position = new Vector2((difAscMenuObject.Position.X + (difAscMenuObject.Rectangle.Width / 2)) - 14, 175);
        }

        /// <summary>
        /// Evento que surge al seleccionar el boton onXtreme.
        /// </summary>
        void OnXtremeSelected(object sender, EventArgs e)
        {
            if (xtremeBool != true)
            {
                xtremeBool = true;

                // Actualizamos el señalador y guardamos
                GameStateManagementGame.Settings.Save("Xtreme", xtremeBool);
                GameStateManagementGame.Settings.LoadAll();
                SetMenuEntryText();
            }
        }

        /// <summary>
        /// Evento que surge al seleccionar el boton offXtreme.
        /// </summary>
        void OffXtremeSelected(object sender, EventArgs e)
        {
            if (xtremeBool != false)
            {
                xtremeBool = false;

                // Actualizamos el señalador y guardamos
                GameStateManagementGame.Settings.Save("Xtreme", xtremeBool);
                GameStateManagementGame.Settings.LoadAll();
                SetMenuEntryText();
            }
        }

        /// <summary>
        /// Evento que surge al presionar el boton Jugar/Play.
        /// </summary>
        void PlaySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new DosPlayerScreen(difficult));
        }

        #endregion
    }
}
