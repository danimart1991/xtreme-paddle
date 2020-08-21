using Microsoft.Xna.Framework;

namespace XtremePaddle
{
    /// <summary>
    /// La pantalla About, es una simple imagen estatica, que
    /// muestra diferentes informacion sobre el juego, como
    /// su creador, musica, agradecimientos, version...
    /// </summary>
    class AboutMenuScreen : MenuScreen
    {
        #region Inicializacion

        /// <summary>
        /// Constructor.
        /// </summary>
        public AboutMenuScreen()
            : base(CatStrings.aboutTittle)
        {

            // Entradas del Menu.
            MenuEntryObject aboutPage = new MenuEntryObject("screens/aboutPage", Vector2.Zero);
            MenuEntryText developedMenuEntry = new MenuEntryText(CatStrings.developedAbout, new Vector2(435, 90), "aboutfont", false);
            MenuEntryText developedMenuEntryShadow = new MenuEntryText(CatStrings.developedAbout, new Vector2(437, 93), Color.Black, "aboutfont", false);
            MenuEntryText musicMenuEntry = new MenuEntryText(CatStrings.musicAbout, new Vector2(435, 180), "aboutfont", false);
            MenuEntryText musicMenuEntryShadow = new MenuEntryText(CatStrings.musicAbout, new Vector2(437, 183), Color.Black, "aboutfont", false);
            MenuEntryText thanksMenuEntry = new MenuEntryText(CatStrings.thanksAbout, new Vector2(435, 250), "aboutfont", false);
            MenuEntryText thanksMenuEntryShadow = new MenuEntryText(CatStrings.thanksAbout, new Vector2(437, 253), Color.Black, "aboutfont", false);
            MenuEntryText infoMenuEntry = new MenuEntryText(CatStrings.infoAbout, new Vector2(435, 355), "aboutfont", false);
            MenuEntryText infoMenuEntryShadow = new MenuEntryText(CatStrings.infoAbout, new Vector2(437, 358), Color.Black, "aboutfont", false);
            MenuEntryText versionMenuEntry = new MenuEntryText(CatStrings.versionAbout, new Vector2(235, 270), "aboutfont", true);
            MenuEntryText versionMenuEntryShadow = new MenuEntryText(CatStrings.versionAbout, new Vector2(237, 273), Color.Black, "aboutfont", true);

            // Añadimos las entradas al Menu.
            MenuEntriesObject.Add(aboutPage);
            MenuEntriesText.Add(developedMenuEntryShadow);
            MenuEntriesText.Add(developedMenuEntry);
            MenuEntriesText.Add(musicMenuEntryShadow);
            MenuEntriesText.Add(musicMenuEntry);
            MenuEntriesText.Add(thanksMenuEntryShadow);
            MenuEntriesText.Add(thanksMenuEntry);
            MenuEntriesText.Add(infoMenuEntryShadow);
            MenuEntriesText.Add(infoMenuEntry);
            MenuEntriesText.Add(versionMenuEntryShadow);
            MenuEntriesText.Add(versionMenuEntry);
        }

        #endregion
    }
}
