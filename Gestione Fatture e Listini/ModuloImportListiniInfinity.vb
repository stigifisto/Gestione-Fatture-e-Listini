Imports System.Data.OleDb
Imports System.Data.SqlClient

''' <summary>
''' Logica di import listini AS400/Infinity e fatture AS400, estratta da frmAnomalie.vb
''' della soluzione "Importazione Fatture Elettroniche" (invariata nel corpo/SQL) per essere
''' riutilizzabile da frmConsolidamentoDati senza dipendere da un'istanza di quel form.
''' </summary>
Module ModuloImportListiniInfinity
    Public Const connectionString As String = "Server=192.168.2.19\inalcasql12;Database=infinitydb;User ID=infinity_UTENTE;Password=antonio.speziali"

    Public Function GetListinoDaAS400(dataInizio As Date) As DataTable
        Dim as400ConnString As String = "Provider=IBMDA400;Data Source=192.168.2.200;User Id=EDPUSER;Password=EDPUSER;"
        Dim dtListino As New DataTable()

        Using conn As New OleDbConnection(as400ConnString)
            Try
                conn.Open()
            Catch ex As OleDbException
                For i As Integer = 0 To ex.Errors.Count - 1
                    Console.WriteLine("Index #" & i.ToString() & ControlChars.CrLf &
                          "Message: " & ex.Errors(i).Message & ControlChars.CrLf &
                          "Native: " & ex.Errors(i).NativeError.ToString() & ControlChars.CrLf &
                          "Source: " & ex.Errors(i).Source & ControlChars.CrLf &
                          "SQL: " & ex.Errors(i).SQLState & ControlChars.CrLf)
                Next
            End Try

            Dim sql As String = "SELECT COFCOF AS CodiceArticoloFornitore, " &
                                "COFCOD AS CodiceArticolo, " &
                                "COFDEF AS DescrizioneArticolo, " &
                                "LIAPRZ AS PrezzoLordo, " &
                                "LIASC1 as Sconto1, LIASC2 as Sconto2, LIASC3 as Sconto3, LIASC4 as Sconto4, " &
                                "LIADAI AS DataInizioValidita, " &
                                "SOGPARTIVA AS PartitaIVAFornitore,  " &
                                "LIAFOR as Fornitore, " &
                                "CONCAT(LIALIS,LIAVAL) as ValidoPer, " &
                                "substring( ATBUNI , 1, 20) as DescrizioneListino " &
                                "FROM inadati.oalia01l left join inadati.anfrn03l on liasoc=frnsocieta and liafor=frnksc left join " &
                                "pjgruppo.ansog01l on frnsogg=sogsogg left join inadati.oacof01l on liasoc=cofsoc and liacod=cofcod and liafor=coffor left join pjgruppo.anatb00f " &
                                "on atbazi=liasoc and CONCAT(LIALIS,LIAVAL)=substring( ATBKEY, 10, 5) and atbcod='F01' " &
                                $" where liasoc='GRD' and LIALIS IN ('01', '02', '03', '04' , '05') and liaval='EUR ' And frnfiliale='0'  And liadai>='{dataInizio:yyyy-MM-dd}'" &
                                " and COFCOF is not null ORDER BY LIALIS, LIAFOR, COFCOF, LIADAI        "
            Using cmd As New OleDbCommand(sql, conn)
                Using reader As OleDbDataReader = cmd.ExecuteReader(CommandBehavior.SequentialAccess)
                    dtListino.Load(reader)
                End Using
            End Using
        End Using

        ' 2. Aggiungiamo la colonna DataFineValidita che l'AS400 non ha
        dtListino.Columns.Add("DataFineValidita", GetType(DateTime))

        ' 3. Ciclo per calcolare la data fine (logica discussa prima)
        For i As Integer = 0 To dtListino.Rows.Count - 1
            Application.DoEvents()
            Dim rigaCorrente = dtListino.Rows(i)

            If i < dtListino.Rows.Count - 1 Then
                Dim rigaSuccessiva = dtListino.Rows(i + 1)

                ' Se è lo stesso articolo, la fine è l'inizio del prossimo -1 giorno
                If rigaCorrente("CodiceArticoloFornitore").ToString() = rigaSuccessiva("CodiceArticoloFornitore").ToString() Then
                    Dim inizioSucc = Convert.ToDateTime(rigaSuccessiva("DataInizioValidita"))
                    rigaCorrente("DataFineValidita") = inizioSucc.AddDays(-1)
                Else
                    ' Ultimo prezzo di questo articolo
                    rigaCorrente("DataFineValidita") = New DateTime(2099, 12, 31)
                End If
            Else
                ' Ultima riga in assoluto del DataTable
                rigaCorrente("DataFineValidita") = New DateTime(2099, 12, 31)
            End If
        Next

        Return dtListino
    End Function

    Public Sub SalvaListinoSuSQL(dt As DataTable, dataInizio As Date, Optional progress As IProgress(Of Integer) = Nothing)
        Dim sqlDelete As String = "DELETE FROM Listini_Acquisto_As400 WHERE DataInizioValidita >= @dataInizio"
        Dim sqlInsert As String = "INSERT INTO Listini_Acquisto_As400 (" &
        "CodiceFornitore, ID_FiscaleIVA_Fornitore, CodiceArticolo, DescrizioneArticolo, " &
        "PrezzoLordo, Sconto1, Sconto2, Sconto3, Sconto4, DataInizioValidita, DataFineValidita, ValidoPer, DescrizioneListino) " &
        "VALUES (@codfor, @piva, @cdart, @desc, @prezzo, @s1, @s2, @s3, @s4, @inizio, @fine, @valper, @desclis)"

        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Using trans = conn.BeginTransaction()
                Try
                    Using cmdDel As New SqlCommand(sqlDelete, conn, trans)
                        cmdDel.Parameters.AddWithValue("@dataInizio", dataInizio)
                        cmdDel.ExecuteNonQuery()
                    End Using

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim row As DataRow = dt.Rows(i)

                        Using cmd As New SqlCommand(sqlInsert, conn, trans)
                            cmd.Parameters.AddWithValue("@codfor", row("Fornitore"))
                            cmd.Parameters.AddWithValue("@piva", row("PartitaIVAFornitore"))
                            cmd.Parameters.AddWithValue("@cdart", row("CodiceArticolo"))
                            cmd.Parameters.AddWithValue("@desc", row("DescrizioneArticolo"))
                            cmd.Parameters.AddWithValue("@prezzo", row("PrezzoLordo"))
                            cmd.Parameters.AddWithValue("@s1", If(IsDBNull(row("Sconto1")), 0, row("Sconto1")))
                            cmd.Parameters.AddWithValue("@s2", If(IsDBNull(row("Sconto2")), 0, row("Sconto2")))
                            cmd.Parameters.AddWithValue("@s3", If(IsDBNull(row("Sconto3")), 0, row("Sconto3")))
                            cmd.Parameters.AddWithValue("@s4", If(IsDBNull(row("Sconto4")), 0, row("Sconto4")))
                            cmd.Parameters.AddWithValue("@inizio", row("DataInizioValidita"))
                            cmd.Parameters.AddWithValue("@fine", row("DataFineValidita"))
                            cmd.Parameters.AddWithValue("@valper", row("ValidoPer"))
                            cmd.Parameters.AddWithValue("@desclis", row("DescrizioneListino"))
                            cmd.ExecuteNonQuery()
                        End Using

                        If i Mod 100 = 99 Then progress?.Report(i + 1)
                    Next
                    trans.Commit()
                    progress?.Report(dt.Rows.Count)
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Sub

    Public Function GetFattureDaAS400(dataInizio As Date) As DataTable
        Dim as400ConnString As String = "Provider=IBMDA400;Data Source=192.168.2.200;User Id=EDPUSER;Password=EDPUSER;"
        Dim dtFatture As New DataTable()

        Using conn As New OleDbConnection(as400ConnString)
            Try
                conn.Open()
            Catch ex As OleDbException
                For i As Integer = 0 To ex.Errors.Count - 1
                    Console.WriteLine("Index #" & i.ToString() & ControlChars.CrLf &
                          "Message: " & ex.Errors(i).Message & ControlChars.CrLf &
                          "Native: " & ex.Errors(i).NativeError.ToString() & ControlChars.CrLf &
                          "Source: " & ex.Errors(i).Source & ControlChars.CrLf &
                          "SQL: " & ex.Errors(i).SQLState & ControlChars.CrLf)
                Next
            End Try

            Dim sql As String = "SELECT DISTINCT MOAFOR, MOACOD, MOADES, MOAQTA, MOAPRZ, " &
                                "MOAVAV, MOASC1, MOASC2, MOASC3, MOASC4, MOACDC, entdescr, MOADBO, " &
                                "MOANBO, MOADFT, MOANFT " &
                                "FROM inadati.oamoa06l " &
                                "left join inadati.anent02l on moasoc = entsocieta and ENTTPRIFAN ='CDC' and moacdc = ententita " &
                                $"WHERE moasoc = 'GRD' and moadbo >= '{dataInizio:yyyy-MM-dd}' " &
                                "AND MOACOD BETWEEN '000001' AND '999999' " &
                                "ORDER BY moafor, moadbo, moanbo, moacod"

            Using cmd As New OleDbCommand(sql, conn)
                Using reader As OleDbDataReader = cmd.ExecuteReader(CommandBehavior.SequentialAccess)
                    dtFatture.Load(reader)
                End Using
            End Using
        End Using

        Return dtFatture
    End Function

    Public Sub SalvaFattureAS400SuSQL(dt As DataTable, dataInizio As Date, Optional progress As IProgress(Of Integer) = Nothing)
        Dim sqlDelete As String = "DELETE FROM Fatture_AS400 WHERE MOADBO >= @dataInizio"
        Dim sqlInsert As String = "INSERT INTO Fatture_AS400 (" &
        "MOAFOR, MOACOD, MOADES, MOAQTA, MOAPRZ, MOAVAV, MOASC1, MOASC2, MOASC3, MOASC4, MOACDC, ENTDESCR, MOADBO, MOANBO, MOADFT, MOANFT) " &
        "VALUES (@moafor, @moacod, @moades, @moaqta, @moaprz, @moavav, @moasc1, @moasc2, @moasc3, @moasc4, @moacdc, @entdescr, @moadbo, @moanbo, @moadft, @moanft)"

        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Using trans = conn.BeginTransaction()
                Try
                    Using cmdDel As New SqlCommand(sqlDelete, conn, trans)
                        cmdDel.Parameters.AddWithValue("@dataInizio", dataInizio)
                        cmdDel.ExecuteNonQuery()
                    End Using

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim row As DataRow = dt.Rows(i)

                        Using cmd As New SqlCommand(sqlInsert, conn, trans)
                            cmd.Parameters.AddWithValue("@moafor", row("MOAFOR"))
                            cmd.Parameters.AddWithValue("@moacod", row("MOACOD"))
                            cmd.Parameters.AddWithValue("@moades", row("MOADES"))
                            cmd.Parameters.AddWithValue("@moaqta", row("MOAQTA"))
                            cmd.Parameters.AddWithValue("@moaprz", row("MOAPRZ"))
                            cmd.Parameters.AddWithValue("@moavav", row("MOAVAV"))
                            cmd.Parameters.AddWithValue("@moasc1", If(IsDBNull(row("MOASC1")), 0, row("MOASC1")))
                            cmd.Parameters.AddWithValue("@moasc2", If(IsDBNull(row("MOASC2")), 0, row("MOASC2")))
                            cmd.Parameters.AddWithValue("@moasc3", If(IsDBNull(row("MOASC3")), 0, row("MOASC3")))
                            cmd.Parameters.AddWithValue("@moasc4", If(IsDBNull(row("MOASC4")), 0, row("MOASC4")))
                            cmd.Parameters.AddWithValue("@moacdc", If(IsDBNull(row("MOACDC")), DBNull.Value, row("MOACDC")))
                            cmd.Parameters.AddWithValue("@entdescr", If(IsDBNull(row("entdescr")), DBNull.Value, row("entdescr")))
                            cmd.Parameters.AddWithValue("@moadbo", row("MOADBO"))
                            cmd.Parameters.AddWithValue("@moanbo", row("MOANBO"))
                            cmd.Parameters.AddWithValue("@moadft", row("MOADFT"))
                            cmd.Parameters.AddWithValue("@moanft", row("MOANFT"))
                            cmd.ExecuteNonQuery()
                        End Using

                        If i Mod 100 = 99 Then progress?.Report(i + 1)
                    Next
                    trans.Commit()
                    progress?.Report(dt.Rows.Count)
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Sub

    Public Function GetListinoInfinity(dataInizio As Date) As DataTable
        Dim dtListino As New DataTable()

        Using conn As New SqlConnection(connectionString)
            conn.Open()

            Dim sql As String =
                "select concat('00', substring(ZX831_Codice_Fornitore, 4 ,6)) AS [FORNITORE], " &
                "dbo.ba_keysog001.KSCODIVA AS PartitaIVAFornitore, " &
                "dbo.ZX831_Articoli_PLU001.ZX831_CodArt_Fornitore AS [CodiceArticoloFornitore], " &
                "ARCODART as [CodiceArticolo], ARDESART AS [DescrizioneArticolo], LDPREZZO AS [PREZZO], " &
                "LDINIVAL as [DataInizioValidita], ldendval as [DataFineValidita], '01EUR' as ValidoPer " &
                "from ba_lisdet001 " &
                "left join dbo.ba_artico001 ON dbo.ba_artico001.ARFLGART = dbo.ba_lisdet001.LDFLGART AND dbo.ba_artico001.ARKEYART = dbo.ba_lisdet001.LDCODART " &
                "left join dbo.ba_artmod ON dbo.ba_artmod.ARFLGART = dbo.ba_artico001.ARFLGART AND dbo.ba_artmod.ARKEYART = dbo.ba_artico001.ARKEYART " &
                "left join dbo.ZX831_Articoli_PLU001 ON dbo.ZX831_Articoli_PLU001.FLGART = dbo.ba_artmod.ARFLGART AND dbo.ZX831_Articoli_PLU001.KEYART = dbo.ba_artmod.ARKEYART " &
                "left join ba_keysog001 on dbo.ZX831_Articoli_PLU001.TIPFOR = ba_keysog001.KSTIPSOG And dbo.ZX831_Articoli_PLU001.codfor = dbo.ba_keysog001.KSCODSOG " &
                "where LDCODLIS = '01 SLA BASE TUTTI   ' and KSCODIVA is not null and ldendval > @dataInizio " &
                "and ZX831_CodArt_Fornitore<>' ' " &
                "order by concat('00', substring(ZX831_Codice_Fornitore, 4 ,6)), ZX831_CodArt_Fornitore, LDINIVAL"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@dataInizio", dataInizio)
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    dtListino.Load(reader)
                End Using
            End Using
        End Using

        ' Aggiunge la colonna CodiceArticoloAs400, recuperata da AS400 (inadati.oacof00f)
        ' incrociando fornitore + codice articolo fornitore
        Dim colCodArtAs400 = dtListino.Columns.Add("CodiceArticoloAs400", GetType(String))
        colCodArtAs400.SetOrdinal(dtListino.Columns("CodiceArticoloFornitore").Ordinal + 1)

        Dim fornitoriDistinti = dtListino.AsEnumerable().
            Select(Function(r) r.Field(Of String)("FORNITORE")).
            Where(Function(f) Not String.IsNullOrWhiteSpace(f)).
            Distinct().ToList()

        Dim mappaCodiciAs400 = GetMappaCodiciArticoloAs400(fornitoriDistinti)

        For Each row As DataRow In dtListino.Rows
            Dim chiave As String = row.Field(Of String)("FORNITORE") & "|" & row.Field(Of String)("CodiceArticoloFornitore")
            Dim codArtAs400 As String = Nothing
            If mappaCodiciAs400.TryGetValue(chiave, codArtAs400) Then
                row("CodiceArticoloAs400") = codArtAs400
            Else
                row("CodiceArticoloAs400") = DBNull.Value
            End If
        Next

        Return dtListino
    End Function

    ''' <summary>
    ''' Recupera da AS400 (inadati.oacof00f) la mappa CodiceArticoloAs400 (COFCOD)
    ''' indicizzata per "Fornitore|CodiceArticoloFornitore" (COFFOR|COFCOF), per i soli
    ''' fornitori indicati. Un'unica query per tutti i fornitori invece di una query per riga.
    ''' </summary>
    Private Function GetMappaCodiciArticoloAs400(fornitori As List(Of String)) As Dictionary(Of String, String)
        Dim mappa As New Dictionary(Of String, String)
        If fornitori Is Nothing OrElse fornitori.Count = 0 Then Return mappa

        Dim as400ConnString As String = "Provider=IBMDA400;Data Source=192.168.2.200;User Id=EDPUSER;Password=EDPUSER;"
        Dim inClause As String = String.Join(",", fornitori.Select(Function(f) "'" & f.Replace("'", "''") & "'"))

        Using conn As New OleDbConnection(as400ConnString)
            conn.Open()

            Dim sql As String = "SELECT COFFOR, COFCOF, COFCOD FROM inadati.oacof00f " &
                                $"WHERE COFSOC='GRD' AND COFFOR IN ({inClause}) AND COFCOF <> ' '"

            Using cmd As New OleDbCommand(sql, conn)
                Using reader As OleDbDataReader = cmd.ExecuteReader(CommandBehavior.SequentialAccess)
                    While reader.Read()
                        Dim fornitore As String = reader("COFFOR").ToString().Trim()
                        Dim codArtFornitore As String = reader("COFCOF").ToString().Trim()
                        Dim codArtAs400 As String = reader("COFCOD").ToString().Trim()
                        Dim chiave As String = fornitore & "|" & codArtFornitore

                        If Not mappa.ContainsKey(chiave) Then
                            mappa(chiave) = codArtAs400
                        End If
                    End While
                End Using
            End Using
        End Using

        Return mappa
    End Function

    Public Sub SalvaListinoInfinitySQL(dt As DataTable, dataInizio As Date, Optional progress As IProgress(Of Integer) = Nothing)
        Dim sqlDelete As String = "DELETE FROM Listini_Acquisto_Infinity WHERE DataFineValidita > @dataInizio"
        Dim sqlInsert As String = "INSERT INTO Listini_Acquisto_Infinity (" &
            "CodiceFornitore, ID_FiscaleIVA_Fornitore, CodiceArticoloFornitore, CodiceArticoloAs400, CodiceArticolo, DescrizioneArticolo, " &
            "PrezzoLordo, DataInizioValidita, DataFineValidita, ValidoPer) " &
            "VALUES (@codfor, @piva, @art, @artas400, @cdart, @desc, @prezzo, @inizio, @fine, @valper)"

        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Using trans = conn.BeginTransaction()
                Try
                    Using cmdDel As New SqlCommand(sqlDelete, conn, trans)
                        cmdDel.Parameters.AddWithValue("@dataInizio", dataInizio)
                        cmdDel.ExecuteNonQuery()
                    End Using

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim row As DataRow = dt.Rows(i)

                        Using cmd As New SqlCommand(sqlInsert, conn, trans)
                            cmd.Parameters.AddWithValue("@codfor", row("FORNITORE"))
                            cmd.Parameters.AddWithValue("@piva", row("PartitaIVAFornitore"))
                            cmd.Parameters.AddWithValue("@art", row("CodiceArticoloFornitore"))
                            cmd.Parameters.AddWithValue("@artas400", If(IsDBNull(row("CodiceArticoloAs400")), DBNull.Value, row("CodiceArticoloAs400")))
                            cmd.Parameters.AddWithValue("@cdart", If(IsDBNull(row("CodiceArticolo")), DBNull.Value, row("CodiceArticolo")))
                            cmd.Parameters.AddWithValue("@desc", If(IsDBNull(row("DescrizioneArticolo")), "", row("DescrizioneArticolo")))
                            cmd.Parameters.AddWithValue("@prezzo", row("PREZZO"))
                            cmd.Parameters.AddWithValue("@inizio", row("DataInizioValidita"))
                            cmd.Parameters.AddWithValue("@fine", row("DataFineValidita"))
                            cmd.Parameters.AddWithValue("@valper", row("ValidoPer"))
                            cmd.ExecuteNonQuery()
                        End Using

                        If i Mod 100 = 99 Then progress?.Report(i + 1)
                    Next
                    trans.Commit()
                    progress?.Report(dt.Rows.Count)
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Sub
End Module
