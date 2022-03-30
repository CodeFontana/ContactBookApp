using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfUI.Controls;

public class TextboxWithPreview : TextBox
{
    static TextboxWithPreview()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TextboxWithPreview), new FrameworkPropertyMetadata(typeof(TextboxWithPreview)));
    }

    public static DependencyProperty TextPreviewProperty = DependencyProperty.Register(
        "TextPreview", 
        typeof(string), 
        typeof(TextboxWithPreview));

    public string TextPreview
    {
        get { return (string)GetValue(TextPreviewProperty); }
        set { SetValue(TextPreviewProperty, value); }
    }

    public static readonly DependencyProperty HasTextProperty =
                DependencyProperty.Register("HasText",
                    typeof(bool),
                    typeof(TextboxWithPreview),
                    new FrameworkPropertyMetadata(false,
                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault 
                        | FrameworkPropertyMetadataOptions.AffectsRender));

    public bool HasText
    {
        get
        {
            return (bool)GetValue(HasTextProperty);
        }
        set
        {
            SetValue(HasTextProperty, value);
        }
    }

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text) == false)
        {
            HasText = true;
        }
        else
        {
            HasText = false;
        }

        base.OnTextInput(e);    
    }

    protected override void OnTextChanged(TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(Text) == false)
        {
            HasText = true;
        }
        else
        {
            HasText = false;
        }

        base.OnTextChanged(e);
    }
}
