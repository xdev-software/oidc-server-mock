# Stage 1: Build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS source
ARG TARGETARCH
ARG target="Release"

WORKDIR /src

COPY ./OpenIdConnectServerMock.csproj ./OpenIdConnectServerMock.csproj
RUN dotnet restore -a $TARGETARCH

COPY . .

RUN dotnet publish -a $TARGETARCH --no-restore -o /app

# Stage 2: Release
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS release
ARG target="Release"

RUN if [ $target = "Debug" ]; then apk add --no-cache curl bash unzip && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg; fi

USER $APP_UID

WORKDIR /app
COPY --from=source /app .

ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080

ENTRYPOINT ["./OpenIdConnectServerMock"]
