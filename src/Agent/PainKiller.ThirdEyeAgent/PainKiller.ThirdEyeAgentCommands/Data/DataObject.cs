namespace PainKiller.ThirdEyeAgentCommands.Data;

public abstract class DataObject<T> where T : class, new()
{
    public DateTime LastUpdated { get; set; }
    public List<T> Items { get; set; } = [];
    public List<T> GetAll() => Items;
}