using System;

namespace MulTyPlayerClient
{
    public class TyClosedException : Exception
    {
        public TyClosedException() : base("Ty the Tasmanian Tiger has closed or stopped responding. Waiting for Ty to reopen.") { }
    }

    public class TyProcessException : Exception
    {
        public TyProcessException() : base ("Mul-Ty-Player.exe process has exited, been lost, or privileges have changed.") { }
        public TyProcessException(string source) : base("Mul-Ty-Player.exe process has exited, been lost, or privileges have changed.") => Source = source;
        public TyProcessException(Exception innerException) : base("Mul-Ty-Player.exe process has exited, been lost, or privileges have changed.", innerException) { }
        public TyProcessException(string source, Exception innerException) : base("Mul-Ty-Player.exe process has exited, been lost, or privileges have changed.", innerException) => Source = source;
    }
}
