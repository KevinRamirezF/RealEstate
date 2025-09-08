import React, { useEffect } from 'react';
import Layout from '@theme/Layout';
import Heading from '@theme/Heading';

declare global {
  namespace JSX {
    interface IntrinsicElements {
      'scalar-api-reference': any;
    }
  }
}

export default function ApiReference(): JSX.Element {
  useEffect(() => {
    // Load Scalar dynamically
    const script = document.createElement('script');
    script.type = 'module';
    script.src = 'https://cdn.jsdelivr.net/npm/@scalar/api-reference@latest';
    document.head.appendChild(script);

    return () => {
      document.head.removeChild(script);
    };
  }, []);

  return (
    <Layout
      title="API Reference"
      description="Complete API reference documentation powered by Scalar">
      <div className="container-fluid" style={{ padding: 0 }}>
        <div style={{ padding: '2rem', textAlign: 'center', background: 'var(--ifm-background-color)' }}>
          <Heading as="h1">API Reference</Heading>
          <p>
            Complete API documentation for the RealEstate API, powered by Scalar.
            This interactive documentation allows you to explore and test all available endpoints.
          </p>
        </div>
        
        <div style={{ height: '100vh', minHeight: '800px' }}>
          <scalar-api-reference
            configuration={{
              spec: {
                // This should point to your OpenAPI/Swagger JSON
                // For now, using a placeholder - you'll need to update this with actual API spec URL
                url: '/openapi.json'
              },
              theme: 'default',
              layout: 'modern',
              showSidebar: true,
              hideDownloadButton: false,
              searchHotKey: 'k',
              metaData: {
                title: 'RealEstate API Reference',
                description: 'Complete API documentation for the RealEstate management system',
                ogDescription: 'Explore and test the RealEstate API endpoints'
              }
            }}
          />
        </div>
        
        <div style={{ padding: '2rem', background: 'var(--ifm-color-emphasis-100)' }}>
          <div className="container">
            <Heading as="h2">API Information</Heading>
            <div className="row">
              <div className="col col--6">
                <div className="api-endpoint">
                  <strong>Base URL (Development)</strong>
                  <br />
                  <code>https://localhost:7001</code>
                </div>
                
                <div className="api-endpoint">
                  <strong>Authentication</strong>
                  <br />
                  JWT Bearer Token required for protected endpoints
                </div>
              </div>
              
              <div className="col col--6">
                <div className="api-endpoint">
                  <strong>Content Type</strong>
                  <br />
                  <code>application/json</code>
                </div>
                
                <div className="api-endpoint">
                  <strong>API Version</strong>
                  <br />
                  <code>v1.0</code>
                </div>
              </div>
            </div>

            <Heading as="h3">Quick Links</Heading>
            <ul>
              <li><a href="#tag/Properties">Properties API</a> - Manage real estate properties</li>
              <li><a href="#tag/Owners">Owners API</a> - Manage property owners</li>
              <li><a href="#tag/Authentication">Authentication API</a> - JWT token management</li>
            </ul>

            <div style={{ marginTop: '2rem', padding: '1rem', background: 'var(--ifm-color-warning-contrast-background)', borderRadius: '4px', border: '1px solid var(--ifm-color-warning)' }}>
              <strong>⚠️ Note:</strong> The OpenAPI specification needs to be generated and made available at <code>/openapi.json</code>. 
              Configure your API to serve the OpenAPI spec, or update the URL above to point to your actual API documentation endpoint.
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
}