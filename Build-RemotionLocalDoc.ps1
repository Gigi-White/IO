set-alias nant "prereq\Tools\NAnt\bin.net-2.0\nant.exe";

nant "-f:Remotion.build" "-D:solution.global-dir=\Development\global" "-D:build.temp.root=\Temp\RemotionLocal" "-t:net-3.5" "-nologo" `
    "-D:build.update.assembly-info=false" `
    clean cleantemp `
    doc-internal;

if ($LastExitCode -ne 0) 
{ 
  [System.Console]::ReadKey($false);
  throw "Build Remotion has failed."; 
}

[System.Console]::ReadKey($false);
