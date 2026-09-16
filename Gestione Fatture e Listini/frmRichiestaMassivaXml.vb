Public Class frmRichiestaMassivaXml

    Private Sub frmRichiestaMassivaXml_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim dataDa As Date
        If Date.TryParseExact(My.Settings.UltimaDataDaRichiestaMassiva, "yyyy-MM-dd", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dataDa) Then
            dtpDa.Value = dataDa
        Else
            dtpDa.Value = DateTime.Now.AddMonths(-1)
        End If

        Dim dataA As Date
        If Date.TryParseExact(My.Settings.UltimaDataARichiestaMassiva, "yyyy-MM-dd", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dataA) Then
            dtpA.Value = dataA
        Else
            dtpA.Value = DateTime.Now
        End If
    End Sub

    Private Sub btnGenera_Click(sender As Object, e As EventArgs) Handles btnGenera.Click
        Dim cartellaDestinazione As String = My.Settings.CartellaXmlRichiestaMassiva

        If String.IsNullOrWhiteSpace(cartellaDestinazione) Then
            MsgBox("Indica prima la cartella di salvataggio del file XML nelle Impostazioni.", MsgBoxStyle.Exclamation)
            Return
        End If

        Dim dataDa As Date = dtpDa.Value.Date
        Dim dataA As Date = dtpA.Value.Date

        If dataDa > dataA Then
            MsgBox("La data di partenza non può essere successiva alla data finale.", MsgBoxStyle.Exclamation)
            Return
        End If

        Try
            Dim percorsoFile As String = ModuloRichiestaMassivaXml.GeneraXmlRichiestaMassiva(dataDa, dataA, cartellaDestinazione)

            My.Settings.UltimaDataDaRichiestaMassiva = dataDa.ToString("yyyy-MM-dd")
            My.Settings.UltimaDataARichiestaMassiva = dataA.ToString("yyyy-MM-dd")
            My.Settings.Save()

            MsgBox("File XML generato correttamente:" & vbCrLf & percorsoFile, MsgBoxStyle.Information)
        Catch ex As Exception
            MsgBox("Errore durante la generazione del file XML: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub btnChiudi_Click(sender As Object, e As EventArgs) Handles btnChiudi.Click
        Me.Close()
    End Sub
End Class
