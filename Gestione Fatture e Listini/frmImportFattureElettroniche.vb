Public Class frmImportFattureElettroniche

    Private Async Sub cmdDecomprimiZip_Click(sender As Object, e As EventArgs) Handles cmdDecomprimiZip.Click
        Dim progress As New Progress(Of String)(Sub(msg) lblStatus.Text = msg)
        ProgressBar1.Style = ProgressBarStyle.Marquee
        ProgressBar1.Visible = True
        Dim risultato As String = Await Task.Run(Function() ModuloImportFattureXml.DecomprimiZip(progress))
        ProgressBar1.Visible = False
        ProgressBar1.Style = ProgressBarStyle.Blocks
        lblStatus.Text = ""
        MsgBox(risultato)
    End Sub

    Private Async Sub cmdRimuoviFirma_Click(sender As Object, e As EventArgs) Handles cmdRimuoviFirma.Click
        Dim progress As New Progress(Of String)(Sub(msg) lblStatus.Text = msg)
        ProgressBar1.Style = ProgressBarStyle.Marquee
        ProgressBar1.Visible = True
        Dim risultato As String = Await Task.Run(Function() ModuloImportFattureXml.RimuoviFirmaDigitale(progress))
        ProgressBar1.Visible = False
        ProgressBar1.Style = ProgressBarStyle.Blocks
        lblStatus.Text = ""
        MsgBox(risultato)
    End Sub

    Private Async Sub cmdImportaXml_Click(sender As Object, e As EventArgs) Handles cmdImportaXml.Click
        Await EseguiImportazioneAsync()
    End Sub

    Private Async Function EseguiImportazioneAsync() As Task
        Dim progress As New Progress(Of String)(Sub(msg) lblStatus.Text = msg)
        ProgressBar1.Style = ProgressBarStyle.Marquee
        ProgressBar1.Visible = True

        Try
            Dim risultato As String = Await Task.Run(Function() ModuloImportFattureXml.ImportaFattureDaXml(progress))
            MessageBox.Show(risultato)
        Catch ex As Exception
            MessageBox.Show("Errore durante l'importazione delle fatture XML: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ProgressBar1.Visible = False
            ProgressBar1.Style = ProgressBarStyle.Blocks
            ProgressBar1.Value = 0
            lblStatus.Text = ""
        End Try
    End Function

    Private Async Sub cmdPipelineCompleta_Click(sender As Object, e As EventArgs) Handles cmdPipelineCompleta.Click
        Dim progress As New Progress(Of String)(Sub(msg) lblStatus.Text = msg)
        ProgressBar1.Style = ProgressBarStyle.Marquee
        ProgressBar1.Visible = True

        Try
            lblStatus.Text = "Pipeline completa: decompressione ZIP in corso..."
            Dim reportZip As String = Await Task.Run(Function() ModuloImportFattureXml.DecomprimiZip(progress))

            lblStatus.Text = "Pipeline completa: rimozione firma digitale in corso..."
            Dim reportFirma As String = Await Task.Run(Function() ModuloImportFattureXml.RimuoviFirmaDigitale(progress))

            lblStatus.Text = "Pipeline completa: importazione fatture XML in corso..."
            Dim reportXml As String = Await Task.Run(Function() ModuloImportFattureXml.ImportaFattureDaXml(progress))

            MsgBox(reportZip & vbCrLf & vbCrLf & reportFirma & vbCrLf & vbCrLf & reportXml)
        Catch ex As Exception
            MessageBox.Show("Errore durante la pipeline completa: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ProgressBar1.Visible = False
            ProgressBar1.Style = ProgressBarStyle.Blocks
            ProgressBar1.Value = 0
            lblStatus.Text = ""
        End Try
    End Sub

End Class
