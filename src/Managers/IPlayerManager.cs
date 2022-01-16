using BSE.Tunes.StoreApp.Collections;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Models.Contract;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Managers
{
    public interface IPlayerManager
    {
        NavigableCollection<int> Playlist
        {
            get; set;
        }
        PlayerMode PlayerMode { get; }
        Track CurrentTrack { get; }
        TimeSpan Duration { get; }
        TimeSpan Position { get; }
        bool CanExecuteNextTrack();
        bool CanExecutePreviousTrack();
        bool CanExecutePlay();
        void ExecuteNextTrack();
        void ExecutePreviousTrack();
        Task SetTrackAsync(Track track);
        void PrepareNextTrack();
        void Play();
        void PlayTracks(ObservableCollection<int> trackIds, PlayerMode playerMode);
        void Pause();
    }
}
