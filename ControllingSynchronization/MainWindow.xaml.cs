using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ControllingSynchronization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int delay = 1000;
        private readonly MainWindowViewModel viewModel;


        public MainWindow()
        {
            DataContext = this.viewModel = new MainWindowViewModel();

            InitializeComponent();
        }


        private void SyncButton_Click(object sender, RoutedEventArgs e)
        {
            SyncButtonText.Text = "Running";
            Thread.Sleep(delay);
            SyncButtonText.Text = "Completed: 42";
        }


        private async void AsyncButton_Click(object sender, RoutedEventArgs e)
        {
            AsyncButtonText.Text = "Running";
            var result = await viewModel.DoWork();
            AsyncButtonText.Text = $"Completed: {result}";
        }


        private void DeadlockButton_Click(object sender, RoutedEventArgs e)
        {
            DeadlockButtonText.Text = "Running";
            var result = viewModel.DoWork().Result;
            DeadlockButtonText.Text = $"Completed: {result}";
        }


        private async void ConfigureAwaitButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigureAwaitButtonText.Text = "Running";
            var result = await viewModel.DoWork_ConfigureAwait();
            ConfigureAwaitButtonText.Text = $"Completed: {result}";
        }


        private async void WithDetatchButton_Click(object sender, RoutedEventArgs e)
        {
            WithDetatchButtonText.Text = "Running";
            var result = await viewModel.DoWork_WithDetatch();
            WithDetatchButtonText.Text = $"Completed: {result}";
        }


        private async void TaskSchedulerButton_Click(object sender, RoutedEventArgs e)
        {
            TaskSchedulerButtonText.Text = "Running";
            var result = await viewModel.DoWork_WithTaskScheduler();
            TaskSchedulerButtonText.Text = $"Completed: {result}";
        }


        private async void ExplicitSwitchingButton_Click(object sender, RoutedEventArgs e)
        {
            // Dispatcher
            ExplicitSwitchingButtonText.Text = "Switching to do work in a task";

            await Awaiters.TaskSchedulerContext();

            // Task
            await Task.Delay(delay);

            await Awaiters.DispatcherContext(Dispatcher);

            // Dispatcher
            ExplicitSwitchingButtonText.Text = "On the dispatcher for a moment";
            
            await Awaiters.TaskSchedulerContext();

            // Task
            await Task.Delay(delay);
            Thread.Sleep(delay * 2);

            await Awaiters.DispatcherContext(Dispatcher);

            // Dispatcher
            ExplicitSwitchingButtonText.Text = "End back on the dispatcher";
        }
    }
}
