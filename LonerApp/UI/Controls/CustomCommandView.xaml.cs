using System.ComponentModel;

namespace LonerApp.UI.Controls;

[ContentProperty(nameof(TargetView))]
public partial class CustomCommandView : ContentView
{

    public static readonly BindableProperty TargetViewProperty =
        BindableProperty.Create(nameof(TargetView),
            typeof(View),
            typeof(CustomCommandView));

    public static readonly BindableProperty RootViewProperty =
        BindableProperty.Create(nameof(RootView),
            typeof(View),
            typeof(CustomCommandView));

    public View TargetView
    {
        get { return (View)GetValue(TargetViewProperty); }
        set { SetValue(TargetViewProperty, value); }
    }

    public View RootView
    {
        get { return (View)GetValue(RootViewProperty); }
        set { SetValue(RootViewProperty, value); }
    }

    private double _margin;

    public CustomCommandView()
	{
		InitializeComponent();
        CalculateMargin(this);

    }

    private void CalculateMargin(View view)
    {
        if (view.Parent != null)
        {
            var parent = view.Parent as View;
            if (parent == null)
            {
                Margin = new Thickness(-_margin, 0);
            }
            else
            {
                if (RootView == null && (parent.BackgroundColor != null && parent.BackgroundColor != Colors.Transparent))
                {
                    RootView = parent;
                }

                if (view.Parent is Grid)
                    _margin += (view.Parent as Grid).Padding.Left;
                if (view.Parent is ScrollView)
                    _margin += (view.Parent as ScrollView).Padding.Left;
                if (view.Parent is StackLayout)
                    _margin += (view.Parent as StackLayout).Padding.Left;
                if (view.Parent is Border)
                    _margin += (view.Parent as Border).Padding.Left;
                if (view.Parent is VerticalStackLayout)
                    _margin += (view.Parent as VerticalStackLayout).Padding.Left;
                if (view.Parent is AbsoluteLayout)
                    _margin += (view.Parent as AbsoluteLayout).Padding.Left;
                if (RootView == view.Parent)
                {
                    Margin = new Thickness(-_margin, 0);
                }
                else
                {
                    _margin += parent.Margin.Left;
                    CalculateMargin(parent);
                }
            }
        }
        else
        {
            view.PropertyChanged += OnChildPropertyChanged;
        }

        void OnChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var view = (View)sender;
            if (e.PropertyName == nameof(Parent) && view.Parent != null)
            {
                ((View)sender).PropertyChanged -= OnChildPropertyChanged;
                CalculateMargin(view);
            }
        }
    }
}