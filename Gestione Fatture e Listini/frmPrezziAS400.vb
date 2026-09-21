Imports System.Data.SqlClient

Public Class frmPrezziAS400
    Dim connectionString As String = "Server=192.168.2.19\inalcasql12;Database=infinitydb;User ID=infinity_UTENTE;Password=antonio.speziali"
    Private ReadOnly bindingRisultati As New BindingSource()

    Private Sub frmPrezziAS400_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dtpDal.Value = DateTime.Now.AddMonths(-1)
        dtpAl.Value = DateTime.Now
        rdoStatoTutti.Checked = True
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
        If Not AssicuraFornitoreSelezionato() Then Return

        Dim codiceFornitore As String = cmbFornitori.SelectedValue.ToString()
        Dim dal As Date = dtpDal.Value.Date
        Dim al As Date = dtpAl.Value.Date

        btnAnalizza.Enabled = False
        lblStatistiche.Text = "Analisi in corso..."

        Dim usaListinoInfinity As Boolean = rdoListinoInfinity.Checked

        Try
            Dim dt As DataTable = Await Task.Run(Function() GetAnomaliePrezziAS400(codiceFornitore, dal, al, usaListinoInfinity))
            txtFiltroNumeroFattura.Clear()
            rdoStatoTutti.Checked = True
            bindingRisultati.DataSource = dt
            dgvRisultati.DataSource = bindingRisultati
            AggiornaFiltro()

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
    ''' Se cmbFornitori ha già una selezione valida non fa nulla e restituisce True. Altrimenti
    ''' cerca i fornitori con bolle nel periodo dtpDal/dtpAl e apre frmScegliFornitore per
    ''' sceglierne uno: se l'utente seleziona una riga, imposta cmbFornitori e restituisce True;
    ''' se annulla, o non ci sono fornitori nel periodo, restituisce False.
    ''' </summary>
    Private Function AssicuraFornitoreSelezionato() As Boolean
        If cmbFornitori.SelectedValue IsNot Nothing AndAlso Not IsDBNull(cmbFornitori.SelectedValue) Then
            Return True
        End If

        Dim dal As Date = dtpDal.Value.Date
        Dim al As Date = dtpAl.Value.Date

        Dim dtFornitori As DataTable
        Try
            dtFornitori = GetFornitoriConFattureNelPeriodo(dal, al)
        Catch ex As Exception
            MessageBox.Show("Errore durante la ricerca dei fornitori: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        If dtFornitori.Rows.Count = 0 Then
            MsgBox("Nessuna fattura trovata nel periodo selezionato.")
            Return False
        End If

        Try
            Using frm As New frmScegliFornitore(dtFornitori)
                If frm.ShowDialog(Me) <> DialogResult.OK Then Return False

                cmbFornitori.SelectedValue = frm.CodiceFornitoreSelezionato
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante la selezione del fornitore: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        Return cmbFornitori.SelectedValue IsNot Nothing AndAlso Not IsDBNull(cmbFornitori.SelectedValue)
    End Function

    ''' <summary>
    ''' Elenca, in ordine alfabetico di descrizione, i fornitori (ba_keysog001) con almeno una
    ''' bolla (Fatture_AS400.MOADBO) nel range di data indicato, con il relativo conteggio fatture.
    ''' </summary>
    Private Function GetFornitoriConFattureNelPeriodo(dal As Date, al As Date) As DataTable
        Dim sql As String = "
            SELECT
                k.Codice,
                k.Descrizione,
                COUNT(DISTINCT f.MOANFT) AS NumeroFatture
            FROM Fatture_AS400 f
            INNER JOIN (
                SELECT CONCAT('00', SUBSTRING(KSCODSOG,3,6)) AS Codice, KSDESCRI AS Descrizione
                FROM ba_keysog001 WHERE KSTIPSOG = 'FOR'
            ) k ON k.Codice = f.MOAFOR
            WHERE f.MOADBO BETWEEN @dal AND @al
            GROUP BY k.Codice, k.Descrizione
            ORDER BY k.Descrizione"

        Dim dt As New DataTable()

        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(sql, conn)
            cmd.CommandTimeout = 180
            cmd.Parameters.AddWithValue("@dal", dal)
            cmd.Parameters.AddWithValue("@al", al)

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        Return dt
    End Function

    ''' <summary>
    ''' Filtra la griglia in tempo reale in base al numero fattura digitato (corrispondenza parziale).
    ''' Selezionando una fattura specifica lo stato anomalia torna su "Tutti".
    ''' </summary>
    Private Sub txtFiltroNumeroFattura_TextChanged(sender As Object, e As EventArgs) Handles txtFiltroNumeroFattura.TextChanged
        If bindingRisultati.DataSource Is Nothing Then Return

        rdoStatoTutti.Checked = True
        AggiornaFiltro()
    End Sub

    ''' <summary>
    ''' Riapplica il filtro alla griglia combinando il numero fattura digitato con lo stato
    ''' anomalia selezionato tramite gli option button.
    ''' </summary>
    Private Sub RadioStatoAnomalia_CheckedChanged(sender As Object, e As EventArgs) Handles rdoStatoTutti.CheckedChanged,
        rdoStatoInBolla.CheckedChanged, rdoStatoMancante.CheckedChanged, rdoStatoEccessivo.CheckedChanged, rdoStatoInferiore.CheckedChanged
        If DirectCast(sender, RadioButton).Checked Then
            AggiornaFiltro()
        End If
    End Sub

    Private Sub AggiornaFiltro()
        If bindingRisultati.DataSource Is Nothing Then Return

        Dim condizioni As New List(Of String)

        Dim testo As String = txtFiltroNumeroFattura.Text.Trim()
        If testo.Length > 0 Then
            Dim valoreEscaped As String = testo.Replace("'", "''")
            condizioni.Add($"CONVERT(NumeroFattura, 'System.String') LIKE '%{valoreEscaped}%'")
        End If

        Dim stato As String = StatoAnomaliaSelezionato()
        If stato IsNot Nothing Then
            condizioni.Add($"Stato_Anomalia = '{stato.Replace("'", "''")}'")
        End If

        If condizioni.Count = 0 Then
            bindingRisultati.RemoveFilter()
        Else
            bindingRisultati.Filter = String.Join(" AND ", condizioni)
        End If

        ' Applicare/rimuovere il filtro ricrea le righe della griglia, perdendo le
        ' formattazioni per stato anomalia: vanno riapplicate ad ogni cambio di filtro.
        FormattazioneEsteticaGriglia()
    End Sub

    ''' <summary>
    ''' Restituisce il valore di Stato_Anomalia corrispondente all'option button selezionato,
    ''' oppure Nothing se è selezionato "Tutti".
    ''' </summary>
    Private Function StatoAnomaliaSelezionato() As String
        If rdoStatoInBolla.Checked Then Return "In Bolla"
        If rdoStatoMancante.Checked Then Return "Mancante a Listino"
        If rdoStatoEccessivo.Checked Then Return "Prezzo Eccessivo"
        If rdoStatoInferiore.Checked Then Return "Prezzo Inferiore"
        Return Nothing
    End Function

    Private Sub btnStampa_Click(sender As Object, e As EventArgs) Handles btnStampa.Click
        Dim fornitore As String = If(cmbFornitori.SelectedIndex >= 0, cmbFornitori.Text, "")
        Dim listino As String = If(rdoListinoInfinity.Checked, "Infinity", "AS400")
        Dim sottotitolo As String = $"Fornitore: {fornitore} — Bolle dal {dtpDal.Value.Date:dd/MM/yyyy} al {dtpAl.Value.Date:dd/MM/yyyy} — Listino: {listino} — Stampato il {DateTime.Now:dd/MM/yyyy HH:mm}"

        ModuloStampaAnomalie.StampaAnomalieGriglia(dgvRisultati, "Analisi prezzi fatture AS400 vs listino — Anomalie", sottotitolo)
    End Sub

    Private Sub btnEsportaExcel_Click(sender As Object, e As EventArgs) Handles btnEsportaExcel.Click
        Dim fornitore As String = If(cmbFornitori.SelectedIndex >= 0, cmbFornitori.Text, "")
        Dim listino As String = If(rdoListinoInfinity.Checked, "Infinity", "AS400")
        Dim sottotitolo As String = $"Fornitore: {fornitore} — Bolle dal {dtpDal.Value.Date:dd/MM/yyyy} al {dtpAl.Value.Date:dd/MM/yyyy} — Listino: {listino} — Esportato il {DateTime.Now:dd/MM/yyyy HH:mm}"

        ModuloEsportaAnomalie.EsportaAnomalieExcel(dgvRisultati, "Analisi prezzi fatture AS400 vs listino — Anomalie", sottotitolo)
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
                    -- Righe puramente descrittive (es. riferimento DDT), senza un vero codice
                    -- articolo: non hanno un listino da confrontare, quindi non sono un'anomalia.
                    WHEN lst.PrezzoNettoCalcolato IS NULL AND (LTRIM(RTRIM(ISNULL(f.MOACOD, ''))) = '' OR LTRIM(RTRIM(f.MOACOD)) = '0') THEN 'In Bolla'
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
            cmd.CommandTimeout = 180
            cmd.Parameters.AddWithValue("@fornitore", codiceFornitore)
            cmd.Parameters.AddWithValue("@dal", dal)
            cmd.Parameters.AddWithValue("@al", al)
            cmd.Parameters.AddWithValue("@scostamento", My.Settings.ScostamentoAccettabile)

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        Return dt
    End Function

    Private Sub dgvRisultati_Sorted(sender As Object, e As EventArgs) Handles dgvRisultati.Sorted
        FormattazioneEsteticaGriglia()
    End Sub

    Private Sub FormattazioneEsteticaGriglia()
        For Each col As DataGridViewColumn In dgvRisultati.Columns
            If col.ValueType Is GetType(Decimal) OrElse col.ValueType Is GetType(Double) Then
                col.DefaultCellStyle.Format = "N3"
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
        Next

        For Each row As DataGridViewRow In dgvRisultati.Rows
            If IsDBNull(row.Cells("Unitario_Netto_Listino").Value) Then
                Dim stato As String = If(row.Cells("Stato_Anomalia").Value Is Nothing, "", row.Cells("Stato_Anomalia").Value.ToString())
                If stato = "Mancante a Listino" Then
                    row.DefaultCellStyle.BackColor = Color.LemonChiffon
                End If
                ' Righe "In Bolla" senza un vero codice articolo (es. riferimento DDT): nessuna evidenziazione.
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
