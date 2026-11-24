Imports System.Data.SqlClient
Imports System.Diagnostics
Imports IBM.Data.DB2.iSeries
Imports OfficeOpenXml

Public Class FrmEstrattore

    ' IDE1006: nomi metodi in PascalCase
    ' IDE0017: object initializer semplificato
    Private Sub CmdEsegui_Click(sender As Object, e As EventArgs) Handles cmdEsegui.Click
        If cmbFornitori.SelectedItem Is Nothing OrElse txtCarica.Text = "" OrElse txtSalva.Text = "" Then
            MessageBox.Show("Assicurati di aver selezionato Fornitore, File PDF e destinazione Excel.")
            Exit Sub
        End If

        Dim estrattoreExe As String = IO.Path.Combine(Application.StartupPath, "Estrattore.exe")
        Dim pdfInput As String = txtCarica.Text
        Dim excelOutput As String = txtSalva.Text
        Dim fornitore As String = cmbFornitori.Text

        If Not IO.File.Exists(estrattoreExe) Then
            MessageBox.Show("File non trovato: " & estrattoreExe & vbCrLf & "Reinstallare l'applicazione.")
            Exit Sub
        End If

        Dim argPagine As String = ""
        If fornitore.ToUpper().Contains("AVIMECC") Then
            Dim inputDa As String = InputBox("Inserisci la pagina INIZIALE del listino nel PDF:", "Intervallo pagine - " & fornitore, "1")
            If String.IsNullOrWhiteSpace(inputDa) Then Exit Sub
            Dim inputA As String = InputBox("Inserisci la pagina FINALE del listino nel PDF:", "Intervallo pagine - " & fornitore, "")
            If String.IsNullOrWhiteSpace(inputA) Then Exit Sub

            Dim numDa As Integer, numA As Integer
            If Not Integer.TryParse(inputDa.Trim(), numDa) OrElse numDa < 1 Then
                MessageBox.Show("Pagina iniziale non valida.")
                Exit Sub
            End If
            If Not Integer.TryParse(inputA.Trim(), numA) OrElse numA < numDa Then
                MessageBox.Show("Pagina finale non valida o inferiore alla pagina iniziale.")
                Exit Sub
            End If
            argPagine = String.Format(" ""{0}"" ""{1}""", numDa, numA)
        End If

        ' IDE0017: object initializer
        Dim psi As New ProcessStartInfo() With {
            .FileName = estrattoreExe,
            .Arguments = String.Format("""{0}"" ""{1}"" ""{2}""", fornitore, pdfInput, excelOutput) & argPagine,
            .UseShellExecute = False,
            .CreateNoWindow = True,
            .RedirectStandardOutput = True,
            .RedirectStandardError = True
        }

        Me.Cursor = Cursors.WaitCursor
        cmdEsegui.Enabled = False
        ImpostaStato("Trasformazione PDF in corso...")
        Try
            Using proc As Process = Process.Start(psi)
                Dim output As String = proc.StandardOutput.ReadToEnd()
                Dim errori As String = proc.StandardError.ReadToEnd()
                proc.WaitForExit()

                If String.IsNullOrEmpty(errori) Then
                    MessageBox.Show("Risultato: " & output)
                Else
                    MessageBox.Show("Errore durante l'elaborazione: " & errori)
                End If

                If output.Contains("Completato") Then
                    txtCarica.Text = ""
                    txtSalva.Text = ""
                    txtExcel.Text = excelOutput
                    Dim idx As Integer = cmbLiafor.FindStringExact(cmbFornitori.Text)
                    If idx >= 0 Then cmbLiafor.SelectedIndex = idx
                    ImpostaStato("Caricamento dati Excel e interrogazione AS400...")
                    CaricaExcelInGriglia(excelOutput)

                    ImpostaStato("Estrazione data validità dal PDF...")
                    Dim dataValidita As Date? = EstrapolDataValidita(pdfInput)
                    If dataValidita.HasValue Then
                        dtpIni.Value = dataValidita.Value
                    End If
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore nel lancio dell'estrattore: " & ex.Message)
        Finally
            Me.Cursor = Cursors.Default
            cmdEsegui.Enabled = True
            ImpostaStato("Pronto.")
        End Try
    End Sub

    Private Const CartellaDefaultPdf As String = "\\padaniafile.padania.local\piazza\Guardamiglio\11 - UFFICIO FORNITORI\FORNITORI\LISTINI E PROMO FORNITORI"

    Private Sub CmdCarica_Click(sender As Object, e As EventArgs) Handles cmdCarica.Click
        Dim ultimaCartella As String = My.Settings.UltimaCartellaPDF
        If Not String.IsNullOrEmpty(ultimaCartella) AndAlso IO.Directory.Exists(ultimaCartella) Then
            ofdApri.InitialDirectory = ultimaCartella
        ElseIf IO.Directory.Exists(CartellaDefaultPdf) Then
            ofdApri.InitialDirectory = CartellaDefaultPdf
        End If

        If ofdApri.ShowDialog() = DialogResult.OK Then
            txtCarica.Text = ofdApri.FileName
            txtSalva.Text = IO.Path.ChangeExtension(ofdApri.FileName, ".xlsx")
            My.Settings.UltimaCartellaPDF = IO.Path.GetDirectoryName(ofdApri.FileName)
            My.Settings.Save()
        End If
    End Sub

    Private Sub CmdSalva_Click(sender As Object, e As EventArgs) Handles cmdSalva.Click
        sfdSalva.ShowDialog()
        txtSalva.Text = sfdSalva.FileName
    End Sub

    ' IDE0028: collection initializer semplificato
    Private Sub FrmEstrattore_Load(sender As Object, e As EventArgs) Handles Me.Load
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
            New Fornitore("Cinque Stelle", "00000447")
        }

        cmbFornitori.DataSource = listaFornitori
        cmbFornitori.DisplayMember = "Nome"
        cmbFornitori.ValueMember = "Codice"
        cmbFornitori.SelectedIndex = -1

        Dim listaliafor As New List(Of Fornitore) From {
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
            New Fornitore("Fiorani", "00008411"),
            New Fornitore("C.C.M.", "00009638")
        }

        cmbLiafor.DataSource = listaliafor
        cmbLiafor.DisplayMember = "Nome"
        cmbLiafor.ValueMember = "Codice"
        cmbLiafor.SelectedIndex = -1
    End Sub

    ' -----------------------------------------------------------------------
    ' Sezione CARICAMENTO IN AS400
    ' -----------------------------------------------------------------------

    Private Sub CmdXls_Click(sender As Object, e As EventArgs) Handles cmdXls.Click
        If cmbLiafor.SelectedItem Is Nothing Then
            MessageBox.Show("Seleziona prima il fornitore.")
            Exit Sub
        End If

        Dim ofd As New OpenFileDialog() With {
            .Filter = "File Excel|*.xlsx;*.xls",
            .Title = "Seleziona file Excel listino"
        }

        If ofd.ShowDialog() <> DialogResult.OK Then Exit Sub

        txtExcel.Text = ofd.FileName
        Me.Cursor = Cursors.WaitCursor
        cmdXls.Enabled = False
        Try
            CaricaExcelInGriglia(ofd.FileName)
        Finally
            Me.Cursor = Cursors.Default
            cmdXls.Enabled = True
            ImpostaStato("Pronto.")
        End Try
    End Sub

    Private Sub CaricaExcelInGriglia(percorso As String)
        If cmbLiafor.SelectedItem Is Nothing Then
            MessageBox.Show("Seleziona prima il fornitore.")
            Exit Sub
        End If

        Try
            Dim isFiorani As Boolean = cmbLiafor.Text = "Fiorani" OrElse cmbLiafor.Text = "Inalca Suino"
            Dim isInalca As Boolean = cmbLiafor.Text = "Inalca"
            ImpostaStato("Lettura file Excel...")
            Dim dtExcel As DataTable
            If isFiorani Then
                dtExcel = LeggiExcelFiorani(percorso)
            ElseIf isInalca Then
                dtExcel = LeggiExcelInalca(percorso)
            Else
                dtExcel = LeggiExcel(percorso)
            End If

            If Not dtExcel.Columns.Contains("Codice") Then
                MessageBox.Show("Il file Excel non contiene la colonna 'Codice'." & vbCrLf &
                                "Colonne trovate: " & String.Join(", ", dtExcel.Columns.Cast(Of DataColumn)().Select(Function(c) c.ColumnName)))
                Exit Sub
            End If

            Dim isFileniElaborati As Boolean = cmbLiafor.Text = "Fileni Elaborati" OrElse cmbLiafor.Text = "Fileni Panati"

            Dim codici As New List(Of String)()
            For Each row As DataRow In dtExcel.Rows
                Dim cod As String = If(row("Codice") Is DBNull.Value, "", row("Codice").ToString().Trim())
                Dim codNorm As String = NormalizzaCodice(cod, isFileniElaborati)
                If Not String.IsNullOrEmpty(codNorm) AndAlso Not codici.Contains(codNorm) Then
                    codici.Add(codNorm)
                End If
            Next

            ImpostaStato("Interrogazione AS400...")
            Dim dtAs400 As DataTable = QueryAs400(codici)
            ImpostaStato("Unione dati in corso...")
            Dim dtRisultato As DataTable = UnisciDati(dtExcel, dtAs400, isFileniElaborati)

            dgvRisultati.DataSource = dtRisultato

        Catch ex As Exception
            MessageBox.Show("Errore durante il caricamento: " & ex.Message)
        End Try
    End Sub

    Private Function LeggiExcel(percorso As String) As DataTable
        Dim dt As New DataTable()

        Using package As New ExcelPackage(New System.IO.FileInfo(percorso))
            Dim ws As ExcelWorksheet = package.Workbook.Worksheets(1)

            If ws.Dimension Is Nothing Then Return dt

            Dim lastCol As Integer = ws.Dimension.End.Column
            Dim lastRow As Integer = ws.Dimension.End.Row

            For col As Integer = 1 To lastCol
                Dim header As String = If(ws.Cells(1, col).Value IsNot Nothing,
                                          ws.Cells(1, col).Value.ToString().Trim(),
                                          "Col" & col)
                dt.Columns.Add(header)
            Next

            For row As Integer = 2 To lastRow
                Dim newRow As DataRow = dt.NewRow()
                For col As Integer = 1 To lastCol
                    newRow(col - 1) = If(ws.Cells(row, col).Value IsNot Nothing,
                                         ws.Cells(row, col).Value.ToString(), "")
                Next
                dt.Rows.Add(newRow)
            Next
        End Using

        Return dt
    End Function

    ' Fiorani: A=Codice, B=Descrizione, C=UM, E=Netto (nessuna riga di intestazione mappata per nome)
    Private Function LeggiExcelFiorani(percorso As String) As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Codice")
        dt.Columns.Add("Descrizione")
        dt.Columns.Add("UM")
        dt.Columns.Add("Netto")

        Using package As New ExcelPackage(New System.IO.FileInfo(percorso))
            Dim ws As ExcelWorksheet = package.Workbook.Worksheets(1)
            If ws.Dimension Is Nothing Then Return dt

            Dim lastRow As Integer = ws.Dimension.End.Row

            For row As Integer = 2 To lastRow
                Dim codice As String = If(ws.Cells(row, 1).Value IsNot Nothing, ws.Cells(row, 1).Value.ToString().Trim(), "")
                Dim dummyLong As Long
                If String.IsNullOrEmpty(codice) OrElse Not Long.TryParse(codice, dummyLong) Then Continue For

                Dim nettoStr As String = If(ws.Cells(row, 5).Value IsNot Nothing, ws.Cells(row, 5).Value.ToString().Trim(), "")
                Dim dummyDec As Decimal
                If String.IsNullOrEmpty(nettoStr) OrElse Not Decimal.TryParse(nettoStr, Globalization.NumberStyles.Number, Globalization.CultureInfo.InvariantCulture, dummyDec) Then Continue For

                Dim newRow As DataRow = dt.NewRow()
                newRow("Codice") = codice
                newRow("Descrizione") = If(ws.Cells(row, 2).Value IsNot Nothing, ws.Cells(row, 2).Value.ToString().Trim(), "")
                newRow("UM") = If(ws.Cells(row, 3).Value IsNot Nothing, ws.Cells(row, 3).Value.ToString().Trim(), "")
                newRow("Netto") = nettoStr
                dt.Rows.Add(newRow)
            Next
        End Using

        Return dt
    End Function

    ' Inalca: B=Codice, C=Descrizione, E=Netto, UM assente
    Private Function LeggiExcelInalca(percorso As String) As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Codice")
        dt.Columns.Add("Descrizione")
        dt.Columns.Add("UM")
        dt.Columns.Add("Netto")

        Using package As New ExcelPackage(New System.IO.FileInfo(percorso))
            Dim ws As ExcelWorksheet = package.Workbook.Worksheets(1)
            If ws.Dimension Is Nothing Then Return dt

            Dim lastRow As Integer = ws.Dimension.End.Row

            For row As Integer = 2 To lastRow
                Dim codice As String = If(ws.Cells(row, 2).Value IsNot Nothing, ws.Cells(row, 2).Value.ToString().Trim(), "")
                Dim dummyLong As Long
                If String.IsNullOrEmpty(codice) OrElse Not Long.TryParse(codice, dummyLong) Then Continue For

                Dim nettoStr As String = If(ws.Cells(row, 5).Value IsNot Nothing, ws.Cells(row, 5).Value.ToString().Trim(), "")
                Dim dummyDec As Decimal
                If String.IsNullOrEmpty(nettoStr) OrElse Not Decimal.TryParse(nettoStr, Globalization.NumberStyles.Number, Globalization.CultureInfo.InvariantCulture, dummyDec) Then Continue For

                Dim newRow As DataRow = dt.NewRow()
                newRow("Codice") = codice
                newRow("Descrizione") = If(ws.Cells(row, 3).Value IsNot Nothing, ws.Cells(row, 3).Value.ToString().Trim(), "")
                newRow("UM") = "KG"
                newRow("Netto") = nettoStr
                dt.Rows.Add(newRow)
            Next
        End Using

        Return dt
    End Function

    Private Function QueryAs400(codici As List(Of String)) As DataTable
        Dim selFornitore As String = cmbLiafor.SelectedValue.ToString()
        Dim dt As New DataTable()
        If codici.Count = 0 Then Return dt

        Dim connStr As String = "DataSource=192.168.2.200;UserID=edpuser;Password=edpuser;DefaultCollection=inadati"

        Dim inClause As String = String.Join(",",
            codici.Select(Function(c) String.Format("'{0}'", c.Replace("'", "''"))))
        Dim sql As String = String.Format(
            "WITH PrezziNumerati AS (" &
            "  SELECT liasoc, liafor, liacod, liauma, liadai," &
            "    ROW_NUMBER() OVER(PARTITION BY LIASOC, LIAFOR, LIACOD, liauma ORDER BY liadai DESC) AS RigaUM" &
            "  FROM PJVARGRD.OALIA£2L" &
            ")" &
            " SELECT cofcof, coffor, cofcod, cofdef, apfupr, liadai" &
            " FROM INADATI.OACOF00F LEFT JOIN inadati.anapf00f on cofcod=apfcod and cofsoc=apfsoc and coffor = apffor " &
            " LEFT JOIN PrezziNumerati ON cofcod = liacod AND cofsoc = liasoc AND coffor = liafor AND rigaum = 1" &
            " WHERE COFSOC='GRD' AND COFCOF IN ({0}) AND COFFOR = @COFFOR",
        inClause)

        Using conn As New iDB2Connection(connStr)
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

    Private Function NormalizzaCodice(codice As String, isFileniElaborati As Boolean) As String
        If isFileniElaborati AndAlso codice.StartsWith("PRD", StringComparison.OrdinalIgnoreCase) Then
            Return codice.Substring(3)
        End If
        Return codice
    End Function

    Private Function UnisciDati(dtExcel As DataTable, dtAs400 As DataTable, isFileniElaborati As Boolean) As DataTable
        Dim dtRisultato As New DataTable()

        For Each col As DataColumn In dtExcel.Columns
            dtRisultato.Columns.Add(col.ColumnName, GetType(String))
        Next
        dtRisultato.Columns.Add("COD_AS400", GetType(String))
        dtRisultato.Columns.Add("DESCRIZIONE_AS400", GetType(String))
        dtRisultato.Columns.Add("UM_AS400", GetType(String))

        Dim lookup As New Dictionary(Of String, DataRow)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In dtAs400.Rows
            Dim key As String = If(row("COFCOF") Is DBNull.Value, "", row("COFCOF").ToString().Trim())
            If Not String.IsNullOrEmpty(key) Then
                lookup(key) = row
            End If
        Next

        For Each excelRow As DataRow In dtExcel.Rows
            Dim newRow As DataRow = dtRisultato.NewRow()

            For Each col As DataColumn In dtExcel.Columns
                newRow(col.ColumnName) = If(excelRow(col.ColumnName) Is DBNull.Value, "", excelRow(col.ColumnName).ToString())
            Next

            Dim codice As String = If(excelRow("Codice") Is DBNull.Value, "", excelRow("Codice").ToString().Trim())
            Dim codiceNorm As String = NormalizzaCodice(codice, isFileniElaborati)
            If Not String.IsNullOrEmpty(codiceNorm) AndAlso lookup.ContainsKey(codiceNorm) Then
                Dim as400Row As DataRow = lookup(codiceNorm)
                newRow("COD_AS400") = If(as400Row("COFCOD") Is DBNull.Value, "", as400Row("COFCOD").ToString().Trim())
                newRow("DESCRIZIONE_AS400") = If(as400Row("COFDEF") Is DBNull.Value, "", as400Row("COFDEF").ToString().Trim())
                newRow("UM_AS400") = If(as400Row("APFUPR") Is DBNull.Value, "", as400Row("APFUPR").ToString().Trim())
            End If

            dtRisultato.Rows.Add(newRow)
        Next

        Return dtRisultato
    End Function

    Private Sub CmdAs400_Click(sender As Object, e As EventArgs) Handles cmdAs400.Click
        If cmbLiafor.SelectedItem Is Nothing Then
            MessageBox.Show("Seleziona prima il fornitore.")
            Exit Sub
        End If

        Dim elabora As DialogResult = MessageBox.Show("VERRA' CARICATO IL FILE " & txtExcel.Text & " DEL FORNITORE " & cmbLiafor.Text &
                           " SU AS400 CON DATA " & dtpIni.Value.ToShortDateString & "!!! PROCEDERE???", "ATTENZIONE!!!", MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question)
        If elabora = DialogResult.No Then Exit Sub


        Dim dtRisultato As DataTable = TryCast(dgvRisultati.DataSource, DataTable)
        If dtRisultato Is Nothing OrElse dtRisultato.Rows.Count = 0 Then
            MessageBox.Show("Nessun dato da importare. Carica prima il file Excel.")
            Exit Sub
        End If

        If Not dtRisultato.Columns.Contains("COD_AS400") OrElse Not dtRisultato.Columns.Contains("Netto") Then
            MessageBox.Show("Colonne attese non trovate nel risultato." & vbCrLf &
                            "Colonne presenti: " & String.Join(", ", dtRisultato.Columns.Cast(Of DataColumn)().Select(Function(c) c.ColumnName)))
            Exit Sub
        End If

        Dim connStr As String = "DataSource=192.168.2.200;UserID=edpuser;Password=edpuser;DefaultCollection=inadati"
        Dim liafor As String = cmbLiafor.SelectedValue.ToString()
        Dim liadai As Date = dtpIni.Value.Date

        Dim lialis As String
        Select Case cmbLiafor.Text
            Case "Aia MD Nord"
                lialis = "02"
            Case "Aia MD Sud"
                lialis = "03"
            Case "Aia Coop Sicilia"
                lialis = "04"
            Case "Avimecc Coop Calabria"
                lialis = "05"
            Case "Avimecc Coop Sicilia"
                lialis = "04"
            Case Else
                lialis = "01"
        End Select

        Me.Cursor = Cursors.WaitCursor
        cmdAs400.Enabled = False

        ' Verifica che il listino non sia già stato importato
        ImpostaStato("Verifica duplicati in corso...")
        Try
            Using connChk As New iDB2Connection(connStr)
                connChk.Open()
                Dim sqlChk As String =
                    "SELECT COUNT(*) FROM GEST_GUA.LISDATINI " &
                    "WHERE LIASOC='GRD' AND LIALIS=@LIALIS AND LIAVAL=@LIAVAL AND LIAFOR=@LIAFOR AND LIADAI=@LIADAI AND LIATIP=@LIATIP"
                Using cmdChk As New iDB2Command(sqlChk, connChk)
                    cmdChk.Parameters.AddWithValue("@LIALIS", lialis)
                    cmdChk.Parameters.AddWithValue("@LIAVAL", "EUR ")
                    cmdChk.Parameters.AddWithValue("@LIAFOR", liafor)
                    cmdChk.Parameters.AddWithValue("@LIADAI", liadai.ToString("yyyy-MM-dd"))
                    cmdChk.Parameters.AddWithValue("@LIATIP", cmbLiafor.Text)
                    Dim contEsistenti As Integer = Convert.ToInt32(cmdChk.ExecuteScalar())
                    If contEsistenti > 0 Then
                        If HaPromozioniAia(dtRisultato) Then
                            ' Il listino è già su AS400, ma le promo (salvate a parte) si possono
                            ' ancora caricare: l'operazione non crea duplicati.
                            If MessageBox.Show(
                                $"Il listino {cmbLiafor.Text} con data {liadai.ToShortDateString()} risulta già importato su AS400." & vbCrLf & vbCrLf &
                                "Vuoi caricare solo le promozioni presenti nel listino?",
                                "Listino già presente", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                                MessageBox.Show(SalvaPromozioniAia(dtRisultato, liafor, lialis, liadai))
                            End If
                        Else
                            MessageBox.Show(
                                $"Il listino {cmbLiafor.Text} con data {liadai.ToShortDateString()} risulta già importato. Operazione annullata.",
                                "Listino già presente", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If
                        Me.Cursor = Cursors.Default
                        cmdAs400.Enabled = True
                        ImpostaStato("Pronto.")
                        Exit Sub
                    End If
                End Using
            End Using
        Catch exChk As Exception
            MessageBox.Show("Errore verifica duplicato: " & exChk.Message)
            Me.Cursor = Cursors.Default
            cmdAs400.Enabled = True
            ImpostaStato("Pronto.")
            Exit Sub
        End Try

        Dim sql As String =
            "INSERT INTO PJVARGRD.OALIA£2L " &
            "(LIASOC, LIAUNT, LIALIS, LIAVAL, LIAFOR, LIACLV, LIACOD, LIAUMA, LIAPRZ, " &
            " LIASC1, LIASC2, LIASC3, LIASC4, LIASCQ, LIADAI, LIADAF, LIAFIL, LIAMAG) " &
            "VALUES (@LIASOC, @LIAUNT, @LIALIS, @LIAVAL, @LIAFOR, @LIACLV, @LIACOD, @LIAUMA, @LIAPRZ, " &
            "        @LIASC1, @LIASC2, @LIASC3, @LIASC4, @LIASCQ, @LIADAI, @LIADAF, @LIAFIL, @LIAMAG)"

        Dim contInsert As Integer = 0
        Dim contSkip As Integer = 0
        Dim contErrore As Integer = 0
        Dim erroriMsg As New System.Text.StringBuilder()
        Dim totale As Integer = dtRisultato.Rows.Count
        Dim rigaCorrente As Integer = 0

        Try
            Using conn As New iDB2Connection(connStr)
                conn.Open()

                For Each row As DataRow In dtRisultato.Rows
                    rigaCorrente += 1
                    ImpostaStato($"Importazione record {rigaCorrente} di {totale}...", rigaCorrente, totale)
                    Dim cod As String = If(row("COD_AS400") Is DBNull.Value, "", row("COD_AS400").ToString().Trim())
                    If String.IsNullOrEmpty(cod) Then
                        contSkip += 1
                        Continue For
                    End If

                    ' Prende UM da AS400; se assente usa UM dal file Excel
                    Dim uma As String = If(row("UM_AS400") Is DBNull.Value, "", row("UM_AS400").ToString().Trim())
                    If String.IsNullOrEmpty(uma) AndAlso dtRisultato.Columns.Contains("UM") Then
                        uma = If(row("UM") Is DBNull.Value, "", row("UM").ToString().Trim())
                    End If

                    Dim nettoStr As String = If(row("Netto") Is DBNull.Value, "0",
                                                row("Netto").ToString().Trim().Replace(",", "."))
                    Dim liaprz As Decimal = 0
                    Decimal.TryParse(nettoStr, Globalization.NumberStyles.Number,
                                     Globalization.CultureInfo.InvariantCulture, liaprz)

                    If liaprz = 0 Then
                        contSkip += 1
                        Continue For
                    End If

                    Try
                        Using cmd As New iDB2Command(sql, conn)
                            cmd.Parameters.AddWithValue("@LIASOC", "GRD")
                            cmd.Parameters.AddWithValue("@LIAUNT", String.Empty)
                            cmd.Parameters.AddWithValue("@LIALIS", lialis)

                            cmd.Parameters.AddWithValue("@LIAVAL", "EUR ")
                            cmd.Parameters.AddWithValue("@LIAFOR", liafor)
                            cmd.Parameters.AddWithValue("@LIACLV", String.Empty)
                            cmd.Parameters.AddWithValue("@LIACOD", cod)
                            cmd.Parameters.AddWithValue("@LIAUMA", uma)
                            cmd.Parameters.AddWithValue("@LIAPRZ", liaprz)
                            cmd.Parameters.AddWithValue("@LIASC1", 0D)
                            cmd.Parameters.AddWithValue("@LIASC2", 0D)
                            cmd.Parameters.AddWithValue("@LIASC3", 0D)
                            cmd.Parameters.AddWithValue("@LIASC4", 0D)
                            cmd.Parameters.AddWithValue("@LIASCQ", 0D)
                            cmd.Parameters.AddWithValue("@LIADAI", liadai.ToString("yyyy-MM-dd"))
                            cmd.Parameters.AddWithValue("@LIADAF", "9999-12-31")
                            cmd.Parameters.AddWithValue("@LIAFIL", 0)
                            cmd.Parameters.AddWithValue("@LIAMAG", String.Empty)
                            cmd.ExecuteNonQuery()
                            contInsert += 1
                        End Using
                    Catch exRow As Exception
                        contErrore += 1
                        If contErrore <= 5 Then
                            erroriMsg.AppendLine($"  {cod}: {exRow.Message}")
                        End If
                    End Try
                Next
            End Using

            Dim msg As String = $"Importazione completata:{vbCrLf}" &
                                $"  Inserite:  {contInsert}{vbCrLf}" &
                                $"  Saltate (cod. AS400 mancante): {contSkip}{vbCrLf}" &
                                $"  Errori:    {contErrore}"
            If erroriMsg.Length > 0 Then
                msg &= vbCrLf & vbCrLf & "Primi errori:" & vbCrLf & erroriMsg.ToString()
            End If
            If HaPromozioniAia(dtRisultato) Then
                ImpostaStato("Salvataggio promozioni...")
                msg &= vbCrLf & vbCrLf & SalvaPromozioniAia(dtRisultato, liafor, lialis, liadai)
            End If
            MessageBox.Show(msg)

            ' Registra l'importazione in LISDATINI
            If contInsert > 0 Then
                Using connReg As New iDB2Connection(connStr)
                    connReg.Open()
                    Dim sqlReg As String =
                        "INSERT INTO GEST_GUA.LISDATINI (LIASOC, LIALIS, LIAVAL, LIAFOR, LIADAI, LIATIP) " &
                        "VALUES ('GRD', @LIALIS, @LIAVAL, @LIAFOR, @LIADAI, @LIATIP)"
                    Using cmdReg As New iDB2Command(sqlReg, connReg)
                        cmdReg.Parameters.AddWithValue("@LIALIS", lialis)
                        cmdReg.Parameters.AddWithValue("@LIAVAL", "EUR ")
                        cmdReg.Parameters.AddWithValue("@LIAFOR", liafor)
                        cmdReg.Parameters.AddWithValue("@LIADAI", liadai.ToString("yyyy-MM-dd"))
                        cmdReg.Parameters.AddWithValue("@LIATIP", cmbLiafor.Text)
                        cmdReg.ExecuteNonQuery()
                    End Using
                End Using

                cmbLiafor.SelectedIndex = -1
                txtExcel.Text = String.Empty
                dtpIni.Value = Date.Today
            End If

        Catch ex As Exception
            MessageBox.Show("Errore di connessione AS400: " & ex.Message)
        Finally
            Me.Cursor = Cursors.Default
            cmdAs400.Enabled = True
            ImpostaStato("Pronto.")
        End Try
    End Sub

    ' -----------------------------------------------------------------------
    ' Promozioni AIA
    ' -----------------------------------------------------------------------

    ' Nei listini AIA il prezzo caricato su AS400 è il "Listino Nuovo" (colonna Netto); le
    ' promozioni hanno un proprio periodo (data bolla dal/al), che può iniziare prima della
    ' data di validità del listino, e vengono quindi salvate a parte in Promozioni_Acquisto_Aia.
    Private Function HaPromozioniAia(dt As DataTable) As Boolean
        Return cmbLiafor.Text.StartsWith("Aia", StringComparison.OrdinalIgnoreCase) AndAlso
               dt.Columns.Contains("PromoPrezzo") AndAlso dt.Columns.Contains("PromoDal") AndAlso dt.Columns.Contains("PromoAl")
    End Function

    ' La stessa promo compare in più listini settimanali consecutivi: la chiave è fornitore +
    ' listino + articolo + periodo, quindi una promo già presente viene solo aggiornata se cambia
    ' il prezzo (DataListino indica il listino da cui proviene il prezzo in vigore).
    Private Function SalvaPromozioniAia(dt As DataTable, liafor As String, lialis As String, dataListino As Date) As String
        Dim sql As String = "
            MERGE dbo.Promozioni_Acquisto_Aia AS t
            USING (SELECT @for AS CodiceFornitore, @vp AS ValidoPer, @cod AS CodiceArticolo,
                          @dal AS DataInizioValidita, @al AS DataFineValidita) AS s
            ON t.CodiceFornitore = s.CodiceFornitore AND t.ValidoPer = s.ValidoPer AND t.CodiceArticolo = s.CodiceArticolo
               AND t.DataInizioValidita = s.DataInizioValidita AND t.DataFineValidita = s.DataFineValidita
            WHEN MATCHED AND t.PrezzoPromo <> @prezzo THEN
                UPDATE SET PrezzoPromo = @prezzo, DataListino = @datalis, TipoListino = @tipo,
                           CodiceArticoloFornitore = @codfor, DescrizioneArticolo = @desc, UM = @um,
                           DataAggiornamento = GETDATE()
            WHEN NOT MATCHED THEN
                INSERT (CodiceFornitore, ValidoPer, CodiceArticolo, CodiceArticoloFornitore, DescrizioneArticolo, UM,
                        PrezzoPromo, DataInizioValidita, DataFineValidita, DataListino, TipoListino)
                VALUES (@for, @vp, @cod, @codfor, @desc, @um, @prezzo, @dal, @al, @datalis, @tipo)
            OUTPUT $action;"

        Dim inserite As Integer = 0, aggiornate As Integer = 0, invariate As Integer = 0
        Dim senzaCodice As Integer = 0, nonValide As Integer = 0
        Dim promoValide As New List(Of PromoAia)()

        Try
            Using conn As New SqlConnection(ModuloImportListiniInfinity.connectionString)
                conn.Open()
                Using tran As SqlTransaction = conn.BeginTransaction()
                    For Each row As DataRow In dt.Rows
                        Dim prezzoStr As String = If(row("PromoPrezzo") Is DBNull.Value, "", row("PromoPrezzo").ToString().Trim().Replace(",", "."))
                        If String.IsNullOrEmpty(prezzoStr) Then Continue For

                        Dim cod As String = If(row("COD_AS400") Is DBNull.Value, "", row("COD_AS400").ToString().Trim())
                        If String.IsNullOrEmpty(cod) Then
                            senzaCodice += 1
                            Continue For
                        End If

                        Dim prezzo As Decimal, dal As Date, al As Date
                        If Not Decimal.TryParse(prezzoStr, Globalization.NumberStyles.Number, Globalization.CultureInfo.InvariantCulture, prezzo) OrElse prezzo <= 0 OrElse
                           Not Date.TryParseExact(row("PromoDal").ToString().Trim(), "dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dal) OrElse
                           Not Date.TryParseExact(row("PromoAl").ToString().Trim(), "dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, al) OrElse
                           al < dal Then
                            nonValide += 1
                            Continue For
                        End If

                        Dim uma As String = If(row("UM_AS400") Is DBNull.Value, "", row("UM_AS400").ToString().Trim())
                        If String.IsNullOrEmpty(uma) AndAlso dt.Columns.Contains("UM") Then uma = row("UM").ToString().Trim()

                        Using cmd As New SqlCommand(sql, conn, tran)
                            cmd.Parameters.AddWithValue("@for", liafor)
                            cmd.Parameters.AddWithValue("@vp", lialis & "EUR")
                            cmd.Parameters.AddWithValue("@cod", cod)
                            cmd.Parameters.AddWithValue("@codfor", row("Codice").ToString().Trim())
                            cmd.Parameters.AddWithValue("@desc", If(row("DESCRIZIONE_AS400") Is DBNull.Value, "", row("DESCRIZIONE_AS400").ToString().Trim()))
                            cmd.Parameters.AddWithValue("@um", uma)
                            cmd.Parameters.AddWithValue("@prezzo", prezzo)
                            cmd.Parameters.AddWithValue("@dal", dal)
                            cmd.Parameters.AddWithValue("@al", al)
                            cmd.Parameters.AddWithValue("@datalis", dataListino)
                            cmd.Parameters.AddWithValue("@tipo", cmbLiafor.Text)
                            Select Case Convert.ToString(cmd.ExecuteScalar())
                                Case "INSERT" : inserite += 1
                                Case "UPDATE" : aggiornate += 1
                                Case Else : invariate += 1
                            End Select
                        End Using
                        promoValide.Add(New PromoAia With {.Codice = cod, .Uma = uma, .Prezzo = prezzo, .Dal = dal, .Al = al})
                    Next
                    tran.Commit()
                End Using
            End Using
        Catch ex As Exception
            Return "Errore nel salvataggio delle promozioni (nessuna promozione salvata): " & ex.Message
        End Try

        Dim msg As String = $"Promozioni AIA:{vbCrLf}" &
                            $"  Nuove:      {inserite}{vbCrLf}" &
                            $"  Aggiornate: {aggiornate}{vbCrLf}" &
                            $"  Già presenti: {invariate}"
        If senzaCodice > 0 Then msg &= $"{vbCrLf}  Saltate (cod. AS400 mancante): {senzaCodice}"
        If nonValide > 0 Then msg &= $"{vbCrLf}  Saltate (prezzo o date non validi): {nonValide}"
        If promoValide.Count > 0 Then
            ImpostaStato("Caricamento promozioni su AS400...")
            msg &= vbCrLf & vbCrLf & SalvaPromozioniAiaSuAs400(promoValide, liafor, lialis)
        End If
        Return msg
    End Function

    Private Class PromoAia
        Public Property Codice As String
        Public Property Uma As String
        Public Property Prezzo As Decimal
        Public Property Dal As Date
        Public Property Al As Date
    End Class

    ' Le promo del listino vanno anche su AS400 (OALIA£2L) in un listino dedicato, P + numero del
    ' listino di provenienza (01 -> P1, 03 -> P3...), con il loro periodo reale LIADAI-LIADAF.
    ' La riga è identificata da fornitore + listino + articolo + UM + data inizio: una promo già
    ' caricata da un listino precedente viene aggiornata solo se cambiano prezzo o data fine
    ' (es. promo prolungata), così da non creare duplicati.
    Private Function SalvaPromozioniAiaSuAs400(promo As List(Of PromoAia), liafor As String, lialis As String) As String
        Dim lialisPromo As String = "P" & CInt(lialis).ToString()
        Dim connStr As String = "DataSource=192.168.2.200;UserID=edpuser;Password=edpuser;DefaultCollection=inadati"

        Dim sqlCerca As String =
            "SELECT LIAPRZ, LIADAF FROM PJVARGRD.OALIA£2L" &
            " WHERE LIASOC = 'GRD' AND LIALIS = @LIALIS AND LIAVAL = 'EUR ' AND LIAFOR = @LIAFOR AND LIAFIL = 0" &
            " AND LIACOD = @LIACOD AND LIAUMA = @LIAUMA AND LIADAI = @LIADAI"
        Dim sqlAggiorna As String =
            "UPDATE PJVARGRD.OALIA£2L SET LIAPRZ = @LIAPRZ, LIADAF = @LIADAF" &
            " WHERE LIASOC = 'GRD' AND LIALIS = @LIALIS AND LIAVAL = 'EUR ' AND LIAFOR = @LIAFOR AND LIAFIL = 0" &
            " AND LIACOD = @LIACOD AND LIAUMA = @LIAUMA AND LIADAI = @LIADAI"
        Dim sqlInserisci As String =
            "INSERT INTO PJVARGRD.OALIA£2L " &
            "(LIASOC, LIAUNT, LIALIS, LIAVAL, LIAFOR, LIACLV, LIACOD, LIAUMA, LIAPRZ, " &
            " LIASC1, LIASC2, LIASC3, LIASC4, LIASCQ, LIADAI, LIADAF, LIAFIL, LIAMAG) " &
            "VALUES ('GRD', '', @LIALIS, 'EUR ', @LIAFOR, '', @LIACOD, @LIAUMA, @LIAPRZ, " &
            "        0, 0, 0, 0, 0, @LIADAI, @LIADAF, 0, '')"

        Dim inserite As Integer = 0, aggiornate As Integer = 0, invariate As Integer = 0

        Try
            Using conn As New iDB2Connection(connStr)
                conn.Open()
                Using tran As iDB2Transaction = conn.BeginTransaction()
                    For Each p In promo
                        Dim prezzoEsistente As Decimal? = Nothing
                        Dim fineEsistente As Date = Date.MinValue
                        Using cmd As New iDB2Command(sqlCerca, conn)
                            cmd.Transaction = tran
                            AggiungiChiavePromoAs400(cmd, lialisPromo, liafor, p)
                            Using reader = cmd.ExecuteReader()
                                If reader.Read() Then
                                    prezzoEsistente = Convert.ToDecimal(reader("LIAPRZ"))
                                    fineEsistente = Convert.ToDateTime(reader("LIADAF")).Date
                                End If
                            End Using
                        End Using

                        Dim prezzoAs400 As Decimal = Math.Round(p.Prezzo, 4)
                        If prezzoEsistente.HasValue AndAlso prezzoEsistente.Value = prezzoAs400 AndAlso fineEsistente = p.Al Then
                            invariate += 1
                            Continue For
                        End If

                        Using cmd As New iDB2Command(If(prezzoEsistente.HasValue, sqlAggiorna, sqlInserisci), conn)
                            cmd.Transaction = tran
                            ' Parametri nell'ordine in cui compaiono nel testo SQL
                            If prezzoEsistente.HasValue Then
                                cmd.Parameters.AddWithValue("@LIAPRZ", prezzoAs400)
                                cmd.Parameters.AddWithValue("@LIADAF", p.Al.ToString("yyyy-MM-dd"))
                                AggiungiChiavePromoAs400(cmd, lialisPromo, liafor, p)
                            Else
                                cmd.Parameters.AddWithValue("@LIALIS", lialisPromo)
                                cmd.Parameters.AddWithValue("@LIAFOR", liafor)
                                cmd.Parameters.AddWithValue("@LIACOD", p.Codice)
                                cmd.Parameters.AddWithValue("@LIAUMA", p.Uma)
                                cmd.Parameters.AddWithValue("@LIAPRZ", prezzoAs400)
                                cmd.Parameters.AddWithValue("@LIADAI", p.Dal.ToString("yyyy-MM-dd"))
                                cmd.Parameters.AddWithValue("@LIADAF", p.Al.ToString("yyyy-MM-dd"))
                            End If
                            cmd.ExecuteNonQuery()
                        End Using
                        If prezzoEsistente.HasValue Then aggiornate += 1 Else inserite += 1
                    Next
                    tran.Commit()
                End Using
            End Using
        Catch ex As Exception
            Return $"Promozioni su AS400 (listino {lialisPromo}): errore, nessuna promozione caricata su AS400: {ex.Message}" & vbCrLf &
                   "Le promozioni sono comunque salvate in SQL Server: si possono ricaricare su AS400 rilanciando il caricamento (opzione ""solo promozioni"")."
        End Try

        Return $"Promozioni su AS400 (listino {lialisPromo}):{vbCrLf}" &
               $"  Nuove:      {inserite}{vbCrLf}" &
               $"  Aggiornate: {aggiornate}{vbCrLf}" &
               $"  Già presenti: {invariate}"
    End Function

    Private Sub AggiungiChiavePromoAs400(cmd As iDB2Command, lialisPromo As String, liafor As String, p As PromoAia)
        cmd.Parameters.AddWithValue("@LIALIS", lialisPromo)
        cmd.Parameters.AddWithValue("@LIAFOR", liafor)
        cmd.Parameters.AddWithValue("@LIACOD", p.Codice)
        cmd.Parameters.AddWithValue("@LIAUMA", p.Uma)
        cmd.Parameters.AddWithValue("@LIADAI", p.Dal.ToString("yyyy-MM-dd"))
    End Sub

    ' -----------------------------------------------------------------------
    ' Estrazione data di validità dal PDF tramite estrai_data_listino.exe
    ' -----------------------------------------------------------------------

    Private Function EstrapolDataValidita(pdfPath As String) As Date?
        Dim estrattoreData As String = IO.Path.Combine(Application.StartupPath, "estrai_data_listino.exe")
        If Not IO.File.Exists(estrattoreData) Then Return Nothing

        Try
            Dim psi As New ProcessStartInfo() With {
                .FileName = estrattoreData,
                .Arguments = String.Format("""{0}""", pdfPath),
                .UseShellExecute = False,
                .CreateNoWindow = True,
                .RedirectStandardOutput = True
            }

            Using proc As Process = Process.Start(psi)
                Dim output As String = proc.StandardOutput.ReadToEnd().Trim()
                proc.WaitForExit()

                If String.IsNullOrEmpty(output) Then Return Nothing

                Dim data As Date
                If Date.TryParseExact(output, "dd/MM/yyyy",
                                      Globalization.CultureInfo.InvariantCulture,
                                      Globalization.DateTimeStyles.None, data) Then
                    Return data
                End If
            End Using
        Catch
            ' Non blocca il flusso principale
        End Try

        Return Nothing
    End Function

    Private Sub ImpostaStato(messaggio As String, Optional corrente As Integer = -1, Optional massimo As Integer = 0)
        lblStato.Text = messaggio
        If corrente >= 0 AndAlso massimo > 0 Then
            prgElaborazione.Visible = True
            prgElaborazione.Maximum = massimo
            prgElaborazione.Value = Math.Min(corrente, massimo)
        Else
            prgElaborazione.Visible = False
        End If
        Application.DoEvents()
    End Sub

    Private Sub CmdPromoMan_Click(sender As Object, e As EventArgs) Handles CmdPromoMan.Click
        Using frm As New FrmPromoManuale()
            frm.ShowDialog(Me)
        End Using
    End Sub
End Class

Public Class Fornitore
    Public Property Nome As String
    Public Property Codice As String

    Public Sub New(nome As String, codice As String)
        Me.Nome = nome
        Me.Codice = codice
    End Sub
End Class
