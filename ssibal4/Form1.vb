Imports System.Threading
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Public Class Form1
    Shared client As UdpClient
    Shared receivePoint As IPEndPoint
    Dim RX_Data As String
    Dim Button2_LEFT As Integer
    Dim PICTUREBOX1_LEFT As Integer
    Dim PICTUREBOX1_TOP As Integer

    Dim My_Score As Integer
    Dim Ball_X_Direction, Ball_Y_Direction As String

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Width = 800
        Me.Height = 600
        Me.FormBorderStyle = FormBorderStyle.FixedSingle

        receivePoint = New IPEndPoint(New IPAddress(0), 0)
        client = New UdpClient(5000)
        Dim thread As Thread = New Thread(New ThreadStart(AddressOf WaitForPackets))
        thread.Start()

        CheckBox1.Text = "호스트"
        My_Score = 0
        PictureBox1.Size = New System.Drawing.Size(50, 50)
        PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
        Ball_X_Direction = "-"
        Ball_Y_Direction = "-"
        PictureBox1.Left = (Me.Width / 2) - (PictureBox1.Width / 2)
        PictureBox1.Top = (Me.Height / 2) - (PictureBox1.Height / 2)
        Button1.Text = ""
        Button1.Width = 100
        Button1.Height = 10
        Button1.Top = Me.Height - 100
        Button2.Text = ""
        Button2.Width = 100
        Button2.Height = 10
        Button2.Top = 50

        Timer1.Interval = 10
        Timer1.Enabled = True
    End Sub

    Private Sub Ball_Next_Direction(ByVal wall As String, X As String, Y As String)
        If wall = "Left" Then
            If X = "-" And Y = "-" Then
                Ball_X_Direction = "+"
                Ball_Y_Direction = "-"
            End If
            If X = "-" And Y = "+" Then
                Ball_X_Direction = "+"
                Ball_Y_Direction = "+"
            End If
        End If
        If wall = "Top" Then
            If X = "-" And Y = "-" Then
                Ball_X_Direction = "-"
                Ball_Y_Direction = "+"
            End If
            If X = "+" And Y = "-" Then
                Ball_X_Direction = "+"
                Ball_Y_Direction = "+"
            End If
        End If
        If wall = "Right" Then
            If X = "+" And Y = "-" Then
                Ball_X_Direction = "-"
                Ball_Y_Direction = "-"
            End If
            If X = "+" And Y = "+" Then
                Ball_X_Direction = "-"
                Ball_Y_Direction = "+"
            End If
        End If
        If wall = "Bottom" Then
            If X = "+" And Y = "+" Then
                Ball_X_Direction = "+"
                Ball_Y_Direction = "-"
            End If
            If X = "-" And Y = "+" Then
                Ball_X_Direction = "-"
                Ball_Y_Direction = "-"
            End If
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Ball_X_Direction = "-" And Ball_Y_Direction = "-" Then
            PictureBox1.Left = PictureBox1.Left - 5
            PictureBox1.Top = PictureBox1.Top - 5
        End If
        If Ball_X_Direction = "-" And Ball_Y_Direction = "+" Then
            PictureBox1.Left = PictureBox1.Left - 5
            PictureBox1.Top = PictureBox1.Top + 5
        End If
        If Ball_X_Direction = "+" And Ball_Y_Direction = "-" Then
            PictureBox1.Left = PictureBox1.Left + 5
            PictureBox1.Top = PictureBox1.Top - 5
        End If
        If Ball_X_Direction = "+" And Ball_Y_Direction = "+" Then
            PictureBox1.Left = PictureBox1.Left + 5
            PictureBox1.Top = PictureBox1.Top + 5
        End If

        If PictureBox1.Left < 0 Then Ball_Next_Direction("Left", Ball_X_Direction, Ball_Y_Direction)
        If PictureBox1.Top < 0 Then Ball_Next_Direction("Top", Ball_X_Direction, Ball_Y_Direction)
        If (PictureBox1.Left + PictureBox1.Width) > Me.Width Then Ball_Next_Direction("Right", Ball_X_Direction, Ball_Y_Direction)
        If (PictureBox1.Top + PictureBox1.Height) > Me.Height Then Ball_Next_Direction("Bottom", Ball_X_Direction, Ball_Y_Direction)
        If (PictureBox1.Left > Button1.Left And PictureBox1.Left < (Button1.Left + Button1.Width)) And (PictureBox1.Top + PictureBox1.Height > Button1.Top) Then
            Ball_Next_Direction("Top", Ball_X_Direction, Ball_Y_Direction)
        End If
        If (PictureBox1.Left > Button2.Left And PictureBox1.Left < (Button2.Left + Button2.Width)) And (PictureBox1.Top < (Button2.Top + Button2.Height)) Then
            Ball_Next_Direction("Top", Ball_X_Direction, Ball_Y_Direction)
        End If

        If CheckBox1.Checked = True Then
            Dim data As Byte() = Encoding.UTF7.GetBytes("B" & PictureBox1.Left.ToString())
            client.Send(data, data.Length, "127.0.0.1", 5000)

            data = Encoding.UTF7.GetBytes("Y" & PictureBox1.Top.ToString())
            client.Send(data, data.Length, "127.0.0.1", 5000)
        End If

        Button2.Left = Button2_LEFT
        PictureBox1.Left = PICTUREBOX1_LEFT
        PictureBox1.Top = PICTUREBOX1_TOP
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.A Then Button1.Left = Button1.Left - 5
        If e.KeyCode = Keys.D Then Button1.Left = Button1.Left + 5
        Dim data As Byte() = Encoding.UTF7.GetBytes("X" & Button1.Left.ToString())
        client.Send(data, data.Length, "127.0.0.1", 5000)
    End Sub

    Private Sub WaitForPackets()
        While True
            Dim data As Byte() = client.Receive(receivePoint)
            RX_Data = Encoding.UTF7.GetString(data)

            If RX_Data.Substring(0, 1) = "x" Then
                Button2_LEFT = Val(RX_Data.Substring(1))
            End If
            If RX_Data.Substring(0, 1) = "B" Then
                PICTUREBOX1_LEFT = Val(RX_Data.Substring(1))
            End If
            If RX_Data.Substring(0, 1) = "Y" Then
                PICTUREBOX1_TOP = Val(RX_Data.Substring(1))
            End If
        End While
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub
End Class
