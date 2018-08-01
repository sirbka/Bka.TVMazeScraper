## Background
For a new metadata ingester we need a service that provides the cast of all the tv shows in the TVMaze database, so we can enrich our metadata system with this information. The TVMaze database provides a public REST API that you can query for this data.
http://www.tvmaze.com/api
This API requires no authentication but it is rate limited, so keep that in mind.

## Assignment
We want you to create an application that:
1.	scrapes the TVMaze API for show and cast information;
2.	persists the data in storage;
3.	provides the scraped data using a REST API.
We want the REST API to satisfy the following business requirements.
1.	It should provide a paginated list of all tv shows containing the id of the TV show and a list of all the cast that are playing in that TV show.
2.	The list of the cast must be ordered by birthday descending.
The REST API should provide a JSON response when a call to a HTTP endpoint is made (it's up to you what URI).
Example response:
[
  {
    "id": 1,
	"name": "Game of Thrones",
    "cast": [
        {
          "id": 9,
          "name": "Dean Norris",
          "birthday": "1963-04-08"
        },
        {
          "id": 7,
          "name": "Mike Vogel",
          "birthday": "1979-07-17"
        }
      ]
  },
  {
    "id": 4,
	"name": "Big Bang Theory",
    "cast": [
        {
          "id": 6,
          "name": "Michael Emerson",
          "birthday": "1950-01-01"
        }
      ]
  }
]
