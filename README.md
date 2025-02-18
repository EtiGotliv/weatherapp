# Weather Data Import Service

This project involves a service that imports weather data for random cities using a free API, stores the data in a database, and provides useful weather insights (like min/max values and averages).

## Prerequisites

- **Azure account** (for using Azure resources, including AKS)
- **Azure CLI** installed
- **Docker** installed
- **Kubernetes (AKS)** cluster set up
- **Database** 
- **Git** installed

## Setup Instructions

### 1. Clone the Repository

Clone the project repository to your local machine:
```bash
git clone https://github.com/EtiGotliv/weatherapp.git
cd weatherapp
```

### 2. Build the Docker Image

You will need Docker installed to build and run the service locally. First, build the Docker image:
```bash
docker build -t weatherapp .
```

### 3. Run the Service Locally with Docker

To run the service locally using Docker:
```bash
docker run -p 8080:80 weatherapp 
```

Once the service is running locally, you can access the weather data through:
- `http://localhost:8080/weather`
- `http://localhost:8080/weather/stats`

### 4. Deploy to Kubernetes

Ensure that you have a Kubernetes cluster running in Azure (AKS).

To deploy the service to your Kubernetes cluster, apply the Kubernetes configuration files:
```bash
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml
```

### 5. Database Setup

Make sure the database is running and accessible. The service will write and read weather data from it using native SQL queries.

You need to update the database connection details in the configuration file with the correct credentials.

### 6. External Access to the Service

Once deployed to Kubernetes, you can access the service externally (e.g., from any machine with internet access) using the public IP address of your Kubernetes service:
- `http://9.163.192.44/weather`
- `http://9.163.192.44/weather/stats`


---

## Service Description

- This service imports weather data for random cities for the past 30 days using a free API.
- Data is stored in a SQL database and can be queried for analysis.
- Implemented features include:
  - Min/Max temperature values for each city.
  - Average temperature for a specific parameter (e.g., daily average temperature).
  - Additional useful weather insights.



