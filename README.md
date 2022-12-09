# General Flow
0. Tests are conducted prior to the start of the containers. This ensures the API is online and the format of the JSON obtained is unchanged.
1. Data is retrieved from [Data.gov.sg](https://data.gov.sg/dataset/weather-forecast)
2. A portion of the entire JSON is extracted, transformed and rewritten into a new JSON with only 4 fields: Area, Forecast, Start Time and End Time.
3. The saved JSON will be located in `/data/pull/` (volume mount point) and saved as dd-MM-yyyy.json, e.g. `/data/pull/01-11-2022.json`
4. This JSON will be retrieved from `/data/push/` (volume mount point) and deserialised into WeatherRecord(s) to be pushed into a SQL Server.

# How the containers will fit together
There will be 4 containers:

A. `test` tests to see if API is online, and if the API returns a JSON in a format which can be deserialised into WeatherRecords.

B. `pull` will produce a [JSON](https://github.com/vms3-demo-purpose/weather-program/files/9934735/01-11-2022.json.txt)
to be saved in a volume. (Steps 1 - 3)

C. `push` will retrieve the JSON from the volume and push it to a container running MSSQL. (Step 4)

D. `save` will store data in the following [schema](https://github.com/vms3-demo-purpose/weather-program/files/10191628/CREATE_TABLE.sql.txt)
 based on data from the JSON. (Step 4)

Another front-end will be connecting to `save` to retrieve weather records to be visually displayed. 

# Running the container
0. Clone the repository. Open PowerShell (preferably with administrator rights), navigate to the `/weather-program/` directory and run the following command: `docker volume create weather` This is the volume where data will be persisted. 

1. Rename `secret.json.example` to `secret.json`. Input the password accordingly.

2. Start up all the containers with: `docker compose --profile all up -d`.

# Verifying that each container is running properly:

* To check `test` is successful, run: `docker logs test`.  The output should be something like:

  `Passed! - Failed: 0, Passed: 2, Skipped: 0, Total: 2, Duration: 1s`

* To check if `pull` is successful, run: `docker logs pull`. The output should be something like:

  `Pulled 1234 weather records for date: 01-01-1970.`

* To check if `push` is successful, run: `docker logs push`. The output should be a looong list of weather records with ID, Area, Forecast, StartTime and EndTime.  
       
  Note that the ID of the last weather record displayed here should tally with the number of weather records pulled by `pull`.

* To double check if `save` is successful, run the following commands:

  `docker exec -ti save bash`

  `/opt/mssql-tools/bin/sqlcmd -U SA`

  `<INPUT PASSWORD>`

  `SELECT * FROM WeatherRecords;`

  `GO`

  The above string of commands should also output a loooong list of weather records (but not formatted nicely). 
       
  Again, the ID of the last weather record displayed here should tally with the number of weather records pulled by `pull`.
  
  The containers `test`, `pull` and `push` exits upon completion of their tasks. To re-sync data from [Data.gov.sg](https://data.gov.sg/dataset/weather-forecast) you can restart just these three containers with `docker compose --profile seed up --build -d`.
  
  All containers can be terminated with the command `docker compose --profile all down`.

# Versions of Framework and Libraries used:
1. docker-compose: 3.1
2. SQL Server: 2019-CU18-ubuntu-20.04
3. .NET: 6.0.402

# Troubleshooting
1. Error response from daemon: Ports are not available ... An attempt was made to access a socket in a way forbidden by its access permissions.

> Restart the Host Network Service with the following commands:   `net stop hns` `net start hns`

2. 'A connection was successfully established with the server, but then an error occurred during the pre-login handshake.' 

> Just wait. Logic has been implemented to restart `push` so just give it a minute and it will start pushing data into `save`.

3. Login failed for user 'SA'

> You probably got the password wrong.

# Weather-API vs weather-program:
Weather-API passes JSON to DB, DB performs queries for data insertion

weather-program deserialises JSON and uses EF Core to insert data into DB
