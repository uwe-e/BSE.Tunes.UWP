using BSE.Tunes.StoreApp.ViewModels;
using Windows.UI.Xaml.Controls;
using WinUI = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BSE.Tunes.StoreApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        //public static ShellPage Instance { get; set; }

        //public static WinUI.NavigationView NavigationView => Instance.navigationView;

        private ShellViewModel ViewModel
        {
            get { return ViewModelLocator.Current.ShellViewModel; }
        }

        public ShellPage()
        {
            
            this.InitializeComponent();
            //Instance = this;
            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);
        }


    }
}
