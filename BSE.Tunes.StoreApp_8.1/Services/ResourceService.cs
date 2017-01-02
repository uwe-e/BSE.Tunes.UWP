﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace BSE.Tunes.StoreApp.Services
{
    public class ResourceService : IResourceService
    {
        #region FieldsPrivate
        private ResourceLoader m_resourceLoader;
        #endregion

        #region MethodsPublic
        public ResourceService()
        {
            this.m_resourceLoader = new ResourceLoader();
        }
        public string GetString(string key)
        {
            return this.m_resourceLoader.GetString(key);
        }
        public string GetString(string key, string defaultValue)
        {
            string strValue = this.GetString(key);
            if (string.IsNullOrEmpty(strValue) == true)
            {
                strValue = defaultValue;
            }
            return strValue;
        }
        #endregion
    }
}