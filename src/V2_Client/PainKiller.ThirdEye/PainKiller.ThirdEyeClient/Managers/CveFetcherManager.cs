using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using PainKiller.ThirdEyeClient.Bootstrap.Configuration;
using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects.Nvd;
using PainKiller.ThirdEyeClient.Services;

namespace PainKiller.ThirdEyeClient.Managers;

public class CveFetcherManager(ICveStorageService storage, NvdConfiguration configuration, string apiKey, IConsoleWriter writer)
{
    private readonly HttpClient _client = new(){Timeout = TimeSpan.FromSeconds(configuration.TimeoutSeconds)};
    private readonly string _baseUrl = configuration.Url;
    private readonly ILogger<CveFetcherManager> _logger = LoggerProvider.CreateLogger<CveFetcherManager>();

    public async Task<List<CveEntry>> FetchAllCvesAsync()
    {
        storage.ReLoad();
        writer.WriteSuccessLine($"NVD storage last updated: {storage.LastUpdated.ToShortDateString()}");
        writer.WriteSuccessLine($"{storage.LoadedCveCount} CVE:s in your local storage.");
        var resultsPerPage = configuration.PageSize;
        var startIndex = storage.LoadedCveCount;
        var hasMoreResults = true;
        var newCves = new List<CveEntry>();

        while (hasMoreResults)
        {
            var url = $"{_baseUrl}?startIndex={startIndex}&resultsPerPage={resultsPerPage}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("apiKey", apiKey);
            try
            {
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    writer.WriteError("API returned 403 Forbidden. Waiting 1 minute before retry...");
                    await Task.Delay(60000); // Wait one minute before retrying
                    continue;
                }
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Rootobject>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.vulnerabilities != null && result.vulnerabilities.Any())
                {
                    writer.WriteSuccessLine($"Fetched {result.vulnerabilities.Length} CVEs, start index: {startIndex}");
                    newCves.AddRange(ProcessCveEntries(result.vulnerabilities));
                    startIndex += resultsPerPage;
                }
                else
                {
                    hasMoreResults = false;
                }
            }
            catch (Exception ex)
            {
                writer.WriteError($"Error fetching CVEs: {ex.Message}");
                break;
            }
            await Task.Delay(configuration.DelayIntervalSeconds); // Protects against rate limiting
        }
        writer.WriteSuccessLine($"Total CVEs fetched: {newCves.Count}");
        return newCves;
    }
    private List<CveEntry> ProcessCveEntries(Vulnerability[] vulnerabilities)
    {
        var result = new List<CveEntry>();
        foreach (var v in vulnerabilities)
        {
            if (v?.cve == null)
            {
                writer.WriteError("Skipped a CVE entry due to missing data.");
                continue;
            }

            try
            {
                var entry = new CveEntry
                {
                    Id = v.cve.id ?? "UNKNOWN",
                    Description = v.cve.descriptions?.FirstOrDefault()?.value ?? "No description",
                    CvssScore = v.cve.metrics?.cvssMetricV2?.FirstOrDefault()?.cvssData?.baseScore ?? 0.0f,
                    Severity = v.cve.metrics?.cvssMetricV2?.FirstOrDefault()?.baseSeverity ?? "UNKNOWN",
                    AffectedProducts = v.cve.configurations?
                        .SelectMany(c => c.nodes?
                            .SelectMany(n => n.cpeMatch?
                                .Where(m => m.vulnerable)
                                .Select(m => m.criteria) ?? Array.Empty<string>()) ?? [])
                        .ToList() ?? []
                };

                result.Add(entry);
            }
            catch (Exception ex)
            {
                writer.WriteError($"Error processing CVE {v.cve.id}: {ex.Message}");
            }
        }
        storage.AppendEntries(result, writer);
        return result;
    }
    public async Task<CveDetailResponse?> FetchCveDetailsAsync(string cveId)
    {
        if (string.IsNullOrWhiteSpace(cveId))
            throw new ArgumentException("CVE ID cannot be null or empty", nameof(cveId));
        var cache = CveCacheObjectsService.Service.Get(cveId);
        if (cache != null)
        {
            _logger.LogError($"CVE {cveId} fetched from local cache.");
            return cache;
        }
        var url = $"{_baseUrl}?cveId={cveId}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("apiKey", apiKey);
        HttpResponseMessage response = await _client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to fetch CVE details. Status Code: {response.StatusCode}");

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var retVal = JsonSerializer.Deserialize<CveDetailResponse>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if(retVal != null) CveCacheObjectsService.Service.Store(retVal);
        return retVal;
    }
}