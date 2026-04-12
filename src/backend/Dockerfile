FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Restore layer: only .csproj files, max cache hits
COPY src/backend/Melpominee.app.csproj src/backend/
RUN dotnet restore src/backend/Melpominee.app.csproj
# Build layer
COPY src/backend/ src/backend/
RUN dotnet publish src/backend/Melpominee.app.csproj \
    -c Release -r linux-x64 --no-self-contained \
    -p:PublishReadyToRun=true \
    -p:PublishReadyToRunComposite=true \
    -p:PublishSingleFile=true \
    -o /app/publish

# Microsoft chiseled (distroless) runtime: smaller than alpine, default
# non-root `app` user (uid 64198), no shell, no package manager. Faster
# container start because there's less filesystem to mount and fewer
# system libraries to load. Compatible with InvariantGlobalization=true
# (no ICU/locales needed).
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app
COPY --from=build --chown=app:app /app/publish .
USER app
ENV DOTNET_TieredPGO=1 \
    DOTNET_ReadyToRun=1 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true \
    DOTNET_EnableDiagnostics=0 \
    ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["./Melpominee.app"]
