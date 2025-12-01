namespace GaiaAgent.Pages;

public class AgentPage : ContentPage
{
    public AgentPage()
    {
        Content = new VerticalStackLayout
        {
            Children =
            {
                new Label
                {
                    Text = "Gaia is listening...",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            }
        };
    }
}
