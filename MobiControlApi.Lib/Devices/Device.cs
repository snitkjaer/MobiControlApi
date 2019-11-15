using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MobiControlApi
{
    public partial class Device
    {
        [JsonProperty("$type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("AndroidForWork", NullValueHandling = NullValueHandling.Ignore)]
        public AndroidForWork AndroidForWork { get; set; }

        [JsonProperty("BluetoothMACAddress")]
        public object BluetoothMacAddress { get; set; }

        [JsonProperty("BuildVersion")]
        public object BuildVersion { get; set; }

        [JsonProperty("CellularTechnology", NullValueHandling = NullValueHandling.Ignore)]
        public string CellularTechnology { get; set; }

        [JsonProperty("UserIdentities", NullValueHandling = NullValueHandling.Ignore)]
        public List<UserIdentity> UserIdentities { get; set; }

        [JsonProperty("AndroidAccountType", NullValueHandling = NullValueHandling.Ignore)]
        public string AndroidAccountType { get; set; }

        [JsonProperty("ElmStatus", NullValueHandling = NullValueHandling.Ignore)]
        public string ElmStatus { get; set; }

        [JsonProperty("AgentUpgradeEnabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AgentUpgradeEnabled { get; set; }

        [JsonProperty("AgentVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string AgentVersion { get; set; }

        [JsonProperty("AndroidApiLevel", NullValueHandling = NullValueHandling.Ignore)]
        public long? AndroidApiLevel { get; set; }

        [JsonProperty("AndroidDeviceAdmin", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AndroidDeviceAdmin { get; set; }

        [JsonProperty("AndroidRcLibraryVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string AndroidRcLibraryVersion { get; set; }

        [JsonProperty("Antivirus", NullValueHandling = NullValueHandling.Ignore)]
        public Antivirus Antivirus { get; set; }

        [JsonProperty("AsuLevel", NullValueHandling = NullValueHandling.Ignore)]
        public long? AsuLevel { get; set; }

        [JsonProperty("Memory", NullValueHandling = NullValueHandling.Ignore)]
        public Memory Memory { get; set; }

        [JsonProperty("BatteryStatus", NullValueHandling = NullValueHandling.Ignore)]
        public long? BatteryStatus { get; set; }

        [JsonProperty("CanResetPassword", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CanResetPassword { get; set; }

        [JsonProperty("CellularCarrier", NullValueHandling = NullValueHandling.Ignore)]
        public string CellularCarrier { get; set; }

        [JsonProperty("CellularSignalStrength", NullValueHandling = NullValueHandling.Ignore)]
        public long? CellularSignalStrength { get; set; }

        [JsonProperty("CustomData", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> CustomData { get; set; }

        [JsonProperty("DeviceTerms")]
        public object DeviceTerms { get; set; }

        [JsonProperty("DeviceUserInfo")]
        public object DeviceUserInfo { get; set; }

        [JsonProperty("ExchangeBlocked", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ExchangeBlocked { get; set; }

        [JsonProperty("ExchangeStatus", NullValueHandling = NullValueHandling.Ignore)]
        public string ExchangeStatus { get; set; }

        [JsonProperty("HardwareEncryptionCaps", NullValueHandling = NullValueHandling.Ignore)]
        public long? HardwareEncryptionCaps { get; set; }

        [JsonProperty("HardwareSerialNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string HardwareSerialNumber { get; set; }

        [JsonProperty("HardwareVersion")]
        public object HardwareVersion { get; set; }

        [JsonProperty("ICCID", NullValueHandling = NullValueHandling.Ignore)]
        public string Iccid { get; set; }

        [JsonProperty("IMEI_MEID_ESN", NullValueHandling = NullValueHandling.Ignore)]
        public string ImeiMeidEsn { get; set; }

        [JsonProperty("InRoaming", NullValueHandling = NullValueHandling.Ignore)]
        public bool? InRoaming { get; set; }

        [JsonProperty("IntegratedApplications")]
        public object IntegratedApplications { get; set; }

        [JsonProperty("Ipv6", NullValueHandling = NullValueHandling.Ignore)]
        public string Ipv6 { get; set; }

        [JsonProperty("IsAgentCompatible", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsAgentCompatible { get; set; }

        [JsonProperty("IsAgentless", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsAgentless { get; set; }

        [JsonProperty("IsEncrypted", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsEncrypted { get; set; }

        [JsonProperty("IsOSSecure", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsOsSecure { get; set; }

        [JsonProperty("LastCheckInTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? LastCheckInTime { get; set; }

        [JsonProperty("LastAgentConnectTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? LastAgentConnectTime { get; set; }

        [JsonProperty("LastAgentDisconnectTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? LastAgentDisconnectTime { get; set; }

        [JsonProperty("LastLoggedOnUser", NullValueHandling = NullValueHandling.Ignore)]
        public string LastLoggedOnUser { get; set; }

        [JsonProperty("NetworkBSSID", NullValueHandling = NullValueHandling.Ignore)]
        public string NetworkBssid { get; set; }

        [JsonProperty("NetworkConnectionType", NullValueHandling = NullValueHandling.Ignore)]
        public string NetworkConnectionType { get; set; }

        [JsonProperty("NetworkRSSI", NullValueHandling = NullValueHandling.Ignore)]
        public long? NetworkRssi { get; set; }

        [JsonProperty("NetworkSSID", NullValueHandling = NullValueHandling.Ignore)]
        public string NetworkSsid { get; set; }

        [JsonProperty("OEMVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string OemVersion { get; set; }

        [JsonProperty("PasscodeEnabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PasscodeEnabled { get; set; }

        [JsonProperty("PasscodeStatus", NullValueHandling = NullValueHandling.Ignore)]
        public string PasscodeStatus { get; set; }

        [JsonProperty("PersonalizedName", NullValueHandling = NullValueHandling.Ignore)]
        public string PersonalizedName { get; set; }

        [JsonProperty("PhoneNumber")]
        public object PhoneNumber { get; set; }

        [JsonProperty("SEForAndroidStatus", NullValueHandling = NullValueHandling.Ignore)]
        public string SeForAndroidStatus { get; set; }

        [JsonProperty("SelectedApn", NullValueHandling = NullValueHandling.Ignore)]
        public string SelectedApn { get; set; }

        [JsonProperty("SubscriberNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string SubscriberNumber { get; set; }

        [JsonProperty("SupportedApis", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> SupportedApis { get; set; }

        [JsonProperty("EFOTAFirmwareVersion")]
        public object EfotaFirmwareVersion { get; set; }

        [JsonProperty("CarrierCode")]
        public object CarrierCode { get; set; }

        [JsonProperty("DeviceFirmwareUpgrade")]
        public object DeviceFirmwareUpgrade { get; set; }

        [JsonProperty("BuildSecurityPatch", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? BuildSecurityPatch { get; set; }

        [JsonProperty("Kind", NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; }

        [JsonProperty("ComplianceStatus", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ComplianceStatus { get; set; }

        [JsonProperty("ComplianceItems", NullValueHandling = NullValueHandling.Ignore)]
        public List<ComplianceItem> ComplianceItems { get; set; }

        [JsonProperty("DeviceId", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceId { get; set; }

        [JsonProperty("DeviceName", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceName { get; set; }

        [JsonProperty("EnrollmentTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? EnrollmentTime { get; set; }

        [JsonProperty("Family", NullValueHandling = NullValueHandling.Ignore)]
        public string Family { get; set; }

        [JsonProperty("HostName", NullValueHandling = NullValueHandling.Ignore)]
        public string HostName { get; set; }

        [JsonProperty("IsAgentOnline", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsAgentOnline { get; set; }

        [JsonProperty("CustomAttributes", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> CustomAttributes { get; set; }

        [JsonProperty("MACAddress", NullValueHandling = NullValueHandling.Ignore)]
        public string MacAddress { get; set; }

        [JsonProperty("Manufacturer", NullValueHandling = NullValueHandling.Ignore)]
        public string Manufacturer { get; set; }

        [JsonProperty("Mode", NullValueHandling = NullValueHandling.Ignore)]
        public string Mode { get; set; }

        [JsonProperty("Model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }

        [JsonProperty("OSVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string OsVersion { get; set; }

        [JsonProperty("Path", NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty("Platform", NullValueHandling = NullValueHandling.Ignore)]
        public string Platform { get; set; }

        [JsonProperty("ServerName", NullValueHandling = NullValueHandling.Ignore)]
        public string ServerName { get; set; }
    }

    public partial class AndroidForWork
    {
        [JsonProperty("$type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("AfwProfileDisabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AfwProfileDisabled { get; set; }

        [JsonProperty("AfwProvisionStage", NullValueHandling = NullValueHandling.Ignore)]
        public string AfwProvisionStage { get; set; }

        [JsonProperty("AfwManagementType", NullValueHandling = NullValueHandling.Ignore)]
        public string AfwManagementType { get; set; }
    }

    public partial class Antivirus
    {
        [JsonProperty("$type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("AntivirusDefinitionsVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string AntivirusDefinitionsVersion { get; set; }

        [JsonProperty("LastEmptyQuarantine", NullValueHandling = NullValueHandling.Ignore)]
        public long? LastEmptyQuarantine { get; set; }

        [JsonProperty("LastVirusDefUpdate", NullValueHandling = NullValueHandling.Ignore)]
        public long? LastVirusDefUpdate { get; set; }

        [JsonProperty("LastVirusScan", NullValueHandling = NullValueHandling.Ignore)]
        public long? LastVirusScan { get; set; }
    }

    public partial class ComplianceItem
    {
        [JsonProperty("$type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("ComplianceType", NullValueHandling = NullValueHandling.Ignore)]
        public string ComplianceType { get; set; }

        [JsonProperty("ComplianceValue")]
        public bool? ComplianceValue { get; set; }
    }

    public partial class Memory
    {
        [JsonProperty("$type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("AvailableExternalStorage", NullValueHandling = NullValueHandling.Ignore)]
        public long? AvailableExternalStorage { get; set; }

        [JsonProperty("AvailableMemory", NullValueHandling = NullValueHandling.Ignore)]
        public long? AvailableMemory { get; set; }

        [JsonProperty("AvailableSDCardStorage")]
        public object AvailableSdCardStorage { get; set; }

        [JsonProperty("AvailableStorage", NullValueHandling = NullValueHandling.Ignore)]
        public long? AvailableStorage { get; set; }

        [JsonProperty("TotalExternalStorage", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalExternalStorage { get; set; }

        [JsonProperty("TotalMemory", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalMemory { get; set; }

        [JsonProperty("TotalSDCardStorage")]
        public object TotalSdCardStorage { get; set; }

        [JsonProperty("TotalStorage", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalStorage { get; set; }
    }

    public partial class UserIdentity
    {
        [JsonProperty("$type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("Email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }
    }

    public partial class Device
    {
        public static Device FromJson(string json) => JsonConvert.DeserializeObject<Device>(json, Helpers.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Device self) => JsonConvert.SerializeObject(self, Helpers.Converter.Settings);
    }


}
