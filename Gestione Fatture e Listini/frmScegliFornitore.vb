''' <summary>
''' Dialog di supporto per frmPrezziAS400 e frmPrezziFattureElettroniche: mostra, in ordine
''' alfabetico di descrizione, i soli fornitori che nel periodo selezionato hanno fatture,
''' con il relativo conteggio. Selezionando una riga la form si chiude con DialogResult.OK e
''' il chiamante legge CodiceFornitoreSelezionato/DescrizioneFornitoreSelezionato.
''' </summary>
Public Class frmScegliFornitore

    Private ReadOnly _dtFornitori As DataTable
    Private _codiceFornitoreSelezionato As String
    Private _descrizioneFornitoreSelezionato As String

    Public ReadOnly Property CodiceFornitoreSelezionato As String
        Get
            Return _codiceFornitoreSelezionato
        End Get
    End Property

    Public ReadOnly Property DescrizioneFornitoreSelezionato As String
        Get
            Return _descrizioneFornitoreSelezionato
        End Get
    End Property

    ''' <summary>
    ''' dtFornitori deve avere le colonne Codice, Descrizione, NumeroFatture, già ordinate per
    ''' Descrizione (l'ordinamento non viene rifatto qui).
    ''' </summary>
    Public Sub New(dtFornitori As DataTable)
        InitializeComponent()
        _dtFornitori = dtFornitori
    End Sub

    Private Sub frmScegliFornitore_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dgvFornitori.DataSource = _dtFornitori

        If dgvFornitori.Columns.Contains("Codice") Then
            dgvFornitori.Columns("Codice").Visible = False
        End If
        If dgvFornitori.Columns.Contains("Descrizione") Then
            dgvFornitori.Columns("Descrizione").HeaderText = "Fornitore"
            dgvFornitori.Columns("Descrizione").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        End If
        If dgvFornitori.Columns.Contains("NumeroFatture") Then
            dgvFornitori.Columns("NumeroFatture").HeaderText = "N. Fatture"
            dgvFornitori.Columns("NumeroFatture").Width = 90
            dgvFornitori.Columns("NumeroFatture").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End If

        lblInfo.Text = $"Nessun fornitore selezionato. Fornitori con fatture nel periodo ({_dtFornitori.Rows.Count}):"

        If dgvFornitori.Rows.Count > 0 Then
            dgvFornitori.Rows(0).Selected = True
            ' Cells(0) sarebbe la colonna "Codice", nascosta sopra: impostare CurrentCell su una
            ' colonna non visibile lancia InvalidOperationException, quindi si punta a una colonna
            ' visibile (la prima disponibile, cioè "Descrizione" se presente).
            Dim colonnaCorrente As DataGridViewColumn = If(dgvFornitori.Columns.Contains("Descrizione"),
                                                            dgvFornitori.Columns("Descrizione"),
                                                            dgvFornitori.Columns.Cast(Of DataGridViewColumn)().FirstOrDefault(Function(c) c.Visible))
            If colonnaCorrente IsNot Nothing Then
                dgvFornitori.CurrentCell = dgvFornitori.Rows(0).Cells(colonnaCorrente.Index)
            End If
        End If
    End Sub

    Private Sub dgvFornitori_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvFornitori.CellDoubleClick
        If e.RowIndex < 0 Then Exit Sub
        SelezionaRigaCorrente()
    End Sub

    Private Sub btnSeleziona_Click(sender As Object, e As EventArgs) Handles btnSeleziona.Click
        SelezionaRigaCorrente()
    End Sub

    Private Sub SelezionaRigaCorrente()
        If dgvFornitori.CurrentRow Is Nothing Then
            MsgBox("Seleziona un fornitore dalla griglia.")
            Exit Sub
        End If

        Dim rigaSelezionata As DataRowView = TryCast(dgvFornitori.CurrentRow.DataBoundItem, DataRowView)
        If rigaSelezionata Is Nothing Then Exit Sub

        _codiceFornitoreSelezionato = rigaSelezionata("Codice").ToString()
        _descrizioneFornitoreSelezionato = rigaSelezionata("Descrizione").ToString()

        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub btnAnnulla_Click(sender As Object, e As EventArgs) Handles btnAnnulla.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

End Class
