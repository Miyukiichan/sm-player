using System.Data;
using Terminal.Gui;
using sm_player.Models;
using HeyRed.Mime;

namespace sm_player.Views;

public class Playlist : FrameView {
    List<Track> Tracks = new List<Track>();
    TableView _table;
    Player _player;

    public Playlist(Player player) {
        this.Border.BorderStyle = BorderStyle.Rounded;
        _table = new TableView {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Table = new DataTable()
        };
        _table.Table.Columns.Add("File");
        _table.CellActivated += CellActivated;
        Add(_table);
        _player = player;
    }
    public void AddTrack(string path) {
        var type = MimeTypesMap.GetMimeType(path);
        if (!type.StartsWith("audio/")) return;
        var track = new Track(path);
        Tracks.Add(track);
        _table.Table.Rows.Add(path);
        _table.SelectedRow = Tracks.Count() - 1;
        _player.SetTrack(track);
    }
    void CellActivated(TableView.CellActivatedEventArgs args) {
        var track = Tracks.ElementAt(args.Row);
        _player.SetTrack(track);
    }
}