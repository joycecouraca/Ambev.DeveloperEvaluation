@echo off
setlocal

set PROJECT=tests/Ambev.DeveloperEvaluation.Unit/Ambev.DeveloperEvaluation.Unit.csproj

echo Executando testes com cobertura...

dotnet test %PROJECT% --collect:"XPlat Code Coverage"

echo.
echo Gerando relatório com ReportGenerator...

reportgenerator ^
 -reports:"**/coverage.cobertura.xml" ^
 -targetdir:coveragereport ^
 -reporttypes:Html

echo.
echo Abrindo no navegador...

start coveragereport\index.htm