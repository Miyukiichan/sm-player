using HeyRed.Mime;
using Terminal.Gui;

namespace sm_player.Views.Tabs;

public class MusicTabNavigation : FrameView {
    Label _backButton;
    MusicTab _tab;
    public MusicTabNavigation(MusicTab tab) {
        CanFocus = false;
        Height = 3;
        Width = Dim.Fill();
        _tab = tab;
        _backButton = new Label("<---");
        _backButton.Width = Dim.Fill();
        _backButton.Height = Dim.Fill();
        _backButton.Clicked += BackClicked;
        _backButton.TextAlignment = TextAlignment.Centered;
        Add(_backButton);
    }
    private void BackClicked() {
        _tab.Back();
    }
}

public class MusicTab : View {
    public string CurrentDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private ListView _lv;
    private MusicTabNavigation _nav;
    private ContextMenu _cm;
    private List<string> _files = new List<string>();
    private Playlist _playlist;

    public MusicTab(Playlist playlist) {
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
        _lv.MouseClick += ListMouseEvent;
        _nav = new MusicTabNavigation(this);
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

    void ListMouseEvent(MouseEventArgs args) { 
        if (args.MouseEvent.Flags.HasFlag(MouseFlags.Button3Clicked))
            _cm.Show();
    }

    public void Back() {
        Load(Directory.GetParent(CurrentDir).FullName);
    }
}