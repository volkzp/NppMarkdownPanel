using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.Text;

namespace NppMarkdownPanel
{
    internal static class PluginLocalization
    {
        private const int RussianCodePage = 1251;

        public static bool IsRussian { get; private set; }

        public static event Action LanguageChanged;

        public static string Text(string english, string russian)
        {
            return IsRussian ? russian : english;
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
            LanguageChanged?.Invoke();
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
