using System;
using System.IO;

namespace MulTyPlayerClient.Classes.ConsoleLog;

internal static class DevLog
{
    private const bool DEVLOG_ENABLED = true;
    private static readonly string _initTime = DateTime.Now.ToString("MM-dd-yyyy h-mm-ss");
    private static readonly string _path;
    private static readonly StreamWriter _writer;

    static DevLog()
    {
#if DEBUG
        Directory.CreateDirectory("./Logs_dev/");
        if (DEVLOG_ENABLED)
        {
            _path = $"./Logs_dev/devlog {_initTime}.txt";
            _writer = File.AppendText(_path);
            _writer.WriteLine($"-----DevLog_{_initTime}-----");
        }
#endif
    }

    public static void WriteLine(string message, int callerLineNumber, string callerMemberName, string callerFilePath)
    {
#if DEBUG
        var timestamp = DateTime.Now.ToString("h:mm:ss");
        var log = $"[{timestamp}]\t{callerMemberName} in {callerFilePath} at line {callerLineNumber}:\n\t{message}";
        _writer.WriteLine(log);
#endif
    }

    public static void Close()
    {
        _writer.Flush();
        _writer?.Close();
    }
}