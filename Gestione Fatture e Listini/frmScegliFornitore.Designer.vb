<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmScegliFornitore
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
        Me.btnSeleziona = New System.Windows.Forms.Button()
        Me.btnAnnulla = New System.Windows.Forms.Button()
        CType(Me.dgvFornitori, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblInfo
        '
        Me.lblInfo.AutoSize = True
        Me.lblInfo.Location = New System.Drawing.Point(12, 9)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(300, 13)
        Me.lblInfo.TabIndex = 0
        Me.lblInfo.Text = "Nessun fornitore selezionato. Fornitori con fatture nel periodo:"
        '
        'dgvFornitori
        '
        Me.dgvFornitori.AllowUserToAddRows = False
        Me.dgvFornitori.AllowUserToDeleteRows = False
        Me.dgvFornitori.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvFornitori.Location = New System.Drawing.Point(12, 30)
        Me.dgvFornitori.MultiSelect = False
        Me.dgvFornitori.Name = "dgvFornitori"
        Me.dgvFornitori.ReadOnly = True
        Me.dgvFornitori.RowHeadersVisible = False
        Me.dgvFornitori.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvFornitori.Size = New System.Drawing.Size(460, 300)
        Me.dgvFornitori.TabIndex = 1
        '
        'btnSeleziona
        '
        Me.btnSeleziona.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSeleziona.Location = New System.Drawing.Point(297, 336)
        Me.btnSeleziona.Name = "btnSeleziona"
        Me.btnSeleziona.Size = New System.Drawing.Size(85, 30)
        Me.btnSeleziona.TabIndex = 2
        Me.btnSeleziona.Text = "SELEZIONA"
        Me.btnSeleziona.UseVisualStyleBackColor = True
        '
        'btnAnnulla
        '
        Me.btnAnnulla.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAnnulla.Location = New System.Drawing.Point(388, 336)
        Me.btnAnnulla.Name = "btnAnnulla"
        Me.btnAnnulla.Size = New System.Drawing.Size(85, 30)
        Me.btnAnnulla.TabIndex = 3
        Me.btnAnnulla.Text = "ANNULLA"
        Me.btnAnnulla.UseVisualStyleBackColor = True
        '
        'frmScegliFornitore
        '
        Me.AcceptButton = Me.btnSeleziona
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnAnnulla
        Me.ClientSize = New System.Drawing.Size(484, 378)
        Me.Controls.Add(Me.btnAnnulla)
        Me.Controls.Add(Me.btnSeleziona)
        Me.Controls.Add(Me.dgvFornitori)
        Me.Controls.Add(Me.lblInfo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmScegliFornitore"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Seleziona fornitore"
        CType(Me.dgvFornitori, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblInfo As Label
    Friend WithEvents dgvFornitori As DataGridView
    Friend WithEvents btnSeleziona As Button
    Friend WithEvents btnAnnulla As Button
End Class
