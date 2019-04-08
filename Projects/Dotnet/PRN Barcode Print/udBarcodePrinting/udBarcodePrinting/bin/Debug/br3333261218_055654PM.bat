set MyPath=%cd%
echo %MyPath%
set PCName=%ComputerName%

FOR /F "tokens=2* delims==" %%A in (
  'wmic printer where "default=True" get sharename /value'
  ) do SET DefaultPrinter=%%A
set PrinterShareName=%DefaultPrinter%

Copy  "%MyPath%\_PRN\br3333261218_055654PM.prn" \\%PCName%\%PrinterShareName%
