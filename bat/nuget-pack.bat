del %CD%\unpkgs\*.nupkg
dotnet pack -c release -o %CD%\unpkgs
for /r . %%a in (unpkgs\*.nupkg) do (
	if "%1"=="" (
		md "d:\\nuget"
 		dotnet nuget push "%%a" -s "d:\\nuget"
	) else (
		set "Key=%1" 
		if "%Key:~1,1%"==":" (
			md "%1%"
			dotnet nuget push "%%a" -s "%1%"
		) else (
			dotnet nuget push "%%a" -s https://api.nuget.org/v3/index.json -k %1%
		)
	)
)
del %CD%\unpkgs\*.nupkg
