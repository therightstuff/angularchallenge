# angularchallenge
A challenge: me learning to work with Web API, Swagger, JWT and Angular

*NOTE: I had three hours to perform this Angular challenge, which assumed that for the backend I would use whatever programming language I'm most comfortable with.
Instead, I decided to use a variety of technologies that I've never worked with but wanted to learn: Web API, Swagger, and JWT. As a result, this took me a LOT longer
than three hours for the backend alone, mostly due to fiddling and refactoring things I'm not familiar with, and trying out a variety of packages (EntityFramework's
InMemory package in particular gave me a memorable amount of trouble with dependency conflicts until I eventually gave up)*

*ANOTHER NOTE: My apologies for including all the packages in the repo, I didn't have time to be smarter about that*

Any resemblance to code found in documentation or on StackOverflow is probably because I lifted and modified it, but with so much rewriting while I learned that
at some point I just gave up on leaving references

## Installation
* Ensure SQL server instance available with user that has permissions to create databases
* Replace connection string in Models\ChallengeContext.cs constructor

## Testing
* development testing was initially done using the swagger interface and do not have impressive coverage as i was more
focused on figuring out all the other parts; they're more functional tests than unit tests but hopefully it'll demonstrate the general idea

## Swagger
* Deploy the code to an IIS instance or run in VS debug mode, the index page has a link to the swagger UI
* use the <tt>POST /api/users</tt> to authenticate and receive a JWT token, the POST body must be application/json in the following format:
{
	"username": "admin",
	"password": "secure password admin"
}
(you can also log in with user A ("secure password A") or B ("secure password B")
* once you have received a token, copy it (without quotation marks) into the api_key field at the top of the page
* you will have five minutes to test the rest of the fields (expiration set in web.config, refresh tokens not implemented)
* there are ways to have swagger provide the correct formats but they were too time-consuming for this exercise

## Angular
I had a little more than an hour to do the frontend, then ran out of time (no unit tests, sorry). I'm really sorry I haven't made space to work with angular before, it's awesome.

Current state: Can authenticate with users (password is always "secure password &lt;username&gt;")
Once authenticated, can attempt to get all users (admin only), get all indices that the authenticated user has permission for,
and search for an index by entering the ticker in the search box

Intention: add a button to add permissions for selected indices to the selected user, possibly another endpoint for removing index permission.
