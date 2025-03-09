using System.Text.Json;
using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class NvdDataFetcherManager
{
    private readonly HttpClient _client = new();
    private const string BaseUrl = "https://services.nvd.nist.gov/rest/json/cves/2.0";

    public async Task<List<CveEntry>> FetchAllCves()
    {
        int resultsPerPage = 2000; // Standard API-limit
        int startIndex = 0;
        bool hasMoreResults = true;
        var allCves = new List<CveEntry>();

        while (hasMoreResults)
        {
            string url = $"{BaseUrl}?startIndex={startIndex}&resultsPerPage={resultsPerPage}";

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
                    Console.WriteLine($"✅ Fetched {result.vulnerabilities.Length} CVEs, start index: {startIndex}");
                    allCves.AddRange(ProcessCveEntries(result.vulnerabilities));
                    startIndex += resultsPerPage;
                }
                else
                {
                    hasMoreResults = false; // Inga fler resultat
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching CVEs: {ex.Message}");
                break;
            }

            await Task.Delay(1000); // Skydda API:et från rate limiting
        }

        Console.WriteLine($"🎯 Total CVEs fetched: {allCves.Count}");
        return allCves;
    }
    private List<CveEntry> ProcessCveEntries(Vulnerability[] vulnerabilities)
    {
        var result = new List<CveEntry>();
        foreach (var v in vulnerabilities)
        {
            if (v?.cve == null)
            {
                Console.WriteLine("⚠️ Skipped a CVE entry due to missing data.");
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
                Console.WriteLine($"❌ Error processing CVE {v.cve.id}: {ex.Message}");
            }
        }
        return result;
    }

}

