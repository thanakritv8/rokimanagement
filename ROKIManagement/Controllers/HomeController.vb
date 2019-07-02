Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.Mvc
Imports System.Web.Script.Serialization

Namespace Controllers
    Public Class HomeController
        Inherits Controller

        Function Index() As ActionResult
            'fnMove("O3M/61iUpsdNVTLmzWFiMt6hCbgPpOxZqs+XGQM4MuM=", "/e1m9jxhN4/mGl8cqC121Kden4SITXkekrtj1bfEmYE=")

            If Session("StatusLogin") = "OK" Then
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Function ComingSoon() As ActionResult
            If Session("StatusLogin") = "OK" Then
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Public Function EncryptSHA256Managed(ByVal ClearString As String) As String
            Dim uEncode As New UnicodeEncoding()
            Dim bytClearString() As Byte = uEncode.GetBytes(ClearString)
            Dim sha As New _
            System.Security.Cryptography.SHA256Managed()
            Dim hash() As Byte = sha.ComputeHash(bytClearString)
            Return Convert.ToBase64String(hash)
        End Function

#Region "QMS"

        Function DocQms() As ActionResult
            If Session("StatusLogin") = "OK" Then
                Return View()
            Else
                Return View("../Account/Login")
            End If

        End Function

        Public Function GetDoc(ByVal Kind As String) As String
            Dim da As DataSetDMSTableAdapters.sp_GetDocTableAdapter = New DataSetDMSTableAdapters.sp_GetDocTableAdapter
            Dim dt As DataSetDMS.sp_GetDocDataTable = New DataSetDMS.sp_GetDocDataTable
            da.Fill(dt, Kind)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function GetDocWithSeq(ByVal Kind As String, ByVal Seq As Integer) As String
            Dim da As DataSetDMSTableAdapters.sp_GetDocWithSeqTableAdapter = New DataSetDMSTableAdapters.sp_GetDocWithSeqTableAdapter
            Dim dt As DataSetDMS.sp_GetDocWithSeqDataTable = New DataSetDMS.sp_GetDocWithSeqDataTable
            da.Fill(dt, Kind, Seq)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function UploadDocQMS() As String
            Try
                Dim nameJP As String = String.Empty
                Dim noJp As String = String.Empty
                Dim nameTH As String = String.Empty
                Dim noTH As String = String.Empty
                For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                    If Request.Form.AllKeys(i) = "nameJP" Then
                        nameJP = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "noJP" Then
                        noJp = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "nameTH" Then
                        nameTH = Request.Form(i)
                    ElseIf Request.Form.AllKeys(i) = "noTH" Then
                        noTH = Request.Form(i)
                    End If
                Next
                Dim ext() As String = {"", ""}
                For i As Integer = 0 To Request.Files.Count - 1
                    Dim file = Request.Files(i)
                    Dim path As String = String.Empty
                    Try
                        Dim p As String = file.FileName
                        Dim extension As String = System.IO.Path.GetExtension(p)
                        If i = 0 And nameJP <> String.Empty Then
                            path = Server.MapPath("../Files/Doc/QMS/JP/") & nameJP & extension
                            ext(0) = extension
                        Else
                            path = Server.MapPath("../Files/Doc/QMS/TH/") & nameTH & extension
                            ext(1) = extension
                        End If
                        file.SaveAs(path)
                    Catch ex As Exception
                        Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                            cn.Open()
                            Dim _SQL As String = "INSERT INTO [dms].[dbo].[log] (logdetail) VALUES('" & path & "=>" & ex.Message & "')"
                            objDB.ExecuteSQL(_SQL, cn)
                            objDB.DisconnectDB(cn)
                        End Using
                    End Try

                Next
                Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                    cn.Open()
                    Dim pathJP As String = String.Empty
                    Dim pathTH As String = String.Empty
                    If nameJP <> String.Empty Then
                        pathJP = "../Files/Doc/QMS/JP/" & nameJP & ext(0)
                    End If
                    If nameTH <> String.Empty Then
                        pathTH = "../Files/Doc/QMS/TH/" & nameTH & ext(1)
                    End If
                    ' Create the command with the sproc name and add the parameter required'
                    Dim cmd As SqlCommand = New SqlCommand("[dms].[dbo].[sp_InsertDoc]", cn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@pathJP", pathJP)
                    cmd.Parameters.AddWithValue("@nameJP", nameJP)
                    cmd.Parameters.AddWithValue("@noJP", noJp)
                    cmd.Parameters.AddWithValue("@pathTH", pathTH)
                    cmd.Parameters.AddWithValue("@nameTH", nameTH)
                    cmd.Parameters.AddWithValue("@noTH", noTH)
                    cmd.Parameters.AddWithValue("@username", Session("UserId"))
                    cmd.Parameters.AddWithValue("@kind", "QMS")
                    Using r = cmd.ExecuteReader()
                        If r.Read() Then

                        End If
                    End Using
                    objDB.DisconnectDB(cn)
                End Using

            Catch ex As Exception
                Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                    cn.Open()
                    Dim _SQL As String = "INSERT INTO [dms].[dbo].[log] (logdetail) VALUES('" & ex.Message & "')"
                    objDB.ExecuteSQL(_SQL, cn)
                    objDB.DisconnectDB(cn)
                End Using
            End Try
            Dim dt As DataTable = New DataTable
            dt.Columns.Add("Status")
            dt.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function DelDocQMS() As String
            Dim cbDelJP As Boolean = False
            Dim cbDelTH As Boolean = False
            Dim seq As Integer = 0
            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "cbDelJP" Then
                    cbDelJP = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "cbDelTH" Then
                    cbDelTH = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "seq" Then
                    seq = Request.Form(i)
                End If
            Next
            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                If cbDelJP And cbDelTH Then
                    Dim _SQL As String = "DELETE [dms].[dbo].[document] WHERE seq = " & seq
                    objDB.ExecuteSQL(_SQL, cn)
                ElseIf cbDelJP And Not cbDelTH Then

                    Dim _SQL As String = "SELECT * FROM [dms].[dbo].[document] WHERE nameFileTH <> '' AND seq = " & seq
                    Dim dt As DataTable = objDB.SelectSQL(_SQL, cn)
                    If dt.Rows.Count > 0 Then
                        _SQL = "UPDATE [dms].[dbo].[document] SET pathFileJP = '', nameFileJP = '', docNoJP = '' WHERE seq = " & seq
                        objDB.ExecuteSQL(_SQL, cn)
                    Else
                        _SQL = "DELETE [dms].[dbo].[document] WHERE seq = " & seq
                        objDB.ExecuteSQL(_SQL, cn)
                    End If
                ElseIf Not cbDelJP And cbDelTH Then
                    Dim _SQL As String = "SELECT * FROM [dms].[dbo].[document] WHERE nameFileJP <> '' AND seq = " & seq
                    Dim dt As DataTable = objDB.SelectSQL(_SQL, cn)
                    If dt.Rows.Count > 0 Then
                        _SQL = "UPDATE [dms].[dbo].[document] SET pathFileTH = '', nameFileTH = '', docNoTH = '' WHERE seq = " & seq
                        objDB.ExecuteSQL(_SQL, cn)
                    Else
                        _SQL = "DELETE [dms].[dbo].[document] WHERE seq = " & seq
                        objDB.ExecuteSQL(_SQL, cn)
                    End If
                End If
                objDB.DisconnectDB(cn)
            End Using
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            dtStatus.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function UpDateDocQMS() As String
            Dim nameJP As String = String.Empty
            Dim noJp As String = String.Empty
            Dim nameTH As String = String.Empty
            Dim noTH As String = String.Empty
            Dim typeFileJP As String = ".pdf"
            Dim typeFileTH As String = ".pdf"
            Dim seq As Integer = 0
            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "nameJP" Then
                    nameJP = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "noJP" Then
                    noJp = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "nameTH" Then
                    nameTH = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "noTH" Then
                    noTH = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "seq" Then
                    seq = Request.Form(i)
                End If
            Next

            Dim da As DataSetDMSTableAdapters.sp_GetDocWithSeqTableAdapter = New DataSetDMSTableAdapters.sp_GetDocWithSeqTableAdapter
            Dim dt As DataSetDMS.sp_GetDocWithSeqDataTable = New DataSetDMS.sp_GetDocWithSeqDataTable
            da.Fill(dt, "QMS", seq)
            If dt.Rows.Count > 0 Then
                Dim oldPathJP As String = dt.Rows(0)("pathFileJP").ToString
                Dim oldPathTH As String = dt.Rows(0)("pathFileTH").ToString
                Dim newPathJP As String = String.Empty
                Dim newPathTH As String = String.Empty

                'JP
                'If oldPathJP <> "../Files/Doc/QMS/JP/" & nameJP & typeFileJP And Request.Files.Count > 0 Then
                If oldPathJP <> "../Files/Doc/QMS/JP/" & nameJP & typeFileJP And Request.Files.Count > 0 Then
                    Dim file = Request.Files(0)
                    Dim p As String = file.FileName
                    Dim extension As String = System.IO.Path.GetExtension(p)
                    If Request.Files.AllKeys(0) = "pathJP" Then
                        newPathJP = Server.MapPath("~/Files/Doc/QMS/JP/") & nameJP & extension
                        If oldPathJP = String.Empty Then
                            Request.Files(0).SaveAs(newPathJP)
                        Else
                            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                                cn.Open()
                                Dim _SQL As String = "SELECT * FROM [dms].[dbo].[document] WHERE pathFileJP = '" & oldPathJP & "'"
                                Dim dtCheckPathJP As DataTable = objDB.SelectSQL(_SQL, cn)
                                If dtCheckPathJP.Rows.Count > 1 Then
                                    Request.Files(0).SaveAs(newPathJP)
                                Else
                                    Dim oldPathJPTemp As String = oldPathJP
                                    oldPathJPTemp.Replace("..", "~")
                                    oldPathJPTemp = Server.MapPath(oldPathJPTemp)
                                    If System.IO.File.Exists(oldPathJPTemp) Then
                                        System.IO.File.Delete(oldPathJPTemp)
                                        Request.Files(0).SaveAs(newPathJP)
                                    End If
                                End If
                                objDB.DisconnectDB(cn)
                            End Using
                        End If
                    Else
                        newPathJP = oldPathJP
                    End If
                Else
                    newPathJP = oldPathJP
                End If

                'TH
                If oldPathTH <> "../Files/Doc/QMS/TH/" & nameTH & typeFileJP And Request.Files.Count > 0 Then
                    Dim file = Request.Files(0)
                    Dim p As String = file.FileName
                    Dim extension As String = System.IO.Path.GetExtension(p)
                    newPathTH = Server.MapPath("../Files/Doc/QMS/TH/") & nameTH & extension
                    If oldPathTH = String.Empty Then
                        Request.Files(Request.Files.Count - 1).SaveAs(newPathTH)
                    Else
                        If (Request.Files.Count = 1 And Request.Files.AllKeys(0) = "pathTH") Or (Request.Files.Count > 1 AndAlso Request.Files.AllKeys(1) = "pathTH") Then
                            Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                                cn.Open()
                                Dim _SQL As String = "SELECT * FROM [dms].[dbo].[document] WHERE pathFileTH = '" & oldPathTH & "'"
                                Dim dtCheckPathTH As DataTable = objDB.SelectSQL(_SQL, cn)
                                If dtCheckPathTH.Rows.Count > 1 Then
                                    Request.Files(Request.Files.Count - 1).SaveAs(newPathTH)
                                Else
                                    Dim oldPathTHTemp As String = oldPathTH
                                    'oldPathTHTemp.Replace("..", "~")
                                    oldPathTHTemp = Server.MapPath(oldPathTHTemp)
                                    If System.IO.File.Exists(oldPathTHTemp) Then
                                        System.IO.File.Delete(oldPathTHTemp)
                                        Request.Files(Request.Files.Count - 1).SaveAs(newPathTH)
                                    End If
                                End If
                                objDB.DisconnectDB(cn)
                            End Using
                        Else
                            newPathTH = oldPathTH
                        End If
                    End If
                Else
                    newPathTH = oldPathTH
                End If

                'Update document
                Using cn = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                    cn.Open()
                    'Dim pathJP As String = String.Empty
                    'Dim pathTH As String = String.Empty
                    'If nameJP <> String.Empty Then
                    '    pathJP = "../Files/Doc/QMS/JP/" & nameJP & ".pdf"
                    'End If
                    'If nameTH <> String.Empty Then
                    '    pathTH = "../Files/Doc/QMS/TH/" & nameTH & ".pdf"
                    'End If
                    If newPathJP <> String.Empty Then
                        newPathJP = newPathJP.Replace("\", "/")
                        newPathJP = ".." & newPathJP.Substring(newPathJP.IndexOf("/Files/Doc/QMS/JP/"))
                    End If

                    If newPathTH <> String.Empty Then
                        newPathTH = newPathTH.Replace("\", "/")
                        newPathTH = ".." & newPathTH.Substring(newPathTH.IndexOf("/Files/Doc/QMS/TH/"))
                    End If

                    Dim cmd As SqlCommand = New SqlCommand("[dms].[dbo].[sp_UpdateDoc]", cn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@pathJP", newPathJP)
                    cmd.Parameters.AddWithValue("@nameJP", nameJP)
                    cmd.Parameters.AddWithValue("@noJP", noJp)
                    cmd.Parameters.AddWithValue("@pathTH", newPathTH)
                    cmd.Parameters.AddWithValue("@nameTH", nameTH)
                    cmd.Parameters.AddWithValue("@noTH", noTH)
                    cmd.Parameters.AddWithValue("@username", "admin")
                    cmd.Parameters.AddWithValue("@kind", "QMS")
                    cmd.Parameters.AddWithValue("@seq", seq)
                    Using r = cmd.ExecuteReader()
                        If r.Read() Then

                        End If
                    End Using
                    objDB.DisconnectDB(cn)
                End Using
            End If

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            dtStatus.Rows.Add("OK")
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

#End Region

#Region "IATF"
        Function IATF() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpanded()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpanded()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [dms].[dbo].[iatf] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuIATF() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [dms].[dbo].[iatf] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Public Function fnNewFolder(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Try
                ClearExpanded()
                Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                Dim _SQL As String = String.Empty
                _SQL = "INSERT INTO [dms].[dbo].[iatf] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) OUTPUT Inserted.seq VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
                Dim ReTurnId As String = objDB.ExecuteSQLReturnId(_SQL, cn)
                dtStatus.Rows.Add("OK")
                objDB.DisconnectDB(cn)
            Catch ex As Exception
                dtStatus.Rows.Add("NOK")
            End Try

            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRename(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            ClearExpanded()
            Dim _SQL As String = "Update [dms].[dbo].[iatf] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Function fnChangeDateIATF(ByVal id As String, ByVal start_date As String, ByVal revision As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()

            ClearExpanded()
            Dim _SQL As String = "UPDATE [dms].[dbo].[iatf] SET start_date = '" & start_date & "', revision = '" & revision & "' WHERE id = '" & id & "'"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFile()
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

            'Edit by Thung 20190619
            Dim _SQLSEQ As String = "SELECT seq FROM [dms].[dbo].[iatf] WHERE id = '" & id & "'"
            Dim _DtSeq As DataTable = objDB.SelectSQL(_SQLSEQ, cn)

            ClearExpanded()
            Dim _SQL As String = "INSERT INTO [dms].[dbo].[iatf] ([name],[id],[parentDirId],[type],[expanded], [create_by], [start_date], [revision]) OUTPUT Inserted.seq VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1', 1, '" & Session("UserId") & "', '" & start_date & "', '" & revision & "')"
            Dim ReturnId As String = objDB.ExecuteSQLReturnId(_SQL, cn)
            Dim pathNoFile As String = "../Files/Doc/V2/IATF/" & _DtSeq.Rows(0)("seq")
            Dim PathToDb As String = "../Files/Doc/V2/IATF/" & _DtSeq.Rows(0)("seq") & "/" & ReturnId
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

            _SQL = "UPDATE [dms].[dbo].[iatf] SET path = N'" & PathToDb & "', icon = '" & strIcon & "' WHERE seq = " & ReturnId
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDelete(ByVal idDel As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Try
                Dim dtSeq As DataTable = New DataTable
                Dim _SQL As String = "Select id, type, path, seq FROM [dms].[dbo].[iatf] where id = '" & idDel & "'"
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
                            pathseqdel = "../Files/Doc/V2/IATF/" & dtId.Rows(i)("seq")
                            If System.IO.Directory.Exists(Server.MapPath(pathseqdel)) Then
                                Directory.Delete(Server.MapPath(pathseqdel), True)
                            End If
                        End If
                    Next

                    _SQL = "SELECT seq, id, type, path FROM [dms].[dbo].[iatf] where parentDirId in (" & id & ")"
                    dtId = objDB.SelectSQL(_SQL, cn)
                    _SQL = "DELETE [dms].[dbo].[iatf] WHERE id in (" & id & ")"
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

#Region "SQM ROKI"
        Function SQMROKI() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedSqmRoki()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedSqmRoki()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [dms].[dbo].[sqm_roki] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuSqmRoki() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [dms].[dbo].[sqm_roki] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Public Function fnNewFolderSqmRoki(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Try
                ClearExpandedSqmRoki()
                Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                Dim _SQL As String = String.Empty
                _SQL = "INSERT INTO [dms].[dbo].[sqm_roki] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) OUTPUT Inserted.seq VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
                Dim ReTurnId As String = objDB.ExecuteSQLReturnId(_SQL, cn)
                dtStatus.Rows.Add("OK")
                objDB.DisconnectDB(cn)
            Catch ex As Exception
                dtStatus.Rows.Add("NOK")
            End Try

            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameSqmRoki(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            ClearExpandedSqmRoki()
            Dim _SQL As String = "Update [dms].[dbo].[sqm_roki] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Function fnChangeDateSqmRoki(ByVal id As String, ByVal start_date As String, ByVal revision As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()

            ClearExpandedSqmRoki()
            Dim _SQL As String = "UPDATE [dms].[dbo].[sqm_roki] SET start_date = '" & start_date & "', revision = '" & revision & "' WHERE id = '" & id & "'"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileSqmRoki()
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

            'Edit by Thung 20190619
            Dim _SQLSEQ As String = "SELECT seq FROM [dms].[dbo].[sqm_roki] WHERE id = '" & id & "'"
            Dim _DtSeq As DataTable = objDB.SelectSQL(_SQLSEQ, cn)

            ClearExpanded()
            Dim _SQL As String = "INSERT INTO [dms].[dbo].[sqm_roki] ([name],[id],[parentDirId],[type],[expanded], [create_by], [start_date], [revision]) OUTPUT Inserted.seq VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1', 1, '" & Session("UserId") & "', '" & start_date & "', '" & revision & "')"
            Dim ReturnId As String = objDB.ExecuteSQLReturnId(_SQL, cn)
            Dim pathNoFile As String = "../Files/Doc/V2/SQM/ROKI/" & _DtSeq.Rows(0)("seq")
            Dim PathToDb As String = "../Files/Doc/V2/SQM/ROKI/" & _DtSeq.Rows(0)("seq") & "/" & ReturnId
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

            _SQL = "UPDATE [dms].[dbo].[sqm_roki] SET path = N'" & PathToDb & "', icon = '" & strIcon & "' WHERE seq = " & ReturnId
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteSqmRoki(ByVal idDel As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Try
                Dim dtSeq As DataTable = New DataTable
                Dim _SQL As String = "Select id, type, path, seq FROM [dms].[dbo].[sqm_roki] where id = '" & idDel & "'"
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
                            pathseqdel = "../Files/Doc/V2/SQM/ROKI/" & dtId.Rows(i)("seq")
                            If System.IO.Directory.Exists(Server.MapPath(pathseqdel)) Then
                                Directory.Delete(Server.MapPath(pathseqdel), True)
                            End If
                        End If
                    Next

                    _SQL = "SELECT seq, id, type, path FROM [dms].[dbo].[sqm_roki] where parentDirId in (" & id & ")"
                    dtId = objDB.SelectSQL(_SQL, cn)
                    _SQL = "DELETE [dms].[dbo].[sqm_roki] WHERE id in (" & id & ")"
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

#Region "SQM CUSTOMER"
        Function SQMCUSTOMER() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedSqmCustomer()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedSqmCustomer()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [dms].[dbo].[sqm_customer] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuSqmCustomer() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [dms].[dbo].[sqm_customer] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Public Function fnNewFolderSqmCustomer(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Try
                ClearExpandedSqmCustomer()
                Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                Dim _SQL As String = String.Empty
                _SQL = "INSERT INTO [dms].[dbo].[sqm_customer] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) OUTPUT Inserted.seq VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
                Dim ReTurnId As String = objDB.ExecuteSQLReturnId(_SQL, cn)
                dtStatus.Rows.Add("OK")
                objDB.DisconnectDB(cn)
            Catch ex As Exception
                dtStatus.Rows.Add("NOK")
            End Try

            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameSqmCustomer(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            ClearExpandedSqmCustomer()
            Dim _SQL As String = "Update [dms].[dbo].[sqm_customer] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Function fnChangeDateSqmCustomer(ByVal id As String, ByVal start_date As String, ByVal revision As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()

            ClearExpandedSqmCustomer()
            Dim _SQL As String = "UPDATE [dms].[dbo].[sqm_customer] SET start_date = '" & start_date & "', revision = '" & revision & "' WHERE id = '" & id & "'"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileSqmCustomer()
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

            'Edit by Thung 20190619
            Dim _SQLSEQ As String = "SELECT seq FROM [dms].[dbo].[sqm_customer] WHERE id = '" & id & "'"
            Dim _DtSeq As DataTable = objDB.SelectSQL(_SQLSEQ, cn)

            ClearExpandedSqmCustomer()
            Dim _SQL As String = "INSERT INTO [dms].[dbo].[sqm_customer] ([name],[id],[parentDirId],[type],[expanded], [create_by], [start_date], [revision]) OUTPUT Inserted.seq VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1', 1, '" & Session("UserId") & "', '" & start_date & "', '" & revision & "')"
            Dim ReturnId As String = objDB.ExecuteSQLReturnId(_SQL, cn)
            Dim pathNoFile As String = "../Files/Doc/V2/SQM/CUSTOMER/" & _DtSeq.Rows(0)("seq")
            Dim PathToDb As String = "../Files/Doc/V2/SQM/CUSTOMER/" & _DtSeq.Rows(0)("seq") & "/" & ReturnId
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

            _SQL = "UPDATE [dms].[dbo].[sqm_customer] SET path = N'" & PathToDb & "', icon = '" & strIcon & "' WHERE seq = " & ReturnId
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteSqmCustomer(ByVal idDel As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Try
                Dim dtSeq As DataTable = New DataTable
                Dim _SQL As String = "Select id, type, path, seq FROM [dms].[dbo].[sqm_customer] where id = '" & idDel & "'"
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
                            pathseqdel = "../Files/Doc/V2/SQM/CUSTOMER/" & dtId.Rows(i)("seq")
                            If System.IO.Directory.Exists(Server.MapPath(pathseqdel)) Then
                                Directory.Delete(Server.MapPath(pathseqdel), True)
                            End If
                        End If
                    Next

                    _SQL = "SELECT seq, id, type, path FROM [dms].[dbo].[sqm_customer] where parentDirId in (" & id & ")"
                    dtId = objDB.SelectSQL(_SQL, cn)
                    _SQL = "DELETE [dms].[dbo].[sqm_customer] WHERE id in (" & id & ")"
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

#Region "ISO"
        Function ISO() As ActionResult
            If Session("StatusLogin") = "OK" Then
                ClearExpandedISO()
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

        Private Sub ClearExpandedISO()
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "UPDATE [dms].[dbo].[iso] SET expanded = NULL WHERE parentDirId <> ''"
            objDB.ExecuteSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
        End Sub

        Public Function GetMenuISO() As String
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [dms].[dbo].[iso] order by type desc, seq asc"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)

            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson.Replace("""parentDirId"":"""",", "")
        End Function

        Public Function fnNewFolderISO(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Try
                ClearExpandedISO()
                Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
                cn.Open()
                Dim _SQL As String = String.Empty
                _SQL = "INSERT INTO [dms].[dbo].[iso] ([name],[id],[parentDirId],[type],[path],[icon],[expanded]) OUTPUT Inserted.seq VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & Id & "', '0','', '../img/fd.png', 1)"
                Dim ReTurnId As String = objDB.ExecuteSQLReturnId(_SQL, cn)
                dtStatus.Rows.Add("OK")
                objDB.DisconnectDB(cn)
            Catch ex As Exception
                dtStatus.Rows.Add("NOK")
            End Try

            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnRenameISO(ByVal Id As String, ByVal NewName As String) As String
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            ClearExpandedISO()
            Dim _SQL As String = "Update [dms].[dbo].[iso] SET name = N'" & NewName & "', expanded = 1 WHERE id = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnChangeDateISO(ByVal id As String, ByVal start_date As String, ByVal revision As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()

            ClearExpandedISO()
            Dim _SQL As String = "UPDATE [dms].[dbo].[iso] SET start_date = '" & start_date & "', revision = '" & revision & "' WHERE id = '" & id & "'"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnNewFileISO()
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

            'Edit by Thung 20190619
            Dim _SQLSEQ As String = "SELECT seq FROM [dms].[dbo].[iso] WHERE id = '" & id & "'"
            Dim _DtSeq As DataTable = objDB.SelectSQL(_SQLSEQ, cn)

            ClearExpandedISO()
            Dim _SQL As String = "INSERT INTO [dms].[dbo].[iso] ([name],[id],[parentDirId],[type],[expanded], [create_by], [start_date], [revision]) OUTPUT Inserted.seq VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '1', 1, '" & Session("UserId") & "', '" & start_date & "', '" & revision & "')"
            Dim ReturnId As String = objDB.ExecuteSQLReturnId(_SQL, cn)
            Dim pathNoFile As String = "../Files/Doc/V2/ISO/" & _DtSeq.Rows(0)("seq")
            Dim PathToDb As String = "../Files/Doc/V2/ISO/" & _DtSeq.Rows(0)("seq") & "/" & ReturnId
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

            _SQL = "UPDATE [dms].[dbo].[iso] SET path = N'" & PathToDb & "', icon = '" & strIcon & "' WHERE seq = " & ReturnId
            objDB.ExecuteSQL(_SQL, cn)

            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteISO(ByVal idDel As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Try
                Dim dtSeq As DataTable = New DataTable
                Dim _SQL As String = "Select id, type, path, seq FROM [dms].[dbo].[iso] where id = '" & idDel & "'"
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
                            pathseqdel = "../Files/Doc/V2/ISO/" & dtId.Rows(i)("seq")
                            If System.IO.Directory.Exists(Server.MapPath(pathseqdel)) Then
                                Directory.Delete(Server.MapPath(pathseqdel), True)
                            End If
                        End If
                    Next

                    _SQL = "SELECT seq, id, type, path FROM [dms].[dbo].[iso] where parentDirId in (" & id & ")"
                    dtId = objDB.SelectSQL(_SQL, cn)
                    _SQL = "DELETE [dms].[dbo].[iso] WHERE id in (" & id & ")"
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

    End Class
End Namespace