FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

# Copy everything else and build
COPY Creadth.Talespire.DungeonGenerator ./Creadth.Talespire.DungeonGenerator
RUN dotnet publish Creadth.Talespire.DungeonGenerator/*.csproj -c Release -o out

FROM node:12.2.0 AS ng
WORKDIR /client
COPY --from=build-env /Creadth.Talespire.DungeonGenerator/client .
RUN npm i
RUN npm i -g @angular/cli@8.3.2
RUN ng build --prod


# Build runtime image
WORKDIR /
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY --from=build-env ./out .
COPY --from=ng /client/dist /client/dist
ENTRYPOINT ["dotnet", "Creadth.Talespire.DungeonGenerator.dll"]
