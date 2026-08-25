Partial Class frmImportFattureElettroniche
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.cmdDecomprimiZip = New System.Windows.Forms.Button()
        Me.cmdRimuoviFirma = New System.Windows.Forms.Button()
        Me.cmdImportaXml = New System.Windows.Forms.Button()
        Me.cmdPipelineCompleta = New System.Windows.Forms.Button()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'cmdDecomprimiZip
        '
        Me.cmdDecomprimiZip.Location = New System.Drawing.Point(12, 12)
        Me.cmdDecomprimiZip.Name = "cmdDecomprimiZip"
        Me.cmdDecomprimiZip.Size = New System.Drawing.Size(390, 36)
        Me.cmdDecomprimiZip.TabIndex = 0
        Me.cmdDecomprimiZip.Text = "1. Decomprimi ZIP"
        Me.cmdDecomprimiZip.UseVisualStyleBackColor = True
        '
        'cmdRimuoviFirma
        '
        Me.cmdRimuoviFirma.Location = New System.Drawing.Point(12, 58)
        Me.cmdRimuoviFirma.Name = "cmdRimuoviFirma"
        Me.cmdRimuoviFirma.Size = New System.Drawing.Size(390, 36)
        Me.cmdRimuoviFirma.TabIndex = 1
        Me.cmdRimuoviFirma.Text = "2. Rimuovi firma digitale"
        Me.cmdRimuoviFirma.UseVisualStyleBackColor = True
        '
        'cmdImportaXml
        '
        Me.cmdImportaXml.Location = New System.Drawing.Point(12, 104)
        Me.cmdImportaXml.Name = "cmdImportaXml"
        Me.cmdImportaXml.Size = New System.Drawing.Size(390, 36)
        Me.cmdImportaXml.TabIndex = 2
        Me.cmdImportaXml.Text = "3. Importa fatture da XML"
        Me.cmdImportaXml.UseVisualStyleBackColor = True
        '
        'cmdPipelineCompleta
        '
        Me.cmdPipelineCompleta.Location = New System.Drawing.Point(12, 150)
        Me.cmdPipelineCompleta.Name = "cmdPipelineCompleta"
        Me.cmdPipelineCompleta.Size = New System.Drawing.Size(390, 36)
        Me.cmdPipelineCompleta.TabIndex = 3
        Me.cmdPipelineCompleta.Text = "Esegui pipeline completa (1+2+3)"
        Me.cmdPipelineCompleta.UseVisualStyleBackColor = True
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(12, 200)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(390, 20)
        Me.ProgressBar1.TabIndex = 4
        Me.ProgressBar1.Visible = False
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(12, 227)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 13)
        Me.lblStatus.TabIndex = 5
        '
        'frmImportFattureElettroniche
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(414, 255)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.cmdPipelineCompleta)
        Me.Controls.Add(Me.cmdImportaXml)
        Me.Controls.Add(Me.cmdRimuoviFirma)
        Me.Controls.Add(Me.cmdDecomprimiZip)
        Me.MaximizeBox = False
        Me.Name = "frmImportFattureElettroniche"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Importazione fatture elettroniche"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cmdDecomprimiZip As Button
    Friend WithEvents cmdRimuoviFirma As Button
    Friend WithEvents cmdImportaXml As Button
    Friend WithEvents cmdPipelineCompleta As Button
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents lblStatus As Label
End Class
