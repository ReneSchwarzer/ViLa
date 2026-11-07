# syntax=docker/dockerfile:1
FROM scratch AS nuget

FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /src
COPY --from=nuget . /nuget/
COPY docker/nuget.config ./
COPY src/ViLa.App/ViLa.App.csproj ViLa.App/
COPY src/ViLa.App/ViLa.ContainerProfiles.targets ViLa.App/
COPY src/ViLa/ViLa.csproj ViLa/
RUN dotnet restore ViLa.App/ViLa.App.csproj --configfile nuget.config
COPY src/ViLa.App/ ViLa.App/
COPY src/ViLa/ ViLa/
RUN dotnet publish ViLa.App/ViLa.App.csproj -c Release --no-restore -p:UseAppHost=false -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS final
RUN apt-get update && apt-get install -y --no-install-recommends libgpiod2t64 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /publish/ ./
RUN mkdir -p data/measurements assets packages Config && ln -s data/measurements measurements && chown -R app:app data assets packages Config settings
ENV WEBEXPRESS_WebExpress__Endpoints__0__Uri=http://0.0.0.0:8080/ \
    WEBEXPRESS_WebExpress__ExternalUri=http://localhost:8080/ \
    WEBEXPRESS_WebExpress__Shutdown=graceful
USER app
EXPOSE 8080
VOLUME ["/app/data", "/app/Config", "/app/settings", "/app/assets", "/app/packages"]
HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=3 \
    CMD ["timeout", "4", "bash", "-c", "exec 3<>/dev/tcp/127.0.0.1/8080 && printf 'GET /health HTTP/1.1\\r\\nHost: localhost\\r\\nConnection: close\\r\\n\\r\\n' >&3 && head -n 1 <&3 | grep -q '^HTTP/1\\.[01] 200 '"]
ENTRYPOINT ["dotnet", "ViLa.App.dll"]
