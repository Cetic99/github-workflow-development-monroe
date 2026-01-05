using CashVault.Application.Interfaces;

namespace CashVault.Infrastructure;

public class AppInfoService : IAppInfoService
{
    public string Version { get; private set; } = string.Empty;

    public AppInfoService()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "monroe-meta.txt");

        if (!File.Exists(path))
        {
            return;
        }

        var lines = File.ReadAllLines(path);

        var dict = lines
            .Where(l => l.Contains('='))
            .Select(l =>
            {
                var parts = l.Split('=', 2);
                return new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim());
            })
            .ToDictionary(x => x.Key, x => x.Value);


        dict.TryGetValue(nameof(Version).ToLowerInvariant(), out var version);

        Version = version ?? "";
    }
}
