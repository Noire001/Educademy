ARG NODE_IMAGE=node:19.0

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80


FROM ${NODE_IMAGE} as node-build
WORKDIR /web/src
COPY Web/WebSPA/Client/package.json .
COPY Web/WebSPA/Client/package-lock.json .
RUN npm install
COPY Web/WebSPA/Client .
RUN npm run build --prod

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
COPY --from=node-build /web/src/build /src/Web/WebSPA/wwwroot/
WORKDIR "/src/Web/WebSPA"
RUN dotnet publish "WebSPA.csproj" -c Release -o /app/publish

FROM build AS publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebSPA.dll"]
