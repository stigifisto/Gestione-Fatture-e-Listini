Imports System.Data.SqlClient

Public Class frmPrezziFattureElettroniche
    Dim connectionString As String = "Server=192.168.2.19\inalcasql12;Database=infinitydb;User ID=infinity_UTENTE;Password=antonio.speziali"
    Private ReadOnly bindingRisultati As New BindingSource()

    ''' <summary>
    ''' Rappresenta una riga di Fatture_Sconti (sconto o maggiorazione) applicata a una linea fattura.
    ''' </summary>
    Private Class ScontoRiga
        Public Property Tipo As String
        Public Property Percentuale As Decimal
        Public Property Importo As Decimal
    End Class

    Private Sub frmPrezziFattureElettroniche_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dtpDal.Value = DateTime.Now.AddMonths(-1)
        dtpAl.Value = DateTime.Now
        CaricaFornitori()
    End Sub

    ''' <summary>
    ''' Carica in cmbFornitori l'elenco dei cedenti/fornitori presenti nelle fatture elettroniche
    ''' (Fatture_Testate), con possibilità di ricerca digitando la denominazione.
    ''' </summary>
    Private Sub CaricaFornitori()
        Dim sql As String = "SELECT DISTINCT CedenteIdCodice, CedenteDenominazione " &
                             "FROM Fatture_Testate " &
                             "ORDER BY CedenteDenominazione"

        Dim dtFornitori As New DataTable()

        Using conn As New SqlConnection(connectionString)
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

    Private Async Sub btnAnalizza_Click(sender As Object, e As EventArgs) Handles btnAnalizza.Click
        If cmbFornitori.SelectedValue Is Nothing OrElse IsDBNull(cmbFornitori.SelectedValue) Then
            MsgBox("Seleziona un fornitore.")
            Return
        End If

        Dim cedenteIdCodice As String = cmbFornitori.SelectedValue.ToString()
        Dim dal As Date = dtpDal.Value.Date
        Dim al As Date = dtpAl.Value.Date

        btnAnalizza.Enabled = False
        lblStatistiche.Text = "Analisi in corso..."

        Try
            Dim dt As DataTable = Await Task.Run(Function() GetAnomaliePrezziFattureElettroniche(cedenteIdCodice, dal, al))
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
            bindingRisultati.Filter = $"NumeroFattura LIKE '%{valoreEscaped}%'"
        End If
    End Sub

    Private Sub btnStampa_Click(sender As Object, e As EventArgs) Handles btnStampa.Click
        Dim fornitore As String = If(cmbFornitori.SelectedIndex >= 0, cmbFornitori.Text, "")
        Dim sottotitolo As String = $"Fornitore: {fornitore} — Fatture dal {dtpDal.Value.Date:dd/MM/yyyy} al {dtpAl.Value.Date:dd/MM/yyyy} — Stampato il {DateTime.Now:dd/MM/yyyy HH:mm}"

        ModuloStampaAnomalie.StampaAnomalieGriglia(dgvRisultati, "Analisi prezzi fatture elettroniche vs listino Infinity — Anomalie", sottotitolo)
    End Sub

    ''' <summary>
    ''' Confronta, per il cedente/fornitore e il range di data fattura indicati, il prezzo unitario
    ''' fatturato (Fatture_Righe.PrezzoUnitario), al netto degli sconti/maggiorazioni di riga
    ''' registrati in Fatture_Sconti, con il prezzo netto di listino
    ''' (Listini_Acquisto_Infinity.PrezzoNettoCalcolato) vigente alla data di riferimento della riga.
    ''' La data di riferimento è la DataDDT del documento di trasporto collegato alla riga
    ''' (Fatture_DDT.RiferimentoNumeroLinea = Fatture_Righe.NumeroLinea); se la riga non ha un DDT
    ''' associato, si usa la DataFattura. Il listino applicabile viene individuato incrociando
    ''' CedenteIdCodice = ID_FiscaleIVA_Fornitore e CodiceArticolo = CodiceArticoloFornitore, con la
    ''' data di riferimento compresa nel range di validità.
    ''' </summary>
    Public Function GetAnomaliePrezziFattureElettroniche(cedenteIdCodice As String, dal As Date, al As Date) As DataTable
        Dim sql As String = "
            SELECT
                r.ID_Fattura,
                t.CedenteIdCodice AS CodiceFornitore,
                t.CedenteDenominazione AS DescrizioneFornitore,
                t.NumeroFattura,
                t.DataFattura,
                r.NumeroLinea,
                r.CodiceArticolo,
                r.Descrizione AS DescrizioneArticolo,
                r.Quantita,
                r.PrezzoUnitario AS PrezzoLordo,
                rif.DataRiferimento,
                lst.PrezzoNettoCalcolato AS Unitario_Netto_Listino
            FROM Fatture_Testate t
            INNER JOIN Fatture_Righe r ON r.ID_Fattura = t.ID_Fattura
            OUTER APPLY (
                SELECT TOP 1 d.DataDDT
                FROM Fatture_DDT d
                WHERE d.ID_Fattura = r.ID_Fattura
                  AND d.RiferimentoNumeroLinea = r.NumeroLinea
                ORDER BY d.DataDDT
            ) primoDdt
            CROSS APPLY (SELECT ISNULL(primoDdt.DataDDT, t.DataFattura) AS DataRiferimento) rif
            OUTER APPLY (
                SELECT TOP 1 l.PrezzoNettoCalcolato
                FROM Listini_Acquisto_Infinity l
                WHERE l.ID_FiscaleIVA_Fornitore = t.CedenteIdCodice
                  AND LTRIM(RTRIM(l.CodiceArticoloFornitore)) = LTRIM(RTRIM(r.CodiceArticolo))
                  AND rif.DataRiferimento >= l.DataInizioValidita
                  AND (l.DataFineValidita IS NULL OR rif.DataRiferimento <= l.DataFineValidita)
                ORDER BY l.DataInizioValidita DESC
            ) lst
            WHERE t.CedenteIdCodice = @fornitore
              AND t.DataFattura BETWEEN @dal AND @al
            ORDER BY t.DataFattura, t.NumeroFattura, r.NumeroLinea"

        Dim dt As New DataTable()

        Using conn As New SqlConnection(connectionString)
            conn.Open()

            Dim cmd As New SqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@fornitore", cedenteIdCodice)
            cmd.Parameters.AddWithValue("@dal", dal)
            cmd.Parameters.AddWithValue("@al", al)

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            Dim sconti As Dictionary(Of String, List(Of ScontoRiga)) = CaricaSconti(conn, cedenteIdCodice, dal, al)
            ApplicaScontiEAnomalie(dt, sconti)
        End Using

        dt.Columns.Remove("ID_Fattura")

        Return dt
    End Function

    ''' <summary>
    ''' Carica, per il fornitore e il range di data indicati, tutte le righe di Fatture_Sconti
    ''' (sconti/maggiorazioni di riga), raggruppate per ID_Fattura+NumeroLinea e ordinate per
    ''' ID_Sconto in modo da rispettare l'ordine di applicazione originario del blocco XML
    ''' ScontoMaggiorazione.
    ''' </summary>
    Private Function CaricaSconti(conn As SqlConnection, cedenteIdCodice As String, dal As Date, al As Date) As Dictionary(Of String, List(Of ScontoRiga))
        Dim risultato As New Dictionary(Of String, List(Of ScontoRiga))

        Dim sql As String = "
            SELECT s.ID_Fattura, s.NumeroLinea, s.Tipo, s.Percentuale, s.Importo
            FROM Fatture_Sconti s
            INNER JOIN Fatture_Testate t ON t.ID_Fattura = s.ID_Fattura
            WHERE t.CedenteIdCodice = @fornitore
              AND t.DataFattura BETWEEN @dal AND @al
            ORDER BY s.ID_Fattura, s.NumeroLinea, s.ID_Sconto"

        Using cmd As New SqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@fornitore", cedenteIdCodice)
            cmd.Parameters.AddWithValue("@dal", dal)
            cmd.Parameters.AddWithValue("@al", al)

            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    Dim chiave As String = reader("ID_Fattura").ToString() & "|" & reader("NumeroLinea").ToString()

                    Dim lista As List(Of ScontoRiga) = Nothing
                    If Not risultato.TryGetValue(chiave, lista) Then
                        lista = New List(Of ScontoRiga)
                        risultato(chiave) = lista
                    End If

                    lista.Add(New ScontoRiga With {
                        .Tipo = If(IsDBNull(reader("Tipo")), "SC", reader("Tipo").ToString().Trim()),
                        .Percentuale = If(IsDBNull(reader("Percentuale")), 0D, Convert.ToDecimal(reader("Percentuale"))),
                        .Importo = If(IsDBNull(reader("Importo")), 0D, Convert.ToDecimal(reader("Importo")))
                    })
                End While
            End Using
        End Using

        Return risultato
    End Function

    ''' <summary>
    ''' Applica in cascata, nell'ordine originario del blocco XML, gli sconti/maggiorazioni di ogni
    ''' riga al PrezzoLordo fatturato per ottenere il prezzo netto realmente pagato, quindi calcola
    ''' lo scostamento rispetto al listino sul prezzo netto (non su quello lordo).
    ''' </summary>
    Private Sub ApplicaScontiEAnomalie(dt As DataTable, sconti As Dictionary(Of String, List(Of ScontoRiga)))
        dt.Columns.Add("Sconti_Applicati", GetType(String))
        dt.Columns.Add("PrezzoNetto_Fatturato", GetType(Decimal))
        dt.Columns.Add("Differenza_Unitaria", GetType(Decimal))
        dt.Columns.Add("Differenza_Totale_Riga", GetType(Decimal))
        dt.Columns.Add("Stato_Anomalia", GetType(String))

        For Each row As DataRow In dt.Rows
            Dim chiave As String = row("ID_Fattura").ToString() & "|" & row("NumeroLinea").ToString()
            Dim listaSconti As List(Of ScontoRiga) = Nothing
            sconti.TryGetValue(chiave, listaSconti)

            Dim prezzoLordo As Decimal = Convert.ToDecimal(row("PrezzoLordo"))
            Dim prezzoNetto As Decimal = prezzoLordo

            If listaSconti IsNot Nothing Then
                For Each s In listaSconti
                    Dim segno As Decimal = If(String.Equals(s.Tipo, "MG", StringComparison.OrdinalIgnoreCase), 1D, -1D)
                    If s.Percentuale <> 0D Then
                        prezzoNetto += segno * prezzoNetto * (s.Percentuale / 100D)
                    ElseIf s.Importo <> 0D Then
                        prezzoNetto += segno * s.Importo
                    End If
                Next
            End If

            row("Sconti_Applicati") = DescriviSconti(listaSconti)
            row("PrezzoNetto_Fatturato") = prezzoNetto

            If IsDBNull(row("Unitario_Netto_Listino")) Then
                row("Differenza_Unitaria") = DBNull.Value
                row("Differenza_Totale_Riga") = DBNull.Value
                row("Stato_Anomalia") = "Mancante a Listino"
            Else
                Dim prezzoListino As Decimal = Convert.ToDecimal(row("Unitario_Netto_Listino"))
                Dim quantita As Decimal = Convert.ToDecimal(row("Quantita"))
                Dim differenzaUnitaria As Decimal = prezzoNetto - prezzoListino

                row("Differenza_Unitaria") = differenzaUnitaria
                row("Differenza_Totale_Riga") = differenzaUnitaria * quantita
                row("Stato_Anomalia") = If(differenzaUnitaria > My.Settings.ScostamentoAccettabile, "Prezzo Eccessivo", "In Bolla")
            End If
        Next
    End Sub

    ''' <summary>
    ''' Genera una descrizione sintetica degli sconti/maggiorazioni applicati a una riga
    ''' (es. "SC 30% + SC 15%" oppure "MG 5,00 €").
    ''' </summary>
    Private Function DescriviSconti(listaSconti As List(Of ScontoRiga)) As String
        If listaSconti Is Nothing OrElse listaSconti.Count = 0 Then Return ""

        Dim parti As New List(Of String)
        For Each s In listaSconti
            If s.Percentuale <> 0D Then
                parti.Add($"{s.Tipo} {s.Percentuale:N2}%")
            ElseIf s.Importo <> 0D Then
                parti.Add($"{s.Tipo} {s.Importo:N2} €")
            End If
        Next

        Return String.Join(" + ", parti)
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
