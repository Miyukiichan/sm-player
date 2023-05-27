using HeyRed.Mime;
using sm_player.Models;
using Terminal.Gui;

namespace sm_player.Views.Tabs;

public class MusicTabNavigation : FrameView {
    Button _backButton;
    Button _addButton;
    Button _appendButton;
    MusicTab _tab;
    public MusicTabNavigation(MusicTab tab) {
        CanFocus = false;
        Height = 3;
        Width = Dim.Fill();
        _tab = tab;
        _backButton = new Button("<---");
        _backButton.X = 0;
        _backButton.Clicked += BackClicked;
        _addButton = new Button("Add");
        _addButton.Clicked += AddAll;
        _addButton.X = 9;
        _appendButton = new Button("Append");
        _appendButton.X = 17;
        _appendButton.Clicked += AppendAll;
        Add(_backButton, _addButton, _appendButton);
    }
    private void BackClicked() {
        _tab.Back();
    }
    private void AppendAll() {
        _tab.AppendAll();
    }
    private void AddAll() {
        _tab.AddAll();
    }
}

public class MusicTab : View {
    public string CurrentDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private ListView _lv;
    private MusicTabNavigation _nav;
    private List<string> _files = new List<string>();
    private Playlist _playlist;
    Settings _settings;

    public MusicTab(Playlist playlist, Settings settings) {
        _settings = settings;
        CanFocus = false;
        _playlist = playlist;
        Width = Dim.Fill();
        Height = Dim.Fill();
        _lv = new ListView {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Y = 3,
            AllowsMultipleSelection = false,
        };
        _lv.OpenSelectedItem += OpenSelected;
        _nav = new MusicTabNavigation(this);
        if (Directory.Exists(_settings.DefaultDir))
            CurrentDir = _settings.DefaultDir;
        Load();
        Add(_nav, _lv);
    }
    private void Load(string? dir = null) {
        if (dir is not null)
            CurrentDir = dir;
        var dirInfo = new DirectoryInfo(CurrentDir).GetDirectories();
        var d = dirInfo.Where(x => !x.Attributes.HasFlag(FileAttributes.Hidden)).Select(x => x.Name).Order();
        var fileInfo = new DirectoryInfo(CurrentDir).GetFiles();
        var f = fileInfo.Where(x => !x.Attributes.HasFlag(FileAttributes.Hidden) && MimeTypesMap.GetMimeType(x.Name).StartsWith("audio/")).Select(x => x.Name).Order();
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
            _playlist.AddTrack(path);
        }
    }

    public void Back() {
        var parent = Directory.GetParent(CurrentDir);
        if (parent == null) return;
        Load(parent.FullName);
    }
    public void AddAll() {
        var paths = _files.Select(x => Path.Combine(CurrentDir, x)).ToList();
        _playlist.SetTracks(paths);
    }
    public void AppendAll() {
        var paths = _files.Select(x => Path.Combine(CurrentDir, x)).ToList();
        _playlist.AddTracks(paths);
    }
}