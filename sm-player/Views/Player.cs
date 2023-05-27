using LibVLCSharp.Shared;
using Terminal.Gui;
using sm_player.Models;

namespace sm_player.Views;

public enum PlaybackMode { Repeat, RepeatSingle, NoRepeat, Shuffle }
public class Player : FrameView {
    public PlaybackMode PlaybackMode { get; set; } = PlaybackMode.Repeat ;
    public Track? Track { get; set; }
    Label _label;
    Button _playPauseButton;
    Button _repeatButton;
    LibVLC libvlc = new LibVLC(enableDebugLogs: false);
    private MediaPlayer _mediaPlayer { get; set; }
    public EventHandler<EventArgs> EndReached;
    public void InitMediaPlayer() {
        if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
            _mediaPlayer.Stop();
        _mediaPlayer = new MediaPlayer(libvlc);
        if (EndReached is not null)
            _mediaPlayer.EndReached += EndReached;
    }
    public Player() {
        Border.BorderStyle = BorderStyle.Rounded;
        Width = Dim.Fill();
        Height = 3;
        InitMediaPlayer();
        _label = new Label {
            Text = "No Track"
        };
        _playPauseButton = new Button("Play");
        _playPauseButton.X = Pos.Percent(100) - 20;
        _playPauseButton.Clicked += PlayPause;
        _repeatButton = new Button("Repeat");
        _repeatButton.X = Pos.Percent(100) - 10;
        _repeatButton.Clicked += RepeatToggle;
        Add(_label, _playPauseButton, _repeatButton);
    }

    public void SetTrack(Track? track) {
        if (track == null) return;
        Track = track;
        _label.Text = track.Name;
        InitMediaPlayer();
        var media = new Media(libvlc, new Uri(track.FullPath));
        _mediaPlayer.Media = media;
        _mediaPlayer.Play();
        _playPauseButton.Text = "Pause";
    }
    public void Stop() {
        _mediaPlayer.Stop();
        Track = null;
        _label.Text = "No Track";
        _playPauseButton.Text = "Play";
    }
    void PlayPause() {
        if (Track is null) return;
        if (_mediaPlayer.IsPlaying) {
            _mediaPlayer.Pause();
            _playPauseButton.Text = "Play";
        }
        else {
            _mediaPlayer.Play();
            _playPauseButton.Text = "Pause";
        }
    }

    private void RepeatToggle()
    {
        switch (PlaybackMode) {
            case (PlaybackMode.Repeat): {
                PlaybackMode = PlaybackMode.RepeatSingle;
                _repeatButton.Text = "Repeat Single";
                _playPauseButton.X = Pos.Percent(100) - 27;
                _repeatButton.X = Pos.Percent(100) - 17;
                break;
            }
            case (PlaybackMode.RepeatSingle): {
                PlaybackMode = PlaybackMode.NoRepeat;
                _repeatButton.Text = "No Repeat";
                _playPauseButton.X = Pos.Percent(100) - 23;
                _repeatButton.X = Pos.Percent(100) - 13;
                break;
            }
            case (PlaybackMode.NoRepeat): {
                PlaybackMode = PlaybackMode.Shuffle;
                _repeatButton.Text = "Shuffle";
                _playPauseButton.X = Pos.Percent(100) - 21;
                _repeatButton.X = Pos.Percent(100) - 11;
                break;
            }
            case (PlaybackMode.Shuffle): {
                PlaybackMode = PlaybackMode.Repeat;
                _repeatButton.Text = "Repeat";
                _playPauseButton.X = Pos.Percent(100) - 20;
                _repeatButton.X = Pos.Percent(100) - 10;
                break;
            }
        }
    }
}