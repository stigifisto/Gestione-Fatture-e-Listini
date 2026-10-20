Imports System.Data.SqlClient
Imports System.IO
Imports System.IO.Compression
Imports System.Security.Cryptography.Pkcs
Imports System.Text.RegularExpressions
Imports System.Xml.Linq

''' <summary>
''' Logica di decompressione ZIP, rimozione firma digitale e import fatture elettroniche da XML,
''' estratta da frmAnomalie.vb della soluzione "Importazione Fatture Elettroniche" (invariata nel
''' corpo/SQL) per essere riutilizzabile da frmImportFattureElettroniche senza dipendere da
''' un'istanza di quel form.
''' </summary>
Module ModuloImportFattureXml
    Public Const connectionString As String = "Server=192.168.2.19\inalcasql12;Database=infinitydb;User ID=infinity_UTENTE;Password=antonio.speziali"

    ''' <summary>
    ''' Cache dei CedenteIdCodice (P.IVA/codice fiscale) dei fornitori configurati in
    ''' FornitoriPrezzoInDescrizione (gestiti da frmFornitoriPrezzoInDescrizione, raggiungibile da
    ''' Impostazioni). Caricata una sola volta per ciclo di importazione; InvalidaCache va chiamata
    ''' dopo ogni modifica alla tabella per non dover riavviare l'applicazione.
    ''' </summary>
    Private _fornitoriPrezzoInDescrizione As HashSet(Of String) = Nothing

    Public Sub InvalidaCacheFornitoriPrezzoInDescrizione()
        _fornitoriPrezzoInDescrizione = Nothing
    End Sub

    Private Function EFornitoreConPrezzoInDescrizione(cedenteIdCodice As String, conn As SqlConnection, trans As SqlTransaction) As Boolean
        If _fornitoriPrezzoInDescrizione Is Nothing Then
            _fornitoriPrezzoInDescrizione = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Using cmd As New SqlCommand("SELECT ID_FiscaleIVA_Fornitore FROM FornitoriPrezzoInDescrizione", conn, trans)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        _fornitoriPrezzoInDescrizione.Add(reader.GetString(0).Trim())
                    End While
                End Using
            End Using
        End If

        Return Not String.IsNullOrEmpty(cedenteIdCodice) AndAlso _fornitoriPrezzoInDescrizione.Contains(cedenteIdCodice.Trim())
    End Function

    ''' <summary>
    ''' Per i fornitori configurati in FornitoriPrezzoInDescrizione, quantità (in kg) e prezzo al kg
    ''' reali non sono nei nodi standard Quantita/PrezzoUnitario (che riportano invece numero di
    ''' colli e prezzo per collo), ma nel testo libero della Descrizione, nella forma
    ''' "... &lt;qta&gt; prezzo al kg &lt;prezzo&gt; Lotto n. ...": es.
    ''' "PEC. ADRIANUS Ø 22 BASSO KG. 3,800 c.a kg.       30,50 prezzo al kg       15,650 Lotto n.239-25"
    ''' dove la quantità reale è 30,50 kg al prezzo di 15,650 €/kg (e non i 10 colli/47,73 € del nodo
    ''' XML). Lo sconto di riga (Fatture_Sconti) si applica comunque sul prezzo al kg così ottenuto.
    ''' </summary>
    Private ReadOnly RegexPrezzoAlKgInDescrizione As New Regex(
        "(\d+(?:[.,]\d+)?)\s+prezzo\s+al\s*kg[.:]?\s+(\d+(?:[.,]\d+)?)",
        RegexOptions.IgnoreCase)

    Private Function ProvaEstraiQuantitaEPrezzoDaDescrizione(descrizione As String, ByRef quantitaKg As Decimal, ByRef prezzoAlKg As Decimal) As Boolean
        quantitaKg = 0
        prezzoAlKg = 0
        If String.IsNullOrEmpty(descrizione) Then Return False

        Dim m = RegexPrezzoAlKgInDescrizione.Match(descrizione)
        If Not m.Success Then Return False

        Dim okQ = Decimal.TryParse(m.Groups(1).Value.Replace(",", "."), Globalization.NumberStyles.Number, Globalization.CultureInfo.InvariantCulture, quantitaKg)
        Dim okP = Decimal.TryParse(m.Groups(2).Value.Replace(",", "."), Globalization.NumberStyles.Number, Globalization.CultureInfo.InvariantCulture, prezzoAlKg)
        Return okQ AndAlso okP AndAlso quantitaKg > 0 AndAlso prezzoAlKg > 0
    End Function

    ''' <summary>
    ''' Cache dei CedenteIdCodice (P.IVA/codice fiscale) dei fornitori configurati in
    ''' FornitoriRecuperoDdtEdi (gestiti da frmFornitoriRecuperoDdtEdi, raggiungibile da
    ''' Impostazioni): fornitori che non indicano in fattura elettronica alcun
    ''' RiferimentoNumeroLinea per i DDT (es. Colosio Formaggi), per i quali si tenta di ricostruire
    ''' l'abbinamento riga/DDT incrociando i dati ricevuti via canale EDI (tabelle WRXTSD/WRXRGD).
    ''' </summary>
    Private _fornitoriRecuperoDdtEdi As HashSet(Of String) = Nothing

    Public Sub InvalidaCacheFornitoriRecuperoDdtEdi()
        _fornitoriRecuperoDdtEdi = Nothing
    End Sub

    Private Function EFornitoreRecuperoDdtEdi(cedenteIdCodice As String, conn As SqlConnection, trans As SqlTransaction) As Boolean
        If _fornitoriRecuperoDdtEdi Is Nothing Then
            _fornitoriRecuperoDdtEdi = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Using cmd As New SqlCommand("SELECT ID_FiscaleIVA_Fornitore FROM FornitoriRecuperoDdtEdi", conn, trans)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        _fornitoriRecuperoDdtEdi.Add(reader.GetString(0).Trim())
                    End While
                End Using
            End Using
        End If

        Return Not String.IsNullOrEmpty(cedenteIdCodice) AndAlso _fornitoriRecuperoDdtEdi.Contains(cedenteIdCodice.Trim())
    End Function

    ''' <summary>
    ''' Cache P.IVA/codice fiscale fornitore -&gt; codice gestionale (KSCODSOG), risolto tramite
    ''' l'anagrafica fornitori ba_keysog001 (KSTIPSOG = 'FOR'), necessario per interrogare le tabelle
    ''' EDI WRXTSD/WRXRGD che indicizzano i documenti per codice gestionale e non per P.IVA.
    ''' </summary>
    Private ReadOnly _codiciGestionaliFornitori As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

    Private Function GetCodiceGestionaleFornitore(cedenteIdCodice As String, conn As SqlConnection, trans As SqlTransaction) As String
        If String.IsNullOrEmpty(cedenteIdCodice) Then Return Nothing
        Dim chiave = cedenteIdCodice.Trim()

        Dim codice As String = Nothing
        If _codiciGestionaliFornitori.TryGetValue(chiave, codice) Then Return codice

        Dim sql = "SELECT TOP 1 KSCODSOG FROM ba_keysog001 WHERE KSTIPSOG = 'FOR' AND (RTRIM(KSCODFIS) = @piva OR RTRIM(KSCODIVA) = @piva)"
        Using cmd As New SqlCommand(sql, conn, trans)
            cmd.Parameters.AddWithValue("@piva", chiave)
            Dim risultato = cmd.ExecuteScalar()
            codice = If(risultato Is Nothing OrElse risultato Is DBNull.Value, Nothing, risultato.ToString().Trim())
        End Using

        _codiciGestionaliFornitori(chiave) = codice
        Return codice
    End Function

    ''' <summary>
    ''' Estrae la parte numerica (intera) del numero DDT, es. "1/8609" -&gt; 8609, per confrontarla col
    ''' codice documento EDI (RXTSDCAT, es. "008609"), che riporta lo stesso numero senza prefisso e
    ''' con zeri di riempimento a sinistra.
    ''' </summary>
    Private Function EstraiNumeroDdtIntero(numeroDdt As String) As Integer?
        If String.IsNullOrEmpty(numeroDdt) Then Return Nothing
        Dim parteNumerica = numeroDdt
        Dim idxSlash = numeroDdt.LastIndexOf("/"c)
        If idxSlash >= 0 Then parteNumerica = numeroDdt.Substring(idxSlash + 1)

        Dim valore As Integer
        If Integer.TryParse(parteNumerica.Trim(), valore) Then Return valore
        Return Nothing
    End Function

    ''' <summary>
    ''' Recupera, per il codice gestionale e il DDT (numero + data) indicati, le coppie
    ''' CodiceArticolo/Quantita ricevute via EDI (WRXTSD/WRXRGD), da incrociare con le righe della
    ''' fattura elettronica dello stesso fornitore per ricostruire l'abbinamento riga/DDT quando
    ''' l'XML non lo fornisce. Il match tra numero DDT XML ed EDI avviene su numero (parte intera) +
    ''' data, perché il solo numero si ripete tra anni diversi.
    ''' </summary>
    Private Function GetRigheEdiPerDdt(codiceGestionale As String, numeroDdt As String, dataDdt As String, conn As SqlConnection, trans As SqlTransaction) As List(Of (CodiceArticolo As String, Quantita As Decimal))
        Dim risultato As New List(Of (CodiceArticolo As String, Quantita As Decimal))

        Dim numeroDdtIntero = EstraiNumeroDdtIntero(numeroDdt)
        Dim dataDdtParsata As Date
        If String.IsNullOrEmpty(codiceGestionale) OrElse Not numeroDdtIntero.HasValue OrElse
           String.IsNullOrEmpty(dataDdt) OrElse Not Date.TryParse(dataDdt, Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dataDdtParsata) Then
            Return risultato
        End If

        Dim sql = "
            SELECT RXRGDPRF, RXRGDQT1
            FROM WRXTSD
            INNER JOIN WRXRGD ON RXTSDCOD = RXRGDCOD
            WHERE RXTSDANA = @cod
              AND RXTSDDTD = @data
              AND TRY_CAST(RXTSDCAT AS INT) = @numddt"

        Using cmd As New SqlCommand(sql, conn, trans)
            cmd.Parameters.AddWithValue("@cod", codiceGestionale)
            cmd.Parameters.AddWithValue("@data", dataDdtParsata.ToString("yyyyMMdd"))
            cmd.Parameters.AddWithValue("@numddt", numeroDdtIntero.Value)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    If reader.IsDBNull(0) Then Continue While
                    Dim codArt = reader.GetString(0).Trim()
                    If String.IsNullOrEmpty(codArt) Then Continue While
                    Dim qta As Decimal = If(reader.IsDBNull(1), 0D, CDec(reader.GetDouble(1)))
                    risultato.Add((codArt, qta))
                End While
            End Using
        End Using

        Return risultato
    End Function

    Public Function DecomprimiZip(Optional progress As IProgress(Of String) = Nothing) As String
        Dim cartellaZip As String = My.Settings.CartellaZip
        Dim cartellaDecompressi As String = My.Settings.CartellaDecompressi

        If Not IO.Directory.Exists(cartellaZip) Then
            Return "Decompressione ZIP: la cartella di download non esiste (" & cartellaZip & ")."
        End If
        If Not IO.Directory.Exists(cartellaDecompressi) Then IO.Directory.CreateDirectory(cartellaDecompressi)

        Dim fileZip As String() = IO.Directory.GetFiles(cartellaZip, "*.zip")
        If fileZip.Length = 0 Then
            Return "Decompressione ZIP: nessun file ZIP trovato nella cartella di download."
        End If

        Dim estratti As Integer = 0
        Dim sovrascritti As Integer = 0
        Dim sbDoppi As New System.Text.StringBuilder()

        For i As Integer = 0 To fileZip.Length - 1
            Dim pathZip As String = fileZip(i)
            Dim nomeZip As String = IO.Path.GetFileName(pathZip)
            progress?.Report(String.Format("Archivio {0}/{1}: {2}...", i + 1, fileZip.Length, nomeZip))

            Using archivio As ZipArchive = ZipFile.OpenRead(pathZip)
                For Each entry As ZipArchiveEntry In archivio.Entries
                    If String.IsNullOrEmpty(entry.Name) Then Continue For
                    Dim destPath As String = IO.Path.Combine(cartellaDecompressi, entry.Name)
                    If IO.File.Exists(destPath) Then
                        sovrascritti += 1
                        sbDoppi.AppendLine(entry.Name)
                        IO.File.Delete(destPath)
                    End If
                    entry.ExtractToFile(destPath)
                    estratti += 1
                Next
            End Using
            IO.File.Delete(pathZip)
            progress?.Report(String.Format("Archivio {0}/{1} completato — {2} file estratti finora", i + 1, fileZip.Length, estratti - sovrascritti))
        Next

        Dim fileUnici As Integer = estratti - sovrascritti
        Dim msg As String = String.Format("Decompressione ZIP completata: {0} file estratti da {1} archivio/i.", fileUnici, fileZip.Length)
        If sovrascritti > 0 Then
            msg &= String.Format("{0}{0}Attenzione: {1} file presenti in più archivi (ultima versione mantenuta):{0}{2}", vbCrLf, sovrascritti, sbDoppi.ToString())
        End If
        Return msg
    End Function

    ''' <summary>
    ''' Rimuove la firma digitale (.p7m) dai file decompressi e li deposita, come XML puro,
    ''' nella cartella pronta per l'importazione. I file XML non firmati (es. metadati) vengono
    ''' semplicemente spostati. I file di origine vengono rimossi una volta elaborati.
    ''' </summary>
    Public Function RimuoviFirmaDigitale(Optional progress As IProgress(Of String) = Nothing) As String
        Dim cartellaDecompressi As String = My.Settings.CartellaDecompressi
        Dim cartellaXmlPronti As String = My.Settings.CartellaXmlPronti

        If Not IO.Directory.Exists(cartellaDecompressi) Then
            Return "Rimozione firma: la cartella dei file decompressi non esiste (" & cartellaDecompressi & ")."
        End If
        If Not IO.Directory.Exists(cartellaXmlPronti) Then IO.Directory.CreateDirectory(cartellaXmlPronti)

        Dim files As String() = IO.Directory.GetFiles(cartellaDecompressi)
        If files.Length = 0 Then
            Return "Rimozione firma: nessun file da elaborare nella cartella dei decompressi."
        End If

        Dim contatoreP7M As Integer = 0
        Dim contatoreCopiati As Integer = 0
        Dim contatoreErrori As Integer = 0
        Dim sbErrori As New System.Text.StringBuilder()

        For i As Integer = 0 To files.Length - 1
            Dim file As String = files(i)
            Dim nomeFileLower As String = IO.Path.GetFileName(file).ToLower()
            progress?.Report(String.Format("Rimozione firma {0}/{1}: {2}", i + 1, files.Length, IO.Path.GetFileName(file)))
            Try
                If nomeFileLower.EndsWith(".p7m") Then
                    Dim baseName As String = nomeFileLower
                    If baseName.EndsWith(".xml.p7m") Then
                        baseName = baseName.Substring(0, baseName.Length - 8)
                    Else
                        baseName = baseName.Substring(0, baseName.Length - 4)
                    End If

                    Dim datiFirmati As Byte() = IO.File.ReadAllBytes(file)
                    Dim cms As New SignedCms()
                    cms.Decode(datiFirmati)
                    Dim datiOriginali As Byte() = cms.ContentInfo.Content

                    Dim destPath As String = GeneraNomeUnivoco(cartellaXmlPronti, baseName)
                    IO.File.WriteAllBytes(destPath, datiOriginali)
                    IO.File.Delete(file)
                    contatoreP7M += 1
                ElseIf nomeFileLower.EndsWith(".xml") Then
                    Dim baseName As String = nomeFileLower.Substring(0, nomeFileLower.Length - 4)
                    Dim destPath As String = GeneraNomeUnivoco(cartellaXmlPronti, baseName)
                    IO.File.Move(file, destPath)
                    contatoreCopiati += 1
                End If
            Catch ex As Exception
                contatoreErrori += 1
                sbErrori.AppendLine(IO.Path.GetFileName(file) & ": " & ex.Message)
            End Try
        Next

        Dim messaggio As String = String.Format(
            "Rimozione firma completata.{0}File P7M convertiti: {1}{0}File XML spostati: {2}{0}Errori: {3}",
            vbCrLf, contatoreP7M, contatoreCopiati, contatoreErrori)
        If sbErrori.Length > 0 Then
            messaggio &= vbCrLf & vbCrLf & "Dettaglio errori:" & vbCrLf & sbErrori.ToString()
        End If
        Return messaggio
    End Function

    ''' <summary>
    ''' Genera, nella cartella indicata, un percorso file .xml univoco a partire dal nome base
    ''' (senza estensione), aggiungendo un suffisso numerico in caso di collisione.
    ''' </summary>
    Private Function GeneraNomeUnivoco(cartella As String, nomeBase As String) As String
        Dim destPath As String = IO.Path.Combine(cartella, nomeBase & ".xml")
        Dim i As Integer = 1
        While IO.File.Exists(destPath)
            destPath = IO.Path.Combine(cartella, nomeBase & "(" & i & ").xml")
            i += 1
        End While
        Return destPath
    End Function

    Public Function ImportaFattureDaXml(Optional progress As IProgress(Of String) = Nothing) As String
        Dim cartellaP7M As String = My.Settings.CartellaXmlPronti
        Dim cartellaArchivio As String = My.Settings.CartellaBackup
        Dim cartellaDuplicati As String = My.Settings.CartellaDoppi

        If Not IO.Directory.Exists(cartellaArchivio) Then IO.Directory.CreateDirectory(cartellaArchivio)
        If Not IO.Directory.Exists(cartellaDuplicati) Then IO.Directory.CreateDirectory(cartellaDuplicati)

        If Not IO.Directory.Exists(cartellaP7M) Then
            Return "La cartella XML non esiste!"
        End If

        Dim files As String() = IO.Directory.GetFiles(cartellaP7M, "*.xml").ToArray()
        Dim totaleFile As Integer = files.Length

        If totaleFile = 0 Then
            Return "Nessun file XML trovato nella cartella di download."
        End If

        Using conn As New SqlConnection(connectionString)
            conn.Open()
            For i As Integer = 0 To totaleFile - 1
                Dim singoloFile As String = files(i)
                Dim soloNomeFile As String = IO.Path.GetFileName(singoloFile)

                If Not IO.File.Exists(singoloFile) Then Continue For

                progress?.Report(String.Format("Elaborazione {0} di {1}: {2}", i + 1, totaleFile, soloNomeFile))

                ' Il controllo duplicati si basa sul contenuto della fattura (cedente + numero + data),
                ' non sul nome del file: il nome assegnato dall'intermediario SDI può ripetersi anche
                ' per fatture diverse, causando lo scarto silenzioso di fatture mai importate.
                Dim isDuplicato As Boolean = False
                If Not soloNomeFile.ToLower().Contains("metadato") Then
                    Dim cedIdCod As String = Nothing, numDoc As String = Nothing, dataDoc As String = Nothing
                    Try
                        EstraiChiaveFattura(singoloFile, cedIdCod, numDoc, dataDoc)
                        isDuplicato = FatturaGiaCaricata(cedIdCod, numDoc, dataDoc, conn)
                    Catch ex As Exception
                        ' File illeggibile/non valido: lo lasciamo gestire da ImportaFatturaXml,
                        ' che loggherà l'errore senza bloccare l'elaborazione degli altri file.
                    End Try
                End If

                If isDuplicato Then
                    FileSpostaConSovrascrittura(singoloFile, IO.Path.Combine(cartellaDuplicati, soloNomeFile))
                    Dim pathMetaDup As String = TrovaFileMetadati(cartellaP7M, soloNomeFile)
                    If Not String.IsNullOrEmpty(pathMetaDup) Then
                        FileSpostaConSovrascrittura(pathMetaDup, IO.Path.Combine(cartellaDuplicati, IO.Path.GetFileName(pathMetaDup)))
                    End If
                    Continue For
                End If

                If ImportaFatturaXml(singoloFile, conn) Then
                    FileSpostaConSovrascrittura(singoloFile, IO.Path.Combine(cartellaArchivio, soloNomeFile))
                    Dim pathMeta As String = TrovaFileMetadati(cartellaP7M, soloNomeFile)
                    If Not String.IsNullOrEmpty(pathMeta) Then
                        InserisciMetadato(pathMeta, conn)
                        FileSpostaConSovrascrittura(pathMeta, IO.Path.Combine(cartellaArchivio, IO.Path.GetFileName(pathMeta)))
                    End If
                End If
            Next
        End Using

        Return "Importazione completata!"
    End Function

    Public Function ImportaFatturaXml(pathXML As String, ByVal conn As SqlConnection) As Boolean
        ' Se il file è un metadato, lo gestiamo separatamente senza toccare la fattura elettronica
        If IO.Path.GetFileName(pathXML).ToLower().Contains("metadato") Then
            Return InserisciMetadato(pathXML, conn)
        End If

        Dim doc As XDocument
        Using reader As New IO.StreamReader(pathXML, True)
            doc = XDocument.Load(reader)
        End Using

        Using trans As SqlTransaction = conn.BeginTransaction()
            Try
                ' --- 1. IDENTIFICAZIONE RADICE E CORPO ---
                Dim root = doc.Root
                ' Cerchiamo il corpo indipendentemente dal prefisso (p:, ns2:, ecc.)
                Dim body = root.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "FatturaElettronicaBody")

                ' --- 2. INSERIMENTO TESTATA (Esempio semplificato) ---
                ' Recupera ID_Fattura dopo l'insert della testata...
                Dim idFattura As Integer = InserisciTestataEGetId(doc, pathXML, conn, trans)

                Dim header = root.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "FatturaElettronicaHeader")
                Dim cedente = header?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "CedentePrestatore")
                Dim cedIdCod = cedente?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "IdCodice")?.Value
                Dim fornitoreConPrezzoInDescrizione As Boolean = EFornitoreConPrezzoInDescrizione(cedIdCod, conn, trans)

                ' --- 3. INSERIMENTO RIGHE, UNITÀ MISURA E SCONTI ---
                Dim nodiLinee = body.Descendants().Where(Function(x) x.Name.LocalName = "DettaglioLinee")

                For Each riga In nodiLinee
                    Dim nLinea = riga.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "NumeroLinea")?.Value
                    Dim descr = riga.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "Descrizione")?.Value
                    Dim um = riga.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "UnitaMisura")?.Value
                    Dim codArt = EstraiCodiceArticolo(riga)

                    ' Helper per decimali XML (gestisce punto e virgola)
                    Dim parseXmlDec = Function(nome As String) As Decimal
                                          Dim v = riga.Elements().FirstOrDefault(Function(x) x.Name.LocalName = nome)?.Value
                                          Return If(String.IsNullOrEmpty(v), 0, Decimal.Parse(v.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture))
                                      End Function

                    Dim quantitaRiga = parseXmlDec("Quantita")
                    Dim prezzoUnitarioRiga = parseXmlDec("PrezzoUnitario")
                    Dim umRiga = um

                    ' Per i fornitori in FornitoriPrezzoInDescrizione, Quantita/PrezzoUnitario del nodo
                    ' XML sono per collo (non per kg): se la Descrizione riporta "<kg> prezzo al kg
                    ' <prezzo>" si usano quei valori reali. Se il pattern non matcha (es. righe
                    ' puramente descrittive, senza prodotto) si mantengono i valori standard.
                    If fornitoreConPrezzoInDescrizione Then
                        Dim quantitaKg As Decimal, prezzoAlKg As Decimal
                        If ProvaEstraiQuantitaEPrezzoDaDescrizione(descr, quantitaKg, prezzoAlKg) Then
                            quantitaRiga = quantitaKg
                            prezzoUnitarioRiga = prezzoAlKg
                            umRiga = "KG"
                        End If
                    End If

                    ' INSERT RIGA
                    Dim sqlR As String = "INSERT INTO Fatture_Righe (ID_Fattura, NumeroLinea, CodiceArticolo, Descrizione, Quantita, PrezzoUnitario, PrezzoTotale, AliquotaIVA, UnitaMisura) " &
                                             "VALUES (@id, @lin, @art, @des, @qta, @pre, @tot, @iva, @um)"

                    Using cmdR As New SqlCommand(sqlR, conn, trans)
                        cmdR.Parameters.AddWithValue("@id", idFattura)
                        cmdR.Parameters.AddWithValue("@lin", If(nLinea, "0"))
                        cmdR.Parameters.AddWithValue("@art", If(codArt, DBNull.Value))
                        cmdR.Parameters.AddWithValue("@des", If(descr, ""))
                        cmdR.Parameters.AddWithValue("@qta", quantitaRiga)
                        cmdR.Parameters.AddWithValue("@pre", prezzoUnitarioRiga)
                        cmdR.Parameters.AddWithValue("@tot", parseXmlDec("PrezzoTotale"))
                        cmdR.Parameters.AddWithValue("@iva", parseXmlDec("AliquotaIVA"))
                        cmdR.Parameters.AddWithValue("@um", If(umRiga, DBNull.Value))
                        cmdR.ExecuteNonQuery()
                    End Using

                    ' --- SOTTO-CICLO SCONTI/MAGGIORAZIONI ---
                    Dim nodiSM = riga.Descendants().Where(Function(x) x.Name.LocalName = "ScontoMaggiorazione")
                    For Each sm In nodiSM
                        Dim sqlS As String = "INSERT INTO Fatture_Sconti (ID_Fattura, NumeroLinea, Tipo, Percentuale, Importo) VALUES (@id, @lin, @tipo, @perc, @imp)"
                        Using cmdS As New SqlCommand(sqlS, conn, trans)
                            cmdS.Parameters.AddWithValue("@id", idFattura)
                            cmdS.Parameters.AddWithValue("@lin", If(nLinea, "0"))
                            cmdS.Parameters.AddWithValue("@tipo", If(sm.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "Tipo")?.Value, "SC"))

                            ' Parsing dedicato per i valori dello sconto
                            Dim pSc = sm.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "Percentuale")?.Value
                            Dim iSc = sm.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "Importo")?.Value
                            cmdS.Parameters.AddWithValue("@perc", If(String.IsNullOrEmpty(pSc), 0, Decimal.Parse(pSc.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture)))
                            cmdS.Parameters.AddWithValue("@imp", If(String.IsNullOrEmpty(iSc), 0, Decimal.Parse(iSc.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture)))

                            cmdS.ExecuteNonQuery()
                        End Using
                    Next

                    ' --- SOTTO-CICLO ALTRI DATI GESTIONALI ---

                    Dim nodiAltriDati = riga.Descendants().Where(Function(x) x.Name.LocalName = "AltriDatiGestionali")

                    For Each ad In nodiAltriDati
                        Dim sqlAD As String = "INSERT INTO Fatture_AltriDati (ID_Fattura, NumeroLinea, TipoDato, RiferimentoTesto, RiferimentoNumero, RiferimentoData) " &
                              "VALUES (@id, @lin, @tipo, @testo, @num, @data)"

                        Using cmdAD As New SqlCommand(sqlAD, conn, trans)
                            cmdAD.Parameters.AddWithValue("@id", idFattura)
                            cmdAD.Parameters.AddWithValue("@lin", If(nLinea, "0"))

                            Dim tipoD = ad.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "TipoDato")?.Value
                            Dim testoD = ad.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "RiferimentoTesto")?.Value
                            Dim numD = ad.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "RiferimentoNumero")?.Value
                            Dim dataD = ad.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "RiferimentoData")?.Value

                            cmdAD.Parameters.AddWithValue("@tipo", If(tipoD, DBNull.Value))
                            cmdAD.Parameters.AddWithValue("@testo", If(testoD, DBNull.Value))

                            ' Gestione decimale per RiferimentoNumero
                            If String.IsNullOrEmpty(numD) Then
                                cmdAD.Parameters.AddWithValue("@num", DBNull.Value)
                            Else
                                cmdAD.Parameters.AddWithValue("@num", Decimal.Parse(numD.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture))
                            End If

                            ' Gestione data per RiferimentoData
                            cmdAD.Parameters.AddWithValue("@data", If(String.IsNullOrEmpty(dataD), DBNull.Value, dataD))

                            cmdAD.ExecuteNonQuery()
                        End Using
                    Next
                Next

                ' --- 4. INSERIMENTO DDT (RIFERIMENTI MULTIPLI) ---
                Dim nodiDDT = body.Descendants().Where(Function(x) x.Name.LocalName = "DatiDDT").ToList()

                ' Raccogliamo per ogni blocco DDT: numero, data e i RiferimentoNumeroLinea (già interi, se presenti)
                Dim blocchiDDT = nodiDDT.Select(Function(ddt)
                                                     Dim rifInteri = ddt.Elements().
                                                         Where(Function(x) x.Name.LocalName = "RiferimentoNumeroLinea").
                                                         Select(Function(x) CInt(x.Value)).ToList()
                                                     Return New With {
                                                         .NumeroDDT = ddt.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "NumeroDDT")?.Value,
                                                         .DataDDT = ddt.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "DataDDT")?.Value,
                                                         .Riferimenti = rifInteri
                                                     }
                                                 End Function).ToList()

                ' Alcuni fornitori indicano nel DDT solo la PRIMA riga del blocco di righe consegnate
                ' (invece di elencarle tutte): quando succede per TUTTI i DDT della fattura,
                ' deduciamo il range di ciascun DDT fino al riferimento del DDT successivo.
                Dim usaRangeFill = blocchiDDT.Count > 0 AndAlso blocchiDDT.All(Function(b) b.Riferimenti.Count = 1)

                If usaRangeFill Then
                    Dim maxLinea = nodiLinee.Select(Function(r) CInt(If(r.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "NumeroLinea")?.Value, "0"))).
                                             DefaultIfEmpty(0).Max()
                    Dim ordinati = blocchiDDT.OrderBy(Function(b) b.Riferimenti(0)).ToList()

                    For i = 0 To ordinati.Count - 1
                        Dim rangeInizio = ordinati(i).Riferimenti(0)
                        Dim rangeFine = If(i < ordinati.Count - 1, ordinati(i + 1).Riferimenti(0) - 1, maxLinea)
                        For numLinea = rangeInizio To rangeFine
                            InserisciDDT(idFattura, ordinati(i).NumeroDDT, ordinati(i).DataDDT, numLinea.ToString(), conn, trans)
                        Next
                    Next
                Else
                    ' Per i fornitori configurati in FornitoriRecuperoDdtEdi (nessun RiferimentoNumeroLinea
                    ' mai presente in XML), prima di arrenderci per i DDT privi di riferimenti tentiamo di
                    ' ricostruirli incrociando CodiceArticolo+Quantita con i dati ricevuti via EDI.
                    If blocchiDDT.Any(Function(b) b.Riferimenti.Count = 0) AndAlso EFornitoreRecuperoDdtEdi(cedIdCod, conn, trans) Then
                        Dim codiceGestionale = GetCodiceGestionaleFornitore(cedIdCod, conn, trans)
                        If Not String.IsNullOrEmpty(codiceGestionale) Then
                            Dim lineeGiaAssegnate As New HashSet(Of Integer)(blocchiDDT.SelectMany(Function(b) b.Riferimenti))
                            Dim lineeDisponibili = nodiLinee.Select(Function(r) New With {
                                    .NumeroLinea = CInt(If(r.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "NumeroLinea")?.Value, "0")),
                                    .CodiceArticolo = EstraiCodiceArticolo(r),
                                    .Quantita = If(String.IsNullOrEmpty(r.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "Quantita")?.Value), 0D,
                                                   Decimal.Parse(r.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "Quantita").Value.Replace(",", "."), Globalization.CultureInfo.InvariantCulture))
                                }).
                                Where(Function(r) Not lineeGiaAssegnate.Contains(r.NumeroLinea) AndAlso Not String.IsNullOrEmpty(r.CodiceArticolo)).
                                ToList()

                            For Each b In blocchiDDT
                                If b.Riferimenti.Count > 0 Then Continue For

                                Dim righeEdi = GetRigheEdiPerDdt(codiceGestionale, b.NumeroDDT, b.DataDDT, conn, trans)
                                For Each rigaEdi In righeEdi
                                    Dim match = lineeDisponibili.FirstOrDefault(Function(l) String.Equals(l.CodiceArticolo, rigaEdi.CodiceArticolo, StringComparison.OrdinalIgnoreCase) AndAlso Math.Abs(l.Quantita - rigaEdi.Quantita) < 0.01D)
                                    If match IsNot Nothing Then
                                        b.Riferimenti.Add(match.NumeroLinea)
                                        lineeDisponibili.Remove(match)
                                    End If
                                Next
                            Next
                        End If
                    End If

                    For Each b In blocchiDDT
                        ' RiferimentoNumeroLinea è opzionale (0..N): se assente (e non risolto via EDI),
                        ' il DDT va comunque registrato
                        If b.Riferimenti.Count = 0 Then
                            InserisciDDT(idFattura, b.NumeroDDT, b.DataDDT, DBNull.Value, conn, trans)
                        Else
                            For Each rif In b.Riferimenti
                                InserisciDDT(idFattura, b.NumeroDDT, b.DataDDT, rif.ToString(), conn, trans)
                            Next
                        End If
                    Next
                End If

                ' Se tutto è andato bene, salviamo fisicamente nel DB
                trans.Commit()
                Return True
            Catch ex As Exception
                If trans IsNot Nothing AndAlso trans.Connection IsNot Nothing Then
                    trans.Rollback()
                End If

                ' Invece di fare Throw (che blocca il programma),
                ' logghiamo l'errore e ritorniamo False per permettere al ciclo di continuare
                Debug.WriteLine("Errore file " & pathXML & ": " & ex.Message)
                Return False
            End Try
        End Using
    End Function

    Private Sub InserisciDDT(id As Integer, num As String, data As String, rif As Object, conn As SqlConnection, trans As SqlTransaction)
        Dim sqlD As String = "INSERT INTO Fatture_DDT (ID_Fattura, NumeroDDT, DataDDT, RiferimentoNumeroLinea) VALUES (@id, @num, @dat, @rif)"
        Using cmdD As New SqlCommand(sqlD, conn, trans)
            cmdD.Parameters.AddWithValue("@id", id)
            cmdD.Parameters.AddWithValue("@num", If(num, ""))
            cmdD.Parameters.AddWithValue("@dat", If(data, DBNull.Value))
            cmdD.Parameters.AddWithValue("@rif", If(rif, DBNull.Value))
            cmdD.ExecuteNonQuery()
        End Using
    End Sub

    Private Function InserisciTestataEGetId(doc As XDocument, percorsoFile As String, conn As SqlConnection, trans As SqlTransaction) As Integer
        Dim root = doc.Root

        ' --- A. DATI DALL'HEADER (Fornitore e Cliente) ---
        Dim header = root.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "FatturaElettronicaHeader")

        ' Cedente (Fornitore)
        Dim cedente = header?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "CedentePrestatore")
        Dim cedDenom = cedente?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "Denominazione")?.Value
        Dim cedIdCod = cedente?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "IdCodice")?.Value

        ' Se la denominazione è vuota (caso ditte individuali), cerchiamo Nome + Cognome
        If String.IsNullOrEmpty(cedDenom) Then
            Dim nome = cedente?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "Nome")?.Value
            Dim cognome = cedente?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "Cognome")?.Value
            cedDenom = (nome & " " & cognome).Trim()
        End If

        ' Cessionario (Cliente - Tu)
        Dim cessionario = header?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "CessionarioCommittente")
        Dim cessDenom = cessionario?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "Denominazione")?.Value

        ' --- B. DATI DAL BODY (Dati Generali e Importi) ---
        Dim body = root.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "FatturaElettronicaBody")
        Dim datiGen = body?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "DatiGeneraliDocumento")

        Dim tipoDoc = datiGen?.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "TipoDocumento")?.Value
        Dim dataDoc = datiGen?.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "Data")?.Value
        Dim numDoc = datiGen?.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "Numero")?.Value

        ' Importo Totale (con gestione punto/virgola)
        Dim impString = datiGen?.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "ImportoTotaleDocumento")?.Value
        Dim importoTot = If(String.IsNullOrEmpty(impString), 0, Decimal.Parse(impString.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture))

        ' Causale (Prendiamo tutte le causali se sono multiple e le uniamo)
        Dim causali = datiGen?.Elements().Where(Function(x) x.Name.LocalName = "Causale").Select(Function(x) x.Value)
        Dim causaleUnita = If(causali IsNot Nothing, String.Join(" | ", causali), "")

        ' Nome File (prendiamo solo il nome, non tutto il percorso)
        Dim soloNomeFile = IO.Path.GetFileName(percorsoFile)

        ' --- C. ESECUZIONE QUERY ---
        Dim sqlT As String = "INSERT INTO Fatture_Testate (TipoDocumento, DataFattura, NumeroFattura, NomeFile, " &
                             "ImportoTotaleDocumento, Causale, CedenteDenominazione, CedenteIdCodice, CessionarioDenominazione) " &
                             "VALUES (@tipo, @data, @num, @file, @imp, @caus, @cedDen, @cedCod, @cessDen); " &
                             "SELECT SCOPE_IDENTITY();"

        Using cmdT As New SqlCommand(sqlT, conn, trans)
            cmdT.Parameters.AddWithValue("@tipo", If(tipoDoc, ""))
            cmdT.Parameters.AddWithValue("@data", If(dataDoc, DBNull.Value))
            cmdT.Parameters.AddWithValue("@num", If(numDoc, ""))
            cmdT.Parameters.AddWithValue("@file", soloNomeFile)
            cmdT.Parameters.AddWithValue("@imp", importoTot)
            cmdT.Parameters.AddWithValue("@caus", causaleUnita)
            cmdT.Parameters.AddWithValue("@cedDen", If(cedDenom, ""))
            cmdT.Parameters.AddWithValue("@cedCod", If(cedIdCod, ""))
            cmdT.Parameters.AddWithValue("@cessDen", If(cessDenom, ""))

            Return Convert.ToInt32(cmdT.ExecuteScalar())
        End Using
    End Function

    ''' <summary>
    ''' Estrae dal file XML la chiave identificativa della fattura (P.IVA/codice del cedente,
    ''' numero e data documento), usata per il controllo duplicati basato sul contenuto.
    ''' </summary>
    ''' <summary>
    ''' Restituisce il CodiceValore dell'articolo di una riga. Se la riga ha più nodi CodiceArticolo
    ''' si ignora quello di tipo EAN (codice a barre) e si usa il primo degli altri; se il nodo è
    ''' unico, se ne usa il valore indipendentemente dal tipo.
    ''' </summary>
    Private Function EstraiCodiceArticolo(riga As XElement) As String
        Dim nodiArticolo As List(Of XElement) = riga.Elements().Where(Function(x) x.Name.LocalName = "CodiceArticolo").ToList()

        If nodiArticolo.Count = 1 Then
            Return ValoreFiglio(nodiArticolo(0), "CodiceValore")
        End If

        For Each nodo In nodiArticolo
            If ValoreFiglio(nodo, "CodiceTipo") <> "EAN" Then
                Return ValoreFiglio(nodo, "CodiceValore")
            End If
        Next

        Return Nothing
    End Function

    Private Function ValoreFiglio(nodo As XElement, nomeFiglio As String) As String
        Return nodo.Elements().FirstOrDefault(Function(x) x.Name.LocalName = nomeFiglio)?.Value?.Trim()
    End Function

    Private Sub EstraiChiaveFattura(pathXML As String, ByRef cedIdCod As String, ByRef numDoc As String, ByRef dataDoc As String)
        Dim doc As XDocument
        Using reader As New IO.StreamReader(pathXML, True)
            doc = XDocument.Load(reader)
        End Using

        Dim root = doc.Root
        Dim header = root.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "FatturaElettronicaHeader")
        Dim cedente = header?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "CedentePrestatore")
        cedIdCod = cedente?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "IdCodice")?.Value

        Dim body = root.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "FatturaElettronicaBody")
        Dim datiGen = body?.Descendants().FirstOrDefault(Function(x) x.Name.LocalName = "DatiGeneraliDocumento")
        numDoc = datiGen?.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "Numero")?.Value
        dataDoc = datiGen?.Elements().FirstOrDefault(Function(x) x.Name.LocalName = "Data")?.Value
    End Sub

    Public Function FatturaGiaCaricata(cedIdCod As String, numDoc As String, dataDoc As String, conn As SqlConnection) As Boolean
        If String.IsNullOrEmpty(cedIdCod) OrElse String.IsNullOrEmpty(numDoc) OrElse String.IsNullOrEmpty(dataDoc) Then
            Return False
        End If

        Dim sql As String = "SELECT COUNT(*) FROM Fatture_Testate WHERE CedenteIdCodice = @ced AND NumeroFattura = @num AND DataFattura = @data"
        Using cmd As New SqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@ced", cedIdCod)
            cmd.Parameters.AddWithValue("@num", numDoc)
            cmd.Parameters.AddWithValue("@data", dataDoc)
            Return Convert.ToInt32(cmd.ExecuteScalar()) > 0
        End Using
    End Function

    Private Sub FileSpostaConSovrascrittura(sorgente As String, destinazione As String)
        If IO.File.Exists(destinazione) Then
            IO.File.Delete(destinazione)
        End If
        IO.File.Move(sorgente, destinazione)
    End Sub

    Private Function TrovaFileMetadati(cartella As String, nomeInvoice As String) As String
        Dim baseInvoice As String = IO.Path.GetFileNameWithoutExtension(nomeInvoice).ToLower()

        ' Cerca prima con il nome fattura come prefisso (formato classico)
        Dim risultatoClassico = IO.Directory.GetFiles(cartella, nomeInvoice & "*.*").
            FirstOrDefault(Function(f) IO.Path.GetFileName(f).ToLower().Contains("metadato"))
        If Not String.IsNullOrEmpty(risultatoClassico) Then Return risultatoClassico

        ' Fallback: cerca file metadato che contiene il nome base della fattura (formato con prefisso SDI)
        Return IO.Directory.GetFiles(cartella, "*.xml").
            FirstOrDefault(Function(f)
                               Dim fn = IO.Path.GetFileName(f).ToLower()
                               Return fn.Contains("metadato") AndAlso fn.Contains(baseInvoice)
                           End Function)
    End Function

    Private Function InserisciMetadato(pathMetadato As String, conn As SqlConnection, Optional ByRef erroreMsg As String = "", Optional ByRef eraDuplicato As Boolean = False, Optional ByRef nomefileUsato As String = "") As Boolean
        Using trans As SqlTransaction = conn.BeginTransaction()
            Try
                Dim docMeta As XDocument
                Using reader As New IO.StreamReader(pathMetadato, True)
                    docMeta = XDocument.Load(reader)
                End Using

                Dim ns As XNamespace = "urn:xml.fatturazione.sogei.it"
                Dim getMetadato = Function(nome As String) As String
                                      Return docMeta.Descendants(ns + "metadato").
                                          FirstOrDefault(Function(m) m.Element(ns + "nome")?.Value = nome)?.
                                          Element(ns + "valore")?.Value
                                  End Function

                Dim dataAccoglienza As String = getMetadato("dataaccoglienza")

                If String.IsNullOrEmpty(dataAccoglienza) Then
                    trans.Rollback()
                    Return False
                End If

                ' Ricava il nomefile dal percorso del file metadato su disco,
                ' così include sempre il prefisso numerico SDI (es. 17244482732_)
                ' Esempio: 17244482732_sm03473_ge62i.p7m_metadato.xml
                '   → senza estensione: 17244482732_sm03473_ge62i.p7m_metadato
                '   → rimuove _metadato: 17244482732_sm03473_ge62i.p7m
                '   → rimuove .p7m:     17244482732_sm03473_ge62i
                '   → aggiunge .xml:    17244482732_sm03473_ge62i.xml
                Dim nomefile As String = IO.Path.GetFileNameWithoutExtension(pathMetadato)
                If nomefile.ToLower().EndsWith("_metadato") Then
                    nomefile = nomefile.Substring(0, nomefile.Length - 9)
                End If
                If nomefile.ToLower().EndsWith(".p7m") Then
                    nomefile = nomefile.Substring(0, nomefile.Length - 4)
                End If
                nomefile &= ".xml"

                nomefileUsato = nomefile

                ' Evita duplicati se il metadato viene incontrato più volte prima dello spostamento
                Dim checkSql As String = "SELECT COUNT(*) FROM Fatture_Ricezione WHERE NomeFile = @nome"
                Using cmdCheck As New SqlCommand(checkSql, conn, trans)
                    cmdCheck.Parameters.AddWithValue("@nome", nomefile)
                    If Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0 Then
                        eraDuplicato = True
                        trans.Rollback()
                        Return True
                    End If
                End Using

                Dim dataRicezione As DateTime = DateTime.Parse(dataAccoglienza, Nothing, Globalization.DateTimeStyles.RoundtripKind)

                Dim sql As String = "INSERT INTO Fatture_Ricezione (NomeFile, DataRicezione) VALUES (@nome, @data)"
                Using cmd As New SqlCommand(sql, conn, trans)
                    cmd.Parameters.AddWithValue("@nome", nomefile)
                    cmd.Parameters.AddWithValue("@data", dataRicezione.Date)
                    cmd.ExecuteNonQuery()
                End Using

                trans.Commit()
                Return True
            Catch ex As Exception
                If trans IsNot Nothing AndAlso trans.Connection IsNot Nothing Then
                    trans.Rollback()
                End If
                erroreMsg = ex.Message
                Debug.WriteLine("Errore metadato " & pathMetadato & ": " & ex.Message)
                Return False
            End Try
        End Using
    End Function
End Module
