FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
COPY . /src
WORKDIR /src
RUN ls
RUN dotnet build "Melpominee.app.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Melpominee.app.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Melpominee.app.dll"]