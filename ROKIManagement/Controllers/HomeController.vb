﻿Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.Mvc
Imports System.Web.Script.Serialization

Namespace Controllers
    Public Class HomeController
        Inherits Controller

        Function Index() As ActionResult
            fnMove("O3M/61iUpsdNVTLmzWFiMt6hCbgPpOxZqs+XGQM4MuM=", "/e1m9jxhN4/mGl8cqC121Kden4SITXkekrtj1bfEmYE=")
            If Session("StatusLogin") = "OK" Then
                Return View()
            Else
                Return View("../Account/Login")
            End If
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
            Dim da As DataSetManagementTableAdapters.sp_GetDocTableAdapter = New DataSetManagementTableAdapters.sp_GetDocTableAdapter
            Dim dt As DataSetManagement.sp_GetDocDataTable = New DataSetManagement.sp_GetDocDataTable
            da.Fill(dt, Kind)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function GetDocWithSeq(ByVal Kind As String, ByVal Seq As Integer) As String
            Dim da As DataSetManagementTableAdapters.sp_GetDocWithSeqTableAdapter = New DataSetManagementTableAdapters.sp_GetDocWithSeqTableAdapter
            Dim dt As DataSetManagement.sp_GetDocWithSeqDataTable = New DataSetManagement.sp_GetDocWithSeqDataTable
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
                            Dim _SQL As String = "INSERT INTO [management].[dbo].[log] (logdetail) VALUES('" & path & "=>" & ex.Message & "')"
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
                    Dim cmd As SqlCommand = New SqlCommand("[management].[dbo].[sp_InsertDoc]", cn)
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
                    Dim _SQL As String = "DELETE [management].[dbo].[document] WHERE seq = " & seq
                    objDB.ExecuteSQL(_SQL, cn)
                ElseIf cbDelJP And Not cbDelTH Then

                    Dim _SQL As String = "SELECT * FROM [management].[dbo].[document] WHERE nameFileTH <> '' AND seq = " & seq
                    Dim dt As DataTable = objDB.SelectSQL(_SQL, cn)
                    If dt.Rows.Count > 0 Then
                        _SQL = "UPDATE [management].[dbo].[document] SET pathFileJP = '', nameFileJP = '', docNoJP = '' WHERE seq = " & seq
                        objDB.ExecuteSQL(_SQL, cn)
                    Else
                        _SQL = "DELETE [management].[dbo].[document] WHERE seq = " & seq
                        objDB.ExecuteSQL(_SQL, cn)
                    End If
                ElseIf Not cbDelJP And cbDelTH Then
                    Dim _SQL As String = "SELECT * FROM [management].[dbo].[document] WHERE nameFileJP <> '' AND seq = " & seq
                    Dim dt As DataTable = objDB.SelectSQL(_SQL, cn)
                    If dt.Rows.Count > 0 Then
                        _SQL = "UPDATE [management].[dbo].[document] SET pathFileTH = '', nameFileTH = '', docNoTH = '' WHERE seq = " & seq
                        objDB.ExecuteSQL(_SQL, cn)
                    Else
                        _SQL = "DELETE [management].[dbo].[document] WHERE seq = " & seq
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

            Dim da As DataSetManagementTableAdapters.sp_GetDocWithSeqTableAdapter = New DataSetManagementTableAdapters.sp_GetDocWithSeqTableAdapter
            Dim dt As DataSetManagement.sp_GetDocWithSeqDataTable = New DataSetManagement.sp_GetDocWithSeqDataTable
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
                                Dim _SQL As String = "SELECT * FROM [management].[dbo].[document] WHERE pathFileJP = '" & oldPathJP & "'"
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
                                Dim _SQL As String = "SELECT * FROM [management].[dbo].[document] WHERE pathFileTH = '" & oldPathTH & "'"
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

                    Dim cmd As SqlCommand = New SqlCommand("[management].[dbo].[sp_UpdateDoc]", cn)
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
        Function DocIatf() As ActionResult
            If Session("StatusLogin") = "OK" Then
                Return View()
            Else
                Return View("../Account/Login")
            End If

        End Function

        Public Function GetMenu() As String
            'Dim a = EncryptSHA256Managed("20180116103800")
            Dim dtMenu As DataTable = New DataTable
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim _SQL As String = "SELECT * FROM [management].[dbo].[iatf]"
            dtMenu = objDB.SelectSQL(_SQL, cn)
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtMenu.Rows Select dtMenu.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function RenameItem(ByVal path As String, ByVal name As String, ByVal newName As String, ByVal id As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")

            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            'Dim _SQLid As String = String.Empty

            'If path = String.Empty Then
            '    _SQLid = "SELECT id FROM [management].[dbo].[iatf] where name = N'" & name & "'"
            'Else
            '    Dim arr() As String = path.Split("/")
            '    Dim bracket As String = String.Empty
            '    _SQLid &= "select id from [management].[dbo].[iatf] where name = N'" & name & "' and parentDirId in ("
            '    For i As Integer = arr.Length - 1 To 0 Step -1
            '        If arr(i) <> String.Empty Then
            '            bracket &= ")"
            '            If i = 0 Then
            '                _SQLid &= "select id from [management].[dbo].[iatf] where name = N'Root'"
            '            Else
            '                _SQLid &= "select id from [management].[dbo].[iatf] where name = N'" & arr(i) & "' and parentDirId in ("
            '            End If
            '        End If
            '    Next
            '    _SQLid &= bracket
            'End If

            Dim PathServer As String = Server.MapPath("../Files/Doc/IATF/" & path & name)

            Try
                If Directory.Exists(PathServer) Then
                    FileIO.FileSystem.RenameDirectory(PathServer, newName)
                Else
                    If System.IO.File.Exists(PathServer) Then
                        FileIO.FileSystem.RenameFile(PathServer, newName)
                    End If
                End If

            Catch ex As Exception
                Dim a = 0
            End Try
            _SQL = "Update [management].[dbo].[iatf] SET name = N'" & newName & "' WHERE id = '" & id & "'"
            objDB.ExecuteSQL(_SQL, cn)
            Dim pathUpdate As String = "../Files/Doc/IATF/" & path & name
            Dim pathNew As String = "../Files/Doc/IATF/" & path & newName
            _SQL = "Update [management].[dbo].[iatf] SET path = REPLACE(path,N'" & pathUpdate & "', N'" & pathNew & "') WHERE path like N'" & pathUpdate & "%'"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")


            'If path = String.Empty Then
            '    _SQL = "SELECT seq FROM [management].[dbo].[iatf] where name = '" & name & "'"
            'Else
            '    Dim arr() As String = path.Split("/")
            '    _SQL = "SELECT seq FROM [management].[dbo].[iatf] where parentDirId in (SELECT id FROM [management].[dbo].[iatf] where name = '" & arr(arr.Length - 2) & "') and name = '" & name & "'"
            'End If
            'dtSeq = objDB.SelectSQL(_SQL, cn)
            'If dtSeq.Rows.Count > 0 Then
            '    _SQL = "Update [management].[dbo].[iatf] SET name = '" & newName & "' WHERE seq = " & dtSeq.Rows(0)("seq")
            '    objDB.ExecuteSQL(_SQL, cn)
            '    dtStatus.Rows.Add("OK")
            'Else
            '    dtStatus.Rows.Add("Error")
            'End If
            objDB.DisconnectDB(cn)



            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnAddFolder(ByVal Path As String, ByVal Name As String, ByVal NewName As String, ByVal id As String)
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim _SQLid As String = String.Empty

            Dim PathServer As String = Server.MapPath("../Files/Doc/IATF/" & Path & Name & "/" & NewName)
            If (Not System.IO.Directory.Exists(PathServer)) Then
                System.IO.Directory.CreateDirectory(PathServer)
            End If
            'If Path = String.Empty Then
            '    _SQLid = "SELECT id FROM [management].[dbo].[iatf] where name = N'" & Name & "'"
            'Else
            '    Dim arr() As String = Path.Split("/")
            '    Dim bracket As String = String.Empty
            '    _SQLid &= "select id from [management].[dbo].[iatf] where name = N'" & Name & "' and parentDirId in ("
            '    For i As Integer = arr.Length - 1 To 0 Step -1
            '        If arr(i) <> String.Empty Then
            '            bracket &= ")"
            '            If i = 0 Then
            '                _SQLid &= "select id from [management].[dbo].[iatf] where name = 'Root'"
            '            Else
            '                _SQLid &= "select id from [management].[dbo].[iatf] where name = N'" & arr(i) & "' and parentDirId in ("
            '            End If
            '        End If
            '    Next
            '    _SQLid &= bracket
            'End If
            _SQL = "INSERT INTO [management].[dbo].[iatf] ([name],[id],[parentDirId],[type],[path]) VALUES (N'" & NewName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', '" & id & "', '0','')"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnDeleteItem(ByVal Path As String, ByVal Name As String)

            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim dtSeq As DataTable = New DataTable
            Dim _SQL As String = String.Empty
            Dim _SQLid As String = String.Empty
            If Path = String.Empty Then
                _SQLid = "SELECT id FROM [management].[dbo].[iatf] where name = N'" & Name & "'"
            Else
                Dim arr() As String = Path.Split("/")
                Dim bracket As String = String.Empty
                _SQLid &= "select id from [management].[dbo].[iatf] where name = N'" & Name & "' and parentDirId in ("
                For i As Integer = arr.Length - 1 To 0 Step -1
                    If arr(i) <> String.Empty Then
                        bracket &= ")"
                        If i = 0 Then
                            _SQLid &= "select id from [management].[dbo].[iatf] where name = 'Root'"
                        Else
                            _SQLid &= "select id from [management].[dbo].[iatf] where name = N'" & arr(i) & "' and parentDirId in ("
                        End If
                    End If
                Next
                _SQLid &= bracket
            End If
            _SQL = "SELECT id FROM [management].[dbo].[iatf] where id in (" & _SQLid & ")"
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
                _SQL = "SELECT id FROM [management].[dbo].[iatf] where parentDirId in (" & id & ")"
                dtId = objDB.SelectSQL(_SQL, cn)
                _SQL = "DELETE [management].[dbo].[iatf] WHERE id in (" & id & ")"
                objDB.ExecuteSQL(_SQL, cn)
            End While
            Dim PathServer As String = Server.MapPath("../Files/Doc/IATF/" & Path & Name)
            If System.IO.File.Exists(PathServer) = True Then
                System.IO.File.Delete(PathServer)
            Else
                Directory.Delete(PathServer, True)
            End If
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function EncryptSHA256Managed(ByVal ClearString As String) As String
            Dim uEncode As New UnicodeEncoding()
            Dim bytClearString() As Byte = uEncode.GetBytes(ClearString)
            Dim sha As New _
            System.Security.Cryptography.SHA256Managed()
            Dim hash() As Byte = sha.ComputeHash(bytClearString)
            Return Convert.ToBase64String(hash)
        End Function

        Public Function fnAddFile()
            Dim dtStatus As DataTable = New DataTable
            dtStatus.Columns.Add("Status")
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim name As String = String.Empty
            Dim path As String = String.Empty
            Dim newName As String = String.Empty

            For i As Integer = 0 To Request.Form.AllKeys.Length - 1
                If Request.Form.AllKeys(i) = "name" Then
                    name = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "path" Then
                    path = Request.Form(i)
                ElseIf Request.Form.AllKeys(i) = "newName" Then
                    newName = Request.Form(i)
                End If
            Next
            Dim pathServer As String = String.Empty
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file = Request.Files(i)
                pathServer = Server.MapPath("../Files/Doc/IATF/" & path & name & "/") & newName
                file.SaveAs(pathServer)
                Dim a = 0
            Next
            Dim _SQL As String = String.Empty
            Dim _SQLid As String = String.Empty
            If path = String.Empty Then
                _SQLid = "SELECT id FROM [management].[dbo].[iatf] where name = N'" & name & "'"
            Else
                Dim arr() As String = path.Split("/")
                Dim bracket As String = String.Empty
                _SQLid &= "select id from [management].[dbo].[iatf] where name = N'" & name & "' and parentDirId in ("
                For i As Integer = arr.Length - 1 To 0 Step -1
                    If arr(i) <> String.Empty Then
                        bracket &= ")"
                        If i = 0 Then
                            _SQLid &= "select id from [management].[dbo].[iatf] where name = 'Root'"
                        Else
                            _SQLid &= "select id from [management].[dbo].[iatf] where name = N'" & arr(i) & "' and parentDirId in ("
                        End If
                    End If
                Next
                _SQLid &= bracket
            End If
            Dim pathfile As String = "../Files/Doc/IATF/" & path & name & "/" & newName
            _SQL = "INSERT INTO [management].[dbo].[iatf] ([name],[id],[parentDirId],[type],[path]) VALUES (N'" & newName & "', '" & EncryptSHA256Managed(Format(Now, "yyyyMMddHHmmss")) & "', (" & _SQLid & "), '1',N'" & pathfile & "')"
            objDB.ExecuteSQL(_SQL, cn)
            dtStatus.Rows.Add("OK")
            objDB.DisconnectDB(cn)
            Return New JavaScriptSerializer().Serialize(From dr As DataRow In dtStatus.Rows Select dtStatus.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        End Function

        Public Function fnMove(ByVal id As String, ByVal idTarget As String) As String

            Dim Path As String = GetPathFromId(id)
            Dim PathTarget As String = GetPathFromId(idTarget)
            Try
                MoveAll(Path, PathTarget)
            Catch ex As Exception
                Dim a = 0
            End Try

            Dim b = 0


        End Function

        Private Sub MoveAll(ByVal currentDir As String, ByVal targetDir As String)
            'currentDir = Directory.GetCurrentDirectory() ' or whatever you call current
            targetDir = Path.Combine(currentDir, targetDir)
            Directory.CreateDirectory(targetDir)

            Dim allDirs = From dir In Directory.EnumerateDirectories(currentDir, "*.*", SearchOption.AllDirectories)
                          Where Not Path.GetFileName(dir).Equals("My Folder", StringComparison.InvariantCultureIgnoreCase)
            For Each dir As String In allDirs
                Try
                    Dim targetPath As String = Path.Combine(targetDir, Path.GetFileName(dir))
                    Directory.Move(dir, targetPath)
                Catch ex As Exception
                    ' log or whatever, here just ignore and continue ...
                End Try
            Next

            'now move the rest of the files in the troot folder
            Dim filesInRoot = From file In Directory.EnumerateFiles(currentDir, "*.*", SearchOption.TopDirectoryOnly)
            For Each file As String In filesInRoot
                Try
                    Dim targetPath As String = Path.Combine(targetDir, Path.GetFileName(file))
                    IO.File.Move(file, targetPath)
                Catch ex As Exception
                    ' log or whatever, here just ignore and continue ...
                End Try
            Next
        End Sub


        Private Function GetPathFromId(ByVal Id As String) As String
            Dim cn As SqlConnection = objDB.ConnectDB(My.Settings.IPServer, My.Settings.User, My.Settings.Pass)
            cn.Open()
            Dim path As String = String.Empty
            Dim _SQL As String = "SELECT parentDirId, name FROM [management].[dbo].[iatf] WHERE id = '" & Id & "'"
            Dim dtPrd As DataTable = objDB.SelectSQL(_SQL, cn)
            While dtPrd.Rows.Count > 0
                path = dtPrd.Rows(0)("name") & "\" & path
                _SQL = "SELECT parentDirId, name FROM [management].[dbo].[iatf] WHERE id = '" & dtPrd.Rows(0)("parentDirId") & "'"
                dtPrd = objDB.SelectSQL(_SQL, cn)
            End While
            path = Server.MapPath("../Files/Doc/IATF/" & Mid(path, 1, path.Length - 1))
            Return path
        End Function
#End Region

    End Class
End Namespace