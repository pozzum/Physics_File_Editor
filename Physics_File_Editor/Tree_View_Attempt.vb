Imports System.IO    'Files
Imports System.Text ' Text Encoding
Public Class Tree_View_Attempt
    Dim active_file As String
    Dim Physics_Array As Byte()
    Public Class Muscle_Container
        Public Node_Name As String
        Public Starting_Number As Integer
        Public Parts As Integer
        Public Header_Length As Integer
        Public Total_Length As Integer
        Public Array As Byte()
        Public Offset As Long
    End Class
    Dim Containter_List As List(Of Muscle_Container)

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            active_file = OpenFileDialog1.FileName
            If File.Exists(active_file) Then
                OpenFile(active_file)
            End If
        End If
    End Sub
    Dim Total_Muscle As New Muscle_Container
    Sub OpenFile(ByVal Source As String)
        Containter_List = New List(Of Muscle_Container)
        ToolStripTextBox1.Text = Path.GetFileName(Source)
        TreeView1.Nodes.Clear()
        Dim fileLength As Long
        Using reader As New BinaryReader(File.Open(Source, FileMode.Open, FileAccess.Read))
            fileLength = reader.BaseStream.Length
            Physics_Array = reader.ReadBytes(fileLength)
        End Using
        Dim Active_Offset As Long = 0
        '--------------------
        'Adding the whole file first
        '--------------------
        Total_Muscle = New Muscle_Container
        Total_Muscle.Array = Physics_Array
        Total_Muscle.Starting_Number = BitConverter.ToInt32(Total_Muscle.Array, Active_Offset + &H0)
        Total_Muscle.Header_Length = BitConverter.ToInt32(Total_Muscle.Array, Active_Offset + &H4)
        Total_Muscle.Total_Length = BitConverter.ToInt32(Total_Muscle.Array, Active_Offset + &H8)
        Total_Muscle.Parts = BitConverter.ToInt32(Total_Muscle.Array, Active_Offset + &HC)
        Total_Muscle.Offset = 0
        Total_Muscle.Node_Name = "File Part- " & Total_Muscle.Starting_Number &
                                " Length- " & Hex(Total_Muscle.Total_Length) &
                                " Parts- " & Total_Muscle.Parts &
                                " Offset- " & Hex(Total_Muscle.Offset)
        TreeView1.Nodes.Add(Total_Muscle.Node_Name)
        Containter_List.Add(Total_Muscle)
        AddSubParts(TreeView1.Nodes(0), Total_Muscle)
        'Dim Total_Node() As TreeNode
        'Total_Node = TreeView1.Nodes.Find(Total_Muscle.Node_Name, True)
        'Active_Offset += Total_Muscle.Header_Length
        'For i As Integer = 0 To Total_Muscle.Parts - 1
        'Dim temp_muscle As New Muscle_Container
        'Try
        '
        'temp_muscle.Starting_Number = BitConverter.ToInt32(Total_Muscle.Array, Active_Offset + &H0)
        'temp_muscle.Header_Length = BitConverter.ToInt32(Total_Muscle.Array, Active_Offset + &H4)
        'temp_muscle.Total_Length = BitConverter.ToInt32(Total_Muscle.Array, Active_Offset + &H8)
        'temp_muscle.Parts = BitConverter.ToInt32(Total_Muscle.Array, Active_Offset + &HC)
        'temp_muscle.Node_Name = "File Part: " & temp_muscle.Starting_Number &
        '               " Length: " & Hex(temp_muscle.Total_Length) &
        '              " Parts: " & temp_muscle.Parts
        'temp_muscle.Array = New Byte(temp_muscle.Total_Length - 1) {}
        'Buffer.BlockCopy(Total_Muscle.Array, Active_Offset, temp_muscle.Array, 0, temp_muscle.Total_Length)
        'TreeView1.Nodes(0).Nodes.Add(temp_muscle.Node_Name)
        'Active_Offset += temp_muscle.Total_Length
        'Catch ex As Exception
        'MessageBox.Show(i & vbNewLine &
        'temp_muscle.Node_Name & vbNewLine & ex.Message)
        'End Try
        'Next
        FileToolStripMenuItem1.Visible = True
    End Sub
    Sub AddSubParts(Parent_Node As TreeNode, Parent_Muscle As Muscle_Container)
        Dim Active_Offset As Integer = Parent_Muscle.Header_Length
        For i As Integer = 0 To Parent_Muscle.Parts - 1
            Dim temp_muscle As New Muscle_Container
            Try

                temp_muscle.Starting_Number = BitConverter.ToInt32(Parent_Muscle.Array, Active_Offset + &H0)
                temp_muscle.Header_Length = BitConverter.ToInt32(Parent_Muscle.Array, Active_Offset + &H4)
                temp_muscle.Total_Length = BitConverter.ToInt32(Parent_Muscle.Array, Active_Offset + &H8)
                temp_muscle.Parts = BitConverter.ToInt32(Parent_Muscle.Array, Active_Offset + &HC)
                temp_muscle.Offset = Parent_Muscle.Offset + Active_Offset
                temp_muscle.Node_Name = "File Part- " & temp_muscle.Starting_Number &
                        " Length- " & Hex(temp_muscle.Total_Length) &
                        " Parts- " & temp_muscle.Parts &
                        " Offset- " & Hex(temp_muscle.Offset)
                temp_muscle.Array = New Byte(temp_muscle.Total_Length - 1) {}
                Buffer.BlockCopy(Parent_Muscle.Array, Active_Offset, temp_muscle.Array, 0, temp_muscle.Total_Length)
                Parent_Node.Nodes.Add(temp_muscle.Node_Name)
                Containter_List.Add(temp_muscle)
                If temp_muscle.Starting_Number = 1 OrElse 'only adding known expandable parts
                    temp_muscle.Starting_Number = 2 OrElse
                    temp_muscle.Starting_Number = 5 OrElse
                    temp_muscle.Starting_Number = 6 OrElse
                    temp_muscle.Starting_Number = 9 OrElse
                    temp_muscle.Starting_Number = 19 OrElse
                    temp_muscle.Starting_Number = 23 OrElse
                    temp_muscle.Starting_Number = 25 OrElse
                    temp_muscle.Starting_Number = 27 OrElse
                    temp_muscle.Starting_Number = 31 Then
                    AddSubParts(Parent_Node.Nodes(i), temp_muscle)
                End If
                Active_Offset += temp_muscle.Total_Length
            Catch ex As Exception
                TreeView1.ExpandAll()
                MessageBox.Show(i & vbNewLine &
                                 temp_muscle.Node_Name & vbNewLine & ex.Message)
            End Try
        Next
    End Sub

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect
        For Each temp_muscle As Muscle_Container In Containter_List
            If e.Node.Text = temp_muscle.Node_Name Then
                ToolStripTextBox2.Text = temp_muscle.Node_Name
                If temp_muscle.Total_Length < &H1000 Then
                    AddText(BitConverter.ToString(temp_muscle.Array).Replace("-", " "))
                Else
                    AddText(BitConverter.ToString(temp_muscle.Array, 0, &H1000).Replace("-", " "))
                End If
            End If
        Next
    End Sub

    Private Sub Tree_View_Attempt_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ToolStripComboBox1.SelectedIndex() = 2
    End Sub
    Sub AddText(ByteArray As String)
        Dim bitwidth As Integer = CInt(ToolStripComboBox1.Items(ToolStripComboBox1.SelectedIndex))
        ByteArray = ByteArray.Replace(vbCr, "").Replace(vbLf, "")
        Dim builder As New StringBuilder(ByteArray)
        Dim startIndex = builder.Length - (builder.Length Mod bitwidth * 3)

        For i As Int32 = startIndex To (bitwidth * 3) Step -(bitwidth * 3)
            builder.Insert(i, vbCr & vbLf)
        Next i
        ByteArray = builder.ToString()
        Text_Selected.Text = ByteArray
    End Sub
    Private Sub ToolStripComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox1.SelectedIndexChanged
        AddText(Text_Selected.Text)
    End Sub

    Private Sub ExtractToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExtractToolStripMenuItem.Click
        If ToolStripTextBox2.Text = "" Then
            MessageBox.Show("Please select Container")
        Else
            SaveFileDialog1.FileName = ToolStripTextBox1.Text & "-" & ToolStripTextBox2.Text & ".dat"
            If SaveFileDialog1.ShowDialog = DialogResult.OK Then
                For Each temp_muscle As Muscle_Container In Containter_List
                    If ToolStripTextBox2.Text = temp_muscle.Node_Name Then
                        File.WriteAllBytes(SaveFileDialog1.FileName, temp_muscle.Array)
                        MessageBox.Show("File Saved")
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub ExpandAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExpandAllToolStripMenuItem.Click
        TreeView1.ExpandAll()
    End Sub

    Private Sub CollapseAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CollapseAllToolStripMenuItem.Click
        TreeView1.CollapseAll()
    End Sub

    Private Sub ReplaceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReplaceToolStripMenuItem.Click
        If OpenFileDialog2.ShowDialog = DialogResult.OK Then
            Dim New_File As String = OpenFileDialog2.FileName
            Dim New_Array As Byte()
            If File.Exists(New_File) Then
                Dim fileLength As Long
                Using reader As New BinaryReader(File.Open(New_File, FileMode.Open, FileAccess.Read))
                    fileLength = reader.BaseStream.Length
                    New_Array = reader.ReadBytes(fileLength)
                End Using
                'Now we have the new file, we need to check it
                Dim Active_Offset As Integer = 0
                Dim New_Muscle As New Muscle_Container
                New_Muscle.Array = New_Array
                New_Muscle.Starting_Number = BitConverter.ToInt32(New_Muscle.Array, Active_Offset + &H0)
                New_Muscle.Header_Length = BitConverter.ToInt32(New_Muscle.Array, Active_Offset + &H4)
                New_Muscle.Total_Length = BitConverter.ToInt32(New_Muscle.Array, Active_Offset + &H8)
                New_Muscle.Parts = BitConverter.ToInt32(New_Muscle.Array, Active_Offset + &HC)
                New_Muscle.Offset = 0
                New_Muscle.Node_Name = "File Part- " & New_Muscle.Starting_Number &
                                " Length- " & Hex(New_Muscle.Total_Length) &
                                " Parts- " & New_Muscle.Parts &
                                " Offset- " & Hex(New_Muscle.Offset)
                For Each temp_muscle As Muscle_Container In Containter_List
                    If ToolStripTextBox2.Text = temp_muscle.Node_Name Then
                        If temp_muscle.Starting_Number = New_Muscle.Starting_Number Then
                            MessageBox.Show("Matching Part Number")
                            Dim difference As Integer = New_Muscle.Total_Length - temp_muscle.Total_Length  'Shorter will be negative
                            Dim final_array As Byte() = New Byte(Total_Muscle.Total_Length + difference) {}
                            Buffer.BlockCopy(Total_Muscle.Array, 0, final_array, 0, Total_Muscle.Header_Length)
                            Buffer.BlockCopy(BitConverter.GetBytes(CInt(Total_Muscle.Total_Length + difference)), 0, final_array, 8, 4)
                            Dim New_File_Added As Boolean = False
                            For i As Integer = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                                For Each Adding_Container As Muscle_Container In Containter_List
                                    If Adding_Container.Node_Name = TreeView1.Nodes(0).Nodes(i).Text Then
                                        If Not Adding_Container.Node_Name = temp_muscle.Node_Name Then
                                            If New_File_Added Then
                                                Buffer.BlockCopy(Adding_Container.Array, 0,
                                                            final_array, Adding_Container.Offset + difference,
                                                            Adding_Container.Total_Length)
                                            Else
                                                Buffer.BlockCopy(Adding_Container.Array, 0,
                                                            final_array, Adding_Container.Offset,
                                                            Adding_Container.Total_Length)
                                            End If
                                        Else
                                            Buffer.BlockCopy(New_Muscle.Array, 0,
                                                            final_array, Adding_Container.Offset,
                                                            New_Muscle.Total_Length)
                                            New_File_Added = True
                                        End If
                                    End If
                                Next
                            Next
                            File.Copy(active_file, active_file & ".bak", True)
                            File.WriteAllBytes(active_file, final_array)
                            MessageBox.Show("File Saved")
                            OpenFile(active_file)
                        Else
                            MessageBox.Show("Different Part Number")
                            Exit Sub
                        End If
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        For Each temp_muscle As Muscle_Container In Containter_List
            If ToolStripTextBox2.Text = temp_muscle.Node_Name Then
                Dim difference As Integer = -temp_muscle.Total_Length  'Shorter will be negative
                Dim final_array As Byte() = New Byte(Total_Muscle.Total_Length + difference) {}
                Buffer.BlockCopy(Total_Muscle.Array, 0, final_array, 0, Total_Muscle.Header_Length)
                Buffer.BlockCopy(BitConverter.GetBytes(CInt(Total_Muscle.Total_Length + difference)), 0, final_array, 8, 4)
                Buffer.BlockCopy(BitConverter.GetBytes(CInt(Total_Muscle.Parts - 1)), 0, final_array, &HC, 4)
                Dim File_Deleted As Boolean = False
                For i As Integer = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                    For Each Adding_Container As Muscle_Container In Containter_List
                        If Adding_Container.Node_Name = TreeView1.Nodes(0).Nodes(i).Text Then
                            If Not Adding_Container.Node_Name = temp_muscle.Node_Name Then
                                If File_Deleted Then
                                    Buffer.BlockCopy(Adding_Container.Array, 0,
                                                final_array, Adding_Container.Offset + difference,
                                                Adding_Container.Total_Length)
                                Else
                                    Buffer.BlockCopy(Adding_Container.Array, 0,
                                                final_array, Adding_Container.Offset,
                                                Adding_Container.Total_Length)
                                End If
                            Else
                                File_Deleted = True
                            End If
                        End If
                    Next
                Next
                File.Copy(active_file, active_file & ".bak", True)
                File.WriteAllBytes(active_file, final_array)
                MessageBox.Show("File Saved")
                OpenFile(active_file)
            End If
        Next
    End Sub

    Private Sub AddToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddToolStripMenuItem.Click
        If OpenFileDialog2.ShowDialog = DialogResult.OK Then
            Dim New_File As String = OpenFileDialog2.FileName
            Dim New_Array As Byte()
            If File.Exists(New_File) Then
                Dim fileLength As Long
                Using reader As New BinaryReader(File.Open(New_File, FileMode.Open, FileAccess.Read))
                    fileLength = reader.BaseStream.Length
                    New_Array = reader.ReadBytes(fileLength)
                End Using
                'Now we have the new file, we need to check it
                Dim Active_Offset As Integer = 0
                Dim New_Muscle As New Muscle_Container
                New_Muscle.Array = New_Array
                New_Muscle.Starting_Number = BitConverter.ToInt32(New_Muscle.Array, Active_Offset + &H0)
                New_Muscle.Header_Length = BitConverter.ToInt32(New_Muscle.Array, Active_Offset + &H4)
                New_Muscle.Total_Length = BitConverter.ToInt32(New_Muscle.Array, Active_Offset + &H8)
                New_Muscle.Parts = BitConverter.ToInt32(New_Muscle.Array, Active_Offset + &HC)
                New_Muscle.Offset = 0
                New_Muscle.Node_Name = "File Part- " & New_Muscle.Starting_Number &
                                " Length- " & Hex(New_Muscle.Total_Length) &
                                " Parts- " & New_Muscle.Parts &
                                " Offset- " & Hex(New_Muscle.Offset)
                For Each temp_muscle As Muscle_Container In Containter_List
                    If ToolStripTextBox2.Text = temp_muscle.Node_Name Then
                        If temp_muscle.Starting_Number = New_Muscle.Starting_Number Then
                            MessageBox.Show("Matching Part Number")
                            Dim difference As Integer = temp_muscle.Total_Length  'Shorter will be negative
                            Dim final_array As Byte() = New Byte(Total_Muscle.Total_Length + difference) {}
                            Buffer.BlockCopy(Total_Muscle.Array, 0, final_array, 0, Total_Muscle.Header_Length)
                            'updates the length
                            Buffer.BlockCopy(BitConverter.GetBytes(CInt(Total_Muscle.Total_Length + difference)), 0, final_array, 8, 4)
                            Buffer.BlockCopy(BitConverter.GetBytes(CInt(Total_Muscle.Parts + 1)), 0, final_array, &HC, 4)
                            Dim New_File_Added As Boolean = False
                            For i As Integer = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                                For Each Adding_Container As Muscle_Container In Containter_List
                                    If Adding_Container.Node_Name = TreeView1.Nodes(0).Nodes(i).Text Then
                                        If Not Adding_Container.Node_Name = temp_muscle.Node_Name Then
                                            If New_File_Added Then
                                                Buffer.BlockCopy(Adding_Container.Array, 0,
                                                            final_array, Adding_Container.Offset + difference,
                                                            Adding_Container.Total_Length)
                                            Else
                                                Buffer.BlockCopy(Adding_Container.Array, 0,
                                                            final_array, Adding_Container.Offset,
                                                            Adding_Container.Total_Length)
                                            End If
                                        Else
                                            Buffer.BlockCopy(Adding_Container.Array, 0,
                                                            final_array, Adding_Container.Offset,
                                                            Adding_Container.Total_Length)
                                            Buffer.BlockCopy(New_Muscle.Array, 0,
                                                            final_array, Adding_Container.Offset + Adding_Container.Total_Length,
                                                            New_Muscle.Total_Length)
                                            New_File_Added = True
                                        End If
                                    End If
                                Next
                            Next
                            File.Copy(active_file, active_file & ".bak", True)
                            File.WriteAllBytes(active_file, final_array)
                            MessageBox.Show("File Saved")
                            OpenFile(active_file)
                        Else
                            MessageBox.Show("Different Part Number")
                            Exit Sub
                        End If
                    End If
                Next
            End If
        End If
    End Sub
End Class