Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Http
Imports System.Threading.Tasks
Imports GhostBin_Wrapper.GhostBinUploaderClass.Core.Models

Namespace GhostBinUploaderClass.Core.Providers
    Public Class GhostBinProvider
        Implements IDisposable
        Private ReadOnly _client As HttpClient
        Dim nCookies As New CookieContainer
        Public Sub New(cookies As CookieContainer)
            nCookies = cookies
            Dim handler = New HttpClientHandler() With {
                .AllowAutoRedirect = False,
                .CookieContainer = nCookies,
               .AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
            }

            _client = New HttpClient(handler)
            _client.BaseAddress = New Uri("https://ghostbin.com")

            _client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml")
            _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36")
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            _client.Dispose()
        End Sub

        Public Async Function UploadText(title As String, text As String, password As String) As Task(Of UploadTextResponse)
            If String.IsNullOrWhiteSpace(title) Then
                title = "Title"
            End If

            If String.IsNullOrWhiteSpace(text) Then
                Throw New ArgumentNullException(NameOf(text))
            End If

            If String.IsNullOrWhiteSpace(password) Then
                password = String.Empty
            End If

            ' Set the authentication cookie so GhostBin can verify that we've been on the homepage once.
            Await _client.GetAsync(_client.BaseAddress)

            Dim request = New HttpRequestMessage(HttpMethod.Post, "/paste/new") With {
                .Content = New FormUrlEncodedContent(New Dictionary(Of String, String)() From {
                    {"lang", "vb"},
                    {"text", text},
                    {"expire", "-1"},
                    {"password", password},
                    {"title", title}
                })
            }

            ' This method retrives the location header and redirects us to that page. 
            ' Since we've set AllowAutoRedirect to false on L18, it doesn't redirect 
            ' us and instead handles just the response. 
            Dim response = Await _client.SendAsync(request)
            Return New UploadTextResponse(response, _client.BaseAddress, nCookies)
        End Function
        Public Async Function UpdateText(title As String, text As String, password As String, index As String) As Task(Of UploadTextResponse)
            If String.IsNullOrWhiteSpace(title) Then
                title = "Title"
            End If

            If String.IsNullOrWhiteSpace(text) Then
                Throw New ArgumentNullException(NameOf(text))
            End If
            ' Set the authentication cookie so GhostBin can verify that we've been on the homepage once.
            Await _client.GetAsync(_client.BaseAddress)

            Dim request = New HttpRequestMessage(HttpMethod.Post, "/paste/" & index & "/edit") With {
                .Content = New FormUrlEncodedContent(New Dictionary(Of String, String)() From {
                    {"lang", "text"},
                    {"text", text},
                    {"expire", "-1"},
                    {"password", password},
                    {"title", title}
                })
            }

            ' This method retrives the location header and redirects us to that page. 
            ' Since we've set AllowAutoRedirect to false on L18, it doesn't redirect 
            ' us and instead handles just the response. 
            Dim response = Await _client.SendAsync(request)
            Return New UploadTextResponse(response, _client.BaseAddress, nCookies)
        End Function

        Public Async Function GrabText(index As String) As Task(Of GrabTextResponse)
            If String.IsNullOrWhiteSpace(index) Then
                Throw New ArgumentNullException(NameOf(index))
            End If
            Dim response As HttpResponseMessage = Await _client.GetAsync(New Uri(_client.BaseAddress.ToString & "paste/" & index))
            Dim result = Await response.Content.ReadAsStringAsync
            Return New GrabTextResponse(result.ToString, nCookies)
        End Function
    End Class
End Namespace