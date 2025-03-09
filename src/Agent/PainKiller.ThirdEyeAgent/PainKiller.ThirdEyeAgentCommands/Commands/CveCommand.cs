namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Check for vulnerabilities in your components",
                  disableProxyOutput: true,
                             example: "//Check for vulnerabilities in your components|cve")]
    public class CveCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            CveStorage.ReLoad();
            IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();

            var components = Storage.GetThirdPartyComponents();
            foreach (var component in components)
            {
                var entries = CveStorage.GetCveEntries().Where(cv => (cv.AffectedProducts.Contains(component.Name) || cv.Description.Contains(component.Name)) && cv.AffectedProducts.Any(p => p.Contains(component.Version))).ToList();
                if (entries.Count > 0)
                {
                    WriteHeadLine($"{component.Name}");
                    foreach (var cveEntry in entries) WriteCodeExample($"{cveEntry.Id} {cveEntry.Description}", string.Join(",", cveEntry.AffectedProducts));
                }
            }
            return Ok();
        }
    }
}