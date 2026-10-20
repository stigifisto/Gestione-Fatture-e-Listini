<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFornitoriRecuperoDdtEdi
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
        Me.lblInfo = New System.Windows.Forms.Label()
        Me.dgvFornitori = New System.Windows.Forms.DataGridView()
        Me.colIdFiscaleIVA = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDescrizione = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnEliminaRiga = New System.Windows.Forms.Button()
        Me.btnSalva = New System.Windows.Forms.Button()
        Me.btnAnnulla = New System.Windows.Forms.Button()
        CType(Me.dgvFornitori, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblInfo
        '
        Me.lblInfo.Location = New System.Drawing.Point(12, 9)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(560, 40)
        Me.lblInfo.TabIndex = 0
        Me.lblInfo.Text = "Fornitori che non indicano mai, in fattura elettronica, il riferimento riga/DDT: in fase " &
            "di importazione si tenta di ricostruirlo incrociando codice articolo e quantità con i dati " &
            "ricevuti via canale EDI, invece di usare sempre la data fattura come riferimento per il listino."
        '
        'dgvFornitori
        '
        Me.dgvFornitori.AllowUserToDeleteRows = False
        Me.dgvFornitori.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvFornitori.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colIdFiscaleIVA, Me.colDescrizione})
        Me.dgvFornitori.Location = New System.Drawing.Point(12, 52)
        Me.dgvFornitori.Name = "dgvFornitori"
        Me.dgvFornitori.RowHeadersWidth = 24
        Me.dgvFornitori.Size = New System.Drawing.Size(560, 300)
        Me.dgvFornitori.TabIndex = 1
        '
        'colIdFiscaleIVA
        '
        Me.colIdFiscaleIVA.HeaderText = "Partita IVA / Codice Fiscale fornitore"
        Me.colIdFiscaleIVA.Name = "colIdFiscaleIVA"
        Me.colIdFiscaleIVA.Width = 200
        '
        'colDescrizione
        '
        Me.colDescrizione.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.colDescrizione.HeaderText = "Descrizione fornitore (facoltativa)"
        Me.colDescrizione.Name = "colDescrizione"
        '
        'btnEliminaRiga
        '
        Me.btnEliminaRiga.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnEliminaRiga.Location = New System.Drawing.Point(12, 358)
        Me.btnEliminaRiga.Name = "btnEliminaRiga"
        Me.btnEliminaRiga.Size = New System.Drawing.Size(140, 30)
        Me.btnEliminaRiga.TabIndex = 2
        Me.btnEliminaRiga.Text = "Elimina riga selezionata"
        Me.btnEliminaRiga.UseVisualStyleBackColor = True
        '
        'btnSalva
        '
        Me.btnSalva.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSalva.Location = New System.Drawing.Point(407, 358)
        Me.btnSalva.Name = "btnSalva"
        Me.btnSalva.Size = New System.Drawing.Size(75, 30)
        Me.btnSalva.TabIndex = 3
        Me.btnSalva.Text = "Salva"
        Me.btnSalva.UseVisualStyleBackColor = True
        '
        'btnAnnulla
        '
        Me.btnAnnulla.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAnnulla.Location = New System.Drawing.Point(497, 358)
        Me.btnAnnulla.Name = "btnAnnulla"
        Me.btnAnnulla.Size = New System.Drawing.Size(75, 30)
        Me.btnAnnulla.TabIndex = 4
        Me.btnAnnulla.Text = "Annulla"
        Me.btnAnnulla.UseVisualStyleBackColor = True
        '
        'frmFornitoriRecuperoDdtEdi
        '
        Me.AcceptButton = Me.btnSalva
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnAnnulla
        Me.ClientSize = New System.Drawing.Size(584, 400)
        Me.Controls.Add(Me.btnAnnulla)
        Me.Controls.Add(Me.btnSalva)
        Me.Controls.Add(Me.btnEliminaRiga)
        Me.Controls.Add(Me.dgvFornitori)
        Me.Controls.Add(Me.lblInfo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmFornitoriRecuperoDdtEdi"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Fornitori con recupero DDT da EDI"
        CType(Me.dgvFornitori, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lblInfo As Label
    Friend WithEvents dgvFornitori As DataGridView
    Friend WithEvents colIdFiscaleIVA As DataGridViewTextBoxColumn
    Friend WithEvents colDescrizione As DataGridViewTextBoxColumn
    Friend WithEvents btnEliminaRiga As Button
    Friend WithEvents btnSalva As Button
    Friend WithEvents btnAnnulla As Button
End Class
