$ProjectName = Read-Host "Enter Project Name"
$TemplateName = "PostgreAPI-Template"
$Namespace = "PostgreAPI_Template"
$JwtKey = "qwertyuiop"

#solution and project
(Get-Content "$TemplateName.sln").Replace("$TemplateName", "$ProjectName") | Set-Content "$TemplateName.sln"
Rename-Item -Path "$TemplateName.sln" -NewName "$ProjectName.sln"
Rename-Item -Path ".\$TemplateName\$TemplateName.csproj" -NewName "$ProjectName.csproj"

#update docker
(Get-Content ".\docker-compose.override.yml").Replace("$TemplateName", "$ProjectName") | Set-Content ".\docker-compose.override.yml"
(Get-Content ".\docker-compose.yml").Replace("$TemplateName", "$ProjectName") | Set-Content ".\docker-compose.yml"
(Get-Content ".\$TemplateName\Dockerfile").Replace("$TemplateName", "$ProjectName") | Set-Content ".\$TemplateName\Dockerfile"

#update code
(Get-Content ".\$TemplateName\program.cs").Replace("$Namespace", "$ProjectName") | Set-Content ".\$TemplateName\program.cs"
(Get-Content ".\$TemplateName\appsettings.json").Replace("jwt-key", "$JwtKey") | Set-Content ".\$TemplateName\appsettings.json"
Rename-Item -Path ".\$TemplateName\$TemplateName.http" -NewName "$ProjectName.http"

#project folder
Rename-Item -Path "$TemplateName" -NewName "$ProjectName"

#clean up
Remove-Item -Path ".\RenameProject.ps1"