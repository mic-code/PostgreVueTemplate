$ProjectName = Read-Host "Enter Project Name"
$TemplateName = "PostgreVueTemplate"
$TemplateFrontend = "PostgreVueTemplate-Frontend"
$BackendName = "APIServer"
$NewBackendName = "$ProjectName-API"
$Namespace = "PostgreVue_Template"
$JwtKey = "qwertyuiop"

# --- Solution file ---
# Update project name and path references inside the .sln
(Get-Content "$TemplateName.sln").Replace("$BackendName", "$NewBackendName") | Set-Content "$TemplateName.sln"
Rename-Item -Path "$TemplateName.sln" -NewName "$ProjectName.sln"

# --- Backend project ---
Rename-Item -Path ".\$BackendName\$BackendName.csproj" -NewName "$NewBackendName.csproj"

# update docker
(Get-Content ".\docker-compose.override.yml").Replace("postgreapi-template", "$ProjectName".ToLower()) | Set-Content ".\docker-compose.override.yml"
(Get-Content ".\docker-compose.yml").Replace("postgreapi-template", "$ProjectName".ToLower()).Replace("postgreapitemplate", "$ProjectName".ToLower()).Replace("postgreAPI-network", "$ProjectName-network").Replace("$BackendName", "$NewBackendName") | Set-Content ".\docker-compose.yml"
(Get-Content ".\docker-compose.dcproj").Replace("postgre-api-template", "$ProjectName".ToLower()) | Set-Content ".\docker-compose.dcproj"
(Get-Content ".\$BackendName\Dockerfile").Replace("$BackendName", "$NewBackendName") | Set-Content ".\$BackendName\Dockerfile"

# update code
(Get-Content ".\$BackendName\Program.cs").Replace("$Namespace", "$ProjectName") | Set-Content ".\$BackendName\Program.cs"
(Get-Content ".\$BackendName\appsettings.json").Replace("jwt-key", "$JwtKey") | Set-Content ".\$BackendName\appsettings.json"
(Get-Content ".\$BackendName\appsettings.Development.json").Replace("PostgreVueTemplateDB", "${ProjectName}DB") | Set-Content ".\$BackendName\appsettings.Development.json"
Rename-Item -Path ".\$BackendName\$BackendName.http" -NewName "$NewBackendName.http"

# backend project folder
Rename-Item -Path "$BackendName" -NewName "$NewBackendName"

# --- Frontend ---
# update content references
(Get-Content ".\$TemplateFrontend\index.html").Replace("Frontend", "$ProjectName") | Set-Content ".\$TemplateFrontend\index.html"
(Get-Content ".\$TemplateFrontend\package.json").Replace('"frontend"', "`"$ProjectName`"").Replace('"Frontend"', "`"$ProjectName`"") | Set-Content ".\$TemplateFrontend\package.json"

# frontend folder
Rename-Item -Path "$TemplateFrontend" -NewName "$ProjectName-Frontend"

# --- Clean up ---
Remove-Item -Path ".\RenameProject.ps1"
