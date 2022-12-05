# Weather-API vs weather-program:
Weather-API passes JSON to DB, DB performs queries for data insertion

weather-program deserialises JSON and uses EF Core to insert data into DB

# General Flow
0. Tests are conducted prior to the start of the containers. This ensures the API is online and the format of the JSON obtained is unchanged.
1. Data is retrieved from [https://api.data.gov.sg](https://api.data.gov.sg/v1/environment/2-hour-weather-forecast?date=2022-11-01)
2. A portion of the entire JSON is extracted, transformed and rewritten into a new JSON with only 4 fields: Area, Forecast, Start Time and End Time.
3. The saved JSON will be located in `/data/pull/` (volume mount point) and saved as dd-MM-yyyy.json, e.g. `/data/pull/01-11-2022.json`
4. This JSON will be retrieved from `/data/push/` (volume mount point) and deserialised into WeatherRecord(s) to be pushed into a SQL Server.

# How the containers will fit together
There will be 4 containers:

A. `weather-unit-test` tests to see if API is online, and if the API returns a JSON in a format which can be deserialised into WeatherRecords.

B. `weather-pull-data` will produce a [JSON](https://github.com/vms3-demo-purpose/weather-program/files/9934735/01-11-2022.json.txt)
to be saved in a volume. (Steps 1 - 3)

C. `weather-push-data` will retrieve the JSON from the volume and push it to a container running MSSQL. (Step 4)

D. `weather-save-data` will store data in the following [schema](https://github.com/vms3-demo-purpose/weather-program/files/9934736/CREATE_TABLE.sql.txt) based on data from the JSON. (Step 4)

Another front-end will be connecting to `weather-save-data` to retrieve weather records to be visually displayed. 

# Running the container
Clone the repository. Open PowerShell (preferably with administrator rights), navigate to the `/weather-program/` directory and run the following command:

`docker compose up --build -d`

# Verifying that each container is running properly:

* To check `weather-unit-test` is successful, run:

&emsp;&emsp;`docker logs --follow weather-unit-test`

&emsp;&emsp;The output should be something like:

&emsp;&emsp;`Passed! - Failed: 0, Passed: 2, Skipped: 0, Total: 2, Duration: 1s`

&emsp;&emsp;Press `Ctrl + C` to stop following the logs.

* To check if `weather-pull-data` is successful, run: 

&emsp;&emsp;`docker logs --follow weather-pull-data`

&emsp;&emsp;The output should be something like:

&emsp;&emsp;`Pulled 1234 weather records for date: 01-01-1970.`

&emsp;&emsp;Press `Ctrl + C` to stop 'following' the logs. Note the number of weather records pulled here.

* To check if `weather-push-data` is successful, run:

&emsp;&emsp;`docker logs --follow weather-push-data`

&emsp;&emsp;The output should be a looong list of weather records with ID, Area, Forecast, StartTime and EndTime. 
       
&emsp;&emsp;Again, press `Ctrl + C` to stop 'following' the logs. 
       
&emsp;&emsp;Note that the ID of the last weather record displayed here should tally with the number of weather records pulled by 

&emsp;&emsp;`weather-pull-data`.

&emsp;&emsp;Press `Ctrl + C` to stop following the logs.

* To double check if `weather-save-data` is successful, run the following commands:

&emsp;&emsp;`docker exec -ti weather-save-data bash`

&emsp;&emsp;`/opt/mssql-tools/bin/sqlcmd -U SA`

&emsp;&emsp;`<INPUT PASSWORD>`

&emsp;&emsp;`SELECT * FROM WeatherRecords;`

&emsp;&emsp;`GO`

&emsp;&emsp;The above string of commands should also output a loooong list of weather records (but not formatted nicely). 
       
&emsp;&emsp;Again, the ID of the last weather record displayed here should tally with the number of weather records pulled by `weather-pull-data`.

# Versions of Framework and Libraries used:
1. docker-compose: 3.1
2. SQL Server: 2019-CU18-ubuntu-20.04
3. .NET: 6.0.402

# Troubleshooting
Q1. Error response from daemon: Ports are not available ... An attempt was made to access a socket in a way forbidden by its access permissions.

A1. Restart the Host Network Service with the following commands:   `net stop hns` `net start hns`

Q2. Sometimes `weather-push-data` cannot establish connection to `weather-save-data` as the database has not finished initialisation. 

A2. Just wait. Logic has been implemented to restart `weather-push-data` so just give it a minute and it will start pushing data into `weather-save-data`.

