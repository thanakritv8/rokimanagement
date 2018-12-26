Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.Mvc
Imports System.Web.Script.Serialization

Namespace Controllers
    Public Class HomeController
        Inherits Controller

        Function Index() As ActionResult
            If Session("StatusLogin") = "OK" Then
                Return View()
            Else
                Return View("../Account/Login")
            End If
        End Function

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

                For i As Integer = 0 To Request.Files.Count - 1
                    Dim file = Request.Files(i)
                    Dim path As String = String.Empty
                    Try
                        Dim p As String = file.FileName
                        Dim extension As String = System.IO.Path.GetExtension(p)
                        If i = 0 And nameJP <> String.Empty Then
                            path = Server.MapPath("../Files/Doc/QMS/JP/") & nameJP & extension
                        Else
                            path = Server.MapPath("../Files/Doc/QMS/TH/") & nameTH & extension
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
                        pathJP = "../Files/Doc/QMS/JP/" & nameJP & ".pdf"
                    End If
                    If nameTH <> String.Empty Then
                        pathTH = "../Files/Doc/QMS/TH/" & nameTH & ".pdf"
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
                    If Request.Files.AllKeys(0) = "pathJP" Then
                        newPathJP = Server.MapPath("~/Files/Doc/QMS/JP/") & nameJP & typeFileJP
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
                    newPathTH = Server.MapPath("../Files/Doc/QMS/TH/") & nameTH & typeFileTH
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
    End Class
End Namespace