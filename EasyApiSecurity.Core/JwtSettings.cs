namespace EasyApiSecurity.Core
{
    public class JwtSettings
    {
        public string Audience { get; init; } = null!;
        public string Issuer { get; init; } = null!;
        public byte[] Key { get; init; } = null!;
        public KeyType KeyType { get; init; }
        public int Lifetime { get; init; }
    }

    public enum KeyType
    {
        Certificate,
        SymmetricKey
    }
}