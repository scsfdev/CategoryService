# CategoryService/Dockerfile

# ---------- build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# ---------- 1. Build ValidateCategoryEvents ----------
COPY ValidateCategoryEvents/ValidateCategoryEvents.sln ValidateCategoryEvents/
COPY ValidateCategoryEvents/ValidateCategoryEvents/ValidateCategoryEvents.csproj ValidateCategoryEvents/ValidateCategoryEvents/
COPY ValidateCategoryEvents/ValidateCategoryEvents/ ValidateCategoryEvents/ValidateCategoryEvents/

# Build ValidateCategoryEvents
RUN dotnet publish ValidateCategoryEvents/ValidateCategoryEvents/ValidateCategoryEvents.csproj -c Release -o ValidateCategoryEvents/ValidateCategoryEvents/bin/Release/net9.0 --no-self-contained

# ---------- 2. Build CategoryService ----------
# Copy solution and project files for restore caching
COPY CategoryService/CategoryService.sln CategoryService/
COPY CategoryService/CategoryService.Api/CategoryService.Api.csproj CategoryService/CategoryService.Api/
COPY CategoryService/CategoryService.Application/CategoryService.Application.csproj CategoryService/CategoryService.Application/
COPY CategoryService/CategoryService.Domain/CategoryService.Domain.csproj CategoryService/CategoryService.Domain/
COPY CategoryService/CategoryService.Infrastructure/CategoryService.Infrastructure.csproj CategoryService/CategoryService.Infrastructure/

# Restoring API will auto restore related dependencies.
RUN dotnet restore CategoryService/CategoryService.Api/CategoryService.Api.csproj

# Copy rest of source
COPY CategoryService/CategoryService.Api/ CategoryService/CategoryService.Api/
COPY CategoryService/CategoryService.Application/ CategoryService/CategoryService.Application/
COPY CategoryService/CategoryService.Domain/ CategoryService/CategoryService.Domain/
COPY CategoryService/CategoryService.Infrastructure/ CategoryService/CategoryService.Infrastructure/

# Publish
RUN dotnet publish CategoryService/CategoryService.Api/CategoryService.Api.csproj -c Release -o /app --no-restore


# ---------- runtime stage ----------
# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# 1. Add a non-root user named 'appuser'
RUN adduser --disabled-password --gecos "" --no-create-home appuser

WORKDIR /app

# 2. Change ownership of the /app directory to the new user.
RUN chown -R appuser:appuser /app

# 3. Copy published output
COPY --from=build /app ./

# 4. Expose the port.
EXPOSE 8080

# 5. Switch to the non-root user
USER appuser

# 6. Set the final entrypoint
ENTRYPOINT ["dotnet", "CategoryService.Api.dll"]
