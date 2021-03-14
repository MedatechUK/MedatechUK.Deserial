Imports System.ComponentModel.Composition
Imports System.ComponentModel.Composition.Hosting
Imports System.IO
Imports System.Reflection
Imports System.Xml.Serialization
Imports Newtonsoft.Json
Imports MedatechUK.oData
Imports MedatechUK.Logging

Namespace Deserialiser

#Region "Enums"
    Public Enum eParser
        xml
        json

    End Enum

#End Region

#Region "Interfaces"

    Public Interface ILexor
        Function Deserialise(ByRef Strm As System.IO.StreamReader) As Object
        Sub Deserialise(ByRef Strm As System.IO.StreamReader, Environment As String)
        ReadOnly Property ConfigFile As FileInfo
        Property Config As lexdef
        Sub saveConfig()

    End Interface

    Public Interface ILexorProps
        ReadOnly Property LexName As String
        ReadOnly Property LexVers As String
        ReadOnly Property Parser As eParser
        ReadOnly Property SerialType As Type
        ReadOnly Property LoadType As String

    End Interface

#End Region

#Region "Lexor MEF Extensions"

    Public Class AppExtension
        Inherits logable
        Implements IDisposable

        Private _lexDef As Lexor = Nothing
        Public ReadOnly Property lexDef As Lexor
            Get
                Return _lexDef
            End Get
        End Property

        <ImportMany()>
        Public Property Lexors As IEnumerable(Of Lazy(Of ILexor, ILexorProps))
        Dim _container As CompositionContainer

        Sub New(ByRef logHandler As EventHandler)

            logHandlerDelegate = logHandler

            Dim catalog = New AggregateCatalog()
            catalog.Catalogs.Add(New DirectoryCatalog(New IO.FileInfo(Assembly.GetEntryAssembly.Location).Directory.FullName))
            catalog.Catalogs.Add(New AssemblyCatalog(Assembly.GetEntryAssembly))
            catalog.Catalogs.Add(New AssemblyCatalog(Assembly.GetExecutingAssembly))

            Dim _container As New CompositionContainer(catalog)
            _container.ComposeParts(Me)

            For Each l As Lazy(Of ILexor, ILexorProps) In Lexors
                With TryCast(l.Value, Lexor)
                    .SetMeta(l.Metadata, Me)
                    If l.Metadata.SerialType Is GetType(Deserialiser.lexdef) And _lexDef Is Nothing Then
                        _lexDef = l.Value

                    End If
                End With

            Next

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

#End Region

#Region "Lexor class"

    Public MustInherit Class Lexor
        Inherits logable
        Implements ILexor

#Region "Meta data"

        Private _props As ILexorProps
        Public ReadOnly Property Props As ILexorProps
            Get
                Return _props
            End Get

        End Property

        Private _myApp As AppExtension
        Public ReadOnly Property myApp As AppExtension
            Get
                Return _myApp
            End Get
        End Property

        Sub SetMeta(ByRef props As ILexorProps, ByRef Parent As AppExtension)
            _props = props
            _myApp = Parent
            logHandlerDelegate = Parent.logHandler

        End Sub

#End Region

#Region "Deserialiser"

        ''' <summary>
        ''' Uses the exported properties of the iLexor to deserialise.
        ''' This overrides the interface in the derived object.
        ''' iLexor Props:
        '''    LexName: Name of Lexor
        '''    LexVers: Version of Lexor
        '''    Parser: Parser type (JSON/XML)
        '''    SerialType: .net type containing data definition
        '''    LoadType: The string value indicating type of Loading
        ''' </summary>
        ''' <param name="Strm">A StreamReader contaning data to deserialise.</param>
        Overloads Function Deserialise(ByRef Strm As StreamReader) As Object Implements ILexor.Deserialise

            Using l As New Loading(_props.LoadType, logHandlerDelegate)
                Select Case _props.Parser
                    Case eParser.json
                        Return JsonConvert.DeserializeObject(Strm.ReadToEnd, _props.SerialType)

                    Case eParser.xml
                        Dim s As New XmlSerializer(_props.SerialType)
                        Return s.Deserialize(Strm)

                    Case Else
                        Throw New NotSupportedException

                End Select

            End Using

        End Function

        ''' <summary>
        ''' Deserialise to Priority with oData
        ''' </summary>
        ''' <param name="Strm">A stream containing data to deserialise</param>
        ''' <param name="Environment">The Priority Company</param>
        Overloads Sub Deserialise(ByRef Strm As StreamReader, Environment As String) Implements ILexor.Deserialise

            Using o As New objectParser(_props.SerialType, Me)
                If Not ConfigFile.Exists Then
                    Log("Assembly config file not found [{0}]", ConfigFile.FullName)
                    Me.Config = o.Config
                    Log("Generating config for assembly [{0}]", o.Config.assembly)
                    saveConfig()

                Else
                    Log("Config for assembly [{0}] found in [{1}]", o.Config.assembly, ConfigFile.FullName)
                    Using sr As New IO.StreamReader(ConfigFile.FullName)
                        Me.Config = _myApp.lexDef.Deserialise(sr)
                    End Using

                    If Not String.Compare(o.Config.version, Me.Config.version) = 0 Then
                        Log("Assembly version [{0}] != config version [{1}]",
                            o.Config.version.ToString,
                            Me.Config.version.ToString
                        )
                        Log("Updating config to version [{0}]",
                            o.Config.version.ToString
                        )
                        Me.Config = o.Config
                        saveConfig()

                    Else
                        Log("Assembly version [{0}] = config version [{1}]",
                            o.Config.version.ToString,
                            Me.Config.version.ToString
                        )
                        Log("Using .config file for form load definitions.")

                    End If
                End If

                Using l As New Loading(_props.LoadType, logHandlerDelegate)
                    Dim ob As Object
                    Select Case _props.Parser
                        Case eParser.json
                            Log("Deserialising JSON data with assembly [{0}] in environment [{1}].", o.Config.assembly, Environment)
                            ob = JsonConvert.DeserializeObject(Strm.ReadToEnd, _props.SerialType)

                        Case eParser.xml
                            Log("Deserialising XML data with assembly [{0}] in environment [{1}].", o.Config.assembly, Environment)
                            Dim s As New XmlSerializer(_props.SerialType)
                            ob = s.Deserialize(Strm)

                        Case Else
                            Throw New NotSupportedException

                    End Select

                    o.Parse(ob, l, o)

                    Dim ex As Exception = l.Post(Environment)
                    If Not ex Is Nothing Then

                    End If

                End Using

            End Using

        End Sub

#End Region

#Region "Config file"

        Private _Config As New lexdef
        Public Property Config As lexdef Implements ILexor.Config
            Get
                Return _Config
            End Get
            Set(value As lexdef)
                _Config = value
            End Set
        End Property

        Public Sub saveConfig() Implements ILexor.saveConfig

            Dim writer As XmlSerializer
            Try
                writer = New XmlSerializer(GetType(lexdef))
            Catch : End Try
            Using file As New System.IO.StreamWriter(ConfigFile.FullName)
                writer.Serialize(file, Me.Config)
            End Using

        End Sub

        Overloads ReadOnly Property ConfigFile As FileInfo Implements ILexor.ConfigFile
            Get
                With Reflection.Assembly.GetAssembly(Me.GetType)
                    Return New FileInfo(
                        Path.Combine(
                            New FileInfo(.Location).DirectoryName,
                            String.Format("{0}.config", Me.GetType.FullName)
                        )
                    )
                End With
            End Get

        End Property

#End Region

    End Class

#End Region

End Namespace