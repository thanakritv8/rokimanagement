Imports System.Data.SqlClient
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

        Public Function UploadApplication(ByVal AppId As Integer, ByVal GroupId As Integer) As String
            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                ' Create the command with the sproc name and add the parameter required'
                Dim cmd As SqlCommand = New SqlCommand("[management].[dbo].[sp_UploadApp]", cn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@AppId", AppId)
                cmd.Parameters.AddWithValue("@GroupId", GroupId)
                cmd.Parameters.AddWithValue("@username", Session("UserId"))
                Using r = cmd.ExecuteReader()
                    If r.Read() Then

                    End If
                End Using
                objDB.DisconnectDB(cn)
            End Using
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            dtStatus.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function DeleteApplication(ByVal AppId As Integer, ByVal GroupId As Integer) As String
            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                ' Create the command with the sproc name and add the parameter required'
                Dim cmd As SqlCommand = New SqlCommand("[management].[dbo].[sp_DelApp]", cn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@AppId", AppId)
                cmd.Parameters.AddWithValue("@GroupId", GroupId)
                Using r = cmd.ExecuteReader()
                    If r.Read() Then

                    End If
                End Using
                objDB.DisconnectDB(cn)
            End Using
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            dtStatus.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function GetApplication(ByVal GroupId As Integer) As String
            Dim dtGroup As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT a.AppId, a.nameApp, isnull(p.Groupid, 0) as ck FROM [management].[dbo].[Application] as a left join [management].[dbo].[Premission] as p on a.AppId = p.AppId where p.groupid = " & GroupId & " union select a.appid, a.nameapp, 0 as ck FROM [management].[dbo].[Application] as a left join [management].[dbo].[Premission] as p on a.AppId = p.AppId where p.GroupId <> " & GroupId & " and p.AppId Not in (SELECT a.AppId FROM [management].[dbo].[Application] as a left join [management].[dbo].[Premission] as p on a.AppId = p.AppId where p.groupid = " & GroupId & " )"
            dtGroup = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtGroup.Rows Select dtGroup.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

#End Region

#Region "Account"

        Function Account() As ActionResult
            If Session("StatusLogin") = "OK" Then
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Public Function UpDateAccount() As String
            Dim firstname As String = String.Empty
            Dim lastname As String = String.Empty
            Dim username As String = String.Empty
            Dim password As String = String.Empty
            Dim department As String = String.Empty
            Dim email As String = String.Empty
            Dim isactive As Integer = 0
            Dim groupid As Integer = 0
            Dim userid As Integer = 0

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "firstname" Then
                    firstname = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "lastname" Then
                    lastname = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "username" Then
                    username = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "password" Then
                    password = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "department" Then
                    department = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "email" Then
                    email = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "isactive" Then
                    isactive = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "groupid" Then
                    groupid = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "userid" Then
                    userid = Request.Form(i)
                End If
            Next
            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                Dim cmd As SqlCommand = New SqlCommand("[management].[dbo].[sp_UpdateAccount]", cn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@firstname", firstname)
                cmd.Parameters.AddWithValue("@lastname", lastname)
                cmd.Parameters.AddWithValue("@username", username)
                cmd.Parameters.AddWithValue("@password", password)
                cmd.Parameters.AddWithValue("@department", department)
                cmd.Parameters.AddWithValue("@email", email)
                cmd.Parameters.AddWithValue("@createBy", Session("UserId"))
                cmd.Parameters.AddWithValue("@groupid", groupid)
                cmd.Parameters.AddWithValue("@isactive", isactive)
                cmd.Parameters.AddWithValue("@userid", userid)
                Using r = cmd.ExecuteReader()
                    If r.Read() Then

                    End If
                End Using
                objDB.DisconnectDB(cn)
            End Using
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            dtStatus.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function GetAccount() As String
            Dim dtGroup As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            'Dim _SQL As String = "SELECT u.UserId, u.Firstname, u.Lastname, u.Username, u.Department, u.email, u.IsActive, g.nameGroup, u.createBy, Format(u.createDate, 'yyyy-MM-dd HH:mm:ss') as createDate FROM [management].[dbo].[UserProfile] AS u join [management].[dbo].[Group] AS g on u.GroupId = g.GroupId"
            Dim _SQL As String = "SELECT u.UserId, u.Firstname, u.Lastname, u.Username, u.Department, u.email, u.IsActive, g.nameGroup FROM [management].[dbo].[UserProfile] AS u join [management].[dbo].[Group] AS g on u.GroupId = g.GroupId"
            dtGroup = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtGroup.Rows Select dtGroup.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function UploadAccount() As String
            Try
                Dim firstname As String = String.Empty
                Dim lastname As String = String.Empty
                Dim username As String = String.Empty
                Dim password As String = String.Empty
                Dim department As String = String.Empty
                Dim email As String = String.Empty
                Dim groupid As Integer = 0

                For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                    If Request.Form.AllKeys(i) = "firstname" Then
                        firstname = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "lastname" Then
                        lastname = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "username" Then
                        username = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "password" Then
                        password = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "department" Then
                        department = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "email" Then
                        email = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "groupid" Then
                        groupid = Request.Form(i)
                    End If
                Next
                Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                    cn.Open()
                    ' Create the command with the sproc name and add the parameter required'
                    Dim cmd As SqlCommand = New SqlCommand("[management].[dbo].[sp_InsertAccount]", cn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@firstname", firstname)
                    cmd.Parameters.AddWithValue("@lastname", lastname)
                    cmd.Parameters.AddWithValue("@username", username)
                    cmd.Parameters.AddWithValue("@password", password)
                    cmd.Parameters.AddWithValue("@department", department)
                    cmd.Parameters.AddWithValue("@email", email)
                    cmd.Parameters.AddWithValue("@createBy", Session("UserId"))
                    cmd.Parameters.AddWithValue("@groupid", groupid)
                    Using r = cmd.ExecuteReader()
                        If r.Read() Then

                        End If
                    End Using
                    objDB.DisconnectDB(cn)
                End Using

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
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[UserProfile] WHERE UserId = " & UserId
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

                Dim _SQL As String = "DELETE [management].[dbo].[UserProfile] WHERE UserId = " & UserId
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

        Public Function DelGroup() As String
            Dim GroupId As Integer = 0
            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "GroupId" Then
                    GroupId = Request.Form(i)
                End If
            Next
            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()

                Dim _SQL As String = "DELETE [management].[dbo].[Group] WHERE GroupId = " & GroupId
                objDB.ExecuteSQL(_SQL, cn)
                objDB.DisconnectDB(cn)
            End Using
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            dtStatus.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function GetGroup() As String
            Dim dtGroup As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT GroupId, nameGroup, Format(createDate, 'yyyy-MM-dd HH:mm:ss') as createDate, createBy, isnull(Format(updateDate, 'yyyy-MM-dd HH:mm:ss'), '')  as updateDate, isnull(updateBy, '') as updateBy, isnull(remark, '') as remark FROM [management].[dbo].[Group]"
            dtGroup = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtGroup.Rows Select dtGroup.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function GetGroupWithGroupId(ByVal GroupId As Integer) As String
            Dim dtGroup As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[Group] WHERE GroupId = " & GroupId
            dtGroup = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtGroup.Rows Select dtGroup.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function UpDateGroup() As String
            Dim GroupId As Integer = 0
            Dim nameGroup As String = String.Empty
            Dim remark As String = String.Empty
            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "nameGroup" Then
                    nameGroup = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "remark" Then
                    remark = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "GroupId" Then
                    GroupId = Request.Form(i)
                End If
            Next
            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                Dim cmd As SqlCommand = New SqlCommand("[management].[dbo].[sp_UpdateGroup]", cn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@nameGroup", nameGroup)
                cmd.Parameters.AddWithValue("@remark", remark)
                cmd.Parameters.AddWithValue("@GroupId", GroupId)
                cmd.Parameters.AddWithValue("@username", Session("UserId"))
                Using r = cmd.ExecuteReader()
                    If r.Read() Then

                    End If
                End Using
                objDB.DisconnectDB(cn)
            End Using
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            dtStatus.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function UploadGroup() As String
            Try
                Dim nameGroup As String = String.Empty
                Dim remark As String = String.Empty
                For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                    If Request.Form.AllKeys(i) = "nameGroup" Then
                        nameGroup = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "remark" Then
                        remark = Request.Form(i)
                    End If
                Next
                Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                    cn.Open()
                    ' Create the command with the sproc name and add the parameter required'
                    Dim cmd As SqlCommand = New SqlCommand("[management].[dbo].[sp_InsertGroup]", cn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@nameGroup", nameGroup)
                    cmd.Parameters.AddWithValue("@remark", remark)
                    cmd.Parameters.AddWithValue("@username", Session("UserId"))
                    Using r = cmd.ExecuteReader()
                        If r.Read() Then

                        End If
                    End Using
                    objDB.DisconnectDB(cn)
                End Using

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
#End Region

#Region "Login && Logout"

        Function Login() As ActionResult
            'Session("StatusLogin") = "Something"
            If Session("StatusLogin") = "OK" Then
                Return View("../Home/Index")
            Else
                Return View()
            End If
        End Function

        Function Logout() As ActionResult
            Session("UserId") = 0
            Session("StatusLogin") = "Fail"
            Return View("../Account/Login")
        End Function

        Function CheckLogin(ByVal Username As String, ByVal Password As String) As String
            Dim dtUser As DataTable = New DataTable
            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                Dim _SQL As String = "SELECT * FROM [management].[dbo].[UserProfile] WHERE Username = '" & Username & "' AND Password = '" & Password & "' AND IsActive = 1"
                dtUser = objDB.SelectSQL(_SQL, cn)
                If dtUser.Rows.Count > 0 Then
                    Session("StatusLogin") = "OK"
                    Session("UserId") = dtUser.Rows(0)("UserId")
                    Session("GroupId") = dtUser.Rows(0)("GroupId")
                Else
                    Session("StatusLogin") = "Fail"
                End If
                objDB.DisconnectDB(cn)
            End Using
            If dtUser Is Nothing Then
                dtUser.Columns.Add("UserId")
                dtUser.Rows.Add(0)
            End If
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtUser.Rows Select dtUser.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

#End Region

    End Class
End Namespace