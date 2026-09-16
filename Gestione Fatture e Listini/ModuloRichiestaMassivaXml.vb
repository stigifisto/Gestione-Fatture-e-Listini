''' <summary>
''' Genera il file XML di richiesta massiva delle fatture elettroniche ricevute
''' da presentare all'Agenzia delle Entrate, secondo il tracciato "InputPubblico" di Sogei.
''' </summary>
Module ModuloRichiestaMassivaXml

    Private Const NamespaceInputPubblico As String = "http://www.sogei.it/InputPubblico"
    Private Const NamespaceXsi As String = "http://www.w3.org/2001/XMLSchema-instance"
    Private Const PivaRichiedente As String = "01312560335"

    ''' <summary>
    ''' Crea il file XML di richiesta massiva per l'intervallo di date indicato e lo salva
    ''' nella cartella di destinazione, restituendo il percorso completo del file generato.
    ''' </summary>
    Public Function GeneraXmlRichiestaMassiva(dataDa As Date, dataA As Date, cartellaDestinazione As String) As String
        Dim ns As XNamespace = NamespaceInputPubblico
        Dim xsi As XNamespace = NamespaceXsi

        Dim documento As New XDocument(
            New XDeclaration("1.0", "UTF-8", Nothing),
            New XElement(ns + "InputMassivo",
                New XAttribute(XNamespace.Xmlns + "ns1", ns.NamespaceName),
                New XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                New XAttribute(xsi + "schemaLocation", $"{ns.NamespaceName} untitled.xsd"),
                New XElement(ns + "TipoRichiesta",
                    New XElement(ns + "Fatture",
                        New XElement(ns + "Richiesta", "FATT"),
                        New XElement(ns + "ElencoPiva",
                            New XElement(ns + "Piva", PivaRichiedente)),
                        New XElement(ns + "TipoRicerca", "PUNTUALE"),
                        New XElement(ns + "FattureRicevute",
                            New XElement(ns + "DataRicezione",
                                New XElement(ns + "Da", dataDa.ToString("yyyy-MM-dd")),
                                New XElement(ns + "A", dataA.ToString("yyyy-MM-dd"))),
                            New XElement(ns + "Flusso",
                                New XElement(ns + "FatturaB2B", "CON")),
                            New XElement(ns + "Ruolo", "CESSIONARIO"))))))

        If Not IO.Directory.Exists(cartellaDestinazione) Then
            IO.Directory.CreateDirectory(cartellaDestinazione)
        End If

        Dim nomeFile As String = $"RichiestaMassiva_{dataDa:yyyyMMdd}_{dataA:yyyyMMdd}.xml"
        Dim percorsoCompleto As String = IO.Path.Combine(cartellaDestinazione, nomeFile)

        documento.Save(percorsoCompleto)

        Return percorsoCompleto
    End Function

End Module
