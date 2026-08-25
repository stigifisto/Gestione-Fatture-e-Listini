Public Class frmConsolidamentoDati

    Private Sub frmConsolidamentoDati_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dtpListiniDal.Value = Date.Today
        dtpFattureAS400Dal.Value = Date.Today
    End Sub

    Private Async Sub cmdImportaListini_ButtonClick(sender As Object, e As EventArgs) Handles cmdImportaListini.ButtonClick
        Dim data As String = dtpListiniDal.Value.Date.ToString("dd/MM/yyyy")
        If MessageBox.Show($"Confermi l'importazione dei listini da AS400 e Infinity dalla data {data}?",
                           "Importa listini", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            Return
        End If
        Await ImportaListiniDaAS400()
        Await ImportaListiniDaInfinity()
    End Sub

    Private Async Sub mnuImportaAS400_Click(sender As Object, e As EventArgs) Handles mnuImportaAS400.Click
        Dim data As String = dtpListiniDal.Value.Date.ToString("dd/MM/yyyy")
        If MessageBox.Show($"Confermi l'importazione del listino da AS400 dalla data {data}?",
                           "Importa listino AS400", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            Return
        End If
        Await ImportaListiniDaAS400()
    End Sub

    Private Async Sub mnuImportaInfinity_Click(sender As Object, e As EventArgs) Handles mnuImportaInfinity.Click
        Dim data As String = dtpListiniDal.Value.Date.ToString("dd/MM/yyyy")
        If MessageBox.Show($"Confermi l'importazione del listino da Infinity dalla data {data}?",
                           "Importa listino Infinity", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            Return
        End If
        Await ImportaListiniDaInfinity()
    End Sub

    Private Async Function ImportaListiniDaAS400() As Task
        Dim dataListini As Date = dtpListiniDal.Value.Date

        ProgressBar1.Style = ProgressBarStyle.Marquee
        ProgressBar1.Visible = True
        lblStatus.Text = "Lettura listino da AS400..."

        Dim dt As DataTable = Await Task.Run(Function() ModuloImportListiniInfinity.GetListinoDaAS400(dataListini))

        ProgressBar1.Style = ProgressBarStyle.Blocks
        ProgressBar1.Maximum = dt.Rows.Count
        ProgressBar1.Value = 0

        Dim progress As New Progress(Of Integer)(Sub(righe)
                                                     lblStatus.Text = $"AS400 – riga {righe} di {dt.Rows.Count}..."
                                                     ProgressBar1.Value = Math.Min(righe, ProgressBar1.Maximum)
                                                 End Sub)

        Await Task.Run(Sub() ModuloImportListiniInfinity.SalvaListinoSuSQL(dt, dataListini, progress))

        ProgressBar1.Visible = False
        ProgressBar1.Value = 0
        lblStatus.Text = ""
        MsgBox($"AS400: importazione completata — {dt.Rows.Count} righe inserite.")
    End Function

    Private Async Function ImportaListiniDaInfinity() As Task
        Dim dataListini As Date = dtpListiniDal.Value.Date

        ProgressBar1.Style = ProgressBarStyle.Marquee
        ProgressBar1.Visible = True
        lblStatus.Text = "Lettura listino da Infinity..."

        Dim dt As DataTable = Await Task.Run(Function() ModuloImportListiniInfinity.GetListinoInfinity(dataListini))

        ProgressBar1.Style = ProgressBarStyle.Blocks
        ProgressBar1.Maximum = dt.Rows.Count
        ProgressBar1.Value = 0

        Dim progress As New Progress(Of Integer)(Sub(righe)
                                                     lblStatus.Text = $"Infinity – riga {righe} di {dt.Rows.Count}..."
                                                     ProgressBar1.Value = Math.Min(righe, ProgressBar1.Maximum)
                                                 End Sub)

        Await Task.Run(Sub() ModuloImportListiniInfinity.SalvaListinoInfinitySQL(dt, dataListini, progress))

        ProgressBar1.Visible = False
        ProgressBar1.Value = 0
        lblStatus.Text = ""
        MsgBox($"Infinity: importazione completata — {dt.Rows.Count} righe inserite.")
    End Function

    Private Async Sub cmdImportaFattureAS400_Click(sender As Object, e As EventArgs) Handles cmdImportaFattureAS400.Click
        Dim data As String = dtpFattureAS400Dal.Value.Date.ToString("dd/MM/yyyy")
        If MessageBox.Show($"Confermi l'importazione delle fatture da AS400 dalla data {data}?",
                           "Importa fatture AS400", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            Return
        End If
        Await ImportaFattureDaAS400()
    End Sub

    Private Async Function ImportaFattureDaAS400() As Task
        Dim dataInizio As Date = dtpFattureAS400Dal.Value.Date

        ProgressBar1.Style = ProgressBarStyle.Marquee
        ProgressBar1.Visible = True
        lblStatus.Text = "Lettura fatture da AS400..."

        Try
            Dim dt As DataTable = Await Task.Run(Function() ModuloImportListiniInfinity.GetFattureDaAS400(dataInizio))

            ProgressBar1.Style = ProgressBarStyle.Blocks
            ProgressBar1.Maximum = dt.Rows.Count
            ProgressBar1.Value = 0

            Dim progress As New Progress(Of Integer)(Sub(righe)
                                                         lblStatus.Text = $"Fatture AS400 – riga {righe} di {dt.Rows.Count}..."
                                                         ProgressBar1.Value = Math.Min(righe, ProgressBar1.Maximum)
                                                     End Sub)

            Await Task.Run(Sub() ModuloImportListiniInfinity.SalvaFattureAS400SuSQL(dt, dataInizio, progress))

            MsgBox($"Fatture AS400: importazione completata — {dt.Rows.Count} righe inserite.")
        Catch ex As Exception
            MsgBox("Errore durante l'importazione delle fatture da AS400: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            ProgressBar1.Visible = False
            ProgressBar1.Value = 0
            lblStatus.Text = ""
        End Try
    End Function

End Class
