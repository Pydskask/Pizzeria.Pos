using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Pizzeria.Pos.Wpf.Views.Controls;

public partial class NumericKeypadControl : UserControl
{
    public NumericKeypadControl()
    {
        InitializeComponent();
    }

    public ICommand? DigitCommand
    {
        get => (ICommand?)GetValue(DigitCommandProperty);
        set => SetValue(DigitCommandProperty, value);
    }

    public static readonly DependencyProperty DigitCommandProperty =
        DependencyProperty.Register(nameof(DigitCommand), typeof(ICommand), typeof(NumericKeypadControl));

    public ICommand? LeftCommand
    {
        get => (ICommand?)GetValue(LeftCommandProperty);
        set => SetValue(LeftCommandProperty, value);
    }

    public static readonly DependencyProperty LeftCommandProperty =
        DependencyProperty.Register(nameof(LeftCommand), typeof(ICommand), typeof(NumericKeypadControl));

    public object? LeftCommandParameter
    {
        get => GetValue(LeftCommandParameterProperty);
        set => SetValue(LeftCommandParameterProperty, value);
    }

    public static readonly DependencyProperty LeftCommandParameterProperty =
        DependencyProperty.Register(nameof(LeftCommandParameter), typeof(object), typeof(NumericKeypadControl));

    public string LeftButtonText
    {
        get => (string)GetValue(LeftButtonTextProperty);
        set => SetValue(LeftButtonTextProperty, value);
    }

    public static readonly DependencyProperty LeftButtonTextProperty =
        DependencyProperty.Register(nameof(LeftButtonText), typeof(string), typeof(NumericKeypadControl), new PropertyMetadata("C"));

    public Brush LeftButtonBackground
    {
        get => (Brush)GetValue(LeftButtonBackgroundProperty);
        set => SetValue(LeftButtonBackgroundProperty, value);
    }

    public static readonly DependencyProperty LeftButtonBackgroundProperty =
        DependencyProperty.Register(nameof(LeftButtonBackground), typeof(Brush), typeof(NumericKeypadControl), new PropertyMetadata(Brushes.Gray));

    public Brush LeftButtonForeground
    {
        get => (Brush)GetValue(LeftButtonForegroundProperty);
        set => SetValue(LeftButtonForegroundProperty, value);
    }

    public static readonly DependencyProperty LeftButtonForegroundProperty =
        DependencyProperty.Register(nameof(LeftButtonForeground), typeof(Brush), typeof(NumericKeypadControl), new PropertyMetadata(Brushes.White));

    public ICommand? RightCommand
    {
        get => (ICommand?)GetValue(RightCommandProperty);
        set => SetValue(RightCommandProperty, value);
    }

    public static readonly DependencyProperty RightCommandProperty =
        DependencyProperty.Register(nameof(RightCommand), typeof(ICommand), typeof(NumericKeypadControl));

    public object? RightCommandParameter
    {
        get => GetValue(RightCommandParameterProperty);
        set => SetValue(RightCommandParameterProperty, value);
    }

    public static readonly DependencyProperty RightCommandParameterProperty =
        DependencyProperty.Register(nameof(RightCommandParameter), typeof(object), typeof(NumericKeypadControl));

    public string RightButtonText
    {
        get => (string)GetValue(RightButtonTextProperty);
        set => SetValue(RightButtonTextProperty, value);
    }

    public static readonly DependencyProperty RightButtonTextProperty =
        DependencyProperty.Register(nameof(RightButtonText), typeof(string), typeof(NumericKeypadControl), new PropertyMetadata("OK"));

    public Brush RightButtonBackground
    {
        get => (Brush)GetValue(RightButtonBackgroundProperty);
        set => SetValue(RightButtonBackgroundProperty, value);
    }

    public static readonly DependencyProperty RightButtonBackgroundProperty =
        DependencyProperty.Register(nameof(RightButtonBackground), typeof(Brush), typeof(NumericKeypadControl), new PropertyMetadata(Brushes.SeaGreen));

    public Brush RightButtonForeground
    {
        get => (Brush)GetValue(RightButtonForegroundProperty);
        set => SetValue(RightButtonForegroundProperty, value);
    }

    public static readonly DependencyProperty RightButtonForegroundProperty =
        DependencyProperty.Register(nameof(RightButtonForeground), typeof(Brush), typeof(NumericKeypadControl), new PropertyMetadata(Brushes.White));

    public Brush NumberButtonBackground
    {
        get => (Brush)GetValue(NumberButtonBackgroundProperty);
        set => SetValue(NumberButtonBackgroundProperty, value);
    }

    public static readonly DependencyProperty NumberButtonBackgroundProperty =
        DependencyProperty.Register(nameof(NumberButtonBackground), typeof(Brush), typeof(NumericKeypadControl), new PropertyMetadata(Brushes.DodgerBlue));

    public double ButtonHeight
    {
        get => (double)GetValue(ButtonHeightProperty);
        set => SetValue(ButtonHeightProperty, value);
    }

    public static readonly DependencyProperty ButtonHeightProperty =
        DependencyProperty.Register(nameof(ButtonHeight), typeof(double), typeof(NumericKeypadControl), new PropertyMetadata(60d));

    public double ButtonFontSize
    {
        get => (double)GetValue(ButtonFontSizeProperty);
        set => SetValue(ButtonFontSizeProperty, value);
    }

    public static readonly DependencyProperty ButtonFontSizeProperty =
        DependencyProperty.Register(nameof(ButtonFontSize), typeof(double), typeof(NumericKeypadControl), new PropertyMetadata(26d));

    public Thickness KeypadMargin
    {
        get => (Thickness)GetValue(KeypadMarginProperty);
        set => SetValue(KeypadMarginProperty, value);
    }

    public static readonly DependencyProperty KeypadMarginProperty =
        DependencyProperty.Register(nameof(KeypadMargin), typeof(Thickness), typeof(NumericKeypadControl), new PropertyMetadata(new Thickness(0)));
}