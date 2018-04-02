FROM microsoft/aspnetcore:2.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore CarteirinhaVacinacao

ARG source

COPY . ./
RUN dotnet publish -c Release -o out

WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore:2.0
WORKDIR /app

COPY --from=build-env /app/CarteirinhaVacinacao/out ./

ENTRYPOINT ["dotnet", "CarteirinhaVacinacao.dll"]
