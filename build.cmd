@echo off

dotnet build ./BradLang.sln
dotnet test ./src/BradLang.Tests/BradLang.Tests.csproj
