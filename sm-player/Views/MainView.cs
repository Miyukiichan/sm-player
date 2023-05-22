using sm_player.Views.Tabs;
using Terminal.Gui;

namespace sm_player.Views;

public class MainView: View {
    private Playlist _playlist;

    public MainView() {
        Width = Dim.Fill();
        Height = Dim.Fill();
        var player = new Player ();
        _playlist = new Playlist(player) {
            Width = Dim.Percent(50),
            Height = Dim.Percent(90),
            X = Pos.Percent(50)
        };
        var tabs = new TabView {
            Width = Dim.Percent(50),
            Height = Dim.Percent(90),
        };
        tabs.AddTab(new TabView.Tab("Music", new MusicTab(_playlist)), false);
        tabs.AddTab(new TabView.Tab("Podcasts", new PodcastTab()), false);
        tabs.AddTab(new TabView.Tab("Spotify", new SpotifyTab()), false);
        tabs.AddTab(new TabView.Tab("Youtube", new YoutubeTab()), false);
        tabs.AddTab(new TabView.Tab("Streams", new StreamTab()), false);
        tabs.SelectedTab = tabs.Tabs.First ();
        Add(tabs, _playlist, player);
    }
}