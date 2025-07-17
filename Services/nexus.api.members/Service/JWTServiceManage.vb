
'using AutoMapper.Internal;
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.IdentityModel.Tokens

Imports System.IdentityModel.Tokens.Jwt
Imports System.Security.Claims
Imports System.Text

Imports nexus.web.auth.Data

Namespace nexus.web.auth
    Public Class JWTServiceManage
        Implements IJWTTokenService
        Private ReadOnly _configuration As IConfiguration
        Private ReadOnly _dbcontext As DbContextData

        Public Sub New(configuration As IConfiguration, dbContext As DbContextData)
            _configuration = configuration
            _dbcontext = dbContext
        End Sub
        Public Function Authenticate(users As Users) As JWTTokens Implements IJWTTokenService.Authenticate

            If Not _dbcontext.Users.Any(Function(e) e.UserName Is users.UserName AndAlso e.Password Is users.Password) Then
                Return Nothing
            End If

            Dim tokenhandler = New JwtSecurityTokenHandler()
            Dim tkey = Encoding.UTF8.GetBytes(_configuration("JWTToken:key"))
            Dim ToeknDescp = New SecurityTokenDescriptor With {
                                                        .Subject = New ClaimsIdentity(New Claim() {New Claim(ClaimTypes.Name, users.UserName)}),
    .Expires = Date.UtcNow.AddMinutes(5),
    .SigningCredentials = New SigningCredentials(New SymmetricSecurityKey(tkey), SecurityAlgorithms.HmacSha256Signature)
}
            Dim toekn = tokenhandler.CreateToken(ToeknDescp)

            Return New JWTTokens With {
                .Token = tokenhandler.WriteToken(toekn)
            }

        End Function
    End Class
End Namespace
