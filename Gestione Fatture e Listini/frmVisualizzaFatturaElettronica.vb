Imports System.Data.SqlClient

''' <summary>
''' Permette di cercare, per fornitore e intervallo di data fattura, le fatture elettroniche
''' importate e di visualizzarle nel formato ministeriale a partire dal file XML archiviato.
''' </summary>
Public Class frmVisualizzaFatturaElettronica
    Private ReadOnly bindingFatture As New BindingSource()

    Private Sub frmVisualizzaFatturaElettronica_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dtpDal.Value = DateTime.Now.AddMonths(-1)
        dtpAl.Value = DateTime.Now
        CaricaFornitori()
    End Sub

    ''' <summary>
    ''' Carica in cmbFornitori l'elenco dei cedenti/fornitori presenti nelle fatture elettroniche
    ''' importate, con possibilità di ricerca digitando la denominazione.
    ''' </summary>
    Private Sub CaricaFornitori()
        Dim sql As String = "SELECT DISTINCT CedenteIdCodice, CedenteDenominazione " &
                             "FROM Fatture_Testate " &
                             "ORDER BY CedenteDenominazione"

        Dim dtFornitori As New DataTable()

        Using conn As New SqlConnection(ModuloImportFattureXml.connectionString)
            Try
                Dim da As New SqlDataAdapter(sql, conn)
                da.Fill(dtFornitori)

                cmbFornitori.DisplayMember = "CedenteDenominazione"
                cmbFornitori.ValueMember = "CedenteIdCodice"
                cmbFornitori.DataSource = dtFornitori
                cmbFornitori.SelectedIndex = -1
            Catch ex As Exception
                MsgBox("Errore nel caricamento fornitori: " & ex.Message)
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' Aggiorna l'elenco delle fatture disponibili non appena viene impostato un fornitore
    ''' e/o cambia l'intervallo di data fattura.
    ''' </summary>
    Private Sub AggiornaElenco(sender As Object, e As EventArgs) Handles cmbFornitori.SelectedIndexChanged, dtpDal.ValueChanged, dtpAl.ValueChanged
        If cmbFornitori.SelectedValue Is Nothing OrElse IsDBNull(cmbFornitori.SelectedValue) Then
            bindingFatture.DataSource = Nothing
            dgvFatture.DataSource = Nothing
            Return
        End If

        Dim cedenteIdCodice As String = cmbFornitori.SelectedValue.ToString()
        Dim dal As Date = dtpDal.Value.Date
        Dim al As Date = dtpAl.Value.Date

        Dim sql As String = "SELECT NomeFile, NumeroFattura, DataFattura " &
                             "FROM Fatture_Testate " &
                             "WHERE CedenteIdCodice = @fornitore AND DataFattura BETWEEN @dal AND @al " &
                             "ORDER BY DataFattura, NumeroFattura"

        Dim dt As New DataTable()

        Try
            Using conn As New SqlConnection(ModuloImportFattureXml.connectionString)
                Dim cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@fornitore", cedenteIdCodice)
                cmd.Parameters.AddWithValue("@dal", dal)
                cmd.Parameters.AddWithValue("@al", al)

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante la ricerca delle fatture: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        bindingFatture.DataSource = dt
        dgvFatture.DataSource = bindingFatture

        If dgvFatture.Columns.Contains("DataFattura") Then
            dgvFatture.Columns("DataFattura").DefaultCellStyle.Format = "dd/MM/yyyy"
        End If
    End Sub

    Private Sub btnVisualizza_Click(sender As Object, e As EventArgs) Handles btnVisualizza.Click
        If dgvFatture.CurrentRow Is Nothing Then
            MsgBox("Seleziona una fattura dalla griglia.")
            Return
        End If

        Dim nomeFile As String = dgvFatture.CurrentRow.Cells("NomeFile").Value.ToString()
        Dim numeroFattura As String = dgvFatture.CurrentRow.Cells("NumeroFattura").Value.ToString()
        Dim percorsoFile As String = IO.Path.Combine(My.Settings.CartellaBackup, nomeFile)

        If Not IO.File.Exists(percorsoFile) Then
            MessageBox.Show("File XML non trovato nella cartella di backup:" & vbCrLf & percorsoFile, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim dataRicezione As Date? = OttieniDataRicezione(nomeFile)

        Using frm As New frmAnteprimaFattura(percorsoFile, $"Fattura {numeroFattura} — {nomeFile}", dataRicezione)
            frm.ShowDialog(Me)
        End Using
    End Sub

    ''' <summary>
    ''' Recupera, se disponibile, la data di ricezione della fattura (dal metadato SDI importato in
    ''' Fatture_Ricezione), mostrata nel formato compatto. Puramente informativa: se non disponibile
    ''' o in caso di errore, la fattura viene comunque visualizzata senza questo dato.
    ''' </summary>
    Private Function OttieniDataRicezione(nomeFile As String) As Date?
        Try
            Using conn As New SqlConnection(ModuloImportFattureXml.connectionString)
                conn.Open()
                Dim cmd As New SqlCommand("SELECT DataRicezione FROM Fatture_Ricezione WHERE NomeFile = @nome", conn)
                cmd.Parameters.AddWithValue("@nome", nomeFile)
                Dim risultato = cmd.ExecuteScalar()
                If risultato IsNot Nothing AndAlso Not IsDBNull(risultato) Then
                    Return Convert.ToDateTime(risultato)
                End If
            End Using
        Catch
        End Try
        Return Nothing
    End Function
End Class
