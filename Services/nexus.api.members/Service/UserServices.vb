
Imports nexus.web.auth.Data

Namespace nexus.web.auth
    Public Class UserServices
        Implements IUsers
        Private _dbcontext As DbContextData

        Public Sub New(dbContextData As DbContextData)
            _dbcontext = dbContextData
        End Sub
        Public Function GetAll() As IEnumerable(Of Users) Implements IUsers.GetAll
            Return _dbcontext.Users
        End Function

    End Class
End Namespace
