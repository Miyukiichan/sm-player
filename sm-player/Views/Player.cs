using LibVLCSharp.Shared;
using Terminal.Gui;
using sm_player.Models;

namespace sm_player.Views;

public class Player : FrameView {
    Track? Track;
    Label _label;
    LibVLC libvlc = new LibVLC(enableDebugLogs: false);
    MediaPlayer _mediaPlayer;
    public Player() {
        Border.BorderStyle = BorderStyle.Rounded;
        Width = Dim.Fill();
        Height = 3;
         _mediaPlayer = new MediaPlayer(libvlc);
        _label = new Label {
            Text = "No Track"
        };
        Add(_label);
    }
    public void SetTrack(Track track) {
        Track = track;
        _label.Text = track.Path;
        var media = new Media(libvlc, new Uri(track.Path));
        _mediaPlayer.Play(media);
    }
}