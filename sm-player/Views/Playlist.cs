using System.Data;
using Terminal.Gui;
using sm_player.Models;

namespace sm_player.Views;

public class Playlist : View {
    List<Track> Tracks = new List<Track>();
    TableView _table;
    Player _player;

    public Playlist(Player player) {
        _table = new TableView {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Table = new DataTable()
        };
        _table.Table.Columns.Add("File");
        Add(_table);
        _player = player;
    }
    public void AddTrack(string path) {
        var track = new Track(path);
        Tracks.Add(track);
        _table.Table.Rows.Add(path);
        _player.SetTrack(track);
    }
}