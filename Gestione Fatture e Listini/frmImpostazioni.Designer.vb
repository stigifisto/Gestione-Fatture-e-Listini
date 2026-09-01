<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmImpostazioni
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
        Me.lblZip = New System.Windows.Forms.Label()
        Me.txtCartellaZip = New System.Windows.Forms.TextBox()
        Me.btnSfogliaZip = New System.Windows.Forms.Button()
        Me.lblDecompressi = New System.Windows.Forms.Label()
        Me.txtCartellaDecompressi = New System.Windows.Forms.TextBox()
        Me.btnSfogliaDecompressi = New System.Windows.Forms.Button()
        Me.lblXmlPronti = New System.Windows.Forms.Label()
        Me.txtCartellaXmlPronti = New System.Windows.Forms.TextBox()
        Me.btnSfogliaXmlPronti = New System.Windows.Forms.Button()
        Me.lblBackup = New System.Windows.Forms.Label()
        Me.txtCartellaBackup = New System.Windows.Forms.TextBox()
        Me.btnSfogliaBackup = New System.Windows.Forms.Button()
        Me.lblDoppi = New System.Windows.Forms.Label()
        Me.txtCartellaDoppi = New System.Windows.Forms.TextBox()
        Me.btnSfogliaDoppi = New System.Windows.Forms.Button()
        Me.lblScostamento = New System.Windows.Forms.Label()
        Me.nudScostamento = New System.Windows.Forms.NumericUpDown()
        Me.btnSalva = New System.Windows.Forms.Button()
        Me.btnAnnulla = New System.Windows.Forms.Button()
        CType(Me.nudScostamento, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblZip
        '
        Me.lblZip.AutoSize = True
        Me.lblZip.Location = New System.Drawing.Point(12, 15)
        Me.lblZip.Name = "lblZip"
        Me.lblZip.Size = New System.Drawing.Size(283, 13)
        Me.lblZip.TabIndex = 0
        Me.lblZip.Text = "Cartella file ZIP scaricati dal Sistema di Interscambio:"
        '
        'txtCartellaZip
        '
        Me.txtCartellaZip.Location = New System.Drawing.Point(15, 31)
        Me.txtCartellaZip.Name = "txtCartellaZip"
        Me.txtCartellaZip.Size = New System.Drawing.Size(480, 20)
        Me.txtCartellaZip.TabIndex = 1
        '
        'btnSfogliaZip
        '
        Me.btnSfogliaZip.Location = New System.Drawing.Point(501, 29)
        Me.btnSfogliaZip.Name = "btnSfogliaZip"
        Me.btnSfogliaZip.Size = New System.Drawing.Size(75, 23)
        Me.btnSfogliaZip.TabIndex = 2
        Me.btnSfogliaZip.Text = "Sfoglia..."
        Me.btnSfogliaZip.UseVisualStyleBackColor = True
        '
        'lblDecompressi
        '
        Me.lblDecompressi.AutoSize = True
        Me.lblDecompressi.Location = New System.Drawing.Point(12, 64)
        Me.lblDecompressi.Name = "lblDecompressi"
        Me.lblDecompressi.Size = New System.Drawing.Size(312, 13)
        Me.lblDecompressi.TabIndex = 3
        Me.lblDecompressi.Text = "Cartella archivio file decompressi (ancora firmati, formato .p7m):"
        '
        'txtCartellaDecompressi
        '
        Me.txtCartellaDecompressi.Location = New System.Drawing.Point(15, 80)
        Me.txtCartellaDecompressi.Name = "txtCartellaDecompressi"
        Me.txtCartellaDecompressi.Size = New System.Drawing.Size(480, 20)
        Me.txtCartellaDecompressi.TabIndex = 4
        '
        'btnSfogliaDecompressi
        '
        Me.btnSfogliaDecompressi.Location = New System.Drawing.Point(501, 78)
        Me.btnSfogliaDecompressi.Name = "btnSfogliaDecompressi"
        Me.btnSfogliaDecompressi.Size = New System.Drawing.Size(75, 23)
        Me.btnSfogliaDecompressi.TabIndex = 5
        Me.btnSfogliaDecompressi.Text = "Sfoglia..."
        Me.btnSfogliaDecompressi.UseVisualStyleBackColor = True
        '
        'lblXmlPronti
        '
        Me.lblXmlPronti.AutoSize = True
        Me.lblXmlPronti.Location = New System.Drawing.Point(12, 113)
        Me.lblXmlPronti.Name = "lblXmlPronti"
        Me.lblXmlPronti.Size = New System.Drawing.Size(303, 13)
        Me.lblXmlPronti.TabIndex = 6
        Me.lblXmlPronti.Text = "Cartella archivio file XML senza firma, pronti per l'importazione:"
        '
        'txtCartellaXmlPronti
        '
        Me.txtCartellaXmlPronti.Location = New System.Drawing.Point(15, 129)
        Me.txtCartellaXmlPronti.Name = "txtCartellaXmlPronti"
        Me.txtCartellaXmlPronti.Size = New System.Drawing.Size(480, 20)
        Me.txtCartellaXmlPronti.TabIndex = 7
        '
        'btnSfogliaXmlPronti
        '
        Me.btnSfogliaXmlPronti.Location = New System.Drawing.Point(501, 127)
        Me.btnSfogliaXmlPronti.Name = "btnSfogliaXmlPronti"
        Me.btnSfogliaXmlPronti.Size = New System.Drawing.Size(75, 23)
        Me.btnSfogliaXmlPronti.TabIndex = 8
        Me.btnSfogliaXmlPronti.Text = "Sfoglia..."
        Me.btnSfogliaXmlPronti.UseVisualStyleBackColor = True
        '
        'lblBackup
        '
        Me.lblBackup.AutoSize = True
        Me.lblBackup.Location = New System.Drawing.Point(12, 162)
        Me.lblBackup.Name = "lblBackup"
        Me.lblBackup.Size = New System.Drawing.Size(231, 13)
        Me.lblBackup.TabIndex = 9
        Me.lblBackup.Text = "Cartella di backup delle fatture importate:"
        '
        'txtCartellaBackup
        '
        Me.txtCartellaBackup.Location = New System.Drawing.Point(15, 178)
        Me.txtCartellaBackup.Name = "txtCartellaBackup"
        Me.txtCartellaBackup.Size = New System.Drawing.Size(480, 20)
        Me.txtCartellaBackup.TabIndex = 10
        '
        'btnSfogliaBackup
        '
        Me.btnSfogliaBackup.Location = New System.Drawing.Point(501, 176)
        Me.btnSfogliaBackup.Name = "btnSfogliaBackup"
        Me.btnSfogliaBackup.Size = New System.Drawing.Size(75, 23)
        Me.btnSfogliaBackup.TabIndex = 11
        Me.btnSfogliaBackup.Text = "Sfoglia..."
        Me.btnSfogliaBackup.UseVisualStyleBackColor = True
        '
        'lblDoppi
        '
        Me.lblDoppi.AutoSize = True
        Me.lblDoppi.Location = New System.Drawing.Point(12, 211)
        Me.lblDoppi.Name = "lblDoppi"
        Me.lblDoppi.Size = New System.Drawing.Size(181, 13)
        Me.lblDoppi.TabIndex = 12
        Me.lblDoppi.Text = "Cartella archivio file doppi:"
        '
        'txtCartellaDoppi
        '
        Me.txtCartellaDoppi.Location = New System.Drawing.Point(15, 227)
        Me.txtCartellaDoppi.Name = "txtCartellaDoppi"
        Me.txtCartellaDoppi.Size = New System.Drawing.Size(480, 20)
        Me.txtCartellaDoppi.TabIndex = 13
        '
        'btnSfogliaDoppi
        '
        Me.btnSfogliaDoppi.Location = New System.Drawing.Point(501, 225)
        Me.btnSfogliaDoppi.Name = "btnSfogliaDoppi"
        Me.btnSfogliaDoppi.Size = New System.Drawing.Size(75, 23)
        Me.btnSfogliaDoppi.TabIndex = 14
        Me.btnSfogliaDoppi.Text = "Sfoglia..."
        Me.btnSfogliaDoppi.UseVisualStyleBackColor = True
        '
        'lblScostamento
        '
        Me.lblScostamento.AutoSize = True
        Me.lblScostamento.Location = New System.Drawing.Point(12, 260)
        Me.lblScostamento.Name = "lblScostamento"
        Me.lblScostamento.Size = New System.Drawing.Size(432, 13)
        Me.lblScostamento.TabIndex = 15
        Me.lblScostamento.Text = "Scostamento di prezzo accettabile (€) oltre il quale segnalare l'anomalia:"
        '
        'nudScostamento
        '
        Me.nudScostamento.DecimalPlaces = 2
        Me.nudScostamento.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.nudScostamento.Location = New System.Drawing.Point(15, 276)
        Me.nudScostamento.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        Me.nudScostamento.Name = "nudScostamento"
        Me.nudScostamento.Size = New System.Drawing.Size(120, 20)
        Me.nudScostamento.TabIndex = 16
        '
        'btnSalva
        '
        Me.btnSalva.Location = New System.Drawing.Point(420, 313)
        Me.btnSalva.Name = "btnSalva"
        Me.btnSalva.Size = New System.Drawing.Size(75, 30)
        Me.btnSalva.TabIndex = 17
        Me.btnSalva.Text = "Salva"
        Me.btnSalva.UseVisualStyleBackColor = True
        '
        'btnAnnulla
        '
        Me.btnAnnulla.Location = New System.Drawing.Point(501, 313)
        Me.btnAnnulla.Name = "btnAnnulla"
        Me.btnAnnulla.Size = New System.Drawing.Size(75, 30)
        Me.btnAnnulla.TabIndex = 18
        Me.btnAnnulla.Text = "Annulla"
        Me.btnAnnulla.UseVisualStyleBackColor = True
        '
        'frmImpostazioni
        '
        Me.AcceptButton = Me.btnSalva
        Me.CancelButton = Me.btnAnnulla
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(588, 355)
        Me.Controls.Add(Me.lblZip)
        Me.Controls.Add(Me.txtCartellaZip)
        Me.Controls.Add(Me.btnSfogliaZip)
        Me.Controls.Add(Me.lblDecompressi)
        Me.Controls.Add(Me.txtCartellaDecompressi)
        Me.Controls.Add(Me.btnSfogliaDecompressi)
        Me.Controls.Add(Me.lblXmlPronti)
        Me.Controls.Add(Me.txtCartellaXmlPronti)
        Me.Controls.Add(Me.btnSfogliaXmlPronti)
        Me.Controls.Add(Me.lblBackup)
        Me.Controls.Add(Me.txtCartellaBackup)
        Me.Controls.Add(Me.btnSfogliaBackup)
        Me.Controls.Add(Me.lblDoppi)
        Me.Controls.Add(Me.txtCartellaDoppi)
        Me.Controls.Add(Me.btnSfogliaDoppi)
        Me.Controls.Add(Me.lblScostamento)
        Me.Controls.Add(Me.nudScostamento)
        Me.Controls.Add(Me.btnSalva)
        Me.Controls.Add(Me.btnAnnulla)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmImpostazioni"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Impostazioni cartelle"
        CType(Me.nudScostamento, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblZip As Label
    Friend WithEvents txtCartellaZip As TextBox
    Friend WithEvents btnSfogliaZip As Button
    Friend WithEvents lblDecompressi As Label
    Friend WithEvents txtCartellaDecompressi As TextBox
    Friend WithEvents btnSfogliaDecompressi As Button
    Friend WithEvents lblXmlPronti As Label
    Friend WithEvents txtCartellaXmlPronti As TextBox
    Friend WithEvents btnSfogliaXmlPronti As Button
    Friend WithEvents lblBackup As Label
    Friend WithEvents txtCartellaBackup As TextBox
    Friend WithEvents btnSfogliaBackup As Button
    Friend WithEvents lblDoppi As Label
    Friend WithEvents txtCartellaDoppi As TextBox
    Friend WithEvents btnSfogliaDoppi As Button
    Friend WithEvents lblScostamento As Label
    Friend WithEvents nudScostamento As NumericUpDown
    Friend WithEvents btnSalva As Button
    Friend WithEvents btnAnnulla As Button
End Class
