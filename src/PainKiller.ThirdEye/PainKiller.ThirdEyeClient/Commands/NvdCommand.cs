using PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Extensions;
using PainKiller.ThirdEyeClient.Managers;

namespace PainKiller.ThirdEyeClient.Commands;
[CommandDesign(description: "Manage your CVEs from National Vulnerability Database (NVD), request an API key from https://nvd.nist.gov/developers/request-an-api-key.",
    arguments: [],
    options: ["!api-key", "fetch", "push-update-file"],
    examples: ["//Update your cve:s from the last page you collected.", "nvd"])]
public class NvdCommand(string identifier) : ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        if (input.HasOption("api-key"))
        {
            SetupApiKey(input.GetOptionValue("api-key"));
            return Ok();
        }
        if (input.HasOption("fetch"))
        {
            var apiKey = Configuration.Core.Modules.Security.DecryptSecret(Configuration.ThirdEye.Nvd.TokenName);
            var nvdFetcher = new CveFetcherManager(CveStorage, Configuration.ThirdEye.Nvd, apiKey, Writer);
            var cve = nvdFetcher.FetchAllCvesAsync().Result;
            Writer.WriteSuccessLine($"{cve.Count} updated in database");
            Writer.WriteLine();
            var checkSum = CveStorage.CreateUpdateFile();
            Writer.WriteSuccessLine($"A new update file created with Checksum: {checkSum}");
            DisplayLatest();
            return Ok();
        }
        if (input.HasOption("push-update-file"))
        {
            var checkSum = CveStorage.CreateUpdateFile();
            Writer.WriteSuccessLine($"A new update file created with Checksum: {checkSum}");
            return Ok();
        }
        CveStorage.Update(Writer);
        return Ok();
    }

    public void DisplayLatest()
    {
        CveStorage.ReLoad();
        var latest = CveStorage.GetCveEntries()
            .Where(cv => cv.CvssScore.IsEqualOrHigher(CvssSeverity.Critical))
            .OrderByDescending(cv => cv.FetchedIndex)
            .Take(Configuration.ThirdEye.Nvd.LatestCount)
            .ToList();
        PresentationManager.DisplayCveEntries(latest);
    }

    private void SetupApiKey(string apiKey)
    {
        Configuration.Core.Modules.Security.EncryptSecret(EnvironmentVariableTarget.User, Configuration.ThirdEye.Nvd.TokenName, apiKey);
        Writer.WriteSuccessLine($"\nApiKey has been created, you can now fetch CVEs from NVD.");
    }
}


    
