//namespace PainKiller.PowerCommands.Core.Services;

//public class ButtonBarService
//{
//    public static TEnum DrawToolbar<TEnum>(Emo[]? consoleColors = null, int padLeft = 1, string title = "Use tab to select, then hit [RETURN]") where TEnum : Enum
//    {
//        ConsoleService.Service.WriteHeaderLine(nameof(DrawToolbar), title.PadLeft(title.Length + padLeft));
//        var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
//        var labels = enumValues
//            .OrderBy(e => Convert.ToInt32(e))
//            .Select(e => e.ToString())
//            .ToArray();
//        var selectedLabel = DrawToolbar(labels, consoleColors, padLeft: padLeft);
//        ConsoleService.Service.ClearRow(Console.CursorTop);
//        ConsoleService.Service.WriteSuccessLine(nameof(DrawToolbar), selectedLabel.PadLeft(selectedLabel.Length + padLeft));
//        return (TEnum)Enum.Parse(typeof(TEnum), selectedLabel);
//    }
//    public static string DrawToolbar(string[] labels, Emo[]? consoleColors = null, int padLeft = 1)
//    {
//        try
//        {
//            if (labels.Length == 0) return "";
//            var colors = consoleColors ??
//            [
//                Emo.CircleBlue, Emo.CircleRed, Emo.CircleYellow, Emo.CircleOrange,
//                Emo.CirclePurple, Emo.CircleBrown, Emo.CircleWhite
//            ];

//            var currentIndex = labels.Length - 1;
//            ConsoleKey key;
//            var consoleForeground = Console.ForegroundColor;
//            var consoleBackground = Console.BackgroundColor;
//            do
//            {
//                var index = 0;
//                foreach (var label in labels)
//                {
//                    if (currentIndex == index)
//                    {
//                        Console.ForegroundColor = ConsoleColor.Green;

//                    }
//                }
//                DrawToolbar(labels, colors, showOnBottom: false, currentIndex, padLeft: padLeft);
//                Console.Title = labels[currentIndex];
//                key = Console.ReadKey(true).Key;

//                if (key == ConsoleKey.Tab)
//                {
//                    currentIndex = (currentIndex - 1) % labels.Length;
//                    if (currentIndex < 0) currentIndex = labels.Length - 1;
//                }
//                else if (key == ConsoleKey.Enter)
//                {
//                    return labels[currentIndex];
//                }

//            } while (key != ConsoleKey.Escape);

//            return "";
//        }
//        catch (Exception e)
//        {
//            Console.WriteLine($"Error: {e.Message}");
//            return "";
//        }
//    }
//}