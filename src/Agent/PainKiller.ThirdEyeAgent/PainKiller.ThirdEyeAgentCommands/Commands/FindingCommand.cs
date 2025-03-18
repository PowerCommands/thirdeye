using PainKiller.PowerCommands.Shared.Extensions;
using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Enums;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Handle findings",
                  disableProxyOutput: true,
                             example: "//List all findings|finding")]
    public class FindingCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            DisableLog();
            var findings = Storage.GetFindings().OrderByDescending(f => f.Updated).ToList();
            var showAllFindings = ListService.ShowSelectFromFilteredList("Filter findings", findings, (f, s) => f.Cve.Id.ToLower().Contains(s.ToLower()) || f.Description.ToLower().Contains(s), PresentationManager.DisplayFindings, this);
            
            var selectedFindings = ListService.ListDialog("Choose finding", showAllFindings.OrderByDescending(fi => fi.Cve.CvssScore).ThenByDescending(fi => fi.Updated).Select(f => $"{f.Cve.Id} {f.Status} {f.Description}").ToList(), autoSelectIfOnlyOneItem: false);
            if (selectedFindings.Count == 0) return Ok();
            var selectedFinding = findings[selectedFindings.First().Key];

            var action = ToolbarService.NavigateToolbar<HandleFindingAction>(title: $"Options for {selectedFinding.Cve.Id}");
            if(action == HandleFindingAction.Quit) return Ok();
            if (action == HandleFindingAction.Details)
            {
                PresentationManager.DisplayFinding(selectedFinding);
                return Ok();
            }
            if (action == HandleFindingAction.Update)
            {
                var mitigationLog = new MitigationLog();
                var status = ToolbarService.NavigateToolbar<FindingStatus>(title: "Set status of the finding.");
                var name = DialogService.QuestionAnswerDialog("Set a name of your mitigation task:");
                var description = DialogService.QuestionAnswerDialog("Describe your mitigation task");
                var projectSelection = ListService.ListDialog("Affected project", selectedFinding.AffectedProjects.Select(p => p.Name).ToList());
                mitigationLog.ProjectName = projectSelection.Count == 0 ? "All" : selectedFinding.AffectedProjects[projectSelection.First().Key].Name;
                mitigationLog.Name = name;
                mitigationLog.Description = description;
                mitigationLog.Created = DateTime.Now;
                mitigationLog.FindingId = selectedFinding.Id;
                selectedFinding.Updated = DateTime.Now;
                selectedFinding.Status = status;
                selectedFinding.Mitigations.Add(mitigationLog);
                Storage.InsertOrUpdateFinding(selectedFinding);
                WriteSuccessLine($"{selectedFinding.Id} has been updated!");
            }
            if (action == HandleFindingAction.Delete)
            {
                var confirmDeletion = DialogService.YesNoDialog($"Confirm deletion of finding {selectedFinding.Cve.Id} {selectedFinding.Description.Truncate(100)}");
                if (confirmDeletion)
                {
                    Storage.RemoveFinding(selectedFinding.Id);
                    WriteSuccessLine($"{selectedFinding.Id} has been deleted!");
                }
            }
            EnableLog();
            return Ok();
        }
    }
}