<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmVisualizzaFatturaElettronica
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
        Me.btnVisualizza = New System.Windows.Forms.Button()
        Me.dtpAl = New System.Windows.Forms.DateTimePicker()
        Me.lblAl = New System.Windows.Forms.Label()
        Me.dtpDal = New System.Windows.Forms.DateTimePicker()
        Me.lblDal = New System.Windows.Forms.Label()
        Me.cmbFornitori = New System.Windows.Forms.ComboBox()
        Me.lblFornitore = New System.Windows.Forms.Label()
        Me.dgvFatture = New System.Windows.Forms.DataGridView()
        Me.Panel1.SuspendLayout()
        CType(Me.dgvFatture, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnVisualizza)
        Me.Panel1.Controls.Add(Me.dtpAl)
        Me.Panel1.Controls.Add(Me.lblAl)
        Me.Panel1.Controls.Add(Me.dtpDal)
        Me.Panel1.Controls.Add(Me.lblDal)
        Me.Panel1.Controls.Add(Me.cmbFornitori)
        Me.Panel1.Controls.Add(Me.lblFornitore)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(900, 60)
        Me.Panel1.TabIndex = 0
        '
        'btnVisualizza
        '
        Me.btnVisualizza.Location = New System.Drawing.Point(575, 18)
        Me.btnVisualizza.Name = "btnVisualizza"
        Me.btnVisualizza.Size = New System.Drawing.Size(200, 30)
        Me.btnVisualizza.TabIndex = 6
        Me.btnVisualizza.Text = "VISUALIZZA FATTURA"
        Me.btnVisualizza.UseVisualStyleBackColor = True
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
        Me.lblAl.Text = "DATA FATTURA AL"
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
        Me.lblDal.Text = "DATA FATTURA DAL"
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
        'dgvFatture
        '
        Me.dgvFatture.AllowUserToAddRows = False
        Me.dgvFatture.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvFatture.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvFatture.Location = New System.Drawing.Point(0, 60)
        Me.dgvFatture.MultiSelect = False
        Me.dgvFatture.Name = "dgvFatture"
        Me.dgvFatture.ReadOnly = True
        Me.dgvFatture.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvFatture.Size = New System.Drawing.Size(900, 440)
        Me.dgvFatture.TabIndex = 1
        '
        'frmVisualizzaFatturaElettronica
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 500)
        Me.Controls.Add(Me.dgvFatture)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "frmVisualizzaFatturaElettronica"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Visualizza fattura elettronica"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.dgvFatture, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblFornitore As Label
    Friend WithEvents cmbFornitori As ComboBox
    Friend WithEvents lblDal As Label
    Friend WithEvents dtpDal As DateTimePicker
    Friend WithEvents lblAl As Label
    Friend WithEvents dtpAl As DateTimePicker
    Friend WithEvents btnVisualizza As Button
    Friend WithEvents dgvFatture As DataGridView
End Class
