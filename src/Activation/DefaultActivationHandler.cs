using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.ViewModels;
using System;
using Windows.ApplicationModel.Activation;

namespace BSE.Tunes.StoreApp.Activation
{
    internal class DefaultActivationHandler : ActivationHandler<IActivatedEventArgs>
    {
        private readonly Type _navType;

        public NavigationServiceEx NavigationService => ViewModelLocator.Current.NavigationService;

        protected override bool CanHandleInternal(IActivatedEventArgs args)
        {
            // None of the ActivationHandlers has handled the app activation
            return NavigationService.Frame.Content == null && _navType != null;
        }
    }
}
