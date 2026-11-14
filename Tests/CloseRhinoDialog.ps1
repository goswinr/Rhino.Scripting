<#
.SYNOPSIS
  Lists, and optionally closes, the dialog windows of a running Rhino. Use it when Rhino looks frozen but still says "Responding",
  e.g. when a RhinoCommon call opened a modal message box that is hidden behind the main window.

.EXAMPLE
  powershell -File CloseRhinoDialog.ps1                 # only lists the windows of Rhino
  powershell -File CloseRhinoDialog.ps1 -Close          # sends WM_CLOSE to every owned dialog window (class #32770)
  powershell -File CloseRhinoDialog.ps1 -Hwnd 0x60A22   # sends WM_CLOSE to this one window only
#>
param(
    [switch]$Close,
    [string]$Hwnd = "",
    [string]$ProcessName = "Rhino"
)

Add-Type @"
using System; using System.Text; using System.Runtime.InteropServices;
public class W {
  public delegate bool EnumProc(IntPtr h, IntPtr l);
  [DllImport("user32.dll")] public static extern bool EnumWindows(EnumProc p, IntPtr l);
  [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
  [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr h);
  [DllImport("user32.dll")] public static extern int GetWindowText(IntPtr h, StringBuilder s, int n);
  [DllImport("user32.dll")] public static extern int GetClassName(IntPtr h, StringBuilder s, int n);
  [DllImport("user32.dll")] public static extern IntPtr GetWindow(IntPtr h, uint cmd);
  [DllImport("user32.dll")] public static extern bool PostMessage(IntPtr h, uint m, IntPtr w, IntPtr l);
}
"@

$procs = Get-Process -Name $ProcessName -ErrorAction SilentlyContinue
if (-not $procs) { Write-Host "No process named '$ProcessName' found."; exit 1 }
$pids = @($procs | ForEach-Object { $_.Id })
Write-Host ("Process ids: " + ($pids -join ", "))

$WM_CLOSE = 0x0010
$GW_OWNER = 4
$found = New-Object System.Collections.ArrayList

[void][W]::EnumWindows({ param($h, $l)
    $p = 0
    [void][W]::GetWindowThreadProcessId($h, [ref]$p)
    if ($pids -contains [int]$p) {
        $t = New-Object Text.StringBuilder 256
        $c = New-Object Text.StringBuilder 256
        [void][W]::GetWindowText($h, $t, 256)
        [void][W]::GetClassName($h, $c, 256)
        [void]$found.Add([pscustomobject]@{
            Handle  = ("0x{0:X}" -f [int64]$h)
            Pid     = $p
            Visible = [W]::IsWindowVisible($h)
            Owner   = ("0x{0:X}" -f [int64][W]::GetWindow($h, $GW_OWNER))
            Class   = $c.ToString()
            Title   = $t.ToString()
            Raw     = $h
        })
    }
    return $true
}, [IntPtr]::Zero)

$found | Select-Object Handle, Pid, Visible, Owner, Class, Title | Format-Table -AutoSize

if ($Hwnd -ne "") {
    $target = [IntPtr][Convert]::ToInt64($Hwnd, 16)
    Write-Host "Sending WM_CLOSE to $Hwnd"
    [void][W]::PostMessage($target, $WM_CLOSE, [IntPtr]::Zero, [IntPtr]::Zero)
}
elseif ($Close) {
    # Owned windows of class #32770 are standard dialogs and message boxes. The Rhino main window and its tool windows are not touched.
    $dialogs = $found | Where-Object { $_.Class -eq "#32770" -and $_.Owner -ne "0x0" }
    if (-not $dialogs) { Write-Host "No owned dialog windows (#32770) found. Use the table above and pass -Hwnd." }
    foreach ($d in $dialogs) {
        Write-Host ("Sending WM_CLOSE to {0} '{1}'" -f $d.Handle, $d.Title)
        [void][W]::PostMessage($d.Raw, $WM_CLOSE, [IntPtr]::Zero, [IntPtr]::Zero)
    }
}
else {
    Write-Host "Nothing closed. Re-run with -Close or -Hwnd <handle>."
}
