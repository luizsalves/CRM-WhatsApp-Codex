FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY NuGet.Config ./
COPY backend/WhatsCrm.Api/WhatsCrm.Api.csproj backend/WhatsCrm.Api/
RUN dotnet restore backend/WhatsCrm.Api/WhatsCrm.Api.csproj --configfile NuGet.Config

COPY backend/WhatsCrm.Api/ backend/WhatsCrm.Api/
WORKDIR /src/backend/WhatsCrm.Api
RUN dotnet publish -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "WhatsCrm.Api.dll"]
