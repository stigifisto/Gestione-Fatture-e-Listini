Imports IBM.Data.DB2.iSeries
Imports ExcelDataReader
Imports System.Text.RegularExpressions

Public Class FrmPromoManuale

    Private Const ConnStr As String = "DataSource=192.168.2.200;UserID=edpuser;Password=edpuser;DefaultCollection=inadati"

    Private _caricamentoForm As Boolean = False
    Private _prezzoListino As Decimal?
    Private _dtArticoliCorrente As DataTable
    Private _filtrandoArticoli As Boolean = False
    Private _righePrezzoAttuali As New List(Of RigaPrezzoEsistente)

    ' Tutte le righe della tabella prezzi hanno LIADAF impostato a questa data sentinella:
    ' il prezzo "in vigore" a una certa data si determina solo in base al LIADAI più recente
    ' non successivo alla data di riferimento (vedi CaricaPrezzoListino), non da un vero
    ' intervallo di validità. Per questo la promo si inserisce come singola riga, senza
    ' toccare le righe di listino precedenti o successive.
    Private Shared ReadOnly DataFineMassima As Date = New Date(9999, 12, 31)

    Private Sub FrmPromoManuale_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _caricamentoForm = True

        Dim listaFornitori As New List(Of Fornitore) From {
            New Fornitore("Fileni Sfuso", "00000665"),
            New Fornitore("Fileni Elaborati", "00000665"),
            New Fornitore("Fileni Panati", "00000665"),
            New Fornitore("Aia", "00000004"),
            New Fornitore("Aia MD Nord", "00000004"),
            New Fornitore("Aia MD Sud", "00000004"),
            New Fornitore("Aia Coop Sicilia", "00000004"),
            New Fornitore("Amadori", "00006572"),
            New Fornitore("Avimecc", "00001632"),
            New Fornitore("Avimecc Coop Calabria", "00001632"),
            New Fornitore("Avimecc Coop Sicilia", "00001632"),
            New Fornitore("Cinque Stelle", "00000447"),
            New Fornitore("Inalca", "00007003"),
            New Fornitore("Inalca Suino", "00007003"),
            New Fornitore("Fiorani", "00008411")
        }

        cmbFornitore.DataSource = listaFornitori
        cmbFornitore.DisplayMember = "Nome"
        cmbFornitore.ValueMember = "Codice"
        cmbFornitore.SelectedIndex = -1

        dtpDataInizio.Value = Date.Today
        dtpDataFine.Value = Date.Today

        _caricamentoForm = False
    End Sub

    Private Sub cmbFornitore_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFornitore.SelectedIndexChanged
        If _caricamentoForm Then Exit Sub

        If pnlFileEsterno.Visible Then ImpostaStatoPannelloFile(False)

        _filtrandoArticoli = True
        cmbArticoli.DataSource = Nothing
        cmbArticoli.Text = String.Empty
        _filtrandoArticoli = False
        _dtArticoliCorrente = Nothing
        txtDescrizione.Text = String.Empty
        txtCodiceAS400.Text = String.Empty
        txtUM.Text = String.Empty
        txtPrezzoListino.Text = String.Empty
        txtPercentualePromo.Text = String.Empty
        txtRiduzioneFissa.Text = String.Empty
        txtPrezzoPromo.Text = String.Empty
        txtPrezzoPromo.ReadOnly = False
        _prezzoListino = Nothing
        _righePrezzoAttuali = New List(Of RigaPrezzoEsistente)
        dgvListinoAttuale.DataSource = Nothing
        dgvListinoDopoPromo.DataSource = Nothing

        If cmbFornitore.SelectedItem Is Nothing Then Exit Sub

        Dim selFornitore As String = cmbFornitore.SelectedValue.ToString()

        Me.Cursor = Cursors.WaitCursor
        Try
            Dim dtArticoli As DataTable = CaricaArticoliAs400(selFornitore)
            dtArticoli.Columns.Add("DISPLAY", GetType(String))
            For Each row As DataRow In dtArticoli.Rows
                Dim cofcof As String = If(row("COFCOF") Is DBNull.Value, "", row("COFCOF").ToString().Trim())
                Dim cofdef As String = If(row("COFDEF") Is DBNull.Value, "", row("COFDEF").ToString().Trim())
                row("DISPLAY") = cofcof & " - " & cofdef
            Next

            _dtArticoliCorrente = dtArticoli

            _filtrandoArticoli = True
            cmbArticoli.DataSource = dtArticoli
            cmbArticoli.DisplayMember = "DISPLAY"
            cmbArticoli.ValueMember = "COFCOF"
            cmbArticoli.SelectedIndex = -1
            cmbArticoli.Text = String.Empty
            _filtrandoArticoli = False
        Catch ex As Exception
            MessageBox.Show("Errore durante il caricamento degli articoli: " & ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ' Ricerca "a contiene" sul codice o sulla descrizione articolo, per velocizzare la selezione
    ' quando il fornitore ha molti codici (elenco altrimenti troppo lungo da scorrere).
    Private Sub cmbArticoli_TextChanged(sender As Object, e As EventArgs) Handles cmbArticoli.TextChanged
        If _filtrandoArticoli OrElse _dtArticoliCorrente Is Nothing Then Exit Sub
        If cmbArticoli.SelectedIndex <> -1 Then Exit Sub

        Dim testo As String = cmbArticoli.Text.Trim()

        _filtrandoArticoli = True
        Try
            Dim posizioneCursore As Integer = cmbArticoli.SelectionStart
            Dim lunghezzaSelezione As Integer = cmbArticoli.SelectionLength

            Dim dv As New DataView(_dtArticoliCorrente)
            If testo.Length > 0 Then
                Try
                    Dim testoEscaped As String = testo.Replace("'", "''")
                    dv.RowFilter = $"COFCOF LIKE '%{testoEscaped}%' OR COFDEF LIKE '%{testoEscaped}%'"
                Catch
                    dv.RowFilter = String.Empty
                End Try
            End If

            cmbArticoli.DataSource = dv.ToTable()
            cmbArticoli.DisplayMember = "DISPLAY"
            cmbArticoli.ValueMember = "COFCOF"
            cmbArticoli.SelectedIndex = -1
            cmbArticoli.Text = testo
            cmbArticoli.SelectionStart = posizioneCursore
            cmbArticoli.SelectionLength = lunghezzaSelezione

            cmbArticoli.DroppedDown = testo.Length > 0 AndAlso dv.Count > 0
        Finally
            _filtrandoArticoli = False
        End Try
    End Sub

    Private Sub cmbArticoli_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbArticoli.SelectedIndexChanged
        Dim rowView As DataRowView = TryCast(cmbArticoli.SelectedItem, DataRowView)

        txtPercentualePromo.Text = String.Empty
        txtRiduzioneFissa.Text = String.Empty
        txtPrezzoPromo.Text = String.Empty
        txtPrezzoPromo.ReadOnly = False

        If rowView Is Nothing Then
            txtDescrizione.Text = String.Empty
            txtCodiceAS400.Text = String.Empty
            txtUM.Text = String.Empty
        Else
            txtDescrizione.Text = If(rowView("COFDEF") Is DBNull.Value, "", rowView("COFDEF").ToString().Trim())
            txtCodiceAS400.Text = If(rowView("COFCOD") Is DBNull.Value, "", rowView("COFCOD").ToString().Trim())
            txtUM.Text = If(rowView("APFUPR") Is DBNull.Value, "", rowView("APFUPR").ToString().Trim())
        End If

        AggiornaPrezzoListino()
        CaricaGrigliaAttuale()
    End Sub

    Private Sub dtpDataInizio_ValueChanged(sender As Object, e As EventArgs) Handles dtpDataInizio.ValueChanged
        If _caricamentoForm Then Exit Sub
        AggiornaPrezzoListino()
        AggiornaGrigliaDopoPromo()
    End Sub

    Private Sub dtpDataFine_ValueChanged(sender As Object, e As EventArgs) Handles dtpDataFine.ValueChanged
        If _caricamentoForm Then Exit Sub
        AggiornaGrigliaDopoPromo()
    End Sub

    Private Sub txtPrezzoPromo_TextChanged(sender As Object, e As EventArgs) Handles txtPrezzoPromo.TextChanged
        If _caricamentoForm Then Exit Sub
        AggiornaGrigliaDopoPromo()
    End Sub

    ' Recupera l'ultimo prezzo di listino dell'articolo antecedente (o uguale) alla data inizio promo:
    ' la percentuale va calcolata sul prezzo in vigore a quella data, non sull'ultimo in assoluto.
    Private Sub AggiornaPrezzoListino()
        _prezzoListino = Nothing
        txtPrezzoListino.Text = String.Empty

        If cmbFornitore.SelectedItem Is Nothing OrElse cmbArticoli.SelectedItem Is Nothing OrElse
           String.IsNullOrWhiteSpace(txtCodiceAS400.Text) Then
            RicalcolaPrezzoPromo()
            Exit Sub
        End If

        Dim fornitoreSelezionato As Fornitore = DirectCast(cmbFornitore.SelectedItem, Fornitore)
        Dim liafor As String = fornitoreSelezionato.Codice
        Dim liacod As String = txtCodiceAS400.Text.Trim()
        Dim liauma As String = txtUM.Text.Trim()
        Dim lialis As String = DeterminaLialis(fornitoreSelezionato)

        Me.Cursor = Cursors.WaitCursor
        Try
            _prezzoListino = CaricaPrezzoListino(liafor, liacod, liauma, lialis, dtpDataInizio.Value.Date)
            If _prezzoListino.HasValue Then
                txtPrezzoListino.Text = _prezzoListino.Value.ToString("0.0000", Globalization.CultureInfo.InvariantCulture)
            End If
        Catch ex As Exception
            MessageBox.Show("Errore durante il recupero del prezzo di listino: " & ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try

        RicalcolaPrezzoPromo()
    End Sub

    Private Function CaricaPrezzoListino(liafor As String, liacod As String, liauma As String, lialis As String, dataRiferimento As Date) As Decimal?
        Dim sql As String =
            "SELECT liaprz FROM PJVARGRD.OALIA£2L" &
            " WHERE LIASOC = 'GRD' AND LIAFOR = @LIAFOR AND LIACOD = @LIACOD AND LIAUMA = @LIAUMA" &
            " AND LIALIS = @LIALIS AND LIAVAL = 'EUR ' AND LIADAI <= @DATARIF" &
            " ORDER BY liadai DESC FETCH FIRST 1 ROWS ONLY"

        Using conn As New iDB2Connection(ConnStr)
            conn.Open()
            Using cmd As New iDB2Command(sql, conn)
                cmd.Parameters.AddWithValue("@LIAFOR", liafor)
                cmd.Parameters.AddWithValue("@LIACOD", liacod)
                cmd.Parameters.AddWithValue("@LIAUMA", liauma)
                cmd.Parameters.AddWithValue("@LIALIS", lialis)
                cmd.Parameters.AddWithValue("@DATARIF", dataRiferimento.ToString("yyyy-MM-dd"))
                Dim risultato As Object = cmd.ExecuteScalar()
                If risultato Is Nothing OrElse risultato Is DBNull.Value Then Return Nothing
                Return Convert.ToDecimal(risultato)
            End Using
        End Using
    End Function

    ' Rappresenta una riga prezzo/listino esistente per l'articolo selezionato.
    Private Class RigaPrezzoEsistente
        Public Property Liaunt As String
        Public Property Lialis As String
        Public Property Liaval As String
        Public Property Liaclv As String
        Public Property Liaprz As Decimal
        Public Property Liasc1 As Decimal
        Public Property Liasc2 As Decimal
        Public Property Liasc3 As Decimal
        Public Property Liasc4 As Decimal
        Public Property Liascq As Decimal
        Public Property Liadai As Date
        Public Property Liadaf As Date
        Public Property Liafil As Integer
        Public Property Liamag As String
    End Class

    ' Carica per intero lo storico dei prezzi/listini dell'articolo (nessun filtro sul periodo),
    ' usato per popolare la griglia "situazione attuale" nel form.
    Private Function CaricaTutteLeRighePrezzo(liafor As String, liacod As String, liauma As String, lialis As String) As List(Of RigaPrezzoEsistente)
        Dim risultato As New List(Of RigaPrezzoEsistente)

        Dim sql As String =
            "SELECT liaunt, lialis, liaval, liaclv, liaprz, liasc1, liasc2, liasc3, liasc4, liascq, liadai, liadaf, liafil, liamag" &
            " FROM PJVARGRD.OALIA£2L" &
            " WHERE LIASOC = 'GRD' AND LIAFOR = @LIAFOR AND LIACOD = @LIACOD AND LIAUMA = @LIAUMA" &
            " AND LIALIS = @LIALIS AND LIAVAL = 'EUR '" &
            " ORDER BY liadai DESC"

        Using conn As New iDB2Connection(ConnStr)
            conn.Open()
            Using cmd As New iDB2Command(sql, conn)
                cmd.Parameters.AddWithValue("@LIAFOR", liafor)
                cmd.Parameters.AddWithValue("@LIACOD", liacod)
                cmd.Parameters.AddWithValue("@LIAUMA", liauma)
                cmd.Parameters.AddWithValue("@LIALIS", lialis)
                Using reader As iDB2DataReader = cmd.ExecuteReader()
                    While reader.Read()
                        risultato.Add(LeggiRigaPrezzoDaReader(reader))
                    End While
                End Using
            End Using
        End Using

        Return risultato
    End Function

    Private Function LeggiRigaPrezzoDaReader(reader As iDB2DataReader) As RigaPrezzoEsistente
        Return New RigaPrezzoEsistente With {
            .Liaunt = reader("liaunt").ToString(),
            .Lialis = reader("lialis").ToString(),
            .Liaval = reader("liaval").ToString(),
            .Liaclv = reader("liaclv").ToString(),
            .Liaprz = Convert.ToDecimal(reader("liaprz")),
            .Liasc1 = Convert.ToDecimal(reader("liasc1")),
            .Liasc2 = Convert.ToDecimal(reader("liasc2")),
            .Liasc3 = Convert.ToDecimal(reader("liasc3")),
            .Liasc4 = Convert.ToDecimal(reader("liasc4")),
            .Liascq = Convert.ToDecimal(reader("liascq")),
            .Liadai = Convert.ToDateTime(reader("liadai")),
            .Liadaf = Convert.ToDateTime(reader("liadaf")),
            .Liafil = Convert.ToInt32(reader("liafil")),
            .Liamag = reader("liamag").ToString()
        }
    End Function

    ' Calcola in memoria (nessun accesso al DB) come apparirebbe il listino dopo l'inserimento
    ' della promo, per l'anteprima nella griglia "situazione dopo la promo": aggiunge la nuova
    ' riga promo e, come SpostaLiadaiRigheSuccessiveAPromo, posticipa l'inizio validità di
    ' un'eventuale riga di listino che cade dentro il periodo della promo. Se nessuna riga cade
    ' dentro il periodo della promo, il prezzo di listino precedente non verrebbe mai ripristinato
    ' (vince sempre il LIADAI più recente non successivo alla data, vedi CaricaPrezzoListino):
    ' va quindi aggiunta anche una riga di "ripristino" del prezzo pre-promo, valida dal giorno
    ' successivo alla fine della promo.
    Private Function CalcolaRigheDopoPromo(righeEsistenti As List(Of RigaPrezzoEsistente),
                                            liadaiPromo As Date, liadafPromo As Date,
                                            prezzoPromo As Decimal, lialisPromo As String) As List(Of RigaPrezzoEsistente)
        Dim risultato As New List(Of RigaPrezzoEsistente)
        Dim esisteRigaSpostata As Boolean = False

        For Each riga As RigaPrezzoEsistente In righeEsistenti
            If riga.Liadai >= liadaiPromo AndAlso riga.Liadai <= liadafPromo Then
                esisteRigaSpostata = True
                risultato.Add(New RigaPrezzoEsistente With {
                    .Liaunt = riga.Liaunt,
                    .Lialis = riga.Lialis,
                    .Liaval = riga.Liaval,
                    .Liaclv = riga.Liaclv,
                    .Liaprz = riga.Liaprz,
                    .Liasc1 = riga.Liasc1,
                    .Liasc2 = riga.Liasc2,
                    .Liasc3 = riga.Liasc3,
                    .Liasc4 = riga.Liasc4,
                    .Liascq = riga.Liascq,
                    .Liadai = liadafPromo.AddDays(1),
                    .Liadaf = riga.Liadaf,
                    .Liafil = riga.Liafil,
                    .Liamag = riga.Liamag
                })
            Else
                risultato.Add(riga)
            End If
        Next

        risultato.Add(New RigaPrezzoEsistente With {
            .Liaunt = String.Empty,
            .Lialis = lialisPromo,
            .Liaval = "EUR ",
            .Liaclv = String.Empty,
            .Liaprz = prezzoPromo,
            .Liasc1 = 0D,
            .Liasc2 = 0D,
            .Liasc3 = 0D,
            .Liasc4 = 0D,
            .Liascq = 0D,
            .Liadai = liadaiPromo,
            .Liadaf = DataFineMassima,
            .Liafil = 0,
            .Liamag = String.Empty
        })

        If Not esisteRigaSpostata Then
            Dim prezzoPrimaPromo As Decimal? = PrezzoInVigoreA(righeEsistenti, liadaiPromo)
            If prezzoPrimaPromo.HasValue AndAlso prezzoPrimaPromo.Value <> prezzoPromo Then
                risultato.Add(New RigaPrezzoEsistente With {
                    .Liaunt = String.Empty,
                    .Lialis = lialisPromo,
                    .Liaval = "EUR ",
                    .Liaclv = String.Empty,
                    .Liaprz = prezzoPrimaPromo.Value,
                    .Liasc1 = 0D,
                    .Liasc2 = 0D,
                    .Liasc3 = 0D,
                    .Liasc4 = 0D,
                    .Liascq = 0D,
                    .Liadai = liadafPromo.AddDays(1),
                    .Liadaf = DataFineMassima,
                    .Liafil = 0,
                    .Liamag = String.Empty
                })
            End If
        End If

        risultato.Sort(Function(a, b) b.Liadai.CompareTo(a.Liadai))
        Return risultato
    End Function

    ' Replica in memoria la stessa logica di CaricaPrezzoListino (ultimo LIADAI non successivo
    ' alla data di riferimento) sulle righe già caricate, per non dover riaccedere al DB.
    Private Function PrezzoInVigoreA(righeEsistenti As List(Of RigaPrezzoEsistente), dataRiferimento As Date) As Decimal?
        Dim riferimento As RigaPrezzoEsistente = Nothing
        For Each riga As RigaPrezzoEsistente In righeEsistenti
            If riga.Liadai <= dataRiferimento Then
                If riferimento Is Nothing OrElse riga.Liadai > riferimento.Liadai Then
                    riferimento = riga
                End If
            End If
        Next
        If riferimento Is Nothing Then Return Nothing
        Return riferimento.Liaprz
    End Function

    Private Function CreaTabellaVisualizzazionePrezzi(righe As List(Of RigaPrezzoEsistente)) As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Listino", GetType(String))
        dt.Columns.Add("Dal", GetType(String))
        dt.Columns.Add("Al", GetType(String))
        dt.Columns.Add("Prezzo", GetType(String))

        For Each riga As RigaPrezzoEsistente In righe
            Dim alTesto As String = If(riga.Liadaf.Year >= 9999, "in corso", riga.Liadaf.ToString("dd/MM/yyyy"))
            dt.Rows.Add(riga.Lialis, riga.Liadai.ToString("dd/MM/yyyy"), alTesto,
                       riga.Liaprz.ToString("N4", Globalization.CultureInfo.InvariantCulture))
        Next

        Return dt
    End Function

    ' Ricarica dal DB lo storico prezzi dell'articolo selezionato per la griglia "situazione attuale"
    ' e aggiorna di conseguenza anche l'anteprima "situazione dopo la promo".
    Private Sub CaricaGrigliaAttuale()
        _righePrezzoAttuali = New List(Of RigaPrezzoEsistente)
        dgvListinoAttuale.DataSource = Nothing
        dgvListinoDopoPromo.DataSource = Nothing

        If cmbFornitore.SelectedItem Is Nothing OrElse cmbArticoli.SelectedItem Is Nothing OrElse
           String.IsNullOrWhiteSpace(txtCodiceAS400.Text) Then
            Exit Sub
        End If

        Dim fornitoreSelezionato As Fornitore = DirectCast(cmbFornitore.SelectedItem, Fornitore)
        Dim liafor As String = fornitoreSelezionato.Codice
        Dim liacod As String = txtCodiceAS400.Text.Trim()
        Dim liauma As String = txtUM.Text.Trim()
        Dim lialis As String = DeterminaLialis(fornitoreSelezionato)

        Me.Cursor = Cursors.WaitCursor
        Try
            _righePrezzoAttuali = CaricaTutteLeRighePrezzo(liafor, liacod, liauma, lialis)
            dgvListinoAttuale.DataSource = CreaTabellaVisualizzazionePrezzi(_righePrezzoAttuali)
        Catch ex As Exception
            MessageBox.Show("Errore durante il caricamento dello storico prezzi: " & ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try

        AggiornaGrigliaDopoPromo()
    End Sub

    ' Ricalcola in memoria (senza toccare il DB) come apparirebbe il listino se la promo venisse
    ' salvata con i valori correnti del form, e aggiorna la griglia "situazione dopo la promo".
    Private Sub AggiornaGrigliaDopoPromo()
        dgvListinoDopoPromo.DataSource = Nothing

        If cmbFornitore.SelectedItem Is Nothing OrElse cmbArticoli.SelectedItem Is Nothing Then Exit Sub
        If dtpDataFine.Value.Date < dtpDataInizio.Value.Date Then Exit Sub

        Dim prezzoPromo As Decimal
        If Not Decimal.TryParse(txtPrezzoPromo.Text.Trim().Replace(",", "."), Globalization.NumberStyles.Number,
                                Globalization.CultureInfo.InvariantCulture, prezzoPromo) OrElse prezzoPromo <= 0 Then
            Exit Sub
        End If

        Dim fornitoreSelezionato As Fornitore = DirectCast(cmbFornitore.SelectedItem, Fornitore)
        Dim lialisPromo As String = DeterminaLialis(fornitoreSelezionato)

        Dim righeDopo As List(Of RigaPrezzoEsistente) =
            CalcolaRigheDopoPromo(_righePrezzoAttuali, dtpDataInizio.Value.Date, dtpDataFine.Value.Date, prezzoPromo, lialisPromo)

        dgvListinoDopoPromo.DataSource = CreaTabellaVisualizzazionePrezzi(righeDopo)
    End Sub

    Private Function DeterminaLialis(fornitoreSelezionato As Fornitore) As String
        Select Case fornitoreSelezionato.Nome
            Case "Aia MD Nord"
                Return "02"
            Case "Aia MD Sud"
                Return "03"
            Case "Aia Coop Sicilia"
                Return "04"
            Case "Avimecc Coop Calabria"
                Return "05"
            Case "Avimecc Coop Sicilia"
                Return "04"
            Case Else
                Return "01"
        End Select
    End Function

    ' Se esiste già una riga di listino "normale" con LIADAI compreso nel periodo della promo
    ' (es. un cambio prezzo comunicato per una data che ricade dentro la promo), quella riga
    ' andrebbe erroneamente scelta al posto della promo per i giorni finali del periodo promo
    ' (vince sempre il LIADAI più recente non successivo alla data). Si posticipa quindi il suo
    ' inizio validità al giorno successivo alla fine della promo.
    Private Function SpostaLiadaiRigheSuccessiveAPromo(conn As iDB2Connection, tran As iDB2Transaction,
                                                   liafor As String, liacod As String, liauma As String, lialis As String,
                                                   liadaiPromo As Date, liadafPromo As Date) As Boolean
        Dim sql As String =
            "UPDATE PJVARGRD.OALIA£2L SET LIADAI = @NUOVOLIADAI" &
            " WHERE LIASOC = 'GRD' AND LIAFOR = @LIAFOR AND LIACOD = @LIACOD AND LIAUMA = @LIAUMA" &
            " AND LIALIS = @LIALIS AND LIAVAL = 'EUR '" &
            " AND LIADAI >= @LIADAIPROMO AND LIADAI <= @LIADAFPROMO"

        Using cmd As New iDB2Command(sql, conn)
            cmd.Transaction = tran
            cmd.Parameters.AddWithValue("@NUOVOLIADAI", liadafPromo.AddDays(1).ToString("yyyy-MM-dd"))
            cmd.Parameters.AddWithValue("@LIAFOR", liafor)
            cmd.Parameters.AddWithValue("@LIACOD", liacod)
            cmd.Parameters.AddWithValue("@LIAUMA", liauma)
            cmd.Parameters.AddWithValue("@LIALIS", lialis)
            cmd.Parameters.AddWithValue("@LIADAIPROMO", liadaiPromo.ToString("yyyy-MM-dd"))
            cmd.Parameters.AddWithValue("@LIADAFPROMO", liadafPromo.ToString("yyyy-MM-dd"))
            Return cmd.ExecuteNonQuery() > 0
        End Using
    End Function

    Private Sub InserisciRigaPrezzo(conn As iDB2Connection, tran As iDB2Transaction,
                                     liaunt As String, lialis As String, liaval As String, liafor As String,
                                     liaclv As String, liacod As String, liauma As String, liaprz As Decimal,
                                     liasc1 As Decimal, liasc2 As Decimal, liasc3 As Decimal, liasc4 As Decimal, liascq As Decimal,
                                     liadai As Date, liadaf As Date, liafil As Integer, liamag As String)
        Dim sql As String =
            "INSERT INTO PJVARGRD.OALIA£2L " &
            "(LIASOC, LIAUNT, LIALIS, LIAVAL, LIAFOR, LIACLV, LIACOD, LIAUMA, LIAPRZ, " &
            " LIASC1, LIASC2, LIASC3, LIASC4, LIASCQ, LIADAI, LIADAF, LIAFIL, LIAMAG) " &
            "VALUES (@LIASOC, @LIAUNT, @LIALIS, @LIAVAL, @LIAFOR, @LIACLV, @LIACOD, @LIAUMA, @LIAPRZ, " &
            "        @LIASC1, @LIASC2, @LIASC3, @LIASC4, @LIASCQ, @LIADAI, @LIADAF, @LIAFIL, @LIAMAG)"

        Using cmd As New iDB2Command(sql, conn)
            cmd.Transaction = tran
            cmd.Parameters.AddWithValue("@LIASOC", "GRD")
            cmd.Parameters.AddWithValue("@LIAUNT", liaunt)
            cmd.Parameters.AddWithValue("@LIALIS", lialis)
            cmd.Parameters.AddWithValue("@LIAVAL", liaval)
            cmd.Parameters.AddWithValue("@LIAFOR", liafor)
            cmd.Parameters.AddWithValue("@LIACLV", liaclv)
            cmd.Parameters.AddWithValue("@LIACOD", liacod)
            cmd.Parameters.AddWithValue("@LIAUMA", liauma)
            cmd.Parameters.AddWithValue("@LIAPRZ", liaprz)
            cmd.Parameters.AddWithValue("@LIASC1", liasc1)
            cmd.Parameters.AddWithValue("@LIASC2", liasc2)
            cmd.Parameters.AddWithValue("@LIASC3", liasc3)
            cmd.Parameters.AddWithValue("@LIASC4", liasc4)
            cmd.Parameters.AddWithValue("@LIASCQ", liascq)
            cmd.Parameters.AddWithValue("@LIADAI", liadai.ToString("yyyy-MM-dd"))
            cmd.Parameters.AddWithValue("@LIADAF", liadaf.ToString("yyyy-MM-dd"))
            cmd.Parameters.AddWithValue("@LIAFIL", liafil)
            cmd.Parameters.AddWithValue("@LIAMAG", liamag)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' Percentuale di sconto e riduzione fissa sono due modalità alternative per calcolare il prezzo
    ' promo a partire dal prezzo di listino: compilarne una svuota automaticamente l'altra.
    Private Sub txtPercentualePromo_TextChanged(sender As Object, e As EventArgs) Handles txtPercentualePromo.TextChanged
        If Not String.IsNullOrWhiteSpace(txtPercentualePromo.Text) AndAlso Not String.IsNullOrWhiteSpace(txtRiduzioneFissa.Text) Then
            txtRiduzioneFissa.Text = String.Empty
        End If
        RicalcolaPrezzoPromo()
    End Sub

    Private Sub txtRiduzioneFissa_TextChanged(sender As Object, e As EventArgs) Handles txtRiduzioneFissa.TextChanged
        If Not String.IsNullOrWhiteSpace(txtRiduzioneFissa.Text) AndAlso Not String.IsNullOrWhiteSpace(txtPercentualePromo.Text) Then
            txtPercentualePromo.Text = String.Empty
        End If
        RicalcolaPrezzoPromo()
    End Sub

    Private Sub RicalcolaPrezzoPromo()
        If Not String.IsNullOrWhiteSpace(txtPercentualePromo.Text) Then
            Dim percentuale As Decimal
            If Not Decimal.TryParse(txtPercentualePromo.Text.Trim().Replace(",", "."), Globalization.NumberStyles.Number,
                                    Globalization.CultureInfo.InvariantCulture, percentuale) Then
                Exit Sub
            End If

            txtPrezzoPromo.ReadOnly = True

            If _prezzoListino Is Nothing Then
                txtPrezzoPromo.Text = String.Empty
                Exit Sub
            End If

            Dim prezzoCalcolato As Decimal = _prezzoListino.Value * (1D - percentuale / 100D)
            If prezzoCalcolato < 0D Then prezzoCalcolato = 0D
            txtPrezzoPromo.Text = prezzoCalcolato.ToString("0.0000", Globalization.CultureInfo.InvariantCulture)
        ElseIf Not String.IsNullOrWhiteSpace(txtRiduzioneFissa.Text) Then
            Dim riduzione As Decimal
            If Not Decimal.TryParse(txtRiduzioneFissa.Text.Trim().Replace(",", "."), Globalization.NumberStyles.Number,
                                    Globalization.CultureInfo.InvariantCulture, riduzione) Then
                Exit Sub
            End If

            txtPrezzoPromo.ReadOnly = True

            If _prezzoListino Is Nothing Then
                txtPrezzoPromo.Text = String.Empty
                Exit Sub
            End If

            Dim prezzoCalcolato As Decimal = _prezzoListino.Value - riduzione
            If prezzoCalcolato < 0D Then prezzoCalcolato = 0D
            txtPrezzoPromo.Text = prezzoCalcolato.ToString("0.0000", Globalization.CultureInfo.InvariantCulture)
        Else
            txtPrezzoPromo.ReadOnly = False
        End If
    End Sub

    ' Consente solo cifre e un unico separatore decimale (. oppure ,) in txtPrezzoPromo, txtPercentualePromo e txtRiduzioneFissa.
    Private Sub TxtNumerico_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtPrezzoPromo.KeyPress, txtPercentualePromo.KeyPress, txtRiduzioneFissa.KeyPress
        Dim tb As TextBox = DirectCast(sender, TextBox)

        If Char.IsControl(e.KeyChar) Then Exit Sub

        If Char.IsDigit(e.KeyChar) Then Exit Sub

        If (e.KeyChar = "," OrElse e.KeyChar = ".") AndAlso Not tb.Text.Contains(",") AndAlso Not tb.Text.Contains(".") Then
            Exit Sub
        End If

        e.Handled = True
    End Sub

    ' Stessa query di FrmEstrattore.QueryAs400, senza il filtro sui codici Excel:
    ' qui serve l'elenco completo degli articoli (COFCOF) del fornitore selezionato.
    Private Function CaricaArticoliAs400(selFornitore As String) As DataTable
        Dim dt As New DataTable()

        Dim sql As String =
            "WITH PrezziNumerati AS (" &
            "  SELECT liasoc, liafor, liacod, liauma, liadai," &
            "    ROW_NUMBER() OVER(PARTITION BY LIASOC, LIAFOR, LIACOD, liauma ORDER BY liadai DESC) AS RigaUM" &
            "  FROM PJVARGRD.OALIA£2L" &
            ")" &
            " SELECT cofcof, coffor, cofcod, cofdef, apfupr, liadai" &
            " FROM INADATI.OACOF00F LEFT JOIN inadati.anapf00f on cofcod=apfcod and cofsoc=apfsoc and coffor = apffor " &
            " LEFT JOIN PrezziNumerati ON cofcod = liacod AND cofsoc = liasoc AND coffor = liafor AND rigaum = 1" &
            " WHERE COFSOC='GRD' and cofcod between '000001' and '999999' AND COFFOR = @COFFOR ORDER BY cofcof"

        Using conn As New iDB2Connection(ConnStr)
            conn.Open()
            Using cmd As New iDB2Command(sql, conn)
                cmd.Parameters.AddWithValue("@COFFOR", selFornitore)
                Using adapter As New iDB2DataAdapter(cmd)
                    adapter.Fill(dt)
                End Using
            End Using
        End Using

        Return dt
    End Function

    ' Una riga articolo/prezzo individuata nel file Excel di conferma promo del fornitore.
    Private Class RigaPromoExcel
        Public Property Codice As String
        Public Property Descrizione As String
        Public Property Prezzo As Decimal
    End Class

    Private Const CartellaDefaultFilePromo As String = "C:\appoggio\"

    ' Geometria originale (da Designer.vb) usata per ridimensionare dinamicamente il form
    ' quando si apre/chiude il pannello di import da file (vedi ImpostaStatoPannelloFile).
    Private Const LarghezzaOriginaleForm As Integer = 1120
    Private Const AltezzaOriginaleForm As Integer = 485
    Private Const TopOriginaleBottoni As Integer = 410
    Private Const AltezzaOriginaleGriglie As Integer = 400
    Private Const AltezzaPannelloFile As Integer = 500
    Private Const DeltaAltezzaPannelloFile As Integer = AltezzaPannelloFile - (TopOriginaleBottoni - 150)

    ' Mostra/nasconde il pannello di import da file (pnlFileEsterno, che copre l'area da cmbArticoli
    ' a txtRiduzioneFissa) e ridimensiona di conseguenza form, bottoni e griglie di anteprima, dato che
    ' il pannello è più alto dell'area che ricopre.
    Private Sub ImpostaStatoPannelloFile(mostra As Boolean)
        pnlFileEsterno.Visible = mostra
        If mostra Then pnlFileEsterno.BringToFront()

        Dim deltaEffettivo As Integer = If(mostra, DeltaAltezzaPannelloFile, 0)
        cmdSalvaPromo.Top = TopOriginaleBottoni + deltaEffettivo
        cmdAnnullaPromo.Top = TopOriginaleBottoni + deltaEffettivo
        dgvListinoAttuale.Height = AltezzaOriginaleGriglie + deltaEffettivo
        dgvListinoDopoPromo.Height = AltezzaOriginaleGriglie + deltaEffettivo
        Me.ClientSize = New Drawing.Size(LarghezzaOriginaleForm, AltezzaOriginaleForm + deltaEffettivo)

        cmdFile.Text = If(mostra, "CHIUDI ELENCO FILE", "CARICA DA FILE")
    End Sub

    ' Legge il file di conferma promo (XLS legacy oppure XLSX, es. "AIA - OS 17 (MD) - CONFERMA PROMO-156.xls"),
    ' individua tutti gli articoli/prezzi presenti e le date della promo (vedi LeggiFilePromoExcel), e mostra
    ' il pannello di import con l'anteprima di ciascuna riga; scorrendo le righe del pannello si aggiornano
    ' gli stessi controlli dell'inserimento manuale (vedi dgvRigheFile_SelectionChanged). cmdFile funziona
    ' come interruttore ON/OFF: se il pannello è già aperto lo richiude senza riaprire la dialog file.
    Private Sub cmdFile_Click(sender As Object, e As EventArgs) Handles cmdFile.Click
        If pnlFileEsterno.Visible Then
            ImpostaStatoPannelloFile(False)
            Exit Sub
        End If

        If cmbFornitore.SelectedItem Is Nothing Then
            MessageBox.Show("Seleziona prima il fornitore.")
            Exit Sub
        End If

        Dim ofd As New OpenFileDialog() With {
            .Filter = "File Excel|*.xls;*.xlsx",
            .Title = "Seleziona file conferma promozione"
        }
        If IO.Directory.Exists(CartellaDefaultFilePromo) Then
            ofd.InitialDirectory = CartellaDefaultFilePromo
        End If

        If ofd.ShowDialog() <> DialogResult.OK Then Exit Sub

        Me.Cursor = Cursors.WaitCursor
        Try
            Dim righeExcel As List(Of RigaPromoExcel) = Nothing
            Dim dataInizioExcel As Date = Date.MinValue
            Dim dataFineExcel As Date = Date.MinValue
            Dim dateTrovate As Boolean = False

            LeggiFilePromoExcel(ofd.FileName, righeExcel, dataInizioExcel, dataFineExcel, dateTrovate)

            If righeExcel.Count = 0 Then
                MessageBox.Show("Non è stato possibile individuare, nel file selezionato, righe con codice e prezzo valorizzati.")
                Exit Sub
            End If

            If dateTrovate Then
                dtpDataInizio.Value = dataInizioExcel
                dtpDataFine.Value = dataFineExcel
            Else
                MessageBox.Show("Non è stato possibile individuare le date della promozione nel file: verificale manualmente.")
            End If

            Dim fornitoreSelezionato As Fornitore = DirectCast(cmbFornitore.SelectedItem, Fornitore)
            Dim codiciNonTrovati As List(Of String) = Nothing
            Dim tabellaImport As DataTable = CostruisciTabellaImportazione(righeExcel, fornitoreSelezionato, dtpDataInizio.Value.Date, codiciNonTrovati)

            If codiciNonTrovati.Count > 0 Then
                MessageBox.Show("I seguenti codici del file non sono stati trovati tra gli articoli del fornitore " &
                                fornitoreSelezionato.Nome & " e sono stati esclusi:" & vbCrLf & String.Join(", ", codiciNonTrovati))
            End If

            If tabellaImport.Rows.Count = 0 Then
                MessageBox.Show("Nessuno dei codici presenti nel file corrisponde a un articolo del fornitore selezionato.")
                Exit Sub
            End If

            dgvRigheFile.DataSource = tabellaImport
            dgvRigheFile.Columns("COFCOF").Visible = False
            dgvRigheFile.Columns("PrezzoExcelRaw").Visible = False

            ImpostaStatoPannelloFile(True)
        Catch ex As Exception
            MessageBox.Show("Errore durante la lettura del file: " & ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ' Analoga a NormalizzaCodice in FrmEstrattore (lì limitata a Elaborati/Panati): i file di tutti i
    ' fornitori Fileni riportano i codici articolo con il prefisso "PRD" (es. "PRD111522"), che invece
    ' non compare nel COFCOF su AS400 (es. "111522"): va quindi tolto prima di cercare/interrogare il listino.
    Private Function NormalizzaCodiceFileni(codice As String, fornitoreSelezionato As Fornitore) As String
        Dim isFileni As Boolean = fornitoreSelezionato.Nome = "Fileni Sfuso" OrElse
                                   fornitoreSelezionato.Nome = "Fileni Elaborati" OrElse
                                   fornitoreSelezionato.Nome = "Fileni Panati"
        If isFileni AndAlso codice.StartsWith("PRD", StringComparison.OrdinalIgnoreCase) Then
            Return codice.Substring(3)
        End If
        Return codice
    End Function

    ' Per ciascuna riga letta dal file Excel, cerca il corrispondente articolo tra quelli del fornitore
    ' selezionato (_dtArticoliCorrente) e calcola le colonne di anteprima (prezzo di listino alla data di
    ' riferimento, promo % e riduzione fissa equivalenti al prezzo del file). I codici non trovati vengono
    ' esclusi e restituiti in codiciNonTrovati. Le colonne COFCOF e PrezzoExcelRaw sono nascoste in griglia
    ' e servono solo a dgvRigheFile_SelectionChanged per ripristinare i controlli del form.
    Private Function CostruisciTabellaImportazione(righeExcel As List(Of RigaPromoExcel), fornitoreSelezionato As Fornitore,
                                                     dataRiferimento As Date, ByRef codiciNonTrovati As List(Of String)) As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Descrizione", GetType(String))
        dt.Columns.Add("Codice AS400", GetType(String))
        dt.Columns.Add("UM", GetType(String))
        dt.Columns.Add("Prezzo Listino", GetType(String))
        dt.Columns.Add("Promo % su listino", GetType(String))
        dt.Columns.Add("Prezzo Promozionale", GetType(String))
        dt.Columns.Add("Riduzione Fissa", GetType(String))
        dt.Columns.Add("COFCOF", GetType(String))
        dt.Columns.Add("PrezzoExcelRaw", GetType(Decimal))

        codiciNonTrovati = New List(Of String)

        Dim liafor As String = fornitoreSelezionato.Codice
        Dim lialis As String = DeterminaLialis(fornitoreSelezionato)

        For Each rigaExcel As RigaPromoExcel In righeExcel
            Dim codiceNormalizzato As String = NormalizzaCodiceFileni(rigaExcel.Codice, fornitoreSelezionato)
            Dim rigaArticolo As DataRow = TrovaRigaArticoloPerCodice(codiceNormalizzato)
            If rigaArticolo Is Nothing Then
                codiciNonTrovati.Add(rigaExcel.Codice)
                Continue For
            End If

            Dim cofcof As String = rigaArticolo("COFCOF").ToString().Trim()
            Dim liacod As String = If(rigaArticolo("COFCOD") Is DBNull.Value, "", rigaArticolo("COFCOD").ToString().Trim())
            Dim liauma As String = If(rigaArticolo("APFUPR") Is DBNull.Value, "", rigaArticolo("APFUPR").ToString().Trim())
            Dim descrizione As String = If(rigaArticolo("COFDEF") Is DBNull.Value, "", rigaArticolo("COFDEF").ToString().Trim())

            Dim prezzoListino As Decimal? = Nothing
            If Not String.IsNullOrWhiteSpace(liacod) Then
                prezzoListino = CaricaPrezzoListino(liafor, liacod, liauma, lialis, dataRiferimento)
            End If

            Dim testoPrezzoListino As String = String.Empty
            Dim testoPercentuale As String = String.Empty
            Dim testoRiduzione As String = String.Empty
            If prezzoListino.HasValue Then
                testoPrezzoListino = prezzoListino.Value.ToString("N4", Globalization.CultureInfo.InvariantCulture)
                If prezzoListino.Value <> 0D Then
                    Dim percentuale As Decimal = (1D - rigaExcel.Prezzo / prezzoListino.Value) * 100D
                    testoPercentuale = percentuale.ToString("N2", Globalization.CultureInfo.InvariantCulture)
                End If
                testoRiduzione = (prezzoListino.Value - rigaExcel.Prezzo).ToString("N4", Globalization.CultureInfo.InvariantCulture)
            End If

            dt.Rows.Add(descrizione, liacod, liauma, testoPrezzoListino, testoPercentuale,
                        rigaExcel.Prezzo.ToString("N4", Globalization.CultureInfo.InvariantCulture), testoRiduzione,
                        cofcof, rigaExcel.Prezzo)
        Next

        Return dt
    End Function

    ' Selezionando una riga nel pannello di import, si pilotano gli stessi controlli/handler usati per
    ' l'inserimento manuale (cmbArticoli, txtPrezzoPromo): nessuna logica di calcolo/anteprima duplicata.
    Private Sub dgvRigheFile_SelectionChanged(sender As Object, e As EventArgs) Handles dgvRigheFile.SelectionChanged
        If dgvRigheFile.CurrentRow Is Nothing Then Exit Sub

        Dim rigaSelezionata As DataRowView = TryCast(dgvRigheFile.CurrentRow.DataBoundItem, DataRowView)
        If rigaSelezionata Is Nothing Then Exit Sub

        Dim cofcof As String = rigaSelezionata("COFCOF").ToString()
        Dim prezzoExcel As Decimal = Convert.ToDecimal(rigaSelezionata("PrezzoExcelRaw"))

        Dim indiceArticolo As Integer = TrovaIndiceArticoloPerCodice(cofcof)
        If indiceArticolo = -1 Then Exit Sub

        cmbArticoli.SelectedIndex = indiceArticolo

        txtPercentualePromo.Text = String.Empty
        txtRiduzioneFissa.Text = String.Empty
        txtPrezzoPromo.Text = prezzoExcel.ToString("0.0000", Globalization.CultureInfo.InvariantCulture)
    End Sub

    ' Cerca, tra gli articoli già caricati per il fornitore selezionato (_dtArticoliCorrente), quello il
    ' cui COFCOF corrisponde al codice letto dal file Excel, e ripristina la lista completa (non filtrata)
    ' nella combo in modo che l'indice trovato sia valido.
    Private Function TrovaIndiceArticoloPerCodice(codice As String) As Integer
        If _dtArticoliCorrente Is Nothing Then Return -1

        _filtrandoArticoli = True
        Try
            cmbArticoli.DataSource = _dtArticoliCorrente
            cmbArticoli.DisplayMember = "DISPLAY"
            cmbArticoli.ValueMember = "COFCOF"
            cmbArticoli.SelectedIndex = -1
        Finally
            _filtrandoArticoli = False
        End Try

        Dim codiceTrim As String = codice.Trim()
        Dim codiceSenzaZeri As String = codiceTrim.TrimStart("0"c)

        For i As Integer = 0 To _dtArticoliCorrente.Rows.Count - 1
            Dim cofcof As String = If(_dtArticoliCorrente.Rows(i)("COFCOF") Is DBNull.Value, "", _dtArticoliCorrente.Rows(i)("COFCOF").ToString().Trim())
            If String.Equals(cofcof, codiceTrim, StringComparison.OrdinalIgnoreCase) OrElse
               (codiceSenzaZeri.Length > 0 AndAlso String.Equals(cofcof.TrimStart("0"c), codiceSenzaZeri, StringComparison.OrdinalIgnoreCase)) Then
                Return i
            End If
        Next

        Return -1
    End Function

    ' Come TrovaIndiceArticoloPerCodice ma senza toccare il DataSource di cmbArticoli: usata per
    ' costruire la griglia di anteprima import (CostruisciTabellaImportazione), dove serve solo
    ' consultare i dati dell'articolo (COFCOD/APFUPR/COFDEF) senza selezionarlo nel form.
    Private Function TrovaRigaArticoloPerCodice(codice As String) As DataRow
        If _dtArticoliCorrente Is Nothing Then Return Nothing

        Dim codiceTrim As String = codice.Trim()
        Dim codiceSenzaZeri As String = codiceTrim.TrimStart("0"c)

        For Each row As DataRow In _dtArticoliCorrente.Rows
            Dim cofcof As String = If(row("COFCOF") Is DBNull.Value, "", row("COFCOF").ToString().Trim())
            If String.Equals(cofcof, codiceTrim, StringComparison.OrdinalIgnoreCase) OrElse
               (codiceSenzaZeri.Length > 0 AndAlso String.Equals(cofcof.TrimStart("0"c), codiceSenzaZeri, StringComparison.OrdinalIgnoreCase)) Then
                Return row
            End If
        Next

        Return Nothing
    End Function

    ' Legge il foglio con ExcelDataReader (supporta sia XLS legacy sia XLSX, a differenza di EPPlus che
    ' legge solo XLSX), individua la riga di intestazione con le colonne CODICE / DESCRIZIONE / PREZZO
    ' (o PRZ CESSIONE) e raccoglie le righe dati sottostanti; cerca inoltre le date della promo nel testo
    ' completo del foglio.
    Private Sub LeggiFilePromoExcel(percorso As String, ByRef righe As List(Of RigaPromoExcel),
                                     ByRef dataInizio As Date, ByRef dataFine As Date, ByRef dateTrovate As Boolean)
        righe = New List(Of RigaPromoExcel)
        dateTrovate = False

        Dim matrice(,) As String

        Using stream As IO.FileStream = IO.File.Open(percorso, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
            Using reader As IExcelDataReader = ExcelReaderFactory.CreateReader(stream)
                Dim ds As DataSet = reader.AsDataSet()
                If ds.Tables.Count = 0 Then Exit Sub
                Dim dt As DataTable = ds.Tables(0)

                ReDim matrice(dt.Rows.Count - 1, dt.Columns.Count - 1)
                For r As Integer = 0 To dt.Rows.Count - 1
                    For c As Integer = 0 To dt.Columns.Count - 1
                        matrice(r, c) = If(dt.Rows(r)(c) Is DBNull.Value, "", dt.Rows(r)(c).ToString().Trim())
                    Next
                Next
            End Using
        End Using

        Dim righeTot As Integer = matrice.GetLength(0)
        Dim coloTot As Integer = matrice.GetLength(1)

        Dim testoCompleto As New Text.StringBuilder()
        For r As Integer = 0 To righeTot - 1
            For c As Integer = 0 To coloTot - 1
                testoCompleto.Append(matrice(r, c)).Append(" "c)
            Next
        Next
        dateTrovate = EstraiDatePromoDaTesto(testoCompleto.ToString(), dataInizio, dataFine)

        For r As Integer = 0 To righeTot - 1
            Dim colCodice As Integer = -1, colPrezzo As Integer = -1, colDescrizione As Integer = -1

            For c As Integer = 0 To coloTot - 1
                Dim intestazione As String = matrice(r, c).ToUpperInvariant()
                If intestazione.Contains("CODICE") Then colCodice = c
                If intestazione.Contains("PREZZO") OrElse intestazione.Contains("PRZ") Then colPrezzo = c
                If intestazione.Contains("DESCRIZ") Then colDescrizione = c
            Next

            If colCodice <> -1 AndAlso colPrezzo <> -1 Then
                For rDati As Integer = r + 1 To righeTot - 1
                    Dim codice As String = matrice(rDati, colCodice)
                    Dim prezzoTesto As String = matrice(rDati, colPrezzo)
                    Dim prezzo As Decimal

                    If Not String.IsNullOrEmpty(codice) AndAlso
                       Decimal.TryParse(prezzoTesto.Replace(",", "."), Globalization.NumberStyles.Number, Globalization.CultureInfo.InvariantCulture, prezzo) AndAlso
                       prezzo > 0 Then
                        Dim descrizione As String = If(colDescrizione <> -1, matrice(rDati, colDescrizione), "")

                        ' Alcune righe accorpano più codici articolo separati da un trattino circondato
                        ' da spazi (es. "PRD111522 - PRD111400"): vanno "spacchettati" in più righe con
                        ' la stessa descrizione/prezzo, senza spezzare codici con trattino "attaccato".
                        For Each singoloCodice As String In Regex.Split(codice, "\s+-\s+")
                            If Not String.IsNullOrWhiteSpace(singoloCodice) Then
                                righe.Add(New RigaPromoExcel With {
                                    .Codice = singoloCodice.Trim(),
                                    .Descrizione = descrizione,
                                    .Prezzo = prezzo
                                })
                            End If
                        Next
                    End If
                Next
                Exit For
            End If
        Next
    End Sub

    ' "Offerta valida [dai/per i] ddt/consegne dal...al..." ha priorità sulla dicitura "promozione dal...al...":
    ' quando presente indica il reale periodo di consegna/fatturazione a cui applicare la promo.
    Private Function EstraiDatePromoDaTesto(testoCompleto As String, ByRef dataInizio As Date, ByRef dataFine As Date) As Boolean
        Dim matchDdt As Match = Regex.Match(testoCompleto,
            "OFFERT\w*\s+VALID\w*.*?(?:DDT|CONSEGN\w*).*?(\d{1,2}[/\-]\d{1,2}[/\-]\d{2,4}).*?(\d{1,2}[/\-]\d{1,2}[/\-]\d{2,4})",
            RegexOptions.IgnoreCase Or RegexOptions.Singleline)

        If matchDdt.Success AndAlso
           ProvaConvertiData(matchDdt.Groups(1).Value, dataInizio) AndAlso
           ProvaConvertiData(matchDdt.Groups(2).Value, dataFine) Then
            Return True
        End If

        Dim matchPromo As Match = Regex.Match(testoCompleto,
            "PROMOZION\w*.*?DAL\s*(\d{1,2}[/\-]\d{1,2}[/\-]\d{2,4}).*?AL\s*(\d{1,2}[/\-]\d{1,2}[/\-]\d{2,4})",
            RegexOptions.IgnoreCase Or RegexOptions.Singleline)

        If matchPromo.Success AndAlso
           ProvaConvertiData(matchPromo.Groups(1).Value, dataInizio) AndAlso
           ProvaConvertiData(matchPromo.Groups(2).Value, dataFine) Then
            Return True
        End If

        Return False
    End Function

    Private Function ProvaConvertiData(testo As String, ByRef risultato As Date) As Boolean
        Dim formati() As String = {"dd/MM/yy", "dd-MM-yy", "dd/MM/yyyy", "dd-MM-yyyy"}
        Return Date.TryParseExact(testo.Trim(), formati, Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, risultato)
    End Function

    Private Sub cmdSalvaPromo_Click(sender As Object, e As EventArgs) Handles cmdSalvaPromo.Click
        If cmbFornitore.SelectedItem Is Nothing OrElse cmbArticoli.SelectedItem Is Nothing Then
            MessageBox.Show("Seleziona fornitore e articolo.")
            Exit Sub
        End If

        If String.IsNullOrWhiteSpace(txtCodiceAS400.Text) Then
            MessageBox.Show("L'articolo selezionato non ha un codice AS400 valido.")
            Exit Sub
        End If

        If Not String.IsNullOrWhiteSpace(txtPercentualePromo.Text) Then
            Dim percentualeCheck As Decimal
            If Not Decimal.TryParse(txtPercentualePromo.Text.Trim().Replace(",", "."), Globalization.NumberStyles.Number,
                                    Globalization.CultureInfo.InvariantCulture, percentualeCheck) OrElse percentualeCheck <= 0 OrElse percentualeCheck >= 100 Then
                MessageBox.Show("Inserisci una percentuale di sconto valida (tra 0 e 100).")
                Exit Sub
            End If

            If _prezzoListino Is Nothing Then
                MessageBox.Show("Prezzo di listino non disponibile per questo articolo: impossibile calcolare la percentuale.")
                Exit Sub
            End If
        End If

        If Not String.IsNullOrWhiteSpace(txtRiduzioneFissa.Text) Then
            Dim riduzioneCheck As Decimal
            If Not Decimal.TryParse(txtRiduzioneFissa.Text.Trim().Replace(",", "."), Globalization.NumberStyles.Number,
                                    Globalization.CultureInfo.InvariantCulture, riduzioneCheck) OrElse riduzioneCheck <= 0 Then
                MessageBox.Show("Inserisci un importo di riduzione valido.")
                Exit Sub
            End If

            If _prezzoListino Is Nothing Then
                MessageBox.Show("Prezzo di listino non disponibile per questo articolo: impossibile calcolare la riduzione.")
                Exit Sub
            End If
        End If

        Dim prezzoPromo As Decimal
        If Not Decimal.TryParse(txtPrezzoPromo.Text.Trim().Replace(",", "."), Globalization.NumberStyles.Number,
                                Globalization.CultureInfo.InvariantCulture, prezzoPromo) OrElse prezzoPromo <= 0 Then
            MessageBox.Show("Inserisci un prezzo promozionale valido.")
            Exit Sub
        End If

        If dtpDataFine.Value.Date < dtpDataInizio.Value.Date Then
            MessageBox.Show("La data fine promo deve essere successiva o uguale alla data inizio promo.")
            Exit Sub
        End If

        Dim fornitoreSelezionato As Fornitore = DirectCast(cmbFornitore.SelectedItem, Fornitore)
        Dim liafor As String = fornitoreSelezionato.Codice
        Dim liacod As String = txtCodiceAS400.Text.Trim()
        Dim liauma As String = txtUM.Text.Trim()
        Dim liadai As Date = dtpDataInizio.Value.Date
        Dim liadaf As Date = dtpDataFine.Value.Date

        Dim lialis As String = DeterminaLialis(fornitoreSelezionato)

        Dim conferma As DialogResult = MessageBox.Show(
            $"Verrà inserita la promozione per l'articolo {liacod} ({txtDescrizione.Text}){vbCrLf}" &
            $"Fornitore: {fornitoreSelezionato.Nome}{vbCrLf}" &
            $"Prezzo promo: {prezzoPromo:N4}{vbCrLf}" &
            $"Dal {liadai.ToShortDateString()} al {liadaf.ToShortDateString()}{vbCrLf}{vbCrLf}PROCEDERE???",
            "Conferma inserimento promozione", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If conferma = DialogResult.No Then Exit Sub

        Me.Cursor = Cursors.WaitCursor
        cmdSalvaPromo.Enabled = False
        Try
            Using conn As New iDB2Connection(ConnStr)
                conn.Open()
                Using tran As iDB2Transaction = conn.BeginTransaction()
                    Try
                        Dim esisteRigaSpostata As Boolean = SpostaLiadaiRigheSuccessiveAPromo(conn, tran, liafor, liacod, liauma, lialis, liadai, liadaf)

                        InserisciRigaPrezzo(conn, tran, String.Empty, lialis, "EUR ", liafor, String.Empty,
                                            liacod, liauma, prezzoPromo, 0D, 0D, 0D, 0D, 0D,
                                            liadai, DataFineMassima, 0, String.Empty)

                        ' Se nessuna riga di listino cadeva già dentro il periodo della promo (e quindi non è
                        ' stata spostata al giorno successivo), il prezzo pre-promo va ripristinato esplicitamente:
                        ' altrimenti resterebbe vigente il prezzo promo anche dopo la sua scadenza (vince sempre
                        ' il LIADAI più recente non successivo alla data, vedi CaricaPrezzoListino).
                        If Not esisteRigaSpostata AndAlso _prezzoListino.HasValue AndAlso _prezzoListino.Value <> prezzoPromo Then
                            InserisciRigaPrezzo(conn, tran, String.Empty, lialis, "EUR ", liafor, String.Empty,
                                                liacod, liauma, _prezzoListino.Value, 0D, 0D, 0D, 0D, 0D,
                                                liadaf.AddDays(1), DataFineMassima, 0, String.Empty)
                        End If

                        tran.Commit()
                    Catch
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            MessageBox.Show("Promozione inserita correttamente.")

            CaricaGrigliaAttuale()

            txtPercentualePromo.Text = String.Empty
            txtRiduzioneFissa.Text = String.Empty
            txtPrezzoPromo.Text = String.Empty
            txtPrezzoPromo.ReadOnly = False
            cmbArticoli.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Errore durante l'inserimento della promozione: " & ex.Message)
        Finally
            Me.Cursor = Cursors.Default
            cmdSalvaPromo.Enabled = True
        End Try
    End Sub

    Private Sub cmdAnnullaPromo_Click(sender As Object, e As EventArgs) Handles cmdAnnullaPromo.Click
        Me.Close()
    End Sub

End Class
