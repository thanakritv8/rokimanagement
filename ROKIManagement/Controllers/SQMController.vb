Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.Mvc
Imports System.Web.Script.Serialization

Namespace Controllers
    Public Class SQMController
        Inherits Controller

        ' GET: SQM
        Function Index() As ActionResult
            Return View()
        End Function

        Private Function GetObjMenu(ByVal obj_id As Integer) As DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[obj_menu] where obj_id = " & obj_id
            Return objDB.SelectSQL(_SQL, cn)
        End Function

        Function SQM(ByVal id As Integer) As ActionResult
            If Session("StatusLogin") = "OK" Then
                Dim Dt As DataTable = GetObjMenu(id)
                If Dt.Rows.Count > 0 Then
                    ViewBag.Obj_ID = id
                    ViewBag.Title = Dt.Rows(0)("obj_display")
                Else
                    ViewBag.Obj_ID = -1
                    ViewBag.Title = "404"
                End If
                Return View()
            Else
                Return View("../Account/Login")
            End If


        End Function

        Private Sub ClearExpanded(ByVal obj_id As String)
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[file_all] SET expanded = NULL WHERE parentDirId <> '' and obj_id = " & obj_id
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function EncryptSHA256Managed(ByVal ClearString As String) As String
            Dim uEncode As New UnicodeEncoding()
            Dim bytClearString() As Byte = uEncode.GetBytes(ClearString)
            Dim sha As New _
            System.Security.Cryptography.SHA256Managed()
            Dim hash() As Byte = sha.ComputeHash(bytClearString)
            Return Convert.ToBase64String(hash)
        End Function

        Public Function GetMenu(ByVal obj_id As String) As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[file_all] where obj_id = " & obj_id & " order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPath(ByVal Id As String, ByVal obj_id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[file_all] WHERE id = '" & Id & "' and obj_id = " & obj_id
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[file_all] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "' and obj_id = " & obj_id
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolder(ByVal Id As String, ByVal NewName As String, ByVal obj_id As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPath(Id, obj_id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/SQM/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpanded(obj_id)
            _SQL = "INSERT INTO [management].[dbo].[file_all] ([name],[id],[parentDirId],[type],[path],[icon],[expanded],[obj_id]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../../img/fd.png', 1, " & obj_id & ")"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRename(ByVal Id As String, ByVal NewName As String, ByVal obj_id As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPath(Id, obj_id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/SQM/" & Path)
            Try
                If Directory.Exists(PathServer) Then
                    FileIO.FileSystem.RenameDirectory(PathServer, NewName)
                Else
                    If System.IO.File.Exists(PathServer) Then
                        FileIO.FileSystem.RenameFile(PathServer, NewName)
                    End If
                End If
            Catch ex As Exception

            End Try

            Dim ArrPath() As String = Path.Split("/")
            Dim PathNew As String = String.Empty
            For i As Integer = 0 To ArrPath.Length - 2
                If i = ArrPath.Length - 2 Then
                    PathNew &= ArrPath(i) & "/" & NewName
                Else
                    PathNew &= ArrPath(i) & "/"
                End If
            Next
            ClearExpanded(obj_id)
            Dim _SQL As String = "Update [management].[dbo].[file_all] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "' and obj_id = " & obj_id
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[file_all] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%' and obj_id = " & obj_id
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Function fnChangeDate(ByVal id As String, ByVal start_date As String, ByVal revision As String, ByVal obj_id As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()

            ClearExpanded(obj_id)
            Dim _SQL As String = "UPDATE [management].[dbo].[file_all] SET start_date = '" & start_date & "', revision = '" & revision & "' WHERE id = '" & id & "' and obj_id = " & obj_id
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFile(ByVal obj_id As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty
            Dim start_date As String = String.Empty
            Dim revision As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "start_date" Then
                    start_date = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "revision" Then
                    revision = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPath(id, obj_id)
            Dim PathToDb As String = "../Files/Doc/SQM/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../../img/excel.png"
            End If
            ClearExpanded(obj_id)
            Dim _SQL As String = "INSERT INTO [management].[dbo].[file_all] ([name],[id],[parentDirId],[type],[path],[icon],[expanded],[start_date], [revision], [create_by], [obj_id]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'../" & PathToDb & "', '" & strIcon & "', 1, '" & start_date & "', '" & revision & "', '" & Session("UserId") & "', " & obj_id & ")"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDelete(ByVal idDel As String, ByVal obj_id As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPath(idDel, obj_id)
            Dim PathToDb As String = "../Files/Doc/SQM/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[file_all] where id = '" & idDel & "' and obj_id = " & obj_id
            Dim dtId As DataTable = objDB.SelectSQL(_SQL, cn)
            While dtId.Rows.Count > 0
                Dim id As String = String.Empty
                For i As Integer = 0 To dtId.Rows.Count - 1
                    If i = dtId.Rows.Count - 1 Then
                        id &= "'" & dtId.Rows(i)("id") & "'"
                    Else
                        id &= "'" & dtId.Rows(i)("id") & "',"
                    End If
                Next
                _SQL = "SELECT id FROM [management].[dbo].[file_all] where parentDirId in (" & id & ") and obj_id = " & obj_id
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[file_all] WHERE id in (" & id & ") and obj_id = " & obj_id
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
    End Class
End Namespace