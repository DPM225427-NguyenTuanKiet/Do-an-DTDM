# Build Stage (Đã đổi sang bản 10.0)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy file csproj từ thư mục con vào
COPY WebApplication1/*.csproj ./WebApplication1/
RUN dotnet restore WebApplication1/*.csproj

# Copy toàn bộ code vào và build
COPY . .
WORKDIR /src/WebApplication1
RUN dotnet publish -c Release -o /app/out

# Run Stage (Đã đổi sang bản 10.0)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Đảm bảo tên DLL này khớp với dự án của bạn
ENTRYPOINT ["dotnet", "WebApplication1.dll"]