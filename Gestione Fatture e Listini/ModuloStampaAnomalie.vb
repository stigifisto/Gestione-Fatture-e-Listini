Imports System.Drawing.Printing
Imports System.Linq

''' <summary>
''' Stampa in orizzontale, con anteprima, le sole righe di un DataGridView di analisi prezzi
''' che presentano una differenza rispetto al listino (Stato_Anomalia diverso da "In Bolla").
''' Le colonne stampate e la loro larghezza relativa ricalcano quelle attualmente visibili nel
''' grid, cosi' la routine è riutilizzabile sia per frmPrezziAS400 sia per frmPrezziFattureElettroniche.
''' </summary>
Module ModuloStampaAnomalie

    Private Const NomeColonnaStato As String = "Stato_Anomalia"

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

        Dim indiceRiga As Integer = 0

        Dim fontTitolo As New Font("Segoe UI", 12, FontStyle.Bold)
        Dim fontSottotitolo As New Font("Segoe UI", 9, FontStyle.Regular)
        Dim fontHeader As New Font("Segoe UI", 8, FontStyle.Bold)
        Dim fontCella As New Font("Segoe UI", 7.5!, FontStyle.Regular)
        Dim fontCellaEvidenziata As New Font("Segoe UI", 7.5!, FontStyle.Bold)

        Dim doc As New PrintDocument()
        doc.DocumentName = titolo
        doc.DefaultPageSettings.Landscape = True

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

                Dim pesoTotale As Integer = colonneVisibili.Sum(Function(c) c.Width)
                Dim larghezzeColonne As New List(Of Single)
                For Each c As DataGridViewColumn In colonneVisibili
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
                For i As Integer = 0 To colonneVisibili.Count - 1
                    Dim rect As New RectangleF(xCorrente, y, larghezzeColonne(i), altezzaHeader)
                    gr.DrawString(colonneVisibili(i).HeaderText, fontHeader, Brushes.Black, rect, formatoTesto)
                    xCorrente += larghezzeColonne(i)
                Next
                y += altezzaHeader
                gr.DrawLine(Pens.Black, x, y, x + larghezzaPagina, y)
                y += 2

                Dim altezzaRiga As Single = fontCella.GetHeight(gr) + 3

                While indiceRiga < righeStampa.Count
                    If y + altezzaRiga > e.MarginBounds.Bottom Then
                        e.HasMorePages = True
                        Return
                    End If

                    Dim row As DataGridViewRow = righeStampa(indiceRiga)
                    Dim stato As String = If(row.Cells(NomeColonnaStato).Value Is Nothing, "", row.Cells(NomeColonnaStato).Value.ToString())
                    Dim daEvidenziare As Boolean = String.Equals(stato, "Prezzo Eccessivo", StringComparison.OrdinalIgnoreCase) OrElse
                                                    String.Equals(stato, "Prezzo Inferiore", StringComparison.OrdinalIgnoreCase)
                    Dim fontRiga As Font = If(daEvidenziare, fontCellaEvidenziata, fontCella)

                    xCorrente = x
                    For i As Integer = 0 To colonneVisibili.Count - 1
                        Dim cella As DataGridViewCell = row.Cells(colonneVisibili(i).Index)
                        Dim testo As String = If(cella.FormattedValue Is Nothing, "", cella.FormattedValue.ToString())
                        Dim allineaDestra As Boolean = cella.ValueType Is GetType(Decimal) OrElse cella.ValueType Is GetType(Double) OrElse cella.ValueType Is GetType(Integer)
                        Dim rect As New RectangleF(xCorrente, y, larghezzeColonne(i), altezzaRiga)
                        gr.DrawString(testo, fontRiga, Brushes.Black, rect, If(allineaDestra, formatoNumero, formatoTesto))
                        xCorrente += larghezzeColonne(i)
                    Next

                    y += altezzaRiga
                    indiceRiga += 1
                End While

                e.HasMorePages = False
            End Sub

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
