Imports System.Xml.Linq
Imports System.Net

''' <summary>
''' Genera, a partire dal file XML di una fattura elettronica, una rappresentazione HTML
''' compatta e tabellare (alternativa al "formato ministeriale" ufficiale), ispirata al layout
''' usato dai portali di ricezione fatture: intestazione cedente/cessionario, dati documento,
''' righe di dettaglio, riepilogo IVA, dati pagamento ed elenco allegati.
''' </summary>
Module ModuloVisualizzaFatturaCompatta

    Private ReadOnly DescrizioniTipoDocumento As New Dictionary(Of String, String) From {
        {"TD01", "Fattura"},
        {"TD02", "Acconto/anticipo su fattura"},
        {"TD03", "Acconto/anticipo su parcella"},
        {"TD04", "Nota di credito"},
        {"TD05", "Nota di debito"},
        {"TD06", "Parcella"},
        {"TD16", "Integrazione fattura reverse charge interno"},
        {"TD17", "Integrazione/autofattura per acquisto servizi dall'estero"},
        {"TD18", "Integrazione per acquisto di beni intracomunitari"},
        {"TD19", "Integrazione/autofattura per acquisto di beni ex art.17 c.2 DPR 633/72"},
        {"TD20", "Autofattura per regolarizzazione e integrazione delle fatture (ex art.6 c.8 e 9-bis d.lgs. 471/97 o art.46 c.5 D.L. 331/93)"},
        {"TD21", "Autofattura per splafonamento"},
        {"TD22", "Estrazione beni da Deposito IVA"},
        {"TD23", "Estrazione beni da Deposito IVA con versamento dell'IVA"},
        {"TD24", "Fattura differita di cui all'art.21, comma 4, terzo periodo lett. a) DPR 633/72"},
        {"TD25", "Fattura differita di cui all'art.21, comma 4, terzo periodo lett. b) DPR 633/72"},
        {"TD26", "Cessione di beni ammortizzabili e per passaggi interni (ex art.36 DPR 633/72)"},
        {"TD27", "Fattura per autoconsumo o per cessioni gratuite senza rivalsa"},
        {"TD28", "Acquisti da San Marino con IVA (fattura cartacea)"},
        {"TD29", "Comunicazione per omessa o irregolare fatturazione"}
    }

    Private ReadOnly DescrizioniRegimeFiscale As New Dictionary(Of String, String) From {
        {"RF01", "ordinario"},
        {"RF02", "dei contribuenti minimi (art.1, c.96-117, L. 244/07)"},
        {"RF04", "agricoltura e attività connesse e pesca (artt.34 e 34-bis, DPR 633/72)"},
        {"RF05", "vendita sali e tabacchi (art.74, c.1, DPR 633/72)"},
        {"RF06", "commercio fiammiferi (art.74, c.1, DPR 633/72)"},
        {"RF07", "editoria (art.74, c.1, DPR 633/72)"},
        {"RF08", "gestione servizi telefonia pubblica (art.74, c.1, DPR 633/72)"},
        {"RF09", "rivendita documenti di trasporto pubblico e di sosta (art.74, c.1, DPR 633/72)"},
        {"RF10", "intrattenimenti, giochi e altre attività di cui alla tariffa allegata al DPR 640/72 (art.74, c.6, DPR 633/72)"},
        {"RF11", "agenzie viaggi e turismo (art.74-ter, DPR 633/72)"},
        {"RF12", "agriturismo (art.5, c.2, L. 413/91)"},
        {"RF13", "vendite a domicilio (art.25-bis, c.6, DPR 600/73)"},
        {"RF14", "rivendita beni usati, oggetti d'arte, d'antiquariato o da collezione (art.36, DL 41/95)"},
        {"RF15", "agenzie di vendite all'asta di oggetti d'arte, antiquariato o da collezione (art.40-bis, DL 41/95)"},
        {"RF16", "IVA per cassa P.A. (art.6, c.5, DPR 633/72)"},
        {"RF17", "IVA per cassa (art.32-bis, DL 83/2012)"},
        {"RF18", "altro"},
        {"RF19", "forfettario"}
    }

    Private ReadOnly DescrizioniModalitaPagamento As New Dictionary(Of String, String) From {
        {"MP01", "Contanti"},
        {"MP02", "Assegno"},
        {"MP03", "Assegno circolare"},
        {"MP04", "Contanti presso Tesoreria"},
        {"MP05", "Bonifico"},
        {"MP06", "Vaglia cambiario"},
        {"MP07", "Bollettino bancario"},
        {"MP08", "Carta di pagamento"},
        {"MP09", "RID"},
        {"MP10", "RID utenze"},
        {"MP11", "RID veloce"},
        {"MP12", "RIBA"},
        {"MP13", "MAV"},
        {"MP14", "Quietanza erario"},
        {"MP15", "Giroconto su conti di contabilità speciale"},
        {"MP16", "Domiciliazione bancaria"},
        {"MP17", "Domiciliazione postale"},
        {"MP18", "Bollettino di c/c postale"},
        {"MP19", "SEPA Direct Debit"},
        {"MP20", "SEPA Direct Debit CORE"},
        {"MP21", "SEPA Direct Debit B2B"},
        {"MP22", "Trattenuta su somme già riscosse"},
        {"MP23", "PagoPA"}
    }

    Private ReadOnly DescrizioniEsigibilitaIVA As New Dictionary(Of String, String) From {
        {"I", "Esigibilità immediata"},
        {"D", "Esigibilità differita"},
        {"S", "Scissione dei pagamenti"}
    }

    ''' <summary>
    ''' Cerca il primo discendente/figlio (a seconda di soloFigli) il cui nome locale corrisponde
    ''' a nomeLocale e ne restituisce il valore, o Nothing se assente.
    ''' </summary>
    Private Function Val(padre As XElement, nomeLocale As String, Optional soloFigli As Boolean = False) As String
        If padre Is Nothing Then Return Nothing
        Dim sorgente = If(soloFigli, padre.Elements(), padre.Descendants())
        Return sorgente.FirstOrDefault(Function(x) x.Name.LocalName = nomeLocale)?.Value
    End Function

    Private Function Nodo(padre As XElement, nomeLocale As String, Optional soloFigli As Boolean = False) As XElement
        If padre Is Nothing Then Return Nothing
        Dim sorgente = If(soloFigli, padre.Elements(), padre.Descendants())
        Return sorgente.FirstOrDefault(Function(x) x.Name.LocalName = nomeLocale)
    End Function

    Private Function Nodi(padre As XElement, nomeLocale As String) As IEnumerable(Of XElement)
        If padre Is Nothing Then Return Enumerable.Empty(Of XElement)()
        Return padre.Descendants().Where(Function(x) x.Name.LocalName = nomeLocale)
    End Function

    Private Function H(testo As String) As String
        Return WebUtility.HtmlEncode(If(testo, ""))
    End Function

    Private Function HMultilinea(testo As String) As String
        Return H(testo).Replace(vbCrLf, "<br>").Replace(vbLf, "<br>")
    End Function

    Private Function FormattaData(valore As String) As String
        If String.IsNullOrEmpty(valore) Then Return ""
        Dim d As Date
        If Date.TryParse(valore, Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
            Return d.ToString("dd/MM/yyyy")
        End If
        Return valore
    End Function

    Private Function FormattaImporto(valore As String) As String
        If String.IsNullOrEmpty(valore) Then Return ""
        Dim n As Decimal
        If Decimal.TryParse(valore.Replace(",", "."), Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, n) Then
            Return n.ToString("N2", New Globalization.CultureInfo("it-IT"))
        End If
        Return valore
    End Function

    Private Function EstraiDenominazione(datiAnagrafici As XElement) As String
        Dim anagrafica = Nodo(datiAnagrafici, "Anagrafica")
        Dim denom = Val(anagrafica, "Denominazione")
        If Not String.IsNullOrEmpty(denom) Then Return denom
        Dim nome = Val(anagrafica, "Nome")
        Dim cognome = Val(anagrafica, "Cognome")
        Return (If(nome, "") & " " & If(cognome, "")).Trim()
    End Function

    Private Function EstraiPartitaIva(datiAnagrafici As XElement) As String
        Dim idFiscale = Nodo(datiAnagrafici, "IdFiscaleIVA")
        Dim idPaese = Val(idFiscale, "IdPaese")
        Dim idCodice = Val(idFiscale, "IdCodice")
        If String.IsNullOrEmpty(idCodice) Then Return ""
        Return If(idPaese, "") & idCodice
    End Function

    Private Function EstraiIndirizzo(sede As XElement) As String
        If sede Is Nothing Then Return ""
        Dim indirizzo = Val(sede, "Indirizzo", soloFigli:=True)
        Dim civico = Val(sede, "NumeroCivico", soloFigli:=True)
        If String.IsNullOrEmpty(civico) Then Return If(indirizzo, "")
        Return $"{indirizzo} N. {civico}"
    End Function

    ''' <summary>
    ''' Applica il layout compatto tabellare al file XML indicato e restituisce l'HTML risultante.
    ''' </summary>
    Public Function RenderizzaFatturaHtmlCompatto(pathXml As String, Optional dataRicezione As Date? = Nothing) As String
        Dim doc As XDocument
        Using reader As New IO.StreamReader(pathXml, True)
            doc = XDocument.Load(reader)
        End Using

        Dim root = doc.Root
        Dim header = Nodo(root, "FatturaElettronicaHeader")
        Dim body = Nodo(root, "FatturaElettronicaBody")

        Dim datiTrasmissione = Nodo(header, "DatiTrasmissione")
        Dim codiceDestinatario = Val(datiTrasmissione, "CodiceDestinatario")

        Dim cedente = Nodo(header, "CedentePrestatore")
        Dim cedenteAnagrafici = Nodo(cedente, "DatiAnagrafici")
        Dim cedenteSede = Nodo(cedente, "Sede")
        Dim cedenteContatti = Nodo(cedente, "Contatti")
        Dim regimeFiscaleCodice = Val(cedenteAnagrafici, "RegimeFiscale")
        Dim regimeFiscaleDescr As String = Nothing
        If Not String.IsNullOrEmpty(regimeFiscaleCodice) Then
            DescrizioniRegimeFiscale.TryGetValue(regimeFiscaleCodice, regimeFiscaleDescr)
        End If

        Dim cessionario = Nodo(header, "CessionarioCommittente")
        Dim cessionarioAnagrafici = Nodo(cessionario, "DatiAnagrafici")
        Dim cessionarioSede = Nodo(cessionario, "Sede")

        Dim terzo = Nodo(datiTrasmissione, "TerzoIntermediarioOSoggettoEmittente")
        Dim terzoAnagrafici = Nodo(terzo, "DatiAnagrafici")

        Dim datiGenerali = Nodo(body, "DatiGenerali")
        Dim datiGeneraliDocumento = Nodo(datiGenerali, "DatiGeneraliDocumento")
        Dim tipoDocCodice = Val(datiGeneraliDocumento, "TipoDocumento", soloFigli:=True)
        Dim tipoDocDescr As String = Nothing
        If Not String.IsNullOrEmpty(tipoDocCodice) Then
            DescrizioniTipoDocumento.TryGetValue(tipoDocCodice, tipoDocDescr)
        End If
        If String.IsNullOrEmpty(tipoDocDescr) Then tipoDocDescr = tipoDocCodice
        Dim art73 = Val(datiGeneraliDocumento, "Art73", soloFigli:=True)
        Dim numeroDoc = Val(datiGeneraliDocumento, "Numero", soloFigli:=True)
        Dim dataDoc = Val(datiGeneraliDocumento, "Data", soloFigli:=True)
        Dim valuta = Val(datiGeneraliDocumento, "Divisa", soloFigli:=True)
        If String.IsNullOrEmpty(valuta) Then valuta = "EUR"
        Dim importoTotale = Val(datiGeneraliDocumento, "ImportoTotaleDocumento", soloFigli:=True)
        Dim importoBollo = Val(datiGeneraliDocumento, "ImportoBollo", soloFigli:=True)

        Dim sb As New Text.StringBuilder()
        sb.Append("<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""><style>")
        sb.Append("
            body { font-family: Segoe UI, Arial, sans-serif; font-size: 12px; color: #222; margin: 16px; }
            table { border-collapse: collapse; width: 100%; margin-bottom: 12px; }
            td, th { border: 1px solid #ccc; padding: 4px 6px; vertical-align: top; text-align: left; }
            th, .bar { background: #E8873A; color: #fff; font-weight: bold; text-transform: uppercase; font-size: 11px; }
            .box { border: 1px solid #ccc; padding: 8px; vertical-align: top; width: 50%; }
            .box h3 { margin: 0 0 6px 0; font-size: 12px; border-bottom: 2px solid #E8873A; padding-bottom: 3px; }
            .box table { border: none; margin: 0; }
            .box td { border: none; padding: 1px 0; }
            .num { text-align: right; }
            .center { text-align: center; }
            .totale { font-weight: bold; font-size: 13px; }
            .intestazione table { table-layout: fixed; }
        ")
        sb.Append("</style></head><body>")

        ' --- Cedente / Cessionario / Terzo Intermediario ---
        sb.Append("<table class=""intestazione""><tr>")
        sb.Append("<td class=""box"">")
        sb.Append("<h3>Cedente Prestatore (fornitore)</h3>")
        sb.Append($"Identificativo fiscale ai fini IVA: {H(EstraiPartitaIva(cedenteAnagrafici))}<br>")
        sb.Append($"Codice fiscale: {H(Val(cedenteAnagrafici, "CodiceFiscale", soloFigli:=True))}<br>")
        sb.Append($"Denominazione: {H(EstraiDenominazione(cedenteAnagrafici))}<br>")
        If Not String.IsNullOrEmpty(regimeFiscaleCodice) Then
            sb.Append($"Regime fiscale: {H(regimeFiscaleCodice)} {H(regimeFiscaleDescr)}<br>")
        End If
        sb.Append($"Indirizzo: {H(EstraiIndirizzo(cedenteSede))}<br>")
        sb.Append($"Comune: {H(Val(cedenteSede, "Comune", soloFigli:=True))} Provincia: {H(Val(cedenteSede, "Provincia", soloFigli:=True))}<br>")
        sb.Append($"Cap: {H(Val(cedenteSede, "CAP", soloFigli:=True))} Nazione: {H(Val(cedenteSede, "Nazione", soloFigli:=True))}<br>")
        Dim telefono = Val(cedenteContatti, "Telefono", soloFigli:=True)
        If Not String.IsNullOrEmpty(telefono) Then sb.Append($"Telefono: {H(telefono)}<br>")
        Dim email = Val(cedenteContatti, "Email", soloFigli:=True)
        If Not String.IsNullOrEmpty(email) Then sb.Append($"Email: {H(email)}<br>")

        If terzoAnagrafici IsNot Nothing Then
            sb.Append("<h3 style=""margin-top:10px"">Terzo Intermediario</h3>")
            sb.Append($"Identificativo fiscale ai fini IVA: {H(EstraiPartitaIva(terzoAnagrafici))}<br>")
            sb.Append($"Codice fiscale: {H(Val(terzoAnagrafici, "CodiceFiscale", soloFigli:=True))}<br>")
            sb.Append($"Denominazione: {H(EstraiDenominazione(terzoAnagrafici))}<br>")
        End If
        sb.Append("</td>")

        sb.Append("<td class=""box"">")
        sb.Append("<h3>Cessionario Committente (cliente)</h3>")
        sb.Append($"Identificativo fiscale ai fini IVA: {H(EstraiPartitaIva(cessionarioAnagrafici))}<br>")
        sb.Append($"Codice fiscale: {H(Val(cessionarioAnagrafici, "CodiceFiscale", soloFigli:=True))}<br>")
        sb.Append($"Denominazione: {H(EstraiDenominazione(cessionarioAnagrafici))}<br>")
        sb.Append($"Indirizzo: {H(EstraiIndirizzo(cessionarioSede))}<br>")
        sb.Append($"Comune: {H(Val(cessionarioSede, "Comune", soloFigli:=True))} Provincia: {H(Val(cessionarioSede, "Provincia", soloFigli:=True))}<br>")
        sb.Append($"Cap: {H(Val(cessionarioSede, "CAP", soloFigli:=True))} Nazione: {H(Val(cessionarioSede, "Nazione", soloFigli:=True))}<br>")
        sb.Append("</td>")
        sb.Append("</tr></table>")

        ' --- Dati documento ---
        sb.Append("<table><tr>")
        sb.Append("<th style=""width:55%"">Tipologia documento</th><th style=""width:8%"">Art73</th><th style=""width:12%"">Numero fattura</th><th style=""width:12%"">Data</th><th style=""width:13%"">Codice destinatario</th>")
        sb.Append("</tr><tr>")
        sb.Append($"<td>{H(tipoDocCodice)} ‐ {H(tipoDocDescr)}</td><td class=""center"">{H(art73)}</td><td>{H(numeroDoc)}</td><td>{FormattaData(dataDoc)}</td><td>{H(codiceDestinatario)}</td>")
        sb.Append("</tr></table>")

        ' --- Righe di dettaglio ---
        Dim datiBeniServizi = Nodo(body, "DatiBeniServizi")
        Dim righe = Nodi(datiBeniServizi, "DettaglioLinee")

        sb.Append("<table><tr>")
        sb.Append("<th style=""width:12%"">Cod. articolo</th><th>Descrizione</th><th style=""width:8%"">Quantità</th><th style=""width:9%"">Prezzo unitario</th><th style=""width:6%"">UM</th><th style=""width:8%"">Sc./Mag. %</th><th style=""width:6%"">%IVA</th><th style=""width:9%"">Prezzo totale</th>")
        sb.Append("</tr>")

        For Each riga In righe.OrderBy(Function(r) CInt(If(Val(r, "NumeroLinea", soloFigli:=True), "0")))
            Dim descrizione = Val(riga, "Descrizione", soloFigli:=True)
            Dim quantita = Val(riga, "Quantita", soloFigli:=True)
            Dim prezzoUnitario = Val(riga, "PrezzoUnitario", soloFigli:=True)
            Dim um = Val(riga, "UnitaMisura", soloFigli:=True)
            Dim iva = Val(riga, "AliquotaIVA", soloFigli:=True)
            Dim prezzoTotale = Val(riga, "PrezzoTotale", soloFigli:=True)

            Dim codArtNodo = Nodo(riga, "CodiceArticolo", soloFigli:=True)
            Dim codArt As String = ""
            If codArtNodo IsNot Nothing Then
                Dim tipo = Val(codArtNodo, "CodiceTipo", soloFigli:=True)
                Dim valore = Val(codArtNodo, "CodiceValore", soloFigli:=True)
                codArt = If(String.IsNullOrEmpty(tipo), valore, $"{valore} ({tipo})")
            End If

            Dim scontiTesto As New List(Of String)
            For Each sm In riga.Elements().Where(Function(x) x.Name.LocalName = "ScontoMaggiorazione")
                Dim tipoSm = Val(sm, "Tipo", soloFigli:=True)
                Dim percSm = Val(sm, "Percentuale", soloFigli:=True)
                Dim impSm = Val(sm, "Importo", soloFigli:=True)
                If Not String.IsNullOrEmpty(percSm) Then
                    scontiTesto.Add($"{tipoSm} {FormattaImporto(percSm)}%")
                ElseIf Not String.IsNullOrEmpty(impSm) Then
                    scontiTesto.Add($"{tipoSm} {FormattaImporto(impSm)}")
                End If
            Next

            sb.Append("<tr>")
            sb.Append($"<td>{H(codArt)}</td>")
            sb.Append($"<td>{HMultilinea(descrizione)}</td>")
            sb.Append($"<td class=""num"">{FormattaImporto(quantita)}</td>")
            sb.Append($"<td class=""num"">{FormattaImporto(prezzoUnitario)}</td>")
            sb.Append($"<td>{H(um)}</td>")
            sb.Append($"<td>{H(String.Join(" + ", scontiTesto))}</td>")
            sb.Append($"<td class=""num"">{FormattaImporto(iva)}</td>")
            sb.Append($"<td class=""num"">{FormattaImporto(prezzoTotale)}</td>")
            sb.Append("</tr>")
        Next
        sb.Append("</table>")

        ' --- Dati riepilogo IVA ---
        Dim riepiloghi = Nodi(datiBeniServizi, "DatiRiepilogo")
        If riepiloghi.Any() Then
            sb.Append("<table><tr>")
            sb.Append("<th>Note</th><th style=""width:10%"">%IVA</th><th style=""width:14%"">Totale imponibile</th><th style=""width:14%"">Totale imposta</th><th style=""width:14%"">Spese accessorie</th><th style=""width:10%"">Arr.</th>")
            sb.Append("</tr>")
            For Each r In riepiloghi
                Dim aliquota = Val(r, "AliquotaIVA", soloFigli:=True)
                Dim imponibile = Val(r, "ImponibileImporto", soloFigli:=True)
                Dim imposta = Val(r, "Imposta", soloFigli:=True)
                Dim esigibilita = Val(r, "EsigibilitaIVA", soloFigli:=True)
                Dim speseAcc = Val(r, "SpeseAccessorie", soloFigli:=True)
                Dim arrotondamento = Val(r, "Arrotondamento", soloFigli:=True)
                Dim esigibilitaDescr As String = Nothing
                If Not String.IsNullOrEmpty(esigibilita) Then DescrizioniEsigibilitaIVA.TryGetValue(esigibilita, esigibilitaDescr)

                sb.Append("<tr>")
                sb.Append($"<td>{H(esigibilitaDescr)}</td>")
                sb.Append($"<td class=""num"">{FormattaImporto(aliquota)}</td>")
                sb.Append($"<td class=""num"">{FormattaImporto(imponibile)}</td>")
                sb.Append($"<td class=""num"">{FormattaImporto(imposta)}</td>")
                sb.Append($"<td class=""num"">{FormattaImporto(speseAcc)}</td>")
                sb.Append($"<td class=""num"">{FormattaImporto(arrotondamento)}</td>")
                sb.Append("</tr>")
            Next
            sb.Append("</table>")
        End If

        ' --- Importo bollo / totale fattura ---
        sb.Append("<table><tr>")
        sb.Append("<th style=""width:20%"">Importo bollo</th><th style=""width:20%"">Sc./Mag.</th><th style=""width:20%"">Valuta</th><th style=""width:20%"">Arr.</th><th style=""width:20%"">Totale fattura</th>")
        sb.Append("</tr><tr>")
        sb.Append($"<td class=""num"">{FormattaImporto(importoBollo)}</td><td></td><td>{H(valuta)}</td><td></td><td class=""num totale"">{FormattaImporto(importoTotale)}</td>")
        sb.Append("</tr></table>")

        ' --- Dati pagamento ---
        ' DatiPagamento è figlio diretto di FatturaElettronicaBody, non di DatiGenerali.
        Dim dettagliPagamento = Nodi(body, "DettaglioPagamento")
        If dettagliPagamento.Any() Then
            sb.Append("<table><tr>")
            sb.Append("<th style=""width:20%"">Modalità pagamento</th><th>Dettagli</th><th style=""width:18%"">Scadenza</th><th style=""width:14%"">Importo</th>")
            sb.Append("</tr>")
            For Each dp In dettagliPagamento
                Dim modCodice = Val(dp, "ModalitaPagamento", soloFigli:=True)
                Dim modDescr As String = Nothing
                If Not String.IsNullOrEmpty(modCodice) Then DescrizioniModalitaPagamento.TryGetValue(modCodice, modDescr)
                Dim iban = Val(dp, "IBAN", soloFigli:=True)
                Dim istituto = Val(dp, "IstitutoFinanziario", soloFigli:=True)
                Dim scadenza = Val(dp, "DataScadenzaPagamento", soloFigli:=True)
                Dim importo = Val(dp, "ImportoPagamento", soloFigli:=True)

                Dim dettagli As New List(Of String)
                If Not String.IsNullOrEmpty(iban) Then dettagli.Add("IBAN " & iban)
                If Not String.IsNullOrEmpty(istituto) Then dettagli.Add(istituto)

                sb.Append("<tr>")
                sb.Append($"<td>{H(If(modDescr, modCodice))}</td>")
                sb.Append($"<td>{H(String.Join(vbCrLf, dettagli)).Replace(vbCrLf, "<br>")}</td>")
                sb.Append($"<td>{FormattaData(scadenza)}</td>")
                sb.Append($"<td class=""num"">{FormattaImporto(importo)}</td>")
                sb.Append("</tr>")
            Next
            sb.Append("</table>")
        End If

        ' --- Allegati ---
        Dim allegati = Nodi(body, "Allegati")
        If allegati.Any() Then
            sb.Append("<div class=""bar"" style=""padding:4px 6px"">Allegati</div><ul>")
            For Each al In allegati
                Dim nome = Val(al, "NomeAttachment", soloFigli:=True)
                Dim descr = Val(al, "DescrizioneAttachment", soloFigli:=True)
                sb.Append($"<li>{H(nome)}{If(String.IsNullOrEmpty(descr), "", " — " & H(descr))}</li>")
            Next
            sb.Append("</ul>")
        End If

        ' --- Piè di pagina ---
        Dim cedenteDenom = EstraiDenominazione(cedenteAnagrafici)
        sb.Append("<hr><div style=""font-size:10px;color:#777"">")
        sb.Append($"FT N. {H(numeroDoc)} DEL {FormattaData(dataDoc)} DI {H(cedenteDenom)}")
        If dataRicezione.HasValue Then
            sb.Append($" — DATA RICEZIONE {dataRicezione.Value:dd/MM/yyyy}")
        End If
        sb.Append("</div>")

        sb.Append("</body></html>")

        Return sb.ToString()
    End Function

End Module
