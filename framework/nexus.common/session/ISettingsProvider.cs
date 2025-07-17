namespace nexus.common
{
    public interface ISettingsProvider
    {
        string? Get(string key);
        string GetString(string key, string fallback = "");
        int GetInt(string key, int fallback = 0);
        bool GetBool(string key, bool fallback = false);
        double GetDouble(string key, double fallback = 0);
        DateTime GetDateTime(string key, DateTime fallback);
    }
}
