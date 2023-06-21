Imports System.IO
Imports System.Net
Imports System.Reflection.PortableExecutable
Imports System.Security.Principal
Imports System.Threading
Imports System.Xml
Imports Microsoft.Win32

Module Program
    Public Const ShutdownBlockReason As Integer = &H200
    Public Const ShutdownBlockReasonQuery As Integer = &H400
    Public Const ES_CONTINUOUS As Integer = &H80000000
    Public Const ES_SYSTEM_REQUIRED As Integer = &H1

    <Runtime.InteropServices.DllImport("kernel32.dll")>
    Public Function SetThreadExecutionState(ByVal esFlags As Integer) As Integer
    End Function

    <Runtime.InteropServices.DllImport("user32.dll")>
    Public Function ShutdownBlockReasonCreate(ByVal hWnd As IntPtr, ByVal pwszReason As String) As Boolean
    End Function
    ReadOnly version As String = "1.1"
    Sub Main(arguments As String())
#Region "basic shit"
        updatever()
        If arguments.Length = 0 Then ' Check if there are any arguments
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Backwards Package Manager " + version)
            Console.ResetColor()
            Console.WriteLine("Use 'backwards -h' for help")
#End Region
#Region "help"
        ElseIf arguments(0) = "help" Or arguments(0) = "/h" Or arguments(0) = "-h" Then
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Backwards Package Manager " + version)
            Console.ResetColor()
            Console.WriteLine("Help     | '/h', '-h', 'help'         | Shows these messages")
            Console.WriteLine("Search   | '/s', '-s', 'search'       | Search for a package")
            Console.WriteLine("Describe | '/des', '-des', 'describe' | Show the description of a package")
            Console.WriteLine("List     | '/l', '-l', 'list'         | Lists installed packages")
            Console.WriteLine("Settings | '/set', '-set', 'setup'    | Opens settings")
            Console.ForegroundColor = ConsoleColor.DarkYellow
            Console.WriteLine("These commands aren't working for now.")
            Console.WriteLine("Install  | '/i', '-i', 'install'      | Install a package")
            Console.WriteLine("Remove   | '/r', '-r', 'remove'       | Uninstall a package")
            Console.WriteLine("Update   | '/u', '-u', 'update'       | Updates a package")
            Console.WriteLine("Updateall| '/ua', '-ua', 'updateall'  | Updates all installed package")
            Console.ResetColor()
#End Region
#Region "setup"
        ElseIf arguments(0) = "setup" Or arguments(0) = "/set" Or arguments(0) = "-set" Then
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Backwards Setup Utility " + version)
            Console.ResetColor()
            Console.WriteLine("Press a button to open its setup.")
            Console.WriteLine("1 - Current Install directory")
            Console.WriteLine("2 - Install Directory management")
            Console.Beep(37, 1)
            Dim userinput As String = Console.ReadKey().KeyChar.ToString
#Region "1"
            If userinput = "1" Then
                ClearCurrentConsoleLine()
                ClearCurrentConsoleLine()
                Dim xmlDoc As New XmlDocument()
                xmlDoc.Load(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.xml"))
                Dim backwsetting As XmlNode = xmlDoc.SelectSingleNode("installloc")
                Dim installoc As XmlNode = backwsetting.SelectSingleNode("pkgloc")
                Dim current As XmlNode = backwsetting.SelectSingleNode(installoc.InnerText)
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("Current location: " + current.InnerText)
                Console.ResetColor()
                Console.WriteLine("Avaiable locations: ")
                Dim result As String = ""
                For Each node As XmlNode In xmlDoc.SelectSingleNode("installloc").ChildNodes
                    If node.Name <> "pkgloc" Then
                        result += "- " + node.InnerText + vbCrLf
                    End If
                Next
                Console.WriteLine(result)
            End If
#End Region
#Region "2"
            If userinput = "2" Then
                Dim nodenames As String
                ClearCurrentConsoleLine()
                ClearCurrentConsoleLine()
                Dim xmlDoc As New XmlDocument()
                xmlDoc.Load(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.xml"))
                Console.WriteLine("Current locations: ")
                Dim result As String = ""
                For Each node As XmlNode In xmlDoc.SelectSingleNode("installloc").ChildNodes
                    If node.Name <> "pkgloc" Then
                        result += "ID: " + node.Name + " | LOC: " + node.InnerText + vbCrLf
                        nodenames += node.Name + " , "
                    End If
                Next
                Console.WriteLine(result)
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("What do you want to do?")
                Console.ResetColor()
                Console.WriteLine("1 - Remove a location")
                Console.WriteLine("2 - Add a location")
                Console.WriteLine("3 - Edit a location")
                Console.WriteLine("4 - Set User directory")
                Dim pressed As String = Console.ReadKey.KeyChar.ToString
#Region "1"
                If pressed = "1" Then
                    ClearCurrentConsoleLine()
                    ClearCurrentConsoleLine()
                    ClearCurrentConsoleLine()
                    ClearCurrentConsoleLine()
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("Which location do you want to remove? (Give It's ID)")
                    Console.ResetColor()
                    Dim id As String = Console.ReadLine
                    If id = "UserFiles" Or id = "ProgFiles" Or id = "ProgFilesX86" Or id = "pkgloc" Then
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                        Console.WriteLine("END: For your safety you cannot remove this location")
                        Console.ResetColor()
                        End
                    End If
                    ClearCurrentConsoleLine()
                    If nodenames.Contains(id) Then
                        For Each node As XmlNode In xmlDoc.SelectSingleNode("installloc").ChildNodes
                            If node.Name = id Then
                                xmlDoc.SelectSingleNode("installloc").RemoveChild(node)
                                xmlDoc.Save(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.xml"))
                                Console.WriteLine("Found " + id + " and removed it.")
                            End If
                        Next
                    Else
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("ERR: No such ID")
                        Console.ResetColor()
                    End If
#End Region
#Region "2"
                ElseIf pressed = "2" Then
                    ClearCurrentConsoleLine()
                    ClearCurrentConsoleLine()
                    ClearCurrentConsoleLine()
                    ClearCurrentConsoleLine()
                    Console.ForegroundColor = ConsoleColor.Cyan
                    Console.WriteLine("Give an ID")
                    Console.ResetColor()
                    Dim id As String = Console.ReadLine
                    If id = "UserFiles" Or id = "ProgFiles" Or id = "ProgFilesX86" Or id = "pkgloc" Then
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                        Console.WriteLine("END: For your safety you cannot add this location")
                        Console.ResetColor()
                        End
                    End If
                    Console.ForegroundColor = ConsoleColor.Cyan
                    Console.WriteLine("Give the path (C:\*\*) but not a file")
                    Console.ResetColor()
                    Dim pathloc As String = Console.ReadLine
                    If pathloc.Contains(".") Then
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("ERR: Given path contains .")
                        Console.ResetColor()
                        End
                    Else
                        If Directory.Exists(pathloc) Then
                            Console.WriteLine("Path exist just saving it.")
                        Else
                            Console.ForegroundColor = ConsoleColor.Yellow
                            Console.WriteLine("WARN: Path doesn't exist, making a new folder.")
                            Console.ResetColor()
                            Directory.CreateDirectory(pathloc)
                        End If
                        Dim hasid As Boolean
                        Dim haspath As Boolean
                        For Each node As XmlNode In xmlDoc.SelectSingleNode("installloc").ChildNodes
                            If node.Name = id Then
                                hasid = True
                            Else
                                hasid = False
                            End If
                            If node.InnerText = pathloc Then
                                haspath = True
                            Else
                                haspath = False
                            End If
                        Next
                        If hasid = False Then
                            If haspath = False Then
                                Console.ForegroundColor = ConsoleColor.Green
                                Console.WriteLine("INFO: No conflicting options found.")
                                Console.ResetColor()
                                Dim rootnode As XmlNode = xmlDoc.SelectSingleNode("installloc")
                                Dim pathnode As XmlNode = xmlDoc.CreateElement(id)
                                pathnode.InnerText = pathloc
                                rootnode.AppendChild(pathnode)
                                xmlDoc.Save(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.xml"))
                            Else
                                Console.ForegroundColor = ConsoleColor.Red
                                Console.WriteLine("ERR: This PATH is already assigned, use EDIT or REM to do something with it")
                                Console.ResetColor()
                                End
                            End If
                        Else
                            Console.ForegroundColor = ConsoleColor.Red
                            Console.WriteLine("ERR: This ID already exist, use EDIT or REM to do something with it")
                            Console.ResetColor()
                            End
                        End If
                    End If
#End Region
#Region "3"
                ElseIf pressed = "3" Then
                    ClearCurrentConsoleLine()
                    ClearCurrentConsoleLine()
                    ClearCurrentConsoleLine()
                    ClearCurrentConsoleLine()
                    Console.ForegroundColor = ConsoleColor.Cyan
                    Console.WriteLine("Which location do you want to edit? (Give It's ID)")
                    Console.ResetColor()
                    Dim id As String = Console.ReadLine
                    Dim hasid As Boolean
                    If id = "UserFiles" Or id = "ProgFiles" Or id = "ProgFilesX86" Or id = "pkgloc" Then
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                        Console.WriteLine("END: For your safety you cannot edit this location")
                        Console.ResetColor()
                        End
                    End If
                    If nodenames.Contains(id) Then
                        hasid = True
                    Else
                        hasid = False
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("ERR: No such ID (" + id + ")")
                        Console.ResetColor()
                        End
                    End If
                    Console.ForegroundColor = ConsoleColor.Cyan
                    Console.WriteLine("Give the new path")
                    Console.ResetColor()
                    Dim newloc As String = Console.ReadLine
                    If Directory.Exists(newloc) Then
                    Else
                        Try
                            Console.ForegroundColor = ConsoleColor.Yellow
                            Console.WriteLine("WARN: Creating new directory")
                            Console.ResetColor()
                            Directory.CreateDirectory(newloc)
                        Catch
                            Console.ForegroundColor = ConsoleColor.Red
                            Console.WriteLine("ERR: An error occured while creating the new directory")
                            Console.ResetColor()
                            End
                        End Try
                    End If
                    For Each node As XmlNode In xmlDoc.SelectSingleNode("installloc").ChildNodes
                        If node.Name = id Then
                            node.InnerText = newloc
                            xmlDoc.Save(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.xml"))
                            Console.WriteLine("Found " + id + " and edited it.")
                        End If
                    Next
#End Region
#Region "4"
                ElseIf pressed = "4" Then
                    ClearCurrentConsoleLine()
                    ClearCurrentConsoleLine()
                    ClearCurrentConsoleLine()
                    ClearCurrentConsoleLine()
                    Console.WriteLine("Updating...")
                    updateuserpath()
#End Region
#End Region
                End If
            End If
#End Region
#Region "search"
        ElseIf arguments(0) = "search" Or arguments(0) = "/s" Or arguments(0) = "-s" Then
            Dim query As String = ""
            search(query, arguments)
            If query.StartsWith("ERR") Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine(query)
                Console.ResetColor()
            Else
                Console.WriteLine(query)
            End If
#End Region
#Region "describe"
        ElseIf arguments(0) = "describe" Or arguments(0) = "/des" Or arguments(0) = "-des" Then
            Dim query As String = ""
            searchdes(query, arguments)
            If query.StartsWith("ERR") Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine(query)
                Console.ResetColor()
            Else
                Console.WriteLine(query)
            End If
#End Region
#Region "list"
        ElseIf arguments(0) = "list" Or arguments(0) = "/l" Or arguments(0) = "-l" Then
            Dim xmlDoc As New XmlDocument()
            xmlDoc.Load(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "packages.xml"))

            Dim packagesNode As XmlNode = xmlDoc.SelectSingleNode("packages")
            Dim packageNodes As XmlNodeList = packagesNode.SelectNodes("*")

            For Each packageNode As XmlNode In packageNodes
                Dim nameNode As XmlNode = packageNode.SelectSingleNode("name")
                Dim sizeNode As XmlNode = packageNode.SelectSingleNode("size")
                Dim versionNode As XmlNode = packageNode.SelectSingleNode("ver")
                Dim descriptionNode As XmlNode = packageNode.SelectSingleNode("description")

                Dim name As String = nameNode.InnerText
                Dim size As String = sizeNode.InnerText
                Dim pkgver As String = versionNode.InnerText
                Dim description As String = descriptionNode.InnerText

                Dim formattedString As String = $"{name}({pkgver}) | {size} | {description}"
                Console.WriteLine(formattedString)
            Next
#End Region
#Region "install"
        ElseIf arguments(0) = "install" Or arguments(0) = "/i" Or arguments(0) = "-i" Then
            Dim query As String = ""
            searchinstall(query, arguments)
            If query.StartsWith("ERR") Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine(query)
                Console.ResetColor()
            ElseIf query.StartsWith("WARN") Then
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine(query)
                Console.ResetColor()
            End If
#End Region
#Region "shit on end"
        Else
            Console.WriteLine("Backwards Package Manager " + version)
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("ERR | Unknown Arguments")
            Console.ResetColor()
        End If
#End Region
    End Sub
    Function search(ByRef query As String, args As String())
        Console.ForegroundColor = ConsoleColor.Yellow
        Try
            Console.WriteLine("Listing all packages starting with '" + args(1) + "'")
        Catch
        End Try
        Console.ResetColor()
        Dim packageName As String
        Try
            packageName = args(1)
        Catch
        End Try
        Dim fileUrl As String = "https://raw.githubusercontent.com/BalazsManus/BalazsManusOffical/main/backwards/defrepo/pkgdb.bwdb"
        Dim webClient As New WebClient()
#Region "linussextips"
        If packageName = "-l" Then
            Console.WriteLine("                                                               [        ░                           ")
            Console.WriteLine("                                                               [        ;                           ")
            Console.WriteLine("                                                               [        ;                           ")
            Console.WriteLine("                                                               [        ░                           ")
            Console.WriteLine("                                                               [        ░                           ")
            Console.WriteLine("    ∩                                                          [        ░                           ")
            Console.WriteLine("    U                                      ,,╓▄▄▄▄▄▄▄▒@        [        ░                           ")
            Console.WriteLine("    L                                ,▄▄▓▓▓▓▓▓▓▓▓▓▓╣▓▓▓▓▓▓▓▄,  [        ░                           ")
            Console.WriteLine("    F                             ▄▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓╣▓▓▓▓▓▓▓▓▓,       ░                           ")
            Console.WriteLine("    F                          ▄▓▓██▓▓▓▓███▓▓██▓▓▓███▓▓▓▓██▓▓▓▓▓▓▓g     '                           ")
            Console.WriteLine("    Γ                        ▄▓███▓▓▓▓▓▓▓▓▓████████▓▓▓▓▓▓███▓▓▓▓▓╢╣▓▄                               ")
            Console.WriteLine("    [                      ,▓████▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒╢╢╢▓▓▓▄                             ")
            Console.WriteLine("    [                     g▓███▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒╜▒╢▓▓▓╣▓▓▓▓                             ")
            Console.WriteLine("    ║                    ▐▓███▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒░░░░░░░░░░░▒▒╢▓╣╢▓▓╫█▄                           ")
            Console.WriteLine("    ║                   `▓▀██▓▓▓▓╣▒▒▒▒▒░░░░░░░░░░░░░░░░░░░░░░   ░╙╢╣▓╣▓▓█W                          ")
            Console.WriteLine("    ║                 ,▓▓▌ ▐▓▓▓▓▒▒░░░░░░░░░░░░░░░░░░░░░░░░░`      ░╢╢▓╣▓▌║,,                        ")
            Console.WriteLine("    ║                 ▓▓▓` ▐█▓▓╣▒▒░░░░░░░░░░░░░░░░░░░░░░░         ░▒╢▓▓▓▓╙╬▒▒.                      ")
            Console.WriteLine("    ├                j▓▓█▄███▓▓╣▒░░░░░░░░░░░░░░░░░░░░░░░░░,,,     ░▒╢▓▓▓█▄╟▓▒▒                      ")
            Console.WriteLine("    ├              ,▓▓▓▓█████▓▓╣▒░▒▒╣╢╢╬▓▓╣╣▒▒▒▒░░░░░▒╢╢╣╫▓╣╣@▒╖,  ░╫▓▓▓████▓▓╥,                    ")
            Console.WriteLine("    ├              ▐▓█▓▓██████▓▒▒▒╜▒░░▒▒@╣▒▒▒░░░░░░░▒▒▒▒▒▒▒▒▒▒░▒▒▒░ ▒▓▓█████▓▐╢▒L                   ")
            Console.WriteLine("    ╞              ▐▓█▓▓██████▓▒▒░░░▒▒Ñ▓▓▓▌▒▒▒▒▒▒░░▒▒▒▒▒▒▒▓▓▓▒▒╢▒░░░ ╫▓█████▓▓▓▓▌                   ")
            Console.WriteLine("    ]              ▐██▓▓██████▓▒░░░░▒▒▒▒▓▓▒▒▒▒▒▒▒░░ ╙╣▒╣╣▒▓▓▓▒▒▒▒░░░ ╟▓█████▓▓▓▓▌                   ")
            Console.WriteLine("    ]              ▐██▓▓▓█████▒▒░░░░░░░░░░▒▒▒▒▒▒░░   ╙╣╢╣▒▒░░░░      j▓█████▓▓▓█C                   ")
            Console.WriteLine("    ]              ¬▓██▓▓█▓▓██▒▒░░░░░░░░░░░░░▒░▒▒░  '░▒▒▒▒░░░`       ░██████▓▓██                    ")
            Console.WriteLine("L   ]               ▓██▓▓▓▓▓██▒▒░░░░░░░░░░░░░▒▒▒░░░  ░░░░░░ ░░░    ░░░███▓██▓▓█▌                    ")
            Console.WriteLine("█▓▓░]               ▐██▓▌▓▓▓▓█▌▒░░░░░░░░░░░░░▒▒░░░  ,,▒▒░░░    ░░░░░░╠██▓▓█▓▓██M                    ")
            Console.WriteLine("▓▓  ]                ▓▓▓▓▓█▓▓██▒░░░░░░░░░░░░░▒░╠▒▒▒▒╫▒░▒░░░░░░░░░░░░░███▓▓▌▓▓██                     ")
            Console.WriteLine("    ]                ▐██▓▓▓████▒░░░░░░░  ░░░░░▒▒░░▒░▒░  '░░░░░░░░▒▒░╫█████████▌ ,                   ")
            Console.WriteLine("    ]                 ▓██▓▓████▌▒░░░░░░░░░░░░░░░░░▒░░░░░   ░░░░░▒▒▒▒▓██████████▓▓▓~                 ")
            Console.WriteLine("    j                  ██▓██████▒░░░░░░░░░░░░░░░░░▒░░░░░░░░░░░░▄▓▓▓▓█████████▓██▀▀                  ")
            Console.WriteLine("  ░ ]                  ╙▀███████▒░░░░░░░░░░░░╖p@@╣@╬╬╬╬▓╣▒░▒░░██████████████ ▐█▌                    ")
            Console.WriteLine("  ░ j                     ▀▓▌ ░  ▒▒░░░░░░░░░░▒▒▒▒░░▒░░░░▒▒▒▒▒░████████Ñ▓█▀`  ▐▌                     ")
            Console.WriteLine("  ░             ░          ╚▌ ░  ║▒▒░▒▒░░░░░░░░░░░░░░░░░░░▒▒▒▒░▀██████,█ ░  Æ▓C                     ")
            Console.WriteLine("  ░  ░          ░           ▐∩░  ▒▒▒▒░░▒░░░░░░░░░░░░░░░░░░░░▒░▒▒╢H   ▀█▄,▄@╝` ▌                     ")
            Console.WriteLine("  ░  ░          ░           ,▓░,▄▒▒░░░▒▒▒░░░░░░░░░░░░░░░░░░░░▒▒▒╢Ç    ▌  ░    ▌                     ")
            Console.WriteLine("  ░  ░     ,,╓╓╖╖p@@@@▓▓▓▓▓▓▓████▒░░░░░▒▒▒░░░░░░░░░░░░░░░░░░▒▒▒▒▒╫▓▓▓▓▓@@╖╓╓,.▓                     ")
            Console.WriteLine("  ░╓╖▒@╢▒╢╫╬╬▒▒▒▒▒▒▒▒▒▒▒▒▓██████▒▒▒░░░░░▒▒▒▒▒░░░░░░░░░░▒▒▒▒▒▒▒▒▒▒▓████▓▓╢▒▓▓▓▓▓▓▓▄@p╖,,             ")
            Console.WriteLine("▒▓Ñ▒▒▒▒▒▒▒╣▒▒╢▒▒▓█████████████▒▓▒▒▒░░░░░░▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒╣╢▒▒▒▒▒▒▒▒███████▓▓╣╢╢╫▓╣╢╢▓▓▓▓▓▓@╖.        ")
            Console.WriteLine("▒▒▒▒▒▒▒▒╣╢╣╢╣▓███████████████▌▒▒▓▒▒░░░░░░▒▒▒░░░░░░░░░░▒▒╢▒▒▒▒▒▒▒▒░▓██████████▓▓▓▓▓▓▓╢╣╣▓╢╣▓▓▓@      ")
            Console.WriteLine("▒▒▒╢╢╣╢╢╢▒▓▓██████████████████▒▒▒▓▒▒░░░░░░▒▒▒░░░░░░░░▒▒▒▒▒▒▒▒▒▒▒░░▐█████████████▓▓▓▓▓▓▓▓▓▓╢▓╣▓▓@    ")
            Console.WriteLine("╢╢╢╢╢╢╢╢██████████████████████▌▒▒▐▌▒▒░░░░░░░░░░▒░░░▒▒▒▒▒▒▒▒▒▒▒▒▒░░█████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▒   ")
            Console.WriteLine("╢╢▓▓███████████████████████████▌▒░▓░▒▒░░░░░░░░░░░░░▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██████████████████████▓▓▓▓▓▓▓█╣   ")
            Console.WriteLine("████████████████████████████████▌░░▓░░▒░░░░░░░░░░░░░▒▒▒▒▒▒▒▒▒▒▒▒█████████████████▓██████████████╬   ")
            Console.WriteLine("██████████████████████████████████╖░▓░░▒░░░░░░░░░░░░▒▒░░▒▒▒▒▒▒▓█████████████████████████████████Ç   ")
            Console.WriteLine("████████████████████████████████████▓▌░░░░░░░░░░░░░░░░░░▒░░░▄█████████████████████████████████████▄,")
            Console.WriteLine("▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀""``````````````````""▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀")
        Else
#End Region
            Try
                Console.WriteLine("Downloading database...")
                Dim fileData As String = webClient.DownloadString(fileUrl)
                ClearCurrentConsoleLine()
                ' kiírja a letöltött dolgokat, csak hogy biztosan letöltötte-e
                Dim lines As String() = fileData.Split(ControlChars.Lf)
                For Each line As String In lines
                    If line.StartsWith(packageName) Then
                        Dim packageInfo As String() = line.Split
                        query = query + (String.Join(" ", packageInfo)) + vbCrLf
                    End If
                Next
            Catch ex As Exception
                If ex.Message = "Value cannot be null. (Parameter 'value')" Then
                    query = "ERR | No package name provided"
                Else
                    query = "ERR | " + ex.Message
                End If
            Finally
                webClient.Dispose()
            End Try
        End If
        If query = Nothing Then
            query = "WARN | No package found"
        End If
        Return query
    End Function
    Function searchdes(ByRef query As String, args As String())
        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine("Showing description for " + args(1))
        Console.ResetColor()
        Dim packageName As String
        Try
            packageName = args(1)
        Catch
        End Try
        Dim fileUrl As String = "https://raw.githubusercontent.com/BalazsManus/BalazsManusOffical/main/backwards/defrepo/pkgdb.bwdb"
        Dim webClient As New WebClient()
        Try
            Console.WriteLine("Downloading database...")
            Dim fileData As String = webClient.DownloadString(fileUrl)
            ClearCurrentConsoleLine()
            Dim lines As String() = fileData.Split(ControlChars.Lf)
            Dim matchingPackages As New List(Of String)()

            For Each line As String In lines
                If line.StartsWith(packageName) Then
                    matchingPackages.Add(line)
                End If
            Next

            If matchingPackages.Count = 1 Then
                Dim packageInfo As String() = matchingPackages(0).Split(" | ")
                query = "Package ID/Name: " + packageInfo(0) + vbCrLf
                query = query + "Package Size: " + packageInfo(1) + vbCrLf
                query = query + "Description: " + packageInfo(2)
            ElseIf matchingPackages.Count > 1 Then
                query = "ERR | There are more than one packages with this name"
            End If
        Catch ex As Exception
            If ex.Message = "Value cannot be null. (Parameter 'value')" Then
                query = "ERR | No package name provided"
            Else
                query = "ERR | " + ex.Message
            End If
        Finally
            webClient.Dispose()
        End Try
        If query = Nothing Then
            query = "WARN | No package found"
        End If
        Return query
    End Function
    Sub ClearCurrentConsoleLine()
        Dim currentLineCursor As Integer = Console.CursorTop
        Console.SetCursorPosition(0, Console.CursorTop - 1)
        Console.Write(New String(" "c, Console.WindowWidth))
        Console.SetCursorPosition(0, currentLineCursor - 1)
        Console.SetCursorPosition(0, Console.CursorTop)
    End Sub
    Function updatever()
        Dim xmlDoc As New XmlDocument()
        xmlDoc.Load(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "packages.xml"))

        Dim verNode As XmlNode = xmlDoc.SelectSingleNode("packages/backwards/ver")
        verNode.InnerText = version
        xmlDoc.Save(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "packages.xml"))
        updateuserpath()
        Return True
    End Function
    Function updateuserpath()
        Dim xmlDoc As New XmlDocument()
        xmlDoc.Load(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.xml"))
        Dim hasfiles As Boolean
        For Each node As XmlNode In xmlDoc.SelectSingleNode("installloc").ChildNodes
            If node.Name = "UserFiles" Then
                hasfiles = True
            End If
        Next

        If hasfiles = True Then
            Dim userfiles As XmlNode = xmlDoc.SelectSingleNode("installloc/UserFiles")

            Try
                If Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.backwards")) Then
                Else
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.backwards"))
                End If
                userfiles.InnerText = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.backwards")
                xmlDoc.Save(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.xml"))
            Catch ex As Exception
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("WARN: UserFiles Location may not be accesible, run backwards as administrator to fix")
                Console.ResetColor()
            End Try
        Else
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("Regenerating userfiles...")
            Console.ResetColor()
            Dim rootnode As XmlNode = xmlDoc.SelectSingleNode("installloc")
            Dim pathnode As XmlNode = xmlDoc.CreateElement("UserFiles")
            rootnode.AppendChild(pathnode)
            xmlDoc.Save(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.xml"))
            Dim userfiles As XmlNode = xmlDoc.SelectSingleNode("installloc/UserFiles")

            Try
                If Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.backwards")) Then
                Else
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.backwards"))
                End If
                userfiles.InnerText = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.backwards")
                xmlDoc.Save(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.xml"))
            Catch ex As Exception
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("WARN: UserFiles Location may not be accesible, run backwards as administrator to fix")
                Console.ResetColor()
            End Try
        End If
        Return True
    End Function
    Function searchinstall(ByRef query As String, args As String())
        Console.ForegroundColor = ConsoleColor.Yellow
        If args.Count = 1 Then
            query = "ERR | No package name provided"
            Exit Function
        End If
        Console.WriteLine("Search for an install script for " + args(1))
        Console.ResetColor()
        Dim packageName As String
        Try
            packageName = args(1)
        Catch
        End Try
        Dim url As String
        Dim fileUrl As String = "https://raw.githubusercontent.com/BalazsManus/BalazsManusOffical/main/backwards/defrepo/pkgdb.bwdb"
        Dim webClient As New WebClient()
        Try
            Console.WriteLine("Downloading database...")
            Dim fileData As String = webClient.DownloadString(fileUrl)
            ClearCurrentConsoleLine()
            Dim lines As String() = fileData.Split(ControlChars.Lf)
            Dim matchingPackages As New List(Of String)()

            For Each line As String In lines
                If line.StartsWith("_" + packageName) Then
                    matchingPackages.Add(line)
                End If
            Next

            If matchingPackages.Count = 1 Then
                Dim packageInfo As String() = matchingPackages(0).Split(" "c)
                Console.WriteLine("Downloading installer script for " + packageInfo(0))
                query = "found"
                url = packageInfo(1)
                webClient.DownloadFile(url, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), "installscript.ini"))
                InUnScripts.Install(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), "installscript.ini"))
            ElseIf matchingPackages.Count > 1 Then
                query = "ERR | There are more than one packages with this name"
            End If
        Catch ex As Exception
            If ex.Message = "Value cannot be null. (Parameter 'value')" Then
                query = "ERR | No package name provided"
            Else
                query = "ERR | " + ex.Message
            End If
        Finally
            webClient.Dispose()
        End Try
        If query = Nothing Or query = "" Or query = " " Then
            query = "WARN | No package found"
        End If
    End Function
End Module
Public Class InUnScripts
    Public Shared Sub Install(path As String)
        If Not IsAdmin() Then
            RestartAsAdmin()
            Return
        End If
        Dim inside As String = File.ReadAllText(path)
        Dim formatted() As String = inside.Split(ControlChars.Lf)
        Dim line As Integer
        line = 0
        For Each dataIn As String In formatted
            Dim format As String() = dataIn.Split(" "c)
            line = line + 1
            If format(0) = "PkgName" Then
                Console.WriteLine("Installer script for " + format(1))
                PkgName = format(1)
            ElseIf format(0) = "ExecPolicy" Then
                Console.WriteLine("Execution policy is now set to " + format(1))
                ExecPolicy = format(1)
                If ExecPolicy = "Danger" Then
                    Console.WriteLine("WARN | There is a high chance that this script is going to edit the registry and/or the system files.")
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("/!\ DO NOT SHUT DOWN THE COMPUTER UNTIL THE LAST MESSAGE!")
                    Console.ResetColor()
                    Dim key As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", True)
                    key.SetValue("ShutdownBlockReasonCreate", 1, RegistryValueKind.DWord)
                    key.Close()
                    ShutdownBlockReasonCreate(IntPtr.Zero, "Installation script in progress")
                    SetThreadExecutionState(ES_SYSTEM_REQUIRED)
                End If
            ElseIf format(0) = "sleep" Then
                Thread.Sleep(100000)
            Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("ERR | Unrecognised code found at line #" + line.ToString)
                Console.WriteLine(format(0))
                Console.ResetColor()
            End If
        Next
        Console.WriteLine("Install script finished. Deleting downloaded script...")
        File.Delete(path)
        Console.WriteLine("You can proceed to shut down your computer safely.")
        ShutdownBlockReasonCreate(IntPtr.Zero, Nothing)
        SetThreadExecutionState(ES_CONTINUOUS)
    End Sub
    Public Sub Uninstall(dataIn As String)

    End Sub

    Public Shared Function IsAdmin() As Boolean
        Dim identity = WindowsIdentity.GetCurrent()
        Dim principal = New WindowsPrincipal(identity)
        Return principal.IsInRole(WindowsBuiltInRole.Administrator)
    End Function
    Public Shared Sub RestartAsAdmin()
        Dim startInfo = New ProcessStartInfo()
        startInfo.UseShellExecute = True
        startInfo.WorkingDirectory = Environment.CurrentDirectory
        startInfo.FileName = Process.GetCurrentProcess.MainModule.FileName
        Dim args As String = Environment.GetCommandLineArgs(1) + " " + Environment.GetCommandLineArgs(2)
        startInfo.Arguments = args
        startInfo.Verb = "runas" ' Ez a kulcsfontosságú rész, ami indításkor admin jogot kér
        Try
            Process.Start(startInfo)
        Catch ex As Exception
            ' Hiba esetén itt kezelheted az alkalmazás újraindítását vagy más hibakezelést
            Console.WriteLine("Hiba történt az adminisztrátori jogosultságú indításkor: " & ex.Message)
        End Try

        Environment.Exit(0)
    End Sub
    Public Shared Property PkgName
    Public Shared Property ExecPolicy
End Class
