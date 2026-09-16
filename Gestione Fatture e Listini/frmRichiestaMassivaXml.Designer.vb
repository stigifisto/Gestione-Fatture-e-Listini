<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRichiestaMassivaXml
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
        Me.lblDa = New System.Windows.Forms.Label()
        Me.dtpDa = New System.Windows.Forms.DateTimePicker()
        Me.lblA = New System.Windows.Forms.Label()
        Me.dtpA = New System.Windows.Forms.DateTimePicker()
        Me.btnGenera = New System.Windows.Forms.Button()
        Me.btnChiudi = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblDa
        '
        Me.lblDa.AutoSize = True
        Me.lblDa.Location = New System.Drawing.Point(12, 15)
        Me.lblDa.Name = "lblDa"
        Me.lblDa.Size = New System.Drawing.Size(93, 13)
        Me.lblDa.TabIndex = 0
        Me.lblDa.Text = "DATA RICEZIONE DAL"
        '
        'dtpDa
        '
        Me.dtpDa.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpDa.Location = New System.Drawing.Point(15, 31)
        Me.dtpDa.Name = "dtpDa"
        Me.dtpDa.Size = New System.Drawing.Size(150, 20)
        Me.dtpDa.TabIndex = 1
        '
        'lblA
        '
        Me.lblA.AutoSize = True
        Me.lblA.Location = New System.Drawing.Point(185, 15)
        Me.lblA.Name = "lblA"
        Me.lblA.Size = New System.Drawing.Size(93, 13)
        Me.lblA.TabIndex = 2
        Me.lblA.Text = "DATA RICEZIONE AL"
        '
        'dtpA
        '
        Me.dtpA.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpA.Location = New System.Drawing.Point(188, 31)
        Me.dtpA.Name = "dtpA"
        Me.dtpA.Size = New System.Drawing.Size(150, 20)
        Me.dtpA.TabIndex = 3
        '
        'btnGenera
        '
        Me.btnGenera.Location = New System.Drawing.Point(15, 70)
        Me.btnGenera.Name = "btnGenera"
        Me.btnGenera.Size = New System.Drawing.Size(150, 30)
        Me.btnGenera.TabIndex = 4
        Me.btnGenera.Text = "GENERA XML"
        Me.btnGenera.UseVisualStyleBackColor = True
        '
        'btnChiudi
        '
        Me.btnChiudi.Location = New System.Drawing.Point(188, 70)
        Me.btnChiudi.Name = "btnChiudi"
        Me.btnChiudi.Size = New System.Drawing.Size(150, 30)
        Me.btnChiudi.TabIndex = 5
        Me.btnChiudi.Text = "Chiudi"
        Me.btnChiudi.UseVisualStyleBackColor = True
        '
        'frmRichiestaMassivaXml
        '
        Me.AcceptButton = Me.btnGenera
        Me.CancelButton = Me.btnChiudi
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(363, 118)
        Me.Controls.Add(Me.lblDa)
        Me.Controls.Add(Me.dtpDa)
        Me.Controls.Add(Me.lblA)
        Me.Controls.Add(Me.dtpA)
        Me.Controls.Add(Me.btnGenera)
        Me.Controls.Add(Me.btnChiudi)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmRichiestaMassivaXml"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Richiesta massiva fatture elettroniche - Agenzia delle Entrate"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblDa As Label
    Friend WithEvents dtpDa As DateTimePicker
    Friend WithEvents lblA As Label
    Friend WithEvents dtpA As DateTimePicker
    Friend WithEvents btnGenera As Button
    Friend WithEvents btnChiudi As Button
End Class
