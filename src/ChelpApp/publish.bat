if exist "Chelp.zip" (
    del Chelp.zip
)
dotnet publish ChelpApp.csproj -c Release --self-contained true
7z a -mx9 Chelp.zip C:\Users\nikita\Documents\GitHub\Chelp\src\ChelpApp\bin\Release\net9.0-windows\win-x64
