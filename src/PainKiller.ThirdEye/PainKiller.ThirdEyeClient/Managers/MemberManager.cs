namespace PainKiller.ThirdEyeClient.Managers;

public static class MemberManager
{
    public static MemberTeams GetMemberAndTeams(string email)
    {
        var retVal = new MemberTeams();
        var storage = ObjectStorageService.Service;
        var teams = storage.GetTeams();
        foreach (var team in teams )
        {
            foreach (var teamMember in team.Members)
            {
                if(string.IsNullOrEmpty(teamMember.Email.Trim())) continue;
                if (teamMember.Email.Trim().ToLower() != email.Trim().ToLower()) continue;
                retVal.Member = teamMember;
                retVal.Teams = GetTeams(email.Trim().ToLower(), teams).Select(t => t.Name).ToList();
            }
        }
        return retVal;
    }
    private static List<Team> GetTeams(string email, List<Team> teams)
    {
        var retVal = new List<Team>();
        foreach (var team in teams) if(team.Members.Any(m => !string.IsNullOrEmpty(m.Email.Trim()) && m.Email.Trim().ToLower() == email)) retVal.Add(team);
        return retVal;
    }
}