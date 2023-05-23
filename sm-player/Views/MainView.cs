using sm_player.Views.Tabs;
using Terminal.Gui;

namespace sm_player.Views;

public class MainTabs : FrameView {
    public MainTabs(Playlist playlist) {
        Width = Dim.Percent(50);
        Height = Dim.Fill() - 3;
        Border.BorderStyle = BorderStyle.Rounded;
        var tabs = new TabView {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        tabs.Style.ShowBorder = false;
        tabs.AddTab(new TabView.Tab("Music", new MusicTab(playlist)), false);
        tabs.AddTab(new TabView.Tab("Podcasts", new PodcastTab()), false);
        tabs.AddTab(new TabView.Tab("Spotify", new SpotifyTab()), false);
        tabs.AddTab(new TabView.Tab("Youtube", new YoutubeTab()), false);
        tabs.AddTab(new TabView.Tab("Streams", new StreamTab()), false);
        tabs.SelectedTab = tabs.Tabs.First ();
        Add(tabs);
    }
}

public class MainView: View {
    private Playlist _playlist;

    public MainView() {
        Width = Dim.Fill();
        Height = Dim.Fill();
        var player = new Player ();
        player.Y = Pos.Bottom(this) - 3;
        _playlist = new Playlist(player) {
            Width = Dim.Percent(50),
            Height = Dim.Fill() - 3,
            X = Pos.Percent(50)
        };
        var tabs = new MainTabs(_playlist);
        Add(tabs, _playlist, player);
    }
}