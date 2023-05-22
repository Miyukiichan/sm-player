using Terminal.Gui;
using sm_player.Views;

Application.Init();

try
{
    Application.Top.Add(new MainView());
    Application.Run();
}
finally
{
    Application.Shutdown();
}
