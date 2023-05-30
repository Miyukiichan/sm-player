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
    ProgressBar _progress;
    private MediaPlayer _mediaPlayer { get; set; }
    public EventHandler<EventArgs> EndReached;
    Logger logger = new Logger();
    public void InitMediaPlayer() {
        try {
            if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
                _mediaPlayer.Stop();
        }
        catch (Exception e) {
            logger.Log(e.Message);
        }
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
        _progress = new ProgressBar {
            X = Pos.Right(_label) + 2,
            Width = Dim.Fill() - 28,
            Height = 1,
            Fraction = 0,
        };
        Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(1), (mainLoop) => {
            if (_mediaPlayer is null) 
                _progress.Fraction = 0;
            else
                _progress.Fraction = _mediaPlayer.Position;
            return true;
        });
        _progress.MouseClick += ProgressClicked;
        Add(_label, _playPauseButton, _repeatButton, _progress);
    }

    public void SetTrack(Track? track) {
        if (track == null) return;
        Track = track;
        _label.Text = track.Name;
        InitMediaPlayer();
        var media = new Media(libvlc, new Uri(track.FullPath));
        try {
            _mediaPlayer.Media = media;
            _mediaPlayer.Play();
        }
        catch (Exception e) {
            logger.Log(e.Message);
        }
        _playPauseButton.Text = "Pause";
    }
    public void Stop() {
        try {
            _mediaPlayer.Stop();
        }
        catch (Exception e) {
            logger.Log(e.Message);
        }
        Track = null;
        _label.Text = "No Track";
        _playPauseButton.Text = "Play";
    }
    void PlayPause() {
        if (Track is null) return;
        try {
            if (_mediaPlayer.IsPlaying) {
                _mediaPlayer.Pause();
                _playPauseButton.Text = "Play";
            }
            else {
                _mediaPlayer.Play();
                _playPauseButton.Text = "Pause";
            }
        }
        catch (Exception e) {
            logger.Log(e.Message);
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
    void ProgressClicked(MouseEventArgs args) {
        if (args.MouseEvent.Flags.HasFlag(MouseFlags.Button1Clicked))
            _mediaPlayer.Position = (float)args.MouseEvent.X / (float)_progress.Bounds.Width;
    }
}