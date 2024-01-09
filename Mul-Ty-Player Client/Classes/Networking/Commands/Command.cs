using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerClient;

public abstract class Command
{
    public string Name;
    public List<string> Aliases;
    public List<string> Usages;
    public Dictionary<string, string> ArgDescriptions;
    public string Description;
    public bool HostOnly;

    public abstract void InitExecute(string[] args);

    public virtual void PrintHelp()
    {
        Logger.Write($"\nCommand: {Name}\n" +
                     $"Description:\n {Description}\n" +
                     $"Aliases:\n {string.Join(", ", Aliases)}\n" +
                     $"Usages:\n {string.Join(",\n ", Usages)}\n" +
                     $"Arguments:\n {string.Join(",\n ", ArgDescriptions.Select(arg => $"{arg.Key}: {arg.Value}"))}\n" +
                     $"HostOnly?: {HostOnly}\n");
    }

    protected virtual void SuggestHelp()
    {
        Logger.Write($"[ERROR] Incorrect usage. Use /help {Name} for more info.");
    }

    protected virtual void LogError(string message)
    {
        Logger.Write($"[ERROR] {message}");
    }
}