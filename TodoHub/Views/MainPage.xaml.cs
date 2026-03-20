namespace TodoHub.Views
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            TodoLoad();
        }

        private void TodoLoad()
        {
            MainArea.Text = "Hello, World!";
        }
    }
}
