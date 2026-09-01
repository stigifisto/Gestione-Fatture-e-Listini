Public Class frmImpostazioni

    Private Sub frmImpostazioni_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtCartellaZip.Text = My.Settings.CartellaZip
        txtCartellaDecompressi.Text = My.Settings.CartellaDecompressi
        txtCartellaXmlPronti.Text = My.Settings.CartellaXmlPronti
        txtCartellaBackup.Text = My.Settings.CartellaBackup
        txtCartellaDoppi.Text = My.Settings.CartellaDoppi
        nudScostamento.Value = My.Settings.ScostamentoAccettabile
    End Sub

    Private Sub Sfoglia(txt As TextBox)
        Using fbd As New FolderBrowserDialog()
            If IO.Directory.Exists(txt.Text) Then fbd.SelectedPath = txt.Text
            If fbd.ShowDialog() = DialogResult.OK Then
                txt.Text = fbd.SelectedPath
            End If
        End Using
    End Sub

    Private Sub btnSfogliaZip_Click(sender As Object, e As EventArgs) Handles btnSfogliaZip.Click
        Sfoglia(txtCartellaZip)
    End Sub

    Private Sub btnSfogliaDecompressi_Click(sender As Object, e As EventArgs) Handles btnSfogliaDecompressi.Click
        Sfoglia(txtCartellaDecompressi)
    End Sub

    Private Sub btnSfogliaXmlPronti_Click(sender As Object, e As EventArgs) Handles btnSfogliaXmlPronti.Click
        Sfoglia(txtCartellaXmlPronti)
    End Sub

    Private Sub btnSfogliaBackup_Click(sender As Object, e As EventArgs) Handles btnSfogliaBackup.Click
        Sfoglia(txtCartellaBackup)
    End Sub

    Private Sub btnSfogliaDoppi_Click(sender As Object, e As EventArgs) Handles btnSfogliaDoppi.Click
        Sfoglia(txtCartellaDoppi)
    End Sub

    Private Sub btnSalva_Click(sender As Object, e As EventArgs) Handles btnSalva.Click
        If String.IsNullOrWhiteSpace(txtCartellaZip.Text) OrElse
           String.IsNullOrWhiteSpace(txtCartellaDecompressi.Text) OrElse
           String.IsNullOrWhiteSpace(txtCartellaXmlPronti.Text) OrElse
           String.IsNullOrWhiteSpace(txtCartellaBackup.Text) OrElse
           String.IsNullOrWhiteSpace(txtCartellaDoppi.Text) Then
            MsgBox("Tutte le cartelle devono essere specificate.")
            Return
        End If

        My.Settings.CartellaZip = txtCartellaZip.Text.Trim()
        My.Settings.CartellaDecompressi = txtCartellaDecompressi.Text.Trim()
        My.Settings.CartellaXmlPronti = txtCartellaXmlPronti.Text.Trim()
        My.Settings.CartellaBackup = txtCartellaBackup.Text.Trim()
        My.Settings.CartellaDoppi = txtCartellaDoppi.Text.Trim()
        My.Settings.ScostamentoAccettabile = nudScostamento.Value
        My.Settings.Save()

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnAnnulla_Click(sender As Object, e As EventArgs) Handles btnAnnulla.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class
