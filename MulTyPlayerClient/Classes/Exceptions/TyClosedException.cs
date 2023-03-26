using System;

namespace MulTyPlayerClient
{
    public class TyClosedException : Exception
    {
        public override string Message => "Ty the Tasmanian Tiger has closed or stopped responding. Waiting for Ty to reopen.";
    }
}
