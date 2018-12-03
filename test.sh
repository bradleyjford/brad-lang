#!/bin/sh

dotnet watch --project ./src/BradLang.Tests/BradLang.Tests.csproj test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info 
