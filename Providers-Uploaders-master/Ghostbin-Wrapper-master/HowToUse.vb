Imports System.IO
Imports System.Net
Imports System.Text
Imports GhostBin_Wrapper.GhostBinUploaderClass.Core.Providers.GhostBinProvider
Public Class HowToUse
    Dim cookies As New CookieContainer
    'Cookies are needed for accessing/editing protected pastes.
    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using Gbin = New GhostBinUploaderClass.Core.Providers.GhostBinProvider(cookies)
            Dim response = Await Gbin.UploadText(txtTitle.Text, RichTextBox1.Text, tbPass.Text)

            Dim message = If(response.Success, response.PasteUri.ToString(), "Upload Failure")
            RichTextBox1.Text = message
            
            cookies = response.PasteCookie
        End Using
    End Sub

    Private Async Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Using Gbin = New GhostBinUploaderClass.Core.Providers.GhostBinProvider(cookies)
            Dim response = Await Gbin.UpdateText(txtTitle.Text, RichTextBox1.Text, tbPass.Text, tbIndex.Text)

            Dim message = If(response.Success, response.PasteUri.ToString(), "Upload Failure")
            RichTextBox1.Text = message
        
            cookies = response.PasteCookie
        End Using
    End Sub

    Private Async Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Using Gbin = New GhostBinUploaderClass.Core.Providers.GhostBinProvider(cookies)
            Dim response = Await Gbin.GrabText(tbIndex.Text)

            Dim message = If(response.Success, response.Paste, "Upload Failure")
            RichTextBox1.Text = message
    
            cookies = response.PasteCookie
        End Using
    End Sub
End Class
