//-----------------------------------------------------------------------
// <copyright file="ImageFromFont.cs" company="Lifeprojects.de">
//     Class: ImageFromFont
//     Copyright © Lifeprojects.de 2019
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>13.02.2019</date>
//
// <summary>
//      Class for create Font Symbols
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core.UI
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;

    /// <summary>
    /// ImageSource="{Tools:ImageFromFont Text=&#xf004;, FontFamily=/GUIDemo;component/Resources/Font/#Font Awesome 6 Free Solid, Brush=HotPink, Weight=ExtraBold}"
    /// </summary>
    public class ImageFromFont : MarkupExtension
    {
        private const float PIXELSPERDIP = 96;

        public ImageFromFont()
        {
            string rootNamespace = Assembly.GetExecutingAssembly().GetName().Name;
            string fontFamily = $"/{rootNamespace};component/Resources/Font/#Font Awesome 6 Free Solid";
            this.Text = "\uf004";
            this.FontFamily = new FontFamily(fontFamily);
            this.Style = FontStyles.Normal;
            this.Weight = FontWeights.Normal;
            this.Stretch = FontStretches.Normal;
            this.Brush = new SolidColorBrush(Colors.Black);
        }

        public ImageFromFont(string textSymbol, Color fontColor)
        {
            string rootNamespace = Assembly.GetExecutingAssembly().GetName().Name;
            string fontFamily = $"/{rootNamespace};component/Resources/Font/#Font Awesome 6 Free Solid";
            this.Text = textSymbol;
            this.FontFamily = new FontFamily(fontFamily);
            this.Style = FontStyles.Normal;
            this.Weight = FontWeights.Normal;
            this.Stretch = FontStretches.Normal;
            this.Brush = new SolidColorBrush(fontColor);
        }

        public string Text { get; set; }

        public FontFamily FontFamily { get; set; }

        public FontStyle Style { get; set; }

        public FontWeight Weight { get; set; }

        public FontStretch Stretch { get; set; }

        public Brush Brush { get; set; }

        public static Size MeasureTextSize(string text, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize)
        {
            FormattedText ft = new FormattedText(text,
                                                 CultureInfo.CurrentCulture,
                                                 FlowDirection.LeftToRight,
                                                 new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                                                 fontSize,
                                                 Brushes.Black, 0);
            return new Size(ft.Width, ft.Height);
        }

        public static Size MeasureText(string text, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize)
        {
            Typeface typeface = new Typeface(fontFamily, fontStyle, fontWeight, fontStretch);
            GlyphTypeface glyphTypeface;

            if (typeface.TryGetGlyphTypeface(out glyphTypeface) == false)
            {
                return MeasureTextSize(text, fontFamily, fontStyle, fontWeight, fontStretch, fontSize);
            }

            double totalWidth = 0;
            double height = 0;

            for (int n = 0; n < text.Length; n++)
            {
                ushort glyphIndex = glyphTypeface.CharacterToGlyphMap[text[n]];

                double width = glyphTypeface.AdvanceWidths[glyphIndex] * fontSize;

                double glyphHeight = glyphTypeface.AdvanceHeights[glyphIndex] * fontSize;

                if (glyphHeight > height)
                {
                    height = glyphHeight;
                }

                totalWidth += width;
            }

            return new Size(totalWidth, height);
        }

        public static string FontVersion()
        {
            string result = string.Empty;
            string rootNamespace = Assembly.GetExecutingAssembly().GetName().Name;
            string fontFamily = $"/{rootNamespace};component/Resources/Font/#Font Awesome 6 Free Solid";
            var fontFam = new FontFamily(fontFamily);
            FontFamily fontFam1 = new FontFamily(new Uri("pack://application:,,,", UriKind.RelativeOrAbsolute), fontFam.Source);
            Typeface typeface = new Typeface(fontFam1, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            GlyphTypeface glyphTypeface;
            bool isGlyphTypeface = typeface.TryGetGlyphTypeface(out glyphTypeface);
            if (isGlyphTypeface == true)
            {
                if (glyphTypeface.VersionStrings.Count > 0)
                {
                    result = glyphTypeface.VersionStrings.FirstOrDefault().Value;
                }
            }

            return result;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return CreateGlyph(this.Text, this.FontFamily, this.Style, this.Weight, this.Stretch, this.Brush);
        }

        public ImageSource CreateGlyph(string text)
        {
            return CreateGlyph(text, this.FontFamily, this.Style, this.Weight, this.Stretch, this.Brush);
        }

        public Size MeasureText(string text)
        {
            return MeasureText(text, this.FontFamily, this.Style, this.Weight, this.Stretch, 12);
        }

        private static ImageSource CreateGlyph(string text, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, Brush foreBrush)
        {
            if (fontFamily != null && !string.IsNullOrEmpty(text))
            {
                Typeface typeface = new Typeface(fontFamily, fontStyle, fontWeight, fontStretch);

                GlyphTypeface glyphTypeface;
                if (typeface.TryGetGlyphTypeface(out glyphTypeface) == false)
                {
                    FontFamily fontFam = new FontFamily(new Uri("pack://application:,,,", UriKind.RelativeOrAbsolute), fontFamily.Source);
                    typeface = new Typeface(fontFam, fontStyle, fontWeight, fontStretch);
                    if (typeface.TryGetGlyphTypeface(out glyphTypeface) == false)
                    {
                        throw new InvalidOperationException("No glyphtypeface found");
                    }
                }

                ushort[] glyphIndexes = new ushort[text.Length];
                double[] advanceWidths = new double[text.Length];

                for (int n = 0; n < text.Length; n++)
                {
                    ushort glyphIndex = 0;
                    try
                    {
                        if (glyphTypeface.CharacterToGlyphMap.ContainsKey(text[n]) == true)
                        {
                            glyphIndex = glyphTypeface.CharacterToGlyphMap[text[n]];
                        }
                    }
                    catch (Exception)
                    {
                        glyphIndex = 42;
                    }

                    glyphIndexes[n] = glyphIndex;

                    double width = glyphTypeface.AdvanceWidths[glyphIndex] * 1.0;
                    advanceWidths[n] = width;
                }

                try
                {
                    GlyphRun gr = new GlyphRun(glyphTypeface, 0, false, 1.0, 0, glyphIndexes, new Point(0, 0), advanceWidths, null, null, null, null, null, null);

                    GlyphRunDrawing glyphRunDrawing = new GlyphRunDrawing(foreBrush, gr);

                    return new DrawingImage(glyphRunDrawing);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in generating Glyphrun : " + ex.Message);
                }
            }

            return null;
        }
    }
}