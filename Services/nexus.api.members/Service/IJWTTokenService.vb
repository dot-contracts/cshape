'using nexus.web.auth.Model;

Namespace nexus.web.auth
    Public Interface IJWTTokenService
        Function Authenticate(users As Users) As JWTTokens
    End Interface
End Namespace
