# Escape-Room

The QU IPE Escape Room is a first-person escape room experience designed to introduce students to the responsibilities and roles of medical professionals. Explore the room with a team of 4-5 other students, finding clues and solving puzzles. Can you determine the mysterious patient’s identity and contact his caretaker?

This was done by Matthew Merritt, Michael Merritt, and Nate Pippin with the guidance of Professor Warren from Game Design and Development and Professor McCave from Social Work. 

## Installation

For playing the game, download the latest build from [Itch.io](https://scarfier.itch.io/qu-ipe-escape-room).

For development, the process will involve the following steps:
- Clone the repository (this can be done with GitHub Desktop or the Git CLI).
- Set up the database cluster (described in `Database Setup`).
- Add the database connection secret (described under `Unity Project Setup`).
- Optional: Build the project (requires the credentials described in `Requirements` and later confirmed in `Unity Project Settings`).

### Requirements

Requires Unity v.2022.3.7f1 and access to Unity Services. The game cannot be built properly if your Unity instance is not connected to the proper Unity Developer account. Contact Professor Warren regarding the Unity Developer credentials. This also assumes that you have a MongoDB account and can create a database cluster. 

### Database Setup

- Once signed in to [MongoDB](https://account.mongodb.com/account/login), create a new organization, and select `MongoDB Atlas` as the Cloud Service.
- Add any existing members by email address and select the required permissions. This is possible later as well.
- For `Require IP Access List for the Atlas Administration API`, ensure that this is disabled.
- Create the organization, and then begin creating a new project.
    - Ensure that the proper users are still added to this project.
- On the project overview page, create a database cluster.
    - Select the free M0 option for learning MongoDB.
    - All the other options are fine as defaults.
- On the screen to connect to your cluster, you will need to create a database user. Before creating the database user, **save these credentials.**
    - When choosing a connection method, select `Drivers`, then `C# / .NET` version `2.13 or later`.
    - Copy the full connection string with password, and store this with the database credentials. **This is the last chance to get these credentials.**
- Under `SECURITY`, select `Network Access`.
    - Add an IP address and select `ALLOW ACCESS FROM ANYWHERE`. This should automatically populate the address `0.0.0.0/0` as the IP address.
- Under `DEPLOYMENT`, select `Database` and the cluster created earlier should appear. Select `Browse Collections`.
    - Select `Add My Own Data` to create a collection and database.
        - Call the database `results`, and the collection `escapes`.

### Unity Project Setup

- Upon cloning and opening the project in the Unity Editor, ensure that the proper Unity Services account is connected. This can be checked by going to `Project Settings` under the `Services` category. The `Project Name` should be `Escape-Room`.
- In the Unity Editor, add the database connection string generated earlier to the project. In order to prevent this login from accidentally making its way into the Git repo, add the ignored `Secrets` folder to the project.
    - Create a `Secrets` folder in `Assets/Resources/`.
    - Right click, select `Create` and add a new `Secret` called `Database Connection`. The content of this secret should be the entire connection string generated during database setup.