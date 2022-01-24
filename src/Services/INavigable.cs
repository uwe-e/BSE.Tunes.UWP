using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.Services
{
    internal interface INavigable
    {
        Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state);

        Task OnNavigatedFromAsync(IDictionary<string, object> suspensionstate, bool suspending);
    }
}