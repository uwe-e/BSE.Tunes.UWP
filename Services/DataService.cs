﻿using BSE.Tunes.Data;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Services
{
    public class DataService : IDataService
    {
        #region FieldsPrivate
        private SettingsService m_settingsService;
        private IAuthenticationService m_authenticationHandler;

        #endregion

        #region Properties
        public string ServiceUrl
        {
            get
            {
                return m_settingsService.ServiceUrl;
            }
        }
        #endregion

        #region MethodsPublic
        public DataService(IAuthenticationService authenticationHandler)
        {
            m_settingsService = SettingsService.Instance;
            m_authenticationHandler = authenticationHandler;
        }
        public static IDataService Instance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDataService>() as DataService;
            }
        }
        public async Task<bool> IsHostAccessible()
        {
            return await IsHostAccessible(m_settingsService.ServiceUrl);
        }
        public async Task<bool> IsHostAccessible(string serviceUrl)
        {
            bool isHostAccessible = false;
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(string.Format("{0}/", serviceUrl));
                try
                {
                    HttpResponseMessage message = await httpClient.GetAsync("api/tunes/IsHostAccessible");
                    if (message.IsSuccessStatusCode)
                    {
                        isHostAccessible = message.Content.ReadAsAsync<bool>().Result;
                    }
                    else
                    {
                        throw new Exception(message.ReasonPhrase);
                    }
                }
                catch (Exception exception)
                {
                    if (exception.InnerException != null)
                    {
                        throw exception.InnerException;
                    }
                    throw exception;
                }
            }
            return isHostAccessible;
        }
        public async Task<ObservableCollection<Album>> GetAlbums(Query query)
        {
            string strUrl = string.Format("{0}/api/albums", this.ServiceUrl);
            return await GetHttpResponseFromPost<ObservableCollection<Album>, Query>(new Uri(strUrl), query);
        }
        public async Task<Album> GetAlbumById(int albumId)
        {
            string strUrl = string.Format("{0}/api/albums/{1}", m_settingsService.ServiceUrl, albumId);
            return await GetHttpResponse<Album>(new Uri(strUrl));
        }
        public Uri GetImage(Guid imageId, bool asThumbnail = false)
        {
            string strUrl = string.Format("{0}/api/files/image/{1}", m_settingsService.ServiceUrl, imageId.ToString());
            if (asThumbnail)
            {
                strUrl = string.Format("{0}/api/files/image/{1}/true", m_settingsService.ServiceUrl, imageId.ToString());
            }
            return new Uri(strUrl);
        }
        public async Task<ObservableCollection<Album>> GetNewestAlbums(int limit)
        {
            string strUrl = string.Format("{0}/api/albums/{1}/newest", this.ServiceUrl, limit);
            return await GetHttpResponse<ObservableCollection<Album>>(new Uri(strUrl));
        }
        public async Task<int> GetNumberOfPlayableAlbums()
        {
            string strUrl = string.Format("{0}/api/albums/number", this.ServiceUrl);
            return await GetHttpResponse<int>(new Uri(strUrl));
        }
        public async Task<Track> GetTrackById(int trackId)
        {
            string strUrl = string.Format("{0}/api/tunes/GetTrackById/{1}", m_settingsService.ServiceUrl, trackId);
            return await GetHttpResponse<Track>(new Uri(strUrl));
        }

        public async Task<ObservableCollection<int>> GetTrackIdsByFilters(Filter filter)
        {
            string strUrl = string.Format("{0}/api/tunes/GetTrackIdsByFilters", m_settingsService.ServiceUrl);
            return await GetHttpResponseFromPost<ObservableCollection<int>, Filter>(new Uri(strUrl), filter);
        }
        public async Task<HttpClient> GetHttpClient(bool withRefreshToken = true)
        {
            var tokenResponse = this.m_authenticationHandler.TokenResponse;
            if (withRefreshToken)
            {
                tokenResponse = await this.m_authenticationHandler.RefreshToken();
            }
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            return client;
        }
        public async Task<bool> UpdateHistory(History history)
        {
            string strUrl = string.Format("{0}/api/tunes/UpdateHistory", this.ServiceUrl);
            return await GetHttpResponseFromPost<bool, History>(new Uri(strUrl), history);
        }
        #endregion

        #region MethodsPrivate
        private async Task<T> GetHttpResponseFromPost<T, U>(Uri uri, U from)
        {
            T result = default(T);
            using (var client = await this.GetHttpClient())
            {
                try
                {
                    var responseMessage = await client.PostAsJsonAsync<U>(uri, from);
                    responseMessage.EnsureExtendedSuccessStatusCode();
                    result = responseMessage.Content.ReadAsAsync<T>().Result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }
        private async Task<T> GetHttpResponse<T>(Uri uri)
        {
            T result = default(T);
            using (var client = await this.GetHttpClient())
            {
                try
                {
                    var responseMessage = await client.GetAsync(uri);
                    responseMessage.EnsureExtendedSuccessStatusCode();
                    result = responseMessage.Content.ReadAsAsync<T>().Result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }
        #endregion
    }
}
