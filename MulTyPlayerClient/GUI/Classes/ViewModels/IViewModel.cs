using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient.GUI.ViewModels
{
    public interface IViewModel
    {
        public void OnEntered();
        public void OnExited();
    }
}
