import React from 'react';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';
import Heading from '@theme/Heading';

interface ADR {
  id: string;
  title: string;
  status: 'proposed' | 'accepted' | 'deprecated' | 'superseded';
  date: string;
  description: string;
}

const adrs: ADR[] = [
  {
    id: 'ADR-001',
    title: 'Adopt Clean Architecture Pattern',
    status: 'accepted',
    date: '2024-01-15',
    description: 'Decision to implement Clean Architecture with clear separation between Domain, Application, Infrastructure, and API layers.'
  },
  {
    id: 'ADR-002', 
    title: 'Use Entity Framework Core with Code-First Approach',
    status: 'accepted',
    date: '2024-01-20',
    description: 'Adopt EF Core with Code-First migrations for database management and ORM functionality.'
  },
  {
    id: 'ADR-003',
    title: 'Implement CQRS Light Pattern',
    status: 'accepted', 
    date: '2024-01-25',
    description: 'Use CQRS Light (without event sourcing) with separate command and query handlers, but no MediatR library.'
  },
  {
    id: 'ADR-004',
    title: 'JWT Authentication with Custom Implementation',
    status: 'accepted',
    date: '2024-02-01',
    description: 'Implement JWT-based authentication with custom token generation and validation logic.'
  },
  {
    id: 'ADR-005',
    title: 'FluentValidation for Input Validation',
    status: 'accepted',
    date: '2024-02-05',
    description: 'Use FluentValidation library for comprehensive input validation across all API endpoints.'
  },
  {
    id: 'ADR-006',
    title: 'Soft Delete Pattern for Data Retention',
    status: 'accepted',
    date: '2024-02-10',
    description: 'Implement soft delete using DeletedAt timestamps to maintain data integrity and audit trails.'
  },
  {
    id: 'ADR-007',
    title: 'NUnit with FluentAssertions for Testing',
    status: 'accepted',
    date: '2024-02-15',
    description: 'Adopt NUnit as testing framework with FluentAssertions for more readable test assertions.'
  }
];

function ADRCard({ adr }: { adr: ADR }) {
  const statusClass = `adr-status ${adr.status}`;
  
  return (
    <div className="card margin-bottom--md">
      <div className="card__header">
        <div className="adr-header">
          <h3>{adr.id}: {adr.title}</h3>
          <div>
            <span className={statusClass}>{adr.status}</span>
            <span style={{ marginLeft: '1rem', color: 'var(--ifm-color-emphasis-600)' }}>
              {adr.date}
            </span>
          </div>
        </div>
      </div>
      <div className="card__body">
        <p>{adr.description}</p>
      </div>
      <div className="card__footer">
        <Link 
          className="button button--primary button--sm"
          to={`/adrs/${adr.id.toLowerCase()}`}>
          Read Full ADR
        </Link>
      </div>
    </div>
  );
}

export default function ADRs(): JSX.Element {
  const acceptedADRs = adrs.filter(adr => adr.status === 'accepted');
  const proposedADRs = adrs.filter(adr => adr.status === 'proposed');
  const otherADRs = adrs.filter(adr => !['accepted', 'proposed'].includes(adr.status));

  return (
    <Layout
      title="Architecture Decision Records"
      description="Architecture Decision Records (ADRs) for the RealEstate API">
      <div className="container margin-vert--lg">
        <div className="row">
          <div className="col col--8 col--offset-2">
            <Heading as="h1">Architecture Decision Records (ADRs)</Heading>
            
            <p>
              Architecture Decision Records document the architectural decisions made during the development 
              of the RealEstate API. Each ADR captures the context, decision, and consequences of significant 
              architectural choices.
            </p>

            <div style={{ marginBottom: '2rem', padding: '1rem', background: 'var(--ifm-color-info-contrast-background)', borderRadius: '4px', border: '1px solid var(--ifm-color-info)' }}>
              <strong>📋 ADR Format:</strong>
              <ul style={{ marginBottom: 0, marginTop: '0.5rem' }}>
                <li><strong>Context:</strong> What situation led to this decision?</li>
                <li><strong>Decision:</strong> What exactly are we deciding to do?</li>
                <li><strong>Status:</strong> Proposed, Accepted, Deprecated, or Superseded</li>
                <li><strong>Consequences:</strong> What are the positive and negative outcomes?</li>
              </ul>
            </div>

            {acceptedADRs.length > 0 && (
              <div>
                <Heading as="h2">✅ Accepted Decisions</Heading>
                {acceptedADRs.map(adr => (
                  <ADRCard key={adr.id} adr={adr} />
                ))}
              </div>
            )}

            {proposedADRs.length > 0 && (
              <div>
                <Heading as="h2">🔄 Proposed Decisions</Heading>
                {proposedADRs.map(adr => (
                  <ADRCard key={adr.id} adr={adr} />
                ))}
              </div>
            )}

            {otherADRs.length > 0 && (
              <div>
                <Heading as="h2">📚 Other Decisions</Heading>
                {otherADRs.map(adr => (
                  <ADRCard key={adr.id} adr={adr} />
                ))}
              </div>
            )}

            <div style={{ marginTop: '3rem', textAlign: 'center' }}>
              <p style={{ fontStyle: 'italic', color: 'var(--ifm-color-emphasis-600)' }}>
                📝 Individual ADR documents will be created in the <code>/docs/adrs/</code> directory.
                Each ADR follows the standard format with context, decision, status, and consequences.
              </p>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
}