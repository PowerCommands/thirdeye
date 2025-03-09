using System.Text.Json;
using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class NvdDataFetcherManager(ICveStorage storage, IConsoleWriter writer)
{
    private readonly HttpClient _client = new();
    private const string BaseUrl = "https://services.nvd.nist.gov/rest/json/cves/2.0";

    public async Task<List<CveEntry>> FetchAllCves()
    {
        storage.ReLoad();
        writer.WriteSuccessLine($"NVD storage last updated: {storage.LastUpdated.ToShortDateString()} Last indexed page was: {storage.LastIndexedPage}");
        writer.WriteSuccessLine($"{storage.LoadedCveCount} CVE:s in your local storage.");
        const int resultsPerPage = ICveStorage.PAGE_SIZE;
        var startIndex = storage.LastIndexedPage * ICveStorage.PAGE_SIZE;
        var hasMoreResults = true;
        var newCves = new List<CveEntry>();

        while (hasMoreResults)
        {
            var url = $"{BaseUrl}?startIndex={startIndex}&resultsPerPage={resultsPerPage}";
            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Rootobject>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.vulnerabilities != null && result.vulnerabilities.Any())
                {
                    writer.WriteLine($"✅ Fetched {result.vulnerabilities.Length} CVEs, start index: {startIndex}");
                    newCves.AddRange(ProcessCveEntries(result.vulnerabilities, startIndex));
                    startIndex += resultsPerPage;
                }
                else
                {
                    hasMoreResults = false;
                }
            }
            catch (Exception ex)
            {
                writer.WriteFailureLine($"❌ Error fetching CVEs: {ex.Message}");
                break;
            }

            await Task.Delay(1000); // Skydda API:et från rate limiting
        }

        writer.WriteSuccessLine($"🎯 Total CVEs fetched: {newCves.Count}");
        return newCves;
    }
    private List<CveEntry> ProcessCveEntries(Vulnerability[] vulnerabilities, int index)
    {
        var result = new List<CveEntry>();
        foreach (var v in vulnerabilities)
        {
            if (v?.cve == null)
            {
                writer.WriteFailureLine("⚠️ Skipped a CVE entry due to missing data.");
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
                                .Select(m => m.criteria)) ?? Enumerable.Empty<string>())
                        .ToList() ?? new List<string>()
                };

                result.Add(entry);
            }
            catch (Exception ex)
            {
                writer.WriteFailureLine($"❌ Error processing CVE {v.cve.id}: {ex.Message}");
            }
        }
        storage.AppendEntries(result, index);
        return result;
    }

}

