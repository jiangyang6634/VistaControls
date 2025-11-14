using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VistaControls
{
    public class VistaCard : ContentControl
    {
        static VistaCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaCard),
                new FrameworkPropertyMetadata(typeof(VistaCard)));
        }

        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(VistaCard),
                new PropertyMetadata(null));

        public DataTemplate? HeaderTemplate
        {
            get => (DataTemplate?)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(VistaCard),
                new PropertyMetadata(null));

        public Thickness BodyPadding
        {
            get => (Thickness)GetValue(BodyPaddingProperty);
            set => SetValue(BodyPaddingProperty, value);
        }

        public static readonly DependencyProperty BodyPaddingProperty =
            DependencyProperty.Register(nameof(BodyPadding), typeof(Thickness), typeof(VistaCard),
                new PropertyMetadata(new Thickness(20)));

        public Brush BodyBackground
        {
            get => (Brush)GetValue(BodyBackgroundProperty);
            set => SetValue(BodyBackgroundProperty, value);
        }

        public static readonly DependencyProperty BodyBackgroundProperty =
            DependencyProperty.Register(nameof(BodyBackground), typeof(Brush), typeof(VistaCard),
                new PropertyMetadata(Brushes.Transparent));

        public CardShadow Shadow
        {
            get => (CardShadow)GetValue(ShadowProperty);
            set => SetValue(ShadowProperty, value);
        }

        public static readonly DependencyProperty ShadowProperty =
            DependencyProperty.Register(nameof(Shadow), typeof(CardShadow), typeof(VistaCard),
                new PropertyMetadata(CardShadow.Always));
    }

    public enum CardShadow
    {
        Always,
        Hover,
        Never
    }
}

