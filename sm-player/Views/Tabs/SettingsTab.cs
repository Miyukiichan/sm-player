using sm_player.Models;
using Terminal.Gui;
using System.Text.Json;

namespace sm_player.Views.Tabs;

public class SettingsTab : FrameView {
    public Settings Settings { get; set; }
    TextField _dirEdit;
    CheckBox _musicEnable;
    CheckBox _podcastsEnable;
    CheckBox _spotifyEnable;
    CheckBox _streamsEnable;
    CheckBox _youtubeEnable;
    public SettingsTab() {
        Height = Dim.Fill();
        Width = Dim.Fill();
        Settings = new Settings();
        var path = Settings.ConfigPath();
        if (File.Exists(path))
        {
            Settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(path));
        }
        else
        {
            Settings.Save();
        }

        var dirlabel = new Label("Default Directory:");
        var dirButton = new Button("Browse") {
            X = Pos.Right(dirlabel) + 2,
        };
        dirButton.Clicked += BrowseDir;
        _dirEdit = new TextField(Settings.DefaultDir) {
            Y = 2,
            Width = Dim.Fill()
        };
        var musicLabel = new Label("Enable Music:") {
            Y = 5,
        };
        _musicEnable = new CheckBox {
            X = Pos.Right(musicLabel) + 2,
            Y = 5,
        };
        var podcastLabel = new Label("Enable Podcasts:") {
            Y = 6,
        };
        _podcastsEnable = new CheckBox {
            X = Pos.Right(podcastLabel) + 2,
            Y = 6,
        };
        var spotifyLabel = new Label("Enable Spotify:") {
            Y = 7,
        };
        _spotifyEnable = new CheckBox {
            X = Pos.Right(spotifyLabel) + 2,
            Y = 7,
        };
        var streamsLabel = new Label("Enable Streams:") {
            Y = 8,
        };
        _streamsEnable = new CheckBox {
            X = Pos.Right(streamsLabel) + 2,
            Y = 8,
        };

        var youtubeLabel = new Label("Enable Youtube:") {
            Y = 9,
        };
        _youtubeEnable = new CheckBox {
            X = Pos.Right(youtubeLabel) + 2,
            Y = 9,
        };
        var saveButton = new Button("Save") {
            Y = 11
        };
        saveButton.Clicked += Save;

        //Load
        _dirEdit.Text = Settings.DefaultDir;
        _musicEnable.Checked = Settings.EnableMusic;
        _podcastsEnable.Checked = Settings.EnablePodcasts;
        _spotifyEnable.Checked = Settings.EnableSpotify;
        _streamsEnable.Checked = Settings.EnableStreams;
        _youtubeEnable.Checked = Settings.EnableYoutube;

        Add(
            dirlabel, 
            dirButton, 
            _dirEdit, 
            musicLabel, 
            _musicEnable, 
            podcastLabel,
            _podcastsEnable,
            spotifyLabel,
            _spotifyEnable,
            streamsLabel,
            _streamsEnable,
            youtubeLabel,
            _youtubeEnable,
            saveButton
        );
    }
    void Save() {
        Settings.DefaultDir = _dirEdit.Text.ToString();
        Settings.EnableMusic = _musicEnable.Checked;
        Settings.EnablePodcasts = _podcastsEnable.Checked;
        Settings.EnableSpotify = _spotifyEnable.Checked;
        Settings.EnableStreams = _streamsEnable.Checked;
        Settings.EnableYoutube = _youtubeEnable.Checked;
        Settings.Save();
    }
    void BrowseDir() {
        var dialog = new OpenDialog();
        dialog.CanChooseDirectories = true;
        dialog.CanChooseFiles = false;
        dialog.AllowsMultipleSelection = false;
        dialog.DirectoryPath = _dirEdit.Text;
        dialog.ColorScheme = Colors.Menu;
        Application.Run(dialog);
        if (dialog.Canceled) return;
        _dirEdit.Text = dialog.FilePath;
    }
}