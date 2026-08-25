Partial Class frmSceltaAnalisiPrezzi
    Inherits System.Windows.Forms.Form

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
        Me.cmdAnalisiAS400 = New System.Windows.Forms.Button()
        Me.cmdAnalisiFatture = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'cmdAnalisiAS400
        '
        Me.cmdAnalisiAS400.Location = New System.Drawing.Point(12, 12)
        Me.cmdAnalisiAS400.Name = "cmdAnalisiAS400"
        Me.cmdAnalisiAS400.Size = New System.Drawing.Size(320, 36)
        Me.cmdAnalisiAS400.TabIndex = 0
        Me.cmdAnalisiAS400.Text = "Analisi prezzi AS400"
        Me.cmdAnalisiAS400.UseVisualStyleBackColor = True
        '
        'cmdAnalisiFatture
        '
        Me.cmdAnalisiFatture.Location = New System.Drawing.Point(12, 58)
        Me.cmdAnalisiFatture.Name = "cmdAnalisiFatture"
        Me.cmdAnalisiFatture.Size = New System.Drawing.Size(320, 36)
        Me.cmdAnalisiFatture.TabIndex = 1
        Me.cmdAnalisiFatture.Text = "Analisi prezzi Fatture Elettroniche"
        Me.cmdAnalisiFatture.UseVisualStyleBackColor = True
        '
        'frmSceltaAnalisiPrezzi
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(344, 106)
        Me.Controls.Add(Me.cmdAnalisiFatture)
        Me.Controls.Add(Me.cmdAnalisiAS400)
        Me.MaximizeBox = False
        Me.Name = "frmSceltaAnalisiPrezzi"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Analisi prezzi fatture"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents cmdAnalisiAS400 As Button
    Friend WithEvents cmdAnalisiFatture As Button
End Class
