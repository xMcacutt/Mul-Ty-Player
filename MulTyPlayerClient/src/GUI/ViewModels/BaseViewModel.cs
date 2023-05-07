using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient.GUI.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ViewModelEntered;
        public event Action<View> OnMoveToView;

        public static BaseViewModel Instance
        {
            get; protected set;
        }

        public BaseViewModel() => Instance = this;

        protected void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnViewModelEntered()
        {
            ViewModelEntered?.Invoke(this, new EventArgs());
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new ArgumentNullException(GetType().Name + " does not contain property: " + propertyName);
        }

        protected void MoveToView(View view)
        {
            OnMoveToView?.Invoke(view);
        }

        public enum View
        {
            Splash,
            Login,
            KoalaSelect,
            Lobby,
            Settings,
            Setup
        }
    }
}
