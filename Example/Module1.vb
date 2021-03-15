Imports MedatechUK.Deserialiser
Imports MedatechUK.Logging

Module Module1

    Sub Main()

        ' Set the log output location
        LogLocation = eLogLocation.GetEntryAssemblyDirectory

        Using ex As New AppExtension(AddressOf Events.logHandler)
            For Each l As Lazy(Of ILexor, ILexorProps) In ex.Lexors
                If l.Metadata.SerialType Is GetType(Ashridge.Order) Then
                    l.Value.Deserialise(New IO.StreamReader("M:\MedatechEDI\RunBat\lextest\lextest\create_order.json"), "test")

                End If

            Next
        End Using

    End Sub

End Module
