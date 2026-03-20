namespace TodoHub.Views
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(Preferences.Default.Get("github_repo", "")))
            {
                MainTitleLabel.Text = Preferences.Default.Get("github_repo", "");
                TodoLoad();
            }
        }

        private void TodoLoad()
        {
            MainArea.Text = "Hello, World!";
        }
    }
}
