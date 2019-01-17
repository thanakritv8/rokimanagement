Public Class FileTreeViewModel
    Private _Name As String
    Public Property Name() As String
        Set(value As String)
            _Name = value
        End Set
        Get
            Return _Name
        End Get
    End Property

    Private _Ext As String
    Public Property Ext() As String
        Get
            Return _Ext
        End Get
        Set(value As String)
            _Ext = value
        End Set
    End Property

    Private _Path As String
    Public Property Path() As String
        Get
            Return _Path
        End Get
        Set(value As String)
            _Path = value
        End Set
    End Property

    Private _IsDirectory As Boolean
    Public Property IsDirectory() As Boolean
        Get
            Return _IsDirectory
        End Get
        Set(value As Boolean)
            _IsDirectory = value
        End Set
    End Property

    Public Function PathAltSeparator() As String
        Return Path.Replace("\\", "/")
    End Function

End Class

