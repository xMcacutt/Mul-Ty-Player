using System.Windows;
using System.Windows.Input;

namespace Mul_Ty_Player_Updater.Views;

public partial class NewMessageBox : Window
{
    
    public NewMessageBox(string message, string iconCode)
    {
        InitializeComponent();
        Message.Text = message;
        MessageIcon.Code = iconCode;
        MouseDown += (sender, e) =>
        {
            // Check if the left mouse button is pressed
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        };
    }

    private void Okay_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}