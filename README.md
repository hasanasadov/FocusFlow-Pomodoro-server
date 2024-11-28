# Todo Lists and our project requirements
- [X] Initial Project Setup
---
### First stage
---
- [X] Create Login and Register Service
- [X] Create Token Service and Email Service
- [X] Create Auth API				
- [X] Create their exceptions and write exception handlers	
- [ ] Create Loging and Validation Behaviors MediatR and explore 		
      what is the MediatR notifications then implement them
---
### Second Stage
---
- [X] Create UserTask Entity 
- [X] Create TaskStep Entity
- [X] Create Group Entity
- [X] Create GroupRole Entity
- [X] Create GroupMembership Entity
- [X] Create Project Entity
- [X] Create ProjectTask Entity

- [X] Craete TaskPriority Enum

- [X] Create UserTask Controller
- [X] Create TaskStep Controller
- [X] Create Group Controller
- [X] Create GroupRole Controller
- [X] Create GroupMembership Controller
- [X] Create Project Controller
- [X] Create ProjectTask Controller

- [X] Create Postman Collection
---
### Third Stage
---
- [X] Create Result Response
- [X] Implement Result Response to Services
- [X] Use CQRS Pattern for all services
- [ ] Grafana integrated for monitoring
- [ ] Logging with Serilog
---
### Fourth Stage
---
- [X] Redis cache implementation
- [X] SignalR
- [ ] Added CICD using GitHub actions
- [X] Dockerize the application
- [ ] RefreshToken Implementation
- [X] Using redis for stroing the roles and permissions
- [X] Role and Permission based Authorization
---
### Fifth Stage
---
- [X] Redis Role Based Authorization
- [X] Domain events for update cache
- [X] Use MediatR for publish event
- [ ] Outbox pattern for event publishing
- [ ] Quartz for scheduling tasks
### Sixth Stage
---
- [ ] Unit Testing
- [ ] Integration Testing
- [ ] Load Testing
- [ ] Stress Testing
- [ ] Security Testing
- [ ] Performance Testing
---
## Tools and Technologies
- .NET 8
- Clean Architecture
- Microsoft Identity for Authentication and Authorization
- Entity Framework Core
- MediatR
- Gloabl Exception Handling
- Redis Cache
- Result Response
- CQRS Pattern
- SignalR
- Outbox Pattern
- Scheduled Tasks
- Quartz
- Domain Events

---
## Domain
- User
- UserTask
- TaskStep
- Group
- GroupRole
- GroupMembership
- Project
- ProjectTask