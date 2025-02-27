﻿Imports System.ComponentModel
Imports System.IO
Imports Eto.WinForms

Public Class FormFileExplorer

    Inherits WeifenLuo.WinFormsUI.Docking.DockContent

    Private TempDir As String

    Private Loaded As Boolean = False

    Public Flowsheet As FormFlowsheet

    Private Sub FormFileExplorer_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
        Directory.CreateDirectory(TempDir)

        Loaded = True

        UpdateSize()
        ListFiles()

    End Sub

    Private Sub UpdateSize()

        lblSize.Text = String.Format(Flowsheet.GetTranslatedString1("DBSize"), Flowsheet.FileDatabaseProvider.GetSizeinKB())

    End Sub

    Public Sub ListFiles()

        Dim provider = Flowsheet.FileDatabaseProvider
        Dim files = provider.GetFiles()
        ListView1.Items.Clear()
        For Each item In files
            ListView1.Items.Add(item)
        Next

    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged

        If Loaded Then
            If ListView1.SelectedItems.Count <= 0 Then Exit Sub
            Dim provider = Flowsheet.FileDatabaseProvider
            DisplayFileInViewer(ListView1.SelectedItems(0).Text)
        End If

    End Sub

    Public Sub DisplayFileInViewer(filename As String)

        UIThread(Sub()
                     Dim provider = Flowsheet.FileDatabaseProvider
                     If provider.CheckIfExists(filename) Then
                         Dim TempFilePath As String = Path.Combine(TempDir, filename)
                         provider.ExportFile(filename, TempFilePath)
                         Viewer.Source = New Uri(TempFilePath)
                     End If
                     Me.Activate()
                 End Sub)


    End Sub

    Public Sub SetViewerURL(url As String)

        UIThread(Sub()
                     Viewer.Source = New Uri(url)
                     Me.Activate()
                 End Sub)

    End Sub

    Private Sub btnImport_Click(sender As Object, e As EventArgs) Handles btnImport.Click

        If Me.ofd1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Dim provider = Flowsheet.FileDatabaseProvider
            For Each file In ofd1.FileNames
                Try
                    provider.PutFile(file)
                Catch ex As Exception
                    MessageBox.Show(file + ":" + ex.Message, Flowsheet.GetTranslatedString1("Erro"))
                End Try
            Next
            ListFiles()
            UpdateSize()
        End If

    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        If ListView1.SelectedItems.Count > 0 Then
            sfd1.FileName = ListView1.SelectedItems(0).Text
            If Me.sfd1.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim provider = Flowsheet.FileDatabaseProvider
                Try
                    provider.ExportFile(ListView1.SelectedItems(0).Text, sfd1.FileName)
                Catch ex As Exception
                    MessageBox.Show(ListView1.SelectedItems(0).Text + ":" + ex.Message, Flowsheet.GetTranslatedString1("Erro"))
                End Try
            End If
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If ListView1.SelectedItems.Count > 0 Then
            If MessageBox.Show(DWSIM.App.GetLocalString("ConfirmOperation"),
                                          DWSIM.App.GetLocalString("Ateno2"),
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question) = DialogResult.Yes Then
                Dim provider = Flowsheet.FileDatabaseProvider
                Try
                    provider.DeleteFile(ListView1.SelectedItems(0).Text)
                Catch ex As Exception
                    MessageBox.Show(ListView1.SelectedItems(0).Text + ":" + ex.Message, Flowsheet.GetTranslatedString1("Erro"))
                End Try
                ListFiles()
                UpdateSize()
            End If
        End If
    End Sub

    Private Sub FormFileExplorer_GotFocus(sender As Object, e As EventArgs) Handles Me.GotFocus

        ListFiles()
        UpdateSize()

    End Sub

    Private Sub FormFileExplorer_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed

        Try
            Directory.Delete(TempDir, True)
        Catch ex As Exception
        End Try

    End Sub

End Class