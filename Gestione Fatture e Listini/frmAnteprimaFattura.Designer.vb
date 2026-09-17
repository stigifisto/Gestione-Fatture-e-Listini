<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAnteprimaFattura
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
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.rdoCompatto = New System.Windows.Forms.RadioButton()
        Me.rdoMinisteriale = New System.Windows.Forms.RadioButton()
        Me.lblFormato = New System.Windows.Forms.Label()
        Me.btnStampa = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'WebBrowser1
        '
        Me.WebBrowser1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebBrowser1.Location = New System.Drawing.Point(0, 36)
        Me.WebBrowser1.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser1.Name = "WebBrowser1"
        Me.WebBrowser1.ScriptErrorsSuppressed = True
        Me.WebBrowser1.Size = New System.Drawing.Size(1000, 764)
        Me.WebBrowser1.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnStampa)
        Me.Panel1.Controls.Add(Me.rdoCompatto)
        Me.Panel1.Controls.Add(Me.rdoMinisteriale)
        Me.Panel1.Controls.Add(Me.lblFormato)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1000, 36)
        Me.Panel1.TabIndex = 1
        '
        'lblFormato
        '
        Me.lblFormato.AutoSize = True
        Me.lblFormato.Location = New System.Drawing.Point(15, 11)
        Me.lblFormato.Name = "lblFormato"
        Me.lblFormato.Size = New System.Drawing.Size(50, 13)
        Me.lblFormato.TabIndex = 0
        Me.lblFormato.Text = "FORMATO"
        '
        'rdoMinisteriale
        '
        Me.rdoMinisteriale.AutoSize = True
        Me.rdoMinisteriale.Checked = True
        Me.rdoMinisteriale.Location = New System.Drawing.Point(90, 10)
        Me.rdoMinisteriale.Name = "rdoMinisteriale"
        Me.rdoMinisteriale.Size = New System.Drawing.Size(90, 17)
        Me.rdoMinisteriale.TabIndex = 1
        Me.rdoMinisteriale.TabStop = True
        Me.rdoMinisteriale.Text = "Ministeriale"
        Me.rdoMinisteriale.UseVisualStyleBackColor = True
        '
        'rdoCompatto
        '
        Me.rdoCompatto.AutoSize = True
        Me.rdoCompatto.Location = New System.Drawing.Point(190, 10)
        Me.rdoCompatto.Name = "rdoCompatto"
        Me.rdoCompatto.Size = New System.Drawing.Size(75, 17)
        Me.rdoCompatto.TabIndex = 2
        Me.rdoCompatto.Text = "Compatto"
        Me.rdoCompatto.UseVisualStyleBackColor = True
        '
        'btnStampa
        '
        Me.btnStampa.Location = New System.Drawing.Point(890, 4)
        Me.btnStampa.Name = "btnStampa"
        Me.btnStampa.Size = New System.Drawing.Size(95, 28)
        Me.btnStampa.TabIndex = 3
        Me.btnStampa.Text = "STAMPA"
        Me.btnStampa.UseVisualStyleBackColor = True
        '
        'frmAnteprimaFattura
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1000, 800)
        Me.Controls.Add(Me.WebBrowser1)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "frmAnteprimaFattura"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Anteprima fattura elettronica"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents WebBrowser1 As WebBrowser
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblFormato As Label
    Friend WithEvents rdoMinisteriale As RadioButton
    Friend WithEvents rdoCompatto As RadioButton
    Friend WithEvents btnStampa As Button
End Class
