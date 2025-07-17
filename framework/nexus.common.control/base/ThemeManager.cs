
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;

namespace nexus.common.control.Themes
{
    public static class NxThemeManager
    {
        public enum NxColorThemes { Light, Dark, Orange, Blue }

        public static event EventHandler ThemeChanged;

        private static NxColorThemes _currentTheme = NxColorThemes.Orange;
        public static NxTheme GetCurrentTheme() => _themes[_currentTheme];

        private static readonly Dictionary<NxColorThemes, NxTheme> _themes = new()
        {

            [NxColorThemes.Light] = new NxTheme
            {
                Name               = "Light",

                PromptBack         = new NxBrush(new SolidColorBrush(NxColors.White)),
                PromptFore         = new NxBrush(new SolidColorBrush(NxColors.Black)),

                ReplyBack          = new NxBrush(new SolidColorBrush(NxColors.WhiteSmoke)),
                ReplyFore          = new NxBrush(new SolidColorBrush(NxColors.Black)),
                ReplyRequired      = CreateReplyBrush(NxColors.WhiteSmoke, NxColors.LightYellow),
                ReplyValid         = CreateReplyBrush(NxColors.WhiteSmoke, NxColors.Green),
                ReplyInValid       = CreateReplyBrush(NxColors.WhiteSmoke, NxColors.Red),

                MenuFore           = new NxBrush(new SolidColorBrush(NxColors.Black)),
                MenuBack           = new NxBrush(new SolidColorBrush(NxColors.LightGray)),
                MenuBorder         = new NxBrush(new SolidColorBrush(NxColors.DarkGray)),
                MenuGroupBack      = new NxBrush(new SolidColorBrush(NxColors.WhiteSmoke)),
                MenuGroupHover     = new NxBrush(new SolidColorBrush(NxColors.LightSteelBlue)),
                MenuItemBack       = new NxBrush(new SolidColorBrush(NxColors.White)),
                MenuItemHover      = new NxBrush(new SolidColorBrush(NxColors.LightGray)),
                MenuItemPressed    = new NxBrush(new SolidColorBrush(NxColors.DarkGray)),
                MenuItemFore       = new NxBrush(new SolidColorBrush(NxColors.Black)),
                MenuCaptionBack    = new NxBrush(new SolidColorBrush(NxColors.LightGray)),
                MenuCaptionFore    = new NxBrush(new SolidColorBrush(NxColors.Black)),
                MenuDivider        = new NxBrush(new SolidColorBrush(NxColors.Gray)),

                PanelBack          = new NxBrush(new SolidColorBrush(NxColors.White)),
                Border             = new NxBrush(new SolidColorBrush(NxColors.Silver)),
                Accent             = new NxBrush(new SolidColorBrush(NxColors.SlateGray)),

                ButtonFore         = new NxBrush(new SolidColorBrush(NxColors.Black)),
                ButtonBack         = new NxBrush(new SolidColorBrush(NxColors.WhiteSmoke)),
                ButtonPointerOver  = new NxBrush(new SolidColorBrush(NxColors.LightGray)),
                ButtonPressed      = new NxBrush(new SolidColorBrush(NxColors.DarkGray)),

                ProgressTrack      = new NxBrush(new SolidColorBrush(NxColors.LightGray)),
                ProgressIndicator  = new NxBrush(new SolidColorBrush(NxColors.SteelBlue)),
                CountdownGlow      = new NxBrush(NxColors.LightBlue),

                ListFore           = new NxBrush(new SolidColorBrush(NxColors.Black)),
                ListBack           = new NxBrush(new SolidColorBrush(NxColors.White)),
                ListAltBack        = new NxBrush(new SolidColorBrush(NxColors.WhiteSmoke)),
                ListBorder         = new NxBrush(new SolidColorBrush(NxColors.LightGray)),
                ListSelectedFore   = new NxBrush(new SolidColorBrush(NxColors.Black)),
                ListSelectedBack   = new NxBrush(new SolidColorBrush(NxColors.WhiteSmoke)),
                ListPointerOver    = new NxBrush(new SolidColorBrush(NxColors.LightGray)),

            
                TabStripBack         = new NxBrush(new SolidColorBrush(NxColors.WhiteSmoke)),
                TabStripFore         = new NxBrush(new SolidColorBrush(NxColors.Black)),
                TabStripHover        = new NxBrush(new SolidColorBrush(NxColors.LightGray)),
                TabStripSelectedBack = new NxBrush(new SolidColorBrush(NxColors.SkyBlue)),
                TabStripSelectedFore = new NxBrush(new SolidColorBrush(NxColors.Black)),
},

            [NxColorThemes.Dark] = new NxTheme
            {
                Name               = "Dark",

                PromptBack         = new NxBrush(new SolidColorBrush(NxColors.Black)),
                PromptFore         = new NxBrush(new SolidColorBrush(NxColors.White)),

                ReplyBack          = new NxBrush(new SolidColorBrush(NxColors.DimGray)),
                ReplyFore          = new NxBrush(new SolidColorBrush(NxColors.WhiteSmoke)),
                ReplyRequired      = CreateReplyBrush(NxColors.DimGray, NxColors.LightYellow),
                ReplyValid         = CreateReplyBrush(NxColors.DimGray, NxColors.Green),
                ReplyInValid       = CreateReplyBrush(NxColors.DimGray, NxColors.Red),

                MenuFore           = new NxBrush(new SolidColorBrush(NxColors.White)),
                MenuBack           = new NxBrush(new SolidColorBrush(NxColors.Gray)),
                MenuBorder         = new NxBrush(new SolidColorBrush(NxColors.Black)),
                MenuGroupBack      = new NxBrush(new SolidColorBrush(NxColors.DarkSlateGray)),
                MenuGroupHover     = new NxBrush(new SolidColorBrush(NxColors.SlateGray)),
                MenuItemBack       = new NxBrush(new SolidColorBrush(NxColors.Black)),
                MenuItemHover      = new NxBrush(new SolidColorBrush(NxColors.DarkGray)),
                MenuItemPressed    = new NxBrush(new SolidColorBrush(NxColors.DimGray)),
                MenuItemFore       = new NxBrush(new SolidColorBrush(NxColors.White)),
                MenuCaptionBack    = new NxBrush(new SolidColorBrush(NxColors.Gray)),
                MenuCaptionFore    = new NxBrush(new SolidColorBrush(NxColors.White)),
                MenuDivider        = new NxBrush(new SolidColorBrush(NxColors.DarkGray)),

                PanelBack          = new NxBrush(new SolidColorBrush(NxColors.Black)),
                Border             = new NxBrush(new SolidColorBrush(NxColors.DimGray)),
                Accent             = new NxBrush(new SolidColorBrush(NxColors.Silver)),

                ButtonFore         = new NxBrush(new SolidColorBrush(NxColors.Black)),
                ButtonBack         = new NxBrush(new SolidColorBrush(NxColors.Gray)),
                ButtonPointerOver  = new NxBrush(new SolidColorBrush(NxColors.DarkSlateGray)),
                ButtonPressed      = new NxBrush(new SolidColorBrush(NxColors.Black)),

                ProgressTrack      = new NxBrush(new SolidColorBrush(NxColors.DarkGray)),
                ProgressIndicator  = new NxBrush(new SolidColorBrush(NxColors.Orange)),
                CountdownGlow      = new NxBrush(NxColors.LightBlue),

                ListFore           = new NxBrush(new SolidColorBrush(NxColors.Black)),
                ListBack           = new NxBrush(new SolidColorBrush(NxColors.LightGray)),
                ListAltBack        = new NxBrush(new SolidColorBrush(NxColors.Gray)),
                ListBorder         = new NxBrush(new SolidColorBrush(NxColors.DarkSlateGray)),
                ListSelectedFore   = new NxBrush(new SolidColorBrush(NxColors.Black)),
                ListSelectedBack   = new NxBrush(new SolidColorBrush(NxColors.DarkSlateGray)),
                ListPointerOver    = new NxBrush(new SolidColorBrush(NxColors.DarkSlateGray)),
            
                TabStripBack         = new NxBrush(new SolidColorBrush(NxColors.DarkSlateGray)),
                TabStripFore         = new NxBrush(new SolidColorBrush(NxColors.White)),
                TabStripHover        = new NxBrush(new SolidColorBrush(NxColors.SlateGray)),
                TabStripSelectedBack = new NxBrush(new SolidColorBrush(NxColors.SteelBlue)),
                TabStripSelectedFore = new NxBrush(new SolidColorBrush(NxColors.White)),
},

            [NxColorThemes.Orange] = new NxTheme
            {
                Name               = "Orange",

                PromptBack         = new NxBrush(new SolidColorBrush(NxColors.LightOrange)),
                PromptFore         = new NxBrush(new SolidColorBrush(NxColors.Black)),

                ReplyBack          = new NxBrush(new SolidColorBrush(NxColors.White)),
                ReplyFore          = new NxBrush(new SolidColorBrush(NxColors.Black)),
                ReplyRequired      = CreateReplyBrush(NxColors.White, NxColors.LightYellow),
                ReplyValid         = CreateReplyBrush(NxColors.White, NxColors.Green),
                ReplyInValid       = CreateReplyBrush(NxColors.White, NxColors.Red),

                MenuFore           = new NxBrush(new SolidColorBrush(NxColors.Black)),
                MenuBack           = new NxBrush(new SolidColorBrush(NxColors.Orange)),
                MenuBorder         = new NxBrush(new SolidColorBrush(NxColors.DarkOrange)),
                MenuGroupBack      = new NxBrush(new SolidColorBrush(NxColors.LightOrange)),
                MenuGroupHover     = new NxBrush(new SolidColorBrush(NxColors.OrangeRed)),
                MenuItemBack       = new NxBrush(new SolidColorBrush(NxColors.LightOrange)),
                MenuItemHover      = new NxBrush(new SolidColorBrush(NxColors.Moccasin)),
                MenuItemPressed    = new NxBrush(new SolidColorBrush(NxColors.DarkOrange)),
                MenuItemFore       = new NxBrush(new SolidColorBrush(NxColors.Black)),
                MenuCaptionBack    = new NxBrush(new SolidColorBrush(NxColors.Orange)),
                MenuCaptionFore    = new NxBrush(new SolidColorBrush(NxColors.White)),
                MenuDivider        = new NxBrush(new SolidColorBrush(NxColors.Gray)),

                PanelBack          = new NxBrush(new SolidColorBrush(NxColors.LightOrange)),
                Border             = new NxBrush(new SolidColorBrush(NxColors.DarkOrange)),
                Accent             = new NxBrush(new SolidColorBrush(NxColors.OrangeRed)),

                ButtonFore         = new NxBrush(new SolidColorBrush(NxColors.White)),
                ButtonBack         = new NxBrush(new SolidColorBrush(NxColors.DarkOrange)),
                ButtonPointerOver  = new NxBrush(new SolidColorBrush(NxColors.LightYellow)),
                ButtonPressed      = new NxBrush(new SolidColorBrush(NxColors.OrangeRed)),

                ProgressTrack      = new NxBrush(new SolidColorBrush(NxColors.LightOrange)),
                ProgressIndicator  = new NxBrush(new SolidColorBrush(NxColors.OrangeRed)),
                CountdownGlow      = new NxBrush(NxColors.LightBlue),

                ListFore           = new NxBrush(new SolidColorBrush(NxColors.White)),
                ListBack           = new NxBrush(new SolidColorBrush(NxColors.LightOrange)),
                ListAltBack        = new NxBrush(new SolidColorBrush(NxColors.Orange)),
                ListBorder         = new NxBrush(new SolidColorBrush(NxColors.DarkOrange)),
                ListSelectedFore   = new NxBrush(new SolidColorBrush(NxColors.White)),
                ListSelectedBack   = new NxBrush(new SolidColorBrush(NxColors.OrangeRed)),
                ListPointerOver    = new NxBrush(new SolidColorBrush(NxColors.DarkOrange)),
            
                TabStripBack         = new NxBrush(new SolidColorBrush(NxColors.DarkOrange)),
                TabStripFore         = new NxBrush(new SolidColorBrush(NxColors.White)),
                TabStripHover        = new NxBrush(new SolidColorBrush(NxColors.OrangeRed)),
                TabStripSelectedBack = new NxBrush(new SolidColorBrush(NxColors.Orange)),
                TabStripSelectedFore = new NxBrush(new SolidColorBrush(NxColors.White)),
            },

            [NxColorThemes.Blue] = new NxTheme
            {
                Name               = "Blue",

                PromptBack         = new NxBrush(new SolidColorBrush(NxColors.LightBlue)),
                PromptFore         = new NxBrush(new SolidColorBrush(NxColors.Black)),

                ReplyBack          = new NxBrush(new SolidColorBrush(NxColors.White)),
                ReplyFore          = new NxBrush(new SolidColorBrush(NxColors.Black)),
                ReplyRequired      = CreateReplyBrush(NxColors.White, NxColors.LightYellow),
                ReplyValid         = CreateReplyBrush(NxColors.White, NxColors.Green),
                ReplyInValid       = CreateReplyBrush(NxColors.White, NxColors.Red),

                MenuFore           = new NxBrush(new SolidColorBrush(NxColors.DarkBlue)),
                MenuBack           = new NxBrush(new SolidColorBrush(NxColors.SkyBlue)),
                MenuBorder         = new NxBrush(new SolidColorBrush(NxColors.SteelBlue)),
                MenuGroupBack      = new NxBrush(new SolidColorBrush(NxColors.LightBlue)),
                MenuGroupHover     = new NxBrush(new SolidColorBrush(NxColors.LightSkyBlue)),
                MenuItemBack       = new NxBrush(new SolidColorBrush(NxColors.White)),
                MenuItemHover      = new NxBrush(new SolidColorBrush(NxColors.LightCyan)),
                MenuItemPressed    = new NxBrush(new SolidColorBrush(NxColors.DeepSkyBlue)),
                MenuItemFore       = new NxBrush(new SolidColorBrush(NxColors.DarkBlue)),
                MenuCaptionBack    = new NxBrush(new SolidColorBrush(NxColors.SteelBlue)),
                MenuCaptionFore    = new NxBrush(new SolidColorBrush(NxColors.White)),
                MenuDivider        = new NxBrush(new SolidColorBrush(NxColors.SlateGray)),

                PanelBack          = new NxBrush(new SolidColorBrush(NxColors.LightBlue)),
                Border             = new NxBrush(new SolidColorBrush(NxColors.White)),
                Accent             = new NxBrush(new SolidColorBrush(NxColors.LightSteelBlue)),

                ButtonFore         = new NxBrush(new SolidColorBrush(NxColors.Black)),
                ButtonBack         = new NxBrush(new SolidColorBrush(NxColors.CornflowerBlue)),
                ButtonPointerOver  = new NxBrush(new SolidColorBrush(NxColors.LightBlue)),
                ButtonPressed      = new NxBrush(new SolidColorBrush(NxColors.SteelBlue)),

                ProgressTrack      = new NxBrush(new SolidColorBrush(NxColors.LightGray)),
                ProgressIndicator  = new NxBrush(new SolidColorBrush(NxColors.Blue)),
                CountdownGlow      = new NxBrush(NxColors.LightBlue),

                ListFore           = new NxBrush(new SolidColorBrush(NxColors.White)),
                ListBack           = new NxBrush(new SolidColorBrush(NxColors.LightBlue)),
                ListAltBack        = new NxBrush(new SolidColorBrush(NxColors.Blue)),
                ListBorder         = new NxBrush(new SolidColorBrush(NxColors.CornflowerBlue)),
                ListSelectedFore   = new NxBrush(new SolidColorBrush(NxColors.White)),
                ListSelectedBack   = new NxBrush(new SolidColorBrush(NxColors.SteelBlue)),
                ListPointerOver    = new NxBrush(new SolidColorBrush(NxColors.CornflowerBlue)),

                TabStripBack         = new NxBrush(new SolidColorBrush(NxColors.LightBlue)),
                TabStripFore         = new NxBrush(new SolidColorBrush(NxColors.White)),
                TabStripHover        = new NxBrush(new SolidColorBrush(NxColors.DodgerBlue )),
                TabStripSelectedBack = new NxBrush(new SolidColorBrush(NxColors.Blue)),
                TabStripSelectedFore = new NxBrush(new SolidColorBrush(NxColors.White)),
            }
        };

        public static NxColorThemes CurrentTheme => _currentTheme;
        public static NxTheme      Current      => _themes[_currentTheme];

        static NxThemeManager()
        {
            SetTheme(_currentTheme); // Apply the default theme at startup
        }

        public static void SetTheme(NxColorThemes theme)
        {
            _currentTheme = theme;

            var current = GetCurrentTheme();

            Application.Current.Resources["NxTransparent"]         = new NxBrush(NxColors.Transparent).Brush;

            Application.Current.Resources["NxPromptBack"]          = current.PromptBack.Brush;
            Application.Current.Resources["NxPromptFore"]          = current.PromptFore.Brush;

            Application.Current.Resources["NxReplyBack"]           = current.ReplyBack.Brush;
            Application.Current.Resources["NxReplyFore"]           = current.ReplyFore.Brush;
            Application.Current.Resources["NxReplyRequired"]       = current.ReplyRequired.Brush;
            Application.Current.Resources["NxReplyValid"]          = current.ReplyValid.Brush;
            Application.Current.Resources["NxReplyInValid"]        = current.ReplyInValid.Brush;

            Application.Current.Resources["NxMenuFore"]            = current.MenuFore.Brush;
            Application.Current.Resources["NxMenuBack"]            = current.MenuBack.Brush;
            Application.Current.Resources["NxMenuBorder"]          = current.MenuBorder.Brush;
            Application.Current.Resources["NxMenuGroupBack"]       = current.MenuGroupBack.Brush;
            Application.Current.Resources["NxMenuGroupHover"]      = current.MenuGroupHover.Brush;
            Application.Current.Resources["NxMenuItemFore"]        = current.MenuItemFore.Brush;
            Application.Current.Resources["NxMenuItemBack"]        = current.MenuItemBack.Brush;
            Application.Current.Resources["NxMenuItemHover"]       = current.MenuItemHover.Brush;
            Application.Current.Resources["NxMenuItemPressed"]     = current.MenuItemPressed.Brush;
            Application.Current.Resources["NxMenuCaptionBack"]     = current.MenuCaptionBack.Brush;
            Application.Current.Resources["NxMenuCaptionFore"]     = current.MenuCaptionFore.Brush;
            Application.Current.Resources["NxMenuDivider"]         = current.MenuDivider.Brush;

            Application.Current.Resources["NxPanelBack"]           = current.PanelBack.Brush;
            Application.Current.Resources["NxBorder"]              = current.Border.Brush;
            Application.Current.Resources["NxAccent"]              = current.Accent.Brush;

            Application.Current.Resources["NxButtonFore"]          = current.ButtonFore.Brush;
            Application.Current.Resources["NxButtonBack"]          = current.ButtonBack.Brush;
            Application.Current.Resources["NxButtonPointerOver"]   = current.ButtonPointerOver.Brush;
            Application.Current.Resources["NxButtonPressed"]       = current.ButtonPressed.Brush;

            Application.Current.Resources["NxProgressTrack"]       = current.ProgressTrack.Brush;
            Application.Current.Resources["NxProgressBar"]         = current.ProgressIndicator.Brush;
            Application.Current.Resources["NxCountdownGlow"]       = current.CountdownGlow.Brush;

            Application.Current.Resources["NxListFore"]            = current.ListFore.Brush;
            Application.Current.Resources["NxListBack"]            = current.ListBack.Brush;
            Application.Current.Resources["NxListAltBack"]         = current.ListAltBack.Brush;
            Application.Current.Resources["NxListBorder"]          = current.ListBorder.Brush;
            Application.Current.Resources["NxListSelectedFore"]    = current.ListSelectedFore.Brush;
            Application.Current.Resources["NxListSelectedBack"]    = current.ListSelectedBack.Brush;
            Application.Current.Resources["NxListPointerOver"]     = current.ListPointerOver.Brush;

            Application.Current.Resources["NxTabStripBack"]           = current.TabStripBack.Brush;
            Application.Current.Resources["NxTabStripFore"]           = current.TabStripFore.Brush;
            Application.Current.Resources["NxTabStripHover"]          = current.TabStripHover.Brush;
            Application.Current.Resources["NxTabStripSelectedBack"]   = current.TabStripSelectedBack.Brush;
            Application.Current.Resources["NxTabStripSelectedFore"]   = current.TabStripSelectedFore.Brush;

            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }

        private static RadialGradientBrush CreateRadialGradient(Color color1, double offset1, Color color2, double offset2)
        {
            var brush = new RadialGradientBrush();
            brush.GradientStops.Add(new GradientStop { Color = color1, Offset = offset1 });
            brush.GradientStops.Add(new GradientStop { Color = color2, Offset = offset2 });
            return brush;
        }

        private static NxBrush CreateReplyBrush(Color baseColor, Color endColor)
        {
            return new NxBrush(new LinearGradientBrush
            {
                StartPoint    = new Point(0, 1),
                EndPoint      = new Point(1, 0),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop { Color = baseColor, Offset = 0.418 },
                    new GradientStop { Color = endColor,  Offset = 1.0   }
                }
            });
        }
    }

    public class NxTheme
    {
        public string    Name              { get; set; } = string.Empty;
        public NxBrush   ButtonFore        { get; set; }
        public NxBrush   ButtonBack        { get; set; }
        public NxBrush   ButtonPointerOver { get; set; }
        public NxBrush   ButtonPressed     { get; set; }
        public NxBrush   ProgressTrack     { get; set; }
        public NxBrush   ProgressIndicator { get; set; }
        public NxBrush   CountdownGlow     { get; set; }
        public NxBrush   PromptBack        { get; set; }
        public NxBrush   PromptFore        { get; set; }
        public NxBrush   ReplyBack         { get; set; }
        public NxBrush   ReplyFore         { get; set; }
        public NxBrush   ReplyRequired     { get; set; }
        public NxBrush   ReplyValid        { get; set; }
        public NxBrush   ReplyInValid      { get; set; }
        public NxBrush   PanelBack         { get; set; }
        public NxBrush   Border            { get; set; }
        public NxBrush   Accent            { get; set; }

        public NxBrush   MenuFore          { get; set; }
        public NxBrush   MenuBack          { get; set; }
        public NxBrush   MenuBorder        { get; set; }
        public NxBrush   MenuGroupBack     { get; set; }
        public NxBrush   MenuGroupHover    { get; set; }
        public NxBrush   MenuItemBack      { get; set; }
        public NxBrush   MenuItemHover     { get; set; }
        public NxBrush   MenuItemPressed   { get; set; }
        public NxBrush   MenuItemFore      { get; set; }
        public NxBrush   MenuCaptionBack   { get; set; }
        public NxBrush   MenuCaptionFore   { get; set; }
        public NxBrush   MenuDivider       { get; set; }

        public NxBrush   ListFore          { get; set; }
        public NxBrush   ListBack          { get; set; }
        public NxBrush   ListAltBack       { get; set; }
        public NxBrush   ListBorder        { get; set; }
        public NxBrush   ListSelectedFore  { get; set; }
        public NxBrush   ListSelectedBack  { get; set; }
        public NxBrush   ListPointerOver   { get; set; }

        public NxBrush   TabStripBack         { get; set; }
        public NxBrush   TabStripFore         { get; set; }
        public NxBrush   TabStripHover        { get; set; }
        public NxBrush   TabStripSelectedBack { get; set; }
        public NxBrush   TabStripSelectedFore { get; set; }


        // Helper to resolve themed colors on demand
        public Brush GetThemeBrush(string resourceKey, Color fallback)
        {
            return new SolidColorBrush(GetThemeColor(resourceKey, fallback));
        }
        public Color GetThemeColor(string resourceKey, Color fallback)
        {

            object value;

            if (Application.Current.Resources.TryGetValue(resourceKey, out value))
            {
                if (value is SolidColorBrush color)
                {
                    SolidColorBrush brush = (SolidColorBrush)value;
                    return brush.Color;
                }
            }

            return fallback;
        }

    }

    public class NxBrush
    {
        public Brush Brush { get; }

        public NxBrush(Color color) => Brush = new SolidColorBrush(color);
        public NxBrush(Brush brush) => Brush = brush;

        public static implicit operator Brush(NxBrush b) => b.Brush;
        public static implicit operator NxBrush(Color c) => new NxBrush(c);
    }

    public static class NxColors
    {
        
        public static Color Gainsboro   => Color.FromArgb(255, 220, 220, 220);
        public static Color SteelBlue  => Color.FromArgb(255, 70, 130, 180);
        public static Color Transparent => Color.FromArgb(0, 0, 0, 0);

        public static Color Black       => Color.FromArgb(255, 0, 0, 0);
        public static Color White       => Color.FromArgb(255, 255, 255, 255);
        public static Color Red         => Color.FromArgb(255, 255, 0, 0);
        public static Color Green       => Color.FromArgb(255, 0, 255, 0);
        public static Color Blue        => Color.FromArgb(255, 0, 0, 255);

        public static Color Yellow      => Color.FromArgb(255, 255, 255, 0);
        public static Color Cyan        => Color.FromArgb(255, 0, 255, 255);
        public static Color Magenta     => Color.FromArgb(255, 255, 0, 255);

        public static Color Gray        => Color.FromArgb(255, 128, 128, 128);
        public static Color LightGray   => Color.FromArgb(255, 211, 211, 211);
        public static Color DarkGray    => Color.FromArgb(255, 64, 64, 64);

        public static Color Orange      => Color.FromArgb(255, 255, 165, 0);
        public static Color Pink        => Color.FromArgb(255, 255, 192, 203);
        public static Color Purple      => Color.FromArgb(255, 128, 0, 128);
        public static Color Brown       => Color.FromArgb(255, 165, 42, 42);

        public static Color Gold        => Color.FromArgb(255, 255, 215, 0);
        public static Color Silver      => Color.FromArgb(255, 192, 192, 192);

        public static Color Lime        => Color.FromArgb(255, 0, 255, 0);
        public static Color Olive       => Color.FromArgb(255, 128, 128, 0);
        public static Color Teal        => Color.FromArgb(255, 0, 128, 128);
        public static Color Navy        => Color.FromArgb(255, 0, 0, 128);
        public static Color Maroon      => Color.FromArgb(255, 128, 0, 0);
        public static Color Aqua        => Color.FromArgb(255, 0, 255, 255);
        public static Color Fuchsia     => Color.FromArgb(255, 255, 0, 255);

        public static Color DarkRed     => Color.FromArgb(255, 139, 0, 0);
        public static Color DarkGreen   => Color.FromArgb(255, 0, 100, 0);
        public static Color DarkBlue    => Color.FromArgb(255, 0, 0, 139);

        public static Color LightBlue   => Color.FromArgb(255, 173, 216, 230);
        public static Color LightGreen  => Color.FromArgb(255, 144, 238, 144);
        public static Color LightPink   => Color.FromArgb(255, 255, 182, 193);
        public static Color LightOrange => Color.FromArgb(255, 255, 222, 209);
        public static Color LightYellow => Color.FromArgb(255, 247, 242, 190);
        public static Color LightRed    => Color.FromArgb(255, 238, 144, 144);

        public static Color DodgerBlue  => Color.FromArgb(255, 30, 144, 255);  
        public static Color DimGray     => Color.FromArgb(255, 105, 105, 105); 
        public static Color DarkOrange  => Color.FromArgb(255, 255, 140, 0); 
        public static Color OrangeRed   => Color.FromArgb(255, 255, 69, 0);
        public static Color NextNetBlue => Color.FromArgb(255, 32, 84, 142);

        public static Color WhiteSmoke      => Color.FromArgb(255, 245, 245, 245);
        public static Color LightSteelBlue  => Color.FromArgb(255, 176, 196, 222);
        public static Color SlateGray       => Color.FromArgb(255, 112, 128, 144);
        public static Color DarkSlateGray   => Color.FromArgb(255, 47, 79, 79);
        public static Color Moccasin        => Color.FromArgb(255, 255, 228, 181);
        public static Color SkyBlue         => Color.FromArgb(255, 135, 206, 235);
        public static Color LightSkyBlue    => Color.FromArgb(255, 135, 206, 250);
        public static Color LightCyan       => Color.FromArgb(255, 224, 255, 255);
        public static Color DeepSkyBlue     => Color.FromArgb(255, 0, 191, 255);
        public static Color CornflowerBlue  => Color.FromArgb(255, 100, 149, 237);

        public static Color FromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Color name cannot be null or empty.", nameof(name));

            var prop = typeof(NxColors).GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);

            if (prop != null && prop.PropertyType == typeof(Color))
                return (Color)prop.GetValue(null)!;

            throw new ArgumentException($"Color '{name}' not found in NxColors.", nameof(name));
        }
    }
}

