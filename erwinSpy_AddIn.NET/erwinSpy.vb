<ComClass(ERwinSpy.ClassId, ERwinSpy.InterfaceId, ERwinSpy.EventsId)> _
Public Class ERwinSpy

#Region "COM GUIDs"
    ' These  GUIDs provide the COM identity for this class 
    ' and its COM interfaces. If you change them, existing 
    ' clients will no longer be able to access the class.
    Public Const ClassId As String = "310f8ec3-525c-4782-b9b2-e2e4fb8dea25"
    Public Const InterfaceId As String = "73a2e1a8-7ea8-4700-93b9-e4c6b70a922a"
    Public Const EventsId As String = "0ad23f15-90df-482e-8673-36e0237dc913"
#End Region

    ' A creatable COM class must have a Public Sub New() 
    ' with no parameters, otherwise, the class will not be 
    ' registered in the COM registry and cannot be created 
    ' via CreateObject.
    Public Sub New()
        MyBase.New()
    End Sub

    <ComRegisterFunctionAttribute()> _
    Public Shared Sub RegisterFunction(ByVal type As Type)
        Dim s As String = ("CLSID\{" _
                    + (type.GUID.ToString.ToUpper + "}"))

        Registry.ClassesRoot.CreateSubKey(GetSubKeyName(type))

        Dim rKey As RegistryKey = Registry.ClassesRoot.OpenSubKey(s, True)
        rKey.SetValue("", "erwin9 Spy Addin")
        rKey.Close()
    End Sub

    <ComUnregisterFunctionAttribute()> _
    Public Shared Sub UnregisterFunction(ByVal type As Type)
        Registry.ClassesRoot.DeleteSubKey(GetSubKeyName(type), False)
    End Sub

    Private Shared Function GetSubKeyName(ByVal type As Type) As String
        Dim s As String = ("CLSID\{" _
                    + (type.GUID.ToString.ToUpper + "}\Programmable"))
        Return s
    End Function

    Public Sub Run()
        Try
            ' Activate the API
            Dim oSCAPI As SCAPI.Application
            oSCAPI = CreateObject("erwin9.SCAPI")

            ' Init the form
            Dim form As New frmERwinSpy
            form.Init(oSCAPI)

            ' Show the form
            form.ShowDialog()
        Catch ex As Exception
            MsgBox("Failed to launch erwin Spy due to: " + ex.Message)
        End Try
    End Sub

End Class


