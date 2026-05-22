FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY PetBuddies-API/PetBuddies-API.csproj PetBuddies-API/
RUN dotnet restore PetBuddies-API/PetBuddies-API.csproj
COPY . .
RUN dotnet publish PetBuddies-API/PetBuddies-API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
RUN addgroup --system app && adduser --system --ingroup app app
COPY --from=build /app/publish .
USER app
EXPOSE 80
ENTRYPOINT ["dotnet", "PetBuddies-API.dll"]
