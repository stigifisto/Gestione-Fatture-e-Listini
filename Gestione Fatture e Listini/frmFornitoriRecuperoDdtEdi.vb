Imports System.Data.SqlClient

''' <summary>
''' Gestione della tabella FornitoriRecuperoDdtEdi (griglia editabile), raggiungibile da
''' frmImpostazioni: elenca i fornitori per i quali, in fattura elettronica, non è mai presente il
''' nodo RiferimentoNumeroLinea (es. Colosio Formaggi), per cui ModuloImportFattureXml.ImportaFatturaXml
''' tenta di ricostruire l'abbinamento riga/DDT incrociando CodiceArticolo+Quantita con i dati
''' ricevuti via canale EDI (tabelle WRXTSD/WRXRGD), invece di usare sempre la data fattura come
''' riferimento per il confronto con il listino.
''' </summary>
Public Class frmFornitoriRecuperoDdtEdi

    Private Sub frmFornitoriRecuperoDdtEdi_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CaricaGriglia()
    End Sub

    Private Sub CaricaGriglia()
        dgvFornitori.Rows.Clear()

        Dim sql As String = "SELECT ID_FiscaleIVA_Fornitore, Descrizione FROM FornitoriRecuperoDdtEdi ORDER BY ID_FiscaleIVA_Fornitore"

        Using conn As New SqlConnection(ModuloImportFattureXml.connectionString)
            Try
                conn.Open()
                Using cmd As New SqlCommand(sql, conn)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            dgvFornitori.Rows.Add(
                                reader("ID_FiscaleIVA_Fornitore").ToString(),
                                If(IsDBNull(reader("Descrizione")), "", reader("Descrizione").ToString()))
                        End While
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Errore nel caricamento dei fornitori: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Sub btnEliminaRiga_Click(sender As Object, e As EventArgs) Handles btnEliminaRiga.Click
        If dgvFornitori.CurrentRow Is Nothing OrElse dgvFornitori.CurrentRow.IsNewRow Then Return
        dgvFornitori.Rows.Remove(dgvFornitori.CurrentRow)
    End Sub

    Private Sub btnSalva_Click(sender As Object, e As EventArgs) Handles btnSalva.Click
        dgvFornitori.EndEdit()

        Dim righe As New List(Of (PIva As String, Descrizione As String))
        Dim pIveViste As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each row As DataGridViewRow In dgvFornitori.Rows
            If row.IsNewRow Then Continue For

            Dim pIva As String = If(row.Cells(colIdFiscaleIVA.Index).Value, "").ToString().Trim()
            Dim descrizione As String = If(row.Cells(colDescrizione.Index).Value, "").ToString().Trim()

            If String.IsNullOrEmpty(pIva) Then Continue For ' riga vuota, ignorata

            If Not pIveViste.Add(pIva) Then
                MessageBox.Show($"La partita IVA/codice fiscale ""{pIva}"" è presente più di una volta.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            righe.Add((pIva, descrizione))
        Next

        Using conn As New SqlConnection(ModuloImportFattureXml.connectionString)
            Try
                conn.Open()
                Using trans As SqlTransaction = conn.BeginTransaction()
                    Try
                        Using cmdDel As New SqlCommand("DELETE FROM FornitoriRecuperoDdtEdi", conn, trans)
                            cmdDel.ExecuteNonQuery()
                        End Using

                        For Each riga In righe
                            Using cmdIns As New SqlCommand("INSERT INTO FornitoriRecuperoDdtEdi (ID_FiscaleIVA_Fornitore, Descrizione) VALUES (@piva, @des)", conn, trans)
                                cmdIns.Parameters.AddWithValue("@piva", riga.PIva)
                                cmdIns.Parameters.AddWithValue("@des", If(String.IsNullOrEmpty(riga.Descrizione), DBNull.Value, riga.Descrizione))
                                cmdIns.ExecuteNonQuery()
                            End Using
                        Next

                        trans.Commit()
                    Catch
                        trans.Rollback()
                        Throw
                    End Try
                End Using
            Catch ex As Exception
                MessageBox.Show("Errore durante il salvataggio: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try
        End Using

        ModuloImportFattureXml.InvalidaCacheFornitoriRecuperoDdtEdi()

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnAnnulla_Click(sender As Object, e As EventArgs) Handles btnAnnulla.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class
