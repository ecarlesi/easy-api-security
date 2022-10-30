using EasyApiSecurity.Core;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace EasyApiSecurity.AuthorizationManager.SqlServer
{
    public class AuthorizationManager : Core.IAuthorizationManager
    {
        private string connectionString;

        public AuthorizationManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool CanAccess(JwtInformations informations, string resource, string method)
        {
            //TODO add cache support

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string statement = @"
select * from [resource]
where 
IsPublic = 1
and [Url] = @r
and Method = @m;
;
";
                using (SqlCommand command = new SqlCommand(statement, connection))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@r", Value = resource });
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@m", Value = method });

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return true;
                        }
                    }
                }

                statement = @"
select r.[name] from [role] r
join [resourceRole] rr on r.id = rr.roleId
join [resource] s on s.id = rr.resourceId
where 
s.[url] = @r
and s. method = @m
;
";
                using (SqlCommand command = new SqlCommand(statement, connection))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@r", Value = resource });
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@m", Value = method });

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string role = reader.GetString(0);

                            string currentRole = informations.Roles.Where(x => x == role).FirstOrDefault();

                            if (currentRole != null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}