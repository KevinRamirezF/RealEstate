import React from 'react';
import Layout from '@theme/Layout';
import Heading from '@theme/Heading';

export default function Database(): JSX.Element {
  return (
    <Layout
      title="Database Schema"
      description="Database schema documentation for RealEstate API">
      <div className="container margin-vert--lg">
        <div className="row">
          <div className="col col--10 col--offset-1">
            <Heading as="h1">Database Schema</Heading>
            
            <p>
              The RealEstate API uses SQL Server with Entity Framework Core. This page documents 
              the database schema, relationships, and key design decisions.
            </p>

            <Heading as="h2">Entity Relationship Diagram</Heading>
            
            <div className="mermaid">
              {`erDiagram
                Owner ||--o{ Property : owns
                Property ||--o{ PropertyImage : has
                Property ||--o{ PropertyTrace : tracks
                
                Owner {
                    int Id PK "Identity, Primary Key"
                    nvarchar_100 Name "Owner full name"
                    nvarchar_255 Address "Owner address"
                    nvarchar_20 Phone "Contact phone number"
                    nvarchar_100 Email "Email address"
                    datetime2 CreatedAt "Record creation timestamp"
                    datetime2 UpdatedAt "Last update timestamp"
                    datetime2 DeletedAt "Soft delete timestamp (nullable)"
                    rowversion RowVersion "Concurrency token"
                }
                
                Property {
                    int Id PK "Identity, Primary Key"
                    nvarchar_100 Name "Property name/title"
                    nvarchar_255 Address "Property physical address"
                    decimal_18_2 BasePrice "Base property price"
                    decimal_18_2 TaxAmount "Tax amount"
                    decimal_18_2 Price "Computed: BasePrice + TaxAmount"
                    int Year "Construction year"
                    nvarchar_50 Status "Property status (Active, Inactive, etc)"
                    datetime2 CreatedAt "Record creation timestamp"
                    datetime2 UpdatedAt "Last update timestamp"
                    datetime2 DeletedAt "Soft delete timestamp (nullable)"
                    rowversion RowVersion "Concurrency token"
                    int OwnerId FK "Foreign key to Owner"
                }
                
                PropertyImage {
                    int Id PK "Identity, Primary Key"
                    nvarchar_255 FileName "Original file name"
                    nvarchar_500 FilePath "Storage path/URL"
                    bit IsPrimary "Primary image flag"
                    datetime2 CreatedAt "Upload timestamp"
                    int PropertyId FK "Foreign key to Property"
                }
                
                PropertyTrace {
                    int Id PK "Identity, Primary Key"
                    nvarchar_100 Action "Action performed (PriceChange, StatusUpdate, etc)"
                    decimal_18_2 OldValue "Previous value (nullable)"
                    decimal_18_2 NewValue "New value (nullable)"
                    nvarchar_500 Description "Additional details"
                    datetime2 CreatedAt "Action timestamp"
                    int PropertyId FK "Foreign key to Property"
                    int UserId FK "User who performed action (nullable)"
                }`}
            </div>

            <Heading as="h2">Key Database Features</Heading>

            <div className="row">
              <div className="col col--6">
                <div className="api-endpoint">
                  <h4>🗑️ Soft Delete Pattern</h4>
                  <ul>
                    <li>All main entities have <code>DeletedAt</code> timestamp</li>
                    <li>Global query filters exclude soft-deleted records</li>
                    <li>Maintains data integrity and audit trail</li>
                    <li>Allows data recovery when needed</li>
                  </ul>
                </div>

                <div className="api-endpoint">
                  <h4>🔄 Optimistic Concurrency</h4>
                  <ul>
                    <li>All entities include <code>RowVersion</code> field</li>
                    <li>Prevents lost update problems</li>
                    <li>Automatic concurrency conflict detection</li>
                    <li>Essential for multi-user scenarios</li>
                  </ul>
                </div>
              </div>

              <div className="col col--6">
                <div className="api-endpoint">
                  <h4>📊 Audit Tracking</h4>
                  <ul>
                    <li><code>CreatedAt</code> and <code>UpdatedAt</code> timestamps</li>
                    <li>PropertyTrace for detailed change history</li>
                    <li>Tracks price changes and status updates</li>
                    <li>User attribution for audit trail</li>
                  </ul>
                </div>

                <div className="api-endpoint">
                  <h4>💰 Price Calculation</h4>
                  <ul>
                    <li><code>Price = BasePrice + TaxAmount</code></li>
                    <li>Computed property for total price</li>
                    <li>Separate tax tracking for reporting</li>
                    <li>Decimal precision for currency values</li>
                  </ul>
                </div>
              </div>
            </div>

            <Heading as="h2">Indexes and Performance</Heading>
            
            <div className="api-endpoint">
              <h4>🔍 Database Indexes</h4>
              <ul>
                <li><strong>Property.OwnerId:</strong> Foreign key index for owner lookups</li>
                <li><strong>Property.DeletedAt:</strong> Supports soft delete filtering</li>
                <li><strong>PropertyTrace.PropertyId:</strong> Efficient trace queries</li>
                <li><strong>PropertyTrace.CreatedAt:</strong> Time-based trace queries</li>
                <li><strong>Owner.Email:</strong> Unique constraint and lookup optimization</li>
              </ul>
            </div>

            <Heading as="h2">Data Constraints and Rules</Heading>

            <table>
              <thead>
                <tr>
                  <th>Table</th>
                  <th>Field</th>
                  <th>Constraint</th>
                  <th>Description</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>Owner</td>
                  <td>Email</td>
                  <td>Unique, Not Null</td>
                  <td>Each owner must have unique email</td>
                </tr>
                <tr>
                  <td>Property</td>
                  <td>BasePrice</td>
                  <td>CHECK ≥ 0</td>
                  <td>Price cannot be negative</td>
                </tr>
                <tr>
                  <td>Property</td>
                  <td>TaxAmount</td>
                  <td>CHECK ≥ 0</td>
                  <td>Tax amount cannot be negative</td>
                </tr>
                <tr>
                  <td>Property</td>
                  <td>Year</td>
                  <td>CHECK 1800-2100</td>
                  <td>Reasonable year range</td>
                </tr>
                <tr>
                  <td>PropertyImage</td>
                  <td>IsPrimary</td>
                  <td>Max 1 per Property</td>
                  <td>Only one primary image per property</td>
                </tr>
              </tbody>
            </table>

            <Heading as="h2">SchemaSpy Integration</Heading>
            
            <div style={{ padding: '1rem', background: 'var(--ifm-color-info-contrast-background)', borderRadius: '4px', border: '1px solid var(--ifm-color-info)', marginTop: '2rem' }}>
              <h4>📈 SchemaSpy Documentation</h4>
              <p>
                SchemaSpy will be integrated to provide interactive database documentation including:
              </p>
              <ul>
                <li>Visual relationship diagrams</li>
                <li>Table and column details</li>
                <li>Constraint documentation</li>
                <li>Index analysis</li>
                <li>Foreign key relationships</li>
              </ul>
              <p>
                <strong>Coming Soon:</strong> SchemaSpy-generated documentation will be available at 
                <code>/database/schemaSpy/</code> once configured.
              </p>
            </div>

            <Heading as="h2">Migration Commands</Heading>
            
            <div className="api-endpoint">
              <h4>Entity Framework Core Migrations</h4>
              <div style={{ fontFamily: 'monospace', background: 'var(--ifm-code-background)', padding: '1rem', borderRadius: '4px' }}>
                {`# Add new migration
dotnet ef migrations add MigrationName --project RealEstate.Infrastructure --startup-project RealEstate.API

# Update database
dotnet ef database update --project RealEstate.Infrastructure --startup-project RealEstate.API

# Generate SQL script
dotnet ef migrations script --project RealEstate.Infrastructure --startup-project RealEstate.API`}
              </div>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
}