//-----------------------------------------------------------------------
// <copyright file="ImageFromRegularFont.cs" company="Lifeprojects.de">
//     Class: ImageFromRegularFont
//     Copyright © Lifeprojects.de 2018
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>26.11.2018</date>
//
// <summary>Class for Convert FontIcon in ImageSource</summary>
//-----------------------------------------------------------------------

namespace PertNET.Core.UI
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;

    /// <summary>
    /// ImageSource="{Tools:ImageFromFont Text=&#xf004;, FontFamily=/GUIDemo;component/Resources/Font/#Font Awesome 5 Free Solid, Brush=HotPink, Weight=ExtraBold}"
    /// </summary>
    public class ImageFromRegularFont : MarkupExtension
    {
        private const float PIXELSPERDIP = 96;

        public ImageFromRegularFont()
        {
            string rootNamespace = Assembly.GetExecutingAssembly().GetName().Name;
            string fontFamily = $"/{rootNamespace};component/Resources/Font/#Font Awesome 6 Free Regular";

            this.Text = "\uf004";
            this.FontFamily = new FontFamily(fontFamily);
            this.Style = FontStyles.Normal;
            this.Weight = FontWeights.Normal;
            this.Stretch = FontStretches.Normal;
            this.Brush = new SolidColorBrush(Colors.Black);
        }

        public ImageFromRegularFont(string textSymbol, Color fontColor)
        {
            string rootNamespace = Assembly.GetExecutingAssembly().GetName().Name;
            string fontFamily = $"/{rootNamespace};component/Resources/Font/#Font Awesome 6 Free Regular";
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
                                                 Brushes.Black,0);
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
                    GlyphRun gr = new GlyphRun(glyphTypeface, 0, false, 1.0,0, glyphIndexes, new Point(0, 0), advanceWidths, null, null, null, null, null, null);

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