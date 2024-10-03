ARG PROJECT_NAME="DfE.DomainDrivenDesignTemplate.Api"
ARG REPO_ORIGIN="https://github.com/DFE-Digital/rsd-ddd-clean-architecture"
ARG DOTNET_SDK_TAG=8.0
ARG DOTNET_EF_TAG=8.0.8
ARG DOTNET_ASPNET_TAG=8.0-bookworm-slim
ARG NUGET_SOURCE="https://nuget.pkg.github.com/DFE-Digital/index.json"
ARG CI

# ==============================================
# Base SDK
# ==============================================
FROM "mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_TAG}" AS builder
ENV CI=${CI}
WORKDIR /build
COPY . .
RUN --mount=type=secret,id=github_token dotnet nuget add source --username USERNAME --password $(cat /run/secrets/github_token) --store-password-in-clear-text --name github ${NUGET_SOURCE}
RUN dotnet restore ${PROJECT_NAME}
RUN dotnet build -c Release ${PROJECT_NAME} -p:CI=${CI}
RUN dotnet publish ${PROJECT_NAME} -c Release -o /app --no-build

# ==============================================
# Entity Framework: Migration Builder
# ==============================================
FROM builder AS efbuilder
WORKDIR /build
ENV PATH=$PATH:/root/.dotnet/tools
RUN dotnet tool install --global dotnet-ef  --version ${DOTNET_EF_TAG}
RUN mkdir /sql
RUN dotnet ef migrations bundle -r linux-x64 --configuration Release -p ${PROJECT_NAME} --no-build -o /sql/migratedb

# ==============================================
# Entity Framework: Migration Runner
# ==============================================
FROM "mcr.microsoft.com/dotnet/aspnet:${DOTNET_ASPNET_TAG}" AS initcontainer
WORKDIR /sql
COPY --from=efbuilder /sql /sql
COPY --from=builder /app/appsettings* /${PROJECT_NAME}/

# ==============================================
# Application
# ==============================================
FROM "mcr.microsoft.com/dotnet/aspnet:${DOTNET_ASPNET_TAG}" AS final
LABEL org.opencontainers.image.source=${REPO_ORIGIN}
ARG COMMIT_SHA
COPY --from=builder /app /app
COPY ./script/docker-entrypoint.sh /app/docker-entrypoint.sh
WORKDIR /app
RUN chown -R app:app /app
RUN chmod +x ./docker-entrypoint.sh
USER app
ENV ASPNETCORE_HTTP_PORTS 80
EXPOSE 80/tcp
