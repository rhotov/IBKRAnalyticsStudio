using System.Text.Json;
using System.Text.Json.Serialization;

namespace IBKRAnalyticsStudio.WebView2;

public sealed class UpdateClient
{
    private const string LatestReleaseUrl = "https://api.github.com/repos/G061206/IBKRAnalyticsStudio/releases/latest";
    private const string ReleasesPageUrl = "https://github.com/G061206/IBKRAnalyticsStudio/releases";
    private readonly HttpClient httpClient;

    public UpdateClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<UpdateCheckResult> CheckLatestAsync(string currentVersion, CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, LatestReleaseUrl);
        request.Headers.Accept.ParseAdd("application/vnd.github+json");

        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        string body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return CreateNoReleaseResult(currentVersion);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new UpdateCheckException($"GitHub update check failed with HTTP {(int)response.StatusCode} {response.StatusCode}.");
        }

        GitHubRelease release = JsonSerializer.Deserialize<GitHubRelease>(
            body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new UpdateCheckException("GitHub returned an empty release response.");

        string latestVersion = NormalizeVersion(release.TagName) ?? NormalizeVersion(release.Name) ?? "";
        if (string.IsNullOrWhiteSpace(latestVersion))
        {
            throw new UpdateCheckException("The latest release does not contain a recognizable version.");
        }

        GitHubAsset? portableAsset = release.Assets?
            .Where((asset) => !string.IsNullOrWhiteSpace(asset.BrowserDownloadUrl))
            .OrderByDescending((asset) => IsPreferredPortableAsset(asset.Name))
            .ThenBy((asset) => asset.Name)
            .FirstOrDefault();

        bool updateAvailable = IsNewerVersion(latestVersion, currentVersion);
        return new UpdateCheckResult(
            CurrentVersion: currentVersion,
            LatestVersion: latestVersion,
            UpdateAvailable: updateAvailable,
            ReleaseUrl: release.HtmlUrl ?? "",
            DownloadUrl: portableAsset?.BrowserDownloadUrl ?? release.HtmlUrl ?? "",
            AssetName: portableAsset?.Name ?? "",
            ReleaseName: release.Name ?? release.TagName ?? latestVersion,
            ReleaseAvailable: true,
            PublishedAt: release.PublishedAt);
    }

    private static UpdateCheckResult CreateNoReleaseResult(string currentVersion)
    {
        return new UpdateCheckResult(
            CurrentVersion: currentVersion,
            LatestVersion: currentVersion,
            UpdateAvailable: false,
            ReleaseUrl: ReleasesPageUrl,
            DownloadUrl: ReleasesPageUrl,
            AssetName: "",
            ReleaseName: "",
            ReleaseAvailable: false,
            PublishedAt: null);
    }

    private static bool IsPreferredPortableAsset(string? name)
    {
        string value = name ?? "";
        return value.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) &&
            value.Contains("portable", StringComparison.OrdinalIgnoreCase) &&
            value.Contains("win-x64", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsNewerVersion(string latestVersion, string currentVersion)
    {
        Version latest = ParseVersion(latestVersion);
        Version current = ParseVersion(currentVersion);
        return latest.CompareTo(current) > 0;
    }

    private static Version ParseVersion(string value)
    {
        string normalized = NormalizeVersion(value) ?? "0.0.0";
        return Version.TryParse(normalized, out Version? version) ? version : new Version(0, 0, 0);
    }

    private static string? NormalizeVersion(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        string trimmed = value.Trim();
        if (trimmed.StartsWith("v", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed[1..];
        }

        int dashIndex = trimmed.IndexOf('-');
        if (dashIndex > 0)
        {
            trimmed = trimmed[..dashIndex];
        }

        return trimmed;
    }
}

public sealed record UpdateCheckResult(
    string CurrentVersion,
    string LatestVersion,
    bool UpdateAvailable,
    string ReleaseUrl,
    string DownloadUrl,
    string AssetName,
    string ReleaseName,
    bool ReleaseAvailable,
    DateTimeOffset? PublishedAt);

public sealed class UpdateCheckException : Exception
{
    public UpdateCheckException(string message) : base(message)
    {
    }
}

internal sealed record GitHubRelease(
    [property: JsonPropertyName("tag_name")]
    string? TagName,
    string? Name,
    [property: JsonPropertyName("html_url")]
    string? HtmlUrl,
    [property: JsonPropertyName("published_at")]
    DateTimeOffset? PublishedAt,
    List<GitHubAsset>? Assets);

internal sealed record GitHubAsset(
    string? Name,
    [property: JsonPropertyName("browser_download_url")]
    string? BrowserDownloadUrl);
