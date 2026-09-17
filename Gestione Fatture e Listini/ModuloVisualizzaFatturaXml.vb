Imports System.Reflection
Imports System.Xml
Imports System.Xml.Xsl

''' <summary>
''' Trasforma il file XML di una fattura elettronica nella rappresentazione HTML "formato
''' ministeriale", usando il foglio di stile ufficiale dell'Agenzia delle Entrate/Sogei
''' (fatturapa.gov.it), incorporato come risorsa nell'eseguibile.
''' </summary>
Module ModuloVisualizzaFatturaXml
    Private Const NomeRisorsaXsl As String = "Gestione_Fatture_e_Listini.Foglio_di_stile_fattura_ordinaria_ver1.2.3.xsl"

    Private trasformazioneCache As XslCompiledTransform

    Private Function OttieniTrasformazione() As XslCompiledTransform
        If trasformazioneCache Is Nothing Then
            Dim assemblyCorrente = Assembly.GetExecutingAssembly()
            Using stream = assemblyCorrente.GetManifestResourceStream(NomeRisorsaXsl)
                If stream Is Nothing Then
                    Throw New InvalidOperationException("Foglio di stile della fattura elettronica non trovato tra le risorse incorporate.")
                End If
                Using reader = XmlReader.Create(stream)
                    Dim xslt As New XslCompiledTransform()
                    xslt.Load(reader, New XsltSettings(True, True), New XmlUrlResolver())
                    trasformazioneCache = xslt
                End Using
            End Using
        End If
        Return trasformazioneCache
    End Function

    ''' <summary>
    ''' Applica il foglio di stile ufficiale al file XML indicato e restituisce l'HTML risultante,
    ''' pronto per essere mostrato in un controllo WebBrowser.
    ''' </summary>
    Public Function RenderizzaFatturaHtml(pathXml As String) As String
        Dim xslt = OttieniTrasformazione()

        Using lettoreXml = XmlReader.Create(pathXml)
            Dim sb As New Text.StringBuilder()
            Using sw As New IO.StringWriter(sb)
                xslt.Transform(lettoreXml, Nothing, sw)
            End Using
            Return sb.ToString()
        End Using
    End Function

    ''' <summary>
    ''' Forza il controllo WebBrowser (basato su MSHTML/IE) a renderizzare in modalità IE11
    ''' invece della modalità IE7 di default, necessaria per un layout CSS fedele.
    ''' Non blocca la visualizzazione se la scrittura del registro fallisce.
    ''' </summary>
    Public Sub ImpostaEmulazioneBrowserModerna()
        Try
            Dim nomeExe As String = IO.Path.GetFileName(Reflection.Assembly.GetEntryAssembly().Location)
            Using chiave = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                "Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION")
                chiave.SetValue(nomeExe, CInt(11001), Microsoft.Win32.RegistryValueKind.DWord)
            End Using
        Catch
            ' La fattura verrà comunque visualizzata, con un rendering meno fedele.
        End Try
    End Sub
End Module
