using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace E2Edit
{
    /// <summary>
    /// Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {
        // I hate you, adobe
        public static readonly IEnumerable<FontFamily> FontList = LoadFontList();
        public readonly Settings Settings;

        // FIXME
        public SettingsDialog(Settings settings)
        {
            Settings = settings;
            InitializeComponent();
            DataContext = settings;
        }

        private static IEnumerable<FontFamily> LoadFontList()
        {
            return Fonts.SystemFontFamilies.Where(font => !font.Source.Contains("ExtraGlyphlets")).ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Settings : ICloneable
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof (Settings));
        public bool AutoIndentEnabled = true;
        [XmlIgnore] public FontFamily Font = new FontFamily("Consolas,Courier New");
        public bool IntelliSenseEnabled;
        public string SteamPath;

        [XmlElement("Font")]
        public string FontFamily
        {
            get { return Font != null ? Font.Source : "Consolas,Courier New"; } // Default fonts
            set { Font = new FontFamily(value); }
        }

        #region ICloneable Members

        public object Clone()
        {
            return new Settings
                       {Font = Font, AutoIndentEnabled = AutoIndentEnabled, IntelliSenseEnabled = IntelliSenseEnabled};
        }

        #endregion

        public void Save(Stream stream)
        {
            Serializer.Serialize(stream, this);
        }

        public static Settings Load(Stream stream)
        {
            return (Settings) Serializer.Deserialize(stream);
        }
    }
}