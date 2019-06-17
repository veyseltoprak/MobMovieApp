#FROM microsoft/dotnet:2.2-sdk AS build
#WORKDIR /src
#COPY MobMovie.csproj MobMovie/
#RUN dotnet restore MobMovie/MobMovie.csproj
#WORKDIR /src/MobMovie
#COPY . .
#RUN dotnet build MobMovie.csproj -c Release -o /app
#
#
FROM microsoft/dotnet:2.2-sdk AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["MobMovie.csproj", "MobMovie/"]
RUN dotnet restore "MobMovie/MobMovie.csproj"
WORKDIR "/src/MobMovie"
COPY . .
RUN dotnet build "MobMovie.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MobMovie.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MobMovie.dll"]