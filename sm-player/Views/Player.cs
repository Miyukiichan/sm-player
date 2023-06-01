using LibVLCSharp.Shared;
using Terminal.Gui;
using sm_player.Models;

namespace sm_player.Views;

public enum PlaybackMode { Repeat, RepeatSingle, NoRepeat, Shuffle }

public class VolumeControls : FrameView {
    Button _up;
    Button _down;
    Label _value;
    Player _player;
    public VolumeControls(Player player) {
        Border.BorderStyle = BorderStyle.Rounded;
        _player = player;
        Height = 3;
        Width = 19;
        _down = new Button("-") {
            TextAlignment = TextAlignment.Centered,
            X = 1
        };
        _value = new Label("100") {
            TextAlignment = TextAlignment.Centered,
            X = 7
        };
        _up = new Button("+") {
            TextAlignment = TextAlignment.Centered,
            X = 11,
        };
        _down.Clicked += DecreaseVolume;
        _up.Clicked += IncreaseVolume;
        _value.MouseClick += VolumeChange;
        Add(_down, _up, _value);
    }
    void VolumeChange(MouseEventArgs args) {
        if (args.MouseEvent.Flags.HasFlag(MouseFlags.WheeledUp)) {
            IncreaseVolume();
        }
        else if (args.MouseEvent.Flags.HasFlag(MouseFlags.WheeledDown)) {
            DecreaseVolume();
        }
    }
    void IncreaseVolume() {
        SetVolume(_player.IncreaseVolume());
    }
    void DecreaseVolume() {
        SetVolume(_player.DecreaseVolume());
    }
    public void SetVolume(int value) {
        _value.Text = value.ToString();
        Application.Refresh();
    }
}

public class Player : FrameView {
    public PlaybackMode PlaybackMode { get; set; } = PlaybackMode.Repeat ;
    public Track? Track { get; set; }
    Label _label;
    Button _playPauseButton;
    Button _repeatButton;
    LibVLC libvlc = new LibVLC(enableDebugLogs: false);
    ProgressBar _progress;
    public MediaPlayer MediaPlayer { get; set; }
    public EventHandler<EventArgs> EndReached;
    Logger logger = new Logger();
    VolumeControls _volume;
    public int Volume { get; set; } = 100;
    public void InitMediaPlayer() {
        try {
            if (MediaPlayer != null && MediaPlayer.IsPlaying)
                MediaPlayer.Stop();
        }
        catch (Exception e) {
            logger.Log(e.Message);
        }
        Volume = 100;
        if (MediaPlayer != null)
            Volume = MediaPlayer.Volume;
        MediaPlayer = new MediaPlayer(libvlc);
        MediaPlayer.Volume = Volume;
        if (EndReached is not null)
            MediaPlayer.EndReached += EndReached;
    }
    public Player() {
        Border.BorderStyle = BorderStyle.Rounded;
        Width = Dim.Fill();
        Height = 7;
        _label = new Label {
            Text = "No Track",
            Y = 1,
            X = 1,
        };
        _playPauseButton = new Button("Play");
        _playPauseButton.X = Pos.Percent(100) - 21;
        _playPauseButton.Y = 1;
        _playPauseButton.Clicked += PlayPause;
        _repeatButton = new Button("Repeat");
        _repeatButton.X = Pos.Percent(100) - 11;
        _repeatButton.Y = 1;
        _repeatButton.Clicked += RepeatToggle;
        _progress = new ProgressBar {
            Y = 4,
            Width = Dim.Fill(),
            Height = 3,
            Fraction = 0,
        };
        Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(1), (mainLoop) => {
            if (MediaPlayer is null) 
                _progress.Fraction = 0;
            else
                _progress.Fraction = MediaPlayer.Position;
            return true;
        });
        _progress.MouseClick += ProgressClicked;
        _volume = new VolumeControls(this) {
            X = Pos.Left(_playPauseButton) - 21,
            Y = 0,
        };
        InitMediaPlayer();
        Add(_label, _progress, _volume, _playPauseButton, _repeatButton);
    }

    public void SetTrack(Track? track) {
        if (track == null) return;
        Track = track;
        _label.Text = track.Name;
        InitMediaPlayer();
        var media = new Media(libvlc, new Uri(track.FullPath));
        try {
            MediaPlayer.Media = media;
            MediaPlayer.Play();
            //Need to sleep here to allow the media player to determine the volume - otherwise it will be 0 or -1
            Thread.Sleep(100);
            _volume.SetVolume(MediaPlayer.Volume);
            Volume = MediaPlayer.Volume;
        }
        catch (Exception e) {
            logger.Log(e.Message);
        }
        _playPauseButton.Text = "Pause";
    }
    public void Stop() {
        try {
            MediaPlayer.Stop();
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
            if (MediaPlayer.IsPlaying) {
                MediaPlayer.Pause();
                _playPauseButton.Text = "Play";
            }
            else {
                MediaPlayer.Play();
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
                _playPauseButton.X = Pos.Percent(100) - 28;
                _repeatButton.X = Pos.Percent(100) - 18;
                break;
            }
            case (PlaybackMode.RepeatSingle): {
                PlaybackMode = PlaybackMode.NoRepeat;
                _repeatButton.Text = "No Repeat";
                _playPauseButton.X = Pos.Percent(100) - 24;
                _repeatButton.X = Pos.Percent(100) - 14;
                break;
            }
            case (PlaybackMode.NoRepeat): {
                PlaybackMode = PlaybackMode.Shuffle;
                _repeatButton.Text = "Shuffle";
                _playPauseButton.X = Pos.Percent(100) - 20;
                _repeatButton.X = Pos.Percent(100) - 10;
                break;
            }
            case (PlaybackMode.Shuffle): {
                PlaybackMode = PlaybackMode.Repeat;
                _repeatButton.Text = "Repeat";
                _playPauseButton.X = Pos.Percent(100) - 19;
                _repeatButton.X = Pos.Percent(100) - 9;
                break;
            }
        }
    }
    void ProgressClicked(MouseEventArgs args) {
        if (args.MouseEvent.Flags.HasFlag(MouseFlags.Button1Clicked))
            MediaPlayer.Position = (float)args.MouseEvent.X / (float)_progress.Bounds.Width;
    }

    public int IncreaseVolume(int value = 5) {
        if (Volume + value > 100)
            Volume = 100;
        else
            Volume = Volume + value;
        MediaPlayer.Volume = Volume;
        return Volume;
    }

    public int DecreaseVolume(int value = 5) {
        if (Volume < value)
            Volume = 0;
        else
            Volume = Volume - value;
        MediaPlayer.Volume = Volume;
        return Volume;
    }
}