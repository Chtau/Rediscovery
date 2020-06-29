using Rediscovery.Models;
using SharedBase.Device;
using SharedBase.Feature;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Rediscovery.Features.Connection.Models
{
    public class ConnectionManifestFeature : BaseModel
    {
        private Guid _id;
        private Guid _configurationId;
        private string _connectionDisplayName;
        private string _featureDisplayName;
        private Guid _featureId;
        private string _featureVersion;
        private string _featureDescription;
        private string _featureMinFeatureIntegrationPoint;
        private string _featureMinControlIntegrationPoint;
        private IntegrationPoint _featureFeatureIntegrationPoint;
        private IntegrationPoint _featureControlIntegrationPoint;

        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        /// <summary>
        /// Desktop Configuration Model Id for the Connection
        /// </summary>
        public Guid ConfigurationId
        {
            get { return _configurationId; }
            set { SetProperty(ref _configurationId, value); }
        }

        public string ConnectionDisplayName
        {
            get { return _connectionDisplayName; }
            set { SetProperty(ref _connectionDisplayName, value); }
        }

        public string FeatureDisplayName
        {
            get { return _featureDisplayName; }
            set { SetProperty(ref _featureDisplayName, value); }
        }

        public Guid FeatureId
        {
            get { return _featureId; }
            set { SetProperty(ref _featureId, value); }
        }

        public string FeatureVersion
        {
            get { return _featureVersion; }
            set { SetProperty(ref _featureVersion, value); }
        }

        public string FeatureDescription
        {
            get { return _featureDescription; }
            set { SetProperty(ref _featureDescription, value); }
        }

        public string FeatureMinFeatureIntegrationPoint
        {
            get { return _featureMinFeatureIntegrationPoint; }
            set { SetProperty(ref _featureMinFeatureIntegrationPoint, value); }
        }

        public string FeatureMinControlIntegrationPoint
        {
            get { return _featureMinControlIntegrationPoint; }
            set { SetProperty(ref _featureMinControlIntegrationPoint, value); }
        }

        public IntegrationPoint FeatureFeatureIntegrationPoint
        {
            get { return _featureFeatureIntegrationPoint; }
            set { SetProperty(ref _featureFeatureIntegrationPoint, value); }
        }

        public IntegrationPoint FeatureControlIntegrationPoint
        {
            get { return _featureControlIntegrationPoint; }
            set { SetProperty(ref _featureControlIntegrationPoint, value); }
        }

        private object settingsObject;
        public object SettingsObject
        {
            get { return settingsObject; }
            set { SetProperty(ref settingsObject, value); }
        }

        public ObservableCollection<FeatureProfil> Profiles { get; set; }

        private bool isOpen;
        public bool IsOpen
        {
            get { return isOpen; }
            set { SetProperty(ref isOpen, value); }
        }
    }
}
