Public Class frmMain

    Private Const LarghezzaRiquadro As Integer = 200
    Private Const AltezzaRiquadro As Integer = 220
    Private Const Margine As Integer = 20
    Private ReadOnly ColoreNormale As Color = Color.White
    Private ReadOnly ColoreHover As Color = Color.FromArgb(245, 247, 250)

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim riquadri As (Titolo As String, Icona As Bitmap, Azione As Action)() = {
            ("CARICAMENTO LISTINI FORNITORI", My.Resources.CaricamentoListini, AddressOf ApriCaricamentoListini),
            ("CONSOLIDAMENTO DATI", My.Resources.ConsolidamentoDati, AddressOf ApriConsolidamentoDati),
            ("IMPORTAZIONE FATTURE ELETTRONICHE", My.Resources.FattureElettroniche, AddressOf ApriImportFattureElettroniche),
            ("ANALISI PREZZI FATTURE", My.Resources.AnalisiPrezzi, AddressOf ApriAnalisiPrezziFatture),
            ("IMPOSTAZIONI", My.Resources.Impostazioni, AddressOf ApriImpostazioni)
        }

        Dim larghezzaTotale As Integer = riquadri.Length * LarghezzaRiquadro + (riquadri.Length + 1) * Margine
        Me.ClientSize = New Size(larghezzaTotale, AltezzaRiquadro + 2 * Margine)

        For i As Integer = 0 To riquadri.Length - 1
            Dim x As Integer = Margine + i * (LarghezzaRiquadro + Margine)
            Dim riquadro As Panel = CreaRiquadro(riquadri(i).Titolo, riquadri(i).Icona, riquadri(i).Azione)
            riquadro.Location = New Point(x, Margine)
            Me.Controls.Add(riquadro)
        Next
    End Sub

    Private Function CreaRiquadro(titolo As String, icona As Bitmap, azione As Action) As Panel
        Dim riquadro As New Panel() With {
            .Size = New Size(LarghezzaRiquadro, AltezzaRiquadro),
            .BorderStyle = BorderStyle.FixedSingle,
            .BackColor = ColoreNormale,
            .Cursor = Cursors.Hand
        }

        Dim pic As New PictureBox() With {
            .Dock = DockStyle.Top,
            .Height = 130,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Image = icona,
            .Cursor = Cursors.Hand
        }

        Dim lbl As New Label() With {
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Text = titolo,
            .Cursor = Cursors.Hand
        }

        riquadro.Controls.Add(lbl)
        riquadro.Controls.Add(pic)

        Dim evidenzia As EventHandler = Sub(sender As Object, e As EventArgs) riquadro.BackColor = ColoreHover
        Dim ripristina As EventHandler = Sub(sender As Object, e As EventArgs) riquadro.BackColor = ColoreNormale
        Dim clic As EventHandler = Sub(sender As Object, e As EventArgs) azione()

        For Each ctrl As Control In New Control() {riquadro, pic, lbl}
            AddHandler ctrl.MouseEnter, evidenzia
            AddHandler ctrl.MouseLeave, ripristina
            AddHandler ctrl.Click, clic
        Next

        Return riquadro
    End Function

    Private Sub ApriCaricamentoListini()
        Dim frm As New FrmEstrattore()
        frm.Show(Me)
    End Sub

    Private Sub ApriConsolidamentoDati()
        Dim frm As New frmConsolidamentoDati()
        frm.Show(Me)
    End Sub

    Private Sub ApriImportFattureElettroniche()
        Dim frm As New frmImportFattureElettroniche()
        frm.Show(Me)
    End Sub

    Private Sub ApriAnalisiPrezziFatture()
        Dim frm As New frmSceltaAnalisiPrezzi()
        frm.ShowDialog(Me)
    End Sub

    Private Sub ApriImpostazioni()
        Using frm As New frmImpostazioni()
            frm.ShowDialog(Me)
        End Using
    End Sub

End Class
