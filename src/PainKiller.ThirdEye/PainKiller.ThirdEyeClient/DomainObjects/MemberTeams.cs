namespace PainKiller.ThirdEyeClient.DomainObjects;
public class MemberTeams
{
    public Member Member { get; set; } = new Member();
    public List<string> Teams { get; set; } = [];
}