# Concert Ticket Management System
Core Requirements: Create a .NET Web API for managing concert tickets with these
Core features:
- Event Management
- Ticket Reservations and Sales
- Venue Capacity Management

## Domain Features:

### Events
- Create/update concert events
- Set ticket types and pricing
- Manage available capacity
- Basic event details (date, venue, description)

### Tickets
- Reserve tickets for a time window
- Purchase tickets (Can assume there is already Payment Processing System in
place which you can leverage)
- Cancel reservations
- View ticket availability

## Design Decisions
This task is fairly open-ended. As such, I have made some decisions in the implementation as summerized below:

- I decided to use a dictionary as an in-memory database. Pros: include simplicity and fit for purpose. Cons: well, it's not a database.
- I used application level services to separate the controllers from the database.
- Although the operations are actually synchronous in nature (due to the db being an in-memory dictionary), I have opted to use Async method accross the board with CancellationToken to demonstrate its use in an actual distributed system. 
- I used dependecy injection where needed.
- I have used ApiResponse<T> class. This serve as a standard way the API wrap responses (where applicable)
- I have used DTO where applicable and hidden the business model.
- I used AutoMapper for, well, mapping betwwen a ConcertDTO and a Concert
- Added some Fluent validation 
- Implemented Ticket Reservations. Used min heap to manage time-based reservation in O(log n) time.
- This code probbaly has some bugs since I didn't get to add unit tests (I cannot do it all in 3 days)
