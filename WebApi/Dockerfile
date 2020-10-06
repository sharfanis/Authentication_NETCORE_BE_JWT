# Get base SDK Image from microosoft
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app


# COPY the csproj file and restore any dependencies (via nuget )
COPY *.csproj ./
RUN dotnet restore

# COPY the project files and build our release
COPY . ./
RUN dotnet publish -c Release -o out

# Generate runtime image
FROM mcr.microosoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "DockerAPI.dll" ]

