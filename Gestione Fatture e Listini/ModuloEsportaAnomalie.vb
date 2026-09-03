Imports OfficeOpenXml
Imports OfficeOpenXml.Style

''' <summary>
''' Esporta su foglio Excel le stesse righe di anomalia (e con lo stesso raggruppamento per
''' fattura) prodotte da ModuloStampaAnomalie, per poter elaborare/condividere i dati oltre
''' alla stampa. Riutilizzabile sia da frmPrezziAS400 sia da frmPrezziFattureElettroniche.
''' </summary>
Module ModuloEsportaAnomalie

    Private Const NomeColonnaStato As String = "Stato_Anomalia"
    Private Const NomeColonnaNumeroFattura As String = "NumeroFattura"
    Private Const NomeColonnaDataFattura As String = "DataFattura"
    Private Const NomeColonnaCodiceFornitore As String = "CodiceFornitore"
    Private Const NomeColonnaDescrizioneFornitore As String = "DescrizioneFornitore"
    Private Const NomeColonnaDifferenzaTotaleRiga As String = "Differenza_Totale_Riga"

    Private Structure GruppoFattura
        Public Chiave As String
        Public Etichetta As String
        Public Righe As List(Of DataGridViewRow)
    End Structure

    Public Sub EsportaAnomalieExcel(dgv As DataGridView, titolo As String, sottotitolo As String)
        If dgv.Columns.Count = 0 OrElse dgv.Rows.Count = 0 Then
            MsgBox("Esegui prima un'analisi.", MsgBoxStyle.Information)
            Return
        End If

        Dim colonneVisibili As List(Of DataGridViewColumn) =
            dgv.Columns.Cast(Of DataGridViewColumn)().
                Where(Function(c) c.Visible).
                OrderBy(Function(c) c.DisplayIndex).
                ToList()

        Dim haColonneFattura As Boolean = dgv.Columns.Contains(NomeColonnaNumeroFattura) AndAlso dgv.Columns.Contains(NomeColonnaDataFattura)
        Dim haColonnaCodiceFornitore As Boolean = dgv.Columns.Contains(NomeColonnaCodiceFornitore)
        Dim haColonnaDescrizioneFornitore As Boolean = dgv.Columns.Contains(NomeColonnaDescrizioneFornitore)

        ' Come nella stampa: se disponibili, NumeroFattura/DataFattura e Codice/Descrizione
        ' fornitore non si ripetono su ogni riga perche' finiscono nell'intestazione del gruppo.
        Dim nomiColonneTestata As String() = {NomeColonnaNumeroFattura, NomeColonnaDataFattura, NomeColonnaCodiceFornitore, NomeColonnaDescrizioneFornitore}
        Dim colonneDettaglio As List(Of DataGridViewColumn) =
            If(haColonneFattura,
               colonneVisibili.Where(Function(c) Not nomiColonneTestata.Contains(c.Name, StringComparer.OrdinalIgnoreCase)).ToList(),
               colonneVisibili)

        Dim righeExport As New List(Of DataGridViewRow)
        For Each row As DataGridViewRow In dgv.Rows
            If row.IsNewRow Then Continue For
            Dim statoCella As DataGridViewCell = row.Cells(NomeColonnaStato)
            Dim stato As String = If(statoCella.Value Is Nothing, "", statoCella.Value.ToString())
            If Not String.Equals(stato, "In Bolla", StringComparison.OrdinalIgnoreCase) Then
                righeExport.Add(row)
            End If
        Next

        If righeExport.Count = 0 Then
            MsgBox("Nessuna riga con differenze rispetto al listino da esportare.", MsgBoxStyle.Information)
            Return
        End If

        ' Raggruppa per fattura mantenendo l'ordine originale (le righe sono gia' ordinate per
        ' fattura a monte, quindi le righe della stessa fattura restano contigue).
        Dim gruppi As New List(Of GruppoFattura)
        For Each row As DataGridViewRow In righeExport
            Dim numeroFattura As String = If(haColonneFattura AndAlso row.Cells(NomeColonnaNumeroFattura).Value IsNot Nothing, row.Cells(NomeColonnaNumeroFattura).Value.ToString(), "")
            Dim dataFatturaTesto As String = If(haColonneFattura AndAlso row.Cells(NomeColonnaDataFattura).FormattedValue IsNot Nothing, row.Cells(NomeColonnaDataFattura).FormattedValue.ToString(), "")
            Dim chiave As String = numeroFattura & "|" & dataFatturaTesto

            If gruppi.Count = 0 OrElse gruppi(gruppi.Count - 1).Chiave <> chiave Then
                Dim etichetta As String = ""
                If haColonneFattura Then
                    etichetta = $"Fattura n. {numeroFattura} del {dataFatturaTesto}"

                    Dim codiceFornitore As String = If(haColonnaCodiceFornitore AndAlso row.Cells(NomeColonnaCodiceFornitore).Value IsNot Nothing, row.Cells(NomeColonnaCodiceFornitore).Value.ToString(), "")
                    Dim descrizioneFornitore As String = If(haColonnaDescrizioneFornitore AndAlso row.Cells(NomeColonnaDescrizioneFornitore).Value IsNot Nothing, row.Cells(NomeColonnaDescrizioneFornitore).Value.ToString(), "")
                    Dim fornitoreTesto As String = If(codiceFornitore <> "" AndAlso descrizioneFornitore <> "", $"{codiceFornitore} - {descrizioneFornitore}", codiceFornitore & descrizioneFornitore)
                    If fornitoreTesto <> "" Then etichetta &= $"  —  Fornitore: {fornitoreTesto}"
                End If
                gruppi.Add(New GruppoFattura With {.Chiave = chiave, .Etichetta = etichetta, .Righe = New List(Of DataGridViewRow)})
            End If
            gruppi(gruppi.Count - 1).Righe.Add(row)
        Next

        Dim nomeFileProposto As String = String.Join("_", titolo.Split(IO.Path.GetInvalidFileNameChars())) & ".xlsx"

        Using sfd As New SaveFileDialog() With {
            .Filter = "File Excel|*.xlsx",
            .FileName = nomeFileProposto,
            .Title = "Esporta anomalie in Excel"
        }
            If sfd.ShowDialog() <> DialogResult.OK Then Return

            Try
                GeneraFileExcel(sfd.FileName, titolo, sottotitolo, colonneDettaglio, gruppi, righeExport)
            Catch ex As Exception
                MessageBox.Show("Errore durante l'esportazione in Excel: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try

            Dim apri As DialogResult = MessageBox.Show(
                "Esportazione completata:" & vbCrLf & sfd.FileName & vbCrLf & vbCrLf & "Aprire il file?",
                "Esportazione Excel", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
            If apri = DialogResult.Yes Then
                Process.Start(sfd.FileName)
            End If
        End Using
    End Sub

    Private Sub GeneraFileExcel(percorso As String, titolo As String, sottotitolo As String,
                                 colonneDettaglio As List(Of DataGridViewColumn),
                                 gruppi As List(Of GruppoFattura),
                                 righeExport As List(Of DataGridViewRow))

        Dim numColonne As Integer = colonneDettaglio.Count

        Using package As New ExcelPackage()
            Dim ws As ExcelWorksheet = package.Workbook.Worksheets.Add("Anomalie")
            Dim rigaCorrente As Integer = 1

            ws.Cells(rigaCorrente, 1).Value = titolo
            ws.Cells(rigaCorrente, 1).Style.Font.Bold = True
            ws.Cells(rigaCorrente, 1).Style.Font.Size = 14
            ws.Cells(rigaCorrente, 1, rigaCorrente, numColonne).Merge = True
            rigaCorrente += 1

            ws.Cells(rigaCorrente, 1).Value = sottotitolo
            ws.Cells(rigaCorrente, 1).Style.Font.Italic = True
            ws.Cells(rigaCorrente, 1).Style.Font.Size = 9
            ws.Cells(rigaCorrente, 1, rigaCorrente, numColonne).Merge = True
            rigaCorrente += 2

            For Each gruppo As GruppoFattura In gruppi
                If gruppo.Etichetta <> "" Then
                    Dim cellaGruppo = ws.Cells(rigaCorrente, 1)
                    cellaGruppo.Value = gruppo.Etichetta
                    cellaGruppo.Style.Font.Bold = True
                    cellaGruppo.Style.Fill.PatternType = ExcelFillStyle.Solid
                    cellaGruppo.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(230, 230, 230))
                    ws.Cells(rigaCorrente, 1, rigaCorrente, numColonne).Merge = True
                    rigaCorrente += 1
                End If

                For i As Integer = 0 To numColonne - 1
                    Dim cellaHeader = ws.Cells(rigaCorrente, i + 1)
                    cellaHeader.Value = colonneDettaglio(i).HeaderText
                    cellaHeader.Style.Font.Bold = True
                    cellaHeader.Style.Fill.PatternType = ExcelFillStyle.Solid
                    cellaHeader.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(210, 225, 245))
                    cellaHeader.Style.Border.Bottom.Style = ExcelBorderStyle.Thin
                Next
                rigaCorrente += 1

                For Each row As DataGridViewRow In gruppo.Righe
                    Dim stato As String = If(row.Cells(NomeColonnaStato).Value Is Nothing, "", row.Cells(NomeColonnaStato).Value.ToString())
                    Dim eccessivo As Boolean = String.Equals(stato, "Prezzo Eccessivo", StringComparison.OrdinalIgnoreCase)
                    Dim inferiore As Boolean = String.Equals(stato, "Prezzo Inferiore", StringComparison.OrdinalIgnoreCase)
                    Dim mancante As Boolean = String.Equals(stato, "Mancante a Listino", StringComparison.OrdinalIgnoreCase)

                    For i As Integer = 0 To numColonne - 1
                        Dim colonna As DataGridViewColumn = colonneDettaglio(i)
                        Dim valoreCella As Object = row.Cells(colonna.Index).Value
                        Dim cella = ws.Cells(rigaCorrente, i + 1)
                        Dim vuoto As Boolean = valoreCella Is Nothing OrElse IsDBNull(valoreCella)

                        If colonna.ValueType Is GetType(Decimal) OrElse colonna.ValueType Is GetType(Double) Then
                            If Not vuoto Then
                                cella.Value = Convert.ToDouble(valoreCella)
                                cella.Style.Numberformat.Format = "#,##0.000"
                            End If
                            cella.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right
                        ElseIf colonna.ValueType Is GetType(Date) Then
                            If Not vuoto Then
                                cella.Value = Convert.ToDateTime(valoreCella)
                                cella.Style.Numberformat.Format = "dd/mm/yyyy"
                            End If
                        Else
                            Dim formattato As Object = row.Cells(colonna.Index).FormattedValue
                            cella.Value = If(formattato Is Nothing, "", formattato.ToString())
                        End If

                        If eccessivo Then
                            cella.Style.Font.Color.SetColor(Color.Red)
                            cella.Style.Font.Bold = True
                        ElseIf inferiore Then
                            cella.Style.Font.Color.SetColor(Color.Blue)
                            cella.Style.Font.Bold = True
                        ElseIf mancante Then
                            cella.Style.Fill.PatternType = ExcelFillStyle.Solid
                            cella.Style.Fill.BackgroundColor.SetColor(Color.LemonChiffon)
                        End If
                    Next
                    rigaCorrente += 1
                Next

                rigaCorrente += 1
            Next

            Dim totale As Decimal = 0
            For Each row As DataGridViewRow In righeExport
                Dim valDiff As Object = row.Cells(NomeColonnaDifferenzaTotaleRiga).Value
                If valDiff IsNot Nothing AndAlso Not IsDBNull(valDiff) Then
                    Dim d As Decimal = Convert.ToDecimal(valDiff)
                    If d > 0 Then totale += d
                End If
            Next
            ws.Cells(rigaCorrente, 1).Value = $"Totale potenziale recupero: € {totale:N2}"
            ws.Cells(rigaCorrente, 1).Style.Font.Bold = True
            ws.Cells(rigaCorrente, 1, rigaCorrente, numColonne).Merge = True

            ws.Cells(ws.Dimension.Address).AutoFitColumns()

            package.SaveAs(New IO.FileInfo(percorso))
        End Using
    End Sub

End Module
