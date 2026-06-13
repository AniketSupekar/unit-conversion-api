#!/bin/bash
# Run this script ONCE to scaffold the .NET solution on your machine
# Usage: bash setup.sh

set -e

echo "Creating solution..."
dotnet new sln -n UnitConversionApi

echo "Creating API project..."
dotnet new webapi -n UnitConversionApi -o src/UnitConversionApi --no-openapi false

echo "Creating test project..."
dotnet new xunit -n UnitConversionApi.Tests -o tests/UnitConversionApi.Tests

echo "Adding projects to solution..."
dotnet sln add src/UnitConversionApi/UnitConversionApi.csproj
dotnet sln add tests/UnitConversionApi.Tests/UnitConversionApi.Tests.csproj

echo "Adding project reference from tests to API..."
cd tests/UnitConversionApi.Tests
dotnet add reference ../../src/UnitConversionApi/UnitConversionApi.csproj
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package FluentAssertions
cd ../..

echo ""
echo "Done! Now copy all the source files into the correct folders."
echo "Then run: cd src/UnitConversionApi && dotnet run"
