namespace BingDailyPicture
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainWindowViewModel viewModel) : this()
        {
            ViewModel = viewModel;
        }

        public MainWindowViewModel ViewModel { get { return DataContext as MainWindowViewModel; } set { DataContext = value; } }
    }
}
