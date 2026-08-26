<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmEstrattore
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Public Sub New()
        MyBase.New()

        'Chiamata richiesta dal progettista.
        InitializeComponent()
    End Sub

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.
    'Non modificarla mediante l'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.txtSalva = New System.Windows.Forms.TextBox()
        Me.txtCarica = New System.Windows.Forms.TextBox()
        Me.cmdEsegui = New System.Windows.Forms.Button()
        Me.cmdSalva = New System.Windows.Forms.Button()
        Me.cmdCarica = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cmbFornitori = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.CmdPromoMan = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.dtpIni = New System.Windows.Forms.DateTimePicker()
        Me.cmdAs400 = New System.Windows.Forms.Button()
        Me.txtExcel = New System.Windows.Forms.TextBox()
        Me.cmdXls = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cmbLiafor = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ofdApri = New System.Windows.Forms.OpenFileDialog()
        Me.sfdSalva = New System.Windows.Forms.SaveFileDialog()
        Me.dgvRisultati = New System.Windows.Forms.DataGridView()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.lblStato = New System.Windows.Forms.ToolStripStatusLabel()
        Me.prgElaborazione = New System.Windows.Forms.ToolStripProgressBar()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.dgvRisultati, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.txtSalva)
        Me.Panel1.Controls.Add(Me.txtCarica)
        Me.Panel1.Controls.Add(Me.cmdEsegui)
        Me.Panel1.Controls.Add(Me.cmdSalva)
        Me.Panel1.Controls.Add(Me.cmdCarica)
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.cmbFornitori)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Location = New System.Drawing.Point(1, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(526, 382)
        Me.Panel1.TabIndex = 0
        '
        'txtSalva
        '
        Me.txtSalva.Location = New System.Drawing.Point(116, 213)
        Me.txtSalva.Multiline = True
        Me.txtSalva.Name = "txtSalva"
        Me.txtSalva.Size = New System.Drawing.Size(386, 67)
        Me.txtSalva.TabIndex = 8
        '
        'txtCarica
        '
        Me.txtCarica.Location = New System.Drawing.Point(116, 130)
        Me.txtCarica.Multiline = True
        Me.txtCarica.Name = "txtCarica"
        Me.txtCarica.Size = New System.Drawing.Size(386, 67)
        Me.txtCarica.TabIndex = 7
        '
        'cmdEsegui
        '
        Me.cmdEsegui.Location = New System.Drawing.Point(84, 290)
        Me.cmdEsegui.Name = "cmdEsegui"
        Me.cmdEsegui.Size = New System.Drawing.Size(358, 45)
        Me.cmdEsegui.TabIndex = 6
        Me.cmdEsegui.Text = "ESEGUI TRASFORMAZIONE"
        Me.cmdEsegui.UseVisualStyleBackColor = True
        '
        'cmdSalva
        '
        Me.cmdSalva.Location = New System.Drawing.Point(15, 213)
        Me.cmdSalva.Name = "cmdSalva"
        Me.cmdSalva.Size = New System.Drawing.Size(95, 45)
        Me.cmdSalva.TabIndex = 5
        Me.cmdSalva.Text = "SALVA OUTPUT"
        Me.cmdSalva.UseVisualStyleBackColor = True
        '
        'cmdCarica
        '
        Me.cmdCarica.Location = New System.Drawing.Point(15, 130)
        Me.cmdCarica.Name = "cmdCarica"
        Me.cmdCarica.Size = New System.Drawing.Size(95, 45)
        Me.cmdCarica.TabIndex = 4
        Me.cmdCarica.Text = "CARICA FILE PDF"
        Me.cmdCarica.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 53)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(145, 13)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "SELEZIONA IL FORNITORE"
        '
        'cmbFornitori
        '
        Me.cmbFornitori.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFornitori.FormattingEnabled = True
        Me.cmbFornitori.Location = New System.Drawing.Point(15, 69)
        Me.cmbFornitori.Name = "cmbFornitori"
        Me.cmbFornitori.Size = New System.Drawing.Size(191, 21)
        Me.cmbFornitori.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(11, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(228, 23)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "TRASFORMAZIONE FILE PDF"
        '
        'Panel2
        '
        Me.Panel2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel2.Controls.Add(Me.CmdPromoMan)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.dtpIni)
        Me.Panel2.Controls.Add(Me.cmdAs400)
        Me.Panel2.Controls.Add(Me.txtExcel)
        Me.Panel2.Controls.Add(Me.cmdXls)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.cmbLiafor)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Location = New System.Drawing.Point(533, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(533, 382)
        Me.Panel2.TabIndex = 1
        '
        'CmdPromoMan
        '
        Me.CmdPromoMan.Location = New System.Drawing.Point(115, 349)
        Me.CmdPromoMan.Name = "CmdPromoMan"
        Me.CmdPromoMan.Size = New System.Drawing.Size(274, 23)
        Me.CmdPromoMan.TabIndex = 12
        Me.CmdPromoMan.Text = "CARICAMENTO MANUALE PROMOZIONI"
        Me.CmdPromoMan.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(4, 226)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(159, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "DATA DECORRENZA LISTINO"
        '
        'dtpIni
        '
        Me.dtpIni.Location = New System.Drawing.Point(169, 223)
        Me.dtpIni.Name = "dtpIni"
        Me.dtpIni.Size = New System.Drawing.Size(200, 20)
        Me.dtpIni.TabIndex = 10
        '
        'cmdAs400
        '
        Me.cmdAs400.Location = New System.Drawing.Point(74, 290)
        Me.cmdAs400.Name = "cmdAs400"
        Me.cmdAs400.Size = New System.Drawing.Size(358, 45)
        Me.cmdAs400.TabIndex = 9
        Me.cmdAs400.Text = "IMPORTA LISTINO IN AS400"
        Me.cmdAs400.UseVisualStyleBackColor = True
        '
        'txtExcel
        '
        Me.txtExcel.Location = New System.Drawing.Point(115, 132)
        Me.txtExcel.Multiline = True
        Me.txtExcel.Name = "txtExcel"
        Me.txtExcel.Size = New System.Drawing.Size(386, 67)
        Me.txtExcel.TabIndex = 9
        '
        'cmdXls
        '
        Me.cmdXls.Location = New System.Drawing.Point(14, 132)
        Me.cmdXls.Name = "cmdXls"
        Me.cmdXls.Size = New System.Drawing.Size(95, 45)
        Me.cmdXls.TabIndex = 8
        Me.cmdXls.Text = "SELEZIONA FILE EXCEL"
        Me.cmdXls.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(302, 53)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(145, 13)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "SELEZIONA IL FORNITORE"
        '
        'cmbLiafor
        '
        Me.cmbLiafor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbLiafor.FormattingEnabled = True
        Me.cmbLiafor.Location = New System.Drawing.Point(305, 69)
        Me.cmbLiafor.Name = "cmbLiafor"
        Me.cmbLiafor.Size = New System.Drawing.Size(191, 21)
        Me.cmbLiafor.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(301, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(202, 23)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "CARICAMENTO IN AS400"
        '
        'ofdApri
        '
        Me.ofdApri.Filter = "File PDF|*.pdf"
        '
        'sfdSalva
        '
        Me.sfdSalva.Filter = "File Excel|*.xlsx"
        '
        'dgvRisultati
        '
        Me.dgvRisultati.AllowUserToAddRows = False
        Me.dgvRisultati.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvRisultati.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dgvRisultati.Location = New System.Drawing.Point(1, 390)
        Me.dgvRisultati.Name = "dgvRisultati"
        Me.dgvRisultati.ReadOnly = True
        Me.dgvRisultati.Size = New System.Drawing.Size(1052, 250)
        Me.dgvRisultati.TabIndex = 2
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStato, Me.prgElaborazione})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 650)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1054, 22)
        Me.StatusStrip1.TabIndex = 3
        '
        'lblStato
        '
        Me.lblStato.Name = "lblStato"
        Me.lblStato.Size = New System.Drawing.Size(1039, 17)
        Me.lblStato.Spring = True
        Me.lblStato.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'prgElaborazione
        '
        Me.prgElaborazione.Name = "prgElaborazione"
        Me.prgElaborazione.Size = New System.Drawing.Size(200, 16)
        Me.prgElaborazione.Visible = False
        '
        'FrmEstrattore
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1054, 672)
        Me.Controls.Add(Me.dgvRisultati)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Name = "FrmEstrattore"
        Me.Text = "CARICAMENTO LISTINI FORNITORI"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        CType(Me.dgvRisultati, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents cmdEsegui As Button
    Friend WithEvents cmdSalva As Button
    Friend WithEvents cmdCarica As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents cmbFornitori As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents ofdApri As OpenFileDialog
    Friend WithEvents sfdSalva As SaveFileDialog
    Friend WithEvents txtCarica As TextBox
    Friend WithEvents txtSalva As TextBox
    Friend WithEvents cmdAs400 As Button
    Friend WithEvents txtExcel As TextBox
    Friend WithEvents cmdXls As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents cmbLiafor As ComboBox
    Friend WithEvents dgvRisultati As DataGridView
    Friend WithEvents Label5 As Label
    Friend WithEvents dtpIni As DateTimePicker
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents lblStato As ToolStripStatusLabel
    Friend WithEvents prgElaborazione As ToolStripProgressBar
    Friend WithEvents CmdPromoMan As Button
End Class
