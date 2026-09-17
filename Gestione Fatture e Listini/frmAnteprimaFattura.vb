''' <summary>
''' Mostra, in un controllo WebBrowser, la fattura elettronica renderizzata a partire dal file
''' XML, con la possibilità di passare dal foglio di stile ufficiale ("Ministeriale") a un
''' layout tabellare compatto ("Compatto").
''' </summary>
Public Class frmAnteprimaFattura
    Private ReadOnly percorsoXml As String
    Private ReadOnly dataRicezione As Date?
    Private htmlMinisteriale As String
    Private htmlCompatto As String

    Public Sub New(pathXml As String, titolo As String, Optional dataRicezioneParam As Date? = Nothing)
        Me.New()
        Me.Text = titolo
        percorsoXml = pathXml
        dataRicezione = dataRicezioneParam
    End Sub

    Private Sub frmAnteprimaFattura_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuloVisualizzaFatturaXml.ImpostaEmulazioneBrowserModerna()
        MostraFormatoSelezionato()
    End Sub

    Private Sub btnStampa_Click(sender As Object, e As EventArgs) Handles btnStampa.Click
        Try
            WebBrowser1.ShowPrintDialog()
        Catch ex As Exception
            MessageBox.Show("Errore durante la stampa della fattura: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub RadioFormato_CheckedChanged(sender As Object, e As EventArgs) Handles rdoMinisteriale.CheckedChanged, rdoCompatto.CheckedChanged
        If DirectCast(sender, RadioButton).Checked Then
            MostraFormatoSelezionato()
        End If
    End Sub

    Private Sub MostraFormatoSelezionato()
        ' Durante Me.New() il designer imposta rdoMinisteriale.Checked = True, il che scatena
        ' CheckedChanged prima ancora che il costruttore assegni percorsoXml: in quella finestra
        ' non c'è ancora nulla da visualizzare.
        If String.IsNullOrEmpty(percorsoXml) Then Return

        Try
            If rdoCompatto.Checked Then
                If htmlCompatto Is Nothing Then
                    htmlCompatto = ModuloVisualizzaFatturaCompatta.RenderizzaFatturaHtmlCompatto(percorsoXml, dataRicezione)
                End If
                WebBrowser1.DocumentText = htmlCompatto
            Else
                If htmlMinisteriale Is Nothing Then
                    htmlMinisteriale = ModuloVisualizzaFatturaXml.RenderizzaFatturaHtml(percorsoXml)
                End If
                WebBrowser1.DocumentText = htmlMinisteriale
            End If
        Catch ex As Exception
            MessageBox.Show("Errore durante la visualizzazione della fattura: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
