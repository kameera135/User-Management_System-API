using Microsoft.Win32;
using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace LicenseLibrary
{
    public class License
    {
        private string BUILD = "[1.00 2024-04-01]";
        private string m_machineKey { get; set; } = "";
        private string m_publicKey { get; set; } = "";
        private string m_licensePath { get; set; } = "";
        public License()
        {
            SetMachineKey();
        }
        public void SetPublicKey(string publicKey)
        {
            m_publicKey = publicKey;
        }
        public void SetLicensePath(string licensePath)
        {
            m_licensePath = licensePath;
        }
        private void SetMachineKey()
        {
            string hardwareProfile = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\IDConfigDB\Hardware Profiles\0001", "HwProfileGuid", null).ToString();
            m_machineKey = hardwareProfile.Replace("{", "").Replace("}", "").Replace("-", "").Trim().ToUpper();
        }
        private string GetEncryptedKey()
        {
            // Create key for decryption using machine key and public key
            int[] pattern = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };

            string decryptionKey = "";
            foreach (int position in pattern)
            {
                if (position % 2 == 0)
                {
                    decryptionKey += m_machineKey[position - 1];
                }
                else
                {
                    decryptionKey += m_publicKey[position - 1];
                }
            }
            //Return decryption key
            return decryptionKey;
        }

        public bool isLicenseValid(string platform)
        {
            //decrypt the license - return false for any error
            //check platform in platform list
            //return true if in
            try
            {
                string decryptionKey = GetEncryptedKey();

                if (decryptionKey != null)
                {
                    string fileContent = File.ReadAllText(m_licensePath);

                    if (fileContent != null)
                    {
                        JObject jsonObject = JObject.Parse(fileContent);

                        string decryptedLicenseType = Decrypt((string)jsonObject["license_type"], decryptionKey);
                        string decryptedPackages = Decrypt((string)jsonObject["packages"], decryptionKey);
                        string decryptedExpDate = Decrypt((string)jsonObject["exp_date"], decryptionKey);


                        JObject decryptedLicense = new JObject();

                        decryptedLicense["license_type"] = decryptedLicenseType;
                        decryptedLicense["packages"] = decryptedPackages;
                        decryptedLicense["exp_date"] = decryptedExpDate;


                        if (decryptedLicense != null && !string.IsNullOrEmpty(decryptedLicenseType) && !string.IsNullOrEmpty(decryptedPackages) && !string.IsNullOrEmpty(decryptedExpDate))
                        {
                            if(decryptedLicenseType == "trial")
                            {
                                DateTime expDate = DateTime.Parse((string)decryptedLicense["exp_date"]);

                                if (expDate.Date >= DateTime.Now.Date)
                                {
                                    string packages = (string)decryptedLicense["packages"];
                                    string[] packagesList = packages.Split(",");

                                    return packagesList.Contains(platform);
                                }
                            }
                            else
                            {
                                string packages = (string)decryptedLicense["packages"];
                                string[] packagesList = packages.Split(",");

                                return packagesList.Contains(platform);
                            }
                        }
                    }
                }
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                return false;

            }

            catch (Exception)
            {
                // Handle other exceptions silently and return false
                return false;
            }

            // Return false if any exception occurred during the process or if the license is invalid
            return false;

        }

        public string LicenseInfo()
        {
            try
            {
                string decryptionKey = GetEncryptedKey();
                string fileContent = File.ReadAllText(m_licensePath);

                // Parse the JSON content
                JObject jsonObject = JObject.Parse(fileContent);

                // Decrypt and return the license info as JSON
                JObject decryptedLicense = new JObject();
                decryptedLicense["license_id"] = Decrypt((string)jsonObject["license_id"], decryptionKey).ToUpper();
                decryptedLicense["client"] = Decrypt((string)jsonObject["client"], decryptionKey);
                decryptedLicense["pc_name"] = Decrypt((string)jsonObject["pc_name"], decryptionKey);
                decryptedLicense["license_type"] = Decrypt((string)jsonObject["license_type"], decryptionKey);
                decryptedLicense["token"] = Decrypt((string)jsonObject["token"], decryptionKey).ToUpper();
                decryptedLicense["packages"] = Decrypt((string)jsonObject["packages"], decryptionKey);
                decryptedLicense["start_date"] = Decrypt((string)jsonObject["start_date"], decryptionKey);
                decryptedLicense["exp_date"] = Decrypt((string)jsonObject["exp_date"], decryptionKey);
                decryptedLicense["signature"] = Decrypt((string)jsonObject["signature"], decryptionKey);
                decryptedLicense["generated_at"] = (string)jsonObject["generated_at"];

                return decryptedLicense.ToString();
            }
            catch (Exception ex)
            {
                // Log and handle exceptions
                LogError("LicenseInfo", ex.Message);
                return null;
            }
        }
        private static string Decrypt(string encryptedData, string key)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

            // Extract IV and tag from the encrypted data
            byte[] iv = encryptedBytes[..12]; // IV length is 12 bytes
            byte[] tag = encryptedBytes[12..28]; // GCM tag length is 16 bytes
            byte[] cipherText = encryptedBytes[28..]; // Rest is cipher text

            // Convert key to bytes
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            // Create AesGcm instance
            using AesGcm aesGcm = new AesGcm(keyBytes);

            // Create plaintext buffer
            byte[] decryptedBytes = new byte[cipherText.Length];

            // Decrypt the data
            aesGcm.Decrypt(iv, cipherText, tag, decryptedBytes);

            // Convert decrypted bytes to string
            string decryptedData = Encoding.UTF8.GetString(decryptedBytes);

            return decryptedData;
        }
        private void LogError(string methodName, string errorMessage)
        {
            // Error log message
            string logMessage = $"Error in method '{methodName}': {errorMessage}"; 

            // File path to error.log
            string logFilePath = "D:\\LicenseRequestApp\\bin\\Debug\\net6.0-windows\\error.log"; 

            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {logMessage}");
                }
            }
            catch (Exception ex)
            {
                // If logging fails, display the error in a message box
                LogError("LogError", ex.Message);
            }
        }
    }
}
