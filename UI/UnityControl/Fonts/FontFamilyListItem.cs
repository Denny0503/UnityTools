using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace UnityControl.Fonts
{
    internal class FontFamilyListItem : TextBlock, IComparable
    {
        private string _displayName;

        public FontFamilyListItem(FontFamily fontFamily)
        {
            _displayName = GetDisplayName(fontFamily);

            this.FontFamily = fontFamily;
            this.Text = _displayName;
            this.ToolTip = _displayName;

            // In the case of symbol font, apply the default message font to the text so it can be read.
            if (IsSymbolFont(fontFamily))
            {
                TextRange range = new TextRange(this.ContentStart, this.ContentEnd);
                range.ApplyPropertyValue(TextBlock.FontFamilyProperty, SystemFonts.MessageFontFamily);
            }
        }

        public override string ToString()
        {
            return _displayName;
        }

        int IComparable.CompareTo(object obj)
        {
            return string.Compare(_displayName, obj.ToString(), true, CultureInfo.CurrentCulture);
        }

        internal static bool IsSymbolFont(FontFamily fontFamily)
        {
            foreach (Typeface typeface in fontFamily.GetTypefaces())
            {
                if (typeface.TryGetGlyphTypeface(out GlyphTypeface face))
                {
                    return face.Symbol;
                }
            }
            return false;
        }

        internal static string GetDisplayName(FontFamily family)
        {
            return NameDictionaryHelper.GetDisplayName(family.FamilyNames);
        }
    }
}
