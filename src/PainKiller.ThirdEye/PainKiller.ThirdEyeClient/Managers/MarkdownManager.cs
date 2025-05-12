namespace PainKiller.ThirdEyeClient.Managers;
using System.Text;

public class MarkdownManager
{
    private readonly StringBuilder _builder = new();
    public void AddHeader(string text, int level = 1)
    {
        _builder.AppendLine($"{new string('#', level)} {text}");
    }
    public void AddParagraph(string text)
    {
        _builder.AppendLine(text);
        _builder.AppendLine();
    }
    public void AddList(IEnumerable<string> items, bool ordered = false)
    {
        int index = 1;
        foreach (var item in items)
        {
            _builder.AppendLine(ordered ? $"{index++}. {item}" : $"- {item}");
        }
        _builder.AppendLine();
    }
    public void AddCodeBlock(string code, string language = "")
    {
        _builder.AppendLine($"```{language}");
        _builder.AppendLine(code);
        _builder.AppendLine("```");
        _builder.AppendLine();
    }
    public void AddTable(string[] headers, List<string[]> rows)
    {
        _builder.AppendLine(string.Join(" | ", headers));
        _builder.AppendLine(string.Join(" | ", headers.Select(_ => "---")));
        foreach (var row in rows)
        {
            _builder.AppendLine(string.Join(" | ", row));
        }
        _builder.AppendLine();
    }
    public void AddBoldText(string text) => _builder.AppendLine($"**{text}**");
    public void AddItalicText(string text) => _builder.AppendLine($"*{text}*");
    public void SaveToFile(string filePath) => File.WriteAllText(filePath, _builder.ToString());
    public string GetContent() => _builder.ToString();
}
