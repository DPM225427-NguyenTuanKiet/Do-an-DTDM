# Stage 1: Build
FROM [mcr.microsoft.com/dotnet/sdk:8.0](https://mcr.microsoft.com/dotnet/sdk:8.0) AS build
WORKDIR /src

# Copy file project và restore 
COPY WebApplication1.csproj ./
RUN dotnet restore

# Copy toàn bộ code và build
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Stage 2: Run
FROM [mcr.microsoft.com/dotnet/aspnet:8.0](https://mcr.microsoft.com/dotnet/aspnet:8.0)
WORKDIR /app
COPY --from=build /app/out .

# Render yêu cầu Port 10000 hoặc tự động map
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Lệnh chạy 
ENTRYPOINT ["dotnet", "WebApplication1.dll"]