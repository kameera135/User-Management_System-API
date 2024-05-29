namespace UMS.Api.Interfaces
{
    public interface IPasswordPolicyService
    {
        void updatePasswordPolicy(string passwordPolicy, long UpdatedBy);

        string getPasswordHash(string password);
    }
}
