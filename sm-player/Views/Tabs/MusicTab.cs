using Terminal.Gui;

namespace sm_player.Views.Tabs;

public class MusicTab : View {
    public string CurrentDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private ListView _lv;
    private ContextMenu _cm;
    private List<string> _files = new List<string>();
    private Playlist _playlist;

    public MusicTab(Playlist playlist) {
        _playlist = playlist;
        Width = Dim.Fill();
        Height = Dim.Fill();
        _lv = new ListView {
            Width = Dim.Fill(1),
            Height = Dim.Fill(),
            AllowsMultipleSelection = true,
        };
        _lv.OpenSelectedItem += OpenSelected;
        _lv.MouseClick += ListMouseEvent;
        _cm = new ContextMenu(_lv, new MenuBarItem("Select All", "", () => {} ));
        Load();
        Add(_lv);
    }
    private void Load(string? dir = null) {
        if (dir is not null)
            CurrentDir = dir;
        var dirInfo = new DirectoryInfo(CurrentDir).GetDirectories();
        var d = dirInfo.Where(x => !x.Attributes.HasFlag(FileAttributes.Hidden)).Select(x => x.Name).Order();
        var fileInfo = new DirectoryInfo(CurrentDir).GetFiles();
        var f = fileInfo.Where(x => !x.Attributes.HasFlag(FileAttributes.Hidden)).Select(x => x.Name).Order();
        _files = d.Concat(f).ToList();
        _lv.SetSource(_files);
    }

    void OpenSelected(ListViewItemEventArgs args) {
        var item = args.Value as string;
        if (item is null) return;
        var path = Path.Combine(CurrentDir, item);
        if (!Path.Exists(path)) return;
        var attr = File.GetAttributes(path);
        if (attr.HasFlag(FileAttributes.Directory)) {
            Load(path);
        }
        else {
            //Add selection to playlist
            _playlist.AddTrack(path);
        }
    }

    void ListMouseEvent(MouseEventArgs args) { 
        if (args.MouseEvent.Flags.HasFlag(MouseFlags.Button3Clicked))
            _cm.Show();
    }
}