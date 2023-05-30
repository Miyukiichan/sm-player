using System.Text.Json;

namespace sm_player.Models;

public class Settings {
    public string DefaultDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public bool EnableMusic { get; set; } = true;
    public bool EnablePodcasts { get; set; } = false;
    public bool EnableSpotify { get; set; } = false;
    public bool EnableStreams { get; set; } = false;
    public bool EnableYoutube { get; set; } = false;
    public void Save() {
        var parent = GetParentDir();
        if (!Directory.Exists(parent))
            Directory.CreateDirectory(parent);
        File.WriteAllText(ConfigPath(), JsonSerializer.Serialize(this));
    }
    string GetParentDir() {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "sm-player");
    }
    public string ConfigPath() {
        return Path.Combine(GetParentDir(), "app.config");
    }
}