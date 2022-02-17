using Xamarin.Forms;

namespace QControlKitXamDemo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
            Current.RequestedThemeChanged += Current_RequestedThemeChanged;
            SetAppResources();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            SetAppResources();
        }

        private void SetAppResources()
        {

            switch (Current.RequestedTheme)
            {
                case OSAppTheme.Light:
                    SetLightResources();
                    break;
                case OSAppTheme.Dark:
                    SetDarkResources();
                    break;
                case OSAppTheme.Unspecified:
                    //Default to Light
                    SetLightResources();
                    break;
                default:
                    SetLightResources();
                    break;
            }
        }

        private void SetDarkResources()
        {

            Resources["NavigationBarColor"] = Color.FromHex("#3f6dab");
            Resources["PageBackgroundColor"] = Color.FromHex("#525252");


            Resources["PrimaryColor"] = Color.WhiteSmoke;
            Resources["SecondaryColor"] = Color.White;
            Resources["PrimaryTextColor"] = Color.LightGray;
            Resources["SecondaryTextColor"] = Color.White;
            Resources["TertiaryTextColor"] = Color.Gray;
            Resources["TransparentColor"] = Color.Transparent;

            Resources["IconTextColor"] = Color.WhiteSmoke;

            Resources["ListViewBackgroundColor"] = Color.FromHex("#363636");

            Resources["ServerCellBackgroundColor"] = Color.FromHex("#363636");
            Resources["WorkspaceCellBackgroundColor"] = Color.FromHex("#737373");

            Resources["SelectedCueCellBackgroundColor"] = Color.FromHex("#737373");

        }

        private void SetLightResources()
        {
            Resources["NavigationBarColor"] = Color.FromHex("#3089ff");
            Resources["PageBackgroundColor"] = Color.LightGray;


            Resources["PrimaryColor"] = Color.WhiteSmoke;
            Resources["SecondaryColor"] = Color.White;
            Resources["PrimaryTextColor"] = Color.Black;
            Resources["SecondaryTextColor"] = Color.White;
            Resources["TertiaryTextColor"] = Color.Gray;
            Resources["TransparentColor"] = Color.Transparent;

            Resources["IconTextColor"] = Color.Black;

            Resources["ListViewBackgroundColor"] = Color.White;

            Resources["ServerCellBackgroundColor"] = Color.White;
            Resources["WorkspaceCellBackgroundColor"] = Color.FromHex("#D8D8D8");

            Resources["SelectedCueCellBackgroundColor"] = Color.FromHex("#D8D8D8");

        }
    }
}
