﻿using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MulTyPlayerClient;

public class LocalCommandServer
{
    private const string Name = "MulTyCommandPipe";
    private StreamReader _reader;
    private NamedPipeServerStream _server;

    public void StartCommandPipe()
    {
        var pipeThread = new Thread(async () => await StartListening());
        pipeThread.IsBackground = true;
        pipeThread.Start();
    }
    
    public void StopCommandPipe()
    {
        _server?.Disconnect();
        _server?.Dispose();
        _reader?.Dispose();
    }

    private async Task StartListening()
    {
        _server = new NamedPipeServerStream(Name, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        Logger.WriteDebug("Opened Command Pipe");
        await _server.WaitForConnectionAsync();
        Logger.WriteDebug("Connected to Command Pipe Client");
        _reader = new StreamReader(_server, Encoding.UTF8);

        await Listen();
    }

    private async Task Listen()
    {
        while (_server.IsConnected)
        {
            //Logger.WriteDebug("Waiting for command.");
            var command = await _reader.ReadLineAsync();
            if (command is null or "PING")
                continue;
            //Logger.WriteDebug("Received Command");
            Client.HCommand.ParseCommand(command);
            await Task.Delay(100);
        }
        Logger.WriteDebug("Disconnected Command Pipe");
    }
}