using BSE.Tunes.StoreApp.Extensions;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace BSE.Tunes.StoreApp.Services
{
    public class SettingsService
    {
		private bool m_isStartUp;
		
		public static SettingsService Instance { get; } = new SettingsService();
		public bool UseLightTheme
        {
            //get
            //{
            //    return m_settingsHelper.Read<bool>(nameof(UseLightTheme), false);
            //}
            //set
            //{
            //    m_settingsHelper.Write(nameof(UseLightTheme), value);
            //}
            get
            {
                return false;
                //return ApplicationData.Current.LocalSettings.ReadAsync<bool>(nameof(UseLightTheme))
                //    .GetAwaiter()
                //    .GetResult();
            }
            set
            {
                ApplicationData.Current.LocalSettings.SaveAsync(nameof(UseLightTheme), value).GetAwaiter();
            }
        }
        public TimeSpan CacheMaxDuration
        {
            //get
            //{
            //    return m_settingsHelper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2));
            //}
            //set
            //{
            //    m_settingsHelper.Write(nameof(CacheMaxDuration), value);
            //    //BootStrapper.Current.CacheMaxDuration = value;
            //}

            get
            {
                var duration = ApplicationData.Current.LocalSettings.ReadAsync<TimeSpan>(nameof(CacheMaxDuration))
                    .GetAwaiter()
                    .GetResult();
                return duration;
            }
            set
            {
                ApplicationData.Current.LocalSettings.SaveAsync(nameof(CacheMaxDuration), value).GetAwaiter();
            }
        }

		/// <summary>
		/// Gets or sets an information that indicates the HamburgerMenu IsOpen state.
		/// </summary>
		public bool IsHamburgerMenuOpen
		{
            //get
            //{
            //	return m_settingsHelper.Read<bool>(nameof(IsHamburgerMenuOpen), true);
            //}
            //set
            //{
            //	if (m_isStartUp)
            //	{
            //		m_settingsHelper.Write(nameof(IsHamburgerMenuOpen), value);
            //	}
            //	m_isStartUp = true;
            //}

            get
            {
                var isMenuOpen = ApplicationData.Current.LocalSettings.ReadAsync<bool>(nameof(IsHamburgerMenuOpen))
                    .GetAwaiter()
                    .GetResult();
                return isMenuOpen;
            }
            set
            {
                if (m_isStartUp)
                {
                    ApplicationData.Current.LocalSettings.SaveAsync(nameof(IsHamburgerMenuOpen), value).GetAwaiter();
                }
                m_isStartUp = true;
            }
        }
        /// <summary>
        /// Gets or sets the url that contains the service
        /// </summary>
        public string ServiceUrl
        {
            get
            {
                return ApplicationData.Current.LocalSettings.Read<string>(nameof(ServiceUrl));
            }
            set
            {
                ApplicationData.Current.LocalSettings.SaveAsync(nameof(ServiceUrl), value).GetAwaiter();
            }

        }
        public User User
        {
            //get
            //{
            //    return m_settingsHelper.Read<User>(nameof(User), null);
            //}
            //set
            //{
            //    m_settingsHelper.Write(nameof(User), value);
            //}
            get
            {
                return ApplicationData.Current.LocalSettings.Read<User>(nameof(User));
            }
            set
            {
                ApplicationData.Current.LocalSettings.SaveAsync(nameof(User), value).GetAwaiter();
            }

        }

		public void ApplyStartUpSettings()
		{
			//A successful startup needs no fullscreen. A fullscreen has no visible hamburger menu.
			//IsFullScreen = false;
			//Sets the HamurgerMenu's IsOpen state.
			//Views.Shell.HamburgerMenu.IsOpen = IsHamburgerMenuOpen;
		}

		private SettingsService()
		{
			//m_settingsHelper = new Template10.Services.SettingsService.SettingsHelper();
		}

        //public async Task<string> GetServiceUrl()
        //{
        //    return await ApplicationData.Current.LocalSettings.ReadAsync<string>(nameof(ServiceUrl));
        //}
	}
}

