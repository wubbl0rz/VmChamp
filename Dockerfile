FROM rockylinux:8 AS build
WORKDIR /App

RUN ulimit -n 1024 && yum -y update 
RUN ulimit -n 1024 && yum -y install curl zlib-devel
RUN ulimit -n 1024 && yum -y groupinstall 'Development Tools'
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin -c 8.0
COPY *.cs ./
COPY *.csproj ./
ARG DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ARG TARGET_VERSION
RUN ~/.dotnet/dotnet publish -r linux-x64 --configuration Release -p:Version=$TARGET_VERSION --self-contained=true -p:PublishAot=true -p:StripSymbols=true -o build

FROM scratch as output
COPY --from=build /App/build/VmChamp /VmChamp

FROM build as release
COPY --from=build /App/build/VmChamp /VmChamp

