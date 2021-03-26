Imports System.IO

Namespace Deserialiser

    Public Class ArchiveFile
        Implements IDisposable

        Private _fn As FileInfo
        Private _initDir As DirectoryInfo
        Private _fName As String
        Private _ext As String

        Public badmail As Boolean = False

        Sub New(fn As FileInfo)
            _fn = fn

        End Sub

        Private ReadOnly Property Result As String
            Get
                Select Case badmail
                    Case True
                        Return "badmail"
                    Case Else
                        Return "save"

                End Select
            End Get
        End Property

        Private ReadOnly Property ArchiveDir As DirectoryInfo
            Get
                Return New DirectoryInfo(
                Path.Combine(
                    Path.Combine(
                        New DirectoryInfo(_fn.Directory.FullName).FullName,
                        Result
                    ),
                    Now.ToString("yyyy-MM")
                )
            )
            End Get
        End Property

        Private _vers As Integer = 0
        Private ReadOnly Property Version As String
            Get
                Dim ret As String = ""
                Select Case _vers
                    Case 0
                    Case Else
                        ret = _vers.ToString
                End Select
                _vers += 1
                Return ret
            End Get
        End Property

#Region "IDisposable Support"

        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    If _fn.Exists Then
                        With ArchiveDir
                            If Not .Exists Then .Create()
                            Dim test As New FileInfo(Path.Combine(.FullName, String.Format("{0}{1}{2}", Replace(_fn.Name, _fn.Extension, ""), Version, _fn.Extension)))
                            While test.Exists
                                test = New FileInfo(Path.Combine(.FullName, String.Format("{0}{1}{2}", Replace(_fn.Name, _fn.Extension, ""), Version, _fn.Extension)))
                            End While
                            File.Move(_fn.FullName, test.FullName)

                        End With
                    End If
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub

#End Region

    End Class

End Namespace