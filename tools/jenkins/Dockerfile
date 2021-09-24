# https://stackoverflow.com/questions/48104954/adding-net-core-to-docker-container-with-jenkins
FROM jenkins/jenkins:lts

# Switch to root to install .NET Core SDK
USER root

# Just for my sanity... Show me this distro information!
RUN uname -a && cat /etc/*release

# Based on instructiions at https://docs.microsoft.com/en-us/dotnet/core/install/linux
# Install depency for dotnet core 5.0
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    curl libunwind8 gettext apt-transport-https && \
    curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg && \
    mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg && \
    sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-debian-stretch-prod stretch main" > /etc/apt/sources.list.d/dotnetdev.list' && \
    apt-get update

# Install the .Net Core framework, set the path, and show the version of core installed.
RUN apt-get install -y dotnet-sdk-5.0 && \
    export PATH=$PATH:$HOME/dotnet && \
    dotnet --version
	
# Install Python & NodeJs
RUN apt-get install -y build-essential python nodejs && \
    python --version && node --version

# Good idea to switch back to the jenkins user.
USER jenkins