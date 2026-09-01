Imports System.IO

''' <summary>
''' Avvio pianificato (es. da Utilità di pianificazione di Windows) tramite l'argomento a riga
''' di comando "/listini": importa i listini AS400 e Infinity (da My.Settings.GiorniImportazioneListini
''' giorni prima di oggi) e le fatture AS400 (da My.Settings.GiorniImportazioneFattureAS400 giorni
''' prima, parametro indipendente) senza aprire l'interfaccia, registrando l'esito in un file di
''' log accanto all'eseguibile. Portato dal progetto originale "Importazione Fatture Elettroniche"
''' (My Project\ApplicationEvents.vb).
''' </summary>
Namespace My
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(sender As Object, e As ApplicationServices.StartupEventArgs) Handles Me.Startup
            If e.CommandLine.Contains("/listini") Then
                EseguiImportazioneNotturnaListini()
                e.Cancel = True
            End If
        End Sub

        Private Sub EseguiImportazioneNotturnaListini()
            Dim logPath As String = Path.Combine(
                Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location),
                "listini_notturno.log")

            Dim giorniListini As Integer = My.Settings.GiorniImportazioneListini
            Dim dataInizio As Date = Date.Today.AddDays(-giorniListini)

            ScriviLog(logPath, $"=== Avvio importazione notturna listini — data di partenza: {dataInizio:dd/MM/yyyy} ({giorniListini} giorni) ===")

            Try
                ScriviLog(logPath, "Importazione AS400 in corso...")
                Dim dtAS400 As DataTable = ModuloImportListiniInfinity.GetListinoDaAS400(dataInizio)
                If dtAS400.Rows.Count = 0 Then
                    ScriviLog(logPath, "ATTENZIONE AS400: nessuna riga recuperata, importazione saltata.")
                Else
                    ModuloImportListiniInfinity.SalvaListinoSuSQL(dtAS400, dataInizio)
                    ScriviLog(logPath, $"AS400 completato: {dtAS400.Rows.Count} righe inserite.")
                End If
            Catch ex As Exception
                ScriviLog(logPath, $"ERRORE AS400: {ex.Message}")
            End Try

            Try
                ScriviLog(logPath, "Importazione Infinity in corso...")
                Dim dtInfinity As DataTable = ModuloImportListiniInfinity.GetListinoInfinity(dataInizio)
                If dtInfinity.Rows.Count = 0 Then
                    ScriviLog(logPath, "ATTENZIONE Infinity: nessuna riga recuperata, importazione saltata.")
                Else
                    ModuloImportListiniInfinity.SalvaListinoInfinitySQL(dtInfinity, dataInizio)
                    ScriviLog(logPath, $"Infinity completato: {dtInfinity.Rows.Count} righe inserite.")
                End If
            Catch ex As Exception
                ScriviLog(logPath, $"ERRORE Infinity: {ex.Message}")
            End Try

            ScriviLog(logPath, "=== Importazione notturna listini terminata ===")

            Dim giorniFatture As Integer = My.Settings.GiorniImportazioneFattureAS400
            Dim dataInizioFatture As Date = Date.Today.AddDays(-giorniFatture)

            ScriviLog(logPath, $"=== Avvio importazione notturna fatture AS400 — data di partenza: {dataInizioFatture:dd/MM/yyyy} ({giorniFatture} giorni) ===")

            Try
                ScriviLog(logPath, "Importazione fatture AS400 in corso...")
                Dim dtFattureAS400 As DataTable = ModuloImportListiniInfinity.GetFattureDaAS400(dataInizioFatture)
                If dtFattureAS400.Rows.Count = 0 Then
                    ScriviLog(logPath, "ATTENZIONE Fatture AS400: nessuna riga recuperata, importazione saltata.")
                Else
                    ModuloImportListiniInfinity.SalvaFattureAS400SuSQL(dtFattureAS400, dataInizioFatture)
                    ScriviLog(logPath, $"Fatture AS400 completato: {dtFattureAS400.Rows.Count} righe inserite.")
                End If
            Catch ex As Exception
                ScriviLog(logPath, $"ERRORE Fatture AS400: {ex.Message}")
            End Try

            ScriviLog(logPath, "=== Importazione notturna fatture AS400 terminata ===")
        End Sub

        Private Sub ScriviLog(logPath As String, messaggio As String)
            File.AppendAllText(logPath, $"[{Now:yyyy-MM-dd HH:mm:ss}] {messaggio}{vbCrLf}")
        End Sub

    End Class
End Namespace
