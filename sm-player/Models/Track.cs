namespace sm_player.Models;

public class Track {
    public string FullPath;
    public string Name;
    public Track(string path) {
        FullPath = path;
        Name = Path.GetFileNameWithoutExtension(FullPath);
    }
}