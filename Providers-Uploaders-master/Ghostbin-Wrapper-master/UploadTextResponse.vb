Imports System.Net
Imports System.Net.Http
Imports System.Text.RegularExpressions

Namespace GhostBinUploaderClass.Core.Models
    Public Class GrabTextResponse
        Public ReadOnly Property Success() As Boolean
        Public ReadOnly Property Paste() As String
        Public ReadOnly Property PasteCookie() As CookieContainer
        Public Sub New(source As String, cookies As CookieContainer)
            Dim match As String = Regex.Split(Regex.Split(source, "id=" & Chr(34) & "code" & Chr(34) & ">")(1), "</div>")(0)
            If match = "" Then
                Success = False
                Return
            End If
            Success = True
            Paste = match
            PasteCookie = cookies
        End Sub
    End Class
    Public Class UploadTextResponse
        Public ReadOnly Property Success() As Boolean
        Public ReadOnly Property PasteUri() As Uri
        Public ReadOnly Property PasteCookie() As CookieContainer
        Public Sub New(response As HttpResponseMessage, baseAddress As Uri, cookies As CookieContainer)
            Dim path = response.Headers.Location
            Dim match = Regex.Match(path.ToString(), "paste/(\w+)")
            If Not match.Success Then
                Success = False
                Return
            End If
            Success = True
            PasteUri = New Uri(baseAddress.ToString & match.Captures(0).ToString)
            PasteCookie = cookies
        End Sub
    End Class

End Namespace