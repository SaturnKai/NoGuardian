using System;
using System.Windows;
using System.Windows.Input;

using System.Threading;

using Fiddler;

namespace NoGuardian
{
    public partial class MainWindow : Window
    {
        private int port = 8888;
        private int total, allowed, blocked;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Loader.Visibility = Visibility.Visible;
            FiddlerApplication.Shutdown();
            Application.Current.Shutdown();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Thread startThread = new Thread(Start);
            startThread.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Thread stopThread = new Thread(Stop);
            stopThread.Start();
        }

        private void Start()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                StartButton.IsEnabled = false;
                Loader.Visibility = Visibility.Visible;
            }));

            FiddlerApplication.BeforeRequest += OnBeforeRequest;
            FiddlerApplication.Startup(port, true, false, true);

            Dispatcher.Invoke(new Action(() =>
            {
                StopButton.IsEnabled = true;
                StatusLabel.Content = "NoGuardian Started.";
                Loader.Visibility = Visibility.Hidden;
            }));
        }

        private void Stop()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                StopButton.IsEnabled = false;
                Loader.Visibility = Visibility.Visible;
            }));

            FiddlerApplication.Shutdown();

            Dispatcher.Invoke(new Action(() =>
            {
                StartButton.IsEnabled = true;
                StatusLabel.Content = "NoGuardian Stopped.";
                Loader.Visibility = Visibility.Hidden;
            }));
        }

        private void OnBeforeRequest(Session oSession)
        {
            total++;

            if (oSession.uriContains("goguardian.com"))
            {
                blocked++;
                UpdateLabels();
                Thread.Sleep(999999999);
            }
            else allowed++;

            UpdateLabels();
        }

        private void UpdateLabels()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (total > 0) TotalLabel.Content = total;
                if (allowed > 0) AllowedLabel.Content = allowed;
                if (blocked > 0) BlockedLabel.Content = blocked;
            }));
        }
    }
}