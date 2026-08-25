FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore CSharp-PKI-Infrastructure.csproj \
 && dotnet publish CSharp-PKI-Infrastructure.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
USER 1654
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sky.X509Lab.dll"]
