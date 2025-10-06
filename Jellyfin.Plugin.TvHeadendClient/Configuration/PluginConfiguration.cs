using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.TvHeadendClient.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public PluginConfiguration()
    {
        Host = "localhost";
        Port = 9981;
        UseSsl = false;
        Username = string.Empty;
        Password = string.Empty;
        HideRecordingsChannel = false;
        ForceAllProgramsAsSeries = false;
    }

    public string Host { get; set; }
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool HideRecordingsChannel { get; set; }
    public bool ForceAllProgramsAsSeries { get; set; }
}
