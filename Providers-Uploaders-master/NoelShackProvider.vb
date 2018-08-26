Imports System.IO
Imports System.Net.Http
Imports System.Text.RegularExpressions
Public Class NoelShackProvider
    Implements IDisposable
    Private _client As HttpClient
    Sub New()
        _client = New HttpClient()
    End Sub
    Public Async Function UploadFile(filePath As String) As Task(Of String)
        Try
            Dim form As New MultipartFormDataContent()
 
            Dim fBytes As Byte() = File.ReadAllBytes(filePath)
            Dim fName As String = Path.GetFileName(filePath)
            
            form.Add(New ByteArrayContent(fBytes, 0, fBytes.Count()), "fichier", fName)
            
            Dim response = Await _client.PostAsync("http://www.noelshack.com/api.php", form)
            Return Await response.Content.ReadAsStringAsync()
        Catch
            Return "Error!"
        End Try
    End Function
 
    Public Sub Dispose() Implements IDisposable.Dispose
        _client.Dispose()
    End Sub
End Class
