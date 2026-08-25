Imports System.Drawing
Imports System.Drawing.Drawing2D

''' <summary>
''' Icone dei 5 riquadri di frmMain, disegnate a runtime con GDI+ (nessuna immagine/icona
''' pronta era disponibile in nessuno dei due progetti originali, né è possibile recuperarne
''' una da internet con licenza chiara): forme geometriche semplici in stile flat/outline,
''' un colore di accento per riquadro.
''' </summary>
Module IconeHub
    Private Const Dimensione As Integer = 56
    Private Const Spessore As Single = 4.0F

    Private Function NuovaTela(ByRef gr As Graphics) As Bitmap
        Dim bmp As New Bitmap(Dimensione, Dimensione, Imaging.PixelFormat.Format32bppArgb)
        gr = Graphics.FromImage(bmp)
        gr.SmoothingMode = SmoothingMode.AntiAlias
        Return bmp
    End Function

    Private Function NuovaPenna(colore As Color) As Pen
        Dim p As New Pen(colore, Spessore)
        p.StartCap = LineCap.Round
        p.EndCap = LineCap.Round
        p.LineJoin = LineJoin.Round
        Return p
    End Function

    ''' <summary>Caricamento listini fornitori: pagina con righe di testo e freccia di importazione.</summary>
    Public Function CreaIconaListini() As Bitmap
        Dim colore As Color = Color.FromArgb(47, 128, 237) ' blu
        Dim gr As Graphics = Nothing
        Dim bmp As Bitmap = NuovaTela(gr)
        Using gr
            Using penna As Pen = NuovaPenna(colore)
                Dim pagina As New Rectangle(12, 4, 24, 32)
                gr.DrawRectangle(penna, pagina)
                gr.DrawLine(penna, 17, 14, 31, 14)
                gr.DrawLine(penna, 17, 20, 31, 20)
                gr.DrawLine(penna, 17, 26, 27, 26)

                ' Freccia di importazione (verso il basso) sovrapposta in basso a destra
                gr.DrawLine(penna, 40, 26, 40, 46)
                gr.DrawLine(penna, 32, 38, 40, 46)
                gr.DrawLine(penna, 48, 38, 40, 46)
            End Using
        End Using
        Return bmp
    End Function

    ''' <summary>Consolidamento dati: cilindro/database.</summary>
    Public Function CreaIconaConsolidamento() As Bitmap
        Dim colore As Color = Color.FromArgb(39, 174, 96) ' verde
        Dim gr As Graphics = Nothing
        Dim bmp As Bitmap = NuovaTela(gr)
        Using gr
            Using penna As Pen = NuovaPenna(colore)
                Dim x As Integer = 10, larghezza As Integer = 36, altezzaEllisse As Integer = 12
                gr.DrawEllipse(penna, x, 8, larghezza, altezzaEllisse)
                gr.DrawLine(penna, x, 8 + altezzaEllisse \ 2, x, 40)
                gr.DrawLine(penna, x + larghezza, 8 + altezzaEllisse \ 2, x + larghezza, 40)
                gr.DrawArc(penna, x, 34, larghezza, altezzaEllisse, 0, 180)
                gr.DrawArc(penna, x, 21, larghezza, altezzaEllisse, 0, 180)
            End Using
        End Using
        Return bmp
    End Function

    ''' <summary>Importazione fatture elettroniche: busta/documento elettronico.</summary>
    Public Function CreaIconaFattureElettroniche() As Bitmap
        Dim colore As Color = Color.FromArgb(242, 153, 74) ' arancio
        Dim gr As Graphics = Nothing
        Dim bmp As Bitmap = NuovaTela(gr)
        Using gr
            Using penna As Pen = NuovaPenna(colore)
                Dim busta As New Rectangle(6, 14, 44, 30)
                gr.DrawRectangle(penna, busta)
                gr.DrawLine(penna, 6, 14, 28, 34)
                gr.DrawLine(penna, 50, 14, 28, 34)
            End Using
        End Using
        Return bmp
    End Function

    ''' <summary>Analisi prezzi fatture: grafico a barre con lente d'ingrandimento.</summary>
    Public Function CreaIconaAnalisiPrezzi() As Bitmap
        Dim colore As Color = Color.FromArgb(155, 81, 224) ' viola
        Dim gr As Graphics = Nothing
        Dim bmp As Bitmap = NuovaTela(gr)
        Using gr
            Using penna As Pen = NuovaPenna(colore)
                gr.DrawLine(penna, 8, 44, 8, 30)
                gr.DrawLine(penna, 18, 44, 18, 22)
                gr.DrawLine(penna, 28, 44, 28, 34)
                gr.DrawLine(penna, 6, 44, 34, 44)

                Dim centro As New Point(38, 20)
                Dim raggio As Integer = 10
                gr.DrawEllipse(penna, centro.X - raggio, centro.Y - raggio, raggio * 2, raggio * 2)
                Dim dirX As Double = Math.Cos(Math.PI / 4)
                Dim dirY As Double = Math.Sin(Math.PI / 4)
                gr.DrawLine(penna,
                            CSng(centro.X + raggio * dirX), CSng(centro.Y + raggio * dirY),
                            CSng(centro.X + (raggio + 8) * dirX), CSng(centro.Y + (raggio + 8) * dirY))
            End Using
        End Using
        Return bmp
    End Function

    ''' <summary>Impostazioni: ingranaggio.</summary>
    Public Function CreaIconaImpostazioni() As Bitmap
        Dim colore As Color = Color.FromArgb(79, 79, 79) ' grigio scuro
        Dim gr As Graphics = Nothing
        Dim bmp As Bitmap = NuovaTela(gr)
        Using gr
            Using penna As Pen = NuovaPenna(colore)
                Dim centro As New PointF(28, 28)
                Dim raggioEsterno As Single = 16
                Dim raggioInterno As Single = 9

                gr.DrawEllipse(penna, centro.X - raggioEsterno, centro.Y - raggioEsterno, raggioEsterno * 2, raggioEsterno * 2)
                gr.DrawEllipse(penna, centro.X - raggioInterno, centro.Y - raggioInterno, raggioInterno * 2, raggioInterno * 2)

                Dim numDenti As Integer = 8
                For i As Integer = 0 To numDenti - 1
                    Dim angolo As Double = 2 * Math.PI * i / numDenti
                    Dim x1 As Single = CSng(centro.X + raggioEsterno * Math.Cos(angolo))
                    Dim y1 As Single = CSng(centro.Y + raggioEsterno * Math.Sin(angolo))
                    Dim x2 As Single = CSng(centro.X + (raggioEsterno + 6) * Math.Cos(angolo))
                    Dim y2 As Single = CSng(centro.Y + (raggioEsterno + 6) * Math.Sin(angolo))
                    gr.DrawLine(penna, x1, y1, x2, y2)
                Next
            End Using
        End Using
        Return bmp
    End Function
End Module
