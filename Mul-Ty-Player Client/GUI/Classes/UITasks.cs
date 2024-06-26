﻿using System;
using System.Threading.Tasks;
using System.Windows;

namespace MulTyPlayerClient.GUI;

public static class UITasks
{
    //Will startup on the UI thread and save it here
    public static TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

    private static string _message;

    //Action to write to the logger 
    public static Action DoOnUIThread = () => { Logger.Write(_message); };

    public static void LoggerWrite(string message)
    {
        Application.Current.Dispatcher.Invoke(() => { _message = message; });
        //Start voidfactory with dummy action, to create a voidto run on another thread
        //(Can't find a way to make it just start on the UI thread)
        var t1 = Task.Factory.StartNew(() => { });
        // when t1 is done run t1..on the Ui thread.
        t1.ContinueWith(t => DoOnUIThread(), _uiScheduler);
    }
}