# GarageProject
Asp .Net Core API for garage management at Geometria Kft.

A complete list of endpoints and their requirements:
authentication cookie is in effect: send credentials with all fetches

/access
(all return IActionResult)
POST /registration: UserDTO in body
POST /login: base64 coded in authorization header, email and password separated by ":"
POST /logout
(delete is available in userController)

/user

GET : gets all users (returns List<UserDTO>?)
GET /{id}: gets user by id (returns UserDTO?)
GET /email/{email}: gets user by email (returns UserDTO?)
GET /allmanagers: gets all managers (retuns List<UserDTO>)
PUT /{id}: updates user (UserDTO in body, returns IActionResult)
DELETE /{id}: deletes user (returns IActionResult)

/spaces

GET : gets all parking spaces (returns List<ParkingSpace>?)
GET /{id}: gets parking spaces by id (returns ParkingSpace)
GET /ids: gets parking spaces by list of ids (List<long> ids in body, returns List<ParkingSpace>?)
POST : adds parking space (ParkingSpace in body, returns IActionResult)
PUT /{id}: updates parking space (ParkingSpace in body, returns IActionResult)
DELETE /{id}: deletes parking space (returns IActionResult)

/booking

GET : gets all bookings (returns List<BookingDTO>?)
GET /{id}: gets booking by id (returns BookingDTO?)
GET /user/{userId}: gets bookings by user (returns List<BookingDTO>?)
GET /dates: gets bookings by dates (query: startDate, endDate, returns List<BookingDTO>?) --> date format: 2024-01-26-15-27-00
GET /user/dates: gets bookings by dates and userId (query: userId, startDate, endDate, returns List<BookingDTO>?) --> date format: 2024-01-26-15-27-00 ("YYYY\\-MM\\-dd\\HH\\-mm\\-ss")
GET /ids: gets bookings by list of ids (returns List<BookingDTO>?)
GET /emptyspaces/date/{date}: gets list of available parking spaces for date (returns List<ParkingSpace>?, options: "today", "tomorrow", date format ("2024-01-17"))
GET /emptyspaces/daterange: gets list of available parking spaces for time range (query: startDate, endDate, returns IEnumerable<ParkingSpace>?)
GET /emptyspaces/amount/{date}: gets number of empty spaces on a given date (returns int, options: "today", "tomorrow", date format ("2024-01-17"))
POST : adds booking (BookingDTO in body in which ParkingSpace is optional (null or object of ParkingSpace type), returns IActionResult)
PUT /removeday/{id}/{date}: removes date from booking (returns IActionResult, date format ("2024-01-17"))
PUT /{id}: updates booking (BookingDTO in body, returns IActionResult)
DELETE /{id}: deletes booking (returns IActionResult)


DTO classes:
//Id should not be provided on front-end, ParkingSpace is optional
public class BookingDTO
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public ParkingSpace? ParkingSpace { get; set; }
    public string Start { get; set; }
    public string End { get; set; }
}

//Type means UserType (enum) with 2 current options: "Manager" or "User"
public class UserDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string DateOfBirth { get; set; }
    public string Password { get; set; }
}

//sending empty object or null enough, since Id is provided by database and IsDeleted is automatically false
public class ParkingSpace
{
   public long Id { get; set; }
   public bool IsDeleted { get; set; } = false;
}
