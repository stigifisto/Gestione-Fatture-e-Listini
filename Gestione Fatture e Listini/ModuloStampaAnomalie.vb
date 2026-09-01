Imports System.Drawing.Printing
Imports System.Linq

''' <summary>
''' Stampa in orizzontale, con anteprima, le sole righe di un DataGridView di analisi prezzi
''' che presentano una differenza rispetto al listino (Stato_Anomalia diverso da "In Bolla"),
''' raggruppate per fattura (NumeroFattura + DataFattura) su due livelli: intestazione fattura
''' e sotto, le sue righe di dettaglio.
''' Le colonne stampate e la loro larghezza relativa ricalcano quelle attualmente visibili nel
''' grid, cosi' la routine è riutilizzabile sia per frmPrezziAS400 sia per frmPrezziFattureElettroniche.
''' </summary>
Module ModuloStampaAnomalie

    Private Const NomeColonnaStato As String = "Stato_Anomalia"
    Private Const NomeColonnaNumeroFattura As String = "NumeroFattura"
    Private Const NomeColonnaDataFattura As String = "DataFattura"
    Private Const NomeColonnaCodiceFornitore As String = "CodiceFornitore"
    Private Const NomeColonnaDescrizioneFornitore As String = "DescrizioneFornitore"

    Private Structure GruppoFattura
        Public Chiave As String
        Public Etichetta As String
        Public Righe As List(Of DataGridViewRow)
    End Structure

    Public Sub StampaAnomalieGriglia(dgv As DataGridView, titolo As String, sottotitolo As String)
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

        ' Colonne stampate nel dettaglio riga: se disponibili, NumeroFattura/DataFattura e
        ' Codice/Descrizione fornitore non si ripetono su ogni riga perche' finiscono
        ' nell'intestazione del gruppo fattura.
        Dim nomiColonneTestata As String() = {NomeColonnaNumeroFattura, NomeColonnaDataFattura, NomeColonnaCodiceFornitore, NomeColonnaDescrizioneFornitore}
        Dim colonneDettaglio As List(Of DataGridViewColumn) =
            If(haColonneFattura,
               colonneVisibili.Where(Function(c) Not nomiColonneTestata.Contains(c.Name, StringComparer.OrdinalIgnoreCase)).ToList(),
               colonneVisibili)

        Dim righeStampa As New List(Of DataGridViewRow)
        For Each row As DataGridViewRow In dgv.Rows
            If row.IsNewRow Then Continue For
            Dim statoCella As DataGridViewCell = row.Cells(NomeColonnaStato)
            Dim stato As String = If(statoCella.Value Is Nothing, "", statoCella.Value.ToString())
            If Not String.Equals(stato, "In Bolla", StringComparison.OrdinalIgnoreCase) Then
                righeStampa.Add(row)
            End If
        Next

        If righeStampa.Count = 0 Then
            MsgBox("Nessuna riga con differenze rispetto al listino da stampare.", MsgBoxStyle.Information)
            Return
        End If

        ' Raggruppa per fattura mantenendo l'ordine originale (le righe sono gia' ordinate per
        ' fattura a monte, quindi le righe della stessa fattura restano contigue).
        Dim gruppi As New List(Of GruppoFattura)
        For Each row As DataGridViewRow In righeStampa
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

        Dim indiceGruppo As Integer = 0
        Dim indiceRigaInGruppo As Integer = 0

        Dim fontTitolo As New Font("Segoe UI", 12, FontStyle.Bold)
        Dim fontSottotitolo As New Font("Segoe UI", 9, FontStyle.Regular)
        Dim fontGruppo As New Font("Segoe UI", 8.5!, FontStyle.Bold)
        Dim fontHeader As New Font("Segoe UI", 8, FontStyle.Bold)
        Dim fontCella As New Font("Segoe UI", 7.5!, FontStyle.Regular)
        Dim fontCellaEvidenziata As New Font("Segoe UI", 7.5!, FontStyle.Bold)

        Dim doc As New PrintDocument()
        doc.DocumentName = titolo
        doc.DefaultPageSettings.Landscape = True

        ' L'anteprima genera un intero "lavoro" di stampa (BeginPrint...PrintPage...EndPrint) per
        ' precalcolare le pagine, e un lavoro separato viene poi avviato quando si stampa davvero:
        ' senza reset gli indici arriverebbero gia' esauriti al secondo lavoro, stampando solo le
        ' intestazioni.
        AddHandler doc.BeginPrint, Sub(sender As Object, e As PrintEventArgs)
                                        indiceGruppo = 0
                                        indiceRigaInGruppo = 0
                                    End Sub

        AddHandler doc.PrintPage,
            Sub(sender As Object, e As PrintPageEventArgs)
                Dim gr As Graphics = e.Graphics
                Dim x As Single = e.MarginBounds.Left
                Dim y As Single = e.MarginBounds.Top
                Dim larghezzaPagina As Single = e.MarginBounds.Width

                gr.DrawString(titolo, fontTitolo, Brushes.Black, x, y)
                y += fontTitolo.GetHeight(gr) + 2
                gr.DrawString(sottotitolo, fontSottotitolo, Brushes.Black, x, y)
                y += fontSottotitolo.GetHeight(gr) + 8

                Dim pesoTotale As Integer = colonneDettaglio.Sum(Function(c) c.Width)
                Dim larghezzeColonne As New List(Of Single)
                For Each c As DataGridViewColumn In colonneDettaglio
                    larghezzeColonne.Add(larghezzaPagina * c.Width / pesoTotale)
                Next

                Dim formatoTesto As New StringFormat() With {
                    .Trimming = StringTrimming.EllipsisCharacter,
                    .FormatFlags = StringFormatFlags.NoWrap
                }
                Dim formatoNumero As New StringFormat() With {
                    .Trimming = StringTrimming.EllipsisCharacter,
                    .FormatFlags = StringFormatFlags.NoWrap,
                    .Alignment = StringAlignment.Far
                }

                Dim xCorrente As Single = x
                Dim altezzaHeader As Single = fontHeader.GetHeight(gr) + 4
                For i As Integer = 0 To colonneDettaglio.Count - 1
                    Dim rect As New RectangleF(xCorrente, y, larghezzeColonne(i), altezzaHeader)
                    gr.DrawString(colonneDettaglio(i).HeaderText, fontHeader, Brushes.Black, rect, formatoTesto)
                    xCorrente += larghezzeColonne(i)
                Next
                y += altezzaHeader
                gr.DrawLine(Pens.Black, x, y, x + larghezzaPagina, y)
                y += 2

                Dim altezzaGruppo As Single = fontGruppo.GetHeight(gr) + 6
                Dim altezzaRiga As Single = fontCella.GetHeight(gr) + 3

                While indiceGruppo < gruppi.Count
                    Dim gruppo As GruppoFattura = gruppi(indiceGruppo)

                    If indiceRigaInGruppo = 0 AndAlso gruppo.Etichetta <> "" Then
                        If y + altezzaGruppo > e.MarginBounds.Bottom Then
                            e.HasMorePages = True
                            Return
                        End If
                        gr.DrawString(gruppo.Etichetta, fontGruppo, Brushes.Black, x, y)
                        y += altezzaGruppo
                    End If

                    While indiceRigaInGruppo < gruppo.Righe.Count
                        If y + altezzaRiga > e.MarginBounds.Bottom Then
                            e.HasMorePages = True
                            Return
                        End If

                        Dim row As DataGridViewRow = gruppo.Righe(indiceRigaInGruppo)
                        Dim stato As String = If(row.Cells(NomeColonnaStato).Value Is Nothing, "", row.Cells(NomeColonnaStato).Value.ToString())
                        Dim daEvidenziare As Boolean = String.Equals(stato, "Prezzo Eccessivo", StringComparison.OrdinalIgnoreCase) OrElse
                                                        String.Equals(stato, "Prezzo Inferiore", StringComparison.OrdinalIgnoreCase)
                        Dim fontRiga As Font = If(daEvidenziare, fontCellaEvidenziata, fontCella)

                        xCorrente = x
                        For i As Integer = 0 To colonneDettaglio.Count - 1
                            Dim cella As DataGridViewCell = row.Cells(colonneDettaglio(i).Index)
                            Dim testo As String = If(cella.FormattedValue Is Nothing, "", cella.FormattedValue.ToString())
                            Dim allineaDestra As Boolean = cella.ValueType Is GetType(Decimal) OrElse cella.ValueType Is GetType(Double) OrElse cella.ValueType Is GetType(Integer)
                            Dim rect As New RectangleF(xCorrente, y, larghezzeColonne(i), altezzaRiga)
                            gr.DrawString(testo, fontRiga, Brushes.Black, rect, If(allineaDestra, formatoNumero, formatoTesto))
                            xCorrente += larghezzeColonne(i)
                        Next

                        y += altezzaRiga
                        indiceRigaInGruppo += 1
                    End While

                    indiceGruppo += 1
                    indiceRigaInGruppo = 0
                    y += 4
                End While

                e.HasMorePages = False
            End Sub

        Using pageSetup As New PageSetupDialog() With {
            .Document = doc,
            .AllowPrinter = True,
            .AllowPaper = True,
            .AllowOrientation = True,
            .AllowMargins = True
        }
            pageSetup.ShowDialog()
        End Using

        Using preview As New PrintPreviewDialog() With {
            .Document = doc,
            .Width = 1000,
            .Height = 700,
            .StartPosition = FormStartPosition.CenterParent
        }
            preview.ShowDialog()
        End Using
    End Sub

End Module
