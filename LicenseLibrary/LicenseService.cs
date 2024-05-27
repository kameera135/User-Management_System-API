using Microsoft.Extensions.Configuration;

namespace LicenseLibrary
{
    public class LicenseService : ILicense
    {
        private License m_license;
        private IConfiguration m_configuration;

        public static string licensePath = "LicenseInfo:LicensePath";
        public static string licensePubKey = "LicenseInfo:LicensePubKey";
        public static string licenseFile = "LicenseInfo:LicenseFile";

        public LicenseService(IConfiguration configuration )
        {
            m_configuration = configuration;

            m_license = new License();

            var path = m_configuration.GetSection(licensePath).Value;
            var pub = m_configuration.GetSection(licensePubKey).Value;
            var file = m_configuration.GetSection(licenseFile).Value;

            m_license.SetLicensePath(path + "//" + file);
            m_license.SetPublicKey(pub);
        }

        public bool isLicenseValid(string platform)
        {
            return m_license.isLicenseValid(platform);
        }
    }
}
