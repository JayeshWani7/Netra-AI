using System.Windows;

namespace NetraAI.Desktop.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => System.Diagnostics.Debug.WriteLine("LoginWindow loaded successfully");
        }
    }
}
