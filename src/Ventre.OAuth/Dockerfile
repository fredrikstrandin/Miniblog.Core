FROM microsoft/aspnetcore:2.0
LABEL manitainer="strandin72@gmail.com"
ARG source
WORKDIR /app
EXPOSE 5000
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "Vivus.OAuth.dll"]
