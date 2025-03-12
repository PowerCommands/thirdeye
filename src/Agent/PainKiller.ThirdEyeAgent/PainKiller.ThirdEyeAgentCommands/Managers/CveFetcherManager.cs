using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;
using PainKiller.ThirdEyeAgentCommands.Services;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class CveFetcherManager(ICveStorageService storage, ThirdEyeConfiguration configuration, string apiKey, IConsoleWriter writer)
{
    private readonly HttpClient _client = new();
    private readonly string _baseUrl = configuration.Nvd.Url;

    public async Task<List<CveEntry>> FetchAllCvesAsync()
    {
        storage.ReLoad();
        writer.WriteSuccessLine($"NVD storage last updated: {storage.LastUpdated.ToShortDateString()}");
        writer.WriteSuccessLine($"{storage.LoadedCveCount} CVE:s in your local storage.");
        var resultsPerPage = configuration.Nvd.PageSize;
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
                    writer.WriteFailureLine("API returned 403 Forbidden. Waiting 1 minute before retry...");
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
                writer.WriteFailureLine($"Error fetching CVEs: {ex.Message}");
                break;
            }
            await Task.Delay(configuration.Nvd.DelayIntervalSeconds); // Protects against rate limiting
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
                writer.WriteFailureLine("Skipped a CVE entry due to missing data.");
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
                writer.WriteFailureLine($"Error processing CVE {v.cve.id}: {ex.Message}");
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
            PowerCommandServices.Service.Logger.Log(LogLevel.Information, $"CVE {cveId} fetched from local cache.");
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