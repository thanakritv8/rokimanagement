Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.Mvc
Imports System.Web.Script.Serialization

Namespace Controllers
    Public Class DrawingController
        Inherits Controller

        ' GET: Drawing
        Function Index() As ActionResult
            Return View()
        End Function

        Public Function EncryptSHA256Managed(ByVal ClearString As String) As String
            Dim uEncode As New UnicodeEncoding()
            Dim bytClearString() As Byte = uEncode.GetBytes(ClearString)
            Dim sha As New _
            System.Security.Cryptography.SHA256Managed()
            Dim hash() As Byte = sha.ComputeHash(bytClearString)
            Return Convert.ToBase64String(hash)
        End Function

#Region "HTAC"
        Function HATC() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedHATC()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedHATC()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[hatc] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuHATC() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[hatc] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathHATC(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[hatc] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[hatc] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderHATC(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathHATC(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedHATC()
            _SQL = "INSERT INTO [management].[dbo].[hatc] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameHATC(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathHATC(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedHATC()
            Dim _SQL As String = "Update [management].[dbo].[hatc] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[hatc] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileHATC()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathHATC(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedHATC()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[hatc] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteHATC(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathHATC(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[hatc] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[hatc] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[hatc] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

#End Region

#Region "THM"
        Function THM() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedTHM()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedTHM()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[thm] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuTHM() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[thm] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathTHM(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[thm] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[thm] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderTHM(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathTHM(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedTHM()
            _SQL = "INSERT INTO [management].[dbo].[thm] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameTHM(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathTHM(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedTHM()
            Dim _SQL As String = "Update [management].[dbo].[thm] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[thm] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        'Public Function fnChangeDateHATC(ByVal id As String)    'wait
        '    Dim dtStatus As DataTable = New DataTable
        '    dtStatus.Columns.Add("Status")
        '    Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
        '    cn.Open()

        '    ClearExpandedHATC()
        '    Dim _SQL As String = "UPDATE [management].[dbo].[hatc] SET start_date = '" & start_date & "', revision = '" & revision & "' WHERE id = '" & id & "'"
        '    objDB.ExecuteSQL(_SQL, cn)
        '    dtStatus.Rows.Add("OK")
        '    objDB.DisconnectDB(cn)
        '    Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        'End Function

        Public Function fnNewFileTHM()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathTHM(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedTHM()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[thm] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteTHM(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathTHM(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[thm] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[thm] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[thm] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

#End Region

#Region "TSM"
        Function TSM() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedTSM()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedTSM()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[tsm] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuTSM() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[tsm] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathTSM(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[tsm] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[tsm] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderTSM(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathTSM(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedTSM()
            _SQL = "INSERT INTO [management].[dbo].[tsm] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameTSM(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathTSM(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedTSM()
            Dim _SQL As String = "Update [management].[dbo].[tsm] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[tsm] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        'Public Function fnChangeDateHATC(ByVal id As String)    'wait
        '    Dim dtStatus As DataTable = New DataTable
        '    dtStatus.Columns.Add("Status")
        '    Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
        '    cn.Open()

        '    ClearExpandedHATC()
        '    Dim _SQL As String = "UPDATE [management].[dbo].[hatc] SET start_date = '" & start_date & "', revision = '" & revision & "' WHERE id = '" & id & "'"
        '    objDB.ExecuteSQL(_SQL, cn)
        '    dtStatus.Rows.Add("OK")
        '    objDB.DisconnectDB(cn)
        '    Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        'End Function

        Public Function fnNewFileTSM()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathTSM(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedTSM()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[tsm] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteTSM(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathTSM(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[tsm] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[tsm] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[tsm] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

#End Region

#Region "AAT"
        Function AAT() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedAAT()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedAAT()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[aat] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuAAT() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[aat] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathAAT(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[aat] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[aat] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderAAT(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathAAT(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedTSM()
            _SQL = "INSERT INTO [management].[dbo].[aat] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameAAT(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathAAT(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedAAT()
            Dim _SQL As String = "Update [management].[dbo].[aat] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[aat] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileAAT()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathAAT(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedAAT()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[aat] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteAAT(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathAAT(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[aat] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[aat] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[aat] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "DUCATI"
        Function DUCATI() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedDUCATI()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedDUCATI()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[DUCATI] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuDUCATI() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[DUCATI] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathDUCATI(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[DUCATI] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[DUCATI] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderDUCATI(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathDUCATI(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedDUCATI()
            _SQL = "INSERT INTO [management].[dbo].[DUCATI] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameDUCATI(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathDUCATI(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedDUCATI()
            Dim _SQL As String = "Update [management].[dbo].[DUCATI] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[DUCATI] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileDUCATI()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathDUCATI(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedDUCATI()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[DUCATI] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteDUCATI(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathDUCATI(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[DUCATI] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[DUCATI] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[DUCATI] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "HRAP"
        Function HRAP() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedHRAP()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedHRAP()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[HRAP] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuHRAP() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[HRAP] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathHRAP(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[HRAP] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[HRAP] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderHRAP(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathHRAP(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedHRAP()
            _SQL = "INSERT INTO [management].[dbo].[HRAP] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameHRAP(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathHRAP(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedHRAP()
            Dim _SQL As String = "Update [management].[dbo].[HRAP] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[HRAP] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileHRAP()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathHRAP(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedHRAP()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[HRAP] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteHRAP(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathHRAP(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[HRAP] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[HRAP] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[HRAP] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "HRST"
        Function HRST() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedHRST()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedHRST()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[HRST] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuHRST() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[HRST] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathHRST(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[HRST] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[HRST] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderHRST(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathHRST(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedHRST()
            _SQL = "INSERT INTO [management].[dbo].[HRST] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameHRST(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathHRST(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedHRST()
            Dim _SQL As String = "Update [management].[dbo].[HRST] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[HRST] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileHRST()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathHRST(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedHRST()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[HRST] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteHRST(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathHRST(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[HRST] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[HRST] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[HRST] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "HTAS"
        Function HTAS() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedHTAS()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedHTAS()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[HTAS] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuHTAS() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[HTAS] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathHTAS(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[HTAS] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[HTAS] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderHTAS(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathHTAS(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedHTAS()
            _SQL = "INSERT INTO [management].[dbo].[HTAS] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameHTAS(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathHTAS(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedHTAS()
            Dim _SQL As String = "Update [management].[dbo].[HTAS] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[HTAS] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileHTAS()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathHTAS(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedHTAS()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[HTAS] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteHTAS(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathHTAS(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[HTAS] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[HTAS] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[HTAS] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "IMCT"
        Function IMCT() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedIMCT()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedIMCT()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[IMCT] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuIMCT() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[IMCT] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathIMCT(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[IMCT] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[IMCT] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderIMCT(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathIMCT(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedIMCT()
            _SQL = "INSERT INTO [management].[dbo].[IMCT] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameIMCT(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathIMCT(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedIMCT()
            Dim _SQL As String = "Update [management].[dbo].[IMCT] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[IMCT] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileIMCT()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathIMCT(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedIMCT()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[IMCT] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteIMCT(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathIMCT(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[IMCT] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[IMCT] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[IMCT] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "KMT"
        Function KMT() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedKMT()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedKMT()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[KMT] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuKMT() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[KMT] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathKMT(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[KMT] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[KMT] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderKMT(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathKMT(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedKMT()
            _SQL = "INSERT INTO [management].[dbo].[KMT] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameKMT(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathKMT(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedKMT()
            Dim _SQL As String = "Update [management].[dbo].[KMT] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[KMT] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileKMT()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathKMT(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedKMT()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[KMT] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteKMT(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathKMT(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[KMT] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[KMT] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[KMT] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "MAZDA"
        Function MAZDA() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedMAZDA()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedMAZDA()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[MAZDA] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuMAZDA() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[MAZDA] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathMAZDA(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[MAZDA] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[MAZDA] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderMAZDA(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathMAZDA(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedMAZDA()
            _SQL = "INSERT INTO [management].[dbo].[MAZDA] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameMAZDA(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathMAZDA(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedMAZDA()
            Dim _SQL As String = "Update [management].[dbo].[MAZDA] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[MAZDA] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileMAZDA()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathMAZDA(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedMAZDA()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[MAZDA] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteMAZDA(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathMAZDA(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[MAZDA] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[MAZDA] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[MAZDA] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "META"
        Function META() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedMETA()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedMETA()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[META] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuMETA() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[META] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathMETA(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[META] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[META] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderMETA(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathMETA(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedMETA()
            _SQL = "INSERT INTO [management].[dbo].[META] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameMETA(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathMETA(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedMETA()
            Dim _SQL As String = "Update [management].[dbo].[META] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[META] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileMETA()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathMETA(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedMETA()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[META] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteMETA(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathMETA(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[META] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[META] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[META] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "MMTH"
        Function MMTH() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedMMTH()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedMMTH()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[MMTH] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuMMTH() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[MMTH] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathMMTH(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[MMTH] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[MMTH] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderMMTH(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathMMTH(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedMMTH()
            _SQL = "INSERT INTO [management].[dbo].[MMTH] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameMMTH(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathMMTH(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedMMTH()
            Dim _SQL As String = "Update [management].[dbo].[MMTH] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[MMTH] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileMMTH()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathMMTH(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedMMTH()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[MMTH] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteMMTH(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathMMTH(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[MMTH] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[MMTH] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[MMTH] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "NMT"
        Function NMT() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedNMT()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedNMT()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[NMT] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuNMT() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[NMT] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathNMT(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[NMT] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[NMT] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderNMT(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathNMT(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedNMT()
            _SQL = "INSERT INTO [management].[dbo].[NMT] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameNMT(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathNMT(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedNMT()
            Dim _SQL As String = "Update [management].[dbo].[NMT] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[NMT] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileNMT()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathNMT(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedNMT()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[NMT] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteNMT(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathNMT(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[NMT] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[NMT] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[NMT] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "RJP"
        Function RJP() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedRJP()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedRJP()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[RJP] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuRJP() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[RJP] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathRJP(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[RJP] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[RJP] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderRJP(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathRJP(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedRJP()
            _SQL = "INSERT INTO [management].[dbo].[RJP] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameRJP(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathRJP(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedRJP()
            Dim _SQL As String = "Update [management].[dbo].[RJP] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[RJP] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileRJP()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathRJP(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedRJP()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[RJP] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteRJP(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathRJP(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[RJP] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[RJP] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[RJP] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "TMT"
        Function TMT() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedTMT()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedTMT()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[TMT] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuTMT() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[TMT] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathTMT(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[TMT] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[TMT] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderTMT(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathTMT(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedTMT()
            _SQL = "INSERT INTO [management].[dbo].[TMT] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameTMT(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathTMT(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedTMT()
            Dim _SQL As String = "Update [management].[dbo].[TMT] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[TMT] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileTMT()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathTMT(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedTMT()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[TMT] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteTMT(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathTMT(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[TMT] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[TMT] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[TMT] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#Region "TYM"
        Function TYM() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedTYM()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedTYM()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [management].[dbo].[TYM] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuTYM() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[TYM] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Private Function fnGetPathTYM(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _Path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[TYM] WHERE id = '" & Id & "'"
            Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
            If dtPdi.Rows.Count > 0 Then
                _Path = dtPdi.Rows(0)("name")
                While dtPdi.Rows.Count > 0
                    _SQL = "SELECT parentDirId, name FROM [management].[dbo].[TYM] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "'"
                    dtPdi = objDB.SelectSQL(_SQL, cn)
                    If dtPdi.Rows.Count > 0 Then
                        _Path = dtPdi.Rows(0)("name") & "/" & _Path
                    End If
                End While
            End If
            objDB.DisconnectDB(cn)
            Return _Path
        End Function

        Public Function fnNewFolderTYM(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathTYM(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            ClearExpandedTYM()
            _SQL = "INSERT INTO [management].[dbo].[TYM] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameTYM(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim Path As String = fnGetPathTYM(Id)
            Dim PathServer As String = Server.MapPath("../Files/Doc/Drawing/" & Path)
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
            ClearExpandedTYM()
            Dim _SQL As String = "Update [management].[dbo].[TYM] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)

            _SQL = "Update [management].[dbo].[TYM] SET path = REPLACE(path,N'" & Path & "', N'" & PathNew & "') WHERE path like N'%" & Path & "%'"
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileTYM()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim newName As String = String.Empty
            Dim id As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "id" Then
                    id = Request.Form(i)
                End If
            Next
            Dim Path As String = fnGetPathTYM(id)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path & "/" & newName
            Dim PathServer As String = Server.MapPath(PathToDb)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".jpg") Or arrFile(arrFile.Length - 1).Contains(".png") Then
                strIcon = "../img/pic.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".ppt") Then
                strIcon = "../img/ppt.png"
            Else
                strIcon = "../img/paper.png"
            End If
            ClearExpandedTYM()
            Dim _SQL As String = "INSERT INTO [management].[dbo].[TYM] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1',N'" & PathToDb & "', '" & strIcon & "', 1)"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteTYM(ByVal idDel As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim Path As String = fnGetPathTYM(idDel)
            Dim PathToDb As String = "../Files/Doc/Drawing/" & Path
            Dim PathServer As String = Server.MapPath(PathToDb)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            _SQL = "SELECT id FROM [management].[dbo].[TYM] where id = '" & idDel & "'"
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
                _SQL = "SELECT id FROM [management].[dbo].[TYM] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[TYM] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region


    End Class
End Namespace