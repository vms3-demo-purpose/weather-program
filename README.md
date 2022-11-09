# General Flow
1. Data is retrieved from [https://api.data.gov.sg](https://api.data.gov.sg/v1/environment/2-hour-weather-forecast?date=2022-11-01)
2. A portion of the entire JSON is extracted, transformed and rewritten into a new JSON with only 4 fields: Area, Forecast, Start Time and End Time.
3. The saved JSON will be located in `/data/pull/` (volume mount point) and saved as dd-MM-yyyy.json, e.g. `/data/pull/01-11-2022.json`
4. This JSON will be retrieved from `/data/push/` (volume mount point) and deserialised into WeatherRecord(s) to be pushed into a SQL Server.

# How the containers will fit together
There will be 4 containers:

A. `weather-pull-data` will produce a [JSON](https://github.com/vms3-demo-purpose/weather-program/files/9934735/01-11-2022.json.txt)
to be saved in a volume. (Steps 1 - 3)

B. `weather-push-data` will retrieve the JSON from the volume and push it to a container running MSSQL. (Step 4)

C. `weather-save-data` will store data in the following [schema](https://github.com/vms3-demo-purpose/weather-program/files/9934736/CREATE_TABLE.sql.txt) based on data from the JSON. (Step 4)

D. `weather-show-data` will present data by reading what is stored in the database.

# Running the container
Clone the repository. Open PowerShell (preferably with administrator rights), navigate to the `/weather-program/` directory and run the following command:

`docker compose up --build --force-recreate -d`

# Verifying that each container is running properly:

To check if `weather-pull-data` is successful, run: 

`docker logs --follow weather-pull-data`

The output should be something like:

`Pulled 1234 weather records for date: 01-01-1970.`

Press `Ctrl + C` to stop 'following' the logs. Note the number of weather records pulled here.

To check if `weather-push-data` is successful, run:

`docker logs --follow weather-push-data`

The output should be a looong list of weather records with ID, Area, Forecast, StartTime and EndTime, followed by some `INFO` messages. Again, press `Ctrl + C` to stop 'following' the logs. Note that the ID of the last weather record displayed here should tally with the number of weather records pulled by `weather-pull-data`.

To double check if `weather-save-data` is successful, run the following commands:

`docker exec -ti weather-save-data bash`

`/opt/mssql-tools/bin/sqlcmd -U SA`

`<INPUT PASSWORD>`

`SELECT * FROM WeatherRecords;`

`GO`

The above string of commands should also output a loooong list of weather records (but not formatted nicely). Again, the ID of the last weather record displayed here should tally with the number of weather records pulled by `weather-pull-data`.

# Versions of Framework and Libraries used:
1. docker-compose: 3
2. SQL Server: 2019-CU18-ubuntu-20.04
3. .NET: 6.0.402

# Troubleshooting
1. Error response from daemon: Ports are not available ... An attempt was made to access a socket in a way forbidden by its access permissions.

Restart the Host Network Service with the following commands:
  
`net stop hns`
  
`net start hns`


