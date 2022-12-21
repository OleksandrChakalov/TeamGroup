# **TeamGroup**

## **Table of Contents**

- [Team](#team)
- [Description](#project-description)
- [Architecture](#architecture)
- [Data Model](#data)


## **Team**

- Chakalov Oleksandr
- Shevyak Ivan
- Dmytro Hnatyshyn
- Anton Vyhovanets

## **Project Description**

### **Go & See**

This web application is a trip planner which can make organizing your trips easier than it was before. With the assistance of our service user has no need to write down long lists of different items before his journey. All these items will be in one place - our planner, and the only thing that traveler should do before his vacation is to check if he doesn’t forget anything from the list that suggests the Go & See service.

**Use case diagram**


<img src="./Docs/use-case.png">

<br/>
<br/>

**Guest sequence diagram**


<img src="./Docs/guest-sequence.png">

<br/>
<br/>

**User sequence diagram**


<img src="./Docs/user-sequence.png">

<br/>
<br/>

## **Architecture**

| Part of project | Description                                               | Technologies                  |
| --------------- | --------------------------------------------------------- | ----------------------------- |
| Back end        | API based on CQRS                                         | .NET 5, ASP.Net Core          |
| Fron end        | SPA                                                       | React, Type Script, AntDesign |
| DB              | SQL database for user management and NoSQL for user trips | Azure SQL Database, MongoDB   |

<br/>

<img src="./Docs/architecture.png">

<br/>
<br/>

Example of _Trip_ document for MongoDB:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "status": 1,
  "userId": "00000000-0000-0000-0000-000000000000",
  "name": "string",
  "description": "string",
  "startDate": "2021-10-23T17:36:04.432Z",
  "endDate": "2021-10-23T17:36:04.432Z",
  "itemsToTake": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "name": "string",
      "isTaken": true
    }
  ],
  "toDoNodes": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "name": "string",
      "description": "string",
      "type": 1,
      "date": "2021-10-23T17:36:04.432Z",
      "status": 1
    }
  ]
}
```

Example of _TripTemplate_ document for MongoDB:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "string",
  "itemsToTake": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "name": "string",
      "isTaken": true
    }
  ],
  "toDoNodes": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "name": "string",
      "description": "string",
      "type": 1,
      "date": "2021-10-23T17:36:04.432Z",
      "status": 1
    }
  ]
}
```

# Data model


# Resiliency model


# Security model
1. HTTP protocol is used cominicatig with API, so secure info can be stollen by adversary.
2. Secure user info can be stollen by sniffing traffic on log in, sign up or password recovery stages.
3. UI can be changed on the fly by JS injection attack and secure info user may be stollen. 
4. Access to the API may be blocked by denial of service attack.
5. Access to the SQL database may be blocked by denial of service attack.
6. Access to the MongoDB service can be blocked by denial of service attack.
7. Access to the web UI may be blocked through denial to service attack.
8. Access to SQL database with user info can be obtained by sniffing trafic.
9. Access to the unencypted MongoDB data can be obtained by elevating user privileges.
10. Access to the databases (SQL or MongoDB) can be obtained due to lack of the network protection.
11. An adversary can spam through browser automation with account creation.



# Deployment model


# Analics model


# Monitoring
| Metrics | Way to collect | Mitigation plan |
| ------- | -------------- | --------------- |
| Quantity of active users | Server side script | Advertisement |
| Interaction with login page | Pixel or Google Analitics on the UI side | UI changes , A/B testing |
| Number of discarded trips per user | Server side script | - |
| Time spent on trip creation | Frontend script | Updating UX to reduce time |
| Time spent with sign up procedure | Frontend script | Updating UX to reduce time |



