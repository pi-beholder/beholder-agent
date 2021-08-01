if [ "${TARGETPLATFORM}" = "linux/arm64" ]; then
    echo "Targeting linux/arm64";
    dotnet build "beholder-stalk.csproj" -c Release -o /app/build --runtime linux-arm64
elif [ "${TARGETPLATFORM}" = "linux/arm/v7" ]; then
    echo "Targeting linux/arm/v7";
    dotnet build "beholder-stalk.csproj" -c Release -o /app/build --runtime linux-arm
elif [ "${TARGETPLATFORM}" = "linux/amd64" ]; then
    echo "Targeting linux/amd64";
    dotnet build "beholder-stalk.csproj" -c Release -o /app/build --runtime linux-x64
else then
    echo "Targeting generic platform";
    dotnet build "beholder-stalk.csproj" -c Release -o /app/build
fi