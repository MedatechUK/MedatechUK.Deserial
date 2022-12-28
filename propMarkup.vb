Namespace Deserialiser

    Public Enum oDataType
        int
        str
    End Enum

    <System.AttributeUsage(System.AttributeTargets.Property)>
    Public Class [Property]
        Inherits System.Attribute

        Public Ignore As Boolean = False

        Public oDataField As String

        Public oDataType As oDataType

    End Class

    <System.AttributeUsage(System.AttributeTargets.Class)>
    Public Class [Class]
        Inherits System.Attribute

        Public EnumerateOnly As Boolean = False

        Public oDataForm As String

    End Class

End Namespace
