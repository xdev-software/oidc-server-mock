# Stage 1: Build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS source
ARG TARGETARCH
ARG target="Release"

RUN apk add --no-cache unzip curl bash

WORKDIR /src

COPY ./OpenIdConnectServerMock.csproj ./OpenIdConnectServerMock.csproj
RUN dotnet restore -a $TARGETARCH

COPY . .

RUN dotnet publish -a $TARGETARCH -c $target -o obj/docker/publish

RUN cp -r /src/obj/docker/publish /OpenIdConnectServerMock

# Stage 2: Release
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS release
ARG target="Release"

RUN apk add --no-cache curl
RUN if [ $target = "Debug" ]; then apk add --no-cache bash unzip && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg; fi

COPY --from=source /OpenIdConnectServerMock /OpenIdConnectServerMock
WORKDIR /OpenIdConnectServerMock

ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 80
EXPOSE 443

HEALTHCHECK --start-period=2s --interval=5s --timeout=1s --retries=10 \
      CMD curl -k --location https://localhost/health || exit 1

ENTRYPOINT ["dotnet", "OpenIdConnectServerMock.dll" ]
