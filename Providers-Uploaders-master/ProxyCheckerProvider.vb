Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Http
Imports System.Threading
Imports System.Threading.Tasks
 
Public Class ProxyCheckerProvider
    Implements IDisposable
    'Initialize Client
    Private ReadOnly _client As New HttpClient
 
    'SetBase URI
    Public Sub New()
        _client.BaseAddress = New Uri("http://proxyipchecker.com")
    End Sub
    Public Async Function ProxyCheck(Ip As String, Port As Integer, token As CancellationToken) As Task(Of String)
        'Check Input Data
        If String.IsNullOrWhiteSpace(Port) Then
            '- If there is no Port set Port to 80
            Port = 80
        End If
        If String.IsNullOrWhiteSpace(Ip) Then
            '- Throw Error if there is no Ip set.
            Throw New ArgumentNullException(NameOf(Ip))
        End If
 
        'SetForm Data--------------------------------------->'Link------------------------------------------------------------------------------------------->'Input; Input Data into Form Data
        Dim request = New HttpRequestMessage(HttpMethod.Post, "/ws.php") With {.Content = New FormUrlEncodedContent(New Dictionary(Of String, String)() From {{"proxycheck", Ip}, {"port", Port}})}
        'Send Request
        Dim response = Await _client.SendAsync(request, token)
        'Read Request
        Dim content As String = Await response.Content.ReadAsStringAsync
        'Sort Request
        Dim source As String() = content.Split(";")
        Dim responsetime As String = source(2)
        Dim score As String = source(3)
 
        'Measure Proxy
        If score = "0" Then
            'If it gets 0 its most likely dead
            score = "Bad Proxy"
        ElseIf score >= 1000 Then
            'If its over 1000 then it's usually a Proxy Server
            score = "Fast Proxy"
        ElseIf score < 1000 Then
            '200-999 is usually average.
            score = "Average Proxy"
        End If
 
        'Return Results
        Return score & "; Response Time: " & responsetime
    End Function
 
    'Dispose when Done
    Public Sub Dispose() Implements IDisposable.Dispose
        _client.Dispose()
    End Sub
 
End Class
