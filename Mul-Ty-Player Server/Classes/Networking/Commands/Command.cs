using System;
using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public abstract class Command
{
    public string Name;
    public List<string> Aliases;
    public List<string> Usages;
    public Dictionary<string, string> ArgDescriptions;
    public string Description;

    public abstract string InitExecute(string[] args);

    public virtual void PrintHelp()
    {
        Console.WriteLine($"\nCommand: {Name}\n" +
                     $"Description:\n {Description}\n" +
                     $"Aliases:\n {string.Join(", ", Aliases)}\n" +
                     $"Usages:\n {string.Join(",\n ", Usages)}\n" +
                     $"Arguments:\n {string.Join(",\n ", ArgDescriptions.Select(arg => $"{arg.Key}: {arg.Value}"))}\n");
    }

    protected virtual string SuggestHelp()
    {
        return LogError($"Incorrect usage. Use /help {Name} for more info.");
    }

    protected virtual string LogError(string message)
    {
        return $"[ERROR] {message}";
    }
}