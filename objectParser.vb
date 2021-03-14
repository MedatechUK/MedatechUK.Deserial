Imports System.Reflection
Imports MedatechUK.oData
Imports MedatechUK.Logging

Namespace Deserialiser

    ''' <summary>
    ''' Parse an object for loading by recursing into LoadLine objects
    ''' </summary>
    Friend Class objectParser
        Inherits LoadLine
        Implements IDisposable

        Sub New(ByRef BusinessObject As Type, Lexor As Lexor)
            MyBase.New(BusinessObject)

            Me.logHandler = Lexor.logHandler

            Config = New lexdef
            With Config
                .name = Lexor.Props.LexName
                .version = Lexor.Props.LexVers
                .assembly = BusinessObject.FullName

                Log("Parsing assembly [{0}] ...", BusinessObject.FullName)
                For Each ty As String In myRecordTypes.Keys
                    Log(
                        "Found RecordType[{0}] in assembly {1}",
                        TryCast(myRecordTypes(ty), LoadLine).RecordType.ToString,
                        myRecordTypes(ty).myType.FullName
                    )
                    .recordtypes.Add(
                        New lexdefRecord(
                            TryCast(myRecordTypes(ty), LoadLine).RecordType,
                            ty,
                            myRecordTypes(ty).myType.FullName
                        )
                    )
                Next

                For Each ty As Type In myTypes.Keys
                    .assemblies.Add(New lexdefAssembly(ty.FullName))
                    For Each key As String In myTypes(ty).Keys
                        .assemblies.Last.property.Add(New lexdefAssemblyProperty(ePropertyType.field, key, myTypes(ty)(key)))
                    Next
                    For Each ch As String In myTypes(ty).Child.Keys
                        .assemblies.Last.property.Add(New lexdefAssemblyProperty(ePropertyType.type, ch, myTypes(ty).Child(ch).myType.FullName))
                    Next
                    For Each en As String In myTypes(ty).Enumerable.Keys
                        .assemblies.Last.property.Add(New lexdefAssemblyProperty(ePropertyType.enumeration, en, myTypes(ty).Enumerable(en).myType.FullName))
                    Next
                Next

            End With

        End Sub

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

    ''' <summary>
    ''' An object that holds the definition of a row
    ''' </summary>
    Friend Class LoadLine
        Inherits LogableDictionary

#Region "Types in Business object"

#Region "Parent only properties"

        ''' <summary>
        ''' Parent Type.
        ''' Only called from the parent object
        ''' </summary>
        Private _myRecordTypes As New Dictionary(Of String, LoadLine)
        Public Property myRecordTypes As Dictionary(Of String, LoadLine)
            Get
                Return _myRecordTypes
            End Get
            Set(value As Dictionary(Of String, LoadLine))
                _myRecordTypes = value
            End Set
        End Property

        ''' <summary>
        ''' Parent Type.
        ''' Only called from the parent object
        ''' </summary>
        Private _myTypes As New Dictionary(Of Type, LoadLine)
        Public Property myTypes As Dictionary(Of Type, LoadLine)
            Get
                Return _myTypes
            End Get
            Set(value As Dictionary(Of Type, LoadLine))
                _myTypes = value
            End Set
        End Property

#End Region

#Region "Local Properties"

        Private _recordtype As Integer = 0
        Public ReadOnly Property RecordType As Integer
            Get
                Return _recordtype
            End Get
        End Property

        Private _myType As Type
        ''' <summary>
        ''' Local Type
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property myType As Type
            Get
                Return _myType
            End Get
        End Property

        Private _Config As New lexdef
        Public Property Config As lexdef
            Get
                Return _Config
            End Get
            Set(value As lexdef)
                _Config = value
            End Set
        End Property

#Region "Type counters"

        Private _textnum As Integer = 0
        Public ReadOnly Property TextNum As Integer
            Get
                _textnum += 1
                Return _textnum
            End Get
        End Property

        Private _intnum As Integer = 0
        Public ReadOnly Property intNum As Integer
            Get
                _intnum += 1
                Return _intnum
            End Get
        End Property

        Private _datenum As Integer = 0
        Public ReadOnly Property dateNum As Integer
            Get
                _datenum += 1
                Return _datenum
            End Get
        End Property

#End Region

#Region "Child / Enumerable Dictionary"

        Private _child As New Dictionary(Of String, LoadLine)
        Public Property Child As Dictionary(Of String, LoadLine)
            Get
                Return _child
            End Get
            Set(value As Dictionary(Of String, LoadLine))
                _child = value
            End Set
        End Property

        Private _enumerable As New Dictionary(Of String, LoadLine)
        Public Property Enumerable As Dictionary(Of String, LoadLine)
            Get
                Return _enumerable
            End Get
            Set(value As Dictionary(Of String, LoadLine))
                _enumerable = value
            End Set
        End Property

#End Region

#End Region

#End Region

#Region "Recursive CTOR / Parse"

        Sub New(ByRef t As Type, Optional Parent As LoadLine = Nothing)

            _myType = t
            If Parent Is Nothing Then
                _recordtype = 1
                myRecordTypes.Add(t.Name, Me)
                Parent = Me
            Else
                _recordtype = Parent.myRecordTypes.Count + 1
            End If

            If Not Parent.myTypes.Keys.Contains(t) Then
                Parent.myTypes.Add(t, Me)
            End If

            For Each prop As PropertyInfo In t.GetProperties
                Select Case prop.PropertyType.Name.ToLower
                    Case "string"
                        Me.Add(prop.Name, String.Format("TEXT{0}", TextNum.ToString))

                    Case "integer"
                        Me.Add(prop.Name, String.Format("INT{0}", intNum.ToString))

                    Case "date"
                        Me.Add(prop.Name, String.Format("DATE{0}", dateNum.ToString))

                    Case Else
                        If InheritsOrImplements(prop.PropertyType, GetType(IEnumerable)) Then
                            If Not Enumerable.Keys.Contains(prop.Name) Then
                                Enumerable.Add(prop.Name, New LoadLine(prop.PropertyType.GetGenericArguments().FirstOrDefault, Parent))
                                Parent.myRecordTypes.Add(prop.Name, Enumerable(prop.Name))

                            End If

                        Else
                            If Not Child.Keys.Contains(prop.Name) Then
                                Child.Add(prop.Name, New LoadLine(prop.PropertyType, Parent))
                                Parent.myRecordTypes.Add(prop.Name, Child(prop.Name))

                            End If

                        End If

                End Select

            Next

        End Sub

        Sub Parse(ByRef ob As Object, ByRef load As Loading, ByRef Parent As LoadLine)

            Dim thistype As String = ob.GetType().FullName
            With load.AddRow(Parent.Config.RecordTypeByAssembly(thistype).type)
                Dim x As lexdefAssembly = Parent.Config.assemblyByName(thistype)
                For Each prop As PropertyInfo In ob.GetType.GetProperties
                    If x.propertyByName(prop.Name).PropType = ePropertyType.field Then
                        .setProperty(
                            GetPropertyValue(ob, x.propertyByName(prop.Name).name),
                            x.propertyByName(prop.Name).destname
                        )
                    End If
                Next

                For Each prop As PropertyInfo In ob.GetType.GetProperties
                    If x.propertyByName(prop.Name).PropType = ePropertyType.type Then
                        Parse(GetPropertyValue(ob, x.propertyByName(prop.Name).name), load, Parent)
                    End If
                Next

                For Each prop As PropertyInfo In ob.GetType.GetProperties
                    If x.propertyByName(prop.Name).PropType = ePropertyType.enumeration Then
                        For Each o As Object In GetPropertyValue(ob, x.propertyByName(prop.Name).name)
                            Parse(o, load, Parent)
                        Next

                    End If
                Next

            End With

        End Sub

#End Region

#Region "Reflection subs"

        Public Function GetPropertyValue(ByVal obj As Object, ByVal PropName As String) As Object
            Dim objType As Type = obj.GetType()
            Dim pInfo As System.Reflection.PropertyInfo = objType.GetProperty(PropName)
            Dim PropValue As Object = pInfo.GetValue(obj, Reflection.BindingFlags.GetProperty, Nothing, Nothing, Nothing)
            Return PropValue

        End Function

#Region "InheritsOrImplements"

        Private Function InheritsOrImplements(child As Type, parent As Type) As Boolean
            If child Is Nothing OrElse parent Is Nothing Then
                Return False
            End If

            parent = resolveGenericTypeDefinition(parent)

            If parent.IsAssignableFrom(child) Then
                Return True
            End If

            Dim currentChild = If(child.IsGenericType, child.GetGenericTypeDefinition(), child)

            While currentChild <> GetType(Object)
                If parent = currentChild OrElse hasAnyInterfaces(parent, currentChild) Then
                    Return True
                End If

                currentChild = If(currentChild.BaseType IsNot Nothing AndAlso currentChild.BaseType.IsGenericType, currentChild.BaseType.GetGenericTypeDefinition(), currentChild.BaseType)

                If currentChild Is Nothing Then
                    Return False
                End If
            End While

            Return False
        End Function

        Function hasAnyInterfaces(parent As Type, child As Type) As Boolean
            Return child.GetInterfaces().Any(Function(childInterface)
                                                 Dim currentInterface = If(childInterface.IsGenericType, childInterface.GetGenericTypeDefinition(), childInterface)

                                                 Return currentInterface = parent

                                             End Function)

        End Function

        Function resolveGenericTypeDefinition(type As Type) As Type
            Dim shouldUseGenericType = Not (type.IsGenericType AndAlso type.GetGenericTypeDefinition() <> type)
            If type.IsGenericType AndAlso shouldUseGenericType Then
                type = type.GetGenericTypeDefinition()
            End If
            Return type
        End Function

    End Class

#End Region

#End Region

End Namespace

