using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NppMarkdownPanel
{
    internal static class PluginLocalization
    {
        private const int RussianCodePage = 1251;
        private static readonly Dictionary<string, string> english = LoadLanguage("english");
        private static Dictionary<string, string> current = english;

        public static bool IsRussian { get; private set; }

        public static event Action LanguageChanged;

        public static string Text(string key)
        {
            if (current.TryGetValue(key, out string value)) return value;
            if (english.TryGetValue(key, out value)) return value;
            return key;
        }

        public static string Format(string key, params object[] arguments)
        {
            return String.Format(CultureInfo.CurrentCulture, Text(key), arguments);
        }

        public static void RefreshFromNotepad()
        {
            string nativeLanguage = GetNativeLanguageFileName();
            bool isRussian;

            if (!String.IsNullOrWhiteSpace(nativeLanguage))
            {
                isRussian = nativeLanguage.StartsWith("russian", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                int codePage = Win32.SendMessage(
                    PluginBase.nppData._nppHandle,
                    (uint)NppMsg.NPPM_GETCURRENTNATIVELANGENCODING,
                    IntPtr.Zero,
                    IntPtr.Zero).ToInt32();
                isRussian = codePage == RussianCodePage;
            }

            if (IsRussian == isRussian) return;

            IsRussian = isRussian;
            current = isRussian ? LoadLanguage("russian") : english;
            LanguageChanged?.Invoke();
        }

        private static Dictionary<string, string> LoadLanguage(string language)
        {
            var values = new Dictionary<string, string>(StringComparer.Ordinal);
            try
            {
                var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var filePath = Path.Combine(assemblyDirectory, "localization", language + ".xml");
                var document = new XmlDocument();
                document.Load(filePath);

                foreach (XmlNode node in document.SelectNodes("/localization/text"))
                {
                    var key = node.Attributes?["key"]?.Value;
                    if (!String.IsNullOrWhiteSpace(key)) values[key] = node.InnerText;
                }
            }
            catch
            {
                // Missing or malformed translations fall back to English and then to the key.
            }

            return values;
        }

        private static string GetNativeLanguageFileName()
        {
            IntPtr nppHandle = PluginBase.nppData._nppHandle;
            uint message = (uint)NppMsg.NPPM_GETNATIVELANGFILENAME;
            int length = Win32.SendMessage(nppHandle, message, IntPtr.Zero, IntPtr.Zero).ToInt32();
            if (length <= 0) return String.Empty;

            var buffer = new StringBuilder(length + 1);
            Win32.SendMessage(nppHandle, message, length + 1, buffer);
            return buffer.ToString();
        }
    }
}
