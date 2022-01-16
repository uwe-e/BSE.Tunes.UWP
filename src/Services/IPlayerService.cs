using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Models.Contract;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BSE.Tunes.StoreApp.Services
{
    public interface IPlayerService
    {
        TimeSpan Duration { get; }
        TimeSpan Position { get; }
        Track CurrentTrack { get; }
        PlayerState CurrentState { get; }
        bool CanExecuteNextTrack { get; set; }
        bool CanExecutePreviousTrack { get; set; }
        void RegisterAsMediaService(MediaPlayerElement mediaElement);
        Task SetTrackAsync(Track track);
        Task PrepareTrack(Track track);
        void Play();
        void Pause();
        void NextTrack();
        void PreviousTrack();
    }
}
