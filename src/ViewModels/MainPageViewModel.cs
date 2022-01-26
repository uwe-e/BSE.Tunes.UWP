namespace BSE.Tunes.StoreApp.ViewModels
{
    public class MainPageViewModel : SearchSuggestionsViewModel
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                //Value = "Designtime value";
            }
        }
    }
}

