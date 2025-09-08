import React from 'react';
import Layout from '@theme/Layout';
import Heading from '@theme/Heading';

export default function Architecture(): JSX.Element {
  return (
    <Layout
      title="System Architecture"
      description="RealEstate API system architecture documentation">
      <div className="container margin-vert--lg">
        <div className="row">
          <div className="col col--8 col--offset-2">
            <Heading as="h1">System Architecture</Heading>
            
            <p>
              The RealEstate API follows Clean Architecture principles with a clear separation of concerns.
              This page provides comprehensive documentation of the system's architectural design.
            </p>

            <Heading as="h2">Clean Architecture Overview</Heading>
            
            <div className="mermaid">
              {`graph TB
                subgraph "External"
                  UI[Web UI]
                  DB[(SQL Server)]
                  EXT[External APIs]
                end
                
                subgraph "API Layer"
                  CTRL[Controllers]
                  MW[Middleware]
                  AUTH[Authentication]
                end
                
                subgraph "Application Layer"
                  CMD[Command Handlers]
                  QRY[Query Handlers]
                  DTO[DTOs]
                  VAL[Validators]
                end
                
                subgraph "Domain Layer"
                  ENT[Entities]
                  AGG[Aggregates]
                  DS[Domain Services]
                  INTF[Interfaces]
                end
                
                subgraph "Infrastructure Layer"
                  REPO[Repositories]
                  EF[Entity Framework]
                  CFG[Configurations]
                end
                
                UI --> CTRL
                CTRL --> CMD
                CTRL --> QRY
                CMD --> ENT
                QRY --> ENT
                CMD --> REPO
                QRY --> REPO
                REPO --> EF
                EF --> DB
                
                CTRL -.-> MW
                MW -.-> AUTH`}
            </div>

            <Heading as="h2">C4 Model Diagrams</Heading>
            <p>
              <em>C4 model diagrams will be added here to show different levels of system architecture.</em>
            </p>

            <Heading as="h3">Level 1: System Context</Heading>
            <div className="mermaid">
              {`graph TB
                subgraph "RealEstate System Context"
                  USER[Property Manager]
                  API[RealEstate API System]
                  DB[(SQL Server Database)]
                  EXT[External Property Services]
                end
                
                USER -->|Manages Properties| API
                API -->|Stores Data| DB
                API -->|Integrates With| EXT`}
            </div>

            <Heading as="h3">Level 2: Container Diagram</Heading>
            <p><em>Container diagram placeholder - to be populated with actual system containers.</em></p>

            <Heading as="h2">Domain Model</Heading>
            
            <div className="mermaid">
              {`erDiagram
                Property ||--o{ PropertyImage : contains
                Property ||--o{ PropertyTrace : has
                Property }|--|| Owner : "owned by"
                
                Property {
                    int Id PK
                    string Name
                    string Address
                    decimal BasePrice
                    decimal TaxAmount
                    decimal Price
                    int Year
                    string Status
                    datetime CreatedAt
                    datetime UpdatedAt
                    datetime DeletedAt
                    byte RowVersion
                    int OwnerId FK
                }
                
                Owner {
                    int Id PK
                    string Name
                    string Address
                    string Phone
                    string Email
                    datetime CreatedAt
                    datetime UpdatedAt
                    datetime DeletedAt
                    byte RowVersion
                }
                
                PropertyImage {
                    int Id PK
                    string FileName
                    string FilePath
                    bool IsPrimary
                    int PropertyId FK
                }
                
                PropertyTrace {
                    int Id PK
                    string Action
                    decimal OldValue
                    decimal NewValue
                    datetime CreatedAt
                    int PropertyId FK
                }`}
            </div>

            <Heading as="h2">Key Architectural Decisions</Heading>
            
            <div className="api-endpoint">
              <strong>Clean Architecture Implementation</strong>
              <ul>
                <li>Domain layer contains business logic and entities</li>
                <li>Application layer orchestrates use cases</li>
                <li>Infrastructure layer handles external concerns</li>
                <li>API layer manages HTTP concerns</li>
              </ul>
            </div>

            <div className="api-endpoint">
              <strong>CQRS Light Pattern</strong>
              <ul>
                <li>Separate command and query handlers</li>
                <li>No event sourcing (CQRS Light)</li>
                <li>Direct dependency injection without MediatR</li>
              </ul>
            </div>

            <div className="api-endpoint">
              <strong>Domain-Driven Design</strong>
              <ul>
                <li>Rich domain models with private setters</li>
                <li>Factory methods for entity creation</li>
                <li>Aggregate roots and bounded contexts</li>
              </ul>
            </div>

            <p>
              For more detailed architectural decisions, see our <a href="/adrs">Architecture Decision Records</a>.
            </p>
          </div>
        </div>
      </div>
    </Layout>
  );
}