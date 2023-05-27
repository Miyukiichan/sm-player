namespace sm_player.Models;

public class Settings {
    public string DefaultDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public bool EnableMusic { get; set; } = true;
    public bool EnablePodcasts { get; set; } = false;
    public bool EnableSpotify { get; set; } = false;
    public bool EnableStreams { get; set; } = false;
    public bool EnableYoutube { get; set; } = false;
}