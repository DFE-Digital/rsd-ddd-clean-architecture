ARG ASPNET_SDK_TAG=8.0
ARG DOTNET_EF_TAG=8.0.8
ARG ASPNET_IMAGE_TAG=8.0-bookworm-slim
ARG CI

# ==============================================
# Base SDK
# ==============================================
FROM "mcr.microsoft.com/dotnet/sdk:${ASPNET_SDK_TAG}" AS builder
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
RUN dotnet ef migrations bundle -r linux-x64 --configuration Release -p DfE.DomainDrivenDesignTemplate.Api --no-build -o /sql/migratedb

# ==============================================
# Entity Framework: Migration Runner
# ==============================================
FROM "mcr.microsoft.com/dotnet/aspnet:${ASPNET_IMAGE_TAG}" AS initcontainer
WORKDIR /sql
COPY --from=efbuilder /sql /sql
COPY --from=builder /app/appsettings* /DfE.DomainDrivenDesignTemplate.Api/

# ==============================================
# Application
# ==============================================
FROM "mcr.microsoft.com/dotnet/aspnet:${ASPNET_IMAGE_TAG}" AS final
LABEL org.opencontainers.image.source=https://github.com/DFE-Digital/record-concerns-support-trusts
ARG COMMIT_SHA
COPY --from=builder /app /app
COPY ./script/docker-entrypoint.sh /app/docker-entrypoint.sh
WORKDIR /app
RUN chown -R app:app /app
RUN chmod +x ./docker-entrypoint.sh
USER app
ENV ASPNETCORE_HTTP_PORTS 80
EXPOSE 80/tcp
