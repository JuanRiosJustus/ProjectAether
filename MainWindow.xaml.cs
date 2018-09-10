using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ProjectAether.src.structure;
using ProjectAether.src.configs;
using ProjectAether.src.globals;
using System;
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;

namespace ProjectAether
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Controller controller;
        private bool isOpen = false;

        public MainWindow()
        {
            InitializeComponent();
            setupApplicationSettings();
            controller = new Controller(new Model(), setupView());
            MainInputTextField.Text = ConfiguredConstants.TEST_PARAMS;
        }

        private void setupApplicationSettings()
        {
            // Sizing and placement
            MainApplicationWindow.MinWidth = ApplicationConstants.APPLICATION_WIDTH;
            MainApplicationWindow.Width = ApplicationConstants.APPLICATION_WIDTH;
            MainApplicationWindow.MaxWidth = ApplicationConstants.APPLICATION_WIDTH;

            MainApplicationWindow.MinHeight = ApplicationConstants.APPLICATION_HEIGHT;
            MainApplicationWindow.Width = ApplicationConstants.APPLICATION_HEIGHT;
            MainApplicationWindow.MaxHeight = ApplicationConstants.APPLICATION_HEIGHT;

            MainApplicationWindow.Title = ApplicationConstants.APPLICATION_NAME;
            MainApplicationWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Coloring
            Brush backgroundBrush = Brushes.Transparent;
            Brush mainBrush = Brushes.Transparent;
            Brush textBrush = Brushes.Black;
            Brush borderBrush = Brushes.WhiteSmoke;
            double size = 0f;

            // CUSTOM BACKGROUND
            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            Uri ur = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\res\bg.jpg");
            image.Source = new BitmapImage(ur);
            myBrush.ImageSource = image.Source;

            MainApplicationWindow.Background = myBrush;
            MainDisplayTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            MainDisplayTextBox.Background = mainBrush;
            MainDisplayTextBox.IsReadOnly = true;
            MainDisplayTextBox.BorderBrush = borderBrush;
            MainDisplayTextBox.Foreground = textBrush;
            MainDisplayTextBox.BorderThickness = new Thickness(size);
            MainDisplayTextBox.TextWrapping = TextWrapping.NoWrap;

            SecondaryDisplayTexBox.Background = mainBrush;
            SecondaryDisplayTexBox.IsReadOnly = true;
            SecondaryDisplayTexBox.BorderBrush = borderBrush;
            SecondaryDisplayTexBox.Foreground = textBrush;
            SecondaryDisplayTexBox.BorderThickness = new Thickness(size);
            SecondaryDisplayTexBox.TextWrapping = TextWrapping.NoWrap;

            MainInputTextField.Background = mainBrush;
            MainInputTextField.BorderBrush = borderBrush;
            MainInputTextField.Foreground = textBrush;
            MainInputTextField.BorderThickness = new Thickness(size);


            MainButton.Background = mainBrush;
            MainButton.Content = ApplicationConstants.EXPLORE_TEXT;
            MainButton.BorderBrush = borderBrush;
            MainButton.Foreground = textBrush;
            MainButton.BorderThickness = new Thickness(size);

            AnalyzeButton.Background = mainBrush;
            AnalyzeButton.Content = ApplicationConstants.ANALYZE_BUTTON_TEXT;
            AnalyzeButton.BorderBrush = borderBrush;
            AnalyzeButton.Foreground = textBrush;
            AnalyzeButton.BorderThickness = new Thickness(size);

            // this will mark the progress until the serch limit is reached
            ProgressBarDisplay.Foreground = myBrush;
            ProgressBarDisplay.BorderBrush = borderBrush;
            ProgressBarDisplay.Background = backgroundBrush;
        }

        private View setupView()
        {
            View newView = new View();
            newView.setMainDisplayTextBox(MainDisplayTextBox);
            newView.setMainInputTextField(MainInputTextField);
            newView.setSecondaryDisplayTexBox(SecondaryDisplayTexBox);
            newView.setProgressBar(ProgressBarDisplay);
            newView.setImageContainer(ImageContainer);
            newView.setMainButton(MainButton);
            newView.setMainWindow(this);

            return newView;
        }

        private void MainButtonClick(object sender, RoutedEventArgs e)
        {
            if (MainButton.Content.Equals(ApplicationConstants.EXPLORE_TEXT))
            {
                exploreState();
                MainButton.Content = ApplicationConstants.RETURN_TEXT;
            }
            else
            {
                returnState();
                MainButton.Content = ApplicationConstants.EXPLORE_TEXT;
                
            }
        }
        private void exploreState()
        {
            MainInputTextField.IsEnabled = false;
            controller.stopSearch();
            controller.tryDestroyingSharedState();
            controller.search(MainInputTextField.Text);
        }
        private void returnState()
        {
            controller.stopSearch();
            MainInputTextField.IsEnabled = true;
            controller.releaseMessages();
        }

        private void closeApplication(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (isOpen == false) { isOpen = true; return; }
            Environment.Exit(0);
        }

        private void AnalyzeButtonClick(object sender, RoutedEventArgs e)
        {
            MainDisplayTextBox.Clear();
            MainDisplayTextBox.AppendText("\n");
            MainDisplayTextBox.AppendText(Hub.getInstance().analyze());
            MainDisplayTextBox.AppendText("\n");
        }
    }
}
