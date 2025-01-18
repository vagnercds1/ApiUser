
.This application is designed for managing user registration on any web platform.
It allows new users to be registered by providing an email and password for later authentication.
The Update, Get, and Delete options require authentication, which is obtained through the Login API.

.To run the .NET 8 application locally, simply open the solution file ApiUser\ApiUser.sln

.The database used is MongoDB, and to start it, you need to have 'Docker Desktop' installed. 
Open 'Windows PowerShell' in the root folder of the application and execute the command below:

docker-compose up -d

.For stop you can use 

docker-compose down

.The docker-compose gets up a container with mongo-express, which will create the database UserDb after the first user inserts request
http://localhost:8081

.To assist with application testing, import the collection UserApi.postman_collection.json into Postman.

Thank you,
Vagner Correa dos Santos