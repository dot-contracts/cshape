//using nexus.web.auth.Model;

namespace nexus.web.auth
{
    public interface IJWTTokenService
    {
        JWTTokens Authenticate(Users users);
    }
}
