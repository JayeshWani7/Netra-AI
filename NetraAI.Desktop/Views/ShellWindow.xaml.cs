using System.Windows;
using System.Windows.Controls;

namespace NetraAI.Desktop.Views
{
    /// <summary>
    /// Main application shell that hosts all screens in a single window.
    /// </summary>
    public partial class ShellWindow : Window
    {
        public ShellWindow()
        {
            InitializeComponent();
        }

        public ContentControl GetContentHost()
        {
            return ShellContentHost;
        }
    }
}
