# Owasp Top 10 Target Machine

A Web Project for learning [Owasp Top 10 2021](https://owasp.org/Top10/) Vulnerability

## File Structure
- `src/` Source code
- `docker/` Docker configuration
- `PoC/` Vulnerability exploit example

## Installation

### Prerequisites

- .NET core 6.0 SDK: [ https://dotnet.microsoft.com/en-us/download/dotnet/6.0]( https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Docker: [https://www.docker.com/](https://www.docker.com/)

### Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/username/project-name.git
   ```
2. Install `Prerequisites`
3. Compose up database:
    ```bash
    cd docker
    cp .envSample .env
    docker-compose up -d
    ```
4. Run the project:
    ```bash
    cd src
    dotnet run
    ```

## Disclaimer

This repository and its contents are intended for educational and research purposes only. The information provided here is meant to help understand security vulnerabilities and improve the security of systems. Unauthorized access to computer systems, networks, or devices is illegal and unethical. The authors and contributors are not responsible for any misuse or damage caused by the use of the information provided in this repository. Use this information responsibly and only on systems for which you have explicit permission.

By using this repository, you agree to comply with all applicable local, state, national, and international laws and regulations.