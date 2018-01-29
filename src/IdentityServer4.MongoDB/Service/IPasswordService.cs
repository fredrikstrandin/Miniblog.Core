namespace IdentityServer4.MongoDB.Service
{
    public interface IPasswordService
    {
        bool CompareHash(string password, string hash, byte[] salt);
        string CreateHash(string password, byte[] salt);
        byte[] GenerateSalt();
    }
}