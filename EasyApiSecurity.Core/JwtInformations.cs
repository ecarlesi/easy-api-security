namespace EasyApiSecurity.Core
{
    public class JwtInformations
    {
        [ThreadStatic]
        public static JwtInformations Current;

        public string Name { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
    }
}
