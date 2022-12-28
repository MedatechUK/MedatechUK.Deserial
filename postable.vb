Imports System.IO
Imports System.Net
Imports Newtonsoft.Json

Namespace Deserialiser

    Public Class postable
        Inherits MedatechUK.Logging.Logable
        Implements IDisposable

        Private _Request As Net.HttpWebRequest

        Sub New(Method As Http.HttpMethod, uri As UriBuilder, Credentials As NetworkCredential)

            Try

                _Request = CType(Net.HttpWebRequest.Create(uri.Uri), Net.HttpWebRequest)
                With _Request
                    .Method = Method.Method
                    .Proxy = Nothing
                    .UserAgent = "MedatechUK"
                    .ContentType = "application/json"
                    .Credentials = Credentials

                End With

            Catch ex As Exception
                Throw New Exception(
                    String.Format(
                        "{0}",
                        ex.Message
                    )
                )

            End Try

        End Sub

        Public Function GetResponse(ByRef o As Object) As Object

            Dim myEncoder As New System.Text.ASCIIEncoding
            Dim Request As New MemoryStream(myEncoder.GetBytes(JsonConvert.SerializeObject(o, Formatting.Indented)))

            Dim e As Object
            Dim buffer(1024) As Byte
            Dim bytesRead As Integer

            System.Net.ServicePointManager.ServerCertificateValidationCallback =
              Function(se As Object,
              cert As System.Security.Cryptography.X509Certificates.X509Certificate,
              chain As System.Security.Cryptography.X509Certificates.X509Chain,
              sslerror As System.Net.Security.SslPolicyErrors) True

            With _Request
                Logging.Log(Me, "{0} {1}", .Method.ToUpper, .RequestUri.ToString)
                Logging.Log(Me, "{0}", JsonConvert.SerializeObject(o, Formatting.Indented))

                Try
                    If Not Request Is Nothing Then
                        .ContentLength = Request.Length
                        Using requestStream As Stream = .GetRequestStream()
                            With requestStream
                                While True
                                    bytesRead = Request.Read(buffer, 0, buffer.Length)
                                    If bytesRead = 0 Then
                                        Exit While

                                    End If
                                    .Write(buffer, 0, bytesRead)

                                End While

                            End With

                        End Using

                    End If

                    e = .GetResponse()

                Catch ex As WebException
                    e = ex

                Catch ex As Exception
                    e = ex

                End Try

            End With

            Return e

        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
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