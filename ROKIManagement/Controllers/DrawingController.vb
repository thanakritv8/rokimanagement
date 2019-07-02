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


#Region "DMS V2 20190614"

#Region "ActionResult"

        Function Drawing(ByVal id As Integer) As ActionResult
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

#End Region

#Region "Private Function"
        Private Function GetObjMenu(ByVal obj_id As Integer) As DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [dms].[dbo].[obj_menu] where obj_id = " & obj_id
            Return objDB.SelectSQL(_SQL, cn)
        End Function
        Private Sub ClearExpandedDMS(ByVal obj_id As String)
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [dms].[dbo].[file_all] SET expanded = NULL WHERE parentDirId <> '' and obj_id = " & obj_id
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        'Private Function fnGetPathDMS(ByVal Id As String, ByVal obj_id As String) As String
        '    Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
        '    cn.Open()
        '    Dim _Path As String = String.Empty
        '    Dim _SQL As String = "SELECT parentDirId, name, seq FROM [dms].[dbo].[file_all] WHERE id = '" & Id & "' and obj_id = " & obj_id
        '    Dim dtPdi As DataTable = objDB.SelectSQL(_SQL, cn)
        '    If dtPdi.Rows.Count > 0 Then
        '        _Path = dtPdi.Rows(0)("seq")
        '        While dtPdi.Rows.Count > 0
        '            _SQL = "SELECT parentDirId, name, seq FROM [dms].[dbo].[file_all] WHERE id = '" & dtPdi.Rows(0)("parentDirId") & "' and obj_id = " & obj_id
        '            dtPdi = objDB.SelectSQL(_SQL, cn)
        '            If dtPdi.Rows.Count > 0 Then
        '                _Path = dtPdi.Rows(0)("seq") & "/" & _Path
        '            End If
        '        End While
        '    End If
        '    objDB.DisconnectDB(cn)
        '    Return _Path
        'End Function

#End Region

#Region "Public Function"

        Public Function GetMenuDMS(ByVal obj_id As String) As String
            ClearExpandedDMS(obj_id)
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [dms].[dbo].[file_all] where obj_id = " & obj_id & " order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Public Function fnNewFolderDMS(ByVal Id As String, ByVal NewName As String, ByVal obj_id As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Try
                ClearExpandedDMS(obj_id)
                Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                Dim _SQL As String = String.Empty
                _SQL = "INSERT INTO [dms].[dbo].[file_all] ([name],[id],[parentDirId],[type],[path],[icon],[expanded],[obj_id]) OUTPUT Inserted.seq VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1, " & obj_id & ")"
                Dim ReTurnId As String = objDB.ExecuteSQLReturnId(_SQL, cn)
                dtStatus.Rows.Add("OK")
                objDB.DisconnectDB(cn)
            Catch ex As Exception
                dtStatus.Rows.Add("NOK")
            End Try

            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameDMS(ByVal Id As String, ByVal NewName As String, ByVal obj_id As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            ClearExpandedDMS(obj_id)
            Dim _SQL As String = "Update [dms].[dbo].[file_all] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "' and obj_id = " & obj_id
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileDMS(ByVal obj_id As String)
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

            'Edit by Thung 20190619
            Dim _SQLSEQ As String = "SELECT seq FROM [dms].[dbo].[file_all] WHERE id = '" & id & "'"
            Dim _DtSeq As DataTable = objDB.SelectSQL(_SQLSEQ, cn)

            ClearExpandedDMS(obj_id)
            Dim _SQL As String = "INSERT INTO [dms].[dbo].[file_all] ([name],[id],[parentDirId],[type],[expanded], [create_by], [obj_id]) OUTPUT Inserted.seq VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1', 1, '" & Session("UserId") & "', " & obj_id & ")"
            Dim ReturnId As String = objDB.ExecuteSQLReturnId(_SQL, cn)
            Dim pathNoFile As String = "../Files/Doc/V2/Drawing/" & _DtSeq.Rows(0)("seq")
            Dim PathToDb As String = "../Files/Doc/V2/Drawing/" & _DtSeq.Rows(0)("seq") & "/" & ReturnId
            If (Not System.IO.Directory.Exists(Server.MapPath(pathNoFile))) Then
                System.IO.Directory.CreateDirectory(Server.MapPath(pathNoFile))
            End If

            Dim PathServer As String = Server.MapPath(PathToDb)
            Dim arrContentType() As String = newName.Split(".")
            Dim ContentType As String = arrContentType(arrContentType.Length - 1)
            PathServer &= "." & ContentType
            PathToDb &= "." & ContentType
            Dim arrFile() As String = PathToDb.Split("/")
            Dim strIcon As String = "doc"
            If arrFile(arrFile.Length - 1).Contains(".pdf") Then
                strIcon = "../img/pdf.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".doc") Then
                strIcon = "../img/word.png"
            ElseIf arrFile(arrFile.Length - 1).Contains(".xl") Then
                strIcon = "../img/excel.png"
            End If

            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                file.SaveAs(PathServer)
            Next

            _SQL = "UPDATE [dms].[dbo].[file_all] SET path = N'" & PathToDb & "', icon = '" & strIcon & "' WHERE seq = " & ReturnId & " and obj_id = " & obj_id
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteDMS(ByVal idDel As String, ByVal obj_id As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Try
                Dim dtSeq As DataTable = New DataTable
                Dim _SQL As String = "Select id, type, path, seq FROM [dms].[dbo].[file_all] where id = '" & idDel & "' and obj_id = " & obj_id
                Dim dtId As DataTable = objDB.SelectSQL(_SQL, cn)
                'Dim bWork As Boolean = True
                While dtId.Rows.Count > 0
                    Dim id As String = String.Empty
                    For i As Integer = 0 To dtId.Rows.Count - 1
                        If i = dtId.Rows.Count - 1 Then
                            id &= "'" & dtId.Rows(i)("id") & "'"
                        Else
                            id &= "'" & dtId.Rows(i)("id") & "',"
                        End If
                        Dim pathseqdel As String = String.Empty

                        If dtId.Rows(i)("Type") = 1 Then
                            pathseqdel = dtId.Rows(i)("path")
                            If System.IO.File.Exists(Server.MapPath(pathseqdel)) Then
                                System.IO.File.Delete(Server.MapPath(pathseqdel))
                            End If
                        Else
                            pathseqdel = "../Files/Doc/V2/Drawing/" & dtId.Rows(i)("seq")
                            If System.IO.Directory.Exists(Server.MapPath(pathseqdel)) Then
                                Directory.Delete(Server.MapPath(pathseqdel), True)
                            End If
                        End If
                    Next

                    _SQL = "SELECT seq, id, type, path FROM [dms].[dbo].[file_all] where parentDirId in (" & id & ") and obj_id = " & obj_id
                    dtId = objDB.SelectSQL(_SQL, cn)
                    _SQL = "DELETE [dms].[dbo].[file_all] WHERE id in (" & id & ") and obj_id = " & obj_id
                    objDB.ExecuteSQL(_SQL, cn)
                End While
            Catch ex As Exception
                Dim _SQL As String = "insert into [dms].[dbo].[log_error] (remark) values ('" & ex.Message & "')"
            End Try


            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function
#End Region

#End Region

    End Class
End Namespace