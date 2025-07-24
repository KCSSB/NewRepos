#First stage(stage for build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

#Second stage (dependecy recovercy stage)
COPY ["API/API.csproj", "API/"]
COPY ["DataBaseInfo/DataBaseInfo.csproj", "DataBaseInfo/"]
RUN dotnet restore "API/API.csproj"

#Third stage(COPY ALL FILES)

COPY . .
RUN dotnet publish "API/API.csproj" -c Release -o /app/publish 
#Stage RunTime

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]