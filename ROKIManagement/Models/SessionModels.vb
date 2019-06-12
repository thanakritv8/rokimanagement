Public Class SessionModels
    Private _QMS As Integer
    Private _ISO As Integer
    Public Property QMS As Integer
        Get
            Return _QMS
        End Get
        Set(value As Integer)
            _QMS = value
        End Set
    End Property

    Public Property ISO As Integer
        Get
            Return _ISO
        End Get
        Set(value As Integer)
            _ISO = value
        End Set
    End Property
End Class
