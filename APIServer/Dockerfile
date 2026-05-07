# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG NAME="APIServer"

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG NAME
WORKDIR /src

# copy csproj and restore as distinct layers
#COPY *.csproj .
COPY "$NAME/*.csproj" "./$NAME/"
RUN dotnet restore "$NAME/$NAME.csproj"

# copy everything else and build app
COPY "$NAME/." "./$NAME/"
WORKDIR "/src/$NAME"
RUN dotnet publish -c release -o /app --no-restore

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
ARG NAME
WORKDIR /app
COPY --from=build /app ./

ENV RunName="$NAME.dll"
ENTRYPOINT dotnet $RunName
