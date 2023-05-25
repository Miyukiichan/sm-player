using System.Data;
using Terminal.Gui;
using sm_player.Models;
using HeyRed.Mime;

namespace sm_player.Views;

public class PlaylistControls : FrameView {
    Button _removeButton;
    Button _removeAllButton;
    Playlist _playlist;
    public PlaylistControls(Playlist playlist) {
        CanFocus = false;
        Height = 3;
        Width = Dim.Fill();
        _playlist = playlist;
        _removeButton = new Button("Remove");
        _removeButton.X = 0;
        _removeButton.Clicked += RemoveTrack;
        _removeAllButton = new Button("Remove All");
        _removeAllButton.X = 11;
        _removeAllButton.Clicked += RemoveAllTracks;
        Add(_removeButton, _removeAllButton);
    }
    void RemoveTrack() {
        _playlist.RemoveTrack();
    }
    void RemoveAllTracks() {
        _playlist.RemoveAllTracks();
    }
}


public class Playlist : FrameView {
    List<Track> Tracks = new List<Track>();
    TableView _table;
    Player _player;
    PlaylistControls _controls;

    public Playlist(Player player) {
        _controls = new PlaylistControls(this);
        this.Border.BorderStyle = BorderStyle.Rounded;
        _table = new TableView {
            Y = 3,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Table = new DataTable()
        };
        _table.Table.Columns.Add("Track");
        _table.CellActivated += CellActivated;
        Add(_controls, _table);
        _player = player;
    }
    public Track AddTrack(string path, bool noplay = false) {
        if (new DirectoryInfo(path).Attributes.HasFlag(FileAttributes.Directory)) return null;
        var type = MimeTypesMap.GetMimeType(path);
        if (!type.StartsWith("audio/")) return null;
        var track = new Track(path);
        Tracks.Add(track);
        _table.Table.Rows.Add(track.Name);
        if (!noplay) {
            _player.SetTrack(track);
            _table.SelectedRow = Tracks.Count() - 1;
        }
        return track;
    }
    public void AddTracks(List<string> paths, bool noplay = true) {
        Track? track = null;
        foreach (var path in paths) {
            var t = AddTrack(path, true);
            if (track == null)
                track = t;
        }
        if (track is null) return; 
        if (!noplay) {
            _player.SetTrack(track);
            _table.SelectedRow = Tracks.IndexOf(track);
        }
        else {
            //Focus to table to refresh the content
            _table.SetFocus();
        }
    }
    public void SetTracks(List<string> paths) {
        _player.Stop();
        Tracks.Clear();
        _table.Table.Clear();
        AddTracks(paths, false);
    }
    void CellActivated(TableView.CellActivatedEventArgs args) {
        var track = Tracks.ElementAt(args.Row);
        _player.SetTrack(track);
    }

    public void RemoveTrack() {
        if (!Tracks.Any()) return;
        var trackIdx = _table.SelectedRow;
        var track = Tracks.ElementAt(trackIdx);
        _player.Stop();
        _table.Table.Rows.RemoveAt(trackIdx);
        Tracks.Remove(track);
        _table.SelectedRow = trackIdx;
        var newTrack = Tracks.ElementAtOrDefault(trackIdx);
        if (newTrack is not null) 
            _player.SetTrack(newTrack);
    }

    public void RemoveAllTracks() {
        Tracks.Clear();
        _table.Table.Rows.Clear();
        _player.Stop();
    }
}