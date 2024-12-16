# Technical Assessment

This project contains a simple REST API which serves as a basic user profile service. Data
is persisted on two collections in MongoDB: "Accounts" and "Profiles." Accounts contain data
required to authenticate the user. Profiles contain data required by a rudimentary  personal
profile system (i.e. personally identifying information). The API is protected with a basic
JWT authentication scheme using a private key.

The API is divided into two different sections. The "user" section provides an interface for a
user to manage their own account and profile. The relevant account is retrieved by the auth
details in the JWT. For example, they can change the username, email, and password for their
account. They may also change the name, phone number, and address associated with their profile.
When a user changes the email associated with their "account" (used for authentication), the
API automatically updates the email associated with their profile.

The "admin" section provides an interface for an administrator to manage accounts and profiles
for any user. They can only get or delete a user account, however they can change certain details
about a user's profile.

**User interface**
```
/account [POST, GET, PATCH, DELETE]
    ../password [PUT]
    ../token [POST]
/profile [POST, GET, PATCH, DELETE]
```

**Administrative interface**
```
/admin
    ../accounts [GET]
    ../accounts/<userId> [GET, DELETE]
    ../profiles [GET, PATCH]
    ../profiles/<userId> [GET, DELETE]
```

Tests are included in the Assessment.Tests project. Due to the shortened time constraint, this
assessment does not have full test coverage. The project does have a relatively full set of tests
for both the controller and endpoints of the user authentication flow. It also has a relatively
full set of tests for the user account management endpoints. Finally, it has a limited set of
"in-progress" tests for the account management controllers.