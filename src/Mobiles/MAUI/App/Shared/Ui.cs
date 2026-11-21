using Microsoft.Maui.Controls.Shapes;

namespace ClassifiedAds.Mobile.Shared;

public static class Ui
{
    public static Color Primary => Color.FromArgb("#4f46e5");

    public static Label Text(string text, bool heading = false)
    {
        var label = new Label
        {
            Text = text,
            FontSize = heading ? 20 : 14,
            FontFamily = heading ? "OpenSansSemibold" : "OpenSansRegular"
        };
        label.SetAppThemeColor(Label.TextColorProperty, Color.FromArgb("#172554"), Color.FromArgb("#e2e8f0"));
        if (heading)
        {
            SemanticProperties.SetHeadingLevel(label, SemanticHeadingLevel.Level1);
        }

        return label;
    }

    public static Button Button(string title, string icon, Func<Task> action, bool danger = false)
    {
        var button = new Button
        {
            Text = title,
            FontSize = 14,
            FontFamily = "OpenSansSemibold",
            CornerRadius = 10,
            MinimumHeightRequest = 48,
            Padding = new Thickness(12, 8),
            BorderWidth = 1,
            ImageSource = new FileImageSource
            {
                File = "action_" + icon + ".png"
            },
            ContentLayout = new Microsoft.Maui.Controls.Button.ButtonContentLayout(Microsoft.Maui.Controls.Button.ButtonContentLayout.ImagePosition.Left, 8)
        };
        // The shared monochrome assets sit on a light button surface in both themes.
        button.BackgroundColor = danger ? Color.FromArgb("#fee2e2") : Color.FromArgb("#f8fafc");
        button.TextColor = danger ? Color.FromArgb("#b91c1c") : Color.FromArgb("#172554");
        button.BorderColor = danger ? Color.FromArgb("#fca5a5") : Color.FromArgb("#cbd5e1");
        SemanticProperties.SetDescription(button, title);
        var running = false;
        button.Clicked += async (_, _) =>
        {
            if (running)
            {
                return;
            }

            running = true;
            button.Opacity = 0.6;
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                await Dialog.Message("Error", ex.Message);
            }
            finally
            {
                running = false;
                button.Opacity = 1;
            }
        };
        return button;
    }

    public static VerticalStackLayout Stack(params View[] children)
    {
        var stack = new VerticalStackLayout
        {
            Spacing = 12
        };
        foreach (var child in children)
        {
            stack.Add(child);
        }

        return stack;
    }

    public static Grid Actions(params View[] children)
    {
        var grid = new Grid
        {
            ColumnSpacing = 12
        };
        for (var i = 0; i < children.Length; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            grid.Add(children[i], i);
        }

        return grid;
    }

    public static Border Card(View content)
    {
        var border = new Border
        {
            Content = content,
            Padding = 16,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 16
            }
        };
        border.SetAppThemeColor(VisualElement.BackgroundColorProperty, Colors.White, Color.FromArgb("#172033"));
        border.SetAppTheme<Brush>(Border.StrokeProperty, new SolidColorBrush(Color.FromArgb("#e2e8f0")), new SolidColorBrush(Color.FromArgb("#334155")));
        return border;
    }

    public static Entry Field(VerticalStackLayout form, string label, string? value, Action<string> change, bool password = false)
    {
        var input = new Entry
        {
            Text = value ?? "",
            Placeholder = label,
            IsPassword = password,
            MinimumHeightRequest = 48
        };
        input.SetAppThemeColor(Entry.TextColorProperty, Color.FromArgb("#172554"), Color.FromArgb("#e2e8f0"));
        // Placeholder supplies the accessible label without replacing editable text on Android.
        input.TextChanged += (_, e) => change(e.NewTextValue ?? "");
        form.Add(Text(label));
        form.Add(input);
        return input;
    }

    public static Switch Toggle(VerticalStackLayout form, string label, bool value, Action<bool> change)
    {
        var toggle = new Switch
        {
            IsToggled = value,
            OnColor = Primary
        };
        SemanticProperties.SetDescription(toggle, label);
        toggle.Toggled += (_, e) => change(e.Value);
        form.Add(Actions(Text(label), toggle));
        return toggle;
    }
    public static View DateTimePicker(Entry target)
    {
        return Button("Pick date", "calendar", async () =>
        {
            var selected = DateTimeOffset.TryParse(target.Text, out var current) ? current.LocalDateTime : DateTime.Now;
            var date = new DatePicker { Date = selected.Date };
            var time = new TimePicker { Time = selected.TimeOfDay };
            SemanticProperties.SetDescription(date, "Lockout date");
            SemanticProperties.SetDescription(time, "Lockout time");
            var dialog = new Dialog("Lockout end", () =>
            {
                target.Text = new DateTimeOffset((date.Date ?? DateTime.Today).Add(time.Time ?? TimeSpan.Zero)).ToString("O");
                return Task.FromResult(true);
            });
            dialog.Body.Add(date);
            dialog.Body.Add(time);
            dialog.Body.Add(Button("Clear date", "close", async () =>
            {
                target.Text = "";
                await dialog.Close();
            }));
            await dialog.Show();
        });
    }

}
