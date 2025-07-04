# Set the major version of dotnet
ARG DOTNET_VERSION=8.0

# ==============================================
# .NET SDK: Build
# ==============================================
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-azurelinux3.0 AS build
WORKDIR /build
ARG CI
ENV CI=${CI}

# Mount GitHub Token as a Docker secret so that NuGet Feed can be accessed
RUN --mount=type=secret,id=github_token dotnet nuget add source --username USERNAME --password $(cat /run/secrets/github_token) --store-password-in-clear-text --name github "https://nuget.pkg.github.com/DFE-Digital/index.json"

# Copy the application code
COPY ./src/ ./src/
COPY Directory.Build.props ./
COPY DfE.DomainDrivenDesignTemplate.sln ./

# Build and publish the dotnet solution
RUN dotnet restore DfE.DomainDrivenDesignTemplate.sln && \
    dotnet build ./src/DfE.DomainDrivenDesignTemplate.Api --no-restore -c Release && \
    dotnet publish ./src/DfE.DomainDrivenDesignTemplate.Api --no-build -o /app

# ==============================================
# Entity Framework: Migration Builder
# ==============================================
FROM build AS efbuilder
WORKDIR /build
ARG DOTNET_EF_TAG=8.0.8

ENV PATH=$PATH:/root/.dotnet/tools
RUN dotnet tool install --global dotnet-ef
RUN mkdir /sql
RUN dotnet ef migrations bundle -r linux-x64 \
      --configuration Release \
      --project ./src/DfE.DomainDrivenDesignTemplate.Api \
      --no-build -o /sql/migratedb

# ==============================================
# Entity Framework: Migration Runner
# ==============================================
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-azurelinux3.0 AS initcontainer
WORKDIR /sql
COPY --from=efbuilder /sql /sql
COPY --from=build /app/appsettings* /DfE.DomainDrivenDesignTemplate.Api/

# ==============================================
# .NET Runtime: Publish
# ==============================================
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-azurelinux3.0 AS final
WORKDIR /app
LABEL org.opencontainers.image.source="https://github.com/DFE-Digital/rsd-ddd-clean-architecture"
LABEL org.opencontainers.image.description="DfE.DomainDrivenDesignTemplate"

COPY --from=build /app /app
COPY ./script/docker-entrypoint.sh /app/docker-entrypoint.sh
RUN chmod +x ./docker-entrypoint.sh

USER $APP_UID
