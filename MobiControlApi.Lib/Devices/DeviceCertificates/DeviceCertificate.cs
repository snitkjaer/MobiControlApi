using System;
namespace MobiControlApi.Devices.DeviceCertificates
{
    public class DeviceCertificate
    {
        public string ReferenceId { get; set; }
        public string SerialNumber { get; set; }
        public string SubjectName { get; set; }
        public string Subject { get; set; }
        public string IssuerName { get; set; }
        public string Issuer { get; set; }
        public DateTime NotBeforeDate { get; set; }
        public DateTime NotAfterDate { get; set; }
        public bool HasPrivateKey { get; set; }
        public bool IsRevoked { get; set; }
        public bool WasRenewed { get; set; }
        public string TemplateName { get; set; }
        public string Usage { get; set; }
        public string Status { get; set; }
        public string Thumbprint { get; set; }
        public bool CanRevoke { get; set; }
    }
}

