# WeatherMachine
WKBW Weather Machine Data Tracker

Author: Mike Dearman


# Local Testing while Azure Function CLI running
	curl --request POST --data '{"input":"sample trigger"}' http://localhost:7071/admin/functions/ReadVotes

	
# Resources

* https://codehollow.com/2017/02/azure-functions-time-trigger-cron-cheat-sheet/
* https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local
* https://docs.microsoft.com/en-us/azure/azure-functions/functions-reference-csharp#writing-multiple-output-values
* https://blogs.endjin.com/2015/04/visualise-your-azure-table-storage-data-with-power-bi/