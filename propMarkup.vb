Namespace Deserialiser

    <System.AttributeUsage(System.AttributeTargets.Property)>
    Public Class [Property]
        Inherits System.Attribute

        Public Ignore As Boolean = False

    End Class

    <System.AttributeUsage(System.AttributeTargets.Class)>
    Public Class [Class]
        Inherits System.Attribute

        Public EnumerateOnly As Boolean = False

    End Class

End Namespace
