namespace PainKiller.ThirdEyeAgentCommands.Contracts;

public interface IDataObjects<T> where T : class, new()
{
    DateTime LastUpdated { get; set; }
    List<T> Items { get; set; }
}