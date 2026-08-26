<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmPromoManuale
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LblDataInizio = New System.Windows.Forms.Label()
        Me.dtpDataInizio = New System.Windows.Forms.DateTimePicker()
        Me.LblDataFine = New System.Windows.Forms.Label()
        Me.dtpDataFine = New System.Windows.Forms.DateTimePicker()
        Me.LblFornitore = New System.Windows.Forms.Label()
        Me.cmbFornitore = New System.Windows.Forms.ComboBox()
        Me.LblArticolo = New System.Windows.Forms.Label()
        Me.cmbArticoli = New System.Windows.Forms.ComboBox()
        Me.LblDescrizione = New System.Windows.Forms.Label()
        Me.txtDescrizione = New System.Windows.Forms.TextBox()
        Me.LblCodiceAS400 = New System.Windows.Forms.Label()
        Me.txtCodiceAS400 = New System.Windows.Forms.TextBox()
        Me.LblUM = New System.Windows.Forms.Label()
        Me.txtUM = New System.Windows.Forms.TextBox()
        Me.LblPrezzoListino = New System.Windows.Forms.Label()
        Me.txtPrezzoListino = New System.Windows.Forms.TextBox()
        Me.LblPercentualePromo = New System.Windows.Forms.Label()
        Me.txtPercentualePromo = New System.Windows.Forms.TextBox()
        Me.LblPrezzoPromo = New System.Windows.Forms.Label()
        Me.txtPrezzoPromo = New System.Windows.Forms.TextBox()
        Me.cmdSalvaPromo = New System.Windows.Forms.Button()
        Me.cmdAnnullaPromo = New System.Windows.Forms.Button()
        Me.lblGrigliaAttuale = New System.Windows.Forms.Label()
        Me.dgvListinoAttuale = New System.Windows.Forms.DataGridView()
        Me.lblGrigliaDopo = New System.Windows.Forms.Label()
        Me.dgvListinoDopoPromo = New System.Windows.Forms.DataGridView()
        Me.LblRiduzioneFissa = New System.Windows.Forms.Label()
        Me.txtRiduzioneFissa = New System.Windows.Forms.TextBox()
        Me.cmdFile = New System.Windows.Forms.Button()
        Me.pnlFileEsterno = New System.Windows.Forms.Panel()
        Me.dgvRigheFile = New System.Windows.Forms.DataGridView()
        CType(Me.dgvListinoAttuale, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvListinoDopoPromo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlFileEsterno.SuspendLayout()
        CType(Me.dgvRigheFile, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(315, 23)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "INSERIMENTO MANUALE PROMOZIONI"
        '
        'LblDataInizio
        '
        Me.LblDataInizio.AutoSize = True
        Me.LblDataInizio.Location = New System.Drawing.Point(13, 50)
        Me.LblDataInizio.Name = "LblDataInizio"
        Me.LblDataInizio.Size = New System.Drawing.Size(114, 13)
        Me.LblDataInizio.TabIndex = 1
        Me.LblDataInizio.Text = "DATA INIZIO PROMO"
        '
        'dtpDataInizio
        '
        Me.dtpDataInizio.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpDataInizio.Location = New System.Drawing.Point(16, 66)
        Me.dtpDataInizio.Name = "dtpDataInizio"
        Me.dtpDataInizio.Size = New System.Drawing.Size(180, 20)
        Me.dtpDataInizio.TabIndex = 2
        '
        'LblDataFine
        '
        Me.LblDataFine.AutoSize = True
        Me.LblDataFine.Location = New System.Drawing.Point(226, 50)
        Me.LblDataFine.Name = "LblDataFine"
        Me.LblDataFine.Size = New System.Drawing.Size(106, 13)
        Me.LblDataFine.TabIndex = 3
        Me.LblDataFine.Text = "DATA FINE PROMO"
        '
        'dtpDataFine
        '
        Me.dtpDataFine.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpDataFine.Location = New System.Drawing.Point(229, 66)
        Me.dtpDataFine.Name = "dtpDataFine"
        Me.dtpDataFine.Size = New System.Drawing.Size(177, 20)
        Me.dtpDataFine.TabIndex = 4
        '
        'LblFornitore
        '
        Me.LblFornitore.AutoSize = True
        Me.LblFornitore.Location = New System.Drawing.Point(13, 100)
        Me.LblFornitore.Name = "LblFornitore"
        Me.LblFornitore.Size = New System.Drawing.Size(145, 13)
        Me.LblFornitore.TabIndex = 5
        Me.LblFornitore.Text = "SELEZIONA IL FORNITORE"
        '
        'cmbFornitore
        '
        Me.cmbFornitore.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFornitore.FormattingEnabled = True
        Me.cmbFornitore.Location = New System.Drawing.Point(16, 116)
        Me.cmbFornitore.Name = "cmbFornitore"
        Me.cmbFornitore.Size = New System.Drawing.Size(280, 21)
        Me.cmbFornitore.TabIndex = 6
        '
        'LblArticolo
        '
        Me.LblArticolo.AutoSize = True
        Me.LblArticolo.Location = New System.Drawing.Point(13, 150)
        Me.LblArticolo.Name = "LblArticolo"
        Me.LblArticolo.Size = New System.Drawing.Size(321, 13)
        Me.LblArticolo.TabIndex = 7
        Me.LblArticolo.Text = "CODICE ARTICOLO FORNITORE (COFCOF) - digitare per cercare"
        '
        'cmbArticoli
        '
        Me.cmbArticoli.FormattingEnabled = True
        Me.cmbArticoli.Location = New System.Drawing.Point(16, 166)
        Me.cmbArticoli.Name = "cmbArticoli"
        Me.cmbArticoli.Size = New System.Drawing.Size(390, 21)
        Me.cmbArticoli.TabIndex = 8
        '
        'LblDescrizione
        '
        Me.LblDescrizione.AutoSize = True
        Me.LblDescrizione.Location = New System.Drawing.Point(13, 200)
        Me.LblDescrizione.Name = "LblDescrizione"
        Me.LblDescrizione.Size = New System.Drawing.Size(80, 13)
        Me.LblDescrizione.TabIndex = 9
        Me.LblDescrizione.Text = "DESCRIZIONE"
        '
        'txtDescrizione
        '
        Me.txtDescrizione.Location = New System.Drawing.Point(16, 216)
        Me.txtDescrizione.Name = "txtDescrizione"
        Me.txtDescrizione.ReadOnly = True
        Me.txtDescrizione.Size = New System.Drawing.Size(390, 20)
        Me.txtDescrizione.TabIndex = 10
        '
        'LblCodiceAS400
        '
        Me.LblCodiceAS400.AutoSize = True
        Me.LblCodiceAS400.Location = New System.Drawing.Point(13, 250)
        Me.LblCodiceAS400.Name = "LblCodiceAS400"
        Me.LblCodiceAS400.Size = New System.Drawing.Size(82, 13)
        Me.LblCodiceAS400.TabIndex = 11
        Me.LblCodiceAS400.Text = "CODICE AS400"
        '
        'txtCodiceAS400
        '
        Me.txtCodiceAS400.Location = New System.Drawing.Point(16, 266)
        Me.txtCodiceAS400.Name = "txtCodiceAS400"
        Me.txtCodiceAS400.ReadOnly = True
        Me.txtCodiceAS400.Size = New System.Drawing.Size(180, 20)
        Me.txtCodiceAS400.TabIndex = 12
        '
        'LblUM
        '
        Me.LblUM.AutoSize = True
        Me.LblUM.Location = New System.Drawing.Point(226, 250)
        Me.LblUM.Name = "LblUM"
        Me.LblUM.Size = New System.Drawing.Size(101, 13)
        Me.LblUM.TabIndex = 13
        Me.LblUM.Text = "UNITA' DI MISURA"
        '
        'txtUM
        '
        Me.txtUM.Location = New System.Drawing.Point(229, 266)
        Me.txtUM.Name = "txtUM"
        Me.txtUM.ReadOnly = True
        Me.txtUM.Size = New System.Drawing.Size(177, 20)
        Me.txtUM.TabIndex = 14
        '
        'LblPrezzoListino
        '
        Me.LblPrezzoListino.AutoSize = True
        Me.LblPrezzoListino.Location = New System.Drawing.Point(13, 300)
        Me.LblPrezzoListino.Name = "LblPrezzoListino"
        Me.LblPrezzoListino.Size = New System.Drawing.Size(110, 13)
        Me.LblPrezzoListino.TabIndex = 15
        Me.LblPrezzoListino.Text = "PREZZO DI LISTINO"
        '
        'txtPrezzoListino
        '
        Me.txtPrezzoListino.Location = New System.Drawing.Point(16, 316)
        Me.txtPrezzoListino.Name = "txtPrezzoListino"
        Me.txtPrezzoListino.ReadOnly = True
        Me.txtPrezzoListino.Size = New System.Drawing.Size(180, 20)
        Me.txtPrezzoListino.TabIndex = 16
        '
        'LblPercentualePromo
        '
        Me.LblPercentualePromo.AutoSize = True
        Me.LblPercentualePromo.Location = New System.Drawing.Point(226, 300)
        Me.LblPercentualePromo.Name = "LblPercentualePromo"
        Me.LblPercentualePromo.Size = New System.Drawing.Size(121, 13)
        Me.LblPercentualePromo.TabIndex = 17
        Me.LblPercentualePromo.Text = "PROMO % SU LISTINO"
        '
        'txtPercentualePromo
        '
        Me.txtPercentualePromo.Location = New System.Drawing.Point(229, 316)
        Me.txtPercentualePromo.Name = "txtPercentualePromo"
        Me.txtPercentualePromo.Size = New System.Drawing.Size(177, 20)
        Me.txtPercentualePromo.TabIndex = 18
        '
        'LblPrezzoPromo
        '
        Me.LblPrezzoPromo.AutoSize = True
        Me.LblPrezzoPromo.Location = New System.Drawing.Point(13, 350)
        Me.LblPrezzoPromo.Name = "LblPrezzoPromo"
        Me.LblPrezzoPromo.Size = New System.Drawing.Size(140, 13)
        Me.LblPrezzoPromo.TabIndex = 19
        Me.LblPrezzoPromo.Text = "PREZZO PROMOZIONALE"
        '
        'txtPrezzoPromo
        '
        Me.txtPrezzoPromo.Location = New System.Drawing.Point(16, 366)
        Me.txtPrezzoPromo.Name = "txtPrezzoPromo"
        Me.txtPrezzoPromo.Size = New System.Drawing.Size(180, 20)
        Me.txtPrezzoPromo.TabIndex = 20
        '
        'cmdSalvaPromo
        '
        Me.cmdSalvaPromo.Location = New System.Drawing.Point(16, 410)
        Me.cmdSalvaPromo.Name = "cmdSalvaPromo"
        Me.cmdSalvaPromo.Size = New System.Drawing.Size(200, 40)
        Me.cmdSalvaPromo.TabIndex = 21
        Me.cmdSalvaPromo.Text = "SALVA PROMOZIONE"
        Me.cmdSalvaPromo.UseVisualStyleBackColor = True
        '
        'cmdAnnullaPromo
        '
        Me.cmdAnnullaPromo.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdAnnullaPromo.Location = New System.Drawing.Point(230, 410)
        Me.cmdAnnullaPromo.Name = "cmdAnnullaPromo"
        Me.cmdAnnullaPromo.Size = New System.Drawing.Size(176, 40)
        Me.cmdAnnullaPromo.TabIndex = 22
        Me.cmdAnnullaPromo.Text = "ANNULLA"
        Me.cmdAnnullaPromo.UseVisualStyleBackColor = True
        '
        'lblGrigliaAttuale
        '
        Me.lblGrigliaAttuale.AutoSize = True
        Me.lblGrigliaAttuale.Location = New System.Drawing.Point(420, 47)
        Me.lblGrigliaAttuale.Name = "lblGrigliaAttuale"
        Me.lblGrigliaAttuale.Size = New System.Drawing.Size(166, 13)
        Me.lblGrigliaAttuale.TabIndex = 23
        Me.lblGrigliaAttuale.Text = "LISTINO PRIMA DELLA PROMO"
        '
        'dgvListinoAttuale
        '
        Me.dgvListinoAttuale.AllowUserToAddRows = False
        Me.dgvListinoAttuale.AllowUserToDeleteRows = False
        Me.dgvListinoAttuale.AllowUserToResizeRows = False
        Me.dgvListinoAttuale.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvListinoAttuale.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvListinoAttuale.Location = New System.Drawing.Point(420, 63)
        Me.dgvListinoAttuale.MultiSelect = False
        Me.dgvListinoAttuale.Name = "dgvListinoAttuale"
        Me.dgvListinoAttuale.ReadOnly = True
        Me.dgvListinoAttuale.RowHeadersVisible = False
        Me.dgvListinoAttuale.Size = New System.Drawing.Size(330, 400)
        Me.dgvListinoAttuale.TabIndex = 24
        '
        'lblGrigliaDopo
        '
        Me.lblGrigliaDopo.AutoSize = True
        Me.lblGrigliaDopo.Location = New System.Drawing.Point(770, 47)
        Me.lblGrigliaDopo.Name = "lblGrigliaDopo"
        Me.lblGrigliaDopo.Size = New System.Drawing.Size(142, 13)
        Me.lblGrigliaDopo.TabIndex = 25
        Me.lblGrigliaDopo.Text = "LISTINO DOPO LA PROMO"
        '
        'dgvListinoDopoPromo
        '
        Me.dgvListinoDopoPromo.AllowUserToAddRows = False
        Me.dgvListinoDopoPromo.AllowUserToDeleteRows = False
        Me.dgvListinoDopoPromo.AllowUserToResizeRows = False
        Me.dgvListinoDopoPromo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvListinoDopoPromo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvListinoDopoPromo.Location = New System.Drawing.Point(770, 63)
        Me.dgvListinoDopoPromo.MultiSelect = False
        Me.dgvListinoDopoPromo.Name = "dgvListinoDopoPromo"
        Me.dgvListinoDopoPromo.ReadOnly = True
        Me.dgvListinoDopoPromo.RowHeadersVisible = False
        Me.dgvListinoDopoPromo.Size = New System.Drawing.Size(330, 400)
        Me.dgvListinoDopoPromo.TabIndex = 26
        '
        'LblRiduzioneFissa
        '
        Me.LblRiduzioneFissa.AutoSize = True
        Me.LblRiduzioneFissa.Location = New System.Drawing.Point(226, 350)
        Me.LblRiduzioneFissa.Name = "LblRiduzioneFissa"
        Me.LblRiduzioneFissa.Size = New System.Drawing.Size(163, 13)
        Me.LblRiduzioneFissa.TabIndex = 27
        Me.LblRiduzioneFissa.Text = "RIDUZIONE FISSA SU LISTINO"
        '
        'txtRiduzioneFissa
        '
        Me.txtRiduzioneFissa.Location = New System.Drawing.Point(229, 366)
        Me.txtRiduzioneFissa.Name = "txtRiduzioneFissa"
        Me.txtRiduzioneFissa.Size = New System.Drawing.Size(177, 20)
        Me.txtRiduzioneFissa.TabIndex = 28
        '
        'cmdFile
        '
        Me.cmdFile.Location = New System.Drawing.Point(302, 100)
        Me.cmdFile.Name = "cmdFile"
        Me.cmdFile.Size = New System.Drawing.Size(112, 40)
        Me.cmdFile.TabIndex = 29
        Me.cmdFile.Text = "CARICA DA FILE"
        Me.cmdFile.UseVisualStyleBackColor = True
        '
        'pnlFileEsterno
        '
        Me.pnlFileEsterno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlFileEsterno.Controls.Add(Me.dgvRigheFile)
        Me.pnlFileEsterno.Location = New System.Drawing.Point(13, 150)
        Me.pnlFileEsterno.Name = "pnlFileEsterno"
        Me.pnlFileEsterno.Size = New System.Drawing.Size(397, 500)
        Me.pnlFileEsterno.TabIndex = 30
        Me.pnlFileEsterno.Visible = False
        '
        'dgvRigheFile
        '
        Me.dgvRigheFile.AllowUserToAddRows = False
        Me.dgvRigheFile.AllowUserToDeleteRows = False
        Me.dgvRigheFile.AllowUserToResizeRows = False
        Me.dgvRigheFile.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvRigheFile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvRigheFile.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvRigheFile.Location = New System.Drawing.Point(0, 0)
        Me.dgvRigheFile.MultiSelect = False
        Me.dgvRigheFile.Name = "dgvRigheFile"
        Me.dgvRigheFile.ReadOnly = True
        Me.dgvRigheFile.RowHeadersVisible = False
        Me.dgvRigheFile.Size = New System.Drawing.Size(395, 498)
        Me.dgvRigheFile.TabIndex = 0
        '
        'FrmPromoManuale
        '
        Me.AcceptButton = Me.cmdSalvaPromo
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdAnnullaPromo
        Me.ClientSize = New System.Drawing.Size(1120, 485)
        Me.Controls.Add(Me.pnlFileEsterno)
        Me.Controls.Add(Me.cmdFile)
        Me.Controls.Add(Me.txtRiduzioneFissa)
        Me.Controls.Add(Me.LblRiduzioneFissa)
        Me.Controls.Add(Me.dgvListinoDopoPromo)
        Me.Controls.Add(Me.lblGrigliaDopo)
        Me.Controls.Add(Me.dgvListinoAttuale)
        Me.Controls.Add(Me.lblGrigliaAttuale)
        Me.Controls.Add(Me.cmdAnnullaPromo)
        Me.Controls.Add(Me.cmdSalvaPromo)
        Me.Controls.Add(Me.dtpDataFine)
        Me.Controls.Add(Me.LblDataFine)
        Me.Controls.Add(Me.dtpDataInizio)
        Me.Controls.Add(Me.LblDataInizio)
        Me.Controls.Add(Me.txtPrezzoPromo)
        Me.Controls.Add(Me.LblPrezzoPromo)
        Me.Controls.Add(Me.txtPercentualePromo)
        Me.Controls.Add(Me.LblPercentualePromo)
        Me.Controls.Add(Me.txtPrezzoListino)
        Me.Controls.Add(Me.LblPrezzoListino)
        Me.Controls.Add(Me.txtUM)
        Me.Controls.Add(Me.LblUM)
        Me.Controls.Add(Me.txtCodiceAS400)
        Me.Controls.Add(Me.LblCodiceAS400)
        Me.Controls.Add(Me.txtDescrizione)
        Me.Controls.Add(Me.LblDescrizione)
        Me.Controls.Add(Me.cmbArticoli)
        Me.Controls.Add(Me.LblArticolo)
        Me.Controls.Add(Me.cmbFornitore)
        Me.Controls.Add(Me.LblFornitore)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmPromoManuale"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Inserimento manuale promozioni"
        CType(Me.dgvListinoAttuale, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvListinoDopoPromo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlFileEsterno.ResumeLayout(False)
        CType(Me.dgvRigheFile, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents LblFornitore As Label
    Friend WithEvents cmbFornitore As ComboBox
    Friend WithEvents LblArticolo As Label
    Friend WithEvents cmbArticoli As ComboBox
    Friend WithEvents LblDescrizione As Label
    Friend WithEvents txtDescrizione As TextBox
    Friend WithEvents LblCodiceAS400 As Label
    Friend WithEvents txtCodiceAS400 As TextBox
    Friend WithEvents LblUM As Label
    Friend WithEvents txtUM As TextBox
    Friend WithEvents LblPrezzoListino As Label
    Friend WithEvents txtPrezzoListino As TextBox
    Friend WithEvents LblPercentualePromo As Label
    Friend WithEvents txtPercentualePromo As TextBox
    Friend WithEvents LblPrezzoPromo As Label
    Friend WithEvents txtPrezzoPromo As TextBox
    Friend WithEvents LblDataInizio As Label
    Friend WithEvents dtpDataInizio As DateTimePicker
    Friend WithEvents LblDataFine As Label
    Friend WithEvents dtpDataFine As DateTimePicker
    Friend WithEvents cmdSalvaPromo As Button
    Friend WithEvents cmdAnnullaPromo As Button
    Friend WithEvents lblGrigliaAttuale As Label
    Friend WithEvents dgvListinoAttuale As DataGridView
    Friend WithEvents lblGrigliaDopo As Label
    Friend WithEvents dgvListinoDopoPromo As DataGridView
    Friend WithEvents LblRiduzioneFissa As Label
    Friend WithEvents txtRiduzioneFissa As TextBox
    Friend WithEvents cmdFile As Button
    Friend WithEvents pnlFileEsterno As Panel
    Friend WithEvents dgvRigheFile As DataGridView
End Class
