Public Class frmSceltaAnalisiPrezzi

    Private Sub cmdAnalisiAS400_Click(sender As Object, e As EventArgs) Handles cmdAnalisiAS400.Click
        Dim frm As New frmPrezziAS400()
        frm.Show()
    End Sub

    Private Sub cmdAnalisiFatture_Click(sender As Object, e As EventArgs) Handles cmdAnalisiFatture.Click
        Dim frm As New frmPrezziFattureElettroniche()
        frm.Show()
    End Sub

End Class
