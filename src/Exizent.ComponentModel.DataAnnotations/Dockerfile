ARG CONFIGURATION="Release"
ARG NUGET_PACKAGE_VERSION="1.0.0"

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore

ARG CONFIGURATION
ENV PATH="/root/.dotnet/tools:${PATH}"

COPY ./*.props .
COPY ./*.targets .
COPY ./*.sln .
COPY ./*.png .
COPY ./global.json .

COPY ./src/Exizent.ComponentModel.DataAnnotations/*.csproj ./src/Exizent.ComponentModel.DataAnnotations/
COPY ./tests/Exizent.ComponentModel.DataAnnotations.Tests/*.csproj ./tests/Exizent.ComponentModel.DataAnnotations.Tests/
RUN dotnet restore

FROM restore as build
ARG CONFIGURATION
ARG NUGET_PACKAGE_VERSION

COPY ./src/ ./src/
COPY ./tests/ ./tests/
RUN dotnet build --configuration $CONFIGURATION --no-restore

FROM build as test
RUN dotnet test --logger "trx;LogFilePath=/TestResults/TestResults.trx" --configuration $CONFIGURATION --no-build

FROM build as pack
RUN mkdir -p artifacts
RUN dotnet pack --configuration Release -p:Version=${NUGET_PACKAGE_VERSION} --no-build --output ./artifacts

FROM scratch
COPY --from=pack /artifacts/*.nupkg /artifacts/
COPY --from=pack /artifacts/*.snupkg /artifacts/
COPY --from=test /TestResults/*.trx /TestResults/
