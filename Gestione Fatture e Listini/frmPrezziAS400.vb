Imports System.Data.SqlClient

Public Class frmPrezziAS400
    Dim connectionString As String = "Server=192.168.2.19\inalcasql12;Database=infinitydb;User ID=infinity_UTENTE;Password=antonio.speziali"
    Private ReadOnly bindingRisultati As New BindingSource()

    Private Sub frmPrezziAS400_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dtpDal.Value = DateTime.Now.AddMonths(-1)
        dtpAl.Value = DateTime.Now
        CaricaFornitori()
    End Sub

    ''' <summary>
    ''' Carica in cmbFornitori l'elenco dei fornitori (codice AS400 + descrizione),
    ''' con possibilità di ricerca digitando la descrizione.
    ''' </summary>
    Private Sub CaricaFornitori()
        Dim sql As String = "SELECT CONCAT('00', SUBSTRING(KSCODSOG,3,6)) AS CODICE, KSDESCRI AS DESCRIZIONE " &
                             "FROM ba_keysog001 WHERE KSTIPSOG = 'FOR' " &
                             "ORDER BY CONCAT('00', SUBSTRING(KSCODSOG,3,6))"

        Dim dtFornitori As New DataTable()

        Using conn As New SqlConnection(connectionString)
            Try
                Dim da As New SqlDataAdapter(sql, conn)
                da.Fill(dtFornitori)

                cmbFornitori.DisplayMember = "DESCRIZIONE"
                cmbFornitori.ValueMember = "CODICE"
                cmbFornitori.DataSource = dtFornitori
                cmbFornitori.SelectedIndex = -1
            Catch ex As Exception
                MsgBox("Errore nel caricamento fornitori: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Async Sub btnAnalizza_Click(sender As Object, e As EventArgs) Handles btnAnalizza.Click
        If cmbFornitori.SelectedValue Is Nothing OrElse IsDBNull(cmbFornitori.SelectedValue) Then
            MsgBox("Seleziona un fornitore.")
            Return
        End If

        Dim codiceFornitore As String = cmbFornitori.SelectedValue.ToString()
        Dim dal As Date = dtpDal.Value.Date
        Dim al As Date = dtpAl.Value.Date

        btnAnalizza.Enabled = False
        lblStatistiche.Text = "Analisi in corso..."

        Dim usaListinoInfinity As Boolean = rdoListinoInfinity.Checked

        Try
            Dim dt As DataTable = Await Task.Run(Function() GetAnomaliePrezziAS400(codiceFornitore, dal, al, usaListinoInfinity))
            txtFiltroNumeroFattura.Clear()
            bindingRisultati.DataSource = dt
            dgvRisultati.DataSource = bindingRisultati
            FormattazioneEsteticaGriglia()

            Dim totale As Decimal = CalcolaTotaleAnomalie(dt)
            lblStatistiche.Text = $"{dt.Rows.Count} righe analizzate — potenziale recupero: € {totale:N2}"
        Catch ex As Exception
            lblStatistiche.Text = "Analisi non riuscita: " & ex.Message
            MessageBox.Show("Errore durante l'analisi dei prezzi: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            btnAnalizza.Enabled = True
        End Try
    End Sub

    ''' <summary>
    ''' Filtra la griglia in tempo reale in base al numero fattura digitato (corrispondenza parziale).
    ''' </summary>
    Private Sub txtFiltroNumeroFattura_TextChanged(sender As Object, e As EventArgs) Handles txtFiltroNumeroFattura.TextChanged
        If bindingRisultati.DataSource Is Nothing Then Return

        Dim testo As String = txtFiltroNumeroFattura.Text.Trim()
        If testo.Length = 0 Then
            bindingRisultati.RemoveFilter()
        Else
            Dim valoreEscaped As String = testo.Replace("'", "''")
            bindingRisultati.Filter = $"CONVERT(NumeroFattura, 'System.String') LIKE '%{valoreEscaped}%'"
        End If
    End Sub

    Private Sub btnStampa_Click(sender As Object, e As EventArgs) Handles btnStampa.Click
        Dim fornitore As String = If(cmbFornitori.SelectedIndex >= 0, cmbFornitori.Text, "")
        Dim listino As String = If(rdoListinoInfinity.Checked, "Infinity", "AS400")
        Dim sottotitolo As String = $"Fornitore: {fornitore} — Bolle dal {dtpDal.Value.Date:dd/MM/yyyy} al {dtpAl.Value.Date:dd/MM/yyyy} — Listino: {listino} — Stampato il {DateTime.Now:dd/MM/yyyy HH:mm}"

        ModuloStampaAnomalie.StampaAnomalieGriglia(dgvRisultati, "Analisi prezzi fatture AS400 vs listino — Anomalie", sottotitolo)
    End Sub

    ''' <summary>
    ''' Confronta, per il fornitore e il range di data bolla indicati, il prezzo fatturato
    ''' (Fatture_AS400.MOAPRZ) con il prezzo netto di listino (PrezzoNettoCalcolato), preso a
    ''' scelta da Listini_Acquisto_As400 (join su CodiceArticolo = MOACOD) oppure da
    ''' Listini_Acquisto_Infinity (join su CodiceArticoloAs400 = MOACOD).
    ''' Il listino applicabile viene individuato per CodiceFornitore + articolo + data bolla
    ''' compresa nel range di validità. Il ValidoPer da usare viene determinato tramite
    ''' FornitoriListini (per fornitore + centro di costo); se non c'è una corrispondenza
    ''' specifica, oppure per quel ValidoPer non esiste un listino applicabile, si ripiega
    ''' sul ValidoPer '01EUR'.
    ''' </summary>
    Public Function GetAnomaliePrezziAS400(codiceFornitore As String, dal As Date, al As Date, usaListinoInfinity As Boolean) As DataTable
        Dim tabellaListino As String = If(usaListinoInfinity, "Listini_Acquisto_Infinity", "Listini_Acquisto_As400")
        Dim colonnaArticoloListino As String = If(usaListinoInfinity, "CodiceArticoloAs400", "CodiceArticolo")

        Dim sql As String = "
            SELECT
                f.MOAFOR AS CodiceFornitore,
                f.MOACDC AS CentroCosto,
                f.ENTDESCR AS DescrizioneCentroCosto,
                f.MOACOD AS CodiceArticolo,
                f.MOADES AS DescrizioneArticolo,
                f.MOADBO AS DataBolla,
                f.MOANBO AS NumeroBolla,
                f.MOADFT AS DataFattura,
                f.MOANFT AS NumeroFattura,
                f.MOAQTA AS Quantita,
                f.MOAPRZ AS PrezzoFatturato,
                lst.ValidoPer AS ValidoPerUtilizzato,
                lst.PrezzoNettoCalcolato AS Unitario_Netto_Listino,
                (f.MOAPRZ - lst.PrezzoNettoCalcolato) AS Differenza_Unitaria,
                ((f.MOAPRZ - lst.PrezzoNettoCalcolato) * f.MOAQTA) AS Differenza_Totale_Riga,
                CASE
                    WHEN lst.PrezzoNettoCalcolato IS NULL THEN 'Mancante a Listino'
                    WHEN (f.MOAPRZ - lst.PrezzoNettoCalcolato) > @scostamento THEN 'Prezzo Eccessivo'
                    WHEN (f.MOAPRZ - lst.PrezzoNettoCalcolato) < 0 THEN 'Prezzo Inferiore'
                    ELSE 'In Bolla'
                END AS Stato_Anomalia
            FROM Fatture_AS400 f
            OUTER APPLY (
                SELECT TOP 1 fl.VALIDOPER
                FROM FornitoriListini fl
                WHERE fl.CODICEFORNITORE = f.MOAFOR
                AND fl.MOACDC = f.MOACDC
            ) vp
            OUTER APPLY (
                SELECT TOP 1 l.PrezzoNettoCalcolato, l.ValidoPer
                FROM " & tabellaListino & " l
                WHERE l.CodiceFornitore = f.MOAFOR
                  AND LTRIM(RTRIM(l." & colonnaArticoloListino & ")) = LTRIM(RTRIM(f.MOACOD))
                  AND f.MOADBO >= l.DataInizioValidita
                  AND (l.DataFineValidita IS NULL OR f.MOADBO <= l.DataFineValidita)
                  AND (l.ValidoPer = vp.VALIDOPER OR l.ValidoPer = '01EUR')
                ORDER BY CASE WHEN l.ValidoPer = vp.VALIDOPER THEN 0 ELSE 1 END
            ) lst
            WHERE f.MOAFOR = @fornitore
              AND f.MOADBO BETWEEN @dal AND @al
            ORDER BY f.MOADBO, f.MOANBO"
        'f.COFCOF AS CodiceArticoloFornitore,
        'AND TRY_CAST(fl.MOACDC AS DECIMAL(10,2)) = f.MOACDC
        'AND l.CodiceArticoloFornitore = f.COFCOF
        Dim dt As New DataTable()

        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@fornitore", codiceFornitore)
            cmd.Parameters.AddWithValue("@dal", dal)
            cmd.Parameters.AddWithValue("@al", al)
            cmd.Parameters.AddWithValue("@scostamento", My.Settings.ScostamentoAccettabile)

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        Return dt
    End Function

    Private Sub FormattazioneEsteticaGriglia()
        For Each col As DataGridViewColumn In dgvRisultati.Columns
            If col.ValueType Is GetType(Decimal) OrElse col.ValueType Is GetType(Double) Then
                col.DefaultCellStyle.Format = "N3"
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
        Next

        For Each row As DataGridViewRow In dgvRisultati.Rows
            If IsDBNull(row.Cells("Unitario_Netto_Listino").Value) Then
                row.DefaultCellStyle.BackColor = Color.LemonChiffon
            ElseIf Convert.ToDecimal(row.Cells("Differenza_Totale_Riga").Value) > My.Settings.ScostamentoAccettabile Then
                row.DefaultCellStyle.ForeColor = Color.Red
                row.Cells("Differenza_Totale_Riga").Style.Font = New Font(dgvRisultati.Font, FontStyle.Bold)
            ElseIf Convert.ToDecimal(row.Cells("Differenza_Totale_Riga").Value) < 0D Then
                row.DefaultCellStyle.ForeColor = Color.Blue
                row.Cells("Differenza_Totale_Riga").Style.Font = New Font(dgvRisultati.Font, FontStyle.Bold)
            End If
        Next
    End Sub

    Private Function CalcolaTotaleAnomalie(dt As DataTable) As Decimal
        Dim totale As Decimal = 0
        For Each row As DataRow In dt.Rows
            If Not IsDBNull(row("Differenza_Totale_Riga")) Then
                Dim diff = Convert.ToDecimal(row("Differenza_Totale_Riga"))
                If diff > 0 Then totale += diff
            End If
        Next
        Return totale
    End Function
End Class
