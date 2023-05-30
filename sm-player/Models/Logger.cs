namespace sm_player.Models;

public class Logger {
    public string LogFile { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "sm-player", "log.txt");
    public void Log(string text) {
        File.AppendAllText(LogFile, text);
    }
}