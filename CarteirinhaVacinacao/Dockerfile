FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 3306
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "CarteirinhaVacinacao.dll"]