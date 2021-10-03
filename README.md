# FoodShop
E-commerce for a food store<br>
Cloned from [bugdaryan FoodStore](https://github.com/bugdaryan/FoodStore)<br>
Rearchitect to .Net Core 3.1<br>
Uses MSSQL database on Local Environment and MySql on Test Environment<br>
The Test Environment is deployed on AWS Beanstalk<br>
The Test Environment Database is deployed on AWS EC2 Ubuntu Instance<br>

# Cloning
`git -clone`<br>

## Requires:
* AWS extension for Visual Studio
* MSSQL Server v13
* Visual Studio (2019 preferred)
* GUI tools for MSSQL (MSSQL Server Management Studio)
* GUI tools for MySql (MySql Workbench)
  
## Running 
Open on visual studio<br>
Then Clean and Rebuild the solution <br>

# Database Set up
For local we will use MSSQL<br>
For Test environment we will use MySql<br>

## Creating Local Database
1. Create Database on MSSQL Server Management Studio or other GUI tools for MSSQL<br>
`CREATE DATABASE FoodShop.Dev`<br>
2. Update Database using Entity Framework Tools on Visual Studio Package manager console<br>
Default project : tutorial-beanstalk2<br>
`[npm]> Update-Database`<br>

## Troubleshoot Local Database Connection
Check the connection string on appsetting if correct<br>
Make sure that its reading correct appsetting or appsetting.development<br>
Check for nugget reference Entity Framework Tools is needed to run the Update-Database command<br>

# Creating Test Environment Database 
[Reference Here](https://towardsdatascience.com/running-mysql-databases-on-aws-ec2-a-tutorial-for-beginners-4301faa0c247) <br>
1. Go to AWS EC2 -> Instances and click Launch Instance<br>
2. Create an instance with the following configuration
    * Amazon Machine Image : Ubuntu Server 20
    * Instance Type : t3.micro
    * Storage : 8gb
    * Security Group : allow everywhere on MySql/Aurora port to interact on remote database and SSH port to be able to connect to the instance

3. Then Launch instance with key pair<br>
4. After few seconds your new EC2 instance is ready to use. <br>
5. To connect, right click on the instance then connect.<br>
6. Go to SSH client tab <br>
7. For windows 10 you already have SSH installed, open the cmd then go to the directory where your keypair is. <br>
8. Then copy paste the example on SSH client tab.<br>
9. Allow or type yes on the security questions.<br>

10. After you've logged in on the instance. Enter the following <br>
`sudo apt update`<br>
`sudo apt install mysql-server`<br>
This will update your instance and install mysql server.<br>
You can check the status after the installation with this command<br>
`sudo systemctl status mysql`<br>
you must see a result like `Active: active (running)`<br>

11. Now login on mysql as root and replace your root password<br>
`sudo mysql`<br>
`[mysql]> ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'your_password_here';`<br>
`[mysql]> FLUSH PRIVILEGES;`<br>

12. Now, let’s exit and log in with the root credentials.<br>
`mysql> exit`<br>
`sudo mysql -u root -p`<br>
Then type the password you just change. <br>
13. Then lets add a new user that will be used to connect remotely on our code.<br>
`[mysql]> CREATE USER 'vincent'@'localhost' IDENTIFIED BY 'vincentpassword';`<br>
`[mysql]> CREATE USER 'vincent'@'%' IDENTIFIED BY 'vincentpassword';`<br>
`[mysql]> GRANT ALL PRIVILEGES ON *.* to vincent@localhost WITH GRANT OPTION;`<br>
`[mysql]> GRANT ALL PRIVILEGES ON *.* to vincent@'%' WITH GRANT OPTION;`<br>
`[mysql]> FLUSH PRIVILEGES;`<br>
`[mysql]> EXIT;`<br>

14. Then we will change the bind address to 0.0.0.0 on the cnf file.<br>
`sudo nano /etc/mysql/mysql.conf.d/mysqld.cnf`<br>
Find the bind-address and set to 0.0.0.0 then ctrl+X to exit. Dont forget to save<br>

15. Then Restart the MySql Server<br>
`sudo /etc/init.d/mysql restart`<br>
`sudo service mysql restart`<br>

16. To check what weve done so far. lets check on the user created<br>
`SELECT user, host FROM mysql.user;`<br>
We should see the user created with both localhost and % or remote<br>

17. Test the connection on Mysql Workbench or other GUI tool for mysql<br>
Add new connection<br>
Hostname will be your EC2 instance' Public IPv4 DNS <br>
Port is 3306<br>
User is the user we created that has remote access, also the password<br>
18. if the connection succeeded create a database FoodShop<br>
`CREATE DATABASE FoodShop;`<br>
> Notice the database name on local FoodShop.Dev and on test will be FoodShop only. 

19. Then on Visual Studio Package manager console<br>
Default project : tutorial-beanstalk2<br>
`[npm]> Update-Database`<br>

## Troubleshoot Test Environment Database Connection
Check the connection string on appsetting if correct<br>
Notice that after restarting/stoping a instance will give you a new Public IPv4 DNS <br>
Make sure that its reading correct appsetting or appsetting.development<br>
Check for nugget reference Entity Framework Tools is needed to run the Update-Database command<br>
Check the security group if the MySql/Aurora port is allowed everywhere<br>
Check the conf file if the bind-address is set to 0.0.0.0<br>
Check the mysql user if its allowed to connect remotely<br>
Flush and restart the mysql server<br>
[Additional Reference Here](https://stackoverflow.com/questions/9766014/connect-to-mysql-on-amazon-ec2-from-a-remote-server)

# Deployment on AWS Elastic Beanstalk
## Configuration
* Environment Tier : Web Server Environment<br>
* Environment Name : shop-vincent-test<br>
* Domain : shop-vincent-test.ap-east-1.elasticbeanstalk.com<br>
* Platform : .Net on Windows Server<br>
* Platform Branch : IIS 10.0 running on 64bit Windows Server 2016 <br>
* Platform version : v2.7.0<br>

## Configure More options 
Instance<br>
* Root volume type : General Purpose (SSD)<br>
* Root volume size(GB) : 30gb<br>

Capacity<br>
* Auto scaling group : single instance<br>
* Instance types : t3.micro<br>

Click Create Environment and wait couple of minutes to finish <br>

After creating, on Visual Studio open AWS Explorer to check if AWS Extension is present<br>
Right Click on the project (tutorial-beanstalk2) -> publish to AWS Elastic Beanstalk<br>
Find the environment shop-vincent-test <br>
Deploy/Finish<br>

