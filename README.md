
This application is designed for managing user registration on any web platform.

It allows new users to be registered by providing an email and password for later authentication.

The Update, Get, and Delete options require authentication, which is obtained through the Login API.

To run the .NET 8 application locally, simply open the solution file ApiUser\ApiUser.sln.

The database used is MongoDB, and to start it, you need to have 'Docker Desktop' installed. Open 'Windows PowerShell' in the root folder of the application and execute the command below:

bash 

docker-compose up -d