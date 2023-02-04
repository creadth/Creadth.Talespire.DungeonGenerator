FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# Copy everything else and build
COPY Creadth.Talespire.DungeonGenerator ./Creadth.Talespire.DungeonGenerator
RUN dotnet restore Creadth.Talespire.DungeonGenerator/*.csproj
RUN dotnet publish Creadth.Talespire.DungeonGenerator/*.csproj -c Release -o out

FROM node:18.14.0 AS ng
WORKDIR /client
COPY --from=build-env /Creadth.Talespire.DungeonGenerator/client .
RUN npm i
RUN npm i -g @angular/cli@15.1.4
RUN ng build --configuration=production


# Build runtime image
WORKDIR /
FROM mcr.microsoft.com/dotnet/aspnet:7.0.2
COPY --from=build-env ./out .
COPY --from=ng /client/dist /client/dist
ENTRYPOINT ["dotnet", "Creadth.Talespire.DungeonGenerator.dll"]
