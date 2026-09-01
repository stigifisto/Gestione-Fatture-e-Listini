<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPrezziAS400
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Public Sub New()
        MyBase.New()

        'Chiamata richiesta dal progettista.
        InitializeComponent()
    End Sub

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.rdoListinoInfinity = New System.Windows.Forms.RadioButton()
        Me.rdoListinoAS400 = New System.Windows.Forms.RadioButton()
        Me.lblListino = New System.Windows.Forms.Label()
        Me.btnAnalizza = New System.Windows.Forms.Button()
        Me.dtpAl = New System.Windows.Forms.DateTimePicker()
        Me.lblAl = New System.Windows.Forms.Label()
        Me.dtpDal = New System.Windows.Forms.DateTimePicker()
        Me.lblDal = New System.Windows.Forms.Label()
        Me.cmbFornitori = New System.Windows.Forms.ComboBox()
        Me.lblFornitore = New System.Windows.Forms.Label()
        Me.txtFiltroNumeroFattura = New System.Windows.Forms.TextBox()
        Me.lblFiltroNumeroFattura = New System.Windows.Forms.Label()
        Me.btnStampa = New System.Windows.Forms.Button()
        Me.dgvRisultati = New System.Windows.Forms.DataGridView()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.lblStatistiche = New System.Windows.Forms.ToolStripStatusLabel()
        Me.Panel1.SuspendLayout()
        CType(Me.dgvRisultati, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.rdoListinoInfinity)
        Me.Panel1.Controls.Add(Me.rdoListinoAS400)
        Me.Panel1.Controls.Add(Me.lblListino)
        Me.Panel1.Controls.Add(Me.btnAnalizza)
        Me.Panel1.Controls.Add(Me.dtpAl)
        Me.Panel1.Controls.Add(Me.lblAl)
        Me.Panel1.Controls.Add(Me.dtpDal)
        Me.Panel1.Controls.Add(Me.lblDal)
        Me.Panel1.Controls.Add(Me.cmbFornitori)
        Me.Panel1.Controls.Add(Me.lblFornitore)
        Me.Panel1.Controls.Add(Me.txtFiltroNumeroFattura)
        Me.Panel1.Controls.Add(Me.lblFiltroNumeroFattura)
        Me.Panel1.Controls.Add(Me.btnStampa)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(900, 118)
        Me.Panel1.TabIndex = 0
        '
        'lblListino
        '
        Me.lblListino.AutoSize = True
        Me.lblListino.Location = New System.Drawing.Point(700, 9)
        Me.lblListino.Name = "lblListino"
        Me.lblListino.Size = New System.Drawing.Size(93, 13)
        Me.lblListino.TabIndex = 6
        Me.lblListino.Text = "VERIFICA PREZZI DA"
        '
        'rdoListinoAS400
        '
        Me.rdoListinoAS400.AutoSize = True
        Me.rdoListinoAS400.Checked = True
        Me.rdoListinoAS400.Location = New System.Drawing.Point(700, 27)
        Me.rdoListinoAS400.Name = "rdoListinoAS400"
        Me.rdoListinoAS400.Size = New System.Drawing.Size(65, 17)
        Me.rdoListinoAS400.TabIndex = 7
        Me.rdoListinoAS400.TabStop = True
        Me.rdoListinoAS400.Text = "AS400"
        Me.rdoListinoAS400.UseVisualStyleBackColor = True
        '
        'rdoListinoInfinity
        '
        Me.rdoListinoInfinity.AutoSize = True
        Me.rdoListinoInfinity.Location = New System.Drawing.Point(700, 44)
        Me.rdoListinoInfinity.Name = "rdoListinoInfinity"
        Me.rdoListinoInfinity.Size = New System.Drawing.Size(70, 17)
        Me.rdoListinoInfinity.TabIndex = 8
        Me.rdoListinoInfinity.TabStop = True
        Me.rdoListinoInfinity.Text = "Infinity"
        Me.rdoListinoInfinity.UseVisualStyleBackColor = True
        '
        'btnAnalizza
        '
        Me.btnAnalizza.Location = New System.Drawing.Point(575, 18)
        Me.btnAnalizza.Name = "btnAnalizza"
        Me.btnAnalizza.Size = New System.Drawing.Size(120, 30)
        Me.btnAnalizza.TabIndex = 9
        Me.btnAnalizza.Text = "ANALIZZA"
        Me.btnAnalizza.UseVisualStyleBackColor = True
        '
        'dtpAl
        '
        Me.dtpAl.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpAl.Location = New System.Drawing.Point(440, 25)
        Me.dtpAl.Name = "dtpAl"
        Me.dtpAl.Size = New System.Drawing.Size(120, 20)
        Me.dtpAl.TabIndex = 5
        '
        'lblAl
        '
        Me.lblAl.AutoSize = True
        Me.lblAl.Location = New System.Drawing.Point(440, 9)
        Me.lblAl.Name = "lblAl"
        Me.lblAl.Size = New System.Drawing.Size(93, 13)
        Me.lblAl.TabIndex = 4
        Me.lblAl.Text = "BOLLA AL"
        '
        'dtpDal
        '
        Me.dtpDal.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpDal.Location = New System.Drawing.Point(310, 25)
        Me.dtpDal.Name = "dtpDal"
        Me.dtpDal.Size = New System.Drawing.Size(120, 20)
        Me.dtpDal.TabIndex = 3
        '
        'lblDal
        '
        Me.lblDal.AutoSize = True
        Me.lblDal.Location = New System.Drawing.Point(310, 9)
        Me.lblDal.Name = "lblDal"
        Me.lblDal.Size = New System.Drawing.Size(93, 13)
        Me.lblDal.TabIndex = 2
        Me.lblDal.Text = "BOLLA DAL"
        '
        'cmbFornitori
        '
        Me.cmbFornitori.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cmbFornitori.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cmbFornitori.FormattingEnabled = True
        Me.cmbFornitori.Location = New System.Drawing.Point(15, 25)
        Me.cmbFornitori.Name = "cmbFornitori"
        Me.cmbFornitori.Size = New System.Drawing.Size(280, 21)
        Me.cmbFornitori.TabIndex = 1
        '
        'lblFornitore
        '
        Me.lblFornitore.AutoSize = True
        Me.lblFornitore.Location = New System.Drawing.Point(15, 9)
        Me.lblFornitore.Name = "lblFornitore"
        Me.lblFornitore.Size = New System.Drawing.Size(70, 13)
        Me.lblFornitore.TabIndex = 0
        Me.lblFornitore.Text = "FORNITORE"
        '
        'txtFiltroNumeroFattura
        '
        Me.txtFiltroNumeroFattura.Location = New System.Drawing.Point(15, 84)
        Me.txtFiltroNumeroFattura.Name = "txtFiltroNumeroFattura"
        Me.txtFiltroNumeroFattura.Size = New System.Drawing.Size(280, 20)
        Me.txtFiltroNumeroFattura.TabIndex = 10
        '
        'lblFiltroNumeroFattura
        '
        Me.lblFiltroNumeroFattura.AutoSize = True
        Me.lblFiltroNumeroFattura.Location = New System.Drawing.Point(15, 68)
        Me.lblFiltroNumeroFattura.Name = "lblFiltroNumeroFattura"
        Me.lblFiltroNumeroFattura.Size = New System.Drawing.Size(93, 13)
        Me.lblFiltroNumeroFattura.TabIndex = 11
        Me.lblFiltroNumeroFattura.Text = "NUMERO FATTURA"
        '
        'btnStampa
        '
        Me.btnStampa.Location = New System.Drawing.Point(600, 86)
        Me.btnStampa.Name = "btnStampa"
        Me.btnStampa.Size = New System.Drawing.Size(120, 26)
        Me.btnStampa.TabIndex = 12
        Me.btnStampa.Text = "STAMPA"
        Me.btnStampa.UseVisualStyleBackColor = True
        '
        'dgvRisultati
        '
        Me.dgvRisultati.AllowUserToAddRows = False
        Me.dgvRisultati.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvRisultati.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvRisultati.Location = New System.Drawing.Point(0, 118)
        Me.dgvRisultati.Name = "dgvRisultati"
        Me.dgvRisultati.ReadOnly = True
        Me.dgvRisultati.Size = New System.Drawing.Size(900, 404)
        Me.dgvRisultati.TabIndex = 1
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatistiche})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 522)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(900, 22)
        Me.StatusStrip1.TabIndex = 2
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'lblStatistiche
        '
        Me.lblStatistiche.Name = "lblStatistiche"
        Me.lblStatistiche.Size = New System.Drawing.Size(54, 17)
        Me.lblStatistiche.Text = "PRONTO"
        '
        'frmPrezziAS400
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 544)
        Me.Controls.Add(Me.dgvRisultati)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Name = "frmPrezziAS400"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Analisi prezzi fatture AS400 vs listino"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.dgvRisultati, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblListino As Label
    Friend WithEvents rdoListinoAS400 As RadioButton
    Friend WithEvents rdoListinoInfinity As RadioButton
    Friend WithEvents btnAnalizza As Button
    Friend WithEvents dtpAl As DateTimePicker
    Friend WithEvents lblAl As Label
    Friend WithEvents dtpDal As DateTimePicker
    Friend WithEvents lblDal As Label
    Friend WithEvents cmbFornitori As ComboBox
    Friend WithEvents lblFornitore As Label
    Friend WithEvents txtFiltroNumeroFattura As TextBox
    Friend WithEvents lblFiltroNumeroFattura As Label
    Friend WithEvents btnStampa As Button
    Friend WithEvents dgvRisultati As DataGridView
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents lblStatistiche As ToolStripStatusLabel
End Class
