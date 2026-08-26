Partial Class frmConsolidamentoDati
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Public Sub New()
        MyBase.New()

        'Chiamata richiesta dal progettista.
        InitializeComponent()
    End Sub

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
        Me.components = New System.ComponentModel.Container()
        Me.cmsImportaListini = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuImportaEntrambi = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuImportaAS400 = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuImportaInfinity = New System.Windows.Forms.ToolStripMenuItem()
        Me.lblListiniDal = New System.Windows.Forms.Label()
        Me.dtpListiniDal = New System.Windows.Forms.DateTimePicker()
        Me.cmdImportaListini = New System.Windows.Forms.Button()
        Me.lblFattureDal = New System.Windows.Forms.Label()
        Me.dtpFattureAS400Dal = New System.Windows.Forms.DateTimePicker()
        Me.cmdImportaFattureAS400 = New System.Windows.Forms.Button()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.cmsImportaListini.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmsImportaListini
        '
        Me.cmsImportaListini.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuImportaEntrambi, Me.mnuImportaAS400, Me.mnuImportaInfinity})
        Me.cmsImportaListini.Name = "cmsImportaListini"
        Me.cmsImportaListini.Size = New System.Drawing.Size(180, 70)
        '
        'mnuImportaEntrambi
        '
        Me.mnuImportaEntrambi.Name = "mnuImportaEntrambi"
        Me.mnuImportaEntrambi.Size = New System.Drawing.Size(179, 22)
        Me.mnuImportaEntrambi.Text = "Importa entrambi"
        '
        'mnuImportaAS400
        '
        Me.mnuImportaAS400.Name = "mnuImportaAS400"
        Me.mnuImportaAS400.Size = New System.Drawing.Size(179, 22)
        Me.mnuImportaAS400.Text = "Solo AS400"
        '
        'mnuImportaInfinity
        '
        Me.mnuImportaInfinity.Name = "mnuImportaInfinity"
        Me.mnuImportaInfinity.Size = New System.Drawing.Size(179, 22)
        Me.mnuImportaInfinity.Text = "Solo Infinity"
        '
        'lblListiniDal
        '
        Me.lblListiniDal.AutoSize = True
        Me.lblListiniDal.Location = New System.Drawing.Point(12, 21)
        Me.lblListiniDal.Name = "lblListiniDal"
        Me.lblListiniDal.Size = New System.Drawing.Size(163, 13)
        Me.lblListiniDal.TabIndex = 0
        Me.lblListiniDal.Text = "Data inizio validità listini dal:"
        '
        'dtpListiniDal
        '
        Me.dtpListiniDal.Format = System.Windows.Forms.DateTimePickerFormat.Short
        Me.dtpListiniDal.Location = New System.Drawing.Point(220, 17)
        Me.dtpListiniDal.Name = "dtpListiniDal"
        Me.dtpListiniDal.Size = New System.Drawing.Size(150, 20)
        Me.dtpListiniDal.TabIndex = 1
        '
        'cmdImportaListini
        '
        Me.cmdImportaListini.Location = New System.Drawing.Point(380, 15)
        Me.cmdImportaListini.Name = "cmdImportaListini"
        Me.cmdImportaListini.Size = New System.Drawing.Size(150, 26)
        Me.cmdImportaListini.TabIndex = 2
        Me.cmdImportaListini.Text = "IMPORTA LISTINI ▼"
        Me.cmdImportaListini.UseVisualStyleBackColor = True
        '
        'lblFattureDal
        '
        Me.lblFattureDal.AutoSize = True
        Me.lblFattureDal.Location = New System.Drawing.Point(12, 61)
        Me.lblFattureDal.Name = "lblFattureDal"
        Me.lblFattureDal.Size = New System.Drawing.Size(163, 13)
        Me.lblFattureDal.TabIndex = 3
        Me.lblFattureDal.Text = "Data inizio fatture AS400 dal:"
        '
        'dtpFattureAS400Dal
        '
        Me.dtpFattureAS400Dal.Format = System.Windows.Forms.DateTimePickerFormat.Short
        Me.dtpFattureAS400Dal.Location = New System.Drawing.Point(220, 57)
        Me.dtpFattureAS400Dal.Name = "dtpFattureAS400Dal"
        Me.dtpFattureAS400Dal.Size = New System.Drawing.Size(150, 20)
        Me.dtpFattureAS400Dal.TabIndex = 4
        '
        'cmdImportaFattureAS400
        '
        Me.cmdImportaFattureAS400.Location = New System.Drawing.Point(380, 55)
        Me.cmdImportaFattureAS400.Name = "cmdImportaFattureAS400"
        Me.cmdImportaFattureAS400.Size = New System.Drawing.Size(150, 26)
        Me.cmdImportaFattureAS400.TabIndex = 5
        Me.cmdImportaFattureAS400.Text = "IMPORTA FATTURE AS400"
        Me.cmdImportaFattureAS400.UseVisualStyleBackColor = True
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(12, 111)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(518, 20)
        Me.ProgressBar1.TabIndex = 6
        Me.ProgressBar1.Visible = False
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(12, 138)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 13)
        Me.lblStatus.TabIndex = 7
        '
        'frmConsolidamentoDati
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(542, 166)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.cmdImportaFattureAS400)
        Me.Controls.Add(Me.dtpFattureAS400Dal)
        Me.Controls.Add(Me.lblFattureDal)
        Me.Controls.Add(Me.cmdImportaListini)
        Me.Controls.Add(Me.dtpListiniDal)
        Me.Controls.Add(Me.lblListiniDal)
        Me.MaximizeBox = False
        Me.Name = "frmConsolidamentoDati"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Consolidamento dati"
        Me.cmsImportaListini.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cmsImportaListini As ContextMenuStrip
    Friend WithEvents mnuImportaEntrambi As ToolStripMenuItem
    Friend WithEvents mnuImportaAS400 As ToolStripMenuItem
    Friend WithEvents mnuImportaInfinity As ToolStripMenuItem
    Friend WithEvents lblListiniDal As Label
    Friend WithEvents dtpListiniDal As DateTimePicker
    Friend WithEvents cmdImportaListini As Button
    Friend WithEvents lblFattureDal As Label
    Friend WithEvents dtpFattureAS400Dal As DateTimePicker
    Friend WithEvents cmdImportaFattureAS400 As Button
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents lblStatus As Label
End Class
