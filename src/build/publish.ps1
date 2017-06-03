param([string]$betaver)

.\build.ps1 $version

if ([string]::IsNullOrEmpty($betaver)) {
	$version = [Reflection.AssemblyName]::GetAssemblyName((resolve-path '..\interface\IParallelTaskQueueRx\bin\Release\netstandard1.2\IParallelTaskQueueRx.dll')).Version.ToString(3)
	}
else {
	$version = [Reflection.AssemblyName]::GetAssemblyName((resolve-path '..\interface\IParallelTaskQueueRx\bin\Release\netstandard1.2\IParallelTaskQueueRx.dll')).Version.ToString(3) + "-" + $betaver
}

NuGet.exe pack ParallelTaskQueueRx.nuspec -Verbosity detailed -Symbols -OutputDir "NuGet" -Version $version

Nuget.exe push .\Nuget\ParallelTaskQueueRx.nuspec.$version.nupkg -Source https://www.nuget.org