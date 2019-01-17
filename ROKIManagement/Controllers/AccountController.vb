Imports System.Data.SqlClient
Imports System.DirectoryServices
Imports System.Web.Mvc
Imports System.Web.Script.Serialization

Namespace Controllers
    Public Class AccountController
        Inherits Controller
#Region "Application"

        Function Application() As ActionResult
            If Session("StatusLogin") = "OK" Then
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Public Function GetUsername() As String
            Dim dtUser As DataTable = New DataTable
            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                Dim _SQL As String = "SELECT UserId, Username FROM [Auth].[dbo].[Account] WHERE Admin <> 1 ORDER BY Username ASC"
                dtUser = objDB.SelectSQL(_SQL, cn)
            End Using
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtUser.Rows Select dtUser.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function GetApplication(ByVal UserId As String) As String
            Dim dtApp As DataTable = New DataTable
            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                Dim _SQL As String = "select per.permissionId as id, app.Name as appName, acc.Name as accName from [Auth].[dbo].[Permission] as per join [Auth].[dbo].[Application] as app on per.AppId = app.AppId join [Auth].[dbo].[Access] as acc on per.AccessId = acc.AccessId where per.UserId = " & UserId
                dtApp = objDB.SelectSQL(_SQL, cn)
            End Using
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtApp.Rows Select dtApp.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

#End Region

#Region "Account"

        Public Function InsertPermission(ByVal AppId As String, ByVal AccessId As String, ByVal UserId As String) As String
            Dim dtStatus As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "insert into [Auth].[dbo].[Permission] (AppId,AccessId,UserId) values (" & AppId & ", " & AccessId & ", " & UserId & ")"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
            dtStatus.Columns.Add("Status")
            dtStatus.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function DeletePermission(ByVal PerId As String) As String
            Dim dtStatus As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "delete [Auth].[dbo].[Permission] where PermissionId = " & PerId
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
            dtStatus.Columns.Add("Status")
            dtStatus.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Sub GetUsers()
            Dim searcher As DirectorySearcher = New DirectorySearcher("(&(objectCategory=person)(objectClass=user))")
            searcher.SearchScope = System.DirectoryServices.SearchScope.Subtree
            searcher.PageSize = 1000
            Dim results As SearchResultCollection = searcher.FindAll()
            Dim b = ""
            For Each result As SearchResult In results
                Dim a = result.Properties("samaccountname")
                Dim p = result.Properties("memberof")
                b &= a(0) & ","
                If a(0) = "Thanakrit.J" Then
                    Dim f = 0
                End If
            Next
            Dim x() = b.Split(",")
            Dim z = 0
        End Sub

        Function Account() As ActionResult
            If Session("StatusLogin") = "OK" Then
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Function Setting() As ActionResult
            If Session("StatusLogin") = "OK" Then
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Public Function UpDateAccount() As String
            Dim name As String = String.Empty
            Dim username As String = String.Empty
            Dim password As String = String.Empty
            Dim department As String = String.Empty
            Dim sections As String = String.Empty
            Dim email As String = String.Empty
            Dim userid As Integer = 0

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "name" Then
                    name = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "sections" Then
                    sections = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "username" Then
                    username = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "password" Then
                    password = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "department" Then
                    department = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "email" Then
                    email = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "userid" Then
                    userid = Request.Form(i)
                End If
            Next
            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                Dim _SQL As String = "update [Auth].[dbo].[Account] set [Name] = '" & name & "',[Username] = '" & username & "',[Password] = '" & password & "',[Department] = '" & department & "',[Sections] = '" & sections & "',[Email] = '" & email & "' where UserId = " & userid
                objDB.ExecuteSQL(_SQL, cn)
                objDB.DisconnectDB(cn)
            End Using
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            dtStatus.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function GetApp(ByVal UserId As String) As String
            Dim dtApp As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = " select * from [Auth].[dbo].[Application] where appId not in (select AppId from [Auth].[dbo].[Permission] where UserId = '" & UserId & "')"
            dtApp = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtApp.Rows Select dtApp.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function GetAccount() As String
            Dim dtAcc As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            'Dim _SQL As String = "SELECT u.UserId, u.Firstname, u.Lastname, u.Username, u.Department, u.email, u.IsActive, g.nameGroup, u.createBy, Format(u.createDate, 'yyyy-MM-dd HH:mm:ss') as createDate FROM [management].[dbo].[UserProfile] AS u join [management].[dbo].[Group] AS g on u.GroupId = g.GroupId"
            'Dim _SQL As String = "SELECT u.UserId, u.Firstname, u.Lastname, u.Username, u.Department, u.email, u.IsActive, g.nameGroup FROM [management].[dbo].[UserProfile] AS u join [management].[dbo].[Group] AS g on u.GroupId = g.GroupId"
            Dim _SQL As String = "select [UserId],[Name],[Username],[Department],[Sections],[Email] from [Auth].[dbo].[Account] where Admin <> 1"

            dtAcc = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtAcc.Rows Select dtAcc.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function InsertAccount() As String
            Try
                Dim name As String = String.Empty
                Dim sections As String = String.Empty
                Dim username As String = String.Empty
                Dim password As String = String.Empty
                Dim department As String = String.Empty
                Dim email As String = String.Empty
                Dim AppPer As String = String.Empty
                Dim AccPer As String = String.Empty

                For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                    If Request.Form.AllKeys(i) = "name" Then
                        name = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "sections" Then
                        sections = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "username" Then
                        username = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "password" Then
                        password = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "department" Then
                        department = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "email" Then
                        email = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "appper" Then
                        AppPer = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "accper" Then
                        AccPer = Request.Form(i)
                    End If
                Next
                Dim arrAppPer1() As String = AppPer.Split(",")
                Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                    cn.Open()
                    Dim _SQL As String = "insert into [Auth].[dbo].[Account] ([Name],[Username],[Password],[Department],[Sections],[Email],[Admin]) OUTPUT Inserted.UserId values ('" & name & "', '" & username & "', '" & password & "', '" & department & "', '" & sections & "', '" & email & "',0)"
                    Dim dtUserId As DataTable = objDB.SelectSQL(_SQL, cn)

                    If dtUserId.Rows.Count > 0 Then
                        Dim arrAppPer() As String = AppPer.Split(",")
                        Dim arrAccPer() As String = AccPer.Split(",")
                        For i As Integer = 0 To arrAccPer.Length - 2
                            _SQL = "INSERT INTO [Auth].[dbo].[Permission] ([AppId],[AccessId],[UserId]) VALUES ((select AppId from [Auth].[dbo].[Application] where name = '" & arrAppPer(i) & "'), (select AccessId from [Auth].[dbo].[Access] where name = '" & arrAccPer(i) & "'), " & dtUserId.Rows(0)("UserId") & ")"
                            objDB.ExecuteSQL(_SQL, cn)
                        Next
                    End If

                    objDB.DisconnectDB(cn)
                End Using
                Dim dtStatus As DataTable = New DataTable
                dtStatus.Columns.Add("Status")
                dtStatus.Rows.Add("OK")

            Catch ex As Exception
                Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                    cn.Open()
                    Dim _SQL As String = "INSERT INTO [management].[dbo].[log] (logdetail) VALUES('" & ex.Message & "')"
                    objDB.ExecuteSQL(_SQL, cn)
                    objDB.DisconnectDB(cn)
                End Using
            End Try
            Dim dt As DataTable = New DataTable
            dt.Columns.Add("Status")
            dt.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function GetAccountWithUserId(ByVal UserId As Integer) As String
            Dim dtGroup As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            'Dim _SQL As String = "SELECT * FROM [management].[dbo].[UserProfile] WHERE UserId = " & UserId
            Dim _SQL As String = "SELECT * FROM [Auth].[dbo].[Account] WHERE UserId = " & UserId
            dtGroup = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtGroup.Rows Select dtGroup.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function DelAccount() As String
            Dim UserId As Integer = 0
            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "UserId" Then
                    UserId = Request.Form(i)
                End If
            Next
            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                Dim _SQL As String = "DELETE [Auth].[dbo].[Account] WHERE UserId = " & UserId
                'Dim _SQL As String = "DELETE [management].[dbo].[UserProfile] WHERE UserId = " & UserId
                objDB.ExecuteSQL(_SQL, cn)
                objDB.DisconnectDB(cn)
            End Using
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            dtStatus.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

#End Region

#Region "Group"
        Function Group() As ActionResult
            If Session("StatusLogin") = "OK" Then
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Public Function GetGroup() As String
            Dim dtGroup As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [Auth].[dbo].[Access]"
            dtGroup = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtGroup.Rows Select dtGroup.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

#End Region

#Region "Login && Logout"

        Function Login() As ActionResult
            '''''' TEST AD '''''''''
            'GetUsers()
            ''''''''''''''''''''''''
            If Session("StatusLogin") = "OK" Then
                Return View("../Home/Index")
            Else
                Return View()
            End If
        End Function

        Function Logout() As ActionResult
            Session("UserId") = 0
            Session("StatusLogin") = "Fail"

            'Clear Application
            Session("QMS") = 0
            Session("ISO") = 0
            Session("IATF") = 0
            Session("Admin") = 0
            'End Clear

            Return View("../Account/Login")
        End Function


        Function CheckLogin(ByVal Username As String, ByVal Password As String) As String
            'Init App
            Session("QMS") = 0
            Session("ISO") = 0
            Session("IATF") = 0
            Session("Admin") = 0

            Dim dtPer As DataTable = New DataTable
            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                Dim _SQL As String = "select acc.Admin, acc.UserId, per.AccessId, per.AppId from [Auth].[dbo].[Account] as acc join [Auth].[dbo].[permission] as per on acc.UserId = per.UserId where acc.Username = '" & Username & "' and acc.Password = '" & Password & "'"
                dtPer = objDB.SelectSQL(_SQL, cn)
                If dtPer.Rows.Count > 0 Then
                    Session("StatusLogin") = "OK"
                    Session("UserId") = dtPer.Rows(0)("UserId")

                    'Permission App
                    For Each _Item In dtPer.Rows
                        If _Item("AppId") = 2 Then
                            Session("QMS") = _Item("AccessId")
                        ElseIf _Item("AppId") = 3 Then
                            Session("ISO") = _Item("AccessId")
                        ElseIf _Item("AppId") = 4 Then
                            Session("IATF") = _Item("AccessId")
                        End If
                    Next
                    'End Permission
                Else
                    _SQL = "select * from [Auth].[dbo].[Account] where Username = '" & Username & "' and Password = '" & Password & "'"
                    dtPer = objDB.SelectSQL(_SQL, cn)
                    If dtPer.Rows.Count > 0 Then
                        Session("StatusLogin") = "OK"
                        Session("UserId") = dtPer.Rows(0)("UserId")
                        Session("Admin") = dtPer.Rows(0)("Admin")
                    Else
                        Session("StatusLogin") = "Fail"
                    End If

                End If
                objDB.DisconnectDB(cn)
            End Using
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtPer.Rows Select dtPer.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

#End Region

    End Class
End Namespace