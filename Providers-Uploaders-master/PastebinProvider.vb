Imports System.Collections.Specialized
Imports System.Net
Imports System.Net.Http

'▓███████▓ Pastbin Wrapper ▓███████▓
'▓▓▓▓▓▓▓▓▓▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▓▓▓▓▓▓▓▓▓
'▓  Credits:                       ▓
'▓      ├ Kevin(Nettro - HF)       ▓
'▓      └ Marco(Virility - HF)     ▓
'▓  Version: 1.0.0.0               ▓
'▓  Date Published: July 31, 2016  ▓
'▓█████████████████████████████████▓
'▓Leave credit where credit is due.▓
'▓█████████████████████████████████▓

Public Class PasteBin

    Implements IDisposable
    Private ReadOnly _apiDevKey As String
    Private ReadOnly _client As HttpClient
    Public ApiUserKey As String
    Public Credential As NetworkCredential

    Sub New(apiDevKey As String)
        _apiDevKey = apiDevKey
        _client = New HttpClient()
        _client.BaseAddress = New Uri("http://pastebin.com/api/")
    End Sub

    Public Async Function Login(credential As NetworkCredential) As Task(Of ApiResponse)
        credential = credential

        Dim postData = New Dictionary(Of String, String)()
        postData.Add("api_dev_key", _apiDevKey)
        postData.Add("api_user_name", credential.UserName)
        postData.Add("api_user_password", credential.Password)

        Dim response = Await _client.PostAsync("api_login.php", New FormUrlEncodedContent(postData))
        Dim content = Await response.Content.ReadAsStringAsync()

        Return New ApiResponse(Me, content, "Login")
    End Function

    Public Async Function GetRaw(pasteKey As String) As Task(Of ApiResponse)
        If (String.IsNullOrWhiteSpace(ApiUserKey)) Then
            Return New ApiResponse(Me, String.Empty, "GetRaw")
        End If

        Dim postData = New Dictionary(Of String, String)()
        postData.Add("api_dev_key", _apiDevKey)
        postData.Add("api_user_key", ApiUserKey)
        postData.Add("api_option", "show_paste")
        postData.Add("api_paste_key", pasteKey)

        Dim response = Await _client.PostAsync("api_raw.php", New FormUrlEncodedContent(postData))
        Dim content = Await response.Content.ReadAsStringAsync()

        Return New ApiResponse(Me, content, "GetRaw")
    End Function
    Public Async Function GetUserInfo() As Task(Of ApiResponse)
        If (String.IsNullOrWhiteSpace(ApiUserKey)) Then
            Return New ApiResponse(Me, String.Empty, "GetUserInfo")
        End If

        Dim postData = New Dictionary(Of String, String)()
        postData.Add("api_dev_key", _apiDevKey)
        postData.Add("api_user_key", ApiUserKey)
        postData.Add("api_option", "userdetails")

        Dim response = Await _client.PostAsync("api_post.php", New FormUrlEncodedContent(postData))
        Dim content = Await response.Content.ReadAsStringAsync()

        Return New ApiResponse(Me, content, "GetUserInfo")
    End Function
    Public Async Function GetPastes() As Task(Of ApiResponse)
        If (String.IsNullOrWhiteSpace(ApiUserKey)) Then
            Return New ApiResponse(Me, String.Empty, "GetPastes")
        End If

        Dim postData = New Dictionary(Of String, String)()
        postData.Add("api_dev_key", _apiDevKey)
        postData.Add("api_user_key", ApiUserKey)
        postData.Add("api_option", "list")

        Dim response = Await _client.PostAsync("api_post.php", New FormUrlEncodedContent(postData))
        Dim content = Await response.Content.ReadAsStringAsync()

        Return New ApiResponse(Me, content, "GetPastes")
    End Function

    Public Async Function CreatePaste(paste As Paste) As Task(Of ApiResponse)
        If (String.IsNullOrWhiteSpace(ApiUserKey)) Then
            Return New ApiResponse(Me, String.Empty, "CreatePaste")
        End If

        Dim postData = New Dictionary(Of String, String)()
        postData.Add("api_dev_key", _apiDevKey)
        postData.Add("api_user_key", ApiUserKey)
        postData.Add("api_option", "paste")
        postData.Add("api_paste_code", paste.Code)
        postData.Add("api_paste_name", paste.Name)
        postData.Add("api_paste_format", paste.Language)
        postData.Add("api_paste_private", paste.Exposure.ToString())
        postData.Add("api_paste_expire_date", paste.ExpireDate)

        Dim response = Await _client.PostAsync("api_post.php", New FormUrlEncodedContent(postData))
        Dim content = Await response.Content.ReadAsStringAsync()

        Return New ApiResponse(Me, content, "CreatePaste")
    End Function

    Public Async Function DeletePaste(pasteKey As String) As Task(Of ApiResponse)
        If (String.IsNullOrWhiteSpace(ApiUserKey)) Then
            Return New ApiResponse(Me, String.Empty, "DeletePaste")
        End If

        Dim postData = New Dictionary(Of String, String)()
        postData.Add("api_dev_key", _apiDevKey)
        postData.Add("api_user_key", ApiUserKey)
        postData.Add("api_option", "delete")
        postData.Add("api_paste_key", pasteKey)

        Dim response = Await _client.PostAsync("api_post.php", New FormUrlEncodedContent(postData))
        Dim content = Await response.Content.ReadAsStringAsync()

        Return New ApiResponse(Me, content, "DeletePaste")
    End Function

    Public Overloads Sub Dispose() Implements IDisposable.Dispose
        _client.Dispose()
    End Sub
End Class

Public Class ApiResponse
    Private Const BadApiRequest = "Bad API request, "

    Public ContentData As String
    Public [Error] As String
    Public Success As Boolean

    Public Sub New(pasteBin As PasteBin, content As String, source As String)
        ContentData = content
        Success = Not (content.StartsWith(BadApiRequest) Or String.IsNullOrWhiteSpace(content))

        If (Not Success) Then
            [Error] = content.Substring(BadApiRequest.Length, content.Length - BadApiRequest.Length)
        ElseIf (source = "Login") Then
            pasteBin.ApiUserKey = content
        End If
    End Sub
End Class

Public Class ExpireDate
    Public Const Never As String = "N"
    Public Const TenMinutes As String = "10M"
    Public Const OneHour As String = "1H"
    Public Const OneDay As String = "1D"
    Public Const OneWeek As String = "1W"
    Public Const TwoWeeks As String = "2W"
    Public Const OneMonth As String = "1M"
End Class

 Public Enum Exposure
    [Public] = 0
    Unlisted = 1  
    [Private] = 2  
End Enum

Public Class Paste                  
    Public Name As String
    Public Code As String
    Public Exposure As Integer
    Public ExpireDate As String        
    Public Language As String   
        
    Public Sub New()
        Exposure = PasteBinWrapperThingy.Exposure.Public 
        ExpireDate = PasteBinWrapperThingy.ExpireDate.Never    
        Language = Languages.None   
    End Sub   
End Class

Public Class Languages
    Public Const _4CS As String = "4cs"
    Public Const _6502ACMECrossAssembler As String = "6502acme"
    Public Const _6502KickAssembler As String = "6502kickass"
    Public Const _6502TASM64TASS As String = "6502tasm"
    Public Const _ABAP As String = "abap"
    Public Const ActionScript As String = "actionscript"
    Public Const ActionScript3 As String = "actionscript3"
    Public Const Ada As String = "ada"
    Public Const AIMMS As String = "aimms"
    Public Const ALGOL68 As String = "algol68"
    Public Const ApacheLog As String = "apache"
    Public Const AppleScript As String = "applescript"
    Public Const APTSources As String = "apt_sources"
    Public Const ARM As String = "arm"
    Public Const ASM As String = "asm"
    Public Const ASP As String = "asp"
    Public Const Asymptote As String = "asymptote"
    Public Const autoconf As String = "autoconf"
    Public Const Autohotkey As String = "autohotkey"
    Public Const AutoIt As String = "autoit"
    Public Const Avisynth As String = "avisynth"
    Public Const Awk As String = "awk"
    Public Const BASCOMAVR As String = "bascomavr"
    Public Const Bash As String = "bash"
    Public Const Basic4GL As String = "basic4gl"
    Public Const Batch As String = "dos"
    Public Const BibTeX As String = "bibtex"
    Public Const BlitzBasic As String = "blitzbasic"
    Public Const Blitz3D As String = "b3d"
    Public Const BlitzMax As String = "bmx"
    Public Const BNF As String = "bnf"
    Public Const BOO As String = "boo"
    Public Const BrainFuck As String = "bf"
    Public Const C As String = "c"
    Public Const CWinAPI As String = "c_winapi"
    Public Const CforMacs As String = "c_mac"
    Public Const CIntermediateLanguage As String = "cil"
    Public Const CSharp As String = "csharp"
    Public Const CPlusPlus As String = "cpp"
    Public Const CPlusPlusWinApi As String = "cpp-winapi"
    Public Const CPlusPlusWithQtextensions As String = "cpp-qt"
    Public Const CLoadrunner As String = "c_loadrunner"
    Public Const CADDCL As String = "caddcl"
    Public Const CADLisp As String = "cadlisp"
    Public Const CFDG As String = "cfdg"
    Public Const ChaiScript As String = "chaiscript"
    Public Const Chapel As String = "chapel"
    Public Const Clojure As String = "clojure"
    Public Const CloneC As String = "klonec"
    Public Const CloneCPlusPlus As String = "klonecpp"
    Public Const CMake As String = "cmake"
    Public Const COBOL As String = "cobol"
    Public Const CoffeeScript As String = "coffeescript"
    Public Const ColdFusion As String = "cfm"
    Public Const CSS As String = "css"
    Public Const Cuesheet As String = "cuesheet"
    Public Const D As String = "d"
    Public Const Dart As String = "dart"
    Public Const DCL As String = "dcl"
    Public Const DCPU16 As String = "dcpu16"
    Public Const DCS As String = "dcs"
    Public Const Delphi As String = "delphi"
    Public Const Oxygene As String = "oxygene"
    Public Const Diff As String = "diff"
    Public Const DIV As String = "div"
    Public Const DOT As String = "dot"
    Public Const E As String = "e"
    Public Const Easytrieve As String = "ezt"
    Public Const ECMAScript As String = "ecmascript"
    Public Const Eiffel As String = "eiffel"
    Public Const Email As String = "email"
    Public Const EPC As String = "epc"
    Public Const Erlang As String = "erlang"
    Public Const Euphoria As String = "euphoria"
    Public Const FSharp As String = "fsharp"
    Public Const Falcon As String = "falcon"
    Public Const Filemaker As String = "filemaker"
    Public Const FOLanguage As String = "fo"
    Public Const FormulaOne As String = "f1"
    Public Const Fortran As String = "fortran"
    Public Const FreeBasic As String = "freebasic"
    Public Const FreeSWITCH As String = "freeswitch"
    Public Const GAMBAS As String = "gambas"
    Public Const GameMaker As String = "gml"
    Public Const GDB As String = "gdb"
    Public Const Genero As String = "genero"
    Public Const Genie As String = "genie"
    Public Const GetText As String = "gettext"
    Public Const Go As String = "go"
    Public Const Groovy As String = "groovy"
    Public Const GwBasic As String = "gwbasic"
    Public Const Haskell As String = "haskell"
    Public Const Haxe As String = "haxe"
    Public Const HicEst As String = "hicest"
    Public Const HQ9Plus As String = "hq9plus"
    Public Const HTML As String = "html4strict"
    Public Const HTML5 As String = "html5"
    Public Const Icon As String = "icon"
    Public Const IDL As String = "idl"
    Public Const INIfile As String = "ini"
    Public Const InnoScript As String = "inno"
    Public Const INTERCAL As String = "intercal"
    Public Const IO As String = "io"
    Public Const ISPFPanelDefinition As String = "ispfpanel"
    Public Const J As String = "j"
    Public Const Java As String = "java"
    Public Const Java5 As String = "java5"
    Public Const JavaScript As String = "javascript"
    Public Const JCL As String = "jcl"
    Public Const jQuery As String = "jquery"
    Public Const JSON As String = "json"
    Public Const Julia As String = "julia"
    Public Const KiXtart As String = "kixtart"
    Public Const Latex As String = "latex"
    Public Const LDIF As String = "ldif"
    Public Const LibertyBASIC As String = "lb"
    Public Const LindenScripting As String = "lsl2"
    Public Const Lisp As String = "lisp"
    Public Const LLVM As String = "llvm"
    Public Const LocoBasic As String = "locobasic"
    Public Const Logtalk As String = "logtalk"
    Public Const LOLCode As String = "lolcode"
    Public Const LotusFormulas As String = "lotusformulas"
    Public Const LotusScript As String = "lotusscript"
    Public Const LScript As String = "lscript"
    Public Const Lua As String = "lua"
    Public Const M68000Assembler As String = "m68k"
    Public Const MagikSF As String = "magiksf"
    Public Const Make As String = "make"
    Public Const MapBasic As String = "mapbasic"
    Public Const MatLab As String = "matlab"
    Public Const mIRC As String = "mirc"
    Public Const MIXAssembler As String = "mmix"
    Public Const Modula2 As String = "modula2"
    Public Const Modula3 As String = "modula3"
    Public Const Motorola68000HiSoftDev As String = "68000devpac"
    Public Const MPASM As String = "mpasm"
    Public Const MXML As String = "mxml"
    Public Const MySQL As String = "mysql"
    Public Const Nagios As String = "nagios"
    Public Const NetRexx As String = "netrexx"
    Public Const newLISP As String = "newlisp"
    Public Const Nginx As String = "nginx"
    Public Const Nimrod As String = "nimrod"
    Public Const None As String = "text"
    Public Const NullSoftInstaller As String = "nsis"
    Public Const Oberon2 As String = "oberon2"
    Public Const ObjeckProgrammingLangua As String = "objeck"
    Public Const ObjectiveC As String = "objc"
    Public Const OCalmBrief As String = "ocaml-brief"
    Public Const OCaml As String = "ocaml"
    Public Const Octave As String = "octave"
    Public Const OpenObjectRexx As String = "oorexx"
    Public Const OpenBSDPACKETFILTER As String = "pf"
    Public Const OpenGLShading As String = "glsl"
    Public Const OpenofficeBASIC As String = "oobas"
    Public Const Oracle11 As String = "oracle11"
    Public Const Oracle8 As String = "oracle8"
    Public Const Oz As String = "oz"
    Public Const ParaSail As String = "parasail"
    Public Const PARIGP As String = "parigp"
    Public Const Pascal As String = "pascal"
    Public Const Pawn As String = "pawn"
    Public Const PCRE As String = "pcre"
    Public Const Per As String = "per"
    Public Const Perl As String = "perl"
    Public Const Perl6 As String = "perl6"
    Public Const PHP As String = "php"
    Public Const PHPBrief As String = "php-brief"
    Public Const Pic16 As String = "pic16"
    Public Const Pike As String = "pike"
    Public Const PixelBender As String = "pixelbender"
    Public Const PLI As String = "pli"
    Public Const PLSQL As String = "plsql"
    Public Const PostgreSQL As String = "postgresql"
    Public Const PostScript As String = "postscript"
    Public Const POVRay As String = "povray"
    Public Const PowerShell As String = "powershell"
    Public Const PowerBuilder As String = "powerbuilder"
    Public Const ProFTPd As String = "proftpd"
    Public Const Progress As String = "progress"
    Public Const Prolog As String = "prolog"
    Public Const Properties As String = "properties"
    Public Const ProvideX As String = "providex"
    Public Const Puppet As String = "puppet"
    Public Const PureBasic As String = "purebasic"
    Public Const PyCon As String = "pycon"
    Public Const Python As String = "python"
    Public Const PythonforS60 As String = "pys60"
    Public Const qkdbplus As String = "q"
    Public Const QBasic As String = "qbasic"
    Public Const QML As String = "qml"
    Public Const R As String = "rsplus"
    Public Const Racket As String = "racket"
    Public Const Rails As String = "rails"
    Public Const RBScript As String = "rbs"
    Public Const REBOL As String = "rebol"
    Public Const REG As String = "reg"
    Public Const Rexx As String = "rexx"
    Public Const Robots As String = "robots"
    Public Const RPMSpec As String = "rpmspec"
    Public Const Ruby As String = "ruby"
    Public Const RubyGnuplot As String = "gnuplot"
    Public Const Rust As String = "rust"
    Public Const SAS As String = "sas"
    Public Const Scala As String = "scala"
    Public Const Scheme As String = "scheme"
    Public Const Scilab As String = "scilab"
    Public Const SCL As String = "scl"
    Public Const SdlBasic As String = "sdlbasic"
    Public Const Smalltalk As String = "smalltalk"
    Public Const Smarty As String = "smarty"
    Public Const SPARK As String = "spark"
    Public Const SPARQL As String = "sparql"
    Public Const SQF As String = "sqf"
    Public Const SQL As String = "sql"
    Public Const StandardML As String = "standardml"
    Public Const StoneScript As String = "stonescript"
    Public Const SuperCollider As String = "sclang"
    Public Const Swift As String = "swift"
    Public Const SystemVerilog As String = "systemverilog"
    Public Const TSQL As String = "tsql"
    Public Const TCL As String = "tcl"
    Public Const TeraTerm As String = "teraterm"
    Public Const thinBasic As String = "thinbasic"
    Public Const TypoScript As String = "typoscript"
    Public Const Unicon As String = "unicon"
    Public Const UnrealScript As String = "uscript"
    Public Const UPC As String = "upc"
    Public Const Urbi As String = "urbi"
    Public Const Vala As String = "vala"
    Public Const VBNET As String = "vbnet"
    Public Const VBScript As String = "vbscript"
    Public Const Vedit As String = "vedit"
    Public Const VeriLog As String = "verilog"
    Public Const VHDL As String = "vhdl"
    Public Const VIM As String = "vim"
    Public Const VisualProLog As String = "visualprolog"
    Public Const VisualBasic As String = "vb"
    Public Const VisualFoxPro As String = "visualfoxpro"
    Public Const WhiteSpace As String = "whitespace"
    Public Const WHOIS As String = "whois"
    Public Const Winbatch As String = "winbatch"
    Public Const XBasic As String = "xbasic"
    Public Const XML As String = "xml"
    Public Const XorgConfig As String = "xorg_conf"
    Public Const XPP As String = "xpp"
    Public Const YAML As String = "yaml"
    Public Const Z80Assembler As String = "z80"
    Public Const ZXBasic As String = "zxbasic"
End Class
