FROM microsoft/aspnetcore:2.0.0

RUN apt-get update && apt-get install -y librdkafka-dev

WORKDIR /app
COPY out .
EXPOSE 9999
ENTRYPOINT ["dotnet", "Kontena.Dashboard.dll"]
