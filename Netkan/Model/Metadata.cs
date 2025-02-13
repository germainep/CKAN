﻿using System;
using CKAN.Versioning;
using Newtonsoft.Json.Linq;

namespace CKAN.NetKAN.Model
{
    internal sealed class Metadata
    {
        private const string KrefPropertyName        = "$kref";
        private const string VrefPropertyName        = "$vref";
        private const string SpecVersionPropertyName = "spec_version";
        private const string VersionPropertyName     = "version";
        private const string DownloadPropertyName    = "download";
        public  const string UpdatedPropertyName     = "x_netkan_asset_updated";
        private const string StagedPropertyName      = "x_netkan_staging";

        private readonly JObject _json;

        public string        Identifier      { get { return (string)_json["identifier"]; } }
        public RemoteRef     Kref            { get; private set; }
        public RemoteRef     Vref            { get; private set; }
        public ModuleVersion SpecVersion     { get; private set; }
        public ModuleVersion Version         { get; private set; }
        public Uri           Download        { get; private set; }
        public DateTime?     RemoteTimestamp { get; private set; }
        public bool          Staged          { get; private set; }

        public Metadata(JObject json)
        {
            if (json == null)
                throw new ArgumentNullException("json");

            _json = json;

            JToken krefToken;
            if (json.TryGetValue(KrefPropertyName, out krefToken))
            {
                if (krefToken.Type == JTokenType.String)
                {
                    Kref = new RemoteRef((string)krefToken);
                }
                else
                {
                    throw new Kraken(string.Format("{0} must be a string.", KrefPropertyName));
                }
            }

            JToken vrefToken;
            if (json.TryGetValue(VrefPropertyName, out vrefToken))
            {
                if (vrefToken.Type == JTokenType.String)
                {
                    Vref = new RemoteRef((string)vrefToken);
                }
                else
                {
                    throw new Kraken(string.Format("{0} must be a string.", VrefPropertyName));
                }
            }

            JToken specVersionToken;
            if (json.TryGetValue(SpecVersionPropertyName, out specVersionToken))
            {
                if (specVersionToken.Type == JTokenType.Integer && (int)specVersionToken == 1)
                {
                    SpecVersion = new ModuleVersion("v1.0");
                }
                else if (specVersionToken.Type == JTokenType.String)
                {
                    SpecVersion = new ModuleVersion((string)specVersionToken);
                }
                else
                {
                    throw new Kraken(string.Format(@"Could not parse {0}: ""{1}""",
                        SpecVersionPropertyName,
                        specVersionToken
                    ));
                }
            }
            else
            {
                throw new Kraken(string.Format("{0} must be specified.", SpecVersionPropertyName));
            }

            JToken versionToken;
            if (json.TryGetValue(VersionPropertyName, out versionToken))
            {
                Version = new ModuleVersion((string)versionToken);
            }

            JToken downloadToken;
            if (json.TryGetValue(DownloadPropertyName, out downloadToken))
            {
                Download = new Uri((string)downloadToken);
            }

            JToken stagedToken;
            if (json.TryGetValue(StagedPropertyName, out stagedToken))
            {
                Staged = (bool)stagedToken;
            }

            JToken   updatedToken;
            DateTime t;
            if (json.TryGetValue(UpdatedPropertyName, out updatedToken)
                && DateTime.TryParse(updatedToken.ToString(), out t))
            {
                RemoteTimestamp = t;
            }
        }

        public JObject Json()
        {
            return (JObject)_json.DeepClone();
        }
    }
}
