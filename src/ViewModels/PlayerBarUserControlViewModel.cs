﻿using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Models.Contract;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlayerBarUserControlViewModel : ViewModelBase
    {
        private readonly PlayerManager m_playerManager;
        private readonly IDialogService m_dialogService;
        private ICommand m_playCommand;
        private RelayCommand m_previousTrackCommand;
        private RelayCommand m_nextTrackCommand;
        private ICommand m_selectItemCommand;
        private PlayerState m_playerState = PlayerState.Closed;
        private double m_iProgressValue;
        private double m_iProgressMaximumValue;
        private string m_currentProgressTime;
        private bool m_isVisible;
        private bool m_bIsPlaying;
        private double m_stepFrequency;
        private DispatcherTimer m_progressTimer;
        private Track m_currentTrack;
        private string m_currentTrackDuration;
        private Uri m_coverSource;

        public Uri CoverSource
        {
            get
            {
                return this.m_coverSource;
            }
            set
            {
                Uri oldValue = this.m_coverSource;
                this.m_coverSource = value;
                RaisePropertyChanged<Uri>(() => this.CoverSource, oldValue, value, true);
            }
        }
        public Track CurrentTrack
        {
            get
            {
                return this.m_currentTrack;
            }
            set
            {
                Track oldValue = this.m_currentTrack;
                this.m_currentTrack = value;
                RaisePropertyChanged<Track>(() => this.CurrentTrack, oldValue, value, true);
            }
        }
        public bool IsPlaying
        {
            get
            {
                return this.m_bIsPlaying;
            }
            set
            {
                this.m_bIsPlaying = value;
                RaisePropertyChanged(nameof(IsPlaying));
            }
        }
        public bool IsVisible
        {
            get
            {
                return m_isVisible;
            }
            set
            {
                m_isVisible = value;
                RaisePropertyChanged(nameof(IsVisible));
            }
        }
        public PlayerState PlayerState
        {
            get
            {
                return this.m_playerState;
            }
            set
            {
                this.m_playerState = value;
                RaisePropertyChanged(nameof(PlayerState));
            }
        }
        public double ProgressValue
        {
            get
            {
                return this.m_iProgressValue;
            }
            set
            {
                this.m_iProgressValue = value;
                RaisePropertyChanged(nameof(ProgressValue));
            }
        }
        public double ProgressMaximumValue
        {
            get
            {
                return this.m_iProgressMaximumValue;
            }
            set
            {
                this.m_iProgressMaximumValue = value;
                RaisePropertyChanged(nameof(ProgressMaximumValue));
            }
        }
        public double StepFrequency
        {
            get
            {
                return this.m_stepFrequency;
            }
            set
            {
                this.m_stepFrequency = value;
                RaisePropertyChanged(nameof(StepFrequency));
            }
        }
        public string CurrentProgressTime
        {
            get
            {
                return this.m_currentProgressTime;
            }
            set
            {
                this.m_currentProgressTime = value;
                RaisePropertyChanged(nameof(CurrentProgressTime));
            }
        }
        public string CurrentTrackDuration
        {
            get
            {
                return this.m_currentTrackDuration;
            }
            set
            {
                this.m_currentTrackDuration = value;
                RaisePropertyChanged(nameof(CurrentTrackDuration));
            }
        }
        public ICommand PlayCommand => m_playCommand ?? (m_playCommand = new RelayCommand<object>(vm => Play()));
        public ICommand SelectItemCommand => m_selectItemCommand ?? (this.m_selectItemCommand = new RelayCommand(this.SelectItem));
        public RelayCommand PreviousTrackCommand => m_previousTrackCommand ?? (m_previousTrackCommand = new RelayCommand(ExecutePreviousTrack, CanExecutePreviousTrack));
        public RelayCommand NextTrackCommand => m_nextTrackCommand ?? (m_nextTrackCommand = new RelayCommand(ExecuteNextTrack, CanExecuteNextTrack));

        public PlayerBarUserControlViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                m_playerManager = PlayerManager.Instance;
                m_playerManager.PlaylistCollectionChanged += OnPlayerManagerPlaylistCollectionChanged;
                
                m_dialogService = DialogService.Instance;

                //This prevents the alignment right of the slider´s thumb at start-up.
                this.ProgressMaximumValue = 100;
                this.ProgressValue = 0;

                //The event when the IsFullScreen property has changed.
                //Messenger.Default.Register<ScreenSizeChangedArgs>(this, args =>
                //{
                //    IsVisible = !args.IsFullScreen;
                //});
                IsVisible = true;
                Messenger.Default.Register<PlayerStateChangedArgs>(this, args =>
                {
                    OnPlayerStateChanged(args.PlayerState);
                });
                Messenger.Default.Register<MediaStateChangedArgs>(this, args =>
                {
                    switch (args.MediaState)
                    {
                        case MediaState.Opened:
                            OnMediaOpened();
                            break;
                        case MediaState.Ended:
                            OnMediaEnded();
                            break;
                    }
                });
                Messenger.Default.Register<TrackChangedArgs>(this, args =>
                {
                    CurrentTrack = args.Track;
                    LoadCoverSource(args.Track);
                });
            }
        }

        private async void Play()
        {
            try
            {
                switch (this.PlayerState)
                {
                    case PlayerState.Stopped:
                    case PlayerState.Closed:
                        if (this.m_playerManager.CanExecutePlay())
                        {
                            this.m_playerManager.ReplayPlayTracks();
                        }
                        break;
                    case PlayerState.Playing:
                        this.m_playerManager.Pause();
                        break;
                    case PlayerState.Paused:
                        this.m_playerManager.Play();
                        break;
                }
            }
            catch (Exception exception)
            {
                await m_dialogService.ShowMessageDialogAsync(exception.Message);
            }
        }
        private async void SelectItem()
        {
            await NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), CurrentTrack.Album);
        }
        private bool CanExecutePreviousTrack()
        {
            //return !this.m_trackCommandExecuted && this.m_playerManager.CanExecutePreviousTrack();
            return this.m_playerManager.CanExecutePreviousTrack();
        }
        private void ExecutePreviousTrack()
        {
            //this.m_trackCommandExecuted = true;
            this.PreviousTrackCommand.RaiseCanExecuteChanged();
            this.NextTrackCommand.RaiseCanExecuteChanged();
            this.m_playerManager.ExecutePreviousTrack();
        }
        private bool CanExecuteNextTrack()
        {
            //return !this.m_trackCommandExecuted && this.m_playerManager.CanExecuteNextTrack();
            return this.m_playerManager.CanExecuteNextTrack();
        }
        private void ExecuteNextTrack()
        {
            //this.m_trackCommandExecuted = true;
            this.PreviousTrackCommand.RaiseCanExecuteChanged();
            this.NextTrackCommand.RaiseCanExecuteChanged();
            this.m_playerManager.ExecuteNextTrack();
        }

        private void OnPlayerManagerPlaylistCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PreviousTrackCommand.RaiseCanExecuteChanged();
            NextTrackCommand.RaiseCanExecuteChanged();
        }

        private void OnPlayerStateChanged(PlayerState playerState)
        {
            this.PlayerState = playerState;
            this.IsPlaying = false;
            switch (this.PlayerState)
            {
                case PlayerState.Stopped:
                case PlayerState.Paused:
                    this.m_progressTimer?.Stop();
                    break;
                case PlayerState.Playing:
                    this.IsPlaying = true;
                    this.m_progressTimer?.Start();
                    this.PreviousTrackCommand.RaiseCanExecuteChanged();
                    this.NextTrackCommand.RaiseCanExecuteChanged();
                    break;
            }
        }
        private void OnMediaOpened()
        {
            this.ProgressValue = 0;
            this.CurrentProgressTime = TimeSpan.FromMinutes(0).ToString();
            this.CurrentTrack = this.m_playerManager.CurrentTrack;
            this.ProgressMaximumValue = this.m_playerManager.Duration.TotalSeconds;
            this.CurrentTrackDuration = this.m_playerManager.Duration.ToString();
            this.StepFrequency = this.SliderFrequency(this.m_playerManager.Duration);

            LoadCoverSource(this.CurrentTrack);

            this.m_progressTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(this.StepFrequency)
            };
            this.m_progressTimer.Tick += OnProgressTimerTick;
            this.m_progressTimer.Start();
        }

        private void OnMediaEnded()
        {
            m_progressTimer.Stop();
            m_progressTimer.Tick -= OnProgressTimerTick;
            m_progressTimer = null;
            ProgressMaximumValue = 100;
            ProgressValue = 0;
        }
        private void OnProgressTimerTick(object sender, object e)
        {
            this.ProgressValue = this.m_playerManager.Position.TotalSeconds;
            TimeSpan position = new TimeSpan(0, this.m_playerManager.Position.Hours, this.m_playerManager.Position.Minutes, this.m_playerManager.Position.Seconds);
            this.CurrentProgressTime = position.ToString();
        }
        private double SliderFrequency(TimeSpan timeSpan)
        {
            double stepfrequency = -1;

            double absvalue = (int)Math.Round(timeSpan.TotalSeconds, MidpointRounding.AwayFromZero);
            stepfrequency = (int)(Math.Round(absvalue / 100));

            if (timeSpan.TotalMinutes >= 10 && timeSpan.TotalMinutes < 30)
            {
                stepfrequency = 10;
            }
            else if (timeSpan.TotalMinutes >= 30 && timeSpan.TotalMinutes < 60)
            {
                stepfrequency = 30;
            }
            else if (timeSpan.TotalHours >= 1)
            {
                stepfrequency = 60;
            }

            if (stepfrequency == 0)
                stepfrequency += 1;

            if (stepfrequency == 1)
            {
                stepfrequency = absvalue / 100;
            }

            return stepfrequency;
        }
        private void LoadCoverSource(Track track)
        {
            if (track != null)
            {
                Uri coverSource = this.DataService.GetImage(track.Album.AlbumId);
                if (coverSource != null && !coverSource.Equals(this.CoverSource))
                {
                    this.CoverSource = coverSource;
                }
            }
        }
    }
}
