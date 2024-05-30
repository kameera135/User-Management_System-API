using CommonInsfrastructure.Interfaces;
using System.Text;
using System.Security.Cryptography;
using UMS.Api.Interfaces;

namespace UMS.Api.Services
{
    public class PasswordPolicyService : IPasswordPolicyService
    {
        private readonly ICommonConfigRepository m_commonConfigRepository;

        public PasswordPolicyService(ICommonConfigRepository commonConfigRepository)
        {
            m_commonConfigRepository = commonConfigRepository;
        }

        public void updatePasswordPolicy(string passwordPolicy, long UpdatedBy)
        {
            try
            {
                m_commonConfigRepository.PutCommonSettingData("UMS_PP", passwordPolicy, UpdatedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string getPasswordHash(string password, string salt)
        {
            // Combine the password and salt
            string combinedPassword = password;

            // Choose the hash algorithm (SHA-256 or SHA-512)
            using (var sha512 = SHA512.Create())
            {
                // Convert the combined password string to a byte array
                byte[] bytes = Encoding.UTF8.GetBytes(combinedPassword);

                // Compute the hash value of the byte array
                byte[] hash = sha512.ComputeHash(bytes);

                // Convert the byte array to a hexadecimal string
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    result.Append(hash[i].ToString("x2"));
                }

                return result.ToString();
            }
        }

        public string generateSalt()
        {
            int length = 20; // Fixed length of 20 digits
            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                byte[] randomByte = new byte[1];
                RandomNumberGenerator.Fill(randomByte);

                // Convert byte to a digit between 0 and 9
                int digit = randomByte[0] % 10;

                // Append the digit to the string builder
                sb.Append(digit);
            }

            return sb.ToString();
        }

        protected string MD5(string text)
        {
            using var provider = System.Security.Cryptography.MD5.Create();
            StringBuilder builder = new StringBuilder();

            foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(text)))
                builder.Append(b.ToString("x2").ToLower());

            return builder.ToString();
        }
    }
}