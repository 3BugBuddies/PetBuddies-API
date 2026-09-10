FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY PetBuddies-API.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
RUN addgroup -S petbuddies && adduser -S petbuddies -G petbuddies
COPY --from=build /app .
USER petbuddies
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 TZ=America/Sao_Paulo
ENTRYPOINT ["dotnet", "PetBuddies-API.dll"]
