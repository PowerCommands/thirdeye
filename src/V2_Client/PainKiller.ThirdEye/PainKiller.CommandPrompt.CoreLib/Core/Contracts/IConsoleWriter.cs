﻿using Spectre.Console;
namespace PainKiller.CommandPrompt.CoreLib.Core.Contracts;
public interface IConsoleWriter
{
    void WriteDescription(string label, string text, bool writeToLog = true, Color? consoleColor = null, string scope = "");
    void Write(string text, bool writeLog = true, Color? consoleColor = null ,string scope = "");
    void WriteLine(string text = "", bool writeLog = true, Color? consoleColor = null, string scope = "");
    void WriteSuccessLine(string text, bool writeLog = true, string scope = "");
    void WriteWarning(string text, string scope = "");
    void WriteError(string text, string scope = "");
    void WriteCritical(string text, string scope = "");
    void WriteHeadLine(string text, bool writeLog = true, string scope = "");
    void WriteUrl(string text, bool writeLog = true, string scope = "");
    void WriteTable<T>(IEnumerable<T> items, string[]? columnNames = null, Color? consoleColor = null);
    void WritePrompt(string prompt);
    void WriteRowWithColor(int top, ConsoleColor foregroundColor, ConsoleColor backgroundColor, string rowContent);
    void Clear();
    void ClearRow(int top);
}