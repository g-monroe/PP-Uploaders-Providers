Imports System.Net.Http
Imports System.Text.RegularExpressions
Public Class PomfProvider
    Implements IDisposable
 
    Private _client As HttpClient
 
    Sub New()
        _client = New HttpClient()
    End Sub
 
    Public Async Function UploadBytes(filePath As String) As Task(Of String)
        Try
            Dim response As String = Await Upload(filepath, "https://pomf.cat/upload.php")
 
            Return "http://a.pomf.cat/" & Regex.Match(response, """url"":""([^""]+)""").Groups(1).Value
        Catch
            Return "Error!"
        End Try
    End Function
    Public Async Function UploadFile(filePath As String) As Task(Of String)
        Try
            Dim response As String = Await Upload(filePath, "https://pomf.is/upload.php")
 
            Return "http://u.pomf.is/" & Regex.Match(response, "u.pomf.is\\\/([^""]+)").Groups(1).Value
        Catch
            Return "Error!"
        End Try
    End Function
 
    Private Async Function Upload(filePath As String, uri As String) As Task(Of String)
        Dim form As New MultipartFormDataContent()
 
        Dim fBytes As Byte() = File.ReadAllBytes(filePath)
        Dim fName As String = Path.GetFileName(filePath)
 
        form.Add(New ByteArrayContent(fBytes, 0, fBytes.Count()), "files[]", fName)
 
        Dim response = Await _client.PostAsync(uri, form)
        Return Await response.Content.ReadAsStringAsync()
    End Function
 
    Public Sub Dispose() Implements IDisposable.Dispose
        _client.Dispose()
    End Sub
End Class
