Imports System.ComponentModel.Composition
Imports System.ComponentModel.Composition.Hosting
Imports System.IO
Imports System.Reflection
Imports System.Xml.Serialization
Imports Newtonsoft.Json
Imports MedatechUK.oData
Imports MedatechUK.Logging
Imports System.Web
Imports System.Net
Imports System.Xml
Imports System.Web.Configuration

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
        Sub Deserialise(ByRef ob As Object, Environment As String)
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
        Inherits Logable
        Implements IDisposable

        Private _lexDef As Lexor = Nothing
        Public ReadOnly Property lexDef As Lexor
            Get
                Return _lexDef
            End Get
        End Property

        Private _LoadedAssemblies As New List(Of String)
        Public ReadOnly Property LoadedAssemblies As List(Of String)
            Get
                Return _LoadedAssemblies
            End Get
        End Property

        <ImportMany()>
        Public Property Lexors As IEnumerable(Of Lazy(Of ILexor, ILexorProps))
        Dim _container As CompositionContainer

#Region "CTOR"

        Sub New(ByRef logHandler As EventHandler)

            Dim catalog = New AggregateCatalog()
            logHandlerDelegate = logHandler

            If HttpContext.Current Is Nothing Then
                Try
                    catalog.Catalogs.Add(New DirectoryCatalog(New IO.FileInfo(Assembly.GetEntryAssembly.Location).Directory.FullName))

                Catch ex As Exception
                    Log("Checking for lexor extentions in [{0}].", New IO.FileInfo(Assembly.GetEntryAssembly.Location).Directory.FullName)
                    Log(ex)

                End Try

                Try
                    catalog.Catalogs.Add(New AssemblyCatalog(Assembly.GetEntryAssembly))

                Catch ex As Exception
                    Log("Checking for lexors in assembly [{0}].", Assembly.GetEntryAssembly.FullName)
                    Log(ex)

                End Try

                If Not Assembly.GetExecutingAssembly = Assembly.GetEntryAssembly Then
                    Try
                        catalog.Catalogs.Add(New AssemblyCatalog(Assembly.GetExecutingAssembly))

                    Catch ex As Exception
                        Log("Checking for lexors in assembly [{0}].", Assembly.GetExecutingAssembly.FullName)
                        Log(ex)

                    End Try
                End If

            Else ' we're a web service

                Try
                    catalog.Catalogs.Add(New DirectoryCatalog(Path.Combine(HttpContext.Current.Server.MapPath(virtualDir), "bin")))

                Catch ex As Exception
                    Log("Checking for lexor extentions in [{0}].", Path.Combine(HttpContext.Current.Server.MapPath(virtualDir), "bin"))
                    Log(ex)

                End Try

            End If

            Try
                _container = New CompositionContainer(catalog)
                _container.ComposeParts(Me)

            Catch ex As ReflectionTypeLoadException
                Log(ex)
                For Each e In ex.Types
                    If e Is Nothing Then
                        Log("Missing type.")
                    Else
                        Log("{0}", e.FullName)
                    End If

                Next

            Catch ex As Exception
                Log(ex)

            End Try

            For Each l As Lazy(Of ILexor, ILexorProps) In Lexors
                With TryCast(l.Value, Lexor)
                    If Not _LoadedAssemblies.Contains(l.Metadata.SerialType.FullName) Then
                        _LoadedAssemblies.Add(l.Metadata.SerialType.FullName)
                        ' Log("Found Lexor [{0}].", l.Metadata.SerialType.FullName)
                    End If
                    .SetMeta(l.Metadata, Me)
                    If l.Metadata.SerialType Is GetType(Deserialiser.lexdef) And _lexDef Is Nothing Then
                        _lexDef = l.Value

                    End If

                End With

            Next

        End Sub

        Private ReadOnly Property virtualDir As String
            Get
                If WebConfigurationManager.AppSettings("virtualdir") Is Nothing Then
                    Return "/api/"
                Else
                    Return String.Format("/{0}/", WebConfigurationManager.AppSettings("virtualdir"))
                End If
            End Get
        End Property

#End Region

#Region "Lexor by reference functions"

        Public Function LexByName(name As String) As Lexor
            For Each l As Lazy(Of ILexor, ILexorProps) In Lexors
                With TryCast(l.Value, Lexor)
                    If String.Compare(l.Metadata.LexName, name) = 0 Then
                        Return l.Value

                    End If
                End With
            Next
            Return Nothing
        End Function

        Public Function LexByAssemblyName(AssemblyFullname As String) As Lexor
            For Each l As Lazy(Of ILexor, ILexorProps) In Lexors
                With TryCast(l.Value, Lexor)
                    If String.Compare(l.Metadata.SerialType.FullName, AssemblyFullname) = 0 Then
                        Return l.Value

                    End If
                End With
            Next
            Return Nothing
        End Function

        Public Function LexByType(AssemblyType As Type) As Lexor
            For Each l As Lazy(Of ILexor, ILexorProps) In Lexors
                With TryCast(l.Value, Lexor)
                    If l.Metadata.SerialType Is AssemblyType Then
                        Return l.Value

                    End If
                End With
            Next
            Return Nothing
        End Function

#End Region

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
        Inherits Logable
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

#Region "Factory"

        Private _factory As Object = Nothing
        Function CreateType() As Object
            _factory = Activator.CreateInstance(_props.SerialType)
            Return _factory
        End Function

        Function CreateType(xml As XmlReader) As Object
            Dim s As XmlSerializer = New XmlSerializer(Props.SerialType)
            _factory = s.Deserialize(xml)
            Return _factory

        End Function

        Private _Request As Net.HttpWebRequest
        Public Function Serialise(Method As Http.HttpMethod, uri As UriBuilder, Credentials As NetworkCredential, Optional proxy As IWebProxy = Nothing) As Object

            Try
                _Request = CType(Net.HttpWebRequest.Create(uri.Uri), Net.HttpWebRequest)
                With _Request
                    .Method = Method.Method
                    .Proxy = proxy
                    .UserAgent = "MedatechUK"
                    Select Case _props.Parser
                        Case eParser.json
                            .ContentType = "application/json"
                            .Accept = "application/json"

                        Case Else
                            .ContentType = "application/xml"
                            .Accept = "application/xml"

                    End Select

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

            Dim myEncoder As New System.Text.ASCIIEncoding
            Dim Request As MemoryStream
            Select Case _props.Parser
                Case eParser.json
                    Request = New MemoryStream(myEncoder.GetBytes(JsonConvert.SerializeObject(_factory, Newtonsoft.Json.Formatting.Indented)))

                Case Else
                    Dim sw1 = New StringWriter()
                    Dim xs1 As New XmlSerializer(_props.SerialType)
                    xs1.Serialize(New XmlTextWriter(sw1), _factory)

                    Request = New MemoryStream(myEncoder.GetBytes(sw1.ToString()))

            End Select

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
                Select Case _props.Parser
                    Case eParser.json
                        Logging.Log(JsonConvert.SerializeObject(_factory, Newtonsoft.Json.Formatting.Indented))

                    Case Else
                        Dim sw1 = New StringWriter()
                        Dim xs1 As New XmlSerializer(_props.SerialType)
                        xs1.Serialize(New XmlTextWriter(sw1), _factory)
                        Logging.Log(sw1.ToString())

                End Select

                Try
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

                    e = .GetResponse()

                Catch ex As WebException
                    e = ex

                Catch ex As Exception
                    e = ex

                End Try

            End With

            Return e

        End Function

#End Region

#Region "Deserialiser"

        Overloads Sub Deserialise(ByRef ob As Object, Environment As String) Implements ILexor.Deserialise

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
                        o.Config = Me.Config

                    End If
                End If

                Using l As New Loading(_props.LoadType, Me.logHandler)
                    o.Parse(ob, l, o)

                    ' added this so that a blank environment will simply 
                    ' generate the lex config, without posting to oData
                    If Len(Environment) > 0 Then
                        Dim ex As Exception = l.Post(Environment)
                        If Not ex Is Nothing Then
                            Throw ex

                        End If
                    End If

                End Using

            End Using

        End Sub

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

            Select Case _props.Parser
                Case eParser.json
                    Log("Deserialising JSON data to assembly [{0}].", _props.SerialType.FullName)
                    Return JsonConvert.DeserializeObject(Strm.ReadToEnd, _props.SerialType)

                Case eParser.xml
                    Log("Deserialising XML data to assembly [{0}].", _props.SerialType.FullName)
                    Dim s As New XmlSerializer(_props.SerialType)
                    Return s.Deserialize(Strm)

                Case Else
                    Throw New NotSupportedException

            End Select

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
                        o.Config = Me.Config

                    End If
                End If

                Using l As New Loading(_props.LoadType, Me.logHandler)
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

                    ' added this so that a blank environment will simply 
                    ' generate the lex config, without posting to oData
                    If Len(Environment) > 0 Then
                        Dim ex As Exception = l.Post(Environment)
                        If Not ex Is Nothing Then
                            Throw ex

                        End If
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
            Using f As New System.IO.StreamWriter(ConfigFile.FullName)
                writer.Serialize(f, Me.Config)
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